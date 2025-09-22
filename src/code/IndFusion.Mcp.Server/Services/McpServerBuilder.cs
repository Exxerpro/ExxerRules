using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IndFusion.Mcp.Server.Services;

/// <summary>
/// Fluent builder for configuring and constructing an IndFusion MCP server host.
/// </summary>
public class McpServerBuilder
{
    private readonly IServiceCollection _services;
    private readonly IHostBuilder _hostBuilder;

    /// <summary>
    /// Initializes a new builder using the supplied <see cref="IHostBuilder"/>.
    /// </summary>
    /// <param name="hostBuilder">Host builder to configure and build.</param>
    public McpServerBuilder(IHostBuilder hostBuilder)
    {
        _hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
        _services = new ServiceCollection();
    }

    /// <summary>
    /// Adds ExxerFactor tools, resources, and prompts discovered in the current assembly.
    /// </summary>
    /// <returns>The current builder.</returns>
    public McpServerBuilder WithExxerFactoringTools()
    {
        _services
            .AddMcpServer()
            .WithToolsFromAssembly()
            .WithResourcesFromAssembly()
            .WithPromptsFromAssembly();

        return this;
    }

    /// <summary>
    /// Configures the server to use stdio transport.
    /// </summary>
    /// <returns>The current builder.</returns>
    public McpServerBuilder WithStdioTransport()
    {
        _services.AddMcpServer().WithStdioServerTransport();
        return this;
    }

    /// <summary>
    /// Configures the server to use WebSocket transport.
    /// </summary>
    /// <param name="port">Port for the WebSocket server. Default is 8080.</param>
    /// <returns>The current builder.</returns>
    public McpServerBuilder WithWebSocketTransport(int port = 8080)
    {
        // TODO: Implement WebSocket transport for web integration
        return this;
    }

    /// <summary>
    /// Adds logging configuration to the underlying host builder.
    /// </summary>
    /// <param name="configureLogging">Delegate to configure logging providers and levels.</param>
    /// <returns>The current builder.</returns>
    public McpServerBuilder WithLogging(Action<ILoggingBuilder> configureLogging)
    {
        _hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddLogging(configureLogging);
        });
        return this;
    }

    /// <summary>
    /// Builds the host by applying accumulated service registrations.
    /// </summary>
    /// <returns>A configured <see cref="IHost"/> instance.</returns>
    public IHost Build()
    {
        _hostBuilder.ConfigureServices((context, services) =>
        {
            foreach (var service in _services)
            {
                services.Add(service);
            }
        });

        return _hostBuilder.Build();
    }
}
