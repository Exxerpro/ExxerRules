# DoNotUseConsoleWriteLine Analyzer – False-Positive Mitigation Spec

**Analyzer ID**: `EXXER801`  
**Source**: `src/code/IndFusion.Analyzer/Logging/DoNotUseConsoleWriteLineAnalyzer.cs`  
**Prepared by**: Codex agent (2025-10-07)

## 0. Selection Rationale

- Specs already exist for analyzers 003, 200, 300, 301, 302, 500, and 800. EXXER801 remains undocumented.  
- The analyzer currently bans *all* usages of `Console.WriteLine/Write`, even in developer tooling, conditional debug blocks, or test harnesses.  
- Real-world occurrences include the build orchestration script (`Test Project\Src\Build\Build.cs:258-287`), where writing to console is necessary for CLI feedback. Safely suppressing those warnings is currently impossible without disabling the rule project-wide.  
- Given its broad scope and lack of guardrails, EXXER801 is a major source of false positives in tooling and test code.

## 1. Specification

- **Intent**  
  Encourage teams to use structured logging (`ILogger`) instead of writing directly to the console in production services.

- **Scope**  
  The analyzer inspects every `InvocationExpressionSyntax` and reports when it detects `Console.WriteLine`, `Console.Write`, `Console.Error`, or `Console.Out`—based purely on name matching. It does not check compilation context, conditional compilation symbols, or whether the code belongs to tooling/test projects.

- **Validation Plan**  
  1. Create `DoNotUseConsoleWriteLineAnalyzerFalsePositiveTests` containing the scenarios below.  
  2. Cover CLI build scripts, debug-only code, unit tests, global `Program.cs` console apps, and console wrappers.  
  3. Run `dotnet test` for analyzer/tests projects pre/post change to confirm warning reduction.  
  4. Retain positive coverage verifying that production services writing to console are still flagged.

## 2. Enhancement Opportunities (>=10 Items)

Each item records the observed pattern, proposes a guardrail, and includes an xUnit/Shouldly test sketch.

### 1. Console Applications & Tooling (`Main` Methods)

