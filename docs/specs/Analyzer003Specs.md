# DoNotThrowExceptions Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER003`  
**Source**: `src/code/IndFusion.Analyzer/FunctionalPatterns/DoNotThrowExceptionsAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- `docs/specs` currently contains specs for analyzers 300 and 301 only; no spec exists for EXXER003.  
- EXXER003 is the most disruptive remaining analyzer in the IndTrace solution: normal guard clauses across domain code trigger diagnostics, forcing teams to ignore the warnings.  
- Evidence gathered from `Test Project\Src\Code`:
  - `ProductFactory` performs required null guards (`Test Project\Src\Code\Core\Domain\Services\Products\ProductFactory.cs:46-50`).  
  - `Sharp7.Rx.VariableNameParser` throws domain-specific exceptions for invalid PLC addresses (`Test Project\Src\Code\Infrastructure\Sharp7.Rx\VariableNameParser.cs:67-172`).  
  - `ValueConverter` enforces buffer requirements through exceptions (`Test Project\Src\Code\Infrastructure\Sharp7.Rx\ValueConverter.cs:47-235`).  
  - These scenarios legitimately throw but currently surface EXXER003 diagnostics, making this analyzer the highest-volume false-positive generator without an existing mitigation spec.

## 1. Specification

- **Intent**  
  Promote functional error handling (e.g., `Result<T>`) in domain code by discouraging ad-hoc exception throwing.

- **Scope**  
  The analyzer inspects every `throw` statement (`SyntaxKind.ThrowStatement`). It filters out bare `throw;` rethrows and a minimal boundary check for controllers/web namespaces, but otherwise reports any exception instantiation.

- **Validation Plan**  
  1. Expand `IndFusion.Analyzer.Tests` with targeted unit tests covering each mitigation described below.  
  2. Introduce integration-style tests that run representative domain files through the analyzer to ensure guard patterns are exempted.  
  3. Validate against `Test Project\Src` by running `dotnet test` and collecting diagnostic counts pre/post change to confirm meaningful drops in EXXER003 noise.  
  4. Maintain positive coverage: ensure true positives (e.g., domain service throwing `Exception`) still surface diagnostics.

## 2. Enhancement Opportunities (>=10 Items)

Each enhancement captures an existing false-positive, proposes a mitigation, and includes a unit-test sketch (xUnit + Shouldly). Test snippets assume helper APIs from `AnalyzerTestHelper`.

### 1. Guard Clauses Throwing `ArgumentNullException`

- **Problem**: Null-guard patterns like those in `ProductFactory` throw `ArgumentNullException` for mandatory arguments, but EXXER003 flags them.  
- **Mitigation**: Detect guard-style `if (parameter is null) throw new ArgumentNullException(nameof(parameter));` or `value ?? throw new ArgumentNullException(...)` and suppress.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_ArgumentNull_Guards()
{
    const string testCode = @"
using System;

public class Guarded
{
    public static void Validate(string input)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }
    }
}";

    var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
    diagnostics.ShouldBeEmpty();
}
```

### 2. Guard Clauses Using Null-Coalescing Throws

