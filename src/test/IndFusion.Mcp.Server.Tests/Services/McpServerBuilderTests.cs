using IndFusion.Mcp.Server.Services;
using Microsoft.Extensions.Hosting;

namespace IndFusion.Mcp.Server.Tests.Services;

/// <summary>
/// Tests for McpServerBuilder fluent configuration and host construction.
/// </summary>
public class McpServerBuilderTests
{
    /// <summary>
    /// Ensures the builder can be constructed with a HostBuilder.
    /// </summary>
    [Fact]
    public void Constructor_ShouldReturnNonNull_McpServerBuilder()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();

        // Act
        var builder = new McpServerBuilder(hostBuilder);

        // Assert
        builder.ShouldNotBeNull();
        builder.ShouldBeOfType<McpServerBuilder>();
    }

    /// <summary>
    /// Verifies the extension method returns a builder instance.
    /// </summary>
    [Fact]
    public void CreateMcpServerBuilder_Extension_ShouldReturnBuilder()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();

        // Act
        var builder = hostBuilder.CreateMcpServerBuilder();

        // Assert
        builder.ShouldNotBeNull();
        builder.ShouldBeOfType<McpServerBuilder>();
    }

    /// <summary>
    /// Ensures WithStdioTransport can be invoked without errors.
    /// </summary>
    [Fact]
    public void WithStdioTransport_ShouldConfigureStdioTransport()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();
        var builder = new McpServerBuilder(hostBuilder);

        // Act
        var result = builder.WithStdioTransport();

        // Assert
        result.ShouldBeSameAs(builder); // Should return same instance for fluent interface
    }

    /// <summary>
    /// Ensures WithWebSocketTransport can be invoked without errors.
    /// </summary>
    [Fact]
    public void WithWebSocketTransport_ShouldConfigureWebSocketTransport()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();
        var builder = new McpServerBuilder(hostBuilder);

        // Act
        var result = builder.WithWebSocketTransport(7042);

        // Assert
        result.ShouldBeSameAs(builder); // Should return same instance for fluent interface
    }

    /// <summary>
    /// Confirms fluent chaining returns the same builder instance.
    /// </summary>
    [Fact]
    public void FluentInterface_ShouldAllowChaining()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();

        // Act & Assert - Should not throw
        var action = () => hostBuilder.CreateMcpServerBuilder()
            .WithExxerFactoringTools()
            .WithStdioTransport()
            .WithWebSocketTransport(7042);

        Should.NotThrow(action);
    }

    /// <summary>
    /// Ensures Build returns an IHost instance.
    /// </summary>
    [Fact]
    public void Build_ShouldReturnIHost()
    {
        // Arrange
        var hostBuilder = Host.CreateDefaultBuilder();
        var builder = new McpServerBuilder(hostBuilder);

        // Act
        var host = builder.Build();

        // Assert
        host.ShouldNotBeNull();
        host.ShouldBeAssignableTo<IHost>();
    }

    /// <summary>
    /// Verifies the constructor throws when given a null host builder.
    /// </summary>
    [Fact]
    public void Constructor_WithNullHostBuilder_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        var action = () => new McpServerBuilder(null!);
        Should.Throw<ArgumentNullException>(action);
    }
}

