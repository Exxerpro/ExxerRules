namespace IndTrace.HubConnection.Contracts;

public static class HubMethods
{
    public const string BroadcastMessageToClients = nameof(BroadcastMessageToClients);
    public const string BroadcastTaskGatewayRequest = nameof(BroadcastTaskGatewayRequest);
    public const string BroadcastTaskGatewayResponse = nameof(BroadcastTaskGatewayResponse);
    public const string BroadcastHeartbeatSignal = nameof(BroadcastHeartbeatSignal);
}
