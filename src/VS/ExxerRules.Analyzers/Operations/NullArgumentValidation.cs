namespace ExxerRules.Analyzers.Operations;

/// <summary>
/// Static factory for creating null argument validation results
/// </summary>
public static class NullArgumentValidation
{
	/// <summary>
	/// Creates a failure result for a single null argument
	/// </summary>
	/// <typeparam name="T">Result type</typeparam>
	/// <param name="parameterName">Name of the null parameter</param>
	/// <param name="message">Optional error message</param>
	/// <returns>Failed result with null argument error</returns>
	public static Result<T> Failure<T>(string parameterName, string? message = null)
	{
		var error = new NullArgumentError(parameterName, message);
		return Result<T>.WithFailure(error.ToString());
	}

	/// <summary>
	/// Creates a failure result for multiple null arguments
	/// </summary>
	/// <typeparam name="T">Result type</typeparam>
	/// <param name="parameterNames">Names of null parameters</param>
	/// <returns>Failed result with multiple null argument errors</returns>
	public static Result<T> Failure<T>(params string[] parameterNames)
	{
		var error = new MultipleNullArgumentsError(parameterNames);
		return Result<T>.WithFailure(error.ToString());
	}

	/// <summary>
	/// Creates a non-generic failure result for a single null argument
	/// </summary>
	/// <param name="parameterName">Name of the null parameter</param>
	/// <param name="message">Optional error message</param>
	/// <returns>Failed result with null argument error</returns>
	public static Result Failure(string parameterName, string? message = null)
	{
		var error = new NullArgumentError(parameterName, message);
		return Result.WithFailure(error.ToString());
	}

	/// <summary>
	/// Creates a non-generic failure result for multiple null arguments
	/// </summary>
	/// <param name="parameterNames">Names of null parameters</param>
	/// <returns>Failed result with multiple null argument errors</returns>
	public static Result Failure(params string[] parameterNames)
	{
		var error = new MultipleNullArgumentsError(parameterNames);
		return Result.WithFailure(error.ToString());
	}
}
