using Microsoft.AspNetCore.Mvc;
using IndFusion.Mcp.Web.Controllers;

namespace IndFusion.Mcp.Web.Tests.Controllers;

/// <summary>
/// Unit tests for McpController actions and helper behavior.
/// </summary>
public class McpControllerTests
{
    private readonly IServiceProvider _mockServiceProvider;
    private readonly ILogger<McpController> _mockLogger;
    private readonly McpController _controller;

    /// <summary>
    /// Initializes the controller under test with mocked dependencies.
    /// </summary>
    public McpControllerTests()
    {
        _mockServiceProvider = Substitute.For<IServiceProvider>();
        _mockLogger = Substitute.For<ILogger<McpController>>();
        _controller = new McpController(_mockServiceProvider, _mockLogger);
    }

    /// <summary>
    /// Returns BadRequest when the requested tool is not found.
    /// </summary>
    [Fact]
    public async Task HandleToolCall_ShouldReturnBadRequest_WhenToolNotFound()
    {
        // Arrange
        var request = new McpToolCallRequest
        {
            ToolName = "nonexistent-tool",
            Parameters = new Dictionary<string, JsonElement>()
        };

        // Act
        var result = await _controller.HandleToolCall(request);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.ShouldBeOfType<McpErrorResponse>();

        var errorResponse = badRequestResult.Value as McpErrorResponse;
        errorResponse!.Error.ShouldContain("Tool not found");
        errorResponse.Code.ShouldBe(-32601);
    }

    /// <summary>
    /// Logs the tool name when HandleToolCall is invoked.
    /// </summary>
    [Fact]
    public async Task HandleToolCall_ShouldLogToolCall()
    {
        // Arrange
        var request = new McpToolCallRequest
        {
            ToolName = "test-tool",
            Parameters = new Dictionary<string, JsonElement>()
        };

        // Act
        await _controller.HandleToolCall(request);

        // Assert
        _mockLogger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Mcp tool call: test-tool")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    /// <summary>
    /// Returns Ok with a McpListToolsResponse when listing tools.
    /// </summary>
    [Fact]
    public void ListTools_ShouldReturnOkResult_WithToolsList()
    {
        // Act
        var result = _controller.ListTools();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<McpListToolsResponse>();

        var response = okResult.Value as McpListToolsResponse;
        response!.Tools.ShouldNotBeNull();
        response!.Tools.ShouldBeOfType<List<McpTool>>();
    }

    /// <summary>
    /// Returns Ok with expected server information.
    /// </summary>
    [Fact]
    public void GetServerInfo_ShouldReturnCorrectServerInfo()
    {
        // Act
        var result = _controller.GetServerInfo();

        // Assert
        result.ShouldBeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        okResult!.Value.ShouldBeOfType<McpServerInfo>();

        var serverInfo = okResult.Value as McpServerInfo;
        serverInfo!.Name.ShouldBe("ExxerFactor.Mcp");
        serverInfo.Version.ShouldBe("1.0.0");
        serverInfo.ProtocolVersion.ShouldBe("2024-11-05");
        serverInfo.Capabilities.ShouldNotBeNull();
        serverInfo.Capabilities!.Tools.ShouldNotBeNull();
        serverInfo.Capabilities.Resources.ShouldNotBeNull();
    }

    /// <summary>
    /// Ensures tool names are reported in kebab-case.
    /// </summary>
    /// <summary>
    /// ToKebabCase ShouldConvertCamelCaseCorrectly.
    /// </summary>
    /// <param name="expected"></param>
    [Theory]
    [InlineData("extract-method")]
    [InlineData("move-method")]
    [InlineData("introduce-variable")]
    public void ToKebabCase_ShouldConvertCamelCaseCorrectly(string expected)
    {
        // This tests the private method indirectly through ListTools
        var result = _controller.ListTools();
        var okResult = result as OkObjectResult;
        var response = okResult!.Value as McpListToolsResponse;

        // The tools list should contain kebab-case names
        var names = response!.Tools.Select(t => t.Name).ToList();
        names.ShouldContain(name => name.Contains("-"));
        names.ShouldContain(expected);
    }

    /// <summary>
    /// Returns BadRequest and logs an error when exceptions occur.
    /// </summary>
    [Fact]
    public async Task HandleToolCall_ShouldReturnBadRequest_WhenExceptionOccurs()
    {
        // Arrange - force an exception by passing invalid parameters
        var request = new McpToolCallRequest
        {
            ToolName = "", // Empty tool name should cause issues
            Parameters = null
        };

        // Act
        var result = await _controller.HandleToolCall(request);

        // Assert
        result.ShouldBeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.ShouldBeOfType<McpErrorResponse>();

        // Verify error was logged
        _mockLogger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Error executing Mcp tool")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}

// Test data builders for better test organization

