# UseResultPattern Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER001`  
**Source**: `src/code/IndFusion.Analyzer/ErrorHandling/UseResultPatternAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- EXXER001 has no existing mitigation spec.  
- The analyzer reports an error whenever it encounters a `throw` inside a method/property/local function/lambda whose return type is not `Result<T>` (or `Task<Result<T>>`), aside from a few coarse exclusions.  
- In the IndTrace solution the vast majority of guard clauses, startup validation, Identity scaffolding, and infrastructure helpers still use exceptions appropriately. Examples include:
  - `ShiftDetectionRule.AppliesTo(int hour)` (`Test Project\Src\Code\Core\Domain\Services\ShiftDetectionRule.cs:21-40`) – returns `bool` and throws `ArgumentOutOfRangeException` for illegal hours.  
  - Connection-string guards inside `Program.cs` (`Test Project\Src\Code\Presentation\IndTrace.OEE\Program.cs:123-190`).  
  - Identity UI method `Register.razor` forcing email support (`Test Project\Src\Code\Presentation\IndTrace.OEE\Components\Account\Pages\Register.razor:123`).  
  - Guard helpers such as `Guard.ThrowIfNull` patterns across application services.  
- Because EXXER001 fires on nearly every legitimate exception outside the strict Result<T> pattern, it is one of the loudest analyzers still lacking a remediation plan.

## 1. Specification

- **Intent**  
  Encourage functional error handling via `Result<T>` rather than throwing exceptions across application/business logic, improving composability and testability.

- **Scope**  
  Visits methods, properties, local functions, and lambdas. Any discovered `throw`/`throw new` yields a diagnostic unless the member already returns `Result<T>` (with or without `Task/ValueTask`), or matches a small set of name-based exclusions (methods containing “Throw”, test attributes, controller namespaces).

## 2. Validation Plan

1. Add `UseResultPatternAnalyzerFalsePositiveTests` covering all scenarios below, plus positive control cases (e.g., domain service throwing `Exception`).  
2. Include integration fixtures referencing actual IndTrace files mentioned above to ensure diagnostics disappear.  
3. Run `dotnet test` for analyzer tests and for selected solution projects to confirm EXXER001 warning counts drop substantially.  
4. Keep positive coverage verifying that genuine misuse (e.g., core domain method throwing `Exception` when Result<T> is expected) still triggers the rule.

## 3. Enhancement Opportunities (>=10 Items)

Each subsection records an observed false positive, outlines mitigation, and includes an xUnit/Shouldly sample.

### 1. Domain Guard Method Returning `bool`

- **Problem**: `ShiftDetectionRule.AppliesTo` returns `bool` yet throws `ArgumentOutOfRangeException` for invalid hours, which is desirable domain validation.  
- **Mitigation**: Detect guard patterns in value objects/domain services (method names containing `Validate`, `AppliesTo`, `Ensure`, returning `bool`) and skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Domain_Guard()
{
    const string testCode = @"
using System;

public static class ShiftRules
{
    public static bool AppliesTo(int hour)
    {
        if (hour < 0 || hour > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hour));
        }

        return hour >= 7 && hour < 15;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Startup/Configuration Guard

- **Problem**: Minimal-host `Program` files throw `InvalidOperationException` when configuration is missing.  
- **Mitigation**: Skip methods inside classes named `Program`, `Startup`, or files with top-level statements.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Program_Config_Guard()
{
    const string testCode = @"
using System;

public static class Program
{
    public static string ResolveConnection(string? connectionString) =>
        connectionString ?? throw new InvalidOperationException(""Connection string not found."");
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Identity UI Scaffold (Enforce Email Support)

- **Problem**: Identity `.razor` code throws `NotSupportedException` when features are disabled (framework requirement).  
- **Mitigation**: Automatically skip components in namespaces containing `.Components.Account` or classes decorated with `[AllowThrowing]` (new attribute).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Identity_Scaffolding()
{
    const string testCode = @"
using System;

namespace Sample.Identity.Components;

public sealed class RegisterHandler
{
    public void EnsureEmailSupport(bool enabled)
    {
        if (!enabled)
        {
            throw new NotSupportedException(""Email support required."");
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Guard Helper/ThrowHelper Methods

- **Problem**: Centralized guard helpers intentionally throw (e.g., `Guard.ThrowIfNull`).  
- **Mitigation**: Skip methods/types whose names contain `Guard`, `ThrowHelper`, `Ensure`, or annotated with `[AllowGuardThrows]`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Guard_Helper()
{
    const string testCode = @"
using System;

public static class Guard
{
    public static void ThrowIfNull(object? value, string name)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Value Object Factory Allowing Exceptions

- **Problem**: Static factory methods returning primitive/bool values still throw when invariants fail (e.g., `Percentage.Clamp`).  
- **Mitigation**: Allow methods inside types named `Value`, `Metrics`, `Id`, `Amount`, etc., to throw guard exceptions (`ArgumentNullException`, `ArgumentOutOfRangeException`).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_ValueObject_Invariant()
{
    const string testCode = @"
using System;

public static class Percentage
{
    public static decimal Clamp(decimal value)
    {
        if (value is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        return value;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. `?? throw` Guards in Properties

- **Problem**: Expression-bodied properties with inline null guards (`get => _service ?? throw ...`) are flagged.  
- **Mitigation**: Recognize `ThrowExpressionSyntax` used in property getters for lazy initialization/DI and skip.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Property_Null_Guard()
{
    const string testCode = @"
using System;

public sealed class LazyService
{
    private IService? _service;
    public IService Service => _service ?? throw new InvalidOperationException(""Service not configured."");
}

public interface IService { }
";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Test Helper Methods Inside Test Classes

- **Problem**: Analyzer already skips attributed test methods but still flags private helper methods within `*Tests` classes that throw intentionally.  
- **Mitigation**: If the containing type name ends with `Tests`, `Specs`, or `Benchmarks`, treat all nested members as test context.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Helper_In_Test_Class()
{
    const string testCode = @"
using System;
using Xunit;

public class CalculationTests
{
    [Fact]
    public void Should_Throw() => Assert.Throws<InvalidOperationException>(() => Helper());

    private static void Helper()
    {
        throw new InvalidOperationException();
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Async Handlers Returning `Task`

- **Problem**: Asynchronous event handlers or background jobs (`Task` return type) may throw to surface fatal errors; analyzer still demands `Task<Result>`.  
- **Mitigation**: Permit throws within methods returning `Task`/`ValueTask` if they are background workers (`ExecuteAsync`, `HandleAsync`, etc.) or decorated with `[HostedService]`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Background_Task_Throws()
{
    const string testCode = @"
using System;
using System.Threading.Tasks;

public sealed class Worker
{
    public async Task ExecuteAsync()
    {
        await Task.Delay(10);
        throw new InvalidOperationException(""Fatal background failure"");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Local Functions Performing Validation

- **Problem**: Local helper functions within a method might throw for validation; analyzer currently inspects them individually and raises diagnostics.  
- **Mitigation**: When a local function is called immediately for guarding (identified via data flow or naming), suppress diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Local_Function_Guard()
{
    const string testCode = @"
using System;

public sealed class Processor
{
    public void Process(string command)
    {
        EnsureValid(command);

        void EnsureValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(""Command required"", nameof(value));
            }
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Opt-Out Attribute (`[AllowExceptions]`)

- **Problem**: Certain modules (migration scripts, telemetry adapters) deliberately use exceptions and should opt out.  
- **Mitigation**: Introduce/recognize `[AllowExceptions]` attribute at class/method/lambda scope.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_OptOut_Attribute()
{
    const string testCode = @"
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowExceptionsAttribute : Attribute { }

[AllowExceptions]
public sealed class MigrationScript
{
    public void Run() => throw new NotSupportedException(""Legacy migration"");
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer())
        .ShouldBeEmpty();
}
```

## 4. Test-Driven Fix Strategy

1. Implement the ten regression tests plus positive control cases in the analyzer test suite.  
2. Enhance `UseResultPatternAnalyzer` to:
   - Incorporate semantic information (method return types, class/namespace context).  
   - Recognize guards, helper conventions, background tasks, and Identity/UI scaffolding.  
   - Honour opt-out attributes and test contexts.  
   - Share heuristics with EXXER002 (AvoidThrowingExceptions) to avoid duplication.  
3. Re-run analyzer tests ensuring new cases fail before the implementation and pass after.  
4. Execute `dotnet test` for affected solution projects (Domain, Application, Presentation) to confirm EXXER001 warnings fall to an acceptable level.  
5. Update `AnalyzerReleases.Unshipped.md` documenting the relaxed enforcement and opt-out support.

## 5. Acceptance Checklist

- [ ] Analyzer updated with guard/context recognition and opt-out attribute.  
- [ ] Ten regression tests added/passing.  
- [ ] Solution builds/tests succeed.  
- [ ] Documented reduction in EXXER001 warnings across domain/application/presentation code.  
- [ ] Release notes refreshed accordingly.
