// <copyright file="PlcExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Gateway.Extensions;

using IndTrace.S7Rx;

public static class PlcExtensions
{
    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate PLC extension logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated extension or validation logic. Refactor for maintainability if necessary.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency extension operations, consider optimizing data processing and memory usage.
    public static async Task<IIndTraceControllerRx> AddControllerAsync(
        this
        PlcDto value,
        ILogger logger,
        IndTrace.HubConnection.Abstractions.IHubConnection hubConnection,
        IndTrace.HubConnection.Abstractions.IHubConnectionFactory connectionFactory,
        DateTimeMachine dateTimeMachine,
        CancellationToken cancellationToken)
    {
        var controllerRx = new IndTraceControllerRx(logger, value, dateTimeMachine);

        var message = $"PLC {value.MachineId} Controller Created";
        await hubConnection.LogAndSendMessageFromControllerAsync(
            message,
            logger, connectionFactory, cancellationToken).ConfigureAwait(false);

        return controllerRx;
    }

    public static async Task<bool> ConfigureControllerAsync(
        this IIndTraceControllerRx controller,
        int key,
        ILogger logger,
        IndTrace.HubConnection.Abstractions.IHubConnection hubConnection,
        IndTrace.HubConnection.Abstractions.IHubConnectionFactory connectionFactory,
        CancellationToken cancellationToken)
    {
        var result = await controller.SetUpAsync(cancellationToken).ConfigureAwait(false);

        var message = result
            ? $"PLC {key} Controller configured successfully"
            : $"PLC {key} Controller configured with errors";

        await hubConnection.LogAndSendMessageFromControllerAsync(message, logger, connectionFactory, cancellationToken).ConfigureAwait(false);

        return result;
    }

    public static async Task<bool> ValidateVariablesAsync(
        this IIndTraceControllerRx controller,
        int key,
        ILogger logger,
        IndTrace.HubConnection.Abstractions.IHubConnection hubConnection,
        IndTrace.HubConnection.Abstractions.IHubConnectionFactory connectionFactory,
        CancellationToken cancellationToken)
    {
        var result = await controller.ValidateThatTheTagExistOnTheController(cancellationToken).ConfigureAwait(false);
        if (result)
        {
            logger.LogError("PLC {RecipeId} Configuration failed", key);
        }

        var message = result
            ? $"PLC {key} Variables from Controller configured successfully"
            : $"PLC {key} Variables from Controller configured with errors";

        await hubConnection.LogAndSendMessageFromControllerAsync(message, logger, connectionFactory, cancellationToken).ConfigureAwait(false);

        return result;
    }

    public static async Task<int> ConnectToControllerAsync(
        this IIndTraceControllerRx controller,
        short key,
        ILogger logger,
        IndTrace.HubConnection.Abstractions.IHubConnection hubConnection,
        IndTrace.HubConnection.Abstractions.IHubConnectionFactory connectionFactory,
        CancellationToken cancellationToken)
    {
        var connected = false;

        connected = await controller.ConnectAndCreateNotificationsAsync(cancellationToken).ConfigureAwait(false);

        if (connected)
        {
            var result = await controller.GetControllerIdAsync(logger, hubConnection, connectionFactory, cancellationToken, connected).ConfigureAwait(false);
            return result;
        }

        return 0;
    }

    private static async Task<int> GetControllerIdAsync(
        this IIndTraceControllerRx controller,
        ILogger logger,
        IndTrace.HubConnection.Abstractions.IHubConnection hubConnection,
        IndTrace.HubConnection.Abstractions.IHubConnectionFactory connectionFactory,
        CancellationToken cancellationToken,
        bool connected)
    {
        await controller.GetPlcIdAsync(cancellationToken).ConfigureAwait(false);
        var result = await controller.GetPlcIdAsync(cancellationToken).ConfigureAwait(false);

        var message = connected
            ? $"PLC {controller.MachineId} Controller connected"
            : $"PLC {controller.MachineId} Controller waiting for connection";

        await hubConnection.LogAndSendMessageFromControllerAsync(message, logger, connectionFactory, cancellationToken).ConfigureAwait(false);

        return result;
    }

    private static async Task<int> SetControllerIdAsync(
        this IIndTraceControllerRx controller,
        short value,
        CancellationToken cancellationToken)
    {
        await controller.SetPlcIdAsync(value, cancellationToken).ConfigureAwait(false);

        var result = await controller.GetPlcIdAsync(cancellationToken).ConfigureAwait(false);
        return result;
    }

    public static async Task LogPlcConnectionStatusAsync(
        this ILogger logger,
        int key,
        int plcId,
        IndTrace.HubConnection.Abstractions.IHubConnection hubConnection,
        IndTrace.HubConnection.Abstractions.IHubConnectionFactory connectionFactory,
        CancellationToken cancellationToken)
    {
        var message = plcId == key
            ? $"Connection to plc {key} successfully"
            : $"Connection to plc {key} with errors";

        await hubConnection.LogAndSendMessageFromControllerAsync(message, logger, connectionFactory, cancellationToken).ConfigureAwait(false);
    }
}
