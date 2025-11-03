using IndFusion.SemanticRag.Domain.Errors;
using IndQuestResults;
using Shouldly;

namespace IndFusion.SemanticRag.Tests.Unit.Helpers;

/// <summary>
/// Extension methods for asserting Result&lt;T&gt; patterns in tests.
/// </summary>
public static class ResultAssertions
{
	/// <summary>
	/// Asserts that a Result&lt;T&gt; is successful and has a non-null value.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="result">The result to assert.</param>
	public static void ShouldSucceed<T>(this Result<T> result)
	{
		result.IsSuccess.ShouldBeTrue($"Expected result to be successful, but it failed with error: {result.Error}");
		
		// Only check for null if T is a reference type (value types can't be null)
		if (!typeof(T).IsValueType)
		{
			((object?)result.Value).ShouldNotBeNull("Result value should not be null when successful");
		}
	}

	/// <summary>
	/// Asserts that a Result&lt;T&gt; is successful and has a specific value.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="result">The result to assert.</param>
	/// <param name="expectedValue">The expected value.</param>
	public static void ShouldSucceed<T>(this Result<T> result, T expectedValue)
	{
		result.ShouldSucceed();
		result.Value.ShouldBe(expectedValue);
	}

	/// <summary>
	/// Asserts that a Result&lt;T&gt; is a failure.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="result">The result to assert.</param>
	public static void ShouldFail<T>(this Result<T> result)
	{
		result.IsFailure.ShouldBeTrue("Expected result to be a failure, but it was successful");
		result.Error.ShouldNotBeNullOrEmpty("Result error should not be null or empty when failed");
	}

	/// <summary>
	/// Asserts that a Result&lt;T&gt; is a failure with a specific error code.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="result">The result to assert.</param>
	/// <param name="expectedErrorCode">The expected error code.</param>
	public static void ShouldFailWith<T>(this Result<T> result, string expectedErrorCode)
	{
		result.ShouldFail();
		result.Error.ShouldBe(expectedErrorCode, $"Expected error code '{expectedErrorCode}', but got '{result.Error}'");
	}

	/// <summary>
	/// Asserts that a Result (non-generic) is successful.
	/// </summary>
	/// <param name="result">The result to assert.</param>
	public static void ShouldSucceed(this Result result)
	{
		result.IsSuccess.ShouldBeTrue($"Expected result to be successful, but it failed with error: {result.Error}");
	}

	/// <summary>
	/// Asserts that a Result (non-generic) is a failure.
	/// </summary>
	/// <param name="result">The result to assert.</param>
	public static void ShouldFail(this Result result)
	{
		result.IsFailure.ShouldBeTrue("Expected result to be a failure, but it was successful");
		result.Error.ShouldNotBeNullOrEmpty("Result error should not be null or empty when failed");
	}

	/// <summary>
	/// Asserts that a Result (non-generic) is a failure with a specific error code.
	/// </summary>
	/// <param name="result">The result to assert.</param>
	/// <param name="expectedErrorCode">The expected error code.</param>
	public static void ShouldFailWith(this Result result, string expectedErrorCode)
	{
		result.ShouldFail();
		result.Error.ShouldBe(expectedErrorCode, $"Expected error code '{expectedErrorCode}', but got '{result.Error}'");
	}

	/// <summary>
	/// Asserts that a Result&lt;T&gt; represents a cancelled operation.
	/// Verifies that the result is a failure with OperationCancelled error code.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="result">The result to assert.</param>
	public static void ShouldBeCancelled<T>(this Result<T> result)
	{
		result.IsFailure.ShouldBeTrue("Expected cancelled result to be a failure");
		result.ShouldFailWith(ErrorCodes.OperationCancelled);
	}

	/// <summary>
	/// Asserts that a Result (non-generic) represents a cancelled operation.
	/// Verifies that the result is a failure with OperationCancelled error code.
	/// </summary>
	/// <param name="result">The result to assert.</param>
	public static void ShouldBeCancelled(this Result result)
	{
		result.IsFailure.ShouldBeTrue("Expected cancelled result to be a failure");
		result.ShouldFailWith(ErrorCodes.OperationCancelled);
	}
}

