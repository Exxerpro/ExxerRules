using System.Collections.Immutable;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Async;
using IndFusion.Analyzers.Testing;
using IndFusion.Analyzer.Tests.Testing;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Additional test cases for false positive scenarios in various analyzers.
/// </summary>
public class MoreFalsePositiveTests
{
    /// <summary>
    /// Tests that TestNamingConventionAnalyzer reports diagnostics as Info (suggestion) for differently styled test names.
    /// </summary>
    [Fact]
    public void Should_ReportDiagnosticAsInfo_When_TestMethodUsesDifferentNamingConvention()
    {
        const string testCode = @"
using Xunit;

namespace TestProject
{
	public class CalculatorTests
	{
		[Fact]
		public void Add_TwoPositiveNumbers_ReturnsSum()
		{
			// Test implementation
		}

		[Theory]
		public void Subtract_FirstNumberLargerThanSecond_ReturnsPositiveResult()
		{
			// Test implementation
		}
	}
}";

        // Get all diagnostics 
        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new TestNamingConventionAnalyzer());
        // Should report diagnostics but as Info level (suggestions) - not errors or warnings
        diagnostics.Length.ShouldBe(2);
        foreach (var diagnostic in diagnostics)
        {
            diagnostic.Id.ShouldBe(DiagnosticIds.TestNamingConvention);
            diagnostic.Severity.ShouldBe(DiagnosticSeverity.Info); // Info = suggestion, not error/warning
        }
    }

    /// <summary>
    /// Tests that AsyncMethodsShouldAcceptCancellationTokenAnalyzer does not report diagnostic for application code.
    /// </summary>
    [Fact]
    public void Should_NotReportDiagnostic_When_AsyncMethodInApplicationCode()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace MyApplication
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await DoWorkAsync();
		}

		private static async Task DoWorkAsync()
		{
			await Task.Delay(1000);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests that UseConfigureAwaitFalseAnalyzer does not report diagnostic for application code.
    /// </summary>
    [Fact]
    public void Should_NotReportDiagnostic_When_AwaitInApplicationCode()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace MyApplication
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			await DoWorkAsync();
		}

		private static async Task DoWorkAsync()
		{
			await Task.Delay(1000);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests that UseConfigureAwaitFalseAnalyzer reports diagnostic for library code.
    /// </summary>
    [Fact]
    public void Should_ReportDiagnostic_When_AwaitInLibraryCode()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace MyLibrary
{
	public class Calculator
	{
		public async Task<int> AddAsync(int a, int b)
		{
			await Task.Delay(100); // Should suggest ConfigureAwait(false)
			return a + b;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.Length.ShouldBe(1);
        diagnostics[0].Id.ShouldBe(DiagnosticIds.UseConfigureAwaitFalse);
    }
}
