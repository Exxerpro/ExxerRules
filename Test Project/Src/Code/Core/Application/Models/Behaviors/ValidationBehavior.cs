// <copyright file="ValidationBehavior.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Behaviors;

using ValidationException = FluentValidation.ValidationException;

/// <summary>
/// Pipeline behavior for validating incoming requests using registered validators.
/// </summary>
/// <typeparam name="TRequest">Type of the request being processed.</typeparam>
/// <typeparam name="TResponse">Type of the response produced.</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">List of Fluent Validators for the TRequest type.</param>
    /// <param name="logger">Logger for diagnostic information.</param>
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        this.validators = validators;
        this.logger = logger;
    }

    /// <summary>
    /// Validates the request before passing it to the next delegate.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response of the action.</returns>
    public async Task<TResponse> HandleAsync(TRequest request, RequestFunctionalHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // If no validators are registered, proceed to next handler
        if (!this.validators.Any())
        {
            return await next().ConfigureAwait(false);
        }

        // Create validation context for the incoming request
        var context = new ValidationContext<TRequest>(request);

        // Run all validators asynchronously
        var validationResults = await Task.WhenAll(this.validators
            .Select(v => v.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);

        // Collect all validation failures
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null).ToList();

        // If there are validation errors, handle them based on response type
        if (failures.Count != 0)
        {
            var responseType = typeof(TResponse);
            var errorMessages = failures.Select(f => f.ErrorMessage).ToList();

            switch (responseType)
            {
                // If TResponse is Result<T>, return a failed Result<T> with error messages
                case Type t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>):
                    var innerType = t.GetGenericArguments()[0];
                    var resultType = typeof(Result<>).MakeGenericType(innerType);
                    var withFailureMethod = resultType.GetMethod("WithFailure", new[] { typeof(IEnumerable<string>) });
                    var failedResult = withFailureMethod!.Invoke(null, new object[] { errorMessages });
                    return failedResult is not null ? (TResponse)failedResult : throw new InvalidOperationException("Failed to create validation result");

                // If TResponse is non-generic Result, return a failed Result with error messages
                case Type t when t == typeof(Result):
                    return (TResponse)(object)Result.WithFailure(errorMessages);

                // For other types, log a warning and proceed to next handler
                default:
                    this.logger?.LogWarning(
                        "ValidationBehavior encountered non-Result type {ResponseType}. " +
                        "Consider updating the handler to return Result<T> for proper functional error handling.",
                        responseType.Name);
                    return await next().ConfigureAwait(false);
            }
        }

        // If no validation errors, proceed to next handler
        return await next().ConfigureAwait(false);
    }
}

// TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated validation or logging logic. Refactor for maintainability if necessary.
// TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Exception handling: avoid catching general Exception, catch specific exceptions where possible. See .NET best practices for exception handling.
// TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Ensure async/await is used efficiently and avoid unnecessary context switches. Use ConfigureAwait(false) in library code.
