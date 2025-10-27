using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace IndFusion.SemanticRag.Infrastructure;

/// <summary>
/// Extension methods for configuring dependency injection in the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Infrastructure services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Register the vector search repository
        services.AddScoped<IVectorSearchPort, VectorSearchRepository>();
        
        return services;
    }
}