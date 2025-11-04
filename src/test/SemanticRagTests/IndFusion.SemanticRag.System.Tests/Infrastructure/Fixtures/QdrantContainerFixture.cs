using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Utilities;
using Qdrant.Client;
using Testcontainers.Qdrant;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;

/// <summary>
/// Qdrant vector database container fixture for SemanticRag system tests.
/// Implements xUnit v3 IAsyncLifetime pattern for proper container lifecycle management.
/// Provides containerized Qdrant instance for vector similarity search and embeddings testing.
/// Handles Docker unavailability gracefully - when Docker is not available, IsAvailable is set to false
/// and tests should skip gracefully using SkipException.
/// </summary>
public sealed class QdrantContainerFixture : IAsyncLifetime
{
    private QdrantContainer? _container;
    private readonly ILogger<QdrantContainerFixture> _logger;

    /// <summary>
    /// Gets the hostname where the Qdrant container is accessible.
    /// </summary>
    public string Hostname => _container?.Hostname ?? "localhost";

    /// <summary>
    /// Gets the mapped HTTP port for Qdrant (default: 6333).
    /// </summary>
    public int HttpPort => _container?.GetMappedPublicPort(6333) ?? 6333;

    /// <summary>
    /// Gets the mapped gRPC port for Qdrant (default: 6334).
    /// </summary>
    public int GrpcPort => _container?.GetMappedPublicPort(6334) ?? 6334;

    /// <summary>
    /// Gets the HTTP endpoint for Qdrant.
    /// </summary>
    public string HttpEndpoint { get; private set; } = null!;

    /// <summary>
    /// Gets the gRPC endpoint for Qdrant.
    /// </summary>
    public string GrpcEndpoint { get; private set; } = null!;

    /// <summary>
    /// Gets the Qdrant configuration options with container endpoints.
    /// </summary>
    public QdrantOptions Options { get; private set; } = null!;

    /// <summary>
    /// Gets the Qdrant client instance connected to the container.
    /// </summary>
    public QdrantClient? Client { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Qdrant container is running and ready.
    /// </summary>
    public bool IsAvailable => _container != null && !string.IsNullOrEmpty(HttpEndpoint);

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantContainerFixture"/> class.
    /// </summary>
    public QdrantContainerFixture()
    {
        _logger = XUnitLogger.CreateLogger<QdrantContainerFixture>();
    }

    /// <summary>
    /// Asynchronously initializes the Qdrant container.
    /// Starts the container, waits for readiness, and creates the connection endpoints.
    /// Handles Docker unavailability gracefully by setting IsAvailable to false.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous initialization operation</returns>
    public async ValueTask InitializeAsync()
    {
        try
        {
            _logger.LogInformation("🚀 Initializing Qdrant container for SemanticRag system tests...");

            _container = new QdrantBuilder()
                .WithImage("qdrant/qdrant:latest")
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .Build();

            _logger.LogInformation("⏳ Starting Qdrant container...");
            await _container.StartAsync();

            HttpEndpoint = $"http://{Hostname}:{HttpPort}";
            GrpcEndpoint = $"http://{Hostname}:{GrpcPort}";

            Options = new QdrantOptions
            {
                Host = _container.Hostname,
                Port = _container.GetMappedPublicPort(6333),
                CollectionName = "test-collection",
                VectorSize = 384,
                ApiKey = null
            };

            // QdrantClient uses gRPC which requires the gRPC port (6334), not the HTTP port (6333)
            // Use GrpcPort instead of HTTP port to avoid HTTP/2 protocol errors
            Client = new QdrantClient(_container.Hostname, GrpcPort);

            _logger.LogInformation("✅ Qdrant container started - HTTP: {HttpEndpoint}, gRPC: {GrpcEndpoint}",
                HttpEndpoint, GrpcEndpoint);
        }
        
        catch (Exception ex) when (ex.Message.Contains("Docker", StringComparison.OrdinalIgnoreCase) ||
                                  ex.Message.Contains("docker", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(ex, "⚠️ Docker-related error - Qdrant container will not be started");
            DockerSkipConditions.ShouldSkipDockerTests = true;

            HttpEndpoint = "http://localhost:6333";
            GrpcEndpoint = "http://localhost:6334";
            Options = new QdrantOptions
            {
                Host = "localhost",
                Port = 6333,
                CollectionName = "test-collection",
                VectorSize = 384,
                ApiKey = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to initialize Qdrant container");
            throw;
        }
    }

    /// <summary>
    /// Asynchronously disposes of the Qdrant container resources.
    /// Ensures proper cleanup of Docker containers and client connections.
    /// Explicitly stops containers before disposal to prevent resource leaks.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous disposal operation</returns>
    public async ValueTask DisposeAsync()
    {
        try
        {
            _logger.LogInformation("🧹 Cleaning up Qdrant container...");

            // Dispose client first
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
                _logger.LogInformation("✅ Qdrant client disposed");
            }

            // Explicitly stop and dispose container
            if (_container != null)
            {
                try
                {
                    // Stop the container first (ensures proper cleanup)
                    await _container.StopAsync();
                    _logger.LogInformation("✅ Qdrant container stopped");
                }
                catch (Exception stopEx)
                {
                    _logger.LogWarning(stopEx, "⚠️ Error stopping Qdrant container (non-fatal, will attempt disposal)");
                }

                try
                {
                    // Dispose the container (removes it)
                    await _container.DisposeAsync();
                    _logger.LogInformation("✅ Qdrant container disposed");
                }
                catch (Exception disposeEx)
                {
                    _logger.LogWarning(disposeEx, "⚠️ Error disposing Qdrant container (non-fatal)");
                }

                _container = null;
            }

            _logger.LogInformation("✅ Qdrant container cleanup completed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error during Qdrant container cleanup (non-fatal)");
        }
    }
}