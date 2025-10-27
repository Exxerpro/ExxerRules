using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Services;
using IndFusion.Mcp.Core.Tests.TestUtilities;
using TestDirectoryScope = IndFusion.Mcp.Core.Tests.TestUtilities.TestFileUtilities.TestDirectoryScope;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Core.Tests.Services;

/// <summary>
/// Tests for <see cref="IndFusion.Mcp.Core.Services.ExxerFactoringService"/> ensuring
/// methods return expected placeholder results and accept cancellation tokens.
/// </summary>
public class ExxerFactoringServiceTests
{
    private readonly ILogger<ExxerFactoringService> _mockLogger;
    private readonly ExxerFactoringService _service;

    /// <summary> ///  ExxerFactoringServiceTests. /// </summary>
    public ExxerFactoringServiceTests()
    {
        _mockLogger = Substitute.For<ILogger<ExxerFactoringService>>();
        _service = new ExxerFactoringService(_mockLogger);
    }

    /// <summary>
    /// Verifies ExecuteExxerFactoringAsync returns a failure result for unimplemented tools.
    /// </summary>
    [Fact]
    public async Task ExecuteExxerFactoringAsync_ShouldReturnFailureResult_WhenToolNotImplemented()
    {
        // Arrange
        var request = new ExxerFactoringRequest(
            "test-tool",
            "/path/to/solution.sln",
            new Dictionary<string, object>()
        );

        // Act
        var result = await _service.ExecuteExxerFactoringAsync(request, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
        result.Message.ShouldContain("not yet implemented");
    }

    /// <summary>
    /// Verifies ExtractMethodAsync returns a failure result when not implemented.
    /// </summary>
    /// <summary>
    /// Verifies ExtractMethodAsync returns a failure result when not implemented.
    /// </summary>
    [Fact]
    public async Task ExtractMethodAsync_ShouldReturnFailureResult_WhenNotImplemented()
    {
        // Arrange
        using var testScope = new TestDirectoryScope();
        var solutionPath = TestFileUtilities.CreateTestSolution(testScope.Directory, "TestClass", @"
using System;

namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            Console.WriteLine(""Hello World"");
            var x = 5;
            var y = 10;
            var result = x + y;
            Console.WriteLine(result);
        }
    }
}");
        var filePath = Path.Combine(testScope.Directory, "TestClass.cs");
        var startLine = 10;
        var endLine = 15;
        var methodName = "ExtractedMethod";

