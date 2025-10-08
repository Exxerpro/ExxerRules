using IndTrace.Application.Notifications;
using IndTrace.Application.UI.Models;
using IndTrace.DataStore.Interfaces;
using IndTrace.DataStore.ModelsComs;
using IndTrace.DataStore.Services.OEE;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;
using IndTrace.Domain.Entities;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using static IndTrace.HubConnection.Contracts.HubMethods;
using System.Diagnostics;
using IndTrace.DataStore.Services.OEE.Interfaces;

namespace IndTrace.DataStore.Services.OEE;

/// <summary>
/// Handles ingestion of S7 PLC data via SignalR hub events for OEE monitoring.
/// Responsible for connecting to the hub, subscribing to events, and managing the ingestion lifecycle.
/// </summary>
public class PerformanceDataCollector : IPerformanceDataCollector
{
    private readonly IHubConnectionFactory connectionFactory;
    private readonly ILogger<PerformanceDataCollector> logger;
    private readonly IRepoPlcService repoPlcService;
    private readonly IPlcManager plcManager;
    private readonly IChannelBroker<PerformanceData> broker;

    /// <summary>
    /// Gets the current SignalR hub connection.
    /// </summary>
    public IHubConnection Connection { get; private set; } = null!;

    private ConcurrentQueue<int> messagesList = new();
    private int errorCounter;
    private int messageCounter;

    private bool ingestDataFromS7;
    private Random random = new Random();

    private IReadOnlyDictionary<int, PlcData> plcs = new Dictionary<int, PlcData>();

    private SemaphoreSlim ingestSemaphore = new SemaphoreSlim(1, 1);

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceDataCollector"/> class.
    /// </summary>
    /// <param name="connectionFactory">Factory for creating SignalR hub connections.</param>
    /// <param name="logger">Logger for tracking operations and errors.</param>
    /// <param name="repoPlcService">Service for loading PLC and tag data from repository.</param>
    /// <param name="plcManager">Manager for coordinating PLC operations.</param>
    /// <param name="broker">Channel broker for asynchronous data processing.</param>
    public PerformanceDataCollector(
        IHubConnectionFactory connectionFactory,
        ILogger<PerformanceDataCollector> logger,
        IRepoPlcService repoPlcService,
        IPlcManager plcManager,
        IChannelBroker<PerformanceData> broker)
    {
        this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.repoPlcService = repoPlcService ?? throw new ArgumentNullException(nameof(repoPlcService));
        this.plcManager = plcManager ?? throw new ArgumentNullException(nameof(plcManager));
        this.broker = broker ?? throw new ArgumentNullException(nameof(broker));
    }

