namespace IndFusion.Analyzer.Operations;

#pragma warning disable CS0702
#pragma warning disable EXXER200 // This extension method is designed to be used with Result<T> and Result types, following the Open/Closed Principle for extensibility.

/// <summary>
/// Extension methods for Result&lt;T&gt; following Open/Closed Principle
/// </summary>
public static class ResultExtensions
{
    private const string ErrorNamesNullOrEmpty = "Parameter names cannot be null, empty, or contain empty values.";
    private const string ErrorNameNullOrEmpty = "Parameter name cannot be null or empty.";
    private const string ErrorValidationNullOrEmpty = "Validations cannot be null or empty.";
    private const string ErrorFactoryFunctionNull = "Factory function cannot be null.";
    private const string ErrorValidationNull = "Validations cannot be null.";

    /// <summary>
    /// Creates a result indicating that an operation was cancelled.
    /// </summary>
    /// <returns>A <see cref="Result"/> object representing a cancelled operation.</returns>
    public static Result Cancelled() => Result.WithFailure(ResultErrors.OperationCancelled);

    /// <summary>
    /// Creates a generic result indicating that an operation was cancelled.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <returns>A <see cref="Result{T}"/> representing a cancelled operation.</returns>
    public static Result<T> Cancelled<T>() => Result<T>.WithFailure(ResultErrors.OperationCancelled);

    /// <summary>
    /// Determines whether the result represents a cancelled operation.
    /// </summary>
    /// <param name="result">The result to check.</param>
    /// <returns><c>true</c> if the result contains an operation cancelled error; otherwise, <c>false</c>.</returns>
    public static bool IsCancelled(this Result result) => result != null && result.Errors != null && result.Errors.Any(e => e == ResultErrors.OperationCancelled);

    /// <summary>
    /// Determines whether the generic result represents a cancelled operation.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to check.</param>
    /// <returns><c>true</c> if the result contains an operation cancelled error; otherwise, <c>false</c>.</returns>
    public static bool IsCancelled<T>(this Result<T> result) => result != null && result.Errors != null && result.Errors.Any(e => e == ResultErrors.OperationCancelled);

