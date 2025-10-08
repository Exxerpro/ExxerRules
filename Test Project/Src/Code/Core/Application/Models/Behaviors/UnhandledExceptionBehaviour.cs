// <copyright file="UnhandledExceptionBehaviour.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Behaviors;

/// <summary>
/// Pipeline behavior for logging unhandled exceptions during request processing.
/// </summary>
/// <typeparam name="TRequest">Type of the request being processed.</typeparam>
/// <typeparam name="TResponse">Type of the response produced.</typeparam>
public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<TRequest> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledExceptionBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">Logger to log unhandled exceptions.</param>
    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Processes unhandled exceptions and logs them.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response of the action.</returns>
    public async Task<TResponse> HandleAsync(TRequest request, RequestFunctionalHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogError(ex, "IndTrace App Request: Unhandled Exception for Request {Name} {@Request}", requestName, request);

            var responseType = typeof(TResponse);

            // Handle Result<T> types
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var result = CreateGenericResultFailure(responseType, $"Unhandled exception: {ex.Message}");
                return (TResponse)result!;
            }

            // Handle non-generic Result type
            if (responseType == typeof(Result))
            {
                var result = Result.WithFailure($"Unhandled exception: {ex.Message}");
                return (TResponse)(object)result;
            }

            // Handle value types - return default value
            if (responseType.IsValueType)
            {
                return (TResponse)Activator.CreateInstance(responseType)!;
            }

            // For reference types, try to create instance or return null if nullable
            if (Nullable.GetUnderlyingType(responseType) != null || !responseType.IsValueType)
            {
                return default(TResponse)!;
            }

            throw new InvalidOperationException($"Cannot create error result for response type: {responseType.Name}");
        }
    }

    private static object CreateGenericResultFailure(Type resultType, string message)
    {
        var methods = resultType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "WithFailure")
            .ToArray();

        var genericArg = resultType.GetGenericArguments()[0];

        // Prefer (string, T? value) overload
        var withFailureMethod = methods.FirstOrDefault(m =>
        {
            var parameters = m.GetParameters();
            return parameters.Length == 2
                   && parameters[0].ParameterType == typeof(string)
                   && (parameters[1].ParameterType == genericArg ||
                       Nullable.GetUnderlyingType(parameters[1].ParameterType) == genericArg);
        });

        if (withFailureMethod != null)
        {
            object? defaultValue = genericArg.IsValueType ? Activator.CreateInstance(genericArg) : null;

            // If the method is non-nullable, ensure we pass a non-null default value
            if (defaultValue is null)
            {
                var parameters = withFailureMethod.GetParameters();
                if (parameters[1].ParameterType.IsValueType && Nullable.GetUnderlyingType(parameters[1].ParameterType) == null)
                {
                    throw new InvalidOperationException($"Cannot call WithFailure with null default value for non-nullable type {genericArg.Name}");
                }
            }

            return withFailureMethod.Invoke(null, new object[] { message, defaultValue! })!;
        }

        // Fallback: try (string) overload
        withFailureMethod = methods.FirstOrDefault(m =>
    {
        var parameters = m.GetParameters();
        return parameters.Length == 1 && parameters[0].ParameterType == typeof(string);
    });

        if (withFailureMethod != null)
        {
            return withFailureMethod.Invoke(null, new object[] { message })!;
        }

        throw new InvalidOperationException($"Cannot find WithFailure method on type {resultType.Name}");
    }
}

// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Exception handling: ensure only expected exceptions are caught and logged. Avoid catching general Exception unless necessary. See .NET best practices for exception handling.
// TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated exception handling or logging logic. Refactor for maintainability if necessary.
