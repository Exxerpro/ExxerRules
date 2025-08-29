using Microsoft.Extensions.DependencyInjection;
using IndFusion.Mcp.Mcp.Server.Resources;
using IndFusion.Mcp.Mcp.Server.Tools;
using IndFusion.Mcp.Mcp.Core.Extensions;

namespace IndFusion.Mcp.Mcp.Server.Extensions;

public static class ServiceCollectionExtensions
{
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
