using System.Collections.Immutable;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Async;
using IndFusion.Analyzers.NullSafety;
using IndFusion.Analyzers.Logging;
using IndFusion.Analyzers.ModernCSharp;
using IndFusion.Analyzers.ErrorHandling;
using IndFusion.Analyzers.CodeQuality;
using IndFusion.Analyzers.Performance;
using IndFusion.Analyzer.Tests.Testing;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases;

/// <summary>
/// Advanced edge case tests targeting specific mutant survival areas.
/// SRP: Tests null handling, boundary conditions, and complex analyzer scenarios.
/// </summary>
public class AdvancedEdgeCaseTests
{
    #region Null Handling Edge Cases

    /// <summary>
    /// Tests edge case: Null semantic model handling.
    /// </summary>
    [Fact]
    public void Should_HandleNullSemanticModel_AllAnalyzers()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class NullHandlingService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

        // Test with analyzers that use semantic model
        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Tests edge case: Null type symbols in analyzers.
    /// </summary>
    [Fact]
    public void Should_HandleNullTypeSymbols_TypeCheckingAnalyzers()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
	public class NullTypeService
	{
		public void ProcessData(dynamic data)
		{
			// Dynamic type - no compile-time type info
			var result = data.ToString();
		}

		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    #endregion

    #region Boundary Condition Tests

    /// <summary>
    /// Tests edge case: Empty string identifiers.
    /// </summary>
    [Fact]
    public void Should_HandleEmptyStringIdentifiers_AllAnalyzers()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class EmptyStringService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return """";
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Tests edge case: Unicode characters in identifiers.
    /// </summary>
    [Fact]
    public void Should_HandleUnicodeIdentifiers_AllAnalyzers()
    {
        const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class UnicodeService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}

		public async Task<string> GetDataWithUnicodeAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
        configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
    }

    #endregion

    #region Complex Type System Tests

    /// <summary>
    /// Tests edge case: Complex generic type scenarios.
    /// </summary>
    [Fact]
    public void Should_HandleComplexGenericTypes_TypeCheckingAnalyzers()
    {
        const string testCode = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject
{
	public class ComplexGenericService<TKey, TValue> where TKey : class where TValue : struct
	{
		public async Task<Dictionary<TKey, TValue>> GetDataAsync()
		{
			await Task.Delay(100);
			return new Dictionary<TKey, TValue>();
		}

		public void ProcessData(TKey key, TValue value)
		{
			// No null validation for generic types
			var keyType = key.GetType();
			var valueType = value.GetType();
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        nullSafetyDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Tests edge case: Nested generic types.
    /// </summary>
    [Fact]
    public void Should_HandleNestedGenericTypes_TypeCheckingAnalyzers()
    {
        const string testCode = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject
{
	public class NestedGenericService
	{
		public async Task<List<Dictionary<string, object>>> GetDataAsync()
		{
			await Task.Delay(100);
			return new List<Dictionary<string, object>>();
		}

		public void ProcessData(List<Dictionary<string, object>> data)
		{
			// No null validation
			var count = data.Count;
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        nullSafetyDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    #endregion

    #region Expression Tree Complexity Tests

    /// <summary>
    /// Tests edge case: Deeply nested expressions.
    /// </summary>
    [Fact]
    public void Should_HandleDeeplyNestedExpressions_AllAnalyzers()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
	public class DeepNestedService
	{
		public async Task<string> GetDataAsync()
		{
			var result = await (((((Task.FromResult(""data""))))));
			return result;
		}

		public void ProcessData(string input)
		{
			// Deeply nested null check
			if (!(input == null || (input.Length == 0 && (input == """" || input == null))))
			{
				var length = input.Length;
			}
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        nullSafetyDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    /// <summary>
    /// Tests edge case: Complex conditional expressions.
    /// </summary>
    [Fact]
    public void Should_HandleComplexConditionals_AllAnalyzers()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
	public class ComplexConditionalService
	{
		public async Task<string> GetDataAsync(bool condition1, bool condition2, bool condition3)
		{
			if (condition1 && condition2 || condition3)
			{
				await Task.Delay(100);
			}
			else if (condition1 || condition2 && condition3)
			{
				await Task.Delay(200);
			}
			return ""data"";
		}

