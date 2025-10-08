using IndTrace.Application.UI.Models;
using IndTrace.Application.UI.Services;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;
using IndTrace.Domain.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using static IndTrace.HubConnection.Contracts.HubMethods;

namespace IndTrace.Monitor.Worker;

/// <summary>
/// Background service that manages SignalR hub connections for monitoring IndTrace events.
/// </summary>
/// <param name="hubConnectionFactory">Factory for creating hub connections.</param>
/// <param name="eventsService">Service for handling IndTrace events.</param>
/// <param name="logger">Logger instance for the worker.</param>
public class HubMonitorWorker(
    IHubConnectionFactory hubConnectionFactory,
    IndTraceEventsService eventsService,
    ILogger<HubMonitorWorker> logger)
    : BackgroundService
{
    private IHubConnection? hubConnection;

    /// <summary>
    /// Gets a value indicating whether the hub connection is established and connected.
    /// </summary>
    public bool IsHubConnected =>
        this.hubConnection is not null && this.hubConnection.State == HubConnectionState.Connected;

    /// <summary>
    /// Executes the background service, maintaining the hub connection and handling events.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token to stop the service.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() => logger.LogInformation("Hub Monitor Worker is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await this.SetupAndStartHubConnectionAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
                logger.LogInformation("Hub Monitor Worker running at: {time}", DateTimeOffset.Now.ToLocalTime());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred in the Hub Monitor Worker.");
            }
        }

        logger.LogInformation("Hub Monitor Worker is stopping.");
    }

    /// <summary>
    /// Stops the background service and disposes of the hub connection.
    /// </summary>
    /// <param name="stoppingToken">Cancellation token for the stop operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Hub Monitor Worker is stopping.");

        if (this.hubConnection is not null)
        {
            await this.hubConnection.StopAsync(stoppingToken);
            await this.hubConnection.DisposeAsync();
        }

        await base.StopAsync(stoppingToken);
    }

    /// <summary>
    /// Sets up and starts the SignalR hub connection with event handlers.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SetupAndStartHubConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.InitializeHubConnectionAsync(cancellationToken);
            this.RegisterHubEventHandlers(cancellationToken);
            this.AddConnectionHandlers(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting SignalR connection");
        }
    }

    /// <summary>
    /// Initializes the hub connection and attempts to start it if not already connected.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task InitializeHubConnectionAsync(CancellationToken cancellationToken)
    {
        this.hubConnection ??= await hubConnectionFactory.CreateAsync(cancellationToken);

        if (!this.IsHubConnected)
        {
            await this.hubConnection.TryStartHubConnectionAsync(logger, cancellationToken);
            logger.LogInformation("Attempting to start HubConnection...");
        }

        if (this.IsHubConnected)
        {
            logger.LogInformation("Hub Monitor Connected at {DateTime}", DateTimeOffset.Now.ToLocalTime());
            await this.hubConnection.SendAsync(BroadcastMessageToClients, new object?[] { "Gateway", "Connected at Hub Monitor" }, cancellationToken);
        }
    }

    /// <summary>
    /// Registers event handlers for various SignalR hub events.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    private void RegisterHubEventHandlers(CancellationToken cancellationToken)
    {
        if (this.hubConnection is null) return;
        // Inline the method calls directly into the event handlers
        this.hubConnection.On<string, string>(
            BroadcastMessageToClients,
            (string user, string message) =>
            {
                try
                {
                    eventsService.PushMessage(user, message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }
                return Task.CompletedTask;
            });

        this.hubConnection.On<int, ControllerMonitor>(
            BroadcastHeartbeatSignal,
            (int plc, ControllerMonitor controller) =>
            {
                try
                {
                    eventsService.AddOrUpdateControllerFromGateway(plc, controller);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }
                return Task.CompletedTask;
            });

        this.hubConnection.On<int, TaskGatewayRequest>(
            BroadcastTaskGatewayRequest,
            (int plc, TaskGatewayRequest request) =>
            {
                try
                {
                    eventsService.AddOrUpdateTaskGatewayRequest(plc, request);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }
                return Task.CompletedTask;
            });

        this.hubConnection.On<int, TaskGatewayResponse>(
            BroadcastTaskGatewayResponse,
            (int plc, TaskGatewayResponse response) =>
            {
                try
                {
                    eventsService.AddOrUpdateTaskGatewayResponse(plc, response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }
                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Adds connection state change handlers for the hub connection.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    private void AddConnectionHandlers(CancellationToken cancellationToken)
    {
        if (this.hubConnection is null) return;
        this.hubConnection.Closed += async (error) => await this.LogConnectionStateAsync("lost");
        this.hubConnection.Reconnected += async (error) => await this.LogConnectionStateAsync("reconnected", cancellationToken);
    }

    /// <summary>
    /// Logs the connection state change and optionally sends a message to the hub.
    /// </summary>
    /// <param name="state">The connection state (e.g., "lost", "reconnected").</param>
    /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task LogConnectionStateAsync(string state, CancellationToken? cancellationToken = null)
    {
        logger.LogInformation("Hub Monitor Connection {State} at {Timestamp}", state, DateTimeOffset.Now.ToLocalTime());
        if (state == "reconnected" && cancellationToken.HasValue && this.hubConnection is not null)
        {
            await this.hubConnection.SendAsync(BroadcastMessageToClients, new object?[] { "Gateway", "Reconnected at Hub Monitor" }, cancellationToken.Value);
        }
    }

    /// <summary>
    /// Disposes of the hub connection and releases resources.
    /// </summary>
    public override void Dispose()
    {
        if (this.hubConnection is not null)
        {
            this.hubConnection.DisposeAsync().GetAwaiter().GetResult();
        }

        base.Dispose();
    }
}
