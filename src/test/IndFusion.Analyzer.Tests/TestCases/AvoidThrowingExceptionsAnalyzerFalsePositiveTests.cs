using IndFusion.Analyzers.ErrorHandling;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Regression tests ensuring EXXER002 (AvoidThrowingExceptionsAnalyzer) avoids false positives per spec.
/// </summary>
public sealed class AvoidThrowingExceptionsAnalyzerFalsePositiveTests
{
    /// <summary>
    /// Allows null-guard idiom with coalesce throw in constructors.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Null_Guard_Throw()
    {
        const string testCode = @"
using System;

public sealed class Service
{
    private readonly object _dependency;

    public Service(object dependency) =>
        _dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows range guard throws for primitive validation.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Range_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows startup/bootstrap invalid operation when missing config.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Startup_Config_Throws()
    {
        const string testCode = @"
using System;

public static class Startup
{
    public static string ResolveConnection(string? connectionString) =>
        connectionString ?? throw new InvalidOperationException(""Connection string not found."");
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows framework-required NotSupportedException in Identity components.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Identity_Scaffolding_Throws()
    {
        const string testCode = @"
using System;

namespace Sample.Identity.Components.Account;

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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows rethrow in catch blocks (fatal error paths).
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Fatal_Error_Reporting()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows exception wrapping with inner exception in catch.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Exception_Wrapping()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows Guard/ThrowHelper methods to throw intentionally.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_ThrowHelper_Methods()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows value object invariant checks throwing guard exceptions.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_ValueObject_Invariants()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows explicit opt-out via AllowExceptions attribute.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_OptOut_Attribute()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows helper methods that throw inside test/benchmark/spec classes.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Helper_In_Test_Class()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Positive control: should report for generic throw in core method.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Reports_When_No_Mitigation_Applies()
    {
        const string testCode = @"
using System;

public sealed class Service
{
    public void DoWork()
    {
        throw new Exception();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AvoidThrowingExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(1);
    }
}