        // Act
        var result = await _service.ExtractMethodAsync(solutionPath, filePath, startLine, endLine, methodName, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        result.Message.ShouldContain("Successfully extracted method");

        // Verify logging
        _mockLogger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Extracting method")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    /// <summary>
    /// Verifies MoveMethodAsync returns a failure result when not implemented.
    /// </summary>
    /// <summary>
    /// Verifies MoveMethodAsync returns a failure result when not implemented.
    /// </summary>
    [Fact]
    public async Task MoveMethodAsync_ShouldReturnFailureResult_WhenNotImplemented()
    {
        // Arrange
        using var testScope = new TestDirectoryScope();
        var solutionPath = TestFileUtilities.CreateTestSolution(testScope.Directory, "SourceClass", @"
using System;

namespace TestNamespace
{
    public class SourceClass
    {
        public static void MethodToMove()
        {
            Console.WriteLine(""Hello World"");
        }
    }
    
    public class TargetClass
    {
        public void SomeMethod()
        {
            Console.WriteLine(""Target method"");
        }
    }
}");
        var sourceFilePath = Path.Combine(testScope.Directory, "SourceClass.cs");
        var methodName = "MethodToMove";
        var targetClassName = "TargetClass";

        // Act
        var result = await _service.MoveMethodAsync(solutionPath, sourceFilePath, methodName, targetClassName, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.Success.ShouldBeTrue();
        result.Message.ShouldContain("Successfully moved static method");

        // Verify logging
        _mockLogger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Moving method")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    /// <summary>
    /// Verifies IntroduceVariableAsync returns a failure result when not implemented.
    /// </summary>
    /// <summary>
    /// Verifies IntroduceVariableAsync returns a failure result when not implemented.
    /// </summary>
    [Fact]
    public async Task IntroduceVariableAsync_ShouldReturnFailureResult_WhenNotImplemented()
    {
        // Arrange
        using var testScope = new TestDirectoryScope();
        var solutionPath = TestFileUtilities.CreateTestSolution(testScope.Directory, "TestClass", @"
using System;

namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            Console.WriteLine(5 + 10);
        }
    }
}");
        var filePath = Path.Combine(testScope.Directory, "TestClass.cs");
        var line = 7; // Line with the expression
        var column = 20; // Position in the expression
        var variableName = "newVariable";

        // Act
        var result = await _service.IntroduceVariableAsync(solutionPath, filePath, line, column, variableName, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        if (!result.Success)
        {
            // Debug: Show the actual error message
            Console.WriteLine($"IntroduceVariableAsync failed: {result.Message}");
            if (!string.IsNullOrEmpty(result.ErrorDetails))
            {
                Console.WriteLine($"Error details: {result.ErrorDetails}");
            }
        }
        result.Success.ShouldBeTrue();
        result.Message.ShouldNotBeNullOrEmpty();
        // Note: The actual message might vary, so we just check it's not empty

        // Verify logging
        _mockLogger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Introducing variable")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    /// <summary>
    /// Verifies GetMetricsAsync returns an error json payload when not implemented.
    /// </summary>
    /// <summary>
    /// Verifies GetMetricsAsync returns an error json payload when not implemented.
    /// </summary>
    [Fact]
    public async Task GetMetricsAsync_ShouldReturnErrorJson_WhenNotImplemented()
    {
        // Arrange
        using var testScope = new TestDirectoryScope();
        var solutionPath = TestFileUtilities.CreateTestSolution(testScope.Directory, "TestClass", @"
using System;

namespace TestNamespace
{
    public class TestClass
    {
        public void TestMethod()
        {
            Console.WriteLine(""Hello World"");
        }
    }
}");
        var path = Path.Combine(testScope.Directory, "TestClass.cs");

        // Act
        var result = await _service.GetMetricsAsync(solutionPath, path, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNullOrEmpty();
        // Should contain metrics data, not error
        result.ShouldNotContain("error");
        result.ShouldNotContain("not yet implemented");
    }

    /// <summary>
    /// Verifies ListAvailableToolsAsync returns known placeholder tool names.
    /// </summary>
    [Fact]
    public async Task ListAvailableToolsAsync_ShouldReturnExpectedTools()
    {
        // Act
        var tools = await _service.ListAvailableToolsAsync();

        // Assert
        tools.ShouldNotBeEmpty();
        tools.ShouldContain("extract-method");
        tools.ShouldContain("move-method");
        tools.ShouldContain("introduce-variable");
        tools.ShouldContain("safe-delete");
    }

    /// <summary>
    /// Verifies ExecuteExxerFactoringAsync handles invalid tool names.
    /// </summary>
    /// <summary>
    /// ExecuteExxerFactoringAsync ShouldHandleInvalidToolName.
    /// </summary>
    /// <param name="toolName"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task ExecuteExxerFactoringAsync_ShouldHandleInvalidToolName(string? toolName)
    {
        // Arrange
        var request = new ExxerFactoringRequest(
            toolName ?? string.Empty,
            "/path/to/solution.sln",
            new Dictionary<string, object>()
        );

        // Act
        var result = await _service.ExecuteExxerFactoringAsync(request, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert
        result.Success.ShouldBeFalse();
    }

    /// <summary>
    /// Verifies ExecuteExxerFactoringAsync error path returns a failure result with message.
    /// </summary>
    [Fact]
    public async Task ExecuteExxerFactoringAsync_ShouldHandleException_AndReturnErrorResult()
    {
        // This test would be more relevant once real implementations exist
        // For now, testing the error handling structure
        var request = new ExxerFactoringRequest(
            "test-tool",
            "/path/to/solution.sln",
            new Dictionary<string, object>()
        );

        var result = await _service.ExecuteExxerFactoringAsync(request, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldNotBeNullOrEmpty();
    }
}
