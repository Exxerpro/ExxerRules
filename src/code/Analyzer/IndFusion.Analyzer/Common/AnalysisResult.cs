using IndFusion.Analyzer.Operations;

namespace IndFusion.Analyzer.Common;

/// <summary>
/// Represents the result of an analysis operation using ExxerRules.Analyzers.Operations.
/// </summary>
public static class AnalysisResult
{
    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to return.</param>
    /// <returns>A successful result containing the value.</returns>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>
    /// Creates a successful result without a value.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success() => Result.Success();

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed result with the error message.</returns>
    public static Result Failure(string errorMessage) => Result.WithFailure(errorMessage);

    /// <summary>
    /// Creates a failed result with an error message for a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A failed result with the error message.</returns>
    public static Result<T> Failure<T>(string errorMessage) => Result<T>.WithFailure(errorMessage);

    /// <summary>
    /// Creates a failed result from an exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed result with the exception details.</returns>
    public static Result Failure(Exception exception) => Result.WithFailure(exception.Message);

    /// <summary>
    /// Creates a failed result from an exception for a typed result.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed result with the exception details.</returns>
    public static Result<T> Failure<T>(Exception exception) => Result<T>.WithFailure(exception.Message);
}
