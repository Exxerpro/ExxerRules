using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Interfaces;

/// <summary>
/// ITDD tests for IGraphQueryService interface contract validation.
/// </summary>
public class IGraphQueryServiceTests
{
    private readonly IGraphQueryService _mockGraphQueryService;

    public IGraphQueryServiceTests()
    {
        _mockGraphQueryService = Substitute.For<IGraphQueryService>();
    }

    [Fact]
    public async Task ExecuteQueryAsync_Should_Return_Success_For_Valid_Query()
    {
        // Arrange
        var query = "MATCH (n) RETURN n";
        var parameters = new Dictionary<string, object> { ["limit"] = 10 };
        var expectedResult = new GraphQueryResult(
            new List<GraphRecord>
            {
                new(new List<object> { "test" }, new List<string> { "n" })
            },
            100,
            1,
            true);

        _mockGraphQueryService.ExecuteQueryAsync(query, parameters, Arg.Any<CancellationToken>())
            .Returns(Result<GraphQueryResult>.Success(expectedResult));

        // Act
        var result = await _mockGraphQueryService.ExecuteQueryAsync(query, parameters, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expectedResult);
        result.Value.IsSuccess.ShouldBeTrue();
        result.Value.RecordCount.ShouldBe(1);
    }

    [Fact]
    public async Task ExecuteQueryAsync_Should_Return_Failure_For_Invalid_Query()
    {
        // Arrange
        var query = "INVALID QUERY";
        var expectedError = "Invalid query syntax";

        _mockGraphQueryService.ExecuteQueryAsync(query, null, Arg.Any<CancellationToken>())
            .Returns(Result<GraphQueryResult>.WithFailure(expectedError));

        // Act
        var result = await _mockGraphQueryService.ExecuteQueryAsync(query, null, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    [Fact]
    public async Task ExecuteQueryAsync_Should_Handle_Cancellation_Token()
    {
        // Arrange
        var query = "MATCH (n) RETURN n";
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockGraphQueryService.ExecuteQueryAsync(query, null, cts.Token)
            .Returns(Result<GraphQueryResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockGraphQueryService.ExecuteQueryAsync(query, null, cts.Token);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("cancelled");
    }

    [Fact]
    public async Task GetNodesAsync_Should_Return_Success_For_Valid_NodeType()
    {
        // Arrange
        var nodeType = "CodeNode";
        var filters = new Dictionary<string, object> { ["language"] = "C#" };
        var expectedNodes = new List<GraphNode>
        {
            new("node1", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" })
        };

        _mockGraphQueryService.GetNodesAsync(nodeType, filters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<GraphNode>>.Success(expectedNodes));

        // Act
        var result = await _mockGraphQueryService.GetNodesAsync(nodeType, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Id.ShouldBe("node1");
        result.Value[0].Type.ShouldBe("CodeNode");
    }

    [Fact]
    public async Task GetNodesAsync_Should_Return_Empty_List_For_No_Matches()
    {
        // Arrange
        var nodeType = "NonExistentType";
        var emptyList = new List<GraphNode>();

        _mockGraphQueryService.GetNodesAsync(nodeType, null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<GraphNode>>.Success(emptyList));

        // Act
        var result = await _mockGraphQueryService.GetNodesAsync(nodeType, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    [Fact]
    public async Task GetRelationshipsAsync_Should_Return_Success_For_Valid_RelationshipType()
    {
        // Arrange
        var relationshipType = "DEPENDS_ON";
        var filters = new Dictionary<string, object> { ["strength"] = 0.8 };
        var expectedRelationships = new List<GraphRelationship>
        {
            new("rel1", "DEPENDS_ON", "node1", "node2", new Dictionary<string, object>())
        };

        _mockGraphQueryService.GetRelationshipsAsync(relationshipType, filters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<GraphRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _mockGraphQueryService.GetRelationshipsAsync(relationshipType, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Id.ShouldBe("rel1");
        result.Value[0].Type.ShouldBe("DEPENDS_ON");
    }

    [Fact]
    public async Task TraverseAsync_Should_Return_Success_For_Valid_Traversal()
    {
        // Arrange
        var startNodeId = "startNode";
        var maxDepth = 2;
        var relationshipTypes = new List<string> { "DEPENDS_ON", "REFERENCES" };
        var expectedTraversal = new GraphTraversalResult(
            new List<GraphNode>
            {
                new("startNode", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" }),
                new("targetNode", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" })
            },
            new List<GraphRelationship>
            {
                new("rel1", "DEPENDS_ON", "startNode", "targetNode", new Dictionary<string, object>())
            },
            new List<GraphPath>(),
            2,
            2,
            1);

        _mockGraphQueryService.TraverseAsync(startNodeId, maxDepth, relationshipTypes, Arg.Any<CancellationToken>())
            .Returns(Result<GraphTraversalResult>.Success(expectedTraversal));

        // Act
        var result = await _mockGraphQueryService.TraverseAsync(startNodeId, maxDepth, relationshipTypes, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalNodesVisited.ShouldBe(2);
        result.Value.TotalRelationshipsTraversed.ShouldBe(1);
        result.Value.MaxDepthReached.ShouldBe(2);
    }

    [Fact]
    public async Task FindShortestPathAsync_Should_Return_Success_For_Existing_Path()
    {
        // Arrange
        var startNodeId = "startNode";
        var endNodeId = "endNode";
        var maxDepth = 5;
        var expectedPath = new GraphPath(
            new List<GraphNode>
            {
                new("startNode", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" }),
                new("middleNode", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" }),
                new("endNode", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" })
            },
            new List<GraphRelationship>
            {
                new("rel1", "DEPENDS_ON", "startNode", "middleNode", new Dictionary<string, object>()),
                new("rel2", "DEPENDS_ON", "middleNode", "endNode", new Dictionary<string, object>())
            },
            2);

        _mockGraphQueryService.FindShortestPathAsync(startNodeId, endNodeId, maxDepth, Arg.Any<CancellationToken>())
            .Returns(Result<GraphPath?>.Success(expectedPath));

        // Act
        var result = await _mockGraphQueryService.FindShortestPathAsync(startNodeId, endNodeId, maxDepth, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Value.Length.ShouldBe(2);
        result.Value.Value.StartNode.Id.ShouldBe("startNode");
        result.Value.Value.EndNode.Id.ShouldBe("endNode");
    }

    [Fact]
    public async Task FindShortestPathAsync_Should_Return_Null_For_No_Path()
    {
        // Arrange
        var startNodeId = "startNode";
        var endNodeId = "unreachableNode";
        var maxDepth = 5;

        _mockGraphQueryService.FindShortestPathAsync(startNodeId, endNodeId, maxDepth, Arg.Any<CancellationToken>())
            .Returns(Result<GraphPath?>.Success(null));

        // Act
        var result = await _mockGraphQueryService.FindShortestPathAsync(startNodeId, endNodeId, maxDepth, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task GetStatisticsAsync_Should_Return_Success_With_Valid_Statistics()
    {
        // Arrange
        var expectedStats = new GraphStatistics(
            TotalNodes: 1000,
            TotalRelationships: 2500,
            NodeTypes: new Dictionary<string, int> { ["CodeNode"] = 800, ["PatternNode"] = 200 },
            RelationshipTypes: new Dictionary<string, int> { ["DEPENDS_ON"] = 1500, ["REFERENCES"] = 1000 },
            AverageDegree: 5.0,
            MaxDegree: 25,
            ConnectedComponents: 5,
            LastUpdated: DateTimeOffset.UtcNow);

        _mockGraphQueryService.GetStatisticsAsync(Arg.Any<CancellationToken>())
            .Returns(Result<GraphStatistics>.Success(expectedStats));

        // Act
        var result = await _mockGraphQueryService.GetStatisticsAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalNodes.ShouldBe(1000);
        result.Value.TotalRelationships.ShouldBe(2500);
        result.Value.AverageDegree.ShouldBe(5.0);
        result.Value.MaxDegree.ShouldBe(25);
        result.Value.ConnectedComponents.ShouldBe(5);
    }

    [Fact]
    public async Task ExecuteQueryAsync_Should_Handle_Null_Parameters()
    {
        // Arrange
        var query = "MATCH (n) RETURN n";
        var expectedResult = new GraphQueryResult(
            new List<GraphRecord>(),
            50,
            0,
            true);

        _mockGraphQueryService.ExecuteQueryAsync(query, null, Arg.Any<CancellationToken>())
            .Returns(Result<GraphQueryResult>.Success(expectedResult));

        // Act
        var result = await _mockGraphQueryService.ExecuteQueryAsync(query, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.RecordCount.ShouldBe(0);
    }

    [Fact]
    public async Task GetNodesAsync_Should_Handle_Null_Filters()
    {
        // Arrange
        var nodeType = "CodeNode";
        var expectedNodes = new List<GraphNode>
        {
            new("node1", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" }),
            new("node2", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" })
        };

        _mockGraphQueryService.GetNodesAsync(nodeType, null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<GraphNode>>.Success(expectedNodes));

        // Act
        var result = await _mockGraphQueryService.GetNodesAsync(nodeType, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetRelationshipsAsync_Should_Handle_Null_Filters()
    {
        // Arrange
        var relationshipType = "DEPENDS_ON";
        var expectedRelationships = new List<GraphRelationship>
        {
            new("rel1", "DEPENDS_ON", "node1", "node2", new Dictionary<string, object>()),
            new("rel2", "DEPENDS_ON", "node2", "node3", new Dictionary<string, object>())
        };

        _mockGraphQueryService.GetRelationshipsAsync(relationshipType, null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<GraphRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _mockGraphQueryService.GetRelationshipsAsync(relationshipType, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task TraverseAsync_Should_Handle_Null_RelationshipTypes()
    {
        // Arrange
        var startNodeId = "startNode";
        var maxDepth = 3;
        var expectedTraversal = new GraphTraversalResult(
            new List<GraphNode>
            {
                new("startNode", "CodeNode", new Dictionary<string, object>(), new List<string> { "CodeNode" })
            },
            new List<GraphRelationship>(),
            new List<GraphPath>(),
            0,
            1,
            0);

        _mockGraphQueryService.TraverseAsync(startNodeId, maxDepth, null, Arg.Any<CancellationToken>())
            .Returns(Result<GraphTraversalResult>.Success(expectedTraversal));

        // Act
        var result = await _mockGraphQueryService.TraverseAsync(startNodeId, maxDepth, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalNodesVisited.ShouldBe(1);
        result.Value.TotalRelationshipsTraversed.ShouldBe(0);
    }

    [Fact]
    public async Task All_Methods_Should_Respect_CancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockGraphQueryService.ExecuteQueryAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), cts.Token)
            .Returns(Result<GraphQueryResult>.WithFailure("Cancelled"));
        _mockGraphQueryService.GetNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), cts.Token)
            .Returns(Result<IReadOnlyList<GraphNode>>.WithFailure("Cancelled"));
        _mockGraphQueryService.GetRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), cts.Token)
            .Returns(Result<IReadOnlyList<GraphRelationship>>.WithFailure("Cancelled"));
        _mockGraphQueryService.TraverseAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IReadOnlyList<string>>(), cts.Token)
            .Returns(Result<GraphTraversalResult>.WithFailure("Cancelled"));
        _mockGraphQueryService.FindShortestPathAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>(), cts.Token)
            .Returns(Result<GraphPath?>.WithFailure("Cancelled"));
        _mockGraphQueryService.GetStatisticsAsync(cts.Token)
            .Returns(Result<GraphStatistics>.WithFailure("Cancelled"));

        // Act & Assert
        var executeResult = await _mockGraphQueryService.ExecuteQueryAsync("test", null, cts.Token);
        executeResult.IsFailure.ShouldBeTrue();

        var nodesResult = await _mockGraphQueryService.GetNodesAsync("test", null, cts.Token);
        nodesResult.IsFailure.ShouldBeTrue();

        var relationshipsResult = await _mockGraphQueryService.GetRelationshipsAsync("test", null, cts.Token);
        relationshipsResult.IsFailure.ShouldBeTrue();

        var traverseResult = await _mockGraphQueryService.TraverseAsync("test", 1, null, cts.Token);
        traverseResult.IsFailure.ShouldBeTrue();

        var pathResult = await _mockGraphQueryService.FindShortestPathAsync("start", "end", 1, cts.Token);
        pathResult.IsFailure.ShouldBeTrue();

        var statsResult = await _mockGraphQueryService.GetStatisticsAsync(cts.Token);
        statsResult.IsFailure.ShouldBeTrue();
    }
}
