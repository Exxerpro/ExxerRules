using IndFusion.Analyzers.ErrorHandling;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Regression tests ensuring EXXER001 (UseResultPatternAnalyzer) avoids false positives per spec.
/// </summary>
public sealed class UseResultPatternAnalyzerFalsePositiveTests
{
    /// <summary>
    /// Allows domain guard methods returning bool and throwing guard exceptions.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Domain_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows configuration guard inside Program-like classes.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Program_Config_Guard()
    {
        const string testCode = @"
using System;

public static class Program
{
    public static string ResolveConnection(string? connectionString) =>
        connectionString ?? throw new InvalidOperationException(""Connection string not found."");
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows Identity scaffolding style throws in Components namespaces.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Identity_Scaffolding()
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
            throw new NotSupportedException(""Email support required."");
        }
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows Guard/ThrowHelper patterns intentionally throwing.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Guard_Helper()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows value-object style invariant enforcement via exceptions.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_ValueObject_Invariant()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows property null-guard with '?? throw' pattern.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Property_Null_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows private helpers that throw inside test classes.
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
    public void Should_Throw() => Assert.Throws<InvalidOperationException>(() => Helper());

    private static void Helper()
    {
        throw new InvalidOperationException();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows background workers returning Task to throw fatal errors.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Background_Task_Throws()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows local validation functions that throw guard exceptions.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_Local_Function_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Allows opt-out via AllowExceptions attribute at class scope.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Allows_OptOut_Attribute()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Positive control: should report when throwing in non-Result method without exemptions.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Analyzer_Reports_When_No_Mitigation_Applies()
    {
        const string testCode = @"
using System;

public sealed class Service
{
    public int Compute()
    {
        throw new InvalidOperationException();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseResultPatternAnalyzer());
        diagnostics.Length.ShouldBe(1);
    }
}
