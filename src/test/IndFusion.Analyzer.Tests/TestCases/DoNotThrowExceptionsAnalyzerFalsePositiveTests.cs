using IndFusion.Analyzers.FunctionalPatterns;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Regression tests ensuring EXXER003 (DoNotThrowExceptionsAnalyzer) avoids false positives per spec.
/// </summary>
public sealed class DoNotThrowExceptionsAnalyzerFalsePositiveTests
{
    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for methods that guard against null arguments by throwing
    /// an ArgumentNullException.
    /// </summary>
    /// <remarks>This test ensures that the analyzer correctly recognizes standard argument null checks as
    /// valid and does not produce false positives when an ArgumentNullException is thrown for null
    /// parameters.</remarks>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_ArgumentNull_Guards()
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
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for null-coalescing throw expressions
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_NullCoalescing_Throw()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for range guard checks that throw ArgumentOutOfRangeException.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_Range_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for switch expressions that throw ArgumentOutOfRangeException in the default case.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_Switch_Default_Throw()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for domain-specific validation exceptions.
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_Domain_Validation_Exception()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for exceptions thrown in a constructor to enforce
    /// invariants.
    /// </summary>
    /// <remarks>This test ensures that the analyzer correctly allows exceptions used for input validation
    /// within constructors, which is a common and recommended practice for enforcing class invariants.</remarks>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_Constructor_Invariant()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for exception throwing in a factory method used for input
    /// validation.
    /// </summary>
    /// <remarks>This test ensures that the DoNotThrowExceptionsAnalyzer does not flag exception usage in
    /// factory methods where argument validation is performed, as such patterns are considered acceptable in .NET
    /// design guidelines.</remarks>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_Validation_In_Factory()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for expression-bodied properties that use guard clauses
    /// with exception throwing.
    /// </summary>
    /// <remarks>This test ensures that properties using expression-bodied syntax with guard clauses, such as
    /// throwing an exception when a required value is not configured, are not incorrectly flagged by the analyzer. The
    /// scenario reflects a common pattern for enforcing invariants in property getters.</remarks>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_Expression_Bodied_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for defensive throws of NotSupportedException in legacy
    /// adapter scenarios.
    /// </summary>
    /// <remarks>This test ensures that the DoNotThrowExceptionsAnalyzer does not flag code where
    /// NotSupportedException is used to indicate unsupported operations in legacy or read-only adapters, which is
    /// considered an acceptable defensive pattern.</remarks>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_NotSupported_Defensive_Throw()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer does not report diagnostics for exception wrapping scenarios where an exception is caught
    /// </summary>
    [Fact(Timeout = 10000)]
    public void Should_Not_Report_For_Exception_Wrapping()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that the analyzer reports a diagnostic when a generic exception is thrown in user code.
    /// </summary>
    /// <remarks>This test provides a code sample that throws a generic Exception and asserts that the
    /// analyzer correctly identifies and reports the violation. Use this test to ensure the analyzer enforces best
    /// practices regarding exception types.</remarks>

    [Fact(Timeout = 10000)]
    public void Positive_Control_Should_Report_When_Generic_Exception()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotThrowExceptionsAnalyzer());
        diagnostics.Length.ShouldBe(1);
    }
}