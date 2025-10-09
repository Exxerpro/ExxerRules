# UseModernPatternMatching Analyzer - False-Positive Mitigation Spec

**Analyzer ID**: `EXXER702`  
**Source**: `src/code/IndFusion.Analyzer/ModernCSharp/UseModernPatternMatchingAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-08)

## 0. Selection Rationale

- `EXXER702` remains the only Modern C# analyzer without a mitigation guide. The current implementation still relies on string-based heuristics that can misidentify safe code paths (see `RulesAnalysis.md:118-124`).
- Several IndTrace helpers (reflection utilities, result pipelines, and tuples) still use legacy `is` guards paired with casts. These are deliberate patterns that the analyzer interprets as violations:
  - `Test Project/Src/Code/Core/Application/Models/Extensions/ReflectionExtensions.cs:25-43` performs `result is not null ? (T)result : …` inside reflection wrappers. The analyzer surfaces EXXER702 even though rewriting to declaration patterns would require restructuring ternary expressions and could introduce scoping issues for reused locals.
  - `Test Project/Src/Code/Core/Application/Models/Extensions/ReflectionExtensions.cs:67-118` repeats the same guard/cast combination for property, field, and method invocation helpers to preserve reflection semantics. These files are flagged despite matching the library’s coding style.
  - `src/test/IndFusion.Analyzer.Tests/TestCases/ModernCSharpTests.cs` already contains regression coverage for EXXER702, but it does not capture these IndTrace-specific patterns, so local developers resort to suppressions.

## 1. Specification

- **Intent**  
  Suggest replacing legacy `if (obj is Type) { var value = (Type)obj; … }` constructs with modern declaration patterns (`if (obj is Type value)`), improving readability and ensuring single type checks.

- **Scope**  
  Registers `SyntaxKind.IfStatement`. For each `if` whose condition is an `is` expression (`BinaryExpressionSyntax`) without a declaration pattern, it scans the statement block for casts of the same variable. The detection uses textual comparison (`ToString()` plus substring checks) along with limited syntax analysis (`CastExpressionSyntax`). It does not understand conditional expressions, nested lambdas, or pattern negations (`is not`), leading to false positives or missed true positives.

## 2. Validation Plan

1. Extend `ModernCSharpTests.cs` with regression cases that mirror the IndTrace helpers listed below (ternaries, nested lambdas, tuple projections, local functions). Ensure expected diagnostics disappear after mitigation.
2. Introduce an integration-style test that runs EXXER702 against `Test Project/Src/Code/Core/Application/Models/Extensions` to confirm no warnings remain after adjustments.
3. Execute `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release` and spot-run `dotnet build Test Project/Src/Code/Core/Application/Core.Application.csproj -c Release` to verify the analyzer behaves correctly across real code.

## 3. Enhancement Opportunities (≥10 Items)

Each item identifies an observed false positive (or family of them), the proposed mitigation, and an xUnit + Shouldly snippet replicating the scenario. The snippets fail today (EXXER702 raised) but pass after the fix.

### 1. Conditional Operator Guards (`result is not null ? (T)result : …`)
- **Problem**: `ReflectionExtensions.InvokeMethod<T, TU>` at `Test Project/Src/Code/Core/Application/Models/Extensions/ReflectionExtensions.cs:25-34` assigns `(TU)result` inside a ternary whose guard uses `result is not null`. The analyzer reports EXXER702, even though transforming into a declaration pattern would mean rewriting the expression-bodied return.
- **Mitigation**: Skip ternary/conditional expressions (`ConditionalExpressionSyntax`) in `ContainsCastOfSameVariable`, or convert them to semantically-aware flow analysis instead of raw string matching.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Ternary_Guarded_Casts()
  {
      const string code = @"
public static class Sample
{
    public static string Invoke(object? value) =>
        value is not null ? ((string)value).ToUpperInvariant() : string.Empty;
}";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 2. Reflection Property Access
- **Problem**: `ReflectionExtensions.GetPrivatePropertyValue<T>` (`...ReflectionExtensions.cs:47-69`) and `GetPrivateFieldValue<T>` use identical ternary guards. Analyzer noise forces teams to disable the rule for reflection utilities.
- **Mitigation**: Treat reflection helpers (detected via pattern `GetValue()` plus ternary guard) as safe, or reuse the conditional-expression fix above.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Reflection_GetValue_Casts()
  {
      const string code = @"
public static class Sample
{
    public static T Read<T>(object source)
    {
        var value = source.GetType().GetProperty(""Name"")?.GetValue(source);
        return value is not null ? (T)value : default!;
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 3. Helper Methods Returning Tuples
- **Problem**: `ValueConverter.ReadFromBuffer<T>` (`Test Project/Src/Code/Infrastructure/Sharp7.Rx/BatchRead/ValueConverter.cs:64-105`) switches on `Type` and converts results with `(T)(object)str`. Although guarded by type checks inside a `switch`, EXXER702 warns on each branch.
- **Mitigation**: Detect switch expressions with guard clauses (pattern matching already applied) and skip diagnostics when the conversion occurs inside `Type switch` arms.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_TypeSwitch_Casts()
  {
      const string code = @"
using System;
public static class Sample
{
    public static (T Result, string Text) Convert<T>(object result, Type type) =>
        type switch
        {
            var t when t == typeof(string) && result is string str => ((T)(object)str, str),
            _ => throw new InvalidCastException()
        };
}";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 4. Local Function Closures
- **Problem**: `ReflectionExtensions.InvokeMethod<T, TU>` defines inline lambdas that capture `result`. When the cast occurs inside a local function, data flow analysis cannot guarantee safety, but the analyzer still reports EXXER702 even if the local function executes immediately inside the guard.
- **Mitigation**: Ignore casts inside local functions or lambdas unless the flow is trivially analyzable (e.g., invoked inline). Alternatively, require the cast expression to be in the same block scope.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_LocalFunction_Casts()
  {
      const string code = @"
using System;
public static class Sample
{
    public static string GetValue(object token)
    {
        if (token is JsonElement)
        {
            string Format() => ((JsonElement)token).ToString();
            return Format();
        }
        return string.Empty;
    }
}
public readonly struct JsonElement { public string ToString() => """"; }";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 5. Guard Clauses Using `is not null` with Return Statements
- **Problem**: `ReflectionExtensions.SetPrivatePropertyValue<T>` uses early returns (`if (obj == null) return Result.WithFailure(...)`) and then performs `(T)value` conversions. EXXER702 flags the conversion even though the guard uses `is not null` instead of `is`.
- **Mitigation**: Recognize `is not` patterns and skip diagnostics; the current implementation only inspects binary `is` expressions.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_IsNotNull_Guards()
  {
      const string code = @"
public static class Sample
{
    public static T Convert<T>(object value)
    {
        if (value is not null)
        {
            return (T)value;
        }
        return default!;
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 6. Nullable Unwrap Patterns
- **Problem**: Helper methods often perform `if (maybe is ValueType)` followed by `(ValueType)maybe`. When `maybe` is declared as `object?`, the analyzer flags even though the cast deliberately unboxes a value type. Example: `Test Project/Src/Code/Core/Application/Models/Extensions/ReflectionExtensions.cs:140-148`.
- **Mitigation**: Detect unboxing scenarios (cast target is a value type) and allow them when the guard ensures `is ValueType`.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Unboxing_Patterns()
  {
      const string code = @"
public static class Sample
{
    public static int Unbox(object maybe)
    {
        if (maybe is int)
        {
            return (int)maybe;
        }
        return -1;
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 7. Dictionary Lookup with Type Check
- **Problem**: `ValueConverter.ReadFunctions` dictionary lambdas guard on `type` equality and still cast `(byte[])result`. The analyzer cannot see the `type switch` guard, so it raises EXXER702.
- **Mitigation**: Recognize dictionary delegates within a `Type` guard and skip diagnostics; or require the analyzer to reason about `if (type == typeof(byte[]))` before the cast.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_TypeEquality_Guards()
  {
      const string code = @"
using System;
public static class Sample
{
    public static byte[] Convert(object result, Type type)
    {
        if (type == typeof(byte[]))
        {
            return (byte[])result;
        }
        throw new InvalidCastException();
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 8. Tuple Pattern Extraction
- **Problem**: `RegisterCleaner` tests use tuple deconstruction with casts (`var target = (Register)item;`). Guards are performed via `if (item is Register)` but the analyzer flags the tuple assignment even though the code sits inside asynchronous helper methods (`Test Project/Src/Tests/Core/Application.UnitTests/Cycles/Services/RegisterCleanerTests.cs:120-158`).
- **Mitigation**: Allow tuple assignments or aggregate initializations where the casted value is rewrapped before leaving the guarded scope.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_TupleAssignments()
  {
      const string code = @"
public static class Sample
{
    public static void Clean(object item, List<Register> target)
    {
        if (item is Register)
        {
            target.Add((Register)item);
        }
    }
}
public record Register;";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 9. Pattern-Matched Exception Handling
- **Problem**: `HubConnectionInterfaceExtensions.TryInvokeAsync` catches `HubException` and performs `string.Join(' ', new[] { s1, s2 })` after pattern matching. Other branches still use `(string)arg1` conversions guarded by type comparisons, triggering EXXER702.
- **Mitigation**: Skip diagnostics inside catch blocks where pattern matching is used for error recovery, or ensure detection logic respects `when` filters already employing patterns.
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_CatchBlock_Casts()
  {
      const string code = @"
using System;
public static class Sample
{
    public static void Handle(object arg)
    {
        try
        {
            throw new InvalidOperationException();
        }
        catch (InvalidOperationException) when (arg is string)
        {
            var text = (string)arg;
            Console.WriteLine(text.ToUpperInvariant());
        }
    }
}";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

### 10. Temporary Variable Reassignment
- **Problem**: Some code intentionally keeps the original variable name after casting (e.g., `var obj = (ControllerMonitor)this.Clone();`). Even if the guard used a pattern, the analyzer sees the cast and reports EXXER702, leading to redundant diagnostics in `ControllerMonitor.Equals` implementations and similar clones.
- **Mitigation**: Avoid flagging casts that assign back to the same identifier outside the guard (especially when the guard is already a pattern).
- **Test**:
  ```csharp
  [Fact]
  public void Should_Not_Report_For_Reassignments()
  {
      const string code = @"
public class ControllerMonitor
{
    public ControllerMonitor CloneIfMatch(object obj)
    {
        if (obj is ControllerMonitor)
        {
            obj = ((ControllerMonitor)obj).Clone();
            return (ControllerMonitor)obj;
        }
        return this;
    }
    public ControllerMonitor Clone() => this;
}";
      AnalyzerTestHelper.RunAnalyzer(code, new UseModernPatternMatchingAnalyzer())
                         .ShouldBeEmpty();
  }
  ```

## 4. Test-Driven Fix Strategy

- Replace string-based detection (`expression.ToString().Contains`) with semantic data flow: use `ISymbol` comparisons to ensure casts and guards refer to the same variable, and track control flow to confirm scope validity.
- Support conditional expressions (`ConditionalExpressionSyntax`), `is not` patterns, tuple assignments, and casts that occur inside local functions or lambdas executed within the guarded scope.
- Introduce helper methods that detect `switch` expressions, equality guards (`type == typeof(T)`), and catch filters to avoid noisy diagnostics.
- Expand unit tests (`ModernCSharpTests.cs`) with the ten scenarios above, ensuring EXXER702 stays silent where pattern matching is not strictly necessary or would harm clarity.
- After the refactor, rerun analyzer tests and spot builds to ensure the IndTrace sample projects are free of EXXER702 noise.