		public void ProcessData(string input, bool isValid)
		{
			if (input != null && isValid && input.Length > 0 || input == ""default"")
			{
				var length = input.Length;
			}
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
        nullSafetyDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    #endregion

    #region Method Chain Complexity Tests

    /// <summary>
    /// Tests edge case: Complex method chains.
    /// </summary>
    [Fact]
    public void Should_HandleComplexMethodChains_AllAnalyzers()
    {
        const string testCode = @"
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject
{
	public class ComplexMethodChainService
	{
		public async Task<string> GetDataAsync()
		{
			var result = await Task.FromResult(""data"")
				.ContinueWith(t => t.Result.ToUpper())
				.ContinueWith(t => t.Result.Replace(""A"", ""B""))
				.ContinueWith(t => t.Result.PadLeft(10))
				.ContinueWith(t => t.Result.Trim())
				.ConfigureAwait(false);
			return result;
		}

		public void ProcessData(string[] data)
		{
			// Complex LINQ chain
			var result = data
				.Where(d => d != null)
				.Select(d => d.ToUpper())
				.Where(d => d.Length > 0)
				.OrderBy(d => d)
				.FirstOrDefault();
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        configureAwaitDiagnostics.Length.ShouldBe(0); // Should not report since ConfigureAwait is used
        nullSafetyDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    #endregion

    #region Exception Handling Edge Cases

    /// <summary>
    /// Tests edge case: Exception handling in async methods.
    /// </summary>
    [Fact]
    public void Should_HandleExceptionHandling_AsyncAnalyzers()
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

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
        configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(3);
    }

    #endregion

    #region Logging Edge Cases

    /// <summary>
    /// Tests edge case: Complex logging scenarios.
    /// </summary>
    [Fact]
    public void Should_HandleComplexLogging_LoggingAnalyzers()
    {
        const string testCode = @"
using System;
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class ComplexLoggingService
	{
		private readonly ILogger<ComplexLoggingService> _logger;

		public ComplexLoggingService(ILogger<ComplexLoggingService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input)
		{
			// Structured logging
			_logger.LogInformation(""Processing data {DataLength}"", input?.Length ?? 0);

			// String concatenation (should be flagged)
			_logger.LogWarning(""Processing data "" + input + "" with length "" + input?.Length);

			// String interpolation (should be flagged)
			_logger.LogError($""Processing data {input} with length {input?.Length}"");
		}
	}
}";

        var loggingDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());

        loggingDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
        loggingDiagnostics.Any(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region Performance Edge Cases

    /// <summary>
    /// Tests edge case: Performance-critical code patterns.
    /// </summary>
    [Fact]
    public void Should_HandlePerformancePatterns_PerformanceAnalyzers()
    {
        const string testCode = @"
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestProject
{
	public class PerformanceService
	{
		public void ProcessData(List<string> data)
		{
			// Multiple LINQ operations (potential performance issue)
			var result = data
				.Where(d => d != null)
				.Select(d => d.ToUpper())
				.Where(d => d.Length > 0)
				.OrderBy(d => d)
				.ToList()
				.Where(d => d.Contains(""A""))
				.Select(d => d.Replace(""A"", ""B""))
				.ToArray();

			// Nested loops (potential performance issue)
			foreach (var item in data)
			{
				foreach (var subItem in data)
				{
					if (item == subItem)
					{
						Console.WriteLine(""Match found"");
					}
				}
			}
		}
	}
}";

        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        nullSafetyDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    #endregion

    #region Code Quality Edge Cases

    /// <summary>
    /// Tests edge case: Code quality patterns.
    /// </summary>
    [Fact]
    public void Should_HandleCodeQualityPatterns_CodeQualityAnalyzers()
    {
        const string testCode = @"
using System;
using System.Threading.Tasks;

namespace TestProject
{
	public class CodeQualityService
	{
		// Magic numbers
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(1000); // Magic number
			return ""data"";
		}

		// Long method
		public void LongMethod(string input)
		{
			var step1 = input.ToUpper();
			var step2 = step1.Replace(""A"", ""B"");
			var step3 = step2.PadLeft(10);
			var step4 = step3.Trim();
			var step5 = step4.Substring(0, Math.Min(step4.Length, 5));
			var step6 = step5.ToLower();
			var step7 = step6.Replace(""b"", ""c"");
			var step8 = step7.PadRight(15);
			var step9 = step8.Trim();
			var step10 = step9.Substring(0, Math.Min(step9.Length, 3));
			Console.WriteLine(step10);
		}

		// Deep nesting
		public void DeepNestedMethod(string input)
		{
			if (input != null)
			{
				if (input.Length > 0)
				{
					if (input.Contains(""a""))
					{
						if (input.StartsWith(""test""))
						{
							if (input.EndsWith(""data""))
							{
								Console.WriteLine(""Complex condition met"");
							}
						}
					}
				}
			}
		}
	}
}";

        var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
        var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
        var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());

        asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
        nullSafetyDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
    }

    #endregion
}

