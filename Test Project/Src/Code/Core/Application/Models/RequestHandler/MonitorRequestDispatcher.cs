// <copyright file="MonitorRequestDispatcher.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Application.Models.RequestHandler;

/// <summary>
/// Dispatches GUI commands and queries to their appropriate handlers and manages pipeline behaviors.
/// </summary>
public class MonitorRequestDispatcher(IServiceProvider provider, ILogger<MonitorRequestDispatcher> logger) : IMonitorRequestDispatcher
{
    /// <summary>
    /// Processes a GUI command and returns the result.
    /// </summary>
    /// <param name="command">The GUI command to process.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the command.</returns>
    public Task<Result> ProcessAsync(IMonitorRequest command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var handlerType = typeof(IMonitorRequestHandler<>).MakeGenericType(commandType);

        return this.InvokePipeline<Result>(commandType, command, handlerType, cancellationToken);
    }

    /// <summary>
    /// Processes a GUI request and returns the result.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="monitorRequest">The GUI request to process.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the request.</returns>
    public Task<Result<TResponse>> ProcessAsync<TResponse>(IMonitorRequest<TResponse> monitorRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            var commandType = monitorRequest.GetType();
            var handlerType = typeof(IMonitorRequestHandler<,>).MakeGenericType(commandType, typeof(TResponse));

            return this.InvokePipeline<Result<TResponse>>(commandType, monitorRequest, handlerType, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception Ocurred {ex}", ex);
            return Task.FromResult(Result<TResponse>.WithFailure(ex.Message));
        }
    }

    /// <summary>
    /// Processes a Monitor query and returns the result.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="monitorRequest">The Monitor query to process.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the query.</returns>
    public Task<Result<TResponse>> QueryAsync<TResponse>(IMonitorRequest<TResponse> monitorRequest, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryType = monitorRequest.GetType();
            var handlerType = typeof(IMonitorQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));

            return this.InvokePipeline<Result<TResponse>>(queryType, monitorRequest, handlerType, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exxception Ocurred {ex}", ex);
            return Task.FromResult(Result<TResponse>.WithFailure(ex.Message));
        }
    }

    private Task<TResponse> InvokePipeline<TResponse>(
        Type requestType,
        object request,
        Type handlerInterfaceType,
        CancellationToken cancellationToken)
    {
        object handler;
        MethodInfo? handleMethod;
        try
        {
            handler = provider.GetRequiredService(handlerInterfaceType);
            handleMethod = handlerInterfaceType.GetMethod("ProcessAsync");

            if (handleMethod is null)
            {
                logger.LogError("Method 'ProcessAsync' not found on handler type {HandlerType}", handlerInterfaceType.FullName);
                return typeof(TResponse) switch
                {
                    var t when t == typeof(Result) => (Task<TResponse>)(object)Task.FromResult(Result.WithFailure($"Method 'ProcessAsync' not found on handler type {handlerInterfaceType.FullName}")),
                    var t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>) =>
                        Task.FromResult((TResponse)(
                            t.GetMethod("WithFailure", new[] { typeof(string) })?.Invoke(null, new object[] { $"Method 'ProcessAsync' not found on handler type {handlerInterfaceType.FullName}" })
                            ?? throw new InvalidOperationException("Failed to create error result"))),
                    _ => Task.FromResult((TResponse)(object)Result.WithFailure($"Method 'ProcessAsync' not found on handler type {handlerInterfaceType.FullName}")),
                };
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception Ocurred {ex}", ex);
            return Task.FromResult((TResponse)(object)Result.WithFailure($"Error processing hander  {ex.Message}"));
        }

        if (handleMethod is null || handler is null)
        {
            return Task.FromResult((TResponse)(object)Result.WithFailure("Handler not found "));
        }

        try
        {
            RequestFunctionalHandlerDelegate<TResponse> finalFunctionalHandler = () => (Task<TResponse>)handleMethod.Invoke(handler, [request, cancellationToken])!;
            var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
            var behaviors = provider.GetServices(behaviorType).Reverse().ToList();

            foreach (var behavior in behaviors)
            {
                var next = finalFunctionalHandler;
                var method = behaviorType.GetMethod("HandleAsync");
                if (method == null)
                {
                    logger.LogError("Method 'HandleAsync' not found on behavior type {BehaviorType}", behaviorType.FullName);
                    return typeof(TResponse) switch
                    {
                        var t when t == typeof(Result) => (Task<TResponse>)(object)Task.FromResult(Result.WithFailure($"Method 'HandleAsync' not found on behavior type {behaviorType.FullName}")),
                        var t when t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Result<>) =>
                            Task.FromResult((TResponse)(
                                t.GetMethod("WithFailure", new[] { typeof(string) })?.Invoke(null, new object[] { $"Method 'HandleAsync' not found on behavior type {behaviorType.FullName}" })
                                ?? throw new InvalidOperationException("Failed to create error result"))),
                        _ => Task.FromResult((TResponse)(object)Result.WithFailure($"Method 'HandleAsync' not found on behavior type {behaviorType.FullName}")),
                    };
                }

                finalFunctionalHandler = () => (Task<TResponse>)method.Invoke(behavior, [request, next, cancellationToken])!;
            }

            return finalFunctionalHandler();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exxception Ocurred {ex}", ex);
            return Task.FromResult((TResponse)(object)Result.WithFailure($"Error processing hander  {ex.Message}"));
        }
    }
}
