namespace IndTrace.HubConnection.Abstractions;

/// <summary>
/// Factory that creates configured <see cref="IHubConnection"/> instances.
/// Does not start the connection; callers decide when to start.
/// </summary>
public interface IHubConnectionFactory
{
    Task<IHubConnection> CreateAsync(CancellationToken cancellationToken = default);
}
