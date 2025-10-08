using System.Diagnostics;
using IndTrace.DataStore.Exceptions;
using IndTrace.DataStore.ModelsComs;
using IndTrace.DataStore.Services.OEE.Interfaces;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;
using IndTrace.Domain.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using static IndTrace.HubConnection.Contracts.HubMethods;

namespace IndTrace.DataStore.Services.OEE.Services;

/// <summary>
/// Handles ingestion of S7 PLC data via SignalR hub events for OEE monitoring.
/// Responsible for connecting to the hub, subscribing to events, and managing the ingestion lifecycle.
/// </summary>
public class PerformanceDataCollector(
    IHubConnectionFactory connectionFactory,
    ILogger<PerformanceDataCollector> logger,
    IRepoPlcService repoPlcService,
    IPlcManager plcManager,
    IChannelBroker<PerformanceData> broker) : IPerformanceDataCollector
{
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
    /// Initializes the hub connection, registers event handlers, and loads PLC/DB info from the database.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
    public async Task StartingAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            // Exit gracefully if the operation is cancelled
            logger.LogInformation("PerformanceDataCollector starting operation was cancelled before initialization.");
            return;
        }

        this.ValidateNullServicesValues();

        try
        {
            logger.LogInformation("Starting PerformanceDataCollector configuration at {DateTime}", DateTimeOffset.Now.ToLocalTime());
            await this.ConnectToHubServerAsync(cancellationToken);

            await this.InitializeHubConnectionAsync(cancellationToken);
            this.RegisterHubEventHandlers(cancellationToken);
            this.AddConnectionHandlers(cancellationToken);

            var plcResult = await repoPlcService.LoadPlcsDataAsync(cancellationToken);

            if (!plcResult.IsSuccess)
            {
                logger.LogError("PLC loading failed: {Error}", @plcResult.Errors);
                return;
            }

            if (plcResult.Value is null)
            {
                logger.LogWarning("No PLCs found in the database. Please ensure PLCs are configured correctly.");
                throw new IndTraceOeeException(" No PLCs found in the database. Please ensure PLCs are configured correctly.");
            }

            var tagResult = await repoPlcService.LoadTagsDataAsync(plcResult.Value.Keys, TagsGroups.PerformanceTags.Value, cancellationToken);
            if (!tagResult.IsSuccess)
            {
                logger.LogError("TagDataStore loading failed: {Error}", @tagResult.Errors);
                return;
            }
            if (tagResult.Value is null)
            {
                logger.LogWarning("No tags found in the database. Please ensure tags are configured correctly.");
                throw new IndTraceOeeException(" No tags found in the database. Please ensure tags are configured correctly.");
            }

            repoPlcService.VerifyOeeIsEnabled(plcResult.Value, tagResult.Value);

            var result = await plcManager.InitializeAsync(plcResult.Value, tagResult.Value);

            if (!result.IsSuccess)
            {
                logger.LogError("S7 Manager initialization failed: {Error}", result.Errors);
                throw new IndTraceOeeException("S7 Manager initialization failed. Please check the PLC and tag configurations.");
            }
        }
        catch (IndTraceOeeException ex)
        {
            logger.LogError(ex, "PLC configuration error occurred");
            throw; // Rethrow to ensure the application can handle this critical error
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("PerformanceDataCollector starting operation was cancelled.");
            return; // Exit gracefully if the operation is cancelled
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error starting PerformanceDataCollector connection");
            throw new IndTraceOeeException("An error occurred while starting the PerformanceDataCollector. Please check the logs for more details.", ex);
        }
    }

    private bool ValidateNullServicesValues()
    {
        // This methods are used to initialize resources

        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger), "Logger cannot be null.");
        }

        if (repoPlcService is null)
        {
            logger.LogError("Repository PLC Service cannot be null.");
            throw new ArgumentNullException(nameof(repoPlcService), "Repository PLC Service cannot be null.");
        }
        if (plcManager is null)
        {
            logger.LogError("S7 Manager cannot be null.");
            throw new ArgumentNullException(nameof(plcManager), "S7 Manager cannot be null.");
        }

        if (connectionFactory is null)
        {
            logger.LogError("Connection Factory cannot be null.");
            throw new ArgumentNullException(nameof(connectionFactory), "Connection Factory cannot be null.");
        }

        if (broker is null)
        {
            logger.LogError("Broker cannot be null.");
            throw new ArgumentNullException(nameof(broker), "Broker cannot be null.");
        }

        return true;
    }

    /// <summary>
    /// Continuously ingests end-of-cycle events from the hub and fetches performance data from S7 PLCs.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns><placeholder>A <see cref="Task"/> representing the asynchronous operation.</placeholder></returns>
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
                // I don't really like the old +- approach to event's subscription, maybe we can use and observable pattern here
                // But is working on the other app, or maybe this is why we have some problems  ??
                if (this.HubIsNotConnected)
                {
                    var timeDelay = this.random.Next(1001, 5001);
                    logger.LogWarning("Hub connection is not established. Retrying in an {delay} milliseconds", timeDelay);
                    await Task.Delay(timeDelay, cancellationToken).ConfigureAwait(false); ; // Wait before retrying
                    await this.ConnectToHubServerAsync(cancellationToken).ConfigureAwait(false);
                    continue;
                }

                try
                {
                    // maybe we can manage on an event handler ?? it seem more appropriated than a while loop
                    // Because the plc is even the callback for the end of cycle event
                    // This can be easily accomplished by using the S7Rx, but i don't want to add more load to the network
                    // and the PLCs, so we will use a simple while loop to check for the end of cycle events
                    // So we Will use the Concurrent Que to manage the messages from the hub and not the simple flag,
                    // To indicate the PLCs from where retrieve the performance data
                    // We also must be sure just the PLC id is be able to be parsed from the message

                    //TODO we need to set this flag to true when a end of cycle event is detected
                    // we must set on the hub event when the end of cycle is detected

                    //TODO CHECK on Monitor, or hub server HOW IS DECODE THIS TASK END OF CYCLE EVENT

                    if (this.ingestDataFromS7)
                    {
                        this.messageCounter++;

                        if (!this.messagesList.TryDequeue(out var plcId)) continue; // If no message is available, skip to the next iteration

                        if (plcId <= 0)
                        {
                            logger.LogWarning("Invalid PLC ID received: {PlcId}. Skipping ingestion.", plcId);
                            continue;
                        }

                        if (!this.plcs.TryGetValue(plcId, out var plc))
                        {
                            logger.LogWarning("PLC with ID {PlcId} not found in the list.", plcId);
                            continue;
                        }

                        logger.LogInformation("Performance data fetched from S7 PLCs.");

                        //Use a semaphore or lock to ensure thread safety when accessing shared resources
                        // This is to prevent multiple threads from accessing the same PLC at the same time
                        // and to ensure that we are not overloading the PLCs with requests
                        // This is also to ensure that we are not overloading the network with requests
                        // and to ensure that we are not overloading the broker with requests
                        // Because the PLC are not able to handle multiple requests at the same time
                        await this.ingestSemaphore.WaitAsync(cancellationToken);
                        try
                        {
                            var performanceData = await plcManager.ReadPerformanceDataAsync(plcId, cancellationToken).ConfigureAwait(false);
                            //TODO [URGENT] : Missing the processing of the performance data
                            // WHAT IS THE JOB OF THE REACTIVE SERVICE PROCESS THIS DATA OR WHERE IS THE PROCESSING DONE?
                            //YES IS ON THE REACTIVE SERVICE
                            if (performanceData.IsSuccess && performanceData.Value is not null)
                            {
                                await broker.WriteAsync(performanceData.Value, cancellationToken).ConfigureAwait(false);
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
                    logger.LogError(e, "Error fetching performance data from S7 PLCs: {Message}", e.Message);
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
            logger.LogError(ex, "Error during Hub S7 ingestion");
        }
    }

    /// <summary>
    /// Attempts to connect to the SignalR hub server if disconnected.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
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
            logger.LogError(e, "Failed to connect to hub");
        }
    }

    /// <summary>
    /// Initializes the SignalR hub connection and attempts to start it if not already connected.
    /// </summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    private async Task InitializeHubConnectionAsync(CancellationToken cancellationToken)
    {
        //TODO [URGENT]
        // 13 JUN 2025
        //ABR : Ensure that the connectionFactory is properly configured to create a HubConnection
        // detect the task end of cycle events and  to fetch the performance data
        // from the plc and use the broker to send the data to the OEE system

        this.Connection ??= await connectionFactory.CreateAsync(cancellationToken).ConfigureAwait(false);

        if (this.HubIsNotConnected)
        {
            var timeDelay = this.random.Next(1001, 5001);
            await Task.Delay(timeDelay, cancellationToken).ConfigureAwait(false); // Wait before retrying
            await this.Connection.TryStartHubConnectionAsync(logger, cancellationToken);
            logger.LogInformation("Attempting to start HubConnection...");
        }

        if (!this.HubIsNotConnected)
        {
            logger.LogInformation("Hub Monitor Connected at {DateTime}", DateTimeOffset.Now.ToLocalTime());
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
            logger.LogWarning("Connection lost. Attempting to reconnect...");
            return Task.CompletedTask;
        };

        this.Connection.Reconnected += connectionId =>
        {
            Debug.Assert(this.Connection.State == HubConnectionState.Connected);
            logger.LogInformation("Reconnected to Hub successfully.");
            return Task.CompletedTask;
        };

        //Managed already bellow, add a method to handle the event
        this.Connection.On<string, string>(BroadcastMessageToClients, (string user, string message) =>
            {
                try
                {
                    this.OnBroadcastMessageReceived(user, message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }

                return Task.CompletedTask;
            });

        //This event is one of the end of cycle events, we will use it to fetch the performance data from the PLCs
        //TODO CHECK which event is the one that we need to use to fetch the performance data from the PLCs

        //this event is used to broadcast the start of a cycle event to the clients
        //so at this time we don't need to handle this event
        //we can use it to be ready, to log something, to prepare the data for the next cycle
        // maybe we can use it to reset the performance data
        // maybe to to instance a new handler for the plc
        // at this time we will just register a dummy handler
        this.Connection.On<int, TaskGatewayRequest>(
            BroadcastTaskGatewayRequest,
            (int plc, TaskGatewayRequest request) =>
            {
                try
                {
                    this.OnStartOfCycle(request, plc);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
                }

                return Task.CompletedTask;
            });

        //this event is used to broadcast the end of a cycle event to the clients
        this.Connection.On<int, TaskGatewayResponse>(
            BroadcastTaskGatewayResponse,
            (int plc, TaskGatewayResponse response) =>
            {
                try
                {
                    this.OnEndOfCycle(response, plc);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error handling BroadcastMessageToClients event.");
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
        logger.LogInformation("TaskGatewayRequest signal received from PLC {PlcId}. Ingesting data from S7.", plc);
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
        logger.LogInformation("TaskGatewayResponse signal received from PLC {PlcId}. Ingesting data from S7.", plc);
    }

    /// <summary>
    /// Handles broadcast messages received from the hub.
    /// </summary>
    /// <param name="user">The user who sent the message.</param>
    /// <param name="message">The message content.</param>
    private void OnBroadcastMessageReceived(string user, string message)
    {
        logger.LogInformation("Received Message from {user}: {message}", user, message);
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
    private async Task LogConnectionStateAsync(string state, CancellationToken? cancellationToken = null)
    {
        logger.LogInformation("Hub Monitor Connection {State} at {Timestamp}", state, DateTimeOffset.Now.ToLocalTime());
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

    //private void LogPlcsAndTags(IEnumerable<PlcData> plcs, Dictionary<int, Dictionary<string, VariableS7>> tagGroups)
    //{
    //    foreach (var plc in plcs)
    //    {
    //        logger.LogInformation("PLC found: {PlcId}, Name: {Name}, IP: {IpAddress}", plc.PlcId, plc.Name, plc.IpAddress);

    //        if (tagGroups.TryGetValue(plc.PlcId, out var tags) && tags.Any())
    //        {
    //            foreach (var tag in tags.Values)
    //            {
    //                logger.LogInformation("TagDataStore found for PLC {PlcId}: {TagName} at {Address}", plc.PlcId, tag.Name, tag.Address);
    //            }
    //        }
    //        else
    //        {
    //            logger.LogWarning("No tags found for PLC {PlcId}.", plc.PlcId);
    //        }
    //    }
    //}

    //TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate performance data collector logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    //TODO [DRY][CURSOR][20/JUNE/2025] - Check for repeated data collection or processing logic. Refactor for maintainability if necessary.
    //TODO [PERFORMANCE][CURSOR][20/JUNE/2025] - For high-frequency data collection, consider optimizing processing and memory usage.
}
