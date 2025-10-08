// <copyright file="GatewayExecutor.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Gateway;

using IndTrace.Application.Performance.Request.Command.Create;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;

public static class GatewayExecutor
{
#if DEBUG
    private const int ErrorTimeoutCreateGatewayCommandFromPlcMilliseconds = 300_000;
    private const int ErrorTimeoutClearReferenceDataOnPlcMilliseconds = 300_000;
    private const int ErrorTimeoutSendCommandAsyncMilliseconds = 300_000;
    private const int ErrorTimeoutHandleAndPublishCommandMilliseconds = 300_000;
    private const int ErrorTimeoutHandleAndPublishResultsMilliseconds = 300_000;
    private const int ErrorTimeoutDisabledSendCommandAsyncMilliseconds = 300__000;
#else
    private const int ErrorTimeoutCreateGatewayCommandFromPlcMilliseconds = 2000;
    private const int ErrorTimeoutClearReferenceDataOnPlcMilliseconds = 2000;
    private const int ErrorTimeoutSendCommandAsyncMilliseconds = 2000;
    private const int ErrorTimeoutHandleAndPublishCommandMilliseconds = 2000;
    private const int ErrorTimeoutHandleAndPublishResultsMilliseconds = 2000;
    private const int ErrorTimeoutDisabledSendCommandAsyncMilliseconds = 60_000;

#endif

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate gateway executor logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static async Task<TCommandData> CreateGatewayCommandFromPlcAsync<TCommandData>(
        GatewayTask gatewayTask,
        IIndTraceControllerRx controller,
        string errorMessage,
        IDateTimeMachine dateTimeMachine,
        ILogger logger,
        CancellationToken cancellationToken)
        where TCommandData : ICommandData, new()
    {
        var timeout = TimeSpan.FromMilliseconds(ErrorTimeoutCreateGatewayCommandFromPlcMilliseconds); // You can adjust timeout per operation

        var result = await GatewayExecutionHelper.ExecuteWithTimeoutAndLogging(
            async ct =>
            {
                var command = new TCommandData();

                var data = await controller.UploadCommandDataFromController<TCommandData>(gatewayTask, dateTimeMachine, logger, ct).ConfigureAwait(false);

                command = (TCommandData)command.Create(data);

                if (GatewayTaskRequireUploadRegisters<TCommandData>(gatewayTask))
                {
                    await controller.ReadRegistersBulkAsync(cancellationToken).ConfigureAwait(false);
                    command.Command.Registers = controller.Registers;
                }

                await controller.ResetCommandAsync(cancellationToken).ConfigureAwait(false);
                command.Command.SetCommandStatusFromTask(gatewayTask.Name);

                return Result<TCommandData>.Success(command);
            },
            timeout,
            cancellationToken,
            logger,
            $"CreateGatewayCommandFromPlc-{gatewayTask.Name}").ConfigureAwait(false);

        return result.Value!;
    }

