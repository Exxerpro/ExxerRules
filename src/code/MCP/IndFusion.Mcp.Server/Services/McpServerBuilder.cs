using IndFusion.Mcp.Server.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModelContextProtocol;
using ModelContextProtocol.AspNetCore;
using ModelContextProtocol.Server;

namespace IndFusion.Mcp.Server.Services;

/// <summary>
/// Fluent builder for configuring and constructing an IndFusion MCP server host.
/// </summary>
public class McpServerBuilder
{
    private readonly IServiceCollection _services;
    private readonly IHostBuilder _hostBuilder;
    private bool _exxerServicesConfigured;
    private IMcpServerBuilder? _mcpBuilder;

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
        var builder = EnsureMcpBuilder();

        builder
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
        EnsureMcpBuilder().WithStdioServerTransport();
        return this;
    }

    /// <summary>
    /// Configures the server to use WebSocket transport via the MCP HTTP transport.
    /// </summary>
    /// <param name="port">Port for the HTTP/WebSocket server. Default is 8080.</param>
    /// <returns>The current builder.</returns>
    public McpServerBuilder WithWebSocketTransport(int port = 8080)
    {
        EnsureMcpBuilder().WithHttpTransport();

        _hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddRouting();
        });

        _hostBuilder.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(port);
            });

            webBuilder.Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapMcp();
                });
            });
        });

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

    private IMcpServerBuilder EnsureMcpBuilder()
    {
        if (_mcpBuilder is not null)
        {
            return _mcpBuilder;
        }

        if (!_exxerServicesConfigured)
        {
            _services.AddExxerFactorMcpServer();
            _exxerServicesConfigured = true;
        }

        _mcpBuilder = _services.AddMcpServer();
        return _mcpBuilder;
    }
}
