using IndFusion.SemanticRag.Application;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Integration.Tests.IntegrationTests;

/// <summary>
/// Provides a reusable integration-test service container composed from application and infrastructure registrations.
/// </summary>
public class IntegrationTestFixture : IDisposable
{
    /// <summary>
    /// Gets the fully initialized service provider that integration tests use to resolve dependencies.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestFixture"/> class and composes the application service graph.
    /// </summary>
    /// <remarks>
    /// The underlying <see cref="ServiceCollection"/> registers logging along with the application and infrastructure layers,
    /// mirroring real production wiring so that integration tests exercise concrete implementations.
    /// xUnit v3 doesn't auto-inject IConfiguration, so we build it internally with test configuration.
    /// </remarks>
    public IntegrationTestFixture()
    {
        // Build test configuration with required sections
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // Qdrant configuration
                ["Qdrant:Host"] = "localhost",
                ["Qdrant:Port"] = "6333",
                ["Qdrant:CollectionName"] = "test-collection",
                ["Qdrant:VectorSize"] = "384",
                
                // Ollama configuration
                ["Ollama:BaseUrl"] = "http://localhost:11434",
                ["Ollama:Model"] = "llama2",
                ["Ollama:TimeoutSeconds"] = "30",
                
                // Neo4j configuration
                ["Neo4j:Uri"] = "bolt://localhost:7687",
                ["Neo4j:Username"] = "neo4j",
                ["Neo4j:Password"] = "test-password",
                ["Neo4j:Database"] = "neo4j",
                
                // Redis configuration
                ["Redis:ConnectionString"] = "localhost:6379",
                ["Redis:Database"] = "0"
            })
            .Build();

        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder => builder.AddConsole());

        // Add Application and Infrastructure services
        services.AddApplication();
        services.AddSemanticRagServices(configuration);

        ServiceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Removes all vector entries from the underlying <see cref="IVectorSearchPort"/> storage to ensure isolated test runs.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the repository purge finishes.</returns>
    public async Task ClearRepositoryAsync()
    {
        var vectorSearchPort = ServiceProvider.GetRequiredService<IVectorSearchPort>();
        await vectorSearchPort.ClearAsync();
    }

    /// <summary>
    /// Disposes the service provider and any scoped resources created during integration testing.
    /// </summary>
    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}