    /// <summary>
    /// Initializes the hub connection, registers event handlers, and loads PLC/DB info from the database.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous startup operation.</returns>
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            // Exit gracefully if the operation is cancelled
            this.logger.LogInformation("PerformanceDataCollector starting operation was cancelled before initialization.");
            return;
        }

        this.ValidateNullServicesValues();

        try
        {
            this.logger.LogInformation("Starting PerformanceDataCollector configuration at {DateTime}", DateTimeOffset.Now.ToLocalTime());
            await this.ConnectToHubServerAsync(cancellationToken);

            await this.InitializeHubConnectionAsync(cancellationToken);
            this.RegisterHubEventHandlers(cancellationToken);
            this.AddConnectionHandlers(cancellationToken);

            var plcResult = await this.repoPlcService.LoadPlcsDataAsync(cancellationToken);

            if (!plcResult.IsSuccess)
            {
                this.logger.LogError("PLC loading failed: {Error}", @plcResult.Errors);
                return;
            }

            if (plcResult.Value is null)
            {
                this.logger.LogWarning("No PLCs found in the database. Please ensure PLCs are configured correctly.");
                throw new InvalidOperationException("No PLCs found in the database. Please ensure PLCs are configured correctly.");
            }

            var tagResult = await this.repoPlcService.LoadTagsDataAsync(plcResult.Value.Keys, TagsGroups.PerformanceTags.Value, cancellationToken);
            if (!tagResult.IsSuccess)
            {
                this.logger.LogError("TagDataStore loading failed: {Error}", @tagResult.Errors);
                return;
            }
            if (tagResult.Value is null)
            {
                this.logger.LogWarning("No tags found in the database. Please ensure tags are configured correctly.");
                throw new InvalidOperationException("No tags found in the database. Please ensure tags are configured correctly.");
            }

            this.repoPlcService.VerifyOeeIsEnabled(plcResult.Value, tagResult.Value);

            var result = await this.plcManager.InitializeAsync(plcResult.Value, tagResult.Value);

            if (!result.IsSuccess)
            {
                this.logger.LogError("S7 Manager initialization failed: {Error}", result.Errors);
                throw new InvalidOperationException("S7 Manager initialization failed. Please check the PLC and tag configurations.");
            }

            this.plcs = plcResult.Value;
        }
        catch (OperationCanceledException)
        {
            this.logger.LogInformation("PerformanceDataCollector starting operation was cancelled.");
            return; // Exit gracefully if the operation is cancelled
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error starting PerformanceDataCollector connection");
            throw new InvalidOperationException("An error occurred while starting the PerformanceDataCollector. Please check the logs for more details.", ex);
        }
    }

    /// <summary>
    /// Validates that all required service dependencies are properly injected.
    /// </summary>
    /// <returns>True if all services are valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when any required service is null.</exception>
    private bool ValidateNullServicesValues()
    {
        if (this.logger is null)
        {
            throw new ArgumentNullException(nameof(this.logger), "Logger cannot be null.");
        }

        if (this.repoPlcService is null)
        {
            this.logger.LogError("Repository PLC Service cannot be null.");
            throw new ArgumentNullException(nameof(this.repoPlcService), "Repository PLC Service cannot be null.");
        }
        if (this.plcManager is null)
        {
            this.logger.LogError("S7 Manager cannot be null.");
            throw new ArgumentNullException(nameof(this.plcManager), "S7 Manager cannot be null.");
        }

        if (this.connectionFactory is null)
        {
            this.logger.LogError("Connection Factory cannot be null.");
            throw new ArgumentNullException(nameof(this.connectionFactory), "Connection Factory cannot be null.");
        }

        if (this.broker is null)
        {
            this.logger.LogError("Broker cannot be null.");
            throw new ArgumentNullException(nameof(this.broker), "Broker cannot be null.");
        }

        return true;
    }

    /// <summary>
    /// Continuously ingests end-of-cycle events from the hub and fetches performance data from S7 PLCs.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A task representing the asynchronous data collection operation.</returns>
    public async Task DispatchPerformanceSnapshotAsync(CancellationToken cancellationToken)
    {
        //we need to ensure that the PLCs are loaded before starting ingestion
        // and that the Hub connection is established
        // we prefer to shutdown the app if there is nothing to ingest
        if (this.plcs is null || !this.plcs.Any())
        {
            throw new InvalidOperationException("No PLCs available for ingestion. Please ensure the PLCs are loaded before starting ingestion.");
        }

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // We need to ensure the Hub is always connected if the connection is lost persist on reconnecting
                if (this.HubIsNotConnected)
                {
                    var timeDelay = this.random.Next(1001, 5001);
                    this.logger.LogWarning("Hub connection is not established. Retrying in an {delay} milliseconds", timeDelay);
                    await Task.Delay(timeDelay, cancellationToken).ConfigureAwait(false); ; // Wait before retrying
                    await this.ConnectToHubServerAsync(cancellationToken).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    if (this.ingestDataFromS7)
                    {
                        this.messageCounter++;

                        if (!this.messagesList.TryDequeue(out var plcId)) continue; // If no message is available, skip to the next iteration

                        if (plcId <= 0)
                        {
                            this.logger.LogWarning("Invalid PLC ID received: {PlcId}. Skipping ingestion.", plcId);
                            continue;
                        }

                        if (!this.plcs.TryGetValue(plcId, out var plc))
                        {
                            this.logger.LogWarning("PLC with ID {PlcId} not found in the list.", plcId);
                            continue;
                        }

                        this.logger.LogInformation("Performance data fetched from S7 PLCs.");

                        //Use a semaphore or lock to ensure thread safety when accessing shared resources
                        await this.ingestSemaphore.WaitAsync(cancellationToken);
                        try
                        {
                            var performanceData = await this.plcManager.ReadPerformanceDataAsync(plcId, cancellationToken).ConfigureAwait(false);

                            if (performanceData.IsSuccess && performanceData.Value is not null)
                            {
                                await this.broker.WriteAsync(performanceData.Value, cancellationToken).ConfigureAwait(false);
                            }
                        }
                        finally
                        {
                            this.ingestSemaphore.Release();
                        }
                    }
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Error fetching performance data from S7 PLCs: {Message}", e.Message);
                }
                finally
                {
                    this.ingestDataFromS7 = false; // Reset the flag after processing
                    this.errorCounter++;
                }
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error during Hub S7 ingestion");
        }
    }

    /// <summary>
    /// Attempts to connect to the SignalR hub server if disconnected.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A task representing the connection attempt.</returns>
    private async Task ConnectToHubServerAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (this.Connection.State == HubConnectionState.Disconnected)
            {
                await this.Connection.StartAsync(cancellationToken);
            }
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Failed to connect to hub");
        }
    }

    /// <summary>
    /// Initializes the SignalR hub connection and attempts to start it if not already connected.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>A task representing the initialization operation.</returns>
    private async Task InitializeHubConnectionAsync(CancellationToken cancellationToken)
    {
        this.Connection ??= await this.connectionFactory.CreateAsync(cancellationToken).ConfigureAwait(false);

        if (this.HubIsNotConnected)
        {
            var timeDelay = this.random.Next(1001, 5001);
            await Task.Delay(timeDelay, cancellationToken).ConfigureAwait(false); // Wait before retrying
            await this.Connection.TryStartHubConnectionAsync(this.logger, cancellationToken);
            this.logger.LogInformation("Attempting to start HubConnection...");
        }

        if (!this.HubIsNotConnected)
        {
            this.logger.LogInformation("Hub Monitor Connected at {DateTime}", DateTimeOffset.Now.ToLocalTime());
            await this.Connection.SendAsync(BroadcastMessageToClients, new object?[] { "Gateway", "Connected at Hub Monitor" }, cancellationToken);
        }
    }

    /// <summary>
    /// Registers SignalR hub event handlers for connection and custom events.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    private void RegisterHubEventHandlers(CancellationToken cancellationToken)
    {
        // Inline the method calls directly into the event handlers

        this.Connection.Reconnecting += error =>
        {
            Debug.Assert(this.Connection.State == HubConnectionState.Reconnecting);
            this.logger.LogWarning("Connection lost. Attempting to reconnect...");
            return Task.CompletedTask;
        };

        this.Connection.Reconnected += connectionId =>
        {
            Debug.Assert(this.Connection.State == HubConnectionState.Connected);
            this.logger.LogInformation("Reconnected to Hub successfully.");
            return Task.CompletedTask;
        };

        //Managed already bellow, add a method to handle the event
        this.Connection.On<string, string>("BroadcastMessageToClients", (string user, string message) =>
            {
                try
                {
                    this.OnBroadcastMessageReceived(user, message);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }

                return Task.CompletedTask;
            });

        this.Connection.On<int, TaskGatewayRequest>(
            "BroadcastTaskGatewayRequest",
            (int plc, TaskGatewayRequest request) =>
            {
                try
                {
                    this.OnStartOfCycle(request, plc);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }

                return Task.CompletedTask;
            });

        //this event is used to broadcast the end of a cycle event to the clients
        this.Connection.On<int, TaskGatewayResponse>(
            "BroadcastTaskGatewayResponse",
            (int plc, TaskGatewayResponse response) =>
            {
                try
                {
                    this.OnEndOfCycle(response, plc);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }

                return Task.CompletedTask;
            });
    }

    /// <summary>
    /// Handles the start of a cycle event from the hub.
    /// </summary>
    /// <param name="controller">The task gateway request data.</param>
    /// <param name="plc">The PLC ID.</param>
    private void OnStartOfCycle(TaskGatewayRequest controller, int plc)
    {
        this.logger.LogInformation("TaskGatewayRequest signal received from PLC {PlcId}. Ingesting data from S7.", plc);
    }

    /// <summary>
    /// Handles the end of a cycle event from the hub, triggers S7 ingestion.
    /// </summary>
    /// <param name="controller">The task gateway response data.</param>
    /// <param name="plc">The PLC ID.</param>
    private void OnEndOfCycle(TaskGatewayResponse controller, int plc)
    {
        //Create method to manage the end of cycle event

        this.ingestDataFromS7 = true; // Set the flag to true to start ingesting data from S7
        this.messagesList.Enqueue(plc); // Enqueue the PLC ID to the messages list
        this.logger.LogInformation("TaskGatewayResponse signal received from PLC {PlcId}. Ingesting data from S7.", plc);
    }

    /// <summary>
    /// Handles broadcast messages received from the hub.
    /// </summary>
    /// <param name="user">The user who sent the message.</param>
    /// <param name="message">The message content.</param>
    private void OnBroadcastMessageReceived(string user, string message)
    {
        this.logger.LogInformation("Received Message from {user}: {message}", user, message);
    }

    /// <summary>
    /// Adds connection state change handlers to the SignalR hub connection.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    private void AddConnectionHandlers(CancellationToken cancellationToken)
    {
        this.Connection.Closed += async (error) => await this.LogConnectionStateAsync("lost");
        this.Connection.Reconnected += async (id) => await this.LogConnectionStateAsync("reconnected", cancellationToken);
    }

    /// <summary>
    /// Logs the connection state changes for the SignalR hub connection.
    /// </summary>
    /// <param name="state">The new connection state.</param>
    /// <param name="cancellationToken">Optional cancellation token for async operations.</param>
    /// <returns>A task representing the logging operation.</returns>
    private async Task LogConnectionStateAsync(string state, CancellationToken? cancellationToken = null)
    {
        this.logger.LogInformation("Hub Monitor Connection {State} at {Timestamp}", state, DateTimeOffset.Now.ToLocalTime());
        if (state == "reconnected" && cancellationToken.HasValue)
        {
            await this.Connection.SendAsync(BroadcastMessageToClients, new object?[] { "Gateway", "Reconnected at Hub Monitor" }, cancellationToken.Value);
        }
    }

    /// <summary>
    /// Gets a value indicating whether the hub is not connected.
    /// </summary>
    private bool HubIsNotConnected => this.Connection is null || this.Connection.State == HubConnectionState.Connected;

    /// <summary>
    /// Gets a value indicating whether the hub is connected.
    /// </summary>
    public bool IsHubConnected => !this.HubIsNotConnected;

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate performance data collector logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    //TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated data collection or processing logic. Refactor for maintainability if necessary.
    //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency data collection, consider optimizing processing and memory usage.
}
