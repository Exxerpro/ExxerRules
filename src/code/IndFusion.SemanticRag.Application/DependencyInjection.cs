using IndFusion.SemanticRag.Application.Commands;
using IndFusion.SemanticRag.Application.Commands.VectorSearch;
using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IndFusion.SemanticRag.Application;

/// <summary>
/// Extension methods for configuring dependency injection in the Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Application layer services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register the SimpleMediator as the IMediator implementation
        services.AddScoped<IMediator, SimpleMediator>();

        // Register command handlers
        services.AddScoped<ICommandHandler<ProcessDocumentCommand>, ProcessDocumentCommandHandler>();
        services.AddScoped<ICommandHandler<StoreVectorCommand>, StoreVectorCommandHandler>();

        // Register application services
        services.AddScoped<SemanticRagOrchestrationService>();

        return services;
    }
}
