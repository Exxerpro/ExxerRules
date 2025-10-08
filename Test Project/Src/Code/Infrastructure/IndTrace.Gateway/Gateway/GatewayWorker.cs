// <copyright file="GatewayWorker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace IndTrace.Gateway.Gateway;

using IndTrace.Gateway.Interfaces;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;
using Microsoft.AspNetCore.SignalR;
using static IndTrace.HubConnection.Contracts.HubMethods;

/// <summary>
/// Represents the GatewayWorker.
/// </summary>
public class GatewayWorker(
    int id,
    ILogger<GatewayWorker> logger,
    IGatewayCommandDispatcher commandDispatcher,
    IndTraceConfigurationService configService,
    IHubConnectionFactory connectionFactory,
    DateTimeMachine dateTimeMachine,
    IHubConnection hubConnection)
    : BackgroundService, IManualStart
{
    public readonly int Id = id;
    private readonly ILogger<GatewayWorker> logger = logger;
    private readonly IndTraceConfigurationService configService = configService;
    private readonly IHubConnectionFactory connectionFactory = connectionFactory;
    private readonly DateTimeMachine dateTimeMachine = dateTimeMachine;
    private CancellationTokenSource? cts; // Allow individual worker cancellation

    public required PlcDto PlcData { get; set; }

    private IIndTraceControllerRx controller = null!;
    private string machineName = string.Empty;

    // Define an event that external components can trigger
    public event Func<SimulatedCommand, Task>? OnCommandReceived;

    public Task StartAsync()
    {
        if (this.cts != null)
        {
            this.logger.LogWarning("[{WorkerId}] Worker is already running", this.Id);
            return Task.CompletedTask;
        }

        this.cts = new CancellationTokenSource();
        this.logger.LogInformation("[{WorkerId}] Starting worker", this.Id);

        return Task.Run(() => this.ExecuteAsync(this.cts.Token));
    }

    public Task StopAsync()
    {
        if (this.cts == null)
        {
            this.logger.LogWarning("[{WorkerId}] Worker is not running", this.Id);
            return Task.CompletedTask;
        }

        this.logger.LogInformation("[{WorkerId}] Stopping worker", this.Id);
        this.cts.Cancel();
        this.cts = null;

        return Task.CompletedTask;
    }

    private ApplicationConfiguration configApplicationConfiguration = new();
    private DateTime lasTimeStampCommandExecuted = dateTimeMachine.Now.ToLocalTime();

    public bool Configured { get; private set; }

    public bool IsHubConnected => this.hubConnection is not null && this.hubConnection.State == HubConnectionState.Connected;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        this.hubConnection = await this.hubConnection.EnsureHubConnectionIsValid(this.connectionFactory, this.logger, stoppingToken).ConfigureAwait(false) ?? this.hubConnection;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                this.Configured = await this.TryConfigureGatewayAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                var errorMessage = $"{ErrorGatewayExecution}: {ex.Message}";
                this.logger.LogError(ex, "Error executing gateway command - {ErrorConstant}", ErrorExecutingCommand);
                await this.LogErrorToHubAsync(errorMessage, stoppingToken).ConfigureAwait(false);
            }

            if (!this.IsHubConnected)
            {
                this.hubConnection = await this.hubConnection.EnsureHubConnectionIsValid(this.connectionFactory, this.logger, stoppingToken).ConfigureAwait(false) ?? this.hubConnection;
            }

            this.LogInformationEveryFiveMinutes();

            await Task.Delay(1000, stoppingToken).ConfigureAwait(false);

            // if (PlcData.PlcId == 100 && dateTimeMachine.Now.Second == 0)
            // {
            //    var performance = _controller.ReadPerformanceDataCommandFromPlcAsync(stoppingToken);

            // _logger.LogInformation(performance.ToString());
            // }
        }
    }

    public async Task<bool> TryConfigureGatewayAsync(CancellationToken cancellationToken)
    {
        if (this.Configured)
        {
            return true;
        }

        this.logger.LogInformation("Communications Worker PLC {KpiOeeId} running at:  {time}", this.Id, this.dateTimeMachine.Now.ToLocalTime());
        await this.hubConnection.TryInvokeAsync(BroadcastMessageToClients, "Gateway", "Starting",
            this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);

        try
        {
            this.logger.LogInformation("Communications Worker  {KpiOeeId} GetConfigAppDetails at : {time}", this.Id, this.dateTimeMachine.Now.ToLocalTime());
            var resultConfiguration = await this.configService.GetConfigurationAsync(false, cancellationToken).ConfigureAwait(false);
            if (resultConfiguration.IsSuccess)
            {
                this.configApplicationConfiguration = resultConfiguration.Value!;
            }
            else
            {
                this.logger.LogError("Error getting configuration details @{Errors}", resultConfiguration.Errors);
            }

            this.logger.LogInformation("Communications Worker SetupCommunications PLC {KpiOeeId} at : {time}", this.Id, this.dateTimeMachine.Now.ToLocalTime());
            await this.SetupCommunicationsPlcs(cancellationToken).ConfigureAwait(false);

            this.logger.LogInformation("Communications Worker SubscribeToEvents PLC {id} at: {time}", this.Id, this.dateTimeMachine.Now.ToLocalTime());
            this.SubscribeToEvents(cancellationToken);

            this.logger.LogInformation("Communications Worker {KpiOeeId} ValidateConfigurationIsCorrect at: {time}", this.Id, this.dateTimeMachine.Now.ToLocalTime());
            this.Configured = this.ValidateConfigurationIsCorrect();
            if (this.Configured)
            {
                this.logger.LogInformation("PLC {KpiOeeId} Configuration and communication successfully at: {time}", this.Id, this.dateTimeMachine.Now.ToLocalTime());
            }

            this.logger.LogInformation("Communications Worker {KpiOeeId} Configured at: {time}", this.Id, this.dateTimeMachine.Now.ToLocalTime());

            _ = Task.Run(
                async () =>
            {
                var random = new Random();
                int delay = random.Next(1000, 5001); // 2001 is exclusive upper bound
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false); // or TimeSpan.FromSeconds(2)

                // Do your work here
                await this.controller.ReadStartUp(cancellationToken).ConfigureAwait(false);
            }, cancellationToken);

            _ = Task.Run(
                async () =>
            {
                var random = new Random();
                int delay = random.Next(1000, 2001); // 2001 is exclusive upper bound
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false); // or TimeSpan.FromSeconds(2)

                // Do your work here
                await this.controller.ResetCommandAsync(cancellationToken).ConfigureAwait(false);
            }, cancellationToken);
            return this.Configured;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing command: {ErrorConstant}", ErrorExecutingCommand);
            await this.hubConnection.TryInvokeAsync(BroadcastMessageToClients, "Gateway", ErrorGatewayExecution,
                this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);
        }

        return false;
    }

    private bool ValidateConfigurationIsCorrect()
    {
        if (this.configApplicationConfiguration?.Plcs is null)
        {
            this.logger.LogError("Configuration or Plcs collection is null for PLC {RecipeId}", this.Id);
            return false;
        }

        this.PlcData = this.configApplicationConfiguration.Plcs.FirstOrDefault(x => x.PlcId == this.Id) ?? new PlcDto();

        if (this.PlcData is null)
        {
            this.logger.LogError(" PLC {RecipeId} Configuration and communication failed ", this.Id);
            return false;
        }

        if (this.Id == this.PlcData.PlcId)
        {
            this.logger.LogInformation("PLC {RecipeId} Configuration and communication Ok ", this.Id);
            return true;
        }
        else
        {
            this.logger.LogError(" PLC {RecipeId} Configuration and communication failed ", this.Id);
            return false;
        }
    }

    private async Task SetupCommunicationsPlcs(CancellationToken cancellationToken)
    {
        if (this.configApplicationConfiguration == null)
        {
            throw new InvalidDataException("App Details is null");
        }

        if (this.configApplicationConfiguration.Plcs == null)
        {
            throw new InvalidDataException("Plc Details is null");
        }

        if (this.configApplicationConfiguration.Machines == null)
        {
            throw new InvalidDataException("Machines Details is null");
        }

        if (this.PlcData == null)
        {
            this.PlcData = this.configApplicationConfiguration.Plcs.FirstOrDefault(x => x.PlcId == this.Id) ?? new PlcDto();
        }

        if (this.PlcData == null)
        {
            this.logger.LogCritical("PlcId {RecipeId} not found in configuration", this.Id);
            return;
        }

        var machine = this.configApplicationConfiguration.Machines.FirstOrDefault(x => x.MachineId == this.Id);
        if (machine is null)
        {
            this.logger.LogCritical("MachineId {MachineId} not found in configuration", this.Id);
            return;
        }
        this.machineName = machine.Name;

        var id = this.PlcData.PlcId;
        this.controller = await this.SetupControllerAsync(id, this.PlcData, this.dateTimeMachine, cancellationToken).ConfigureAwait(false);

        if (this.controller is null)
        {
            this.logger.LogError("PLC {RecipeId} Configuration and communication failed ", this.Id);

            // throw new InvalidOperationException($"PLC {KpiOeeId} Configuration and communication failed");
            await this.LogErrorToHubAsync($"PLC {this.Id} Configuration and communication failed", cancellationToken).ConfigureAwait(false);
            return;
        }

        if (this.controller is not null)
        {
            var message = this.controller.MachineId == id ? $"PlcId {id} Configured" : $"PlcId {id} Error configuring";

            await this.hubConnection.TryInvokeAsync(BroadcastMessageToClients, "Gateway", message,
                this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);
            await this.HeartBeatChangedHandler(this.controller, cancellationToken).ConfigureAwait(false);
        }
    }

    private readonly ConcurrentDictionary<int, DateTime> lastExecutionTimeStamps = new();
    private readonly double minIntervalExecutionTime = 1500;

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Communications Worker started at: {DateTime}", DateTimeOffset.Now.ToLocalTime());
        this.hubConnection = await this.hubConnection.EnsureHubConnectionIsValid(this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false) ?? this.hubConnection;
        await base.StartAsync(cancellationToken).ConfigureAwait(false);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (this.hubConnection is not null)
        {
            await this.hubConnection.StopAsync(cancellationToken).ConfigureAwait(false);
            await this.hubConnection.DisposeAsync().ConfigureAwait(false);
        }
        await base.StopAsync(cancellationToken).ConfigureAwait(false);
    }

    private void LogInformationEveryFiveMinutes()
    {
        if (this.dateTimeMachine.Now.Minute % 5 != 0 || this.dateTimeMachine.Now.Second != 0)
        {
            return;
        }

        this.logger.LogInformation("Communications Worker for plc {id} running at: {DateTime}", this.Id, this.dateTimeMachine.Now.ToLocalTime());
        this.logger.LogInformation("Last Request executed at {LasTimeCommandExecuted}", this.lasTimeStampCommandExecuted);
    }

    private async Task LogErrorToHubAsync(string message, CancellationToken cancellationToken)
    {
        if (this.hubConnection is not null)
        {
            await this.hubConnection.TryInvokeAsync(BroadcastMessageToClients, "Gateway", message,
                this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task HeartBeatChangedHandler(IIndTraceControllerRx controller, CancellationToken cancellationToken)
    {
        if (!this.IsHubConnected)
        {
            this.hubConnection = await this.hubConnection.EnsureHubConnectionIsValid(this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false) ?? this.hubConnection;
        }

        try
        {
            if (controller is not null)
            {
                var monitor = await controller.GetPlcMonitorAsync(cancellationToken).ConfigureAwait(false);
                await this.hubConnection.TryInvokeAsync(BroadcastHeartbeatSignal, controller.PlcId, monitor, this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (TaskCanceledException)
        {
            // Log task cancellation
            if (controller is not null)
            {
                this.logger.LogWarning("Heartbeat task was canceled for PLC {PlcId}", controller.PlcId);
            }
        }
        catch (HubException ex)
        {
            this.logger.LogError(ex, ErrorWhileSendingHeartbeat);
            if (this.hubConnection is not null)
            {
                await this.hubConnection.TryInvokeAsync(BroadcastMessageToClients, "Gateway", ErrorWhileSendingHeartbeat,
                    this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, ErrorWhileSendingHeartbeat);
        }
    }

    private IDisposable? heartbeatSubscription;
    private CancellationTokenSource? heartbeatCts;
    private readonly Lock syncLock = new();
    private IDisposable? commandChangedSubscription;
    private bool isCommandHandling;
    private readonly TimeSpan resubscribeDelay = TimeSpan.FromMilliseconds(20);
    private IHubConnection hubConnection = hubConnection;

    private async Task CommandChangedHandler(IIndTraceControllerRx controller, CancellationToken cancellationToken)
    {
        if (this.ShouldSkipCommandProcessing())
        {
            return;
        }

        try
        {
            this.logger.LogInformation("Handling command {Request} from controller {RecipeId}", controller.Command, this.Id);

            if (this.CommandIsFiringBeforeAcceptedInterval(this.Id))
            {
                this.logger.LogWarning("Command fired too quickly {PlcId}", this.Id);
                var message = $"Command fired too quickly {this.Id}";
                await this.hubConnection.TryInvokeAsync(BroadcastMessageToClients, "Gateway", message,
                    this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);
                return;
            }

            if (!this.IsHubConnected)
            {
                EnsureHubConnectionInBackground(this.hubConnection, this.connectionFactory, this.logger, cancellationToken);
            }

            await controller.HandleCommandAsync(commandDispatcher, this.hubConnection, this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);
            await Task.Delay(this.resubscribeDelay, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            lock (this.syncLock)
            {
                this.isCommandHandling = false;
            }

            this.SubscribeToEvents(cancellationToken); // Re-subscribe after command is fully processed
        }
    }

    private bool ShouldSkipCommandProcessing()
    {
        lock (this.syncLock)
        {
            if (this.isCommandHandling)
            {
                this.logger.LogWarning("Command already in progress. New command skipped.");
                return true;
            }
            else
            {
                this.isCommandHandling = true;
                this.UnsubscribeFromEvents();
                return false;
            }
        }
    }

    private bool CommandIsFiringBeforeAcceptedInterval(int machineId)
    {
        this.lasTimeStampCommandExecuted = this.dateTimeMachine.Now.ToLocalTime();
        if (this.lastExecutionTimeStamps.TryGetValue(machineId, out var lastExecutionTime) &&
            (this.lasTimeStampCommandExecuted - lastExecutionTime).TotalMilliseconds < this.minIntervalExecutionTime)
        {
            this.logger.LogInformation("Command for MachineId {MachineId} was executed too recently. Skipping execution.", this.Id);
            return true;
        }

        this.lastExecutionTimeStamps[machineId] = this.lasTimeStampCommandExecuted;
        return false;
    }

    private async Task<IndTrace.S7Rx.Interfaces.IIndTraceControllerRx> SetupControllerAsync(int key, PlcDto value, DateTimeMachine dateTimeMachine, CancellationToken cancellationToken)
    {
        try
        {
            this.controller = await value.AddControllerAsync(this.logger, this.hubConnection, this.connectionFactory, dateTimeMachine, cancellationToken).ConfigureAwait(false);

            var plcConfigured = await this.controller.ConfigureControllerAsync(key, this.logger, this.hubConnection, this.connectionFactory, cancellationToken).ConfigureAwait(false);

            var variablesConfigured = await this.controller.ValidateVariablesAsync(key, this.logger, this.hubConnection, this.connectionFactory, cancellationToken).ConfigureAwait(false);

            var plcId = await this.controller.ConnectToControllerAsync((short)key, this.logger, this.hubConnection, this.connectionFactory, cancellationToken).ConfigureAwait(false);

            await this.logger.LogPlcConnectionStatusAsync(key, plcId, this.hubConnection, this.connectionFactory, cancellationToken).ConfigureAwait(false);

            return this.controller;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error executing command: {ErrorConstant}", ErrorExecutingCommand);
            var message = $"PlcId {key} Error configuring";
            if (this.hubConnection is not null)
            {
                await this.hubConnection.TryInvokeAsync(BroadcastMessageToClients, "Gateway", message,
                    this.connectionFactory, this.logger, cancellationToken).ConfigureAwait(false);
            }
        }

        return default!;
    }

    private void SubscribeToEvents(CancellationToken cancellationToken)
    {
        if (this.controller is null)
        {
            throw new InvalidDataException("Plc is null");
        }

        this.commandChangedSubscription = this.controller.CommandChanged.Subscribe(e =>
            this.SafeFireAndForget(() => this.CommandChangedHandler(e, cancellationToken), nameof(this.CommandChangedHandler)));

        this.heartbeatCts = new CancellationTokenSource();
        var localToken = this.heartbeatCts.Token;

        this.heartbeatSubscription = this.controller.HeartBeatChanged.Subscribe(e =>
            this.SafeFireAndForget(() => this.HeartBeatChangedHandler(e, localToken), nameof(this.HeartBeatChangedHandler)));
    }

    private void UnsubscribeFromEvents()
    {
        lock (this.syncLock)
        {
            this.commandChangedSubscription?.Dispose();
            this.commandChangedSubscription = null;

            this.heartbeatSubscription?.Dispose();
            this.heartbeatSubscription = null;

            this.heartbeatCts?.Cancel();
            this.heartbeatCts = null;
        }
    }

    private static void EnsureHubConnectionInBackground(
        IHubConnection? currentConnection,
        IHubConnectionFactory factory,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        _ = Task.Run(
            async () =>
        {
            try
            {
                await currentConnection.EnsureHubConnectionIsValid(factory, logger, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Background task failed: EnsureHubConnectionIsValid");
            }
        }, cancellationToken);
    }

    private void SafeFireAndForget(Func<Task> action, string context)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await action().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.logger?.LogError(ex, "Unhandled exception in {Context}", context);
            }
        });
    }

    public async Task SimulateCommand(SimulatedCommand command, CancellationToken cancellationToken)
    {
        try
        {
            if (command.MachineId == this.Id)
            {
                this.logger.LogInformation("Simulating Request");
                await this.controller.SimulateCommandAsync(command, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Failed to parse Request.");
        }
    }

    // Expose a public method to trigger the event
    public async Task TriggerCommand(IndTrace.S7Rx.Models.SimulatedCommand command)
    {
        if (this.OnCommandReceived != null)
        {
            await this.OnCommandReceived.Invoke(command).ConfigureAwait(false);
        }
        else
        {
            this.logger.LogWarning("No handlers registered for SimulateCommand.");
        }
    }

    // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate gateway worker logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    // TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated worker or task logic. Refactor for maintainability if necessary.
    // TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency gateway operations, consider optimizing task scheduling and resource usage.
}
