using DotNet.Testcontainers.Builders;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Utilities;
using Qdrant.Client;
using Testcontainers.Qdrant;
using Xunit;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;

/// <summary>
/// xUnit fixture for Qdrant container lifecycle management.
/// Provides Qdrant container, options, and client instance for system tests.
/// Handles Docker unavailability gracefully - when Docker is not available, IsAvailable is set to false
/// and tests should skip using SkipWhen attribute.
/// </summary>
public class QdrantContainerFixture : IAsyncLifetime
{
    private QdrantContainer? _container;

    /// <summary>
    /// Gets a value indicating whether the Qdrant container is available and running.
    /// </summary>
    public bool IsAvailable { get; private set; }

    /// <summary>
    /// Gets the Qdrant configuration options with container endpoints.
    /// </summary>
    public QdrantOptions Options { get; private set; } = null!;

    /// <summary>
    /// Gets the Qdrant client instance connected to the container.
    /// </summary>
    public QdrantClient? Client { get; private set; }

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
        try
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
            IsAvailable = true;
        }
        catch (DockerUnavailableException)
        {
            // Docker is not available - set defaults and mark as unavailable
            IsAvailable = false;
            DockerSkipConditions.ShouldSkipDockerTests = true;
            
            // Set default options to prevent null reference exceptions
            Options = new QdrantOptions
            {
                Host = "localhost",
                Port = 6333,
                CollectionName = "test-collection",
                VectorSize = 384,
                ApiKey = null
            };
            
            // Don't throw - let tests skip via attribute
        }
        catch (Exception ex) when (ex.Message.Contains("Docker") || ex.Message.Contains("docker"))
        {
            // Other Docker-related exceptions
            IsAvailable = false;
            DockerSkipConditions.ShouldSkipDockerTests = true;
            
            Options = new QdrantOptions
            {
                Host = "localhost",
                Port = 6333,
                CollectionName = "test-collection",
                VectorSize = 384,
                ApiKey = null
            };
        }
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