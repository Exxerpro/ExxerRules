using DotNet.Testcontainers.Builders;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Utilities;
using Neo4j.Driver;
using Testcontainers.Neo4j;
using Xunit;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;

/// <summary>
/// xUnit fixture for Neo4j container lifecycle management.
/// Provides Neo4j container, options, and driver instance for system tests.
/// Handles Docker unavailability gracefully - when Docker is not available, IsAvailable is set to false
/// and tests should skip using SkipWhen attribute.
/// </summary>
public class Neo4jContainerFixture : IAsyncLifetime
{
    private Neo4jContainer? _container;

    /// <summary>
    /// Gets a value indicating whether the Neo4j container is available and running.
    /// </summary>
    public bool IsAvailable { get; private set; }

    /// <summary>
    /// Gets the Neo4j configuration options with container endpoints.
    /// </summary>
    public Neo4jOptions Options { get; private set; } = null!;

    /// <summary>
    /// Gets the Neo4j driver instance connected to the container.
    /// </summary>
    public IDriver? Driver { get; private set; }

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
        try
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
            IsAvailable = true;
        }
        catch (DockerUnavailableException)
        {
            // Docker is not available - set defaults and mark as unavailable
            IsAvailable = false;
            DockerSkipConditions.ShouldSkipDockerTests = true;
            
            // Set default options to prevent null reference exceptions
            Options = new Neo4jOptions
            {
                Uri = "bolt://localhost:7687",
                Username = "neo4j",
                Password = "password",
                Database = "neo4j",
                MaxConnectionPoolSize = 50
            };
            
            // Don't throw - let tests skip via attribute
        }
        catch (Exception ex) when (ex.Message.Contains("Docker") || ex.Message.Contains("docker"))
        {
            // Other Docker-related exceptions
            IsAvailable = false;
            DockerSkipConditions.ShouldSkipDockerTests = true;
            
            Options = new Neo4jOptions
            {
                Uri = "bolt://localhost:7687",
                Username = "neo4j",
                Password = "password",
                Database = "neo4j",
                MaxConnectionPoolSize = 50
            };
        }
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