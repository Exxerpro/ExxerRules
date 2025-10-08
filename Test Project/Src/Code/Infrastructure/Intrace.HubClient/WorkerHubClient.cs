// <copyright file="WorkerHubClient.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Hub.Client;

using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Extensions;
/// <summary>
/// A background service that acts as a client for the SignalR Hub, sending and receiving simulated data.
/// </summary>
public class WorkerHubClient : BackgroundService
{
    private readonly ILogger<WorkerHubClient> logger;
    private readonly IHubConnectionFactory connectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerHubClient"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public WorkerHubClient(ILogger<WorkerHubClient> logger, IHubConnectionFactory connectionFactory)
    {
        this.logger = logger;
        this.connectionFactory = connectionFactory;
    }

    private IHubConnection connection = default!; // initialized via ConnectToHubServer before use
    private List<string> messagesList = [];
    private int counter;

    /// <summary>
    /// Executes the background service logic, connecting to the Hub and sending simulated data periodically.
    /// </summary>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await this.ConnectToHubServer(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (this.logger.IsEnabled(LogLevel.Information))
            {
                this.logger.LogInformation("WorkerHubClient running at: {time}", DateTimeOffset.Now);
            }

            if (this.counter % 30 == 0 && this.IsHubConnected)
            {
                await this.connection.SendAsync(HubMethods.ReceiveMessage, new object?[] { "user", $"Message sent by WorkerHubClient at: {DateTime.Now}" }, stoppingToken);
            }

            // Simulate sending process monitors and heartbeat data
            await this.SendSimulatedData(stoppingToken);

            await Task.Delay(3000, stoppingToken);
        }
    }

    /// <summary>
    /// Sends simulated process monitor and heartbeat data to the Hub.
    /// </summary>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task SendSimulatedData(CancellationToken stoppingToken)
    {
        var processMonitor1 = new TaskGatewayResponse()
        {
            ResultValidation = ResultValidation.Valid,
            CycleStatus = CycleStatus.Started,
            FlowStatus = FlowStatus.Created,
            PartStatus = PartStatus.Ok,
            MachineType = MachineType.Process,
            WorkFlowType = WorkFlowType.Initial,
            TimeStamp = DateTime.Now.ToLocalTime(),
            MachineId = 101,

            BarCodeId = 2001,
            CycleId = 301,
            CyclesOk = this.counter,
            ShiftId = 1,
            LastMachineId = 100 - this.counter,
            NextMachineId = this.counter,
            Label = "BatchA",
        };

        var heartBeat1 = new ControllerMonitor
        {
            TimeStamp = DateTime.Now.ToLocalTime(),
            PlcId = 100,
            MachineId = 101,
            PartNumber = "PN-12345",
            Label = "Urgent",
            HeartBeat = DateTime.Now.ToLocalTime().Second,
            CyclesOk = this.counter,
        };

        this.counter = (this.counter + 1) % 120;

        var plcId = 100;

        await this.connection.SendAsync(HubMethods.BroadcastTaskGatewayResponse, new object?[] { plcId, processMonitor1 }, stoppingToken);
        await this.connection.SendAsync(HubMethods.BroadcastHeartbeatSignal, new object?[] { plcId, heartBeat1 }, stoppingToken);
    }

    /// <summary>
    /// Connects to the SignalR Hub server.
    /// </summary>
    /// <param name="stoppingToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task ConnectToHubServer(CancellationToken stoppingToken)
    {
        this.connection = await connectionFactory.CreateAsync(stoppingToken);

        this.connection.Reconnecting += error =>
        {
            Debug.Assert(this.connection.State == HubConnectionState.Reconnecting);
            this.logger.LogWarning("Connection lost. Attempting to reconnect...");
            return Task.CompletedTask;
        };

        this.connection.Reconnected += connectionId =>
        {
            Debug.Assert(this.connection.State == HubConnectionState.Connected);
            this.logger.LogInformation("Reconnected to Hub successfully.");
            return Task.CompletedTask;
        };

        this.connection.On<string, string>(HubMethods.ReceiveMessage, (user, message) =>
        {
            var newMessage = $"Received Message from {user}: {message}";
            this.logger.LogInformation(newMessage);
            this.messagesList.Add(newMessage);
            return Task.CompletedTask;
        });

        try
        {
            await this.connection.StartAsync(stoppingToken);
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Failed to connect to hub");
        }
    }

    /// <summary>
    /// Gets a value indicating whether the Hub connection is established.
    /// </summary>
    private bool IsHubConnected => this.connection is not null && this.connection.State == HubConnectionState.Connected;
}

/// <summary>
/// Defines constant strings for SignalR Hub method names.
/// </summary>
public static class HubMethods
{
    /// <summary>
    /// Represents the "BroadcastMessageToClients" Hub method.
    /// </summary>
    public const string ReceiveMessage = "BroadcastMessageToClients";

    /// <summary>
    /// Represents the "BroadcastTaskGatewayRequest" Hub method.
    /// </summary>
    public const string BroadcastTaskGatewayRequest = "BroadcastTaskGatewayRequest";

    /// <summary>
    /// Represents the "BroadcastTaskGatewayResponse" Hub method.
    /// </summary>
    public const string BroadcastTaskGatewayResponse = "BroadcastTaskGatewayResponse";

    /// <summary>
    /// Represents the "BroadcastHeartbeatSignal" Hub method.
    /// </summary>
    public const string BroadcastHeartbeatSignal = "BroadcastHeartbeatSignal";

    // Add other hub methods if needed
}
