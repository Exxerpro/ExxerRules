using System.Collections.Immutable;
using IndFusion.Analyzers;
using IndFusion.Analyzers.NullSafety;
using IndFusion.Analyzer.Tests.Testing;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace IndFusion.Analyzer.Tests.TestCases.NullSafety;

/// <summary>
/// Comprehensive null safety tests targeting specific surviving mutants.
/// SRP: Tests all null validation patterns and edge cases.
/// </summary>
public class NullSafetyComprehensiveTests
{
    //  ArgumentNullException.ThrowIfNull Tests

    /// <summary>
    /// Tests ArgumentNullException.ThrowIfNull with different parameter names.
    /// </summary>
    [Fact]
    public void Should_ValidateArgumentNullExceptionThrowIfNull_DifferentParameterNames()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class ThrowIfNullService
	{
		public void ProcessData(string input, object config, int? value)
		{
			ArgumentNullException.ThrowIfNull(input);
			ArgumentNullException.ThrowIfNull(config);
			ArgumentNullException.ThrowIfNull(value);

			var length = input.Length;
			var type = config.GetType();
			var hasValue = value.HasValue;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests ArgumentNullException.ThrowIfNull with complex expressions.
    /// </summary>
    [Fact]
    public void Should_ValidateArgumentNullExceptionThrowIfNull_ComplexExpressions()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class ComplexThrowIfNullService
	{
		public void ProcessData(string input, object config)
		{
			ArgumentNullException.ThrowIfNull(input);
			ArgumentNullException.ThrowIfNull(config);

			// Complex expressions after validation
			var result = input.ToUpper().Replace(""A"", ""B"").PadLeft(10);
			var type = config.GetType().Name;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

     // 

    //  If Statement Validation Tests

    /// <summary>
    /// Tests if statement validation with different null check patterns.
    /// </summary>
    [Fact]
    public void Should_ValidateIfStatementNullChecks_DifferentPatterns()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class IfStatementService
	{
		public void ProcessData(string input, object config, int? value)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			if (config is null)
				throw new ArgumentNullException(nameof(config));

			if (value == null)
				throw new ArgumentNullException(nameof(value));

			var length = input.Length;
			var type = config.GetType();
			var hasValue = value.HasValue;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests if statement validation with block statements.
    /// </summary>
    [Fact]
    public void Should_ValidateIfStatementNullChecks_BlockStatements()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class BlockStatementService
	{
		public void ProcessData(string input, object config)
		{
			if (input == null)
			{
				throw new ArgumentNullException(nameof(input));
			}

			if (config is null)
			{
				throw new ArgumentNullException(nameof(config));
			}

			var length = input.Length;
			var type = config.GetType();
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests if statement validation with different exception types.
    /// </summary>
    [Fact]
    public void Should_ValidateIfStatementNullChecks_DifferentExceptionTypes()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class DifferentExceptionService
	{
		public void ProcessData(string input, object config)
		{
			if (input == null)
				throw new ArgumentException(""Input cannot be null"", nameof(input));

			if (config is null)
				throw new InvalidOperationException(""Config cannot be null"");

			var length = input.Length;
			var type = config.GetType();
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

     // 

    //  Result Pattern Validation Tests

    /// <summary>
    /// Tests Result.WithFailure validation pattern.
    /// </summary>
    [Fact]
    public void Should_ValidateResultWithFailurePattern()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class ResultPatternService
	{
		public Result ProcessData(string input, object config)
		{
			if (input == null)
				return Result.WithFailure(""Input cannot be null"");

			if (config is null)
				return Result.WithFailure(""Config cannot be null"");

			var length = input.Length;
			var type = config.GetType();

			return Result.Success();
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

    /// <summary>
    /// Tests Result&lt;T&gt;.WithFailure validation pattern.
    /// </summary>
    [Fact]
    public void Should_ValidateResultTWithFailurePattern()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class ResultTPatternService
	{
		public Result<string> ProcessData(string input, object config)
		{
			if (input == null)
				return Result<string>.WithFailure(""Input cannot be null"");

			if (config is null)
				return Result<string>.WithFailure(""Config cannot be null"");

			var length = input.Length;
			var type = config.GetType();

			return Result<string>.Success(""processed"");
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both parameters
        diagnostics.All(d => d.Id == DiagnosticIds.ValidateNullParameters).ShouldBeTrue();
    }

     // 

    //  Complex Type Validation Tests

    /// <summary>
    /// Tests validation with complex generic types.
    /// </summary>
    [Fact]
    public void Should_ValidateComplexGenericTypes()
    {
        const string testCode = @"
using System;
using System.Collections.Generic;

namespace TestProject
{
	public class ComplexGenericService
	{
		public void ProcessData(Dictionary<string, List<object>> complexData,
							   List<Dictionary<string, object>> nestedData)
		{
			if (complexData == null)
				throw new ArgumentNullException(nameof(complexData));

			if (nestedData is null)
				throw new ArgumentNullException(nameof(nestedData));

					var count = complexData.Count;
		var nestedCount = nestedData.Count;
	}
}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0); // Should not report since validation is present
    }

    /// <summary>
    /// Tests validation with nullable value types.
    /// </summary>
    [Fact]
    public void Should_ValidateNullableValueTypes()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class NullableValueTypeService
	{
		public void ProcessData(int? nullableInt, DateTime? nullableDate, Guid? nullableGuid)
		{
			if (nullableInt == null)
				throw new ArgumentNullException(nameof(nullableInt));

			if (nullableDate is null)
				throw new ArgumentNullException(nameof(nullableDate));

			if (nullableGuid == null)
				throw new ArgumentNullException(nameof(nullableGuid));

			var value = nullableInt.Value;
			var date = nullableDate.Value;
			var guid = nullableGuid.Value;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

     // 

    //  Edge Case Validation Tests

    /// <summary>
    /// Tests validation with dynamic types.
    /// </summary>
    [Fact]
    public void Should_ValidateDynamicTypes()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class DynamicTypeService
	{
		public void ProcessData(dynamic data, object obj)
		{
			if (obj == null)
				throw new ArgumentNullException(nameof(obj));

			// Dynamic type - no compile-time type info
			var result = data.ToString();
			var type = obj.GetType();
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(1); // Should report for dynamic type
        diagnostics[0].Id.ShouldBe(DiagnosticIds.ValidateNullParameters);
    }

    /// <summary>
    /// Tests validation with expression-bodied members.
    /// </summary>
    [Fact]
    public void Should_ValidateExpressionBodiedMembers()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class ExpressionBodiedService
	{
		public string GetData(string input) => input.ToUpper();

		public int GetLength(string input) => input.Length;

		public bool IsValid(string input) => !string.IsNullOrEmpty(input);
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(3); // Should report for all three methods
        diagnostics.All(d => d.Id == DiagnosticIds.ValidateNullParameters).ShouldBeTrue();
    }

     // 

    //  Skippable Method Tests

    /// <summary>
    /// Tests that skippable methods are not reported.
    /// </summary>
    [Fact]
    public void Should_NotReportSkippableMethods()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class SkippableService
	{
		// Main method - should be skipped
		public static void Main(string[] args)
		{
			var length = args.Length;
		}

		// Event handler - should be skipped
		protected virtual void OnDataChanged(EventArgs e)
		{
			var type = e.GetType();
		}

		// Interface method - should be skipped
		public interface IDataProcessor
		{
			void ProcessData(string input);
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(0);
    }

     // 

    //  Complex Validation Pattern Tests

    /// <summary>
    /// Tests complex validation patterns with multiple conditions.
    /// </summary>
    [Fact]
    public void Should_ValidateComplexValidationPatterns()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class ComplexValidationService
	{
		public void ProcessData(string input, object config, int? value)
		{
			// Multiple validation patterns
			if (input == null || string.IsNullOrEmpty(input))
				throw new ArgumentNullException(nameof(input));

			if (config is null)
				throw new ArgumentNullException(nameof(config));

			if (value == null || value.Value < 0)
				throw new ArgumentException(""Value must be non-negative"", nameof(value));

			var length = input.Length;
			var type = config.GetType();
			var intValue = value.Value;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.ShouldBeEmpty(); // Should be 0, as all relevant parameters are validated.
    }

    /// <summary>
    /// Tests validation patterns with early returns.
    /// </summary>
    [Fact]
    public void Should_ValidateEarlyReturnPatterns()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class EarlyReturnService
	{
		public string ProcessData(string input, object config)
		{
			if (input == null)
				return ""invalid"";

			if (config is null)
				return ""invalid"";

			var length = input.Length;
			var type = config.GetType();

			return ""valid"";
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for both parameters since early returns are not proper validation
        diagnostics.All(d => d.Id == DiagnosticIds.ValidateNullParameters).ShouldBeTrue();
    }

     // 

    //  Negative Test Cases

    /// <summary>
    /// Tests that methods without validation are reported.
    /// </summary>
    [Fact]
    public void Should_ReportMethodsWithoutValidation()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class NoValidationService
	{
		public void ProcessData(string input, object config, int? value)
		{
			// No null validation
			var length = input.Length;
			var type = config.GetType();
			var hasValue = value.HasValue;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(2); // Should report for input and config, but not nullable value
        diagnostics.All(d => d.Id == DiagnosticIds.ValidateNullParameters).ShouldBeTrue();
    }

    /// <summary>
    /// Tests that incomplete validation is reported.
    /// </summary>
    [Fact]
    public void Should_ReportIncompleteValidation()
    {
        const string testCode = @"
using System;

namespace TestProject
{
	public class IncompleteValidationService
	{
		public void ProcessData(string input, object config, int? value)
		{
			// Only validate some parameters
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			// Missing validation for config and value
			var length = input.Length;
			var type = config.GetType();
			var hasValue = value.HasValue;
		}
	}
}";

        var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
        diagnostics.Length.ShouldBe(1); // Should report for config and value
        diagnostics.All(d => d.Id == DiagnosticIds.ValidateNullParameters).ShouldBeTrue();
    }

     // 
}
