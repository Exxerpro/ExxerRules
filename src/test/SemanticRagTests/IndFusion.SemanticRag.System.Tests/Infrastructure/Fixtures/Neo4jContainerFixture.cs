using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.Infrastructure.Factories;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Utilities;
using Neo4j.Driver;
using Testcontainers.Neo4j;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;

/// <summary>
/// Neo4j graph database container fixture for SemanticRag system tests.
/// Implements xUnit v3 IAsyncLifetime pattern for proper container lifecycle management.
/// Provides containerized Neo4j instance for graph relationship and traversal testing.
/// Handles Docker unavailability gracefully - when Docker is not available, IsAvailable is set to false
/// and tests should skip gracefully using SkipException.
/// </summary>
public sealed class Neo4jContainerFixture : IAsyncLifetime
{
    private Neo4jContainer? _container;
    private IDriver? _driver;
    private readonly ILogger<Neo4jContainerFixture> _logger;

    /// <summary>
    /// Gets the hostname where the Neo4j container is accessible.
    /// </summary>
    public string Hostname => _container?.Hostname ?? "localhost";

    /// <summary>
    /// Gets the mapped Bolt port for Neo4j (default: 7687).
    /// </summary>
    public int BoltPort => _container?.GetMappedPublicPort(7687) ?? 7687;

    /// <summary>
    /// Gets the mapped HTTP port for Neo4j (default: 7474).
    /// </summary>
    public int HttpPort => _container?.GetMappedPublicPort(7474) ?? 7474;

    /// <summary>
    /// Gets the Neo4j username.
    /// </summary>
    public string Username => "neo4j";

    /// <summary>
    /// Gets the Neo4j password.
    /// </summary>
    public string Password => "password";

    /// <summary>
    /// Gets the Bolt URI for Neo4j connection.
    /// </summary>
    public string BoltUri { get; private set; } = null!;

    /// <summary>
    /// Gets the Neo4j configuration options with container endpoints.
    /// </summary>
    public Neo4jOptions Options { get; private set; } = null!;

    /// <summary>
    /// Gets the Neo4j driver instance connected to the container.
    /// </summary>
    public IDriver? Driver => _driver;

    /// <summary>
    /// Gets a value indicating whether the Neo4j container is running and ready.
    /// </summary>
    public bool IsAvailable => _container != null && _driver != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jContainerFixture"/> class.
    /// </summary>
    public Neo4jContainerFixture()
    {
        _logger = XUnitLogger.CreateLogger<Neo4jContainerFixture>();
    }

    /// <summary>
    /// Asynchronously initializes the Neo4j container.
    /// Starts the container, waits for readiness, and creates the driver connection.
    /// Handles Docker unavailability gracefully by setting IsAvailable to false.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous initialization operation</returns>
    public async ValueTask InitializeAsync()
    {
        try
        {
            _logger.LogInformation("🚀 Initializing Neo4j container for SemanticRag system tests...");

            _container = new Neo4jBuilder()
                .WithImage("neo4j:5.15-community")
                .WithEnvironment("NEO4J_AUTH", $"{Username}/{Password}")
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .Build();

            _logger.LogInformation("⏳ Starting Neo4j container...");
            await _container.StartAsync();

            BoltUri = _container.GetConnectionString();
            Options = new Neo4jOptions
            {
                Uri = BoltUri,
                Username = Username,
                Password = Password,
                Database = "neo4j",
                MaxConnectionPoolSize = 50
            };

            var optionsWrapper = Microsoft.Extensions.Options.Options.Create(Options);
            var factory = new Neo4jDriverFactory(optionsWrapper);
            _driver = factory.CreateDriver();

            _logger.LogInformation("✅ Neo4j container started - Bolt: {BoltUri}", BoltUri);
            _logger.LogInformation("📊 Credentials: {Username}/{Password}", Username, Password);
        }
        catch (Exception ex) when (ex.Message.Contains("Docker", StringComparison.OrdinalIgnoreCase) ||
                                  ex.Message.Contains("docker", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(ex, "⚠️ Docker-related error - Neo4j container will not be started");
            DockerSkipConditions.ShouldSkipDockerTests = true;

            BoltUri = "bolt://localhost:7687";
            Options = new Neo4jOptions
            {
                Uri = BoltUri,
                Username = Username,
                Password = Password,
                Database = "neo4j",
                MaxConnectionPoolSize = 50
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to initialize Neo4j container");
            throw;
        }
    }

    /// <summary>
    /// Asynchronously disposes of the Neo4j container resources.
    /// Ensures proper cleanup of Docker containers and driver connections.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous disposal operation</returns>
    public async ValueTask DisposeAsync()
    {
        try
        {
            _logger.LogInformation("🧹 Cleaning up Neo4j container...");

            if (_driver != null)
            {
                await _driver.DisposeAsync();
                _logger.LogInformation("✅ Neo4j driver disposed");
            }

            if (_container != null)
            {
                await _container.DisposeAsync();
                _logger.LogInformation("✅ Neo4j container cleaned up successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error during Neo4j container cleanup (non-fatal)");
        }
    }
}