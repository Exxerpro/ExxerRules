// <copyright file="RequestPerformanceBehaviour.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Behaviors;

/// <summary>
/// Pipeline behavior for logging the performance of request handling, including execution time.
/// </summary>
/// <typeparam name="TRequest">Type of the request being processed.</typeparam>
/// <typeparam name="TResponse">Type of the response produced.</typeparam>
public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> logger;
    private readonly Stopwatch timer;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPerformanceBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">Logger to log performance information.</param>
    public RequestPerformanceBehaviour(ILogger<TRequest> logger)
    {
        this.logger = logger;
        this.timer = new Stopwatch();
    }

    /// <summary>
    /// Processes the incoming request, measures, and logs the execution time.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response of the action.</returns>
    public async Task<TResponse> HandleAsync(TRequest request, RequestFunctionalHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        this.timer.Reset();
        this.timer.Start();

        var response = await next().ConfigureAwait(false);

        this.timer.Stop();

        if (this.timer.ElapsedMilliseconds <= 500)
        {
            this.logger.LogInformation(
                "IndTrace Performance Request: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                typeof(TRequest).Name, this.timer.ElapsedMilliseconds, request);
        }
        else
        {
            this.logger.LogWarning(
                "IndTrace Performance Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                typeof(TRequest).Name, this.timer.ElapsedMilliseconds, request);
        }

        return response;
    }
}

// TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated performance measurement or logging logic. Refactor for maintainability if necessary.
// TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - Ensure async/await is used efficiently and avoid unnecessary context switches. Use ConfigureAwait(false) in library code.
