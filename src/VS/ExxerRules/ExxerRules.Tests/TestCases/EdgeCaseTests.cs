using System.Collections.Immutable;
using ExxerRules.Analyzers;
using ExxerRules.Analyzers.Async;
using ExxerRules.Analyzers.NullSafety;
using ExxerRules.Analyzers.Logging;
using ExxerRules.Analyzers.ModernCSharp;
using ExxerRules.Analyzers.ErrorHandling;
using ExxerRules.Tests.Testing;
using Microsoft.CodeAnalysis;
using Shouldly;
using Xunit;

namespace ExxerRules.Tests.TestCases;

/// <summary>
/// Comprehensive edge case tests for all analyzers.
/// SRP: Tests boundary conditions, null handling, and complex scenarios.
/// </summary>
public class EdgeCaseTests
{
	#region Async Analyzer Edge Cases

	/// <summary>
	/// Tests edge case: Async method with complex nested namespaces.
	/// </summary>
	[Fact]
	public void Should_HandleComplexNestedNamespaces_AsyncAnalyzer()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace Company.Project.Core.Services.Infrastructure.DataAccess
{
	public class ComplexAsyncService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		diagnostics.Any(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
	}

	/// <summary>
	/// Tests edge case: Async method with partial class declaration.
	/// </summary>
	[Fact]
	public void Should_HandlePartialClass_AsyncAnalyzer()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public partial class PartialService
	{
		public async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		diagnostics.Any(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
	}

	/// <summary>
	/// Tests edge case: Async method with generic constraints.
	/// </summary>
	[Fact]
	public void Should_HandleGenericConstraints_AsyncAnalyzer()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class GenericService<T> where T : class
	{
		public async Task<T> GetDataAsync()
		{
			await Task.Delay(100);
			return default(T);
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		diagnostics.Any(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
	}

	/// <summary>
	/// Tests edge case: Async method with explicit interface implementation.
	/// </summary>
	[Fact]
	public void Should_HandleExplicitInterfaceImplementation_AsyncAnalyzer()
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
		async Task<string> IAsyncService.GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		diagnostics.Any(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
	}

	/// <summary>
	/// Tests edge case: Async method with record types.
	/// </summary>
	[Fact]
	public void Should_HandleRecordTypes_AsyncAnalyzer()
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
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		diagnostics.Any(d => d.Id == DiagnosticIds.AsyncMethodsShouldAcceptCancellationToken).ShouldBeTrue();
	}

	#endregion

	#region ConfigureAwait Analyzer Edge Cases

	/// <summary>
	/// Tests edge case: ConfigureAwait with complex method chains.
	/// </summary>
	[Fact]
	public void Should_HandleComplexMethodChains_ConfigureAwaitAnalyzer()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class ComplexService
	{
		public async Task<string> GetDataAsync()
		{
			var result = await Task.FromResult(""data"")
				.ContinueWith(t => t.Result.ToUpper())
				.ContinueWith(t => t.Result.Replace(""A"", ""B""))
				.ConfigureAwait(false);
			return result;
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
		diagnostics.Length.ShouldBe(0);
	}

	/// <summary>
	/// Tests edge case: ConfigureAwait with conditional expressions.
	/// </summary>
	[Fact]
	public void Should_HandleConditionalExpressions_ConfigureAwaitAnalyzer()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class ConditionalService
	{
		public async Task<string> GetDataAsync(bool useConfigureAwait)
		{
			var task = Task.FromResult(""data"");
			var result = await (useConfigureAwait ? task.ConfigureAwait(false) : task);
			return result;
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
		// Should report diagnostic for the non-ConfigureAwait branch
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
	}

	/// <summary>
	/// Tests edge case: ConfigureAwait with nested await expressions.
	/// </summary>
	[Fact]
	public void Should_HandleNestedAwaitExpressions_ConfigureAwaitAnalyzer()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class NestedService
	{
		public async Task<string> GetDataAsync()
		{
			var innerTask = Task.FromResult(""inner"");
			var outerTask = Task.FromResult(await innerTask);
			await outerTask;
			return ""data"";
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
	}

	#endregion

	#region Null Safety Analyzer Edge Cases

	/// <summary>
	/// Tests edge case: Null validation with complex type patterns.
	/// </summary>
	[Fact]
	public void Should_HandleComplexTypePatterns_NullSafetyAnalyzer()
	{
		const string testCode = @"
using System;
using System.Collections.Generic;

namespace TestProject
{
	public class ComplexTypeService
	{
		public void ProcessData(Dictionary<string, List<object>> complexData)
		{
			// No null validation
			var count = complexData.Count;
		}

		public void ProcessNullableValue(int? nullableValue)
		{
			// No null validation for nullable value type
			var value = nullableValue.Value;
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
	}

	/// <summary>
	/// Tests edge case: Null validation with generic types.
	/// </summary>
	[Fact]
	public void Should_HandleGenericTypes_NullSafetyAnalyzer()
	{
		const string testCode = @"
using System;

namespace TestProject
{
	public class GenericService<T> where T : class
	{
		public void ProcessGenericData(T data)
		{
			// No null validation
			var type = data.GetType();
		}

		public void ProcessGenericList(List<T> dataList)
		{
			// No null validation
			var count = dataList.Count;
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
	}

	/// <summary>
	/// Tests edge case: Null validation with expression-bodied members.
	/// </summary>
	[Fact]
	public void Should_HandleExpressionBodiedMembers_NullSafetyAnalyzer()
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
		diagnostics.Length.ShouldBeGreaterThanOrEqualTo(3);
	}

	/// <summary>
	/// Tests edge case: Null validation with different validation patterns.
	/// </summary>
	[Fact]
	public void Should_HandleDifferentValidationPatterns_NullSafetyAnalyzer()
	{
		const string testCode = @"
using System;

namespace TestProject
{
	public class ValidationPatternService
	{
		public void ProcessWithThrowIfNull(string input)
		{
			ArgumentNullException.ThrowIfNull(input);
			var length = input.Length;
		}

		public void ProcessWithIsNull(string input)
		{
			if (input is null)
				throw new ArgumentNullException(nameof(input));
			var length = input.Length;
		}

		public void ProcessWithEqualsNull(string input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));
			var length = input.Length;
		}

		public void ProcessWithNoValidation(string input)
		{
			var length = input.Length;
		}
	}
}";

		var diagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
		// Should only report for the method without validation
		diagnostics.Length.ShouldBe(1);
		diagnostics[0].Id.ShouldBe(DiagnosticIds.ValidateNullParameters);
	}

	#endregion

	#region Boundary Condition Tests

	/// <summary>
	/// Tests boundary condition: Empty namespace.
	/// </summary>
	[Fact]
	public void Should_HandleEmptyNamespace_AllAnalyzers()
	{
		const string testCode = @"
using System.Threading.Tasks;

public class GlobalClass
{
	public async Task<string> GetDataAsync()
	{
		await Task.Delay(100);
		return ""data"";
	}
}";

		var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
		
		asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
	}

	/// <summary>
	/// Tests boundary condition: Single character identifiers.
	/// </summary>
	[Fact]
	public void Should_HandleSingleCharacterIdentifiers_AllAnalyzers()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class A
	{
		public async Task<string> B()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

		var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
		
		asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
	}

	/// <summary>
	/// Tests boundary condition: Very long identifiers.
	/// </summary>
	[Fact]
	public void Should_HandleVeryLongIdentifiers_AllAnalyzers()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public class VeryLongClassNameThatExceedsNormalNamingConventionsAndMightCauseIssuesWithStringMatchingAlgorithms
	{
		public async Task<string> VeryLongMethodNameThatExceedsNormalNamingConventionsAndMightCauseIssuesWithStringMatchingAlgorithms()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}
}";

		var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
		
		asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
	}

	#endregion

	#region Complex Scenario Tests

	/// <summary>
	/// Tests complex scenario: Multiple analyzers on same code.
	/// </summary>
	[Fact]
	public void Should_HandleMultipleAnalyzers_ComplexScenario()
	{
		const string testCode = @"
using System;
using System.Threading.Tasks;

namespace Company.Project.Core.Services
{
	public class ComplexService
	{
		public async Task<string> ProcessDataAsync(string input, object config)
		{
			// Missing ConfigureAwait
			await Task.Delay(100);
			
			// Missing null validation
			var length = input.Length;
			var type = config.GetType();
			
			return ""processed"";
		}
	}
}";

		var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
		var nullSafetyDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new ValidateNullParametersAnalyzer());
		
		asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(1);
		nullSafetyDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
	}

	/// <summary>
	/// Tests complex scenario: Nested classes and inheritance.
	/// </summary>
	[Fact]
	public void Should_HandleNestedClassesAndInheritance_ComplexScenario()
	{
		const string testCode = @"
using System.Threading.Tasks;

namespace TestProject
{
	public abstract class BaseService
	{
		public abstract Task<string> GetDataAsync();
	}

	public class DerivedService : BaseService
	{
		public override async Task<string> GetDataAsync()
		{
			await Task.Delay(100);
			return ""data"";
		}
	}

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
	}
}";

		var asyncDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new AsyncMethodsShouldAcceptCancellationTokenAnalyzer());
		var configureAwaitDiagnostics = AnalyzerTestHelper.RunAnalyzer(testCode, new UseConfigureAwaitFalseAnalyzer());
		
		asyncDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
		configureAwaitDiagnostics.Length.ShouldBeGreaterThanOrEqualTo(2);
	}

	#endregion
}
