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
    /// </remarks>
    public IntegrationTestFixture(IConfiguration configuration)
    {
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