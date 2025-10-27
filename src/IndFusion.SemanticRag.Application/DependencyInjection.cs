using Microsoft.Extensions.DependencyInjection;

namespace IndFusion.SemanticRag.Application;

/// <summary>
/// Extension methods for configuring dependency injection in the Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Application services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Note: This is the old Application project - the new one is in code/IndFusion.SemanticRag.Application
        // Custom Mediator implementation is in the new Application project
        
        return services;
    }
}