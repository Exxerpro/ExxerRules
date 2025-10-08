# AvoidThrowingExceptions Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER002`  
**Source**: `src/code/IndFusion.Analyzer/ErrorHandling/AvoidThrowingExceptionsAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- No spec exists for EXXER002.  
- The analyzer flags every `throw` or `throw new` statement unless it falls into narrowly hard-coded exclusions. In practice, the IndTrace codebase relies on guard clauses, domain invariants, configuration validation, and framework-required exceptions.  
- High-noise examples:
  - Constructors/guards using `?? throw new ArgumentNullException(...)`, e.g., `GatewayAuditFactory` (`Test Project\Src\Code\Core\Application\Gateway\Auditing\GatewayAuditFactory.cs:28-29`).  
  - Domain validation such as `ShiftDetectionRule` (`Test Project\Src\Code\Core\Domain\Services\ShiftDetectionRule.cs:30`) throwing `ArgumentOutOfRangeException`.  
  - Startup/bootstrap logic throwing `InvalidOperationException` when configuration is missing (`Test Project\Src\Code\Presentation\IndTrace.OEE\Program.cs:123`).  
  - UI components throwing `NotSupportedException` required by Identity scaffolding (`Test Project\Src\Code\Presentation\IndTrace.OEE\Components\Account\Pages\Register.razor:123`).  
- Developers are forced to suppress diagnostics globally; therefore EXXER002 is the largest remaining unmitigated analyzer producing false positives.

## 1. Specification

- **Intent**  
  Encourage functional error handling via the `Result<T>` pattern instead of indiscriminate exceptions, improving composability and predictability.

- **Scope**  
  Registers syntax actions for `ThrowStatementSyntax` and `ThrowExpressionSyntax`. A diagnostic is emitted for every throw unless it occurs in a test method, a “Throw” helper method, a catch filter, or a heuristic boundary layer (controllers, namespaces containing `.Web`, `.Api`, `.Presentation`).

## 2. Validation Plan

1. Create `AvoidThrowingExceptionsAnalyzerFalsePositiveTests` covering the ten scenarios below plus positive cases.  
2. Seed integration tests using real IndTrace files (guards, configuration checks, Identity UI) to ensure diagnostics disappear post-fix.  
3. Run `dotnet test` for analyzer suites and selected solution projects to confirm the warning count drop.  
4. Maintain positive coverage to surface genuine misuse (e.g., throwing generic `Exception` inside core services).

## 3. Enhancement Opportunities (>=10 Items)

Each item documents the false-positive and supplies an xUnit/Shouldly test sketch.

### 1. Null-Guard Pattern (`?? throw new ArgumentNullException`)

- **Example**: `GatewayAuditFactory` constructor.  
- **Mitigation**: Detect null-guard idioms (`x ?? throw new ArgumentNullException(...)`, `if (x is null) throw ...`) and allow them.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Null_Guard_Throw()
{
    const string testCode = @"
using System;

public sealed class Service
{
    private readonly object _dependency;

    public Service(object dependency) =>
        _dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Range Validation (`ArgumentOutOfRangeException`)

- **Example**: `ShiftDetectionRule` clamps hours `0-23`.  
- **Mitigation**: Permit throws inside range guards for primitive value validation.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Range_Guard()
{
    const string testCode = @"
using System;

public static class Clock
{
    public static void ValidateHour(int hour)
    {
        if (hour < 0 || hour > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(hour), ""Hour must be between 0 and 23"");
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Configuration Failures (`InvalidOperationException` on missing config)

- **Example**: Minimal `Program.cs` retrieving connection strings.  
- **Mitigation**: Allow `InvalidOperationException` when thrown inside startup/bootstrap types or top-level methods.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Startup_Config_Throws()
{
    const string testCode = @"
using System;

public static class Startup
{
    public static string ResolveConnection(string? connectionString) =>
        connectionString ?? throw new InvalidOperationException(""Connection string not found."");
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Framework Requirements (`NotSupportedException` in Identity UI)

- **Example**: Identity scaffolding requires throwing when email support is disabled.  
- **Mitigation**: Skip diagnostics in auto-generated Identity UI or code annotated with `[AllowConsoleLogging]` etc. (by namespace `.Components.Account`).  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Identity_Scaffolding_Throws()
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
            throw new NotSupportedException(""The default UI requires a user store with email support."");
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Fatal Error Logging with Exit

- **Example**: Bootstrap code catching exceptions, writing to console, then `Environment.Exit(-1)`.  
- **Mitigation**: Allow throws immediately re-thrown inside a `catch` when followed by termination logic.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Fatal_Error_Reporting()
{
    const string testCode = @"
using System;

public static class Bootstrap
{
    public static void Start()
    {
        try
        {
            Run();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            throw;
        }
    }

    private static void Run() => throw new InvalidOperationException();
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Exception Wrapping for Context

- **Example**: `catch (Exception ex) { throw new InvalidOperationException("... ", ex); }` adds context but analyzer flags it.  
- **Mitigation**: Recognise wrapping that preserves `InnerException`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_Exception_Wrapping()
{
    const string testCode = @"
using System;

public static class Orchestrator
{
    public static void Execute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(""Execution failed."", ex);
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. `ThrowHelper` Methods and Assertions

- **Problem**: Helper methods with names like `ThrowIfInvalid` still get flagged when they intentionally centralize exception logic.  
- **Mitigation**: Treat methods whose declaring type/method name contains `ThrowHelper`, `Ensure`, `Guard` as safe.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_ThrowHelper_Methods()
{
    const string testCode = @"
using System;

public static class Guard
{
    public static void ThrowIfNull([System.Diagnostics.CodeAnalysis.NotNull] object? value, string name)
    {
        if (value is null)
        {
            throw new ArgumentNullException(name);
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Domain Value Invariants (Records/Value Objects)

- **Problem**: Value objects (e.g., `OeeMetrics.Build`) throw domain-specific `ArgumentException` when constraints fail.  
- **Mitigation**: Allow throws inside value object factories (types ending with `Value`, `Metrics`, `Id`) especially when returning `Result<T>`.  
- **Test**:

```csharp
[Fact]
public async Task Analyzer_Allows_ValueObject_Invariants()
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

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Opt-Out Attribute for Legacy Code

