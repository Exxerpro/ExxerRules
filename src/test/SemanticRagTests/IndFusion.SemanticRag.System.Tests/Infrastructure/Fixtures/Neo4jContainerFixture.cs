namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;

/// <summary>
/// xUnit fixture for Neo4j container lifecycle management.
/// Provides Neo4j container, options, and driver instance for system tests.
/// </summary>
public class Neo4jContainerFixture : IAsyncLifetime
{
    private Neo4jContainer? _container;

    /// <summary>
    /// Gets the Neo4j configuration options with container endpoints.
    /// </summary>
    public Neo4jOptions Options { get; private set; } = null!;

    /// <summary>
    /// Gets the Neo4j driver instance connected to the container.
    /// </summary>
    public IDriver Driver { get; private set; } = null!;

    /// <summary>
    /// Gets the container hostname for connection.
    /// </summary>
    public string Hostname => _container?.Hostname ?? "localhost";

    /// <summary>
    /// Gets the mapped Bolt port for Neo4j.
    /// </summary>
    public int BoltPort => _container?.GetMappedPublicPort(7687) ?? 7687;

    /// <summary>
    /// Gets the mapped HTTP port for Neo4j.
    /// </summary>
    public int HttpPort => _container?.GetMappedPublicPort(7474) ?? 7474;

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        _container = new Neo4jBuilder()
            .WithImage("neo4j:5.15-community")
            .WithEnvironment("NEO4J_AUTH", "neo4j/password")
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();

        Options = new Neo4jOptions
        {
            Uri = $"bolt://{_container.Hostname}:{_container.GetMappedPublicPort(7687)}",
            Username = "neo4j",
            Password = "password",
            Database = "neo4j",
            MaxConnectionPoolSize = 50
        };

        var optionsWrapper = Microsoft.Extensions.Options.Options.Create(Options);
        var factory = new Neo4jDriverFactory(optionsWrapper);
        Driver = factory.CreateDriver();
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        Driver?.Dispose();

        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }
}