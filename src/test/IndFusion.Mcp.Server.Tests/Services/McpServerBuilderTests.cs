using IndFusion.Mcp.Server.Services;
using IndFusion.Mcp.Server.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IndFusion.Mcp.Server.Tests.Services;

/// <summary>
/// Tests for McpServerBuilder fluent configuration and host construction.
/// </summary>
public class McpServerBuilderTests
{
    /// <summary>
    /// Ensures the constructor returns a builder instance.
    /// </summary>
    [Fact]
    public void Constructor_ShouldReturnNonNull_McpServerBuilder()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var builder = new McpServerBuilder(hostBuilder);

        builder.ShouldNotBeNull();
        builder.ShouldBeOfType<McpServerBuilder>();
    }

    /// <summary>
    /// Verifies the extension helper returns a builder.
    /// </summary>
    [Fact]
    public void CreateMcpServerBuilder_Extension_ShouldReturnBuilder()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var builder = hostBuilder.CreateMcpServerBuilder();

        builder.ShouldNotBeNull();
        builder.ShouldBeOfType<McpServerBuilder>();
    }

    /// <summary>
    /// Ensures the stdio transport path remains fluent.
    /// </summary>
    [Fact]
    public void WithStdioTransport_ShouldConfigureStdioTransport()
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        var builder = new McpServerBuilder(hostBuilder);

        var result = builder.WithStdioTransport();

        result.ShouldBeSameAs(builder);
    }

    /// <summary>
    /// Ensures the HTTP/WebSocket transport path remains fluent and buildable.
    /// </summary>
    [Fact]
    public void WithWebSocketTransport_ShouldConfigureHttpTransport()
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        var builder = new McpServerBuilder(hostBuilder);

        var result = builder.WithWebSocketTransport(0);

        result.ShouldBeSameAs(builder);

        using var host = builder.Build();
        host.ShouldNotBeNull();
    }

    /// <summary>
    /// Confirms the fluent interface allows chaining all configuration helpers.
    /// </summary>
    [Fact]
    public void FluentInterface_ShouldAllowChaining()
    {
        var hostBuilder = Host.CreateDefaultBuilder();

        var action = () => hostBuilder.CreateMcpServerBuilder()
            .WithExxerFactoringTools()
            .WithStdioTransport()
            .WithWebSocketTransport(0);

        Should.NotThrow(action);
    }

    /// <summary>
    /// Verifies Build returns an IHost implementation.
    /// </summary>
    [Fact]
    public void Build_ShouldReturnIHost()
    {
        var hostBuilder = Host.CreateDefaultBuilder();
        var builder = new McpServerBuilder(hostBuilder);

        var host = builder.Build();

        host.ShouldNotBeNull();
        host.ShouldBeAssignableTo<IHost>();
    }

    /// <summary>
    /// Ensures the constructor validates null host builders.
    /// </summary>
    [Fact]
    public void Constructor_WithNullHostBuilder_ShouldThrowArgumentNullException()
    {
        var action = () => new McpServerBuilder(null!);

        Should.Throw<ArgumentNullException>(action);
    }

    /// <summary>
    /// Ensures stdio transport wiring can resolve tool dependencies.
    /// </summary>
    [Fact]
    public async Task Build_WithStdioTransport_ShouldResolve_ListToolsMcp()
    {
        using var host = Host.CreateDefaultBuilder()
            .CreateMcpServerBuilder()
            .WithExxerFactoringTools()
            .WithStdioTransport()
            .Build();

        using var scope = host.Services.CreateScope();
        var tool = scope.ServiceProvider.GetRequiredService<ListToolsMcp>();

        var result = await tool.ListToolsCommand();

        result.ShouldNotBeNull();
        result.ShouldContain("extract-method");
    }

    /// <summary>
    /// Ensures HTTP/WebSocket transport wiring can resolve tool dependencies.
    /// </summary>
    [Fact]
    public async Task Build_WithWebSocketTransport_ShouldResolve_ListToolsMcp()
    {
        using var host = Host.CreateDefaultBuilder()
            .CreateMcpServerBuilder()
            .WithWebSocketTransport(0)
            .WithExxerFactoringTools()
            .Build();

        using var scope = host.Services.CreateScope();
        var tool = scope.ServiceProvider.GetRequiredService<ListToolsMcp>();

        var result = await tool.ListToolsCommand();

        result.ShouldNotBeNull();
        result.ShouldContain("extract-method");
    }
}
