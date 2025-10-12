  fil# Task: Replace NSubstitute ILogger<T> with Meziantou Xunit Logger

## Goal
Use real xUnit loggers from `Meziantou.Extensions.Logging.Xunit.v3` instead of `NSubstitute` for `ILogger<T> or NulLogger` in all test projects.

## SCope
All Project test on ExxerAI.sln Solution

## Steps
1) Add package and initialize loggers
- Ensure test csproj references `Meziantou.Extensions.Logging.Xunit.v3`.
- Use `ITestOutputHelper` to create a logger per test class:
```csharp
using Meziantou.Extensions.Logging.Xunit;

public class MyTests
{
    private readonly ILogger<MySut> _logger;

    public MyTests(ITestOutputHelper output)
    {
        _logger = new XUnitLogger.CreateLogger<MySut>(output);
    }
}
```

2) Replace usages
- Incorrect:
```csharp
var logger = Substitute.For<ILogger<MySut>>();
```
- Correct:
```csharp
var logger =  XUnitLogger.CreateLogger<MySut>(output);
```
- Optional factory:
```csharp
public static class TestLoggerFactory
{
    public static ILogger<T> Create<T>(ITestOutputHelper output) =>var logger = XUnitLogger.CreateLogger<T>(output);
}
```

## Do 
- Do: Use `XUnitLogger<T>` and structured logging.
- Do: Prefer behavior assertions over exact log message string assertions.

## Don’t
- Don’t: Mock `ILogger<T>` with any mocking library.

## Acceptance Criteria
- No `Substitute.For<ILogger<...>>()` remains in tests.
- All tests compile and logs are visible in xUnit output.
- No warnings related to logger usage.
