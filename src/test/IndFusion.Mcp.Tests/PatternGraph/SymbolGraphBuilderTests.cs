using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndFusion.Mcp.Core.Services;
using IndFusion.Mcp.Tests.Tools;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// TDD implementation tests for SymbolGraphBuilder.
/// These tests call the REAL service class and should FAIL with NotImplementedException initially.
/// </summary>
public class SymbolGraphBuilderTests
{
    private readonly SymbolGraphBuilder _service;
    private readonly ILogger<SymbolGraphBuilder> _logger;

    /// <summary>
    /// Initializes a new instance of the SymbolGraphBuilderTests class.
    /// </summary>
    public SymbolGraphBuilderTests()
    {
        _logger = Substitute.For<ILogger<SymbolGraphBuilder>>();
        _service = new SymbolGraphBuilder(_logger);
    }

    /// <summary>
    /// Verifies that BuildAsync with valid path returns symbol graph.
    /// </summary>
    [Fact]
    public async Task BuildAsync_WithValidPath_ShouldReturnSymbolGraph()
    {
        // Arrange
        var projectPath = TestUtilities.GetSolutionPath();
        
        // Act
        var result = await _service.BuildAsync(projectPath, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ProjectPath.ShouldBe(projectPath);
        result.Value.Nodes.ShouldNotBeNull();
        result.Value.Edges.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that BuildAsync with null path returns failure.
    /// </summary>
    [Fact]
    public async Task BuildAsync_WithNullPath_ShouldReturnFailure()
    {
        // Arrange
        string? projectPath = null;
        
        // Act
        var result = await _service.BuildAsync(projectPath!, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Project path cannot be null or empty");
    }

    /// <summary>
    /// Verifies that BuildAsync with empty path returns failure.
    /// </summary>
    [Fact]
    public async Task BuildAsync_WithEmptyPath_ShouldReturnFailure()
    {
        // Arrange
        var projectPath = string.Empty;
        
        // Act
        var result = await _service.BuildAsync(projectPath, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Project path cannot be null or empty");
    }

    /// <summary>
    /// Verifies that BuildAsync with cancelled token throws OperationCanceledException.
    /// </summary>
    [Fact]
    public async Task BuildAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var projectPath = TestUtilities.GetSolutionPath();
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            () => _service.BuildAsync(projectPath, cts.Token));
    }

    /// <summary>
    /// Verifies that UpdateAsync with valid data succeeds.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var graph = new SymbolGraph(
            ProjectPath: "/test/project",
            ProjectHash: "test-hash",
            Nodes: new List<GraphNode>(),
            Edges: new List<GraphEdge>(),
            CreatedAt: DateTime.UtcNow,
            LastUpdated: DateTime.UtcNow,
            Metadata: new Dictionary<string, object>());
        var changedFiles = new List<string> { "/test/Test.cs" };
        
        // Act
        var result = await _service.UpdateAsync(graph, changedFiles, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that UpdateAsync with null graph returns failure.
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithNullGraph_ShouldReturnFailure()
    {
        // Arrange
        SymbolGraph? graph = null;
        var changedFiles = new List<string> { "/test/Test.cs" };
        
        // Act
        var result = await _service.UpdateAsync(graph!, changedFiles, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Symbol graph cannot be null");
    }
}
