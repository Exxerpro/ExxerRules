# ValidateNullParameters Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER200`  
**Source**: `src/code/IndFusion.Analyzer/NullSafety/ValidateNullParametersAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- Existing specs cover analyzers 003, 300, and 301; EXXER200 currently has no specification document.  
- `ValidateNullParametersAnalyzer` is one of the most intrusive rules in the IndTrace codebase: it inspects every method and frequently misclassifies parameters, leading to hundreds of warnings in the test project.  
- Evidence from `Test Project\Src`:
  - Methods with value-type parameters (`int`, `bool`) still trigger diagnostics due to string-based heuristics (`Contains("Enumerable")`, `Contains("List")`).  
  - Cancellation tokens and `IServiceProvider` parameters still receive false positives, especially in constructors.  
  - Expression-bodied members and secondary guard patterns are not recognised, generating noise across domain and application layers.  
- Because the rule remains undocumented and is responsible for substantial false positives, EXXER200 is the next candidate for deep review.

## 1. Specification

- **Intent**  
  Ensure reference-type parameters are validated for null at method entry to support defensive programming and fail-safe defaults.

- **Scope**  
  Visits `MethodDeclarationSyntax` nodes. It collects parameters by checking strings such as `Type.ToString().Contains("String")` or semantic fallbacks, then verifies statements/expressions in the method body for null checks (`if (param == null)`, `ArgumentNullException.ThrowIfNull(param)`, etc.). It also synthesises statements for expression-bodied members.

- **Validation Plan**  
  1. Expand `IndFusion.Analyzer.Tests` with a dedicated `ValidateNullParametersAnalyzerFalsePositiveTests` suite.  
  2. Introduce regression tests covering constructors, expression-bodied members, lambdas, extension methods, and guard helper usage.  
  3. Run `dotnet test` against the analyzer test project and representative IndTrace projects (`Test Project\Src\Tests\...`) before/after implementing mitigations to confirm the warning count drop.  
  4. Maintain positive coverage by keeping existing true-positive tests in `AsyncTests` and `EdgeCaseTests`.

## 2. Enhancement Opportunities (>=10 Items)

Each enhancement describes an observed false-positive, proposes a mitigation, and provides a minimal xUnit/Shouldly test sketch.

### 1. Value-Type Parameters Misclassified

- **Problem**: The analyzer uses string heuristics (`Type.ToString().Contains("Enumerable")`) and often treats value types or structs as reference types, resulting in diagnostics for parameters like `int id`.  
- **Mitigation**: Use semantic model (`ITypeSymbol.IsReferenceType`) exclusively; skip types where `IsValueType` is true, including nullable value types (`Nullable<T>`).  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Value_Types()
{
    const string testCode = @"
public static class Calculator
{
    public static int Add(int left, int right) => left + right;
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. CancellationToken Parameters

- **Problem**: Methods accepting `CancellationToken` are marked unvalidated, even though tokens are value types and should be exempt.  
- **Mitigation**: Recognise tokens via semantic check (`SpecialType.System_Threading_CancellationToken`) and skip them entirely.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_CancellationToken()
{
    const string testCode = @"
using System.Threading;
using System.Threading.Tasks;

public static class Worker
{
    public static Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Services Resolved via DI (`IServiceProvider`)

- **Problem**: Constructors taking `IServiceProvider` and immediately storing it (null-checked by DI container) are flagged.  
- **Mitigation**: Allow interface-type parameters decorated with `[ServiceDescriptor]` patterns or when `[ActivatorUtilitiesConstructor]` is used. At minimum, skip `IServiceProvider` and `ILogger` dependencies.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_ServiceProvider()
{
    const string testCode = @"
using System;
using Microsoft.Extensions.Logging;

public sealed class Handler
{
    private readonly IServiceProvider _services;
    private readonly ILogger<Handler> _logger;

    public Handler(IServiceProvider services, ILogger<Handler> logger)
    {
        _services = services;
        _logger = logger;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Expression-Bodied Members with Guard Helpers

- **Problem**: Expression-bodied methods delegating to guard helpers (`Guard.Against.Null(param, nameof(param));`) are still flagged because the analyzer only inspects statements.  
- **Mitigation**: When `method.ExpressionBody` contains a guard invocation or throws, treat it as validation.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Expression_Bodied_Guard()
{
    const string testCode = @"
using Ardalis.GuardClauses;

public static class Guarded
{
    public static string Echo(string value) => Guard.Against.Null(value, nameof(value));
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Guard Helper Methods (`ThrowIfNull`)

- **Problem**: Methods that immediately call `ArgumentNullException.ThrowIfNull(parameter, nameof(parameter));` in expression form are missed when the argument order differs or is invoked via extension (`parameter.ThrowIfNull();`).  
- **Mitigation**: Enhance detection to include extension-method guard patterns and variations in argument lists.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Extension_Guard()
{
    const string testCode = @"
using System;

public static class GuardExtensions
{
    public static void ThrowIfNull<T>(this T instance, string name)
        where T : class
    {
        if (instance is null) throw new ArgumentNullException(name);
    }
}

public sealed class Consumer
{
    public void Execute(object dependency)
    {
        dependency.ThrowIfNull(nameof(dependency));
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Optional Parameters with Default Values

- **Problem**: Parameters with default values (e.g., `string? name = null`) are flagged even though null is an expected input.  
- **Mitigation**: Skip nullable reference parameters with default value `null` or optional parameters using `[Optional]`.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Optional_String()
{
    const string testCode = @"
public static class Formatter
{
    public static string ToDisplay(string? label = null) => label ?? ""(none)"";
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Params Arrays

- **Problem**: `params string[] segments` are considered reference-type parameters and raise diagnostics, even though `Array.Empty<T>()` is a valid default.  
- **Mitigation**: Treat `params` arrays as exempt or require only a null-check when the method uses the array directly (most call sites pass zero arguments).  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Params_Array()
{
    const string testCode = @"
public static class PathBuilder
{
    public static string Combine(params string[] segments) =>
        string.Join('/', segments);
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Record Primary Constructors

- **Problem**: Record primary constructors performing inline validation (`record User(string Name) { ... }`) are ignored by the analyzer's statement scan, causing false positives when property initialisers already guard the values.  
- **Mitigation**: Recognise `RecordDeclarationSyntax` with parameter property initialisers that include guard expressions (`Name = name ?? throw ...`).  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Record_Primary_Constructor_Guard()
{
    const string testCode = @"
using System;

public sealed record User(string Name)
{
    public string Email { get; init; } = string.Empty;
    public User(string name, string email) : this(name ?? throw new ArgumentNullException(nameof(name)))
    {
        Email = email ?? throw new ArgumentNullException(nameof(email));
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Local Functions and Lambdas

- **Problem**: Methods containing local functions or lambdas with guards still trigger diagnostics because the analyzer only inspects the top-level method body.  
- **Mitigation**: When guard logic lives in a local function called at the start of the method, recognise it as validation.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Local_Function_Guard()
{
    const string testCode = @"
using System;

public sealed class GuardedService
{
    public void Process(string command)
    {
        Guard(command);

        void Guard(string value)
        {
            if (value is null) throw new ArgumentNullException(nameof(value));
        }
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Methods Returning `Result<T>` Already Encapsulating Guards

- **Problem**: Methods that immediately return `Result.Failure(...)` when parameters are null are flagged because the analyzer only looks for explicit throws.  
- **Mitigation**: Detect guard patterns that return failure results (e.g., `return Result.Failure("name cannot be null");`) or call into a guard helper before other logic.  
- **Test Sketch**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Result_Failure_Guard()
{
    const string testCode = @"
using System;

public readonly record struct Result(bool Success, string? Error = null)
{
    public static Result Failure(string error) => new(false, error);
}

public static class Processor
{
    public static Result Validate(string name)
    {
        if (name is null)
        {
            return Result.Failure(""Name required"");
        }

        return new Result(true);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer())
        .ShouldBeEmpty();
}
```

