using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndFusion.Mcp.Core.Services;
using IndFusion.Mcp.Tests.Tools;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;
using SourceLocation = IndFusion.Mcp.Core.Models.PatternGraph.SourceLocation;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// TDD implementation tests for PatternGraphService.
/// These tests call the REAL service class and verify actual functionality.
/// </summary>
public class PatternGraphServiceTests
{
    private readonly PatternGraphService _service;
    private readonly ILogger<PatternGraphService> _logger;
    private readonly ISymbolGraphBuilder _symbolGraphBuilder;
    private readonly IGraphCacheManager _cacheManager;

    /// <summary>
    /// Initializes a new instance of the PatternGraphServiceTests class.
    /// </summary>
    public PatternGraphServiceTests()
    {
        _logger = Substitute.For<ILogger<PatternGraphService>>();
        _symbolGraphBuilder = Substitute.For<ISymbolGraphBuilder>();
        _cacheManager = Substitute.For<IGraphCacheManager>();
        _service = new PatternGraphService(_logger, _symbolGraphBuilder, _cacheManager);
    }

    /// <summary>
    /// Verifies that QueryAsync with valid query returns query result.
    /// </summary>
    [Fact]
    public async Task QueryAsync_WithValidQuery_ShouldReturnQueryResult()
    {
        // Arrange
        var query = new PatternGraphQuery(
            ProjectPath: Path.Combine(TestUtilities.GetTestProjectDirectory(), "TestOutput", "TestProject", "TestProject.csproj"),
            NodeTypes: new[] { "Class", "Method" },
            MaxDepth: 5,
            IncludeMetadata: true);
        
        // Setup mocks
        var mockGraph = new SymbolGraph(
            ProjectPath: query.ProjectPath,
            ProjectHash: "test-hash",
            Nodes: new List<GraphNode>(),
            Edges: new List<GraphEdge>(),
            CreatedAt: DateTime.UtcNow,
            LastUpdated: DateTime.UtcNow,
            Metadata: new Dictionary<string, object>());
        
        _cacheManager.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<SymbolGraph?>.Success(null));
        _symbolGraphBuilder.BuildAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<SymbolGraph>.Success(mockGraph));
        _cacheManager.SetAsync(Arg.Any<string>(), Arg.Any<SymbolGraph>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        
        // Act
        var result = await _service.QueryAsync(query, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Nodes.ShouldNotBeNull();
        result.Value.Edges.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that QueryAsync with null query returns failure result.
    /// </summary>
    [Fact]
    public async Task QueryAsync_WithNullQuery_ShouldReturnFailure()
    {
        // Arrange
        PatternGraphQuery? query = null;
        
        // Act
        var result = await _service.QueryAsync(query!, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Pattern graph query cannot be null");
    }

    /// <summary>
    /// Verifies that QueryAsync with cancelled token throws OperationCanceledException.
    /// </summary>
    [Fact]
    public async Task QueryAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var query = new PatternGraphQuery(
            ProjectPath: Path.Combine(TestUtilities.GetTestProjectDirectory(), "TestOutput", "TestProject", "TestProject.csproj"),
            NodeTypes: new[] { "Class" },
            MaxDepth: 3,
            IncludeMetadata: false);
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            () => _service.QueryAsync(query, cts.Token));
    }

    /// <summary>
    /// Verifies that GetNodesAsync with valid path returns nodes.
    /// </summary>
    [Fact]
    public async Task GetNodesAsync_WithValidPath_ShouldReturnNodes()
    {
        // Arrange
        var projectPath = Path.Combine(TestUtilities.GetTestProjectDirectory(), "TestOutput", "TestProject", "TestProject.csproj");
        
        // Setup mocks
        var mockGraph = new SymbolGraph(
            ProjectPath: projectPath,
            ProjectHash: "test-hash",
            Nodes: new List<GraphNode>
            {
                new GraphNode(
                    Id: "node1",
                    Type: "Class",
                    Name: "TestClass",
                    FullName: "TestNamespace.TestClass",
                    Location: new SourceLocation("Test.cs", 1, 10, 1, 20),
                    Metadata: new Dictionary<string, object>())
            },
            Edges: new List<GraphEdge>(),
            CreatedAt: DateTime.UtcNow,
            LastUpdated: DateTime.UtcNow,
            Metadata: new Dictionary<string, object>());
        
        _cacheManager.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<SymbolGraph?>.Success(null));
        _symbolGraphBuilder.BuildAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<SymbolGraph>.Success(mockGraph));
        _cacheManager.SetAsync(Arg.Any<string>(), Arg.Any<SymbolGraph>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        
        // Act
        var result = await _service.GetNodesAsync(projectPath, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
    }

    /// <summary>
    /// Verifies that GetNodesAsync with null path returns failure result.
    /// </summary>
    [Fact]
    public async Task GetNodesAsync_WithNullPath_ShouldReturnFailure()
    {
        // Arrange
        string? projectPath = null;
        
        // Act
        var result = await _service.GetNodesAsync(projectPath!, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Project path cannot be null or empty");
    }

    /// <summary>
    /// Verifies that GetEdgesAsync with valid path returns edges.
    /// </summary>
    [Fact]
    public async Task GetEdgesAsync_WithValidPath_ShouldReturnEdges()
    {
        // Arrange
        var projectPath = Path.Combine(TestUtilities.GetTestProjectDirectory(), "TestOutput", "TestProject", "TestProject.csproj");
        
        // Setup mocks
        var mockGraph = new SymbolGraph(
            ProjectPath: projectPath,
            ProjectHash: "test-hash",
            Nodes: new List<GraphNode>(),
            Edges: new List<GraphEdge>
            {
                new GraphEdge(
                    Id: "edge1",
                    SourceNodeId: "node1",
                    TargetNodeId: "node2",
                    RelationshipType: "Inheritance",
                    Weight: 1.0,
                    Metadata: new Dictionary<string, object>())
            },
            CreatedAt: DateTime.UtcNow,
            LastUpdated: DateTime.UtcNow,
            Metadata: new Dictionary<string, object>());
        
        _cacheManager.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<SymbolGraph?>.Success(null));
        _symbolGraphBuilder.BuildAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Result<SymbolGraph>.Success(mockGraph));
        _cacheManager.SetAsync(Arg.Any<string>(), Arg.Any<SymbolGraph>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        
        // Act
        var result = await _service.GetEdgesAsync(projectPath, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
    }

    /// <summary>
    /// Verifies that GetEdgesAsync with null path returns failure result.
    /// </summary>
    [Fact]
    public async Task GetEdgesAsync_WithNullPath_ShouldReturnFailure()
    {
        // Arrange
        string? projectPath = null;
        
        // Act
        var result = await _service.GetEdgesAsync(projectPath!, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Project path cannot be null or empty");
    }
}
