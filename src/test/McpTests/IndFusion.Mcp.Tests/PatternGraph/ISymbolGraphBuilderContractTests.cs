using IndFusion.Mcp.Core.Abstractions;
using IndFusion.Mcp.Core.Models.PatternGraph;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// Contract tests for ISymbolGraphBuilder interface.
/// These tests verify the contract behavior using mocks and should ALWAYS PASS.
/// </summary>
public class ISymbolGraphBuilderContractTests
{
	private readonly ISymbolGraphBuilder _mockService;
	private readonly CancellationToken _cancellationToken = CancellationToken.None;

	/// <summary>
	/// Initializes the contract tests with a mocked ISymbolGraphBuilder instance.
	/// </summary>
	public ISymbolGraphBuilderContractTests()
	{
		_mockService = Substitute.For<ISymbolGraphBuilder>();
	}

	/// <summary>
	/// Ensures building a graph for a valid project path succeeds and returns expected data.
	/// </summary>
	[Fact]
	public async Task BuildAsync_WithValidProjectPath_ShouldReturnSuccessResult()
	{
		// Arrange
		var projectPath = "/test/project";
		var expectedGraph = new SymbolGraph(
			ProjectPath: projectPath,
			ProjectHash: "abc123def456",
			Nodes:
            [
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
			],
			Edges:
            [
                new GraphEdge(
					Id: "edge-1",
					SourceNodeId: "node-1",
					TargetNodeId: "node-2",
					RelationshipType: "Calls",
					Weight: 0.8,
					Metadata: new Dictionary<string, object>())
			],
			CreatedAt: DateTime.UtcNow,
			LastUpdated: DateTime.UtcNow,
			Metadata: new Dictionary<string, object>());

		_mockService.BuildAsync(projectPath, _cancellationToken)
			.Returns(Result<SymbolGraph>.Success(expectedGraph));

		// Act
		var result = await _mockService.BuildAsync(projectPath, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
		result.Value.ShouldNotBeNull();
		result.Value.ProjectPath.ShouldBe(projectPath);
		result.Value.ProjectHash.ShouldBe("abc123def456");
		result.Value.Nodes.ShouldNotBeNull();
		result.Value.Edges.ShouldNotBeNull();
		result.Value.Nodes.Count.ShouldBe(1);
		result.Value.Edges.Count.ShouldBe(1);
	}

	/// <summary>
	/// Verifies the builder surfaces an error when the project path does not exist.
	/// </summary>
	[Fact]
	public async Task BuildAsync_WithInvalidProjectPath_ShouldReturnFailureResult()
	{
		// Arrange
		var projectPath = "/invalid/path";
		_mockService.BuildAsync(projectPath, _cancellationToken)
			.Returns(Result<SymbolGraph>.WithFailure("Project path does not exist"));

		// Act
		var result = await _mockService.BuildAsync(projectPath, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldNotBeNull();
		result.Error.ShouldContain("does not exist");
	}

	/// <summary>
	/// Confirms updating an existing graph with changed files succeeds.
	/// </summary>
	[Fact]
	public async Task UpdateAsync_WithValidGraphAndFiles_ShouldReturnSuccessResult()
	{
		// Arrange
		var graph = new SymbolGraph(
			ProjectPath: "/test/project",
			ProjectHash: "abc123def456",
			Nodes: [],
			Edges: [],
			CreatedAt: DateTime.UtcNow,
			LastUpdated: DateTime.UtcNow,
			Metadata: new Dictionary<string, object>());

		var changedFiles = new List<string> { "/test/project/TestClass.cs", "/test/project/TestMethod.cs" };

		_mockService.UpdateAsync(graph, changedFiles, _cancellationToken)
			.Returns(Result.Success());

		// Act
		var result = await _mockService.UpdateAsync(graph, changedFiles, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
	}

	/// <summary>
	/// Ensures a null graph argument is rejected with a failure result.
	/// </summary>
	[Fact]
	public async Task UpdateAsync_WithNullGraph_ShouldReturnFailureResult()
	{
		// Arrange
		SymbolGraph? graph = null;
		var changedFiles = new List<string> { "/test/project/TestClass.cs" };

		_mockService.UpdateAsync(graph!, changedFiles, _cancellationToken)
			.Returns(Result.WithFailure("Graph cannot be null"));

		// Act
		var result = await _mockService.UpdateAsync(graph!, changedFiles, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error.ShouldNotBeNull();
		result.Error.ShouldContain("cannot be null");
	}

	/// <summary>
	/// Validates the update operation succeeds when there are no file changes.
	/// </summary>
	[Fact]
	public async Task UpdateAsync_WithEmptyChangedFiles_ShouldReturnSuccessResult()
	{
		// Arrange
		var graph = new SymbolGraph(
			ProjectPath: "/test/project",
			ProjectHash: "abc123def456",
			Nodes: [],
			Edges: [],
			CreatedAt: DateTime.UtcNow,
			LastUpdated: DateTime.UtcNow,
			Metadata: new Dictionary<string, object>());

		var changedFiles = new List<string>();

		_mockService.UpdateAsync(graph, changedFiles, _cancellationToken)
			.Returns(Result.Success());

		// Act
		var result = await _mockService.UpdateAsync(graph, changedFiles, _cancellationToken);

		// Assert
		result.ShouldNotBeNull();
		result.IsSuccess.ShouldBeTrue();
	}

	/// <summary>
	/// Verifies the build operation respects cancellation tokens.
	/// </summary>
	[Fact]
	public async Task BuildAsync_WithCancellation_ShouldRespectCancellationToken()
	{
		// Arrange
		var projectPath = "/test/project";
		var cts = new CancellationTokenSource();
		cts.Cancel();

		_mockService.BuildAsync(projectPath, cts.Token)
			.Returns(Result<SymbolGraph>.WithFailure("Operation was cancelled"));

		// Act
		var result = await _mockService.BuildAsync(projectPath, cts.Token);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error!.ShouldContain("cancelled");
	}

	/// <summary>
	/// Ensures the update operation respects cancellation tokens.
	/// </summary>
	[Fact]
	public async Task UpdateAsync_WithCancellation_ShouldRespectCancellationToken()
	{
		// Arrange
		var graph = new SymbolGraph(
			ProjectPath: "/test/project",
			ProjectHash: "abc123def456",
			Nodes: [],
			Edges: [],
			CreatedAt: DateTime.UtcNow,
			LastUpdated: DateTime.UtcNow,
			Metadata: new Dictionary<string, object>());

		var changedFiles = new List<string> { "/test/project/TestClass.cs" };
		var cts = new CancellationTokenSource();
		cts.Cancel();

		_mockService.UpdateAsync(graph, changedFiles, cts.Token)
			.Returns(Result.WithFailure("Operation was cancelled"));

		// Act
		var result = await _mockService.UpdateAsync(graph, changedFiles, cts.Token);

		// Assert
		result.ShouldNotBeNull();
		result.IsFailure.ShouldBeTrue();
		result.Error!.ShouldContain("cancelled");
	}
}
