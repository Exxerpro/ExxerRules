using Microsoft.Extensions.DependencyInjection;
using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Services;

namespace IndFusion.Mcp.Core.Extensions;

/// <summary>
/// Dependency injection extensions for registering ExxerFactor MCP core services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers core services required by ExxerFactor MCP.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddExxerFactorMcpCore(this IServiceCollection services)
    {
        // Logging support for services that depend on ILogger<T>
        services.AddLogging();

        // Core services
        services.AddScoped<IExxerFactoringService, ExxerFactoringService>();

        // Sprint 4 Safe Transformation services
        services.AddScoped<IBuildValidationService, BuildValidationService>();
        services.AddScoped<ISafeRegexService, SafeRegexService>();
        services.AddScoped<IFixer001Service, Fixer001Service>();
        services.AddScoped<ICodeTransformationService, CodeTransformationService>();

        // Add other core services here
        services.AddMemoryCache();

        return services;
    }
}