- **Problem**: Some modules deliberately use exceptions (e.g., integration harnesses, console tools). Provide explicit opt-out attribute.  
- **Mitigation**: Respect `[AllowExceptions]` at method/class/assembly scope.  
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
public sealed class LegacyAdapter
{
    public void Execute()
    {
        throw new NotSupportedException(""Legacy path"");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Test/Benchmark Harnesses

- **Problem**: Even with test detection, helper methods invoked by tests (same class but without attributes) still raise diagnostics.  
- **Mitigation**: If the containing type name ends with `Tests`, `Benchmarks`, `Specs`, treat all throws within as test context.  
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
    public void ShouldThrow() => Assert.Throws<InvalidOperationException>(() => Helper());

    private static void Helper()
    {
        throw new InvalidOperationException();
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer())
        .ShouldBeEmpty();
}
```

## 4. Test-Driven Fix Strategy

1. Add the ten regression tests above (plus positive control cases) to the analyzer test suite.  
2. Update `AvoidThrowingExceptionsAnalyzer` to:
   - Recognize guard patterns, value objects, configuration checks, and helper conventions using semantic analysis.  
   - Honour opt-out attributes, namespaces for tests/UI, and generated Identity scaffolding.  
   - Distinguish context via symbol information (e.g., `ICollection`, `ValueObject` types) rather than simple string matching.  
3. Re-run analyzer tests ensuring new cases fail prior to implementation and pass after adjustments.  
4. Execute `dotnet test` on affected IndTrace projects (Core.Application, Domain, Presentation OEE) to confirm the EXXER002 warning drop.  
5. Update `AnalyzerReleases.Unshipped.md` summarising the improved heuristics and opt-out options.

## 5. Acceptance Checklist

- [ ] Analyzer enhanced with guard/invariant recognition and opt-outs.  
- [ ] Ten regression tests added/passing.  
- [ ] Solution builds/tests succeed.  
- [ ] Reduction in EXXER002 warnings verified across domain/application layers.  
- [ ] Release notes updated.
