using System.Collections.Immutable;
using IndFusion.Analyzers;
using IndFusion.Analyzers.Logging;
using IndFusion.Analyzer.Tests.Testing;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.Logging;

/// <summary>
/// Comprehensive logging tests targeting specific surviving mutants.
/// SRP: Tests all logging patterns and edge cases.
/// </summary>
public class LoggingComprehensiveTests
{
    #region Structured Logging Tests

    /// <summary>
    /// Tests structured logging with different patterns.
    /// </summary>
    [Fact]
    public void Should_NotReportStructuredLoggingPatterns()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class StructuredLoggingService
	{
		private readonly ILogger<StructuredLoggingService> _logger;

		public StructuredLoggingService(ILogger<StructuredLoggingService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, int count)
		{
			_logger.LogInformation(""Processing data {DataLength} with count {Count}"", input?.Length ?? 0, count);
			_logger.LogWarning(""Data validation failed for {DataLength}"", input?.Length ?? 0);
			_logger.LogError(""Processing failed for data {DataLength}"", input?.Length ?? 0);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests structured logging with complex expressions.
    /// </summary>
    [Fact]
    public void Should_NotReportStructuredLoggingWithComplexExpressions()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class ComplexStructuredLoggingService
	{
		private readonly ILogger<ComplexStructuredLoggingService> _logger;

		public ComplexStructuredLoggingService(ILogger<ComplexStructuredLoggingService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, object config)
		{
			_logger.LogInformation(""Processing data {DataLength} with type {Type}"", 
				input?.Length ?? 0, config?.GetType().Name ?? ""unknown"");
			
			_logger.LogWarning(""Data validation failed for {DataLength} with type {Type}"", 
				input?.Length ?? 0, config?.GetType().Name ?? ""unknown"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    #endregion

    #region String Concatenation Tests

    /// <summary>
    /// Tests string concatenation logging patterns.
    /// </summary>
    [Fact]
    public void Should_ReportStringConcatenationPatterns()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class StringConcatenationService
	{
		private readonly ILogger<StringConcatenationService> _logger;

		public StringConcatenationService(ILogger<StringConcatenationService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, int count)
		{
			_logger.LogInformation(""Processing data "" + input + "" with count "" + count);
			_logger.LogWarning(""Data validation failed for "" + input);
			_logger.LogError(""Processing failed for data "" + input + "" with count "" + count);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three concatenation patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    /// <summary>
    /// Tests string concatenation with complex expressions.
    /// </summary>
    [Fact]
    public void Should_ReportStringConcatenationWithComplexExpressions()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class ComplexStringConcatenationService
	{
		private readonly ILogger<ComplexStringConcatenationService> _logger;

		public ComplexStringConcatenationService(ILogger<ComplexStringConcatenationService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, object config)
		{
			_logger.LogInformation(""Processing data "" + input + "" with type "" + config?.GetType().Name);
			_logger.LogWarning(""Data validation failed for "" + input + "" with type "" + config?.GetType().Name);
			_logger.LogError(""Processing failed for data "" + input + "" with type "" + config?.GetType().Name);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three concatenation patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region String Interpolation Tests

    /// <summary>
    /// Tests string interpolation logging patterns.
    /// </summary>
    [Fact]
    public void Should_ReportStringInterpolationPatterns()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class StringInterpolationService
	{
		private readonly ILogger<StringInterpolationService> _logger;

		public StringInterpolationService(ILogger<StringInterpolationService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, int count)
		{
			_logger.LogInformation($""Processing data {input} with count {count}"");
			_logger.LogWarning($""Data validation failed for {input}"");
			_logger.LogError($""Processing failed for data {input} with count {count}"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three interpolation patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    /// <summary>
    /// Tests string interpolation with complex expressions.
    /// </summary>
    [Fact]
    public void Should_ReportStringInterpolationWithComplexExpressions()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class ComplexStringInterpolationService
	{
		private readonly ILogger<ComplexStringInterpolationService> _logger;

		public ComplexStringInterpolationService(ILogger<ComplexStringInterpolationService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, object config)
		{
			_logger.LogInformation($""Processing data {input} with type {config?.GetType().Name}"");
			_logger.LogWarning($""Data validation failed for {input} with type {config?.GetType().Name}"");
			_logger.LogError($""Processing failed for data {input} with type {config?.GetType().Name}"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three interpolation patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region Mixed Logging Pattern Tests

    /// <summary>
    /// Tests mixed logging patterns.
    /// </summary>
    [Fact]
    public void Should_ReportMixedLoggingPatterns()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class MixedLoggingService
	{
		private readonly ILogger<MixedLoggingService> _logger;

		public MixedLoggingService(ILogger<MixedLoggingService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, int count)
		{
			// Structured logging - should not report
			_logger.LogInformation(""Processing data {DataLength} with count {Count}"", input?.Length ?? 0, count);
			
			// String concatenation - should report
			_logger.LogWarning(""Data validation failed for "" + input);
			
			// String interpolation - should report
			_logger.LogError($""Processing failed for data {input} with count {count}"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for concatenation and interpolation
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region Different Logger Types Tests

    /// <summary>
    /// Tests different logger types.
    /// </summary>
    [Fact]
    public void Should_ReportDifferentLoggerTypes()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class DifferentLoggerService
	{
		private readonly ILogger _logger;
		private readonly ILogger<DifferentLoggerService> _typedLogger;

		public DifferentLoggerService(ILogger logger, ILogger<DifferentLoggerService> typedLogger)
		{
			_logger = logger;
			_typedLogger = typedLogger;
		}

		public void ProcessData(string input)
		{
			// Untyped logger with concatenation
			_logger.LogInformation(""Processing data "" + input);
			
			// Typed logger with interpolation
			_typedLogger.LogWarning($""Data validation failed for {input}"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region Log Level Tests

    /// <summary>
    /// Tests different log levels.
    /// </summary>
    [Fact]
    public void Should_ReportDifferentLogLevels()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class LogLevelService
	{
		private readonly ILogger<LogLevelService> _logger;

		public LogLevelService(ILogger<LogLevelService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input)
		{
			_logger.LogTrace(""Processing data "" + input);
			_logger.LogDebug(""Processing data "" + input);
			_logger.LogInformation(""Processing data "" + input);
			_logger.LogWarning(""Processing data "" + input);
			_logger.LogError(""Processing data "" + input);
			_logger.LogCritical(""Processing data "" + input);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(6); // Should report for all log levels
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region Complex Expression Tests

    /// <summary>
    /// Tests complex expressions in logging.
    /// </summary>
    [Fact]
    public void Should_ReportComplexExpressionsInLogging()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class ComplexExpressionService
	{
		private readonly ILogger<ComplexExpressionService> _logger;

		public ComplexExpressionService(ILogger<ComplexExpressionService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, object config)
		{
			// Complex concatenation
			_logger.LogInformation(""Processing data "" + input + "" with type "" + config?.GetType().Name + "" and length "" + input?.Length);
			
			// Complex interpolation
			_logger.LogWarning($""Data validation failed for {input} with type {config?.GetType().Name} and length {input?.Length}"");
			
			// Nested expressions
			_logger.LogError($""Processing failed for data {input} with type {config?.GetType().Name} and length {input?.Length} and hash {input?.GetHashCode()}"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region Method Chain Tests

    /// <summary>
    /// Tests method chains in logging.
    /// </summary>
    [Fact]
    public void Should_ReportMethodChainsInLogging()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class MethodChainService
	{
		private readonly ILogger<MethodChainService> _logger;

		public MethodChainService(ILogger<MethodChainService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input)
		{
			// Method chain in concatenation
			_logger.LogInformation(""Processing data "" + input.ToUpper().Replace(""A"", ""B"").PadLeft(10));
			
			// Method chain in interpolation
			_logger.LogWarning($""Data validation failed for {input.ToUpper().Replace(""A"", ""B"").PadLeft(10)}"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region Conditional Logging Tests

    /// <summary>
    /// Tests conditional logging patterns.
    /// </summary>
    [Fact]
    public void Should_ReportConditionalLoggingPatterns()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class ConditionalLoggingService
	{
		private readonly ILogger<ConditionalLoggingService> _logger;

		public ConditionalLoggingService(ILogger<ConditionalLoggingService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, bool isValid)
		{
			if (isValid)
			{
				_logger.LogInformation(""Processing data "" + input);
			}
			else
			{
				_logger.LogWarning($""Data validation failed for {input}"");
			}
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion

    #region Exception Logging Tests

    /// <summary>
    /// Tests exception logging patterns.
    /// </summary>
    [Fact]
    public void Should_ReportExceptionLoggingPatterns()
    {
        const string testCode = @"
using System;
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class ExceptionLoggingService
	{
		private readonly ILogger<ExceptionLoggingService> _logger;

		public ExceptionLoggingService(ILogger<ExceptionLoggingService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input)
		{
			try
			{
				// Some processing
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ""Processing failed for data "" + input);
				_logger.LogError(ex, $""Processing failed for data {input}"");
			}
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(0); // Exception logging with exception parameter is not reported
    }

    #endregion

    #region Scope Logging Tests

    /// <summary>
    /// Tests scope logging patterns.
    /// </summary>
    [Fact]
    public void Should_ReportScopeLoggingPatterns()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class ScopeLoggingService
	{
		private readonly ILogger<ScopeLoggingService> _logger;

		public ScopeLoggingService(ILogger<ScopeLoggingService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input)
		{
			using (_logger.BeginScope(""Processing data "" + input))
			{
				_logger.LogInformation(""Processing started"");
			}

			using (_logger.BeginScope($""Processing data {input}""))
			{
				_logger.LogInformation(""Processing started"");
			}
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(0); // Scope logging is not reported
    }

    #endregion

    #region Negative Test Cases

    /// <summary>
    /// Tests that structured logging is not reported.
    /// </summary>
    [Fact]
    public void Should_NotReportStructuredLogging()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class StructuredLoggingOnlyService
	{
		private readonly ILogger<StructuredLoggingOnlyService> _logger;

		public StructuredLoggingOnlyService(ILogger<StructuredLoggingOnlyService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input, int count)
		{
			_logger.LogInformation(""Processing data {DataLength} with count {Count}"", input?.Length ?? 0, count);
			_logger.LogWarning(""Data validation failed for {DataLength}"", input?.Length ?? 0);
			_logger.LogError(""Processing failed for data {DataLength} with count {Count}"", input?.Length ?? 0, count);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    #endregion

    #region Edge Case Tests

    /// <summary>
    /// Tests edge cases in logging patterns.
    /// </summary>
    [Fact]
    public void Should_HandleEdgeCasesInLogging()
    {
        const string testCode = @"
using Microsoft.Extensions.Logging;

namespace TestProject
{
	public class EdgeCaseLoggingService
	{
		private readonly ILogger<EdgeCaseLoggingService> _logger;

		public EdgeCaseLoggingService(ILogger<EdgeCaseLoggingService> logger)
		{
			_logger = logger;
		}

		public void ProcessData(string input)
		{
			// Empty string concatenation
			_logger.LogInformation("""" + input);
			
			// Single variable interpolation
			_logger.LogWarning($""{input}"");
			
			// Complex nested expressions
			_logger.LogError($""Processing failed for data {input} with length {input?.Length} and hash {input?.GetHashCode()} and type {input?.GetType().Name}"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseStructuredLoggingAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three patterns
        diagnostics.All(d => d.Id == DiagnosticIds.UseStructuredLogging).ShouldBeTrue();
    }

    #endregion
}

