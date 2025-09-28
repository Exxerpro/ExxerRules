using Microsoft.Extensions.Hosting;

namespace IndFusion.Mcp.Server.Services;

/// <summary>
/// Extension methods to create <see cref="McpServerBuilder"/> from a host builder.
/// </summary>
public static class McpServerBuilderExtensions
{
    /// <summary>
    /// Creates a new <see cref="McpServerBuilder"/> wrapper over the provided host builder.
    /// </summary>
    /// <param name="hostBuilder">The host builder to extend.</param>
    /// <returns>A new MCP server builder.</returns>
    public static McpServerBuilder CreateMcpServerBuilder(this IHostBuilder hostBuilder)
    {
        return new McpServerBuilder(hostBuilder);
    }
}
