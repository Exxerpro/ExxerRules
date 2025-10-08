// <copyright file="ControllerExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Gateway.Gateway;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;
using static IndTrace.HubConnection.Contracts.HubMethods;

namespace Gateway.Extensions;

public static class ControllerExtensions
{
    public static async Task HandleCommandAsync(
        this IIndTraceControllerRx controller,
        IGatewayCommandDispatcher commandDispatcher,
        IHubConnection hubConnection,
        IHubConnectionFactory connectionFactory,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var timer = LogTimerExtensions.StartAndLog(controller, logger);

        var command = controller.Command;
        var controllerId = controller.PlcId;

        try
        {
            logger.LogInformation("Handling Command {Command} From controller {Plc}", controller.Command, controller.PlcId);
            if (command <= 0)
            {
                return;
            }

            if (!EnumModel.Exists<GatewayTask>(command))
            {
                logger.LogInformation("Command {Command} does not exist for Machine {Machine} and PLC {Plc}", command, controller.MachineId, controller.PlcId);
                return;
            }

            var task = EnumModel.FromValue<GatewayTask>(command);
            var taskName = GatewayTaskDefinition.GetEventPlc(task);

            var plcId = controller.PlcId;
            var machineId = controller.MachineId;
            var cmdMessage = $"PLC {plcId} Command {command}={taskName} from Machine {machineId} Invoked";

            SafeFireAndForget(
                () => hubConnection.TryInvokeAsync(BroadcastMessageToClients, "Gateway", cmdMessage, connectionFactory, logger, cancellationToken),
                nameof(GatewayTasks), logger);

            try
            {
                var result = await GatewayTaskDefinition.ExecuteWithRateLimitingAsync(
                    task,
                    controller,
                    commandDispatcher,
                    hubConnection,
                    connectionFactory,
                    logger,
                    cancellationToken).ConfigureAwait(false);

                if (result.IsFailure)
                {
                    logger.LogError(
                        "Command {Command}={TaskName} from Machine {Machine} and PLC {Plc} Status {Result}",
                        command, taskName, machineId, plcId, result.Errors.FirstOrDefault());
                    timer.StopAndLogTimer(logger, controllerId, command);
                    return;
                }

                logger.LogInformation(
                    "Command {Command}={TaskName} from Machine {Machine} and PLC {Plc} Status {Result}",
                    command, taskName, machineId, plcId, result);

                var message = $"PLC {plcId} Command {command}={taskName} from Machine {machineId} returned with status {result}";
                await hubConnection.TryInvokeAsync(
                    BroadcastMessageToClients, "Gateway", message,
                   connectionFactory, logger,
                    cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError("{ErrorMessage} {Exception}", IndTrace.Dependencies.Gateway.GatewayConstants.ErrorExecutingCommand, ex);
                var errorMessage = $"PlcId {plcId} {IndTrace.Dependencies.Gateway.GatewayConstants.ErrorPlcExecution} {command} from machine {machineId}";
                await hubConnection.TryInvokeAsync(
                    BroadcastMessageToClients, "Gateway", errorMessage,
                   connectionFactory, logger,
                    cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            await controller.ResetCommandAsync(cancellationToken).ConfigureAwait(false);
            logger.LogError("{ErrorMessage} {Exception}", IndTrace.Dependencies.Gateway.GatewayConstants.ErrorExecutingCommand, e);
        }

        timer.StopAndLogTimer(logger, controllerId, command);
    }

    public static async Task<TaskGatewayRequest> UploadCommandDataFromController<TCommandData>(
        this
      IIndTraceControllerRx controller,
        GatewayTask gatewayTask,
        IDateTimeMachine dateTimeMachine,
        ILogger logger,
        CancellationToken cancellationToken)
        where TCommandData : ICommandData, new()
    {
        var resultData = await AsyncCallers.ExecuteAsync(
            controller.UploadCommandDataFromController,
            logger,
            IndTrace.Dependencies.Gateway.GatewayConstants.ErrorUploadingRegisters,
            controller,
            cancellationToken);

        if (resultData is null || resultData.Value is null)
        {
            logger.LogError("{ErrorMessage} {@Controller}", IndTrace.Dependencies.Gateway.GatewayConstants.ErrorUploadingRegisters, controller);
            return new TaskGatewayRequest();
        }

        var data = TaskGatewayRequest.FromPlc(resultData.Value, controller.Name, gatewayTask, dateTimeMachine.Now.ToLocalTime());
        data.EnsureIsValidToRenderAndPersist();
        return data;
    }

    public static async Task DownloadReferenceDataToPlc<TCommandData>(
        this
            IIndTraceControllerRx controller,
        GatewayTask gatewayTask,
        IDateTimeMachine dateTimeMachine,
        ILogger logger,
        CancellationToken cancellationToken)
        where TCommandData : ICommandData, new()
    {
        ClearDefaultReferences(controller, logger, cancellationToken);

        // Downloads
        controller.Retry = false;
        await controller.SetFeedBackAsync((short)(gatewayTask.Value % 256), cancellationToken);
        await AsyncCallers.ExecuteAsync(controller.DownloadReferencesBulkAsync, logger, IndTrace.Dependencies.Gateway.GatewayConstants.ErrorDownloadingReferences, controller, cancellationToken);
    }

    private static readonly IEnumerable<string> DefaultReferences =
    [
        "LastMachineId",
        "NextMachineId",
        "CycleStatus",
        "FlowStatus",
        "PartStatus",
        "MachineType",
        "WorkFlowType",
        "BarCodeId",
        "CycleId",
        "Label",
        "ResultValidation"
    ];

    public static void ClearDefaultReferences(
        this IIndTraceControllerRx controller,
        ILogger logger, CancellationToken cancellationToken)
    {
        // First clear references value on memory
        try
        {
            if (controller.References is null || controller.References.Count <= 0)
            {
                return;
            }

            foreach (var key in DefaultReferences)
            {
                if (controller.References.TryGetValue(key, out var reference))
                {
                    reference.Value = string.Empty;
                }
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "{ErrorMessage} {@Controller}", IndTrace.Dependencies.Gateway.GatewayConstants.ErrorClearReferences, controller);
        }
    }

    public static async Task PublishResultToPlc(
        this IIndTraceControllerRx controller,
        TaskGatewayRequest request,
        Result<TaskGatewayResponse> response,
        IHubConnection? hubConnection,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        // We have to ensure the plc receive error message when the monitorRequest was not successful
        // Even if the response don't have enough information to update the plc
        if (response.Value is not null)
        {
            await SetBarCodeOnControllerAsync(controller, response.Value.Label, logger, cancellationToken);
            controller.References = response.Value.References;
            SetReferencesForController(controller, response.Value.References);
        }

        if (response.Value is null || (response.IsFailure && response.Value.ResultValidation.Value >= 0))
        {
            await SetErrorReferences(controller, logger, cancellationToken);
        }

        controller.Retry = true;
        await AsyncCallers.ExecuteAsync(controller.DownloadReferencesBulkAsync, logger, IndTrace.Dependencies.Gateway.GatewayConstants.ErrorDownloadingReferences, controller, cancellationToken);
    }

    private static async Task SetBarCodeOnControllerAsync(this IIndTraceControllerRx controller, string? value, ILogger logger, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(value))
        {
            logger.LogError("Label must not be empty after creating a label plcID {Controller}", controller.PlcId);
            return;
        }

        await AsyncCallers.ExecuteAsync(() => controller.SetBarCodeAsync(value, cancellationToken), logger, IndTrace.Dependencies.Gateway.GatewayConstants.ErrorSettingBarCode, controller);
    }

    private static async Task SetErrorReferences(IIndTraceControllerRx controller, ILogger logger, CancellationToken cancellationToken)
    {
        try
        {
            if (controller.References == null || controller.References.Count == 0)
            {
                return;
            }

            foreach (var key in new[] { "CycleStatus", "PartStatus", "ResultValidation" })
            {
                if (controller.References.TryGetValue(key, out var reference))
                {
                    reference.Value = "-1";
                }
            }

            controller.Retry = true;
            await AsyncCallers.ExecuteAsync(controller.DownloadReferencesBulkAsync, logger, IndTrace.Dependencies.Gateway.GatewayConstants.ErrorDownloadingReferences, controller, cancellationToken);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "{ErrorMessage} {@Controller}", IndTrace.Dependencies.Gateway.GatewayConstants.ErrorClearReferences, controller);
        }
    }

    private static void SetReferencesForController(IIndTraceControllerRx controller, IDictionary<string, Register> references)
    {
        if (references is not null && references.Count > 0)
        {
            controller.References = references;
        }
    }

    private static void SafeFireAndForget(Func<Task> action, string context, ILogger logger)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Unhandled exception in {Context}", context);
            }
        });
    }
}