    /// <summary>
    /// Creates a failure result for a single null argument using fluent syntax.
    /// </summary>
    /// <typeparam name="T">The type carried by the result.</typeparam>
    /// <param name="parameterName">The name of the parameter that was <c>null</c>.</param>
    /// <param name="message">An optional custom error message.</param>
    /// <returns>A failed result containing the null argument error.</returns>
    public static Result<T> FailForNullArgument<T>(string parameterName, string? message = null)
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            return Result<T>.WithFailure(ErrorNameNullOrEmpty);
        }

        var error = new NullArgumentError(parameterName, message);
        return Result<T>.WithFailure(error.ToString());
    }

    /// <summary>
    /// Creates a failure result that aggregates multiple null argument errors using fluent syntax.
    /// </summary>
    /// <typeparam name="T">The type carried by the result.</typeparam>
    /// <param name="parameterNames">The names of the parameters that were <c>null</c>.</param>
    /// <returns>A failed result describing each null argument error.</returns>
    public static Result<T> FailForNullArguments<T>(params string[] parameterNames)
    {
        if (parameterNames == null || parameterNames.Length == 0 || parameterNames.Any(string.IsNullOrEmpty))
        {
            return Result<T>.WithFailure(ErrorNamesNullOrEmpty);
        }

        var error = new MultipleNullArgumentsError(parameterNames);
        return Result<T>.WithFailure(error.ToString());
    }

    /// <summary>
    /// Creates a non-generic failure result for a single null argument using fluent syntax.
    /// </summary>
    /// <param name="parameterName">The name of the parameter that was <c>null</c>.</param>
    /// <param name="message">An optional custom error message.</param>
    /// <returns>A failed result containing the null argument error.</returns>
    public static Result FailForNullArgument(string parameterName, string? message = null)
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            return Result.WithFailure(ErrorNamesNullOrEmpty);
        }

        var error = new NullArgumentError(parameterName, message);
        return Result.WithFailure(error.ToString());
    }

    /// <summary>
    /// Creates a non-generic failure result that aggregates multiple null argument errors using fluent syntax.
    /// </summary>
    /// <param name="parameterNames">The names of the parameters that were <c>null</c>.</param>
    /// <returns>A failed result describing each null argument error.</returns>
    public static Result FailForNullArguments(params string[] parameterNames)
    {
        if (parameterNames == null || parameterNames.Length == 0 || parameterNames.Any(string.IsNullOrEmpty))
        {
            return Result.WithFailure(ErrorNamesNullOrEmpty);
        }

        var error = new MultipleNullArgumentsError(parameterNames);
        return Result.WithFailure(error.ToString());
    }

    /// <summary>
    /// Validates that a reference-type parameter is not <c>null</c> and returns the appropriate result.
    /// </summary>
    /// <typeparam name="T">The type of the parameter to validate.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <returns>A success result when the value is non-null; otherwise, a failure describing the missing parameter.</returns>
    public static Result<T> EnsureNotNull<T>(T? value, string parameterName) where T : class
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            return Result<T>.WithFailure(ErrorNameNullOrEmpty);
        }

        return value is null
            ? FailForNullArgument<T>(parameterName)
            : Result<T>.Success(value);
    }

    /// <summary>
    /// Validates that a nullable value type is not <c>null</c> and returns the appropriate result.
    /// </summary>
    /// <typeparam name="T">The value type being validated.</typeparam>
    /// <param name="value">The nullable value to validate.</param>
    /// <param name="parameterName">The name of the parameter being validated.</param>
    /// <returns>A success result when the value has a value; otherwise, a failure describing the missing parameter.</returns>
    public static Result<T> EnsureNotNull<T>(T? value, string parameterName) where T : struct
    {
        if (string.IsNullOrEmpty(parameterName))
        {
            return Result<T>.WithFailure(ErrorNameNullOrEmpty);
        }

        return value.HasValue
            ? Result<T>.Success(value.Value)
            : FailForNullArgument<T>(parameterName);
    }

    /// <summary>
    /// Validates multiple parameters and returns a result indicating which values were <c>null</c>.
    /// </summary>
    /// <param name="validations">An array of parameter validations consisting of the value and parameter name.</param>
    /// <returns>A success result when all parameters are non-null; otherwise, a failure describing the null parameters.</returns>
    public static Result ValidateNotNull(params (object? value, string parameterName)[] validations)
    {
        if (validations == null || validations.Length == 0)
        {
            return Result.WithFailure(ErrorValidationNullOrEmpty);
        }

        var nullParameters = validations
            .Where(v => v.value is null)
            .Select(v => v.parameterName)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToArray();

        return nullParameters.Length switch
        {
            0 => Result.Success(),
            1 => FailForNullArgument(nullParameters[0]),
            _ => FailForNullArguments(nullParameters)
        };
    }

    /// <summary>
    /// Validates multiple parameters and, when successful, produces a result using the provided factory function.
    /// </summary>
    /// <typeparam name="T">The type produced by the factory function.</typeparam>
    /// <param name="factory">The factory function invoked when all validations pass.</param>
    /// <param name="validations">An array of parameter validations consisting of the value and parameter name.</param>
    /// <returns>A success result produced by the factory or a failure listing the null parameters.</returns>
    public static Result<T> CreateIfValid<T>(Func<T> factory, params (object? value, string parameterName)[] validations)
    {
        if (factory == null)
        {
            return Result<T>.WithFailure(ErrorFactoryFunctionNull);
        }

        if (validations == null)
        {
            return Result<T>.WithFailure(ErrorValidationNull);
        }

        var nullParameters = validations
            .Where(v => v.value is null)
            .Select(v => v.parameterName)
            .Where(n => !string.IsNullOrEmpty(n))
            .ToArray();

        return nullParameters.Length switch
        {
            0 => Result<T>.Success(factory()),
            1 => FailForNullArgument<T>(nullParameters[0]),
            _ => FailForNullArguments<T>(nullParameters)
        };
    }
}

#pragma warning restore CS0702
#pragma warning restore EXXER200 // This extension method is designed to be used with Result<T> and Result types, following the Open/Closed Principle for extensibility.
