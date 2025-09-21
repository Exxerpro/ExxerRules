using Microsoft.Extensions.DependencyInjection;
using IndFusion.Mcp.Server.Resources;
using IndFusion.Mcp.Server.Tools;
using IndFusion.Mcp.Core.Extensions;

namespace IndFusion.Mcp.Server.Extensions;

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
