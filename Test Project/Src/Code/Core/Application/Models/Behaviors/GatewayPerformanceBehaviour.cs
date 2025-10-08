// <copyright file="GatewayPerformanceBehaviour.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.Behaviors;

/// <summary>
/// Pipeline behavior for measuring performance of gateway commands and recording execution time on the response.
/// </summary>
/// <typeparam name="TRequest">The type of the incoming gateway request.</typeparam>
public class GatewayPerformanceBehaviour<TRequest, TResponse>(ILogger<TRequest> logger) : IPipelineBehavior<TRequest, Result<TaskGatewayResponse>>
    where TRequest : IGatewayRequest
{
    /// <summary>
    /// Logger instance for logging performance information.
    /// </summary>
    private readonly ILogger<TRequest> logger = logger;

    /// <summary>
    /// Threshold for considering a request as long-running.
    /// </summary>
    private readonly TimeSpan longExecutionTime = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Handles the request, measures execution time, and records it on the response.
    /// </summary>
    /// <param name="request">The incoming request.</param>
    /// <param name="next">Delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response of the action, with execution time recorded.</returns>
    public async Task<Result<TaskGatewayResponse>> HandleAsync(
        TRequest request,
        RequestFunctionalHandlerDelegate<Result<TaskGatewayResponse>> next,
        CancellationToken cancellationToken)
    {
        var startTime = Stopwatch.GetTimestamp();

        var response = await next().ConfigureAwait(false);

        var elapsedTime = Stopwatch.GetElapsedTime(startTime);

        if (response.Value is not null)
        {
            response.Value.ExecutionTime = elapsedTime;
            response = response.IsSuccess
                ? Result<TaskGatewayResponse>.Success(response.Value)
                : Result<TaskGatewayResponse>.WithFailure(response.Errors, response.Value);
        }
        else
        {
            // var result = new TaskGatewayResponse().MapFrom(request);
            response = Result<TaskGatewayResponse>.WithFailure("No response value found", null);
        }

        if (elapsedTime <= this.longExecutionTime)
        {
            this.logger.LogInformation(
                "IndTrace Performance Request: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                typeof(TRequest).Name, elapsedTime, request);
        }
        else
        {
            this.logger.LogWarning(
                "IndTrace Performance Long Running Request: {Name} ({ElapsedMilliseconds} ms) {@Request}",
                typeof(TRequest).Name, elapsedTime, request);
        }

        return response;
    }
}
