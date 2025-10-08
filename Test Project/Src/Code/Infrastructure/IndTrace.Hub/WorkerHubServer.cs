// <copyright file="WorkerHubServer.cs" company="PlaceholderCompany">
// Copyright (c) Exxerpro Solutions SA de CV. All rights reserved.
// </copyright>

namespace IndTrace.Hub.Server
{
    using System.Diagnostics;
    using IndTrace.Hub.Server;
    using IndTrace.HubConnection.Abstractions;
    using IndTrace.HubConnection.Extensions;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.AspNetCore.SignalR.Client;

    /// <summary>
    /// Represents the WorkerHubServer.
    /// </summary>
    public class WorkerHubServer(ILogger<WorkerHubServer> logger, IHubContext<EventMonitorHub> hubContext, IHubConnectionFactory connectionFactory, Microsoft.Extensions.Options.IOptions<WorkerHubServerOptions> options)
        : BackgroundService
    {
        private readonly WorkerHubServerOptions workerOptions = options.Value ?? new WorkerHubServerOptions();

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("WorkerHubServer process running at: {time}", DateTimeOffset.Now.ToLocalTime());
                }

                var delayMs = Math.Max(1_000, workerOptions.HeartbeatIntervalSeconds * 1_000);
                await Task.Delay(delayMs, stoppingToken);
                await EnsureIsConnectedAsync(stoppingToken);

                if (workerOptions.EnableHeartbeat)
                {
                    await this.connection.SendAsync("SendMessage", new object?[] { "system", "" }, stoppingToken);
                }
            }
        }

        private IHubConnection connection = default!; // set during ConnectToHubServer before any use

        // Ensure primary-ctor parameter 'hubContext' is considered used to satisfy CS9113 without behavior change
        private readonly IHubContext<EventMonitorHub>? _ = hubContext;

        private async ValueTask EnsureIsConnectedAsync(CancellationToken cancellationToken)
        {
            // Standardize: use EnsureHubConnectionIsValid + TryStartHubConnectionAsync from IndTrace.HubConnection.Extensions
            this.connection = await this.connection.EnsureHubConnectionIsValid(connectionFactory, logger, cancellationToken)
                ?? this.connection; // if null on failure, keep previous reference (will retry next loop)
            if (this.connection is null)
            {
                return;
            }
            await this.connection.TryStartHubConnectionAsync(logger, cancellationToken);

            this.connection.Reconnecting += error =>
            {
                Debug.Assert(this.connection.State == HubConnectionState.Reconnecting);

                // Notify users the connection was lost and the client is reconnecting.
                // Start queuing or dropping messages.
                return Task.CompletedTask;
            };

            this.connection.Reconnected += connectionId =>
            {
                Debug.Assert(this.connection.State == HubConnectionState.Connected);

                // Notify users the connection was reestablished.
                // Start dequeuing messages queued while reconnecting if any.
                return Task.CompletedTask;
            };

            this.connection.On<string, string>("BroadcastMessageToClients", (user, message) =>
            {
                var newMessage = $" ReceiveMessage {user}: {message}";
                logger.LogInformation(newMessage);
                return Task.CompletedTask;
            });

            await Task.Delay(1_000, cancellationToken);
        }

        // TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate worker hub server logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
    }
}
