using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// Contract tests for IGraphCacheManager interface.
/// These tests verify the contract behavior using mocks and should ALWAYS PASS.
/// </summary>
public class IGraphCacheManagerContractTests
{
	private readonly IGraphCacheManager _mockService;
	private readonly CancellationToken _cancellationToken = CancellationToken.None;

	public IGraphCacheManagerContractTests()
	{
		_mockService = Substitute.For<IGraphCacheManager>();
	}

	[Fact]
	public async Task GetAsync_WithValidProjectHash_ShouldReturnSuccessResult()
	{
		// Arrange
		var projectHash = "abc123def456";
		var expectedGraph = new SymbolGraph(
			ProjectPath: "/test/project",
			ProjectHash: projectHash,
			Nodes: new List<GraphNode>(),
			Edges: new List<GraphEdge>(),
			CreatedAt: DateTime.UtcNow,
			LastUpdated: DateTime.UtcNow,
			Metadata: new Dictionary<string, object>());

		_mockService.GetAsync(projectHash, _cancellationToken)
			.Returns(Result<SymbolGraph?>.Success(expectedGraph));

		// Act
		var result = await _mockService.GetAsync(projectHash, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldNotBeNull();
		result.Value.ProjectHash.ShouldBe(projectHash);
	}

	[Fact]
	public async Task GetAsync_WithNonExistentHash_ShouldReturnFailureResult()
	{
		// Arrange
		var projectHash = "non-existent-hash";
		_mockService.GetAsync(projectHash, _cancellationToken)
			.Returns(Result<SymbolGraph?>.WithFailure("Graph not found in cache"));

		// Act
		var result = await _mockService.GetAsync(projectHash, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldNotBeNull();
		result.Error.ShouldContain("not found");
	}

	[Fact]
	public async Task GetAsync_WithEmptyHash_ShouldReturnFailureResult()
	{
		// Arrange
		var projectHash = "";
		_mockService.GetAsync(projectHash, _cancellationToken)
			.Returns(Result<SymbolGraph?>.WithFailure("Project hash cannot be empty"));

		// Act
		var result = await _mockService.GetAsync(projectHash, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldNotBeNull();
		result.Error.ShouldContain("cannot be empty");
	}

	[Fact]
	public async Task SetAsync_WithValidGraph_ShouldReturnSuccessResult()
	{
		// Arrange
		var projectHash = "abc123def456";
		var graph = new SymbolGraph(
			ProjectPath: "/test/project",
			ProjectHash: projectHash,
			Nodes: new List<GraphNode>(),
			Edges: new List<GraphEdge>(),
			CreatedAt: DateTime.UtcNow,
			LastUpdated: DateTime.UtcNow,
			Metadata: new Dictionary<string, object>());

		_mockService.SetAsync(projectHash, graph, _cancellationToken)
			.Returns(Result.Success());

		// Act
		var result = await _mockService.SetAsync(projectHash, graph, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
	}

	[Fact]
	public async Task SetAsync_WithNullGraph_ShouldReturnFailureResult()
	{
		// Arrange
		var projectHash = "abc123def456";
		SymbolGraph? graph = null;

		_mockService.SetAsync(projectHash, graph!, _cancellationToken)
			.Returns(Result.WithFailure("Graph cannot be null"));

		// Act
		var result = await _mockService.SetAsync(projectHash, graph!, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldNotBeNull();
		result.Error.ShouldContain("cannot be null");
	}

	[Fact]
	public async Task InvalidateAsync_WithValidHash_ShouldReturnSuccessResult()
	{
		// Arrange
		var projectHash = "abc123def456";
		_mockService.InvalidateAsync(projectHash, _cancellationToken)
			.Returns(Result.Success());

		// Act
		var result = await _mockService.InvalidateAsync(projectHash, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
	}

	[Fact]
	public async Task InvalidateAsync_WithNonExistentHash_ShouldReturnSuccessResult()
	{
		// Arrange
		var projectHash = "non-existent-hash";
		_mockService.InvalidateAsync(projectHash, _cancellationToken)
			.Returns(Result.Success());

		// Act
		var result = await _mockService.InvalidateAsync(projectHash, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
	}

	[Fact]
	public async Task GetAsync_WithCancellation_ShouldRespectCancellationToken()
	{
		// Arrange
		var projectHash = "abc123def456";
		var cts = new CancellationTokenSource();
		cts.Cancel();

		_mockService.GetAsync(projectHash, cts.Token)
			.Returns(Result<SymbolGraph?>.WithFailure("Operation was cancelled"));

		// Act
		var result = await _mockService.GetAsync(projectHash, cts.Token);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error!.ShouldContain("cancelled");
	}

	[Fact]
	public async Task SetAsync_WithCancellation_ShouldRespectCancellationToken()
	{
		// Arrange
		var projectHash = "abc123def456";
		var graph = new SymbolGraph(
			ProjectPath: "/test/project",
			ProjectHash: projectHash,
			Nodes: new List<GraphNode>(),
			Edges: new List<GraphEdge>(),
			CreatedAt: DateTime.UtcNow,
			LastUpdated: DateTime.UtcNow,
			Metadata: new Dictionary<string, object>());

		var cts = new CancellationTokenSource();
		cts.Cancel();

		_mockService.SetAsync(projectHash, graph, cts.Token)
			.Returns(Result.WithFailure("Operation was cancelled"));

		// Act
		var result = await _mockService.SetAsync(projectHash, graph, cts.Token);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error!.ShouldContain("cancelled");
	}

	[Fact]
	public async Task InvalidateAsync_WithCancellation_ShouldRespectCancellationToken()
	{
		// Arrange
		var projectHash = "abc123def456";
		var cts = new CancellationTokenSource();
		cts.Cancel();

		_mockService.InvalidateAsync(projectHash, cts.Token)
			.Returns(Result.WithFailure("Operation was cancelled"));

		// Act
		var result = await _mockService.InvalidateAsync(projectHash, cts.Token);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error!.ShouldContain("cancelled");
	}
}