## 3. Test-Driven Fix Strategy

1. Create `ValidateNullParametersAnalyzerFalsePositiveTests` under `src/test/IndFusion.Analyzer.Tests/TestCases`.  
2. Add the ten scenarios above as `[Fact]` methods asserting `ShouldBeEmpty()`.  
3. Retain/extend positive coverage to ensure EXXER200 still reports when no guard exists on true reference-type parameters.  
4. Update the analyzer:
   - Replace string-based parameter classification with semantic `ITypeSymbol` checks.  
   - Add exemption logic for known infrastructure types (`CancellationToken`, `ILogger<>`, `IServiceProvider`, etc.).  
   - Extend guard detection to expression bodies, null-coalescing throws, extension guards, record constructors, local functions, and `Result<T>` failure paths.  
5. Run the analyzer test suite and confirm new tests fail before implementation and pass afterward.  
6. Execute `dotnet test` against `Test Project\Src\Tests` to measure reduction in EXXER200 diagnostics.  
7. Update `AnalyzerReleases.Unshipped.md` documenting the refined guard detection.

## 4. Acceptance Checklist

- [ ] Analyzer heuristics enhanced for all ten scenarios.  
- [ ] Ten new regression tests added and passing.  
- [ ] Build/test pipelines succeed (`dotnet build`, `dotnet test`).  
- [ ] Diagnostic volume in IndTrace projects is materially reduced.  
- [ ] Documentation updated (this spec + release notes).