    private static bool GatewayTaskRequireUploadRegisters<TCommandData>(GatewayTask gatewayTask)
        where TCommandData : ICommandData, new()
    {
        return gatewayTask.Name == GatewayTask.UpdateCycleOkAsync.Name ||
               gatewayTask.Name == GatewayTask.UpdateCycleNotOkAsync.Name ||
               gatewayTask.Name == GatewayTask.RejectPartAsync.Name;
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate gateway executor logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    public static async Task<Result> ClearReferenceDataOnPlcAsync<TCommandData>(
        GatewayTask gatewayTask,
        IIndTraceControllerRx controller,
        string errorMessage,
        IDateTimeMachine dateTimeMachine,
        ILogger logger,
        CancellationToken cancellationToken)
        where TCommandData : ICommandData, new()
    {
        var timeout = TimeSpan.FromMilliseconds(ErrorTimeoutClearReferenceDataOnPlcMilliseconds); // You can adjust timeout per operation

        return await GatewayExecutionHelper.ExecuteWithTimeoutAndLogging(
            async ct =>
            {
                await controller.DownloadReferenceDataToPlc<TCommandData>(gatewayTask, dateTimeMachine, logger, ct).ConfigureAwait(false);
                return Result.Success();
            },
            timeout,
            cancellationToken,
            logger,
            "ClearReferenceDataOnPlc").ConfigureAwait(false);
    }

    // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated executor or task logic. Refactor for maintainability if necessary.
    public static async Task<Result<TaskGatewayResponse>> SendCommandAsync<TCommand>(
        TCommand command,
        string errorMessage,
        IGatewayCommandDispatcher commandDispatcher,
        ILogger logger,
        CancellationToken cancellationToken)
        where TCommand : IGatewayRequest<TaskGatewayResponse>, ICommandData
    {
        var timeout = TimeSpan.FromMilliseconds(ErrorTimeoutSendCommandAsyncMilliseconds); // You can adjust timeout per operation
        if (command.Command.WatchDogTime == WatchDog.Disable)
        {
            timeout = TimeSpan.FromMilliseconds(ErrorTimeoutDisabledSendCommandAsyncMilliseconds); // You can adjust timeout per operation
        }

        return await GatewayExecutionHelper.ExecuteWithTimeoutAndLogging(
            async ct =>
            {
                var result = await commandDispatcher.ProcessAsync(command, ct).ConfigureAwait(false);

                if (result.Value is null)
                {
                    return result;
                }

                result.Value.EnsureIsValidToRenderAndPersist();
                result.Value.RequestTask = command.Command.GatewayTask.Name;

                return result;
            },
            timeout,
            cancellationToken,
            logger,
            $"SendCommandAsync-{command.Command.GatewayTask.Name}").ConfigureAwait(false);
    }

    public static async Task PublishResultsToPlcAndHubAsync(
        TaskGatewayRequest request,
        Result<TaskGatewayResponse> response,
        IIndTraceControllerRx controller,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMilliseconds(ErrorTimeoutHandleAndPublishResultsMilliseconds); // You can adjust timeout per operation

        await GatewayExecutionHelper.ExecuteWithTimeoutAndLogging(
            async ct =>
            {
                logger.LogInformation("Publishing results to PLC and Hub for MachineId {MachineId}", request.MachineId);

                var plcTask = controller.PublishResultToPlc(request, response, hubConnection, logger, ct);

                _ = Task.Run(
                    async () =>
                {
                    try
                    {
                        await hubConnection.PublishResultsToHubAsync(request, response, connectionFactory, logger, ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while publishing results to the Hub.");
                    }
                }, ct);

                await plcTask.ConfigureAwait(false);

                logger.LogInformation("request {request} result {result}", request.GatewayTask, response.Value);

                return Result.Success();
            },
            timeout,
            cancellationToken,
            logger,
            "PublishResultsToPlcAndHub").ConfigureAwait(false);
    }

    public static async Task PublishCommandToHubAsync(
        TaskGatewayRequest request,
        IIndTraceControllerRx controller,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMilliseconds(ErrorTimeoutHandleAndPublishCommandMilliseconds); // Same as for publishing results

        await GatewayExecutionHelper.ExecuteWithTimeoutAndLogging(
            async ct =>
            {
                if (hubConnection is not null)
                {
                    await hubConnection.PublishCommandToHubAsync(request, connectionFactory, logger, ct).ConfigureAwait(false);
                }
                else
                {
                    logger.LogWarning("HubConnection is null while trying to publish command for MachineId {MachineId}", request.MachineId);
                }

                return Result.Success();
            },
            timeout,
            cancellationToken,
            logger,
            "PublishCommandToHub").ConfigureAwait(false);
    }

    public static async Task<PerformanceDataCommand> ReadPerformanceDataCommandFromPlcAsync(
        TaskGatewayRequest request,
        Result<TaskGatewayResponse> response,
        IIndTraceControllerRx controller,
        IHubConnection? hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var timeout = TimeSpan.FromMilliseconds(ErrorTimeoutHandleAndPublishResultsMilliseconds); // You can adjust timeout per operation

        var result = await GatewayExecutionHelper.ExecuteWithTimeoutAndLogging(
            async ct =>
            {
                logger.LogInformation("Reading Performance Data From PLC {PlcId}", controller.PlcId);
                var performanceDataCommandFromPlc = await controller.ReadPerformanceDataFromPlcAsync(ct).ConfigureAwait(false);

                logger.LogInformation("Sending Performance Data to DB {MachineId}", request.MachineId);

                return performanceDataCommandFromPlc;
            },
            timeout,
            cancellationToken,
            logger,
            "ReadingPerformanceDataCommandFromPLC").ConfigureAwait(false);

        if (result is { IsSuccess: true, Value: not null })
        {
            result.Value.PlcId = controller.PlcId;
            return result.Value;
        }

        return new PerformanceDataCommand { PlcId = controller.PlcId };
    }

    public static Task<Result<TaskGatewayResponse>> SendPerformanceDataCommandToApplication<TCommand>(
        TCommand command,
        string errorMessage,
        IGatewayCommandDispatcher commandDispatcher,
        ILogger logger,
        CancellationToken cancellationToken)
        where TCommand : IGatewayRequest<TaskGatewayResponse>, ICommandData
    {
        var timeout = TimeSpan.FromMilliseconds(
            command.Command.WatchDogTime == WatchDog.Disable
                ? ErrorTimeoutDisabledSendCommandAsyncMilliseconds
                : ErrorTimeoutSendCommandAsyncMilliseconds);

        // Fire-and-forget block, with scoped cancellation token
        _ = Task.Run(
            async () =>
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(timeout);

            try
            {
                var result = await commandDispatcher.ProcessAsync(command, cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("Command {Command} was cancelled or timed out", command.Command.GatewayTask.Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Fire-and-forget failed for task {Task}", command.Command.GatewayTask.Name);
            }
        }, cancellationToken);

        // Always return a basic success result, as this is non-blocking
        return Task.FromResult(Result<TaskGatewayResponse>.Success(new TaskGatewayResponse
        {
            RequestTask = command.Command.GatewayTask.Name,
        }));
    }
}
