using IndFusion.SemanticRag.Application;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IndFusion.SemanticRag.Tests.Integration.IntegrationTests;

/// <summary>
/// Integration test fixture for setting up the test environment.
/// </summary>
public class IntegrationTestFixture : IDisposable
{
    /// <summary>
    /// Gets the service provider for the test environment.
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Initializes a new instance of the IntegrationTestFixture class.
    /// </summary>
    public IntegrationTestFixture()
    {
        var services = new ServiceCollection();
        
        // Add logging
        services.AddLogging(builder => builder.AddConsole());
        
        // Add Application and Infrastructure services
        services.AddApplication();
        services.AddInfrastructure();
        
        ServiceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Clears all vectors from the repository.
    /// </summary>
    public async Task ClearRepositoryAsync()
    {
        var vectorSearchPort = ServiceProvider.GetRequiredService<IVectorSearchPort>();
        await vectorSearchPort.ClearAllVectorsAsync();
    }

    /// <summary>
    /// Disposes the test fixture.
    /// </summary>
    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}