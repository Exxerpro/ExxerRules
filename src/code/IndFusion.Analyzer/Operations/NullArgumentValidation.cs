namespace IndFusion.Analyzers.Operations;

/// <summary>
/// Provides factory methods for creating <see cref="Result"/> instances that capture null argument validation failures.
/// </summary>
public static class NullArgumentValidation
{
    /// <summary>
    /// Creates a generic failure result for a single null argument.
    /// </summary>
    /// <typeparam name="T">The type carried by the result.</typeparam>
    /// <param name="parameterName">The name of the parameter that was <c>null</c>.</param>
    /// <param name="message">An optional custom error message.</param>
    /// <returns>A failed result containing the null argument error.</returns>
    public static Result<T> Failure<T>(string parameterName, string? message = null)
    {
        var error = new NullArgumentError(parameterName, message);
        return Result<T>.WithFailure(error.ToString());
    }

    /// <summary>
    /// Creates a generic failure result for multiple null arguments.
    /// </summary>
    /// <typeparam name="T">The type carried by the result.</typeparam>
    /// <param name="parameterNames">The names of the parameters that were <c>null</c>.</param>
    /// <returns>A failed result describing each null argument error.</returns>
    public static Result<T> Failure<T>(params string[] parameterNames)
    {
        var error = new MultipleNullArgumentsError(parameterNames);
        return Result<T>.WithFailure(error.ToString());
    }

    /// <summary>
    /// Creates a non-generic failure result for a single null argument.
    /// </summary>
    /// <param name="parameterName">The name of the parameter that was <c>null</c>.</param>
    /// <param name="message">An optional custom error message.</param>
    /// <returns>A failed result containing the null argument error.</returns>
    public static Result Failure(string parameterName, string? message = null)
    {
        var error = new NullArgumentError(parameterName, message);
        return Result.WithFailure(error.ToString());
    }

    /// <summary>
    /// Creates a non-generic failure result for multiple null arguments.
    /// </summary>
    /// <param name="parameterNames">The names of the parameters that were <c>null</c>.</param>
    /// <returns>A failed result describing each null argument error.</returns>
    public static Result Failure(params string[] parameterNames)
    {
        var error = new MultipleNullArgumentsError(parameterNames);
        return Result.WithFailure(error.ToString());
    }
}
