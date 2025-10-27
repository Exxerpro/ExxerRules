using System.Diagnostics.CodeAnalysis;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public readonly record struct Result<T>
{
    private readonly T? _value;
    private readonly string? _error;

    private Result(T? value, string? error)
    {
        _value = value;
        _error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess => _error is null;

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the value if the operation was successful.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the operation failed.</exception>
    public T Value => IsSuccess ? _value! : throw new InvalidOperationException($"Cannot access value of failed result. Error: {_error}");

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error => _error;

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful result containing the value.</returns>
    public static Result<T> Success(T value) => new(value, null);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result containing the error.</returns>
    public static Result<T> WithFailure(string error) => new(default, error);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result containing the error.</returns>
    public static Result<T> Failure(string error) => WithFailure(error);

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>A successful result containing the value.</returns>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts a result to its value.
    /// </summary>
    /// <param name="result">The result to convert.</param>
    /// <returns>The value if successful, otherwise throws.</returns>
    public static implicit operator T(Result<T> result) => result.Value;
}

/// <summary>
/// Represents the result of an operation that can either succeed or fail without returning a value.
/// </summary>
public readonly record struct Result
{
    private readonly string? _error;

    private Result(string? error)
    {
        _error = error;
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess => _error is null;

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error => _error;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    public static Result Success() => new(null);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result containing the error.</returns>
    public static Result WithFailure(string error) => new(error);

    /// <summary>
    /// Creates a failed result with the specified error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A failed result containing the error.</returns>
    public static Result Failure(string error) => WithFailure(error);
}

/// <summary>
/// Extension methods for Result types.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Maps the value of a successful result to a new value using the specified function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="U">The type of the output value.</typeparam>
    /// <param name="result">The result to map.</param>
    /// <param name="mapper">The function to apply to the value.</param>
    /// <returns>A new result with the mapped value or the same error.</returns>
    public static Result<U> Map<T, U>(this Result<T> result, Func<T, U> mapper)
    {
        return result.IsSuccess 
            ? Result<U>.Success(mapper(result.Value))
            : Result<U>.WithFailure(result.Error!);
    }

    /// <summary>
    /// Binds the value of a successful result to a new result using the specified function.
    /// </summary>
    /// <typeparam name="T">The type of the input value.</typeparam>
    /// <typeparam name="U">The type of the output value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="binder">The function to apply to the value.</param>
    /// <returns>A new result from the binder function or the same error.</returns>
    public static Result<U> Bind<T, U>(this Result<T> result, Func<T, Result<U>> binder)
    {
        return result.IsSuccess 
            ? binder(result.Value)
            : Result<U>.WithFailure(result.Error!);
    }

    /// <summary>
    /// Converts a Result<T> to a Result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>A Result without a value.</returns>
    public static Result ToResult<T>(this Result<T> result)
    {
        return result.IsSuccess 
            ? Result.Success()
            : Result.WithFailure(result.Error!);
    }

    /// <summary>
    /// Validates that the specified parameters are not null.
    /// </summary>
    /// <param name="parameters">The parameters to validate.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public static Result ValidateNotNull(params (object? value, string name)[] parameters)
    {
        var nullParams = parameters
            .Where(p => p.value is null)
            .Select(p => p.name)
            .ToArray();

        return nullParams.Length == 0 
            ? Result.Success()
            : Result.WithFailure($"Null arguments: {string.Join(", ", nullParams)}");
    }
}