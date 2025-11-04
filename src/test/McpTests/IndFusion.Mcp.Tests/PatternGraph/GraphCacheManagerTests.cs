using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndFusion.Mcp.Core.Services;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// TDD implementation tests for GraphCacheManager.
/// These tests call the REAL service class and should FAIL with NotImplementedException initially.
/// </summary>
public class GraphCacheManagerTests
{
    private readonly GraphCacheManager _service;
    private readonly ILogger<GraphCacheManager> _logger;

    /// <summary>
    /// Initializes a new instance of the GraphCacheManagerTests class.
    /// </summary>
    public GraphCacheManagerTests()
    {
        _logger = Substitute.For<ILogger<GraphCacheManager>>();
        _service = new GraphCacheManager(_logger);
    }

    /// <summary>
    /// Verifies that GetAsync with valid hash returns null when not cached.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithValidHash_ShouldReturnNullWhenNotCached()
    {
        // Arrange
        var projectHash = "test-hash-123";
        
        // Act
        var result = await _service.GetAsync(projectHash, CancellationToken.None);
        
        // Assert
        result.IsSuccessValueNull.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    /// <summary>
    /// Verifies that GetAsync with null hash returns null.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithNullHash_ShouldReturnNull()
    {
        // Arrange
        string? projectHash = null;
        
        // Act
        var result = await _service.GetAsync(projectHash!, CancellationToken.None);
        
        // Assert
        result.IsSuccessValueNull.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    /// <summary>
    /// Verifies that GetAsync with empty hash returns null.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithEmptyHash_ShouldReturnNull()
    {
        // Arrange
        var projectHash = string.Empty;
        
        // Act
        var result = await _service.GetAsync(projectHash, CancellationToken.None);
        
        // Assert
        result.IsSuccessValueNull.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    /// <summary>
    /// Verifies that SetAsync with valid data succeeds.
    /// </summary>
    [Fact]
    public async Task SetAsync_WithValidData_ShouldSucceed()
    {
        // Arrange
        var projectHash = "test-hash-456";
        var graph = new SymbolGraph(
            ProjectPath: "/test/project",
            ProjectHash: projectHash,
            Nodes: [],
            Edges: [],
            CreatedAt: DateTime.UtcNow,
            LastUpdated: DateTime.UtcNow,
            Metadata: new Dictionary<string, object>());
        
        // Act
        var result = await _service.SetAsync(projectHash, graph, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that SetAsync with null graph returns failure.
    /// </summary>
    [Fact]
    public async Task SetAsync_WithNullGraph_ShouldReturnFailure()
    {
        // Arrange
        var projectHash = "test-hash-789";
        SymbolGraph? graph = null;
        
        // Act
        var result = await _service.SetAsync(projectHash, graph!, CancellationToken.None);
        
        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Symbol graph cannot be null");
    }

    /// <summary>
    /// Verifies that InvalidateAsync with valid hash succeeds.
    /// </summary>
    [Fact]
    public async Task InvalidateAsync_WithValidHash_ShouldSucceed()
    {
        // Arrange
        var projectHash = "test-hash-invalidate";
        
        // Act
        var result = await _service.InvalidateAsync(projectHash, CancellationToken.None);
        
        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that GetAsync with cancelled token throws OperationCanceledException.
    /// </summary>
    [Fact]
    public async Task GetAsync_WithCancelledToken_ShouldThrowOperationCanceledException()
    {
        // Arrange
        var projectHash = "test-hash-cancelled";
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            () => _service.GetAsync(projectHash, cts.Token));
    }

    /// <summary>
    /// Verifies that SetAsync and GetAsync work together for caching.
    /// </summary>
    [Fact]
    public async Task SetAsync_ThenGetAsync_ShouldReturnCachedGraph()
    {
        // Arrange
        var projectHash = "test-hash-cache";
        var graph = new SymbolGraph(
            ProjectPath: "/test/project",
            ProjectHash: projectHash,
            Nodes: [],
            Edges: [],
            CreatedAt: DateTime.UtcNow,
            LastUpdated: DateTime.UtcNow,
            Metadata: new Dictionary<string, object>());

        // Act
        var setResult = await _service.SetAsync(projectHash, graph, CancellationToken.None);
        var getResult = await _service.GetAsync(projectHash, CancellationToken.None);

        // Assert
        setResult.IsSuccess.ShouldBeTrue();
        getResult.IsSuccess.ShouldBeTrue();
        getResult.Value.ShouldNotBeNull();
        getResult.Value.ProjectHash.ShouldBe(projectHash);
        getResult.Value.ProjectPath.ShouldBe("/test/project");
    }

    /// <summary>
    /// Verifies that InvalidateAsync removes cached graph.
    /// </summary>
    [Fact]
    public async Task SetAsync_ThenInvalidateAsync_ThenGetAsync_ShouldReturnNull()
    {
        // Arrange
        var projectHash = "test-hash-invalidate-cache";
        var graph = new SymbolGraph(
            ProjectPath: "/test/project",
            ProjectHash: projectHash,
            Nodes: [],
            Edges: [],
            CreatedAt: DateTime.UtcNow,
            LastUpdated: DateTime.UtcNow,
            Metadata: new Dictionary<string, object>());

        // Act
        await _service.SetAsync(projectHash, graph, CancellationToken.None);
        await _service.InvalidateAsync(projectHash, CancellationToken.None);
        var getResult = await _service.GetAsync(projectHash, CancellationToken.None);

        // Assert
        getResult.IsSuccessValueNull.ShouldBeTrue();
        getResult.Value.ShouldBeNull();
    }
}
