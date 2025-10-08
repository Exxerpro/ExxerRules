// <copyright file="GatewayTasks.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Gateway;

using IndTrace.Application.BarCodes.Commands.Create;
using IndTrace.Application.BarCodes.Commands.Update;
using IndTrace.Application.Cycles.Commands.Create;
using IndTrace.Application.Cycles.Commands.UpdateCyclesNok;
using IndTrace.Application.Cycles.Commands.UpdateCyclesOk;
using IndTrace.Application.Performance.Request.Command.Create;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;

public static class GatewayTasks
{
    private static readonly IDateTimeMachine DateTimeMachine = new DateTimeMachine();

    private static readonly bool OeeEnabledFromEnvironment =
        (Environment.GetEnvironmentVariable("OEE_ENABLED") ?? "false")
            .Equals("true", StringComparison.OrdinalIgnoreCase);

    private static async Task<Result<TaskGatewayResponse>> ExecuteGatewayCommandAsync<TCommand>(
        GatewayTask task,
        string failureMessage,
        IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken)
        where TCommand : ICommandData, IGatewayRequest<TaskGatewayResponse>, new()
    {
        try
        {
            var timer = Stopwatch.GetTimestamp();
            logger.LogInformation("Gateway Task {TaskName} Starting", task.Name);

            var command = await GatewayExecutor.CreateGatewayCommandFromPlcAsync<TCommand>(
                task, controller, failureMessage, DateTimeMachine, logger, cancellationToken).ConfigureAwait(false);

            await GatewayExecutor.PublishCommandToHubAsync(command.Command, controller, hubConnection, connectionFactory, logger, cancellationToken).ConfigureAwait(false);

            var downloadTask = GatewayExecutor.ClearReferenceDataOnPlcAsync<TCommand>(
                task, controller, failureMessage, DateTimeMachine, logger, cancellationToken);

            var sendTask = GatewayExecutor.SendCommandAsync<TCommand>(
                command, failureMessage, commandDispatcher, logger, cancellationToken);

            await Task.WhenAll(downloadTask, sendTask).ConfigureAwait(false);

            var result = sendTask.Result;

            if (result?.Value is not null)
            {
                result.Value.ExecutionTime = Stopwatch.GetElapsedTime(timer);
            }

            logger.LogInformation("Gateway Task {task.Name} from Machine ID {PerformanceDataCommandId} Finished on {Elapsed}ms  ", task.Name, result?.Value?.ExecutionTime, controller.PlcId);

            if (result is not null)
            {
                await GatewayExecutor.PublishResultsToPlcAndHubAsync(command.Command, result, controller, hubConnection, connectionFactory, logger, cancellationToken).ConfigureAwait(false);
            }

            // [NOTE]
            // [IMPORTANT]
            // [ABR]
            // 16 JUN 2025
            // Disable this task from the controller, using the environment variable OEE_ENABLED
            // send this to an another application using a vertical slice
            // Still left the code here for future reference, but it will not be executed, maybe in another release we will manage
            // directly the OEE performance data from the controller to the application
            if (result is not null && result.Value is not null)
            {
                return result;
            }

            // Use static field for OEE_ENABLED environment variable
            if (controller.IsOeeEnabled || !OeeEnabledFromEnvironment)
            {
                return result ?? Result<TaskGatewayResponse>.WithFailure("Gateway task result is null");
            }

            if (result is null)
            {
                return Result<TaskGatewayResponse>.WithFailure("Gateway task result is null");
            }

            logger.LogInformation("Reading performance Tags from PLC {PlcId}", controller.PlcId);
            var resultOee = await GatewayExecutor.ReadPerformanceDataCommandFromPlcAsync(command.Command, result, controller, hubConnection, connectionFactory, logger, cancellationToken).ConfigureAwait(false);

            resultOee.UpdateDataFromResult(result);

            await GatewayExecutor.SendPerformanceDataCommandToApplication<PerformanceDataCommand>(
                resultOee, failureMessage, commandDispatcher, logger, cancellationToken).ConfigureAwait(false);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, failureMessage);
            await controller.ResetCommandAsync(cancellationToken).ConfigureAwait(false);
            return Result<TaskGatewayResponse>.WithFailure(failureMessage);
        }
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate gateway tasks logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated task or scheduling logic. Refactor for maintainability if necessary.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency task operations, consider optimizing scheduling and resource usage.
    public static Task<Result<TaskGatewayResponse>> CreateBarCodeAsync(
        IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken) => ExecuteGatewayCommandAsync<CreateBarCodeCommand>(GatewayTask.CreateBarCodeAsync, GatewayMessages.FailedCreateBarcode,
            controller, commandDispatcher, hubConnection, connectionFactory, logger, cancellationToken);

    public static Task<Result<TaskGatewayResponse>> ReadBarCodeAsync(
        IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken) => ExecuteGatewayCommandAsync<ReadBarCodeQuery>(GatewayTask.ReadBarCodeAsync, GatewayMessages.FailedReadBarcode,
            controller, commandDispatcher, hubConnection, connectionFactory, logger, cancellationToken);

    public static Task<Result<TaskGatewayResponse>> CreateCycleAsync(
        IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken) => ExecuteGatewayCommandAsync<CreateCyclesCommand>(GatewayTask.CreateCycleAsync, GatewayMessages.FailedCreateCycle,
            controller, commandDispatcher, hubConnection, connectionFactory, logger, cancellationToken);

    public static Task<Result<TaskGatewayResponse>> UpdateCycleOkAsync(
        IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken) => ExecuteGatewayCommandAsync<UpdateCyclesOkCommand>(GatewayTask.UpdateCycleOkAsync, GatewayMessages.FailedUpdateCycle,
            controller, commandDispatcher, hubConnection, connectionFactory, logger, cancellationToken);

    public static Task<Result<TaskGatewayResponse>> UpdateCycleNotOkAsync(
        IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken) => ExecuteGatewayCommandAsync<UpdateCyclesNotOkCommand>(GatewayTask.UpdateCycleNotOkAsync, GatewayMessages.FailedUpdateCycle,
            controller, commandDispatcher, hubConnection, connectionFactory, logger, cancellationToken);

    public static Task<Result<TaskGatewayResponse>> EndOfProcessAsync(
        IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken) => ExecuteGatewayCommandAsync<UpdateBarCodeCommand>(GatewayTask.EndOfProcessAsync, GatewayMessages.FailedEndProcess,
            controller, commandDispatcher, hubConnection, connectionFactory, logger, cancellationToken);
}
