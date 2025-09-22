using System.Collections.Immutable;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Async;
using IndFusion.Analyzer.Tests.Testing;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Async;

/// <summary>
/// Comprehensive async tests targeting specific surviving mutants.
/// SRP: Tests all async patterns and edge cases.
/// </summary>
public class AsyncComprehensiveTests
{
    #region CancellationToken Parameter Tests

    /// <summary>
    /// Tests async methods with CancellationToken parameters.
    /// </summary>
    [Fact]
    public void Should_NotReportAsyncMethodsWithCancellationToken()
    {
        const string testCode = @"
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject
{
	public class CancellationTokenService
	{
		public async Task<string> GetDataAsync(CancellationToken cancellationToken = default)
		{
			await Task.Delay(100, cancellationToken);
			return ""data"";
		}

		public async Task<object> ProcessDataAsync(object input, CancellationToken cancellationToken)
		{
			await Task.Delay(100, cancellationToken);
			return input;
		}

		public async Task<bool> ValidateDataAsync(string data, CancellationToken cancellationToken = default)
		{
			await Task.Delay(100, cancellationToken);
			return !string.IsNullOrEmpty(data);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests async methods with different CancellationToken parameter positions.
    /// </summary>
    [Fact]
    public void Should_NotReportAsyncMethodsWithCancellationTokenInDifferentPositions()
    {
        const string testCode = @"
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject
{
	public class CancellationTokenPositionService
	{
		public async Task<string> GetDataAsync(CancellationToken cancellationToken, string input)
		{
			await Task.Delay(100, cancellationToken);
			return input.ToUpper();
		}

		public async Task<int> ProcessDataAsync(string input, CancellationToken cancellationToken, bool flag)
		{
			await Task.Delay(100, cancellationToken);
			return input.Length;
		}

		public async Task<object> ValidateDataAsync(bool flag, string data, CancellationToken cancellationToken)
		{
			await Task.Delay(100, cancellationToken);
			return data;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    #endregion

    #region ConfigureAwait Tests

    /// <summary>
    /// Tests ConfigureAwait with different patterns.
    /// </summary>
    [Fact]
    public void Should_NotReportConfigureAwaitPatterns()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class ConfigureAwaitService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100).ConfigureAwait(false);
			return ""data"";
		}

		public async Task<object> ProcessDataAsync()
		{
			var result = await Task.FromResult(""data"").ConfigureAwait(false);
			return result;
		}

		public async Task<bool> ValidateDataAsync()
		{
			var task = Task.FromResult(true);
			var result = await task.ConfigureAwait(false);
			return result;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests ConfigureAwait with complex method chains.
    /// </summary>
    [Fact]
    public void Should_NotReportConfigureAwaitWithComplexChains()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class ComplexConfigureAwaitService
	{
		public async Task<string> GetDataAsync()
		{
			var result = await Task.FromResult(""data"")
				.ContinueWith(t => t.Result.ToUpper())
				.ContinueWith(t => t.Result.Replace(""A"", ""B""))
				.ConfigureAwait(false);
			return result;
		}

		public async Task<object> ProcessDataAsync()
		{
			var result = await Task.FromResult(""data"")
				.ContinueWith(t => t.Result.PadLeft(10))
				.ContinueWith(t => t.Result.Trim())
				.ConfigureAwait(false);
			return result;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    #endregion

    #region Skippable Method Tests

    /// <summary>
    /// Tests that skippable async methods are not reported.
    /// </summary>
    [Fact]
    public void Should_NotReportSkippableAsyncMethods()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
	public class SkippableAsyncService
	{
		// Main method - should be skipped
		public static async Task Main(string[] args)
		{
			await Task.Delay(100);
		}

		// Event handler - should be skipped
		protected virtual async Task OnDataChangedAsync(EventArgs e)
		{
			await Task.Delay(100);
		}

		// Interface method - should be skipped
		public interface IAsyncDataProcessor
		{
			Task<string> ProcessDataAsync(string input);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests that async void methods are not reported.
    /// </summary>
    [Fact]
    public void Should_NotReportAsyncVoidMethods()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
	public class AsyncVoidService
	{
		public async void EventHandlerAsync(object sender, EventArgs e)
		{
			await Task.Delay(100);
		}

		public async void OnDataChangedAsync(EventArgs e)
		{
			await Task.Delay(100);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    #endregion

    #region Application Code Tests

    /// <summary>
    /// Tests that application code is not reported.
    /// </summary>
    [Fact]
    public void Should_NotReportApplicationCode()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace Program
{
	public class Program
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}

namespace App
{
	public class AppService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}

namespace ConsoleApp
{
	public class ConsoleAppService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    #endregion

    #region Complex Async Pattern Tests

    /// <summary>
    /// Tests complex async patterns with multiple awaits.
    /// </summary>
    [Fact]
    public void Should_ReportComplexAsyncPatternsWithoutCancellationToken()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class ComplexAsyncService
	{
		public async Task<string> GetDataAsync()
		{
			var data = await Task.FromResult(""data"");
			var processed = await Task.FromResult(data.ToUpper());
			var result = await Task.FromResult(processed.Replace(""A"", ""B""));
			return result;
		}

		public async Task<object> ProcessDataAsync()
		{
			await Task.Delay(100);
			await Task.Delay(200);
			await Task.Delay(300);
			return ""processed"";
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both methods
        diagnostics.All(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
    }

    /// <summary>
    /// Tests complex async patterns with ConfigureAwait missing.
    /// </summary>
    [Fact]
    public void Should_ReportComplexAsyncPatternsWithoutConfigureAwait()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class ComplexAsyncService
	{
		public async Task<string> GetDataAsync()
		{
			var data = await Task.FromResult(""data"");
			var processed = await Task.FromResult(data.ToUpper());
			var result = await Task.FromResult(processed.Replace(""A"", ""B""));
			return result;
		}

		public async Task<object> ProcessDataAsync()
		{
			await Task.Delay(100);
			await Task.Delay(200);
			await Task.Delay(300);
			return ""processed"";
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.Length.ShouldBeGreaterThanOrEqualTo(6); // Should report for all await expressions
        diagnostics.All(d => d.Id == DiagnosticIds.UseConfigureAwaitFalse).ShouldBeTrue();
    }

    #endregion

    #region Generic Async Method Tests

    /// <summary>
    /// Tests generic async methods.
    /// </summary>
    [Fact]
    public void Should_ReportGenericAsyncMethodsWithoutCancellationToken()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class GenericAsyncService<T>
	{
		public async Task<T> GetDataAsync()
		{
			await Task.Delay(100);
			return default(T);
		}

		public async Task<TResult> ProcessDataAsync<TResult>(T input)
		{
			await Task.Delay(100);
			return default(TResult);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both methods
        diagnostics.All(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
    }

    /// <summary>
    /// Tests generic async methods with CancellationToken.
    /// </summary>
    [Fact]
    public void Should_NotReportGenericAsyncMethodsWithCancellationToken()
    {
        const string testCode = @"
using System.Threading;
using System.Threading.Tasks;

namespace TestProject
{
	public class GenericAsyncService<T>
	{
		public async Task<T> GetDataAsync(CancellationToken cancellationToken = default)
		{
			await Task.Delay(100, cancellationToken);
			return default(T);
		}

		public async Task<TResult> ProcessDataAsync<TResult>(T input, CancellationToken cancellationToken)
		{
			await Task.Delay(100, cancellationToken);
			return default(TResult);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    #endregion

    #region Exception Handling Tests

    /// <summary>
    /// Tests async methods with exception handling.
    /// </summary>
    [Fact]
    public void Should_ReportAsyncMethodsWithExceptionHandlingWithoutCancellationToken()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
	public class ExceptionHandlingService
	{
		public async Task<string> GetDataAsync()
		{
			try
			{
				await Task.Delay(100);
				return ""data"";
			}
			catch (Exception ex)
			{
				await Task.Delay(50);
				throw;
			}
			finally
			{
				await Task.Delay(25);
			}
		}

		public async Task<string> GetDataWithCancellationAsync()
		{
			try
			{
				await Task.Delay(100);
				return ""data"";
			}
			catch (OperationCanceledException)
			{
				return ""cancelled"";
			}
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both methods
        diagnostics.All(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
    }

    #endregion

    #region Nested Async Method Tests

    /// <summary>
    /// Tests nested async methods.
    /// </summary>
    [Fact]
    public void Should_ReportNestedAsyncMethodsWithoutCancellationToken()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class OuterClass
	{
		public class InnerClass
		{
			public async Task<string> GetDataAsync()
			{
				await Task.Delay(100);
				return ""data"";
			}
		}

		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both methods
        diagnostics.All(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
    }

    #endregion

    #region Record Type Tests

    /// <summary>
    /// Tests async methods in record types.
    /// </summary>
    [Fact]
    public void Should_ReportAsyncMethodsInRecordTypesWithoutCancellationToken()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public record AsyncRecord
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}

	public record AsyncRecordWithCancellationToken
	{
		public async Task<string> GetDataAsync(System.Threading.CancellationToken cancellationToken = default)
		{
			await Task.Delay(100, cancellationToken);
			return ""data"";
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(1); // Should report only for the first record
        diagnostics[0].Id.ShouldBe(DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken);
    }

    #endregion

    #region Partial Class Tests

    /// <summary>
    /// Tests async methods in partial classes.
    /// </summary>
    [Fact]
    public void Should_ReportAsyncMethodsInPartialClassesWithoutCancellationToken()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public partial class PartialAsyncService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}

	public partial class PartialAsyncService
	{
		public async Task<string> ProcessDataAsync()
		{
			await Task.Delay(100);
			return ""processed"";
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both methods
        diagnostics.All(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
    }

    #endregion

    #region Interface Implementation Tests

    /// <summary>
    /// Tests async methods in interface implementations.
    /// </summary>
    [Fact]
    public void Should_ReportAsyncMethodsInInterfaceImplementationsWithoutCancellationToken()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public interface IAsyncService
	{
		Task<string> GetDataAsync();
	}

	public class AsyncService : IAsyncService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}

	public class ExplicitAsyncService : IAsyncService
	{
		async Task<string> IAsyncService.GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both implementations
        diagnostics.All(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
    }

    #endregion

    #region Negative Test Cases

    /// <summary>
    /// Tests that methods without CancellationToken are reported.
    /// </summary>
    [Fact]
    public void Should_ReportMethodsWithoutCancellationToken()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class NoCancellationTokenService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}

		public async Task<object> ProcessDataAsync(object input)
		{
			await Task.Delay(100);
			return input;
		}

		public async Task<bool> ValidateDataAsync(string data)
		{
			await Task.Delay(100);
			return !string.IsNullOrEmpty(data);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three methods
        diagnostics.All(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
    }

    /// <summary>
    /// Tests that methods without ConfigureAwait are reported.
    /// </summary>
    [Fact]
    public void Should_ReportMethodsWithoutConfigureAwait()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class NoConfigureAwaitService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}

		public async Task<object> ProcessDataAsync()
		{
			var result = await Task.FromResult(""data"");
			return result;
		}

		public async Task<bool> ValidateDataAsync()
		{
			var task = Task.FromResult(true);
			var result = await task;
			return result;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three await expressions
        diagnostics.All(d => d.Id == DiagnosticIds.UseConfigureAwaitFalse).ShouldBeTrue();
    }

    #endregion
}


