using IndFusion.Analyzer.Tests.Testing;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Testing;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Test cases for UseShouldly analyzer false-positive mitigation scenarios.
/// </summary>
public class UseShouldlyAnalyzerFalsePositiveTests
{
    /// <summary>
    /// Tests that FluentAssertions usage reports diagnostic.
    /// </summary>
    [Fact]
    public void Should_Report_For_FluentAssertions_Usage()
    {
        const string testCode = @"
using FluentAssertions;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        true.Should().BeTrue();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer());
        diagnostics.Length.ShouldBe(1);
        diagnostics[0].Id.ShouldBe(DiagnosticIds.UseShouldly);
    }

    /// <summary>
    /// Tests that Shouldly usage does not report diagnostic.
    /// </summary>
    [Fact]
    public void Should_Not_Report_For_Shouldly_Usage()
    {
        const string testCode = @"
using Shouldly;
using Xunit;

public class TestClass
{
    [Fact]
    public void TestMethod()
    {
        true.ShouldBeTrue();
    }
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new DoNotUseFluentAssertionsAnalyzer());
        diagnostics.ShouldBeEmpty();
    }
}