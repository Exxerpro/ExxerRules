using IndQuestResults;
using IndFusion.SemanticRag.Domain.Errors;

namespace IndQuestResults.Operations;

/// <summary>
/// Temporary extensions for error codes with exceptions until IndQuestResults is extended.
/// These extensions provide convenient methods for creating Result instances with error codes,
/// particularly useful for cancellation handling and exception wrapping.
/// </summary>
public static class ResultExtensionsWithErrorCodes
{
	/// <summary>
	/// Creates a cancelled result with an error code.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="errorCode">The error code to use. Defaults to <see cref="ErrorCodes.OperationCancelled"/>.</param>
	/// <returns>A <see cref="Result{T}"/> representing a cancelled operation.</returns>
	public static Result<T> Cancelled<T>(string errorCode = ErrorCodes.OperationCancelled)
	{
		return Result<T>.WithFailure(errorCode);
	}

	/// <summary>
	/// Creates a failure result with an error code and exception details.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="errorCode">The error code to use.</param>
	/// <param name="exception">The exception that caused the failure.</param>
	/// <returns>A <see cref="Result{T}"/> representing the failure with error code and exception message.</returns>
	public static Result<T> WithFailure<T>(string errorCode, Exception exception)
	{
		var errorMessage = $"{errorCode}: {exception.Message}";
		return Result<T>.WithFailure(errorMessage);
	}

	/// <summary>
	/// Creates a failure result with multiple error codes.
	/// </summary>
	/// <typeparam name="T">The type of the result value.</typeparam>
	/// <param name="errorCodes">The error codes to use.</param>
	/// <returns>A <see cref="Result{T}"/> representing the failure with the specified error codes.</returns>
	public static Result<T> WithFailure<T>(params string[] errorCodes)
	{
		return Result<T>.WithFailure(errorCodes);
	}
}

