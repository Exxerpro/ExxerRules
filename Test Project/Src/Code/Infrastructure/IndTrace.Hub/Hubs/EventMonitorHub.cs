using IndTrace.Application.UI.Models;
using IndTrace.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace IndTrace.Hub.Server;

/// <summary>
/// SignalR hub for broadcasting events, heartbeats, and gateway requests/responses to connected clients.
/// </summary>
public class EventMonitorHub(ILogger<EventMonitorHub> logger) : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task BroadcastMessageToClients(string user, string message)
    {
        try
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(message))
            {
                var ex = new ArgumentException("IndTraceUser and message must not be null or empty.");
                logger.LogError(ex, "Error in BroadcastMessageToClients");
                return;
            }

            await this.Clients.All.SendAsync(nameof(this.BroadcastMessageToClients), new object?[] { user, message }, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in BroadcastMessageToClients");
        }
    }

    public async Task BroadcastHeartbeatSignal(int plc, ControllerMonitor controller)
    {
        try
        {
            if (controller == null)
            {
                var ex = new ArgumentException("PLC ID must be greater than zero and ControllerMonitor must not be null.");
                logger.LogError(ex, "Error in BroadcastHeartbeatSignal");
                return;
            }

            await this.Clients.All.SendAsync(nameof(this.BroadcastHeartbeatSignal), new object?[] { plc, controller }, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in BroadcastHeartbeatSignal");
        }
    }

    public async Task BroadcastTaskGatewayRequest(int id, TaskGatewayRequest request)
    {
        try
        {
            if (request == null)
            {
                var ex = new ArgumentException("ID must be greater than zero and monitorRequest must not be null.");
                logger.LogError(ex, "Error in BroadcastTaskGatewayRequest");
                return;
            }

            await this.Clients.All.SendAsync(nameof(this.BroadcastTaskGatewayRequest), new object?[] { id, request }, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in BroadcastTaskGatewayRequest");
        }
    }

    public async Task BroadcastTaskGatewayResponse(int id, TaskGatewayResponse response)
    {
        try
        {
            if (response == null)
            {
                var ex = new ArgumentException("ID must be greater than zero and TaskGatewayResponse must not be null.");
                logger.LogError(ex, "Error in BroadcastTaskGatewayResponse");
                return;
            }

            await this.Clients.All.SendAsync(nameof(this.BroadcastTaskGatewayResponse), new object?[] { id, response }, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in BroadcastTaskGatewayResponse");
        }
    }

    /// <summary>
    /// Echo method for testing connectivity and basic hub functionality.
    /// </summary>
    public Task<string> Echo(string message)
    {
        logger.LogDebug("Echo called with message: {Message}", message);
        return Task.FromResult($"Echo: {message}");
    }

    /// <summary>
    /// Gets the current connection count for testing purposes.
    /// </summary>
    public Task<int> GetConnectionCount()
    {
        // This is a simplified implementation for testing
        // In production, you might want to track this more accurately
        return Task.FromResult(1);
    }
}
