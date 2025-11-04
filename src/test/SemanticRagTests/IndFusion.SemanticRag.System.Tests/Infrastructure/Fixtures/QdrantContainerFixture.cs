using IndFusion.SemanticRag.Infrastructure.Configuration;
using Qdrant.Client;
using Testcontainers.Qdrant;
using Xunit;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;

/// <summary>
/// xUnit fixture for Qdrant container lifecycle management.
/// Provides Qdrant container, options, and client instance for system tests.
/// </summary>
public class QdrantContainerFixture : IAsyncLifetime
{
    private QdrantContainer? _container;

    /// <summary>
    /// Gets the Qdrant configuration options with container endpoints.
    /// </summary>
    public QdrantOptions Options { get; private set; } = null!;

    /// <summary>
    /// Gets the Qdrant client instance connected to the container.
    /// </summary>
    public QdrantClient Client { get; private set; } = null!;

    /// <summary>
    /// Gets the container hostname for connection.
    /// </summary>
    public string Hostname => _container?.Hostname ?? "localhost";

    /// <summary>
    /// Gets the mapped HTTP port for Qdrant.
    /// </summary>
    public int HttpPort => _container?.GetMappedPublicPort(6333) ?? 6333;

    /// <summary>
    /// Gets the mapped gRPC port for Qdrant.
    /// </summary>
    public int GrpcPort => _container?.GetMappedPublicPort(6334) ?? 6334;

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        _container = new QdrantBuilder()
            .WithImage("qdrant/qdrant:latest")
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();

        Options = new QdrantOptions
        {
            Host = _container.Hostname,
            Port = _container.GetMappedPublicPort(6333),
            CollectionName = "test-collection",
            VectorSize = 384,
            ApiKey = null
        };

        Client = new QdrantClient(Options.Host, Options.Port);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        Client?.Dispose();

        if (_container != null)
        {
            await _container.DisposeAsync();
        }
    }
}