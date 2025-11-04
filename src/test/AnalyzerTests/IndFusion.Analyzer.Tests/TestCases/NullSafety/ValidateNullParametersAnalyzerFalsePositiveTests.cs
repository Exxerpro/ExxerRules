using System.Threading.Tasks;
using IndFusion.Analyzer.NullSafety;
using IndFusion.Analyzer.Tests.Testing;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.NullSafety;

/// <summary>
/// Tests for ValidateNullParametersAnalyzer false-positive mitigation scenarios.
/// Based on Analyzer200Specs.md enhancement opportunities.
/// </summary>
public class ValidateNullParametersAnalyzerFalsePositiveTests
{
    /// <summary>
    /// Tests that value-type parameters are not flagged for null validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Value_Types()
    {
        const string testCode = @"
public static class Calculator
{
    public static int Add(int left, int right) => left + right;
    public static bool IsValid(decimal amount) => amount > 0;
    public static DateTime GetDate(Guid id) => DateTime.Now;
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that CancellationToken parameters are exempt from null validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_CancellationToken()
    {
        const string testCode = @"
using System.Threading;
using System.Threading.Tasks;

public static class Worker
{
    public static Task RunAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public static void Process(CancellationToken ct) { }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that service provider and logger dependencies are exempt from null validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_ServiceProvider()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that expression-bodied members with guard helpers are recognized as validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Expression_Bodied_Guard()
    {
        const string testCode = @"
using Ardalis.GuardClauses;

public static class Guarded
{
    public static string Echo(string value) => Guard.Against.Null(value, nameof(value));
    public static object Process(object input) => Guard.Against.Null(input, nameof(input));
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that extension method guard patterns are recognized as validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Extension_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that optional parameters with default values are exempt from null validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Optional_String()
    {
        const string testCode = @"
public static class Formatter
{
    public static string ToDisplay(string? label = null) => label ?? ""(none)"";
    public static string Format(string? prefix = null, string? suffix = null) => $""{prefix}content{suffix}"";
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that params arrays are exempt from null validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Params_Array()
    {
        const string testCode = @"
public static class PathBuilder
{
    public static string Combine(params string[] segments) =>
        string.Join('/', segments);
    public static string Join(params object[] items) =>
        string.Join("", "", items);
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that record primary constructors with inline validation are recognized.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Record_Primary_Constructor_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that local functions with guards are recognized as validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Local_Function_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that methods returning Result.Failure for null parameters are recognized as validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Result_Failure_Guard()
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

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that nullable value types are exempt from null validation.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Nullable_Value_Types()
    {
        const string testCode = @"
public static class NullableProcessor
{
    public static int? Process(int? value) => value;
    public static DateTime? GetDate(DateTime? date) => date;
    public static bool? IsValid(bool? flag) => flag;
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that methods with null-coalescing operators are recognized as handling nulls.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Null_Coalescing_Handling()
    {
        const string testCode = @"
public static class NullHandler
{
    public static string Process(string? input) => input ?? ""default"";
    public static object Handle(object? value) => value ?? new object();
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    /// <summary>
    /// Tests that methods with null-conditional operators are recognized as handling nulls.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Null_Conditional_Handling()
    {
        const string testCode = @"
public static class ConditionalHandler
{
    public static int GetLength(string? text) => text?.Length ?? 0;
    public static string GetName(object? obj) => obj?.ToString() ?? ""unknown"";
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty();
    }

    // Positive control tests - these SHOULD be flagged

    /// <summary>
    /// Tests that methods without null validation are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Missing_Validation()
    {
        const string testCode = @"
public static class Unvalidated
{
    public static void Process(string input)
    {
        // No validation - should be flagged
        var length = input.Length;
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.ValidateNullParameters);
    }

    /// <summary>
    /// Tests that methods with incomplete validation are still flagged (positive control).
    /// </summary>
    [Fact]
    public void Should_Report_For_Incomplete_Validation()
    {
        const string testCode = @"
public static class PartiallyValidated
{
    public static void Process(string input, object data)
    {
        if (input is null) throw new ArgumentNullException(nameof(input));
        // Missing validation for 'data' - should be flagged
        var result = data.ToString();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldNotBeEmpty();
        diagnostics.ShouldAllBe(d => d.Id == DiagnosticIds.ValidateNullParameters);
    }
}
