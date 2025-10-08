namespace IndTrace.HubConnection.Dashboard;

using IndTrace.HubConnection.Abstractions;

/// <summary>
/// Decorator for <see cref="IHubConnectionFactory"/> that registers created connections
/// with the <see cref="IHubMetricsDashboard"/> for observability. This class preserves
/// original factory behavior and adds non-blocking registration side-effects.
/// </summary>
public sealed class MetricsRegisteringHubConnectionFactory : IHubConnectionFactory
{
    private readonly IHubConnectionFactory innerFactory;
    private readonly IHubMetricsDashboard dashboard;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricsRegisteringHubConnectionFactory"/> class.
    /// </summary>
    /// <param name="innerFactory">The underlying factory that actually creates connections.</param>
    /// <param name="dashboard">The dashboard where created connections will be registered.</param>
    public MetricsRegisteringHubConnectionFactory(IHubConnectionFactory innerFactory, IHubMetricsDashboard dashboard)
    {
        this.innerFactory = innerFactory ?? throw new ArgumentNullException(nameof(innerFactory));
        this.dashboard = dashboard ?? throw new ArgumentNullException(nameof(dashboard));
    }

    /// <summary>
    /// Creates a new <see cref="IHubConnection"/> and registers it with the metrics dashboard.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created connection.</returns>
    public async Task<IHubConnection> CreateAsync(CancellationToken cancellationToken = default)
    {
        var connection = await this.innerFactory.CreateAsync(cancellationToken).ConfigureAwait(false);
        // Registration must be exception-safe and non-blocking by contract
        try
        {
            this.dashboard.RegisterConnection(connection);
        }
        catch
        {
            // Dashboard failures must never affect connection creation
        }

        return connection;
    }
}