- **Problem**: Console apps (like build/CLI utilities) legitimately use `Console.WriteLine`, yet the analyzer flags them.  
- **Mitigation**: If the enclosing type is `Program` with `Main` entry point or project is marked as exe/tooling (namespace contains `.Build`, `.Tools`), skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Console_Main()
{
    const string testCode = @"
using System;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine(""Preparing build..."");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 2. Build/Deployment Scripts

- **Problem**: Scripts under `Build.cs` (e.g., `Test Project\Src\Build\Build.cs:258`) print status information; these should be exempt.  
- **Mitigation**: When namespace or file path contains `Build`, `Deployment`, or `Scripts`, allow console usage.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Build_Script()
{
    const string testCode = @"
using System;

namespace Company.Build;

public static class Status
{
    public static void Notify(string message) => Console.WriteLine(message);
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 3. Conditional Compilation (`#if DEBUG`)

- **Problem**: Debug-only diagnostics guarded by `#if DEBUG` still raise warnings.  
- **Mitigation**: If the invocation is inside a conditional compilation region limited to `DEBUG` (or `TRACE`), suppress.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Debug_Only_Code()
{
    const string testCode = @"
using System;

public static class DebugProbe
{
    public static void Emit()
    {
#if DEBUG
        Console.WriteLine(""Debug info"");
#endif
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 4. `ConditionalAttribute` Methods

- **Problem**: Methods decorated with `[Conditional("DEBUG")]` or `[Conditional("TRACE")]` should be allowed to write to console, but the analyzer still reports.  
- **Mitigation**: When the enclosing method has `ConditionalAttribute`, skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Conditional_Method()
{
    const string testCode = @"
using System;
using System.Diagnostics;

public static class DebugLogger
{
    [Conditional(""DEBUG"")]
    public static void Trace(string message) => Console.WriteLine(message);
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 5. Unit Tests / Integration Tests

- **Problem**: Tests sometimes write to console for temporary diagnostics (when `ITestOutputHelper` is unavailable). The analyzer should not block these scenarios.  
- **Mitigation**: If the containing namespace or class ends with `Tests`, skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Test_Code()
{
    const string testCode = @"
using System;
using Xunit;

public sealed class DiagnosticTests
{
    [Fact]
    public void Should_Log()
    {
        Console.WriteLine(""Running test..."");
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 6. Redirected Console Output

- **Problem**: Some tooling temporarily redirects `Console.Out` to capture output, requiring legitimate `Console.WriteLine` calls.  
- **Mitigation**: When `Console.SetOut` or `Console.SetError` is invoked in the same type, allow subsequent writes.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_When_Console_Is_Redirected()
{
    const string testCode = @"
using System;
using System.IO;

public sealed class Capture
{
    public string Run()
    {
        using var writer = new StringWriter();
        Console.SetOut(writer);
        Console.WriteLine(""Captured output"");
        return writer.ToString();
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 7. CLI Prompting (`Console.ReadLine` / `Write`)

- **Problem**: Interactive prompts (`Console.Write("Enter value: ")`) are mandatory for CLI tools.  
- **Mitigation**: If the same method also reads from `Console.ReadLine/ReadKey`, treat the writes as acceptable.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_CLI_Prompt()
{
    const string testCode = @"
using System;

public static class Prompt
{
    public static string Ask()
    {
        Console.Write(""Enter value: "");
        return Console.ReadLine() ?? string.Empty;
    }
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 8. Exception Reporting During Startup

- **Problem**: Minimal apps might log fatal startup exceptions to console before `ILogger` is available. The analyzer should allow `Console.Error.WriteLine` inside top-level `Program`.  
- **Mitigation**: When the call occurs inside a `try/catch` that immediately exits the process (`Environment.Exit`), allow it.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Fatal_Startup_Reporting()
{
    const string testCode = @"
using System;

public static class Bootstrapper
{
    public static void Run()
    {
        try
        {
            Start();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            Environment.Exit(-1);
        }
    }

    private static void Start() => throw new InvalidOperationException();
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 9. `Console.WriteLine` Wrapped in Logging Adapter

- **Problem**: Some adapters implement temporary logging by delegating to console inside classes named `ConsoleLogger`. These should be allowed if they adapt to `ILogger`.  
- **Mitigation**: If the containing type name ends with `ConsoleLogger` or `[AllowConsoleLogging]` attribute is present, skip diagnostics.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Console_Logger_Adapter()
{
    const string testCode = @"
using System;

[AttributeUsage(AttributeTargets.Class)]
public sealed class AllowConsoleLoggingAttribute : Attribute { }

[AllowConsoleLogging]
public sealed class ConsoleLogger
{
    public void Log(string message) => Console.WriteLine(message);
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

### 10. Output in Generated/Tool-Generated Code

- **Problem**: Source generators or T4 templates may produce code with console writes for tracing.  
- **Mitigation**: Skip diagnostics for nodes marked with `[GeneratedCode]`, `[DebuggerNonUserCode]`, or located in `Generated` folders.  
- **Test**:

```csharp
[Fact]
public async Task Should_Not_Report_For_Generated_Code()
{
    const string testCode = @"
using System;
using System.CodeDom.Compiler;

[GeneratedCode(""Tool"", ""1.0"")]
public sealed class GeneratedRunner
{
    public void Execute() => Console.WriteLine(""Generated output"");
}";

    AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseConsoleWriteLineAnalyzer())
        .ShouldBeEmpty();
}
```

## 3. Test-Driven Fix Strategy

1. Add the ten scenarios above under `DoNotUseConsoleWriteLineAnalyzerFalsePositiveTests`.  
2. Keep positive coverage ensuring production services with console writes still trigger EXXER801.  
3. Update analyzer to:
   - Inspect semantic context (project type, namespace patterns, conditional compilation) before reporting.  
   - Honor attributes (`Conditional`, `GeneratedCode`, custom opt-outs).  
   - Detect when console output is redirected, used for prompts, or occurs in fatal-error handlers.  
4. Run analyzer tests to confirm new cases fail before changes and pass after.  
5. Execute `dotnet test` on representative IndTrace projects (build scripts, CLI tools) to measure warning reduction.  
6. Update `AnalyzerReleases.Unshipped.md` summarizing the relaxed enforcement.

## 4. Acceptance Checklist

- [ ] Analyzer updated with CLI/test/debug exemptions and context awareness.  
- [ ] Ten regression tests added and passing.  
- [ ] Builds/tests succeed for analyzer and consumer projects.  
- [ ] EXXER801 warning count reduced in build/test code.  
- [ ] Release notes updated with the new behaviour.