- **Problem**: Property initialisers like `Label = label ?? throw new ArgumentNullException(nameof(label));` (see `BarCodeFactory.cs:28`) are reported.  
- **Mitigation**: Recognise null-coalescing throw expressions assigning to fields/properties during construction.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_NullCoalescing_Throw()
{
    const string testCode = @"
using System;

public sealed class Widget
{
    public string Label { get; }

    public Widget(string label)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Range Checks Throwing `ArgumentOutOfRangeException`

- **Problem**: `VariableNameParser` throws `ArgumentOutOfRangeException` when PLC addresses contain invalid values.  
- **Mitigation**: Allow guard throws targeting range validation (`ArgumentOutOfRangeException`, `ArgumentException`) when bound to parameter comparison.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Range_Guard()
{
    const string testCode = @"
using System;

public static class Parser
{
    public static void Validate(int port)
    {
        if (port < 0 || port > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port));
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Switch Expressions Throwing in Default Arms

- **Problem**: Expressions like `_ => throw new ArgumentOutOfRangeException(...)` inside switch expressions (e.g., `VariableNameParser.cs:105`) currently surface diagnostics.  
- **Mitigation**: Suppress diagnostics when the throw occurs inside a switch expression arm guarding exhaustiveness.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Switch_Default_Throw()
{
    const string testCode = @"
using System;

public enum DbType { Byte, Word }

public static class Resolver
{
    public static int Resolve(DbType type) => type switch
    {
        DbType.Byte => 1,
        DbType.Word => 2,
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Domain Parsing Exceptions (`InvalidS7AddressException`)

- **Problem**: Custom domain exceptions ensure invalid PLC addresses surface meaningful errors; EXXER003 flags them even though they represent validation, not control flow.  
- **Mitigation**: Whitelist domain-specific validation exceptions (configurable via attribute or naming convention like `*Exception`) when thrown inside parser/validator classes.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Domain_Validation_Exception()
{
    const string testCode = @"
using System;

public sealed class InvalidWidgetException : Exception
{
    public InvalidWidgetException(string message) : base(message) { }
}

public static class WidgetParser
{
    public static void Parse(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            throw new InvalidWidgetException(""Input required"");
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Constructors Enforcing Invariants

- **Problem**: Aggregate constructors sometimes throw when invariants fail (e.g., product line capacity). Such throws are necessary to prevent invalid state.  
- **Mitigation**: Permit throws inside constructors when validating constructor parameters or derived invariants.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Constructor_Invariant()
{
    const string testCode = @"
using System;

public sealed class Capacity
{
    public Capacity(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Factory Methods Performing Validation

- **Problem**: Static factory methods (e.g., `Create` patterns) often throw when invalid data is provided rather than returning `Result<T>`. During ongoing migrations, the analyzer should allow those until the functional pattern is adopted.  
- **Mitigation**: Downgrade diagnostics when method name matches `Create`, `Build`, or `Factory` and the exception type is one of the guard set.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Validation_In_Factory()
{
    const string testCode = @"
using System;

public static class WidgetFactory
{
    public static Widget Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException(""Name required"", nameof(name));
        }

        return new Widget(name);
    }
}

public sealed record Widget(string Name);
";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Throw Expressions in Expression-Bodied Members

- **Problem**: Expression-bodied properties or methods using throw expressions (`=> throw new ...`) for enforcement are flagged (e.g., domain value objects).  
- **Mitigation**: Apply guard heuristics to `ThrowExpressionSyntax` as well as `ThrowStatementSyntax`.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Expression_Bodied_Guard()
{
    const string testCode = @"
using System;

public sealed class Settings
{
    private readonly string _connectionString = """";

    public string ConnectionString => string.IsNullOrEmpty(_connectionString)
        ? throw new InvalidOperationException(""Connection string not configured"")
        : _connectionString;
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Public API Defensive Checks

- **Problem**: Public API layers (non-web) still need to throw `NotSupportedException` or `NotImplementedException` for unsupported features.  
- **Mitigation**: Allow defensive throws when exception type is `NotSupportedException`, `NotImplementedException`, or derived, optionally gated by `[Obsolete]` or `[EditorBrowsable]` context.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_NotSupported_Defensive_Throw()
{
    const string testCode = @"
using System;

public sealed class LegacyAdapter
{
    public void Execute()
    {
        throw new NotSupportedException(""Legacy adapter is read-only"");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Catch Blocks Re-throwing Wrapped Exceptions

- **Problem**: Wrapping exceptions in catch blocks (e.g., `catch (Exception ex) { throw new InvalidOperationException(""...", ex); }`) is flagged even though it adds context.  
- **Mitigation**: Detect throws inside catch clauses that include the caught exception as an inner exception and suppress them.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Exception_Wrapping()
{
    const string testCode = @"
using System;

public static class Wrapper
{
    public static void Execute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(""Failed to execute action"", ex);
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

## 3. Test-Driven Fix Strategy

1. Create `DoNotThrowExceptionsAnalyzerFalsePositiveTests` under `src/test/IndFusion.Analyzer.Tests/TestCases`.  
2. Add the ten snippets above as `[Fact]` tests asserting `AnalyzerTestHelper.RunAnalyzer(...).ShouldBeEmpty()`.  
3. Introduce companion positive tests ensuring EXXER003 still fires when, for example, a domain service throws `Exception` for control flow.  
4. Incrementally update analyzer logic:
   - Add heuristics recognising guard patterns (`ArgumentNullException`, `ArgumentOutOfRangeException`, etc.).  
   - Extend analysis to handle `ThrowExpressionSyntax` and catch-block wrapping detection.  
   - Provide configuration hooks for domain-specific exceptions if needed (attributes or naming conventions).  
5. Run the analyzer test suite before and after code changes to prove the new cases fail prior to implementation and pass afterward.  
6. Execute `dotnet test` against key IndTrace projects to confirm EXXER003 warnings recede materially.  
7. Update `AnalyzerReleases.Unshipped.md` noting the enhanced guard recognition and reduced noise.

## 4. Acceptance Checklist

- [ ] Analyzer updated with heuristic guardrails for the ten scenarios above.  
- [ ] Ten regression tests added and passing in `IndFusion.Analyzer.Tests`.  
- [ ] Standard builds/tests succeed (`dotnet build`, `dotnet test`).  
- [ ] Documentation (`AnalyzerReleases.Unshipped.md`) reflects improved behaviour.  
- [ ] Diagnostic counts in `Test Project\Src` confirm a significant reduction in EXXER003 false positives.

## 6. Audit Results (2025-10-08)

### Summary
- EXXER003 mitigations implemented in `src/code/IndFusion.Analyzer/FunctionalPatterns/DoNotThrowExceptionsAnalyzer.cs` now cover all 10 scenarios in this spec.
- A dedicated test class `DoNotThrowExceptionsAnalyzerFalsePositiveTests` was added with 10 mitigation tests plus one positive-control test.
- All new analyzer tests have an explicit xUnit v3 timeout to prevent hangs.

### What’s Implemented
- Guard clauses: `ArgumentNullException` via `if (x is null)` and `x ?? throw new ArgumentNullException(...)`.
- Range checks: `ArgumentOutOfRangeException`/`ArgumentException` with comparison-based conditions.
- Switch expression default-arm throws suppressed.
- Domain parsing exceptions: any `*Exception` thrown inside `*Parser`/`*Validator` classes.
- Constructor invariants: throws inside constructors for `Argument*`/`InvalidOperationException`.
- Factory methods `Create*`/`Build*`/`*Factory` with guard exceptions allowed.
- Expression-bodied guards using conditional `?: throw` suppressed.
- Defensive `NotSupportedException`/`NotImplementedException` allowed.
- Catch wrapping: `throw new X(..., ex)` in `catch` allowed.
- Boundary/test contexts ignored (web/controller namespaces; `*Tests`/`*Specs`/`*Benchmarks`; `Program`/`Startup`/`Bootstrap`).

### Gaps / Notes
- Domain exception allowlist is heuristic (class name contains `Parser`/`Validator` and thrown type ends with `Exception`). If narrower control is desired, introduce an `[AllowDomainValidation]` attribute.
- Expression-bodied property guards using null-coalescing `=> value ?? throw ...` are covered through `ArgumentNullException` coalesce detection; extend if other exception types are needed.
- Release notes not yet updated for EXXER003 in `AnalyzerReleases.Unshipped.md`.

### Remediation Plan
1. Add optional `[AllowDomainValidation]` attribute recognition to refine domain exception allowlist.
2. Extend guard recognition to support additional domain-specific guard exception types if required by usage (e.g., `FormatException`).
3. Update `src/code/IndFusion.Analyzer/AnalyzerReleases.Unshipped.md` with a brief note describing EXXER003 relaxed heuristics.
4. Monitor EXXER003 diagnostics across `Test Project\Src`; if hotspots persist, tune heuristics with targeted tests.

### Test Timeout Standard
- All analyzer tests updated to use `[Fact(Timeout = 10000)]` to prevent long-running hangs. Apply this to any future analyzer tests for consistency.

## 7. Handoff Notes for Next Agent

- Focus areas to finish enhancement:
  - Implement `[AllowDomainValidation]` attribute support (method/class) to precisely allow domain validation exceptions beyond name heuristics.
  - Extend guard recognition beyond `ArgumentNullException`/`ArgumentOutOfRangeException` to include `FormatException` for parsers and similar domain guards when appropriate.
  - Add tests for the attribute path (positive/negative), and for expression-bodied guards throwing other allowed exception types.
- Validation checklist to run:
  - `dotnet build src/IndFusion.sln -c Release`
  - `dotnet test src/test/IndFusion.Analyzer.Tests/IndFusion.Analyzer.Tests.csproj -c Release --filter FullyQualifiedName~DoNotThrowExceptionsAnalyzerFalsePositiveTests`
- Notes:
  - All new tests should include `[Fact(Timeout = 10000)]`.
  - If you widen heuristics, prefer adding narrowly targeted tests first to avoid masking true positives.
