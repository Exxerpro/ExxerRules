using System;
using System.Linq;
using System.Net.Http;
using IndFusion.Mcp.Server.Services;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Server.Tests.Integration;

/// <summary>
/// Integration tests that exercise the HTTP/WebSocket transport using the MCP client SDK.
/// </summary>
public sealed class McpServerHttpIntegrationTests : IAsyncLifetime
{
    private IHost? _host;
    private Uri? _endpoint;
    private ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    public async ValueTask InitializeAsync()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        _host = hostBuilder
            .CreateMcpServerBuilder()
            .WithExxerFactoringTools()
            .WithWebSocketTransport(port: 0)
            .Build();

        await _host.StartAsync();

        _loggerFactory = _host.Services.GetService<ILoggerFactory>();

        var server = _host.Services.GetRequiredService<IServer>();
        var addressesFeature = server.Features.Get<IServerAddressesFeature>()
            ?? throw new InvalidOperationException("Server did not expose addresses.");

        var address = addressesFeature.Addresses.FirstOrDefault()
            ?? throw new InvalidOperationException("No addresses registered for the MCP server.");

        _endpoint = NormalizeAddress(address);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
    }

    /// <summary>
    /// Verifies that the HTTP transport exposes the ListToolsCommand tool and executes it successfully.
    /// </summary>
    [Fact]
    public async Task ListToolsCommand_ShouldReturnToolInventory()
    {
        // Using StdioClientTransport for a local server
        var transport = new StdioClientTransport(new StdioClientTransportOptions
        {
            Name = "MyLocalServer",
            Command = "dotnet",
            Arguments = ["run", "--project", "path/to/your/server.csproj"]
        });

        var endpoint = _endpoint ?? throw new InvalidOperationException("Endpoint unavailable.");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        var clientOptions = new McpClientOptions()
        {
            //Fill the clients options as documented above
            ProtocolVersion = "1.0.0",

            InitializationTimeout = TimeSpan.FromSeconds(30),
        };

        await using var client = await McpClient.CreateAsync(transport, clientOptions, cancellationToken: cts.Token);

        var tools = await client.ListToolsAsync(cancellationToken: cts.Token);

        tools.Count.ShouldBeGreaterThan(0);
        tools.Any(t => string.Equals(t.Name, "list_tools_command", StringComparison.OrdinalIgnoreCase))
            .ShouldBeTrue();

        var callResult = await client.CallToolAsync("list_tools_command", cancellationToken: cts.Token);
        var textContent = callResult.Content.OfType<TextContentBlock>().FirstOrDefault();

        textContent.ShouldNotBeNull();
        textContent.Text.ShouldContain("extract-method");
    }

    private static Uri NormalizeAddress(string address)
    {
        var normalized = address
            .Replace("0.0.0.0", "localhost", StringComparison.OrdinalIgnoreCase)
            .Replace("[::]", "localhost", StringComparison.OrdinalIgnoreCase);

        if (!normalized.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !normalized.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            normalized = $"http://{normalized}";
        }

        return new Uri(normalized, UriKind.Absolute);
    }
}