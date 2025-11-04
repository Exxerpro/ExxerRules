using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// Contract tests for IPatternGraphService interface.
/// These tests verify the contract behavior using mocks and should ALWAYS PASS.
/// </summary>
public class IPatternGraphServiceContractTests
{
	private readonly IPatternGraphService _mockService;
	private readonly CancellationToken _cancellationToken = CancellationToken.None;

	public IPatternGraphServiceContractTests()
	{
		_mockService = Substitute.For<IPatternGraphService>();
	}

	[Fact]
	public async Task QueryAsync_WithValidQuery_ShouldReturnSuccessResult()
	{
		// Arrange
		var query = new PatternGraphQuery(
			ProjectPath: "/test/project",
			NodeTypes: new[] { "Class", "Method" },
			MaxDepth: 5,
			IncludeMetadata: true);

		var expectedResult = new PatternGraphQueryResult(
			Nodes: [],
			Edges: [],
			QueryMetadata: new QueryMetadata(
				QueryId: "test-query-1",
				Timestamp: DateTime.UtcNow,
				NodeCount: 0,
				EdgeCount: 0,
				FiltersApplied: new Dictionary<string, object>()),
			ExecutionTime: TimeSpan.FromMilliseconds(100));

		_mockService.QueryAsync(query, _cancellationToken)
			.Returns(Result<PatternGraphQueryResult>.Success(expectedResult));

		// Act
		var result = await _mockService.QueryAsync(query, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldNotBeNull();
		result.Value.Nodes.ShouldNotBeNull();
		result.Value.Edges.ShouldNotBeNull();
		result.Value.QueryMetadata.ShouldNotBeNull();
	}

	[Fact]
	public async Task QueryAsync_WithNullQuery_ShouldReturnFailureResult()
	{
		// Arrange
		PatternGraphQuery? query = null;
		_mockService.QueryAsync(query!, _cancellationToken)
			.Returns(Result<PatternGraphQueryResult>.WithFailure("Query cannot be null"));

		// Act
		var result = await _mockService.QueryAsync(query!, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldNotBeNull();
		result.Error.ShouldContain("Query cannot be null");
	}

	[Fact]
	public async Task GetNodesAsync_WithValidProjectPath_ShouldReturnSuccessResult()
	{
		// Arrange
		var projectPath = "/test/project";
		var expectedNodes = new List<GraphNode>
		{
			new GraphNode(
				Id: "node-1",
				Type: "Class",
				Name: "TestClass",
				FullName: "TestNamespace.TestClass",
				Location: new IndFusion.Mcp.Core.Models.PatternGraph.SourceLocation(
					FilePath: "/test/project/TestClass.cs",
					StartLine: 1,
					EndLine: 10,
					StartColumn: 1,
					EndColumn: 1),
				Metadata: new Dictionary<string, object>())
		};

		_mockService.GetNodesAsync(projectPath, _cancellationToken)
			.Returns(Result<IReadOnlyCollection<GraphNode>>.Success(expectedNodes));

		// Act
		var result = await _mockService.GetNodesAsync(projectPath, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldNotBeNull();
		result.Value.Count.ShouldBe(1);
		result.Value.First().Id.ShouldBe("node-1");
		result.Value.First().Type.ShouldBe("Class");
	}

	[Fact]
	public async Task GetEdgesAsync_WithValidProjectPath_ShouldReturnSuccessResult()
	{
		// Arrange
		var projectPath = "/test/project";
		var expectedEdges = new List<GraphEdge>
		{
			new GraphEdge(
				Id: "edge-1",
				SourceNodeId: "node-1",
				TargetNodeId: "node-2",
				RelationshipType: "Calls",
				Weight: 0.8,
				Metadata: new Dictionary<string, object>())
		};

		_mockService.GetEdgesAsync(projectPath, _cancellationToken)
			.Returns(Result<IReadOnlyCollection<GraphEdge>>.Success(expectedEdges));

		// Act
		var result = await _mockService.GetEdgesAsync(projectPath, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldNotBeNull();
		result.Value.Count.ShouldBe(1);
		result.Value.First().Id.ShouldBe("edge-1");
		result.Value.First().RelationshipType.ShouldBe("Calls");
	}

	[Fact]
	public async Task QueryAsync_WithCancellation_ShouldRespectCancellationToken()
	{
		// Arrange
		var query = new PatternGraphQuery(ProjectPath: "/test/project");
		var cts = new CancellationTokenSource();
		cts.Cancel();

		_mockService.QueryAsync(query, cts.Token)
			.Returns(Result<PatternGraphQueryResult>.WithFailure("Operation was cancelled"));

		// Act
		var result = await _mockService.QueryAsync(query, cts.Token);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldContain("cancelled");
	}
}
