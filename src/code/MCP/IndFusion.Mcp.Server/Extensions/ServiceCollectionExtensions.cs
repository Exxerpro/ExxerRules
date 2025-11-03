using Microsoft.Extensions.DependencyInjection;
using IndFusion.Mcp.Server.Resources;
using IndFusion.Mcp.Server.Tools;
using IndFusion.Mcp.Core.Extensions;

namespace IndFusion.Mcp.Server.Extensions;

/// <summary>
/// Extension methods for configuring IndFusion MCP server services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers core MCP server services, tools, and resources for ExxerFactor.
    /// </summary>
    /// <param name="services">The dependency injection service collection.</param>
    /// <returns>The same <paramref name="services"/> instance to allow chaining.</returns>
    public static IServiceCollection AddExxerFactorMcpServer(this IServiceCollection services)
    {
        // Add core services
        services.AddExxerFactorMcpCore();

        // Add Mcp tools and resources
        services.AddScoped<ListToolsMcp>();
        services.AddScoped<MetricsResourceMcp>();

        return services;
    }
}
