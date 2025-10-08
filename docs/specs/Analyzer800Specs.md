# UseStructuredLogging Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER800`  
**Source**: `src/code/IndFusion.Analyzer/Logging/UseStructuredLoggingAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- Existing specs now cover analyzers 003, 200, 300, 301, 302, and 500; EXXER800 still lacks documentation.  
- The current implementation flags *every* interpolated or concatenated string passed as the first argument to methods whose name starts with “Log”, regardless of whether the call targets `ILogger`.  
- False positives observed in `Test Project\Src` include:
  - `IdentityComponentsEndpointRouteBuilderExtensions.cs:86` – `_logger.LogInformation("User with ID '{UserId}' asked...", userId);` (already structured, yet still flagged).  
  - Diagnostics triggered on interpolated strings sent to domain-provided loggers or test doubles.  
- Because the analyzer lacks scope control and produces widespread noise, it is the next high-impact candidate.

## 1. Specification

- **Intent**  
  Encourage structured logging usage in `ILogger` calls by discouraging string concatenation or interpolation in log message templates.

- **Scope**  
  Inspects `InvocationExpressionSyntax`. If the method name begins with “Log”, it flags the first argument whenever it detects a `BinaryExpressionSyntax` (string concatenation) or `InterpolatedStringExpressionSyntax`, without verifying the receiver type or existing structured templates.

- **Validation Plan**  
  1. Add `UseStructuredLoggingAnalyzerFalsePositiveTests` covering each mitigation case below.  
  2. Introduce regression tests for calls on non-`ILogger` types, templated messages, interpolation handlers, and logging abstraction wrappers.  
  3. Run `dotnet test` for analyzer/IndTrace projects pre/post change to validate warning reductions.  
  4. Maintain true-positive coverage by keeping tests that ensure bare string concatenation on `ILogger` still raises EXXER800.

## 2. Enhancement Opportunities (>=10 Items)

Each entry outlines an observed false-positive, suggests the mitigation, and supplies an xUnit/Shouldly test sketch.

### 1. Verify the Receiver Is `ILogger`

- **Problem**: Any method whose name starts with “Log” is treated as logging, even if it is not an `ILogger` call.  
- **Mitigation**: Use semantic analysis to confirm the target method is an `ILogger` extension or instance method (`Microsoft.Extensions.Logging`).  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_NonILogger_LogMethod()
{
    const string testCode = @"
public static class Diagnostics
{
    public static void LogMessage(string message) { }
}

public sealed class Consumer
{
    public void Run()
    {
        Diagnostics.LogMessage($""Status: online"");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Structured Templates Already Present

- **Problem**: Calls like `_logger.LogInformation("User {UserId} requested", userId);` are flagged even though they already use structured placeholders.  
- **Mitigation**: When the string literal contains format tokens (`{Identifier}`), do not report.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Structured_Template()
{
    const string testCode = @"
using Microsoft.Extensions.Logging;

public sealed class Handler
{
    private readonly ILogger<Handler> _logger;

    public Handler(ILogger<Handler> logger) => _logger = logger;

    public void Execute(int userId)
    {
        _logger.LogInformation(""User {UserId} requested data"", userId);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Interpolated String Handlers

- **Problem**: Modern logging APIs accept interpolated string handlers (e.g., `LoggerMessageAttribute`, custom handlers). The analyzer unconditionally flags them.  
- **Mitigation**: Detect when the target method parameter type ends with `InterpolatedStringHandler` and skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Interpolated_String_Handler()
{
    const string testCode = @"
using Microsoft.Extensions.Logging;

public sealed class Handler
{
    private readonly ILogger<Handler> _logger;

    public Handler(ILogger<Handler> logger) => _logger = logger;

    [LoggerMessage(Level = LogLevel.Information, Message = ""Processing {Id}"")]
    private partial void LogProcessing(int id);

    public void Execute(int id) => LogProcessing(id);
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. Logging Wrappers Returning Structured Templates

- **Problem**: Helper methods often return structured strings (e.g., `LoggingMessage.UserRequested(userId)`). The analyzer should not report when the first argument is an identifier referencing such a helper.  
- **Mitigation**: Skip when the first argument is an identifier or invocation that resolves to string constants or helper methods annotated as structured.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Logging_Message_Helper()
{
    const string testCode = @"
using Microsoft.Extensions.Logging;

public static class LoggingMessages
{
    public static string UserRequested(int id) => ""User {UserId} requested""; // structured template
}

public sealed class Handler
{
    private readonly ILogger<Handler> _logger;

    public Handler(ILogger<Handler> logger) => _logger = logger;

    public void Execute(int id)
    {
        _logger.LogInformation(LoggingMessages.UserRequested(id), id);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. String Interpolation with Positional Arguments

- **Problem**: The analyzer flags interpolated strings even if they contain structured placeholders inside the interpolation (`$"User {userId}"` followed by additional properties).  
- **Mitigation**: Evaluate the interpolated string; if it only interpolates simple variables and the template contains braces, treat it as valid.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Interpolated_Template_With_Structured_Places()
{
    const string testCode = @"
using Microsoft.Extensions.Logging;

public sealed class Handler
{
    private readonly ILogger<Handler> _logger;

    public Handler(ILogger<Handler> logger) => _logger = logger;

    public void Execute(int userId)
    {
        _logger.LogInformation($""User {{UserId}} requested"", userId);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Localization Resources

- **Problem**: Some logging calls pass localized strings retrieved from resource managers; the analyzer misinterprets them as unstructured.  
- **Mitigation**: If the message argument resolves to `IStringLocalizer` indexer or `.GetString`, skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Localized_Message()
{
    const string testCode = @"
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

public sealed class Handler
{
    private readonly ILogger<Handler> _logger;
    private readonly IStringLocalizer<Handler> _localizer;

    public Handler(ILogger<Handler> logger, IStringLocalizer<Handler> localizer)
    {
        _logger = logger;
        _localizer = localizer;
    }

    public void Execute()
    {
        _logger.LogInformation(_localizer[""UserRequested""], 42);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. Structured Logging Libraries (Serilog, NLog)

- **Problem**: When Serilog’s `ILogger` is used (`Log.Information("User {UserId}", userId);`), the analyzer still warns.  
- **Mitigation**: Extend detection to accept known structured logging frameworks by namespace/type (Serilog, NLog).  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Serilog_Usage()
{
    const string testCode = @"
using Serilog;

public sealed class Handler
{
    private readonly ILogger _logger;

    public Handler(ILogger logger) => _logger = logger;

    public void Execute(int userId)
    {
        _logger.Information(""User {UserId} processed"", userId);
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Logging Level Methods Without Templates

- **Problem**: Calls that simply log an interpolated string without additional parameters but to non-structured sinks (e.g., console) should perhaps be allowed when not targeting `ILogger`.  
- **Mitigation**: After verifying the receiver type, allow interpolated strings if the method is not structured (e.g., `Console.WriteLine`).  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Console_WriteLine()
{
    const string testCode = @"
public static class ConsoleLogger
{
    public static void Write(string value) => System.Console.WriteLine(value);

    public static void Demo()
    {
        Write($""Status: {42}"");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. Testing Context (xUnit Output Helper)

- **Problem**: Test code often uses `_output.WriteLine($"Scenario: {state}")`; the analyzer flags it despite not being production logging.  
- **Mitigation**: Exempt calls where the receiver type is `ITestOutputHelper` or other known test sinks.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Test_Output_Helper()
{
    const string testCode = @"
using Xunit.Abstractions;

public sealed class Spec
{
    private readonly ITestOutputHelper _output;

    public Spec(ITestOutputHelper output) => _output = output;

    public void Trace(string state)
    {
        _output.WriteLine($""State: {state}"");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Diagnostic Loggers with Explicit Annotations

- **Problem**: Certain loggers are intentionally non-structured but decorated with custom attributes (e.g., `[AllowInterpolatedLogging]`). A static analyzer should respect such opt-outs.  
- **Mitigation**: If the method or its containing type carries an opt-out attribute, skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_OptOut_Attribute()
{
    const string testCode = @"
using Microsoft.Extensions.Logging;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AllowInterpolatedLoggingAttribute : Attribute { }

[AllowInterpolatedLogging]
public sealed class LegacyLogger
{
    private readonly ILogger<LegacyLogger> _logger;

    public LegacyLogger(ILogger<LegacyLogger> logger) => _logger = logger;

    public void Trace(int id)
    {
        _logger.LogInformation($""Trace {id}"");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer())
        .ShouldBeEmpty();
}
```

## 3. Test-Driven Fix Strategy

1. Add the ten test cases above under `UseStructuredLoggingAnalyzerFalsePositiveTests`.  
2. Maintain positive coverage ensuring true violations (string concatenation on `ILogger`) still trigger diagnostics.  
3. Update analyzer logic:
   - Confirm the receiver is an `ILogger` (or supported structured logger).  
   - Skip when structured templates `{Property}` already exist, or when the argument resolves to known structured helpers.  
   - Honor interpolated string handler parameters and opt-out attributes.  
   - Allow localized resources, test sinks, and wrappers that output to non-structured destinations.  
4. Execute analyzer tests to confirm the new cases fail before code changes and pass afterward.  
5. Run `dotnet test` on IndTrace projects to observe reduction of EXXER800 warnings.  
6. Update `AnalyzerReleases.Unshipped.md` documenting the refined logging detection.

## 4. Acceptance Checklist

- [ ] Analyzer enhanced with receiver/type validation and structured-template awareness.  
- [ ] Ten regression tests added/passing.  
- [ ] Solution builds/tests succeed.  
- [ ] EXXER800 false positives demonstrably reduced in IndTrace code.  
- [ ] Release notes updated accordingly.
