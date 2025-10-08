// <copyright file="GatewayTaskDefinition.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Gateway;

public static class GatewayTaskDefinition
{
    private static readonly Dictionary<GatewayTask, Func<IIndTraceControllerRx, IGatewayCommandDispatcher, IndTrace.HubConnection.Abstractions.IHubConnection, IndTrace.HubConnection.Abstractions.IHubConnectionFactory, ILogger, CancellationToken, Task<Result<TaskGatewayResponse>>>> Commands;
    private static readonly ConcurrentDictionary<GatewayTask, RateLimitInfo> RateLimits;

    static GatewayTaskDefinition()
    {
        Commands = [];

        RateLimits = new ConcurrentDictionary<GatewayTask, RateLimitInfo>();

        AddCommand(GatewayTask.CreateBarCodeAsync, (controller, mediator, hubConnection, connectionFactory, logger, cancellationToken) => GatewayTasks.CreateBarCodeAsync(controller, mediator, hubConnection, connectionFactory, logger, cancellationToken));
        AddCommand(GatewayTask.ReadBarCodeAsync, (controller, mediator, hubConnection, connectionFactory, logger, cancellationToken) => GatewayTasks.ReadBarCodeAsync(controller, mediator, hubConnection, connectionFactory, logger, cancellationToken));
        AddCommand(GatewayTask.CreateCycleAsync, (controller, mediator, hubConnection, connectionFactory, logger, cancellationToken) => GatewayTasks.CreateCycleAsync(controller, mediator, hubConnection, connectionFactory, logger, cancellationToken));
        AddCommand(GatewayTask.UpdateCycleOkAsync, (controller, mediator, hubConnection, connectionFactory, logger, cancellationToken) => GatewayTasks.UpdateCycleOkAsync(controller, mediator, hubConnection, connectionFactory, logger, cancellationToken));
        AddCommand(GatewayTask.UpdateCycleNotOkAsync, (controller, mediator, hubConnection, connectionFactory, logger, cancellationToken) => GatewayTasks.UpdateCycleNotOkAsync(controller, mediator, hubConnection, connectionFactory, logger, cancellationToken));
        AddCommand(GatewayTask.EndOfProcessAsync, (controller, mediator, hubConnection, connectionFactory, logger, cancellationToken) => GatewayTasks.EndOfProcessAsync(controller, mediator, hubConnection, connectionFactory, logger, cancellationToken));
    }

    private static void AddCommand(GatewayTask id, Func<IIndTraceControllerRx, IGatewayCommandDispatcher, IndTrace.HubConnection.Abstractions.IHubConnection, IndTrace.HubConnection.Abstractions.IHubConnectionFactory, ILogger, CancellationToken, Task<Result<TaskGatewayResponse>>> commandTask)
    {
        Commands.Add(id, commandTask);
        RateLimits[id] = new RateLimitInfo(); // Initialize rate limiting for each monitorRequest
    }

    public static Func<IIndTraceControllerRx, IGatewayCommandDispatcher, IndTrace.HubConnection.Abstractions.IHubConnection, IndTrace.HubConnection.Abstractions.IHubConnectionFactory, ILogger, CancellationToken, Task> GetCommandTask(GatewayTask id) => Commands[id];

    public static string GetEventPlc(GatewayTask id) => id.ToString();

    public static async Task<Result<TaskGatewayResponse>> ExecuteWithRateLimitingAsync(
        GatewayTask id, IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher, IndTrace.HubConnection.Abstractions.IHubConnection hubConnection,
        IndTrace.HubConnection.Abstractions.IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var rateLimitInfo = RateLimits[id];
        await rateLimitInfo.Semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            var currentTime = DateTime.UtcNow.ToLocalTime();
            var currentController = controller.PlcId;
            var timeSinceLastExecution = currentTime - rateLimitInfo.LastExecutionTime;

            if (timeSinceLastExecution < rateLimitInfo.RateLimitInterval && currentController == rateLimitInfo.Controller)
            {
                if (rateLimitInfo.LastResult is not null && rateLimitInfo.LastResult.IsSuccess && rateLimitInfo.LastResult.Value is not null && !string.IsNullOrEmpty(rateLimitInfo.LastResult.Value.Label))
                {
                    logger?.LogInformation("Rate limit exceeded for {Command}. Returning cached result.", id);
                    return rateLimitInfo.LastResult;
                }
                else
                {
                    var waitTime = rateLimitInfo.RateLimitInterval - timeSinceLastExecution;
                    logger?.LogInformation("Waiting {WaitTime} to execute {Command} due to rate limit.", waitTime, id);
                    await Task.Delay(waitTime, cancellationToken).ConfigureAwait(false);
                }
            }

            rateLimitInfo.LastExecutionTime = DateTime.UtcNow.ToLocalTime();
            rateLimitInfo.Controller = currentController;
            var command = Commands[id];
            rateLimitInfo.LastResult = await command(controller, commandDispatcher, hubConnection, connectionFactory, logger ?? throw new ArgumentNullException(nameof(logger)), cancellationToken).ConfigureAwait(false);
            return rateLimitInfo.LastResult;
        }
        finally
        {
            rateLimitInfo.Semaphore.Release();
        }
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate gateway task definition logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
}
