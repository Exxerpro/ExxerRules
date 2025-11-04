using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Implementations;

/// <summary>
/// TDD tests for GraphQueryService using real implementations.
/// </summary>
public class GraphQueryServiceTests
{
    private readonly IKnowledgeGraphPort _knowledgeGraphPort;
    private readonly ILogger<GraphQueryService> _logger;
    private readonly GraphQueryService _graphQueryService;

    /// <summary>
    /// Initializes a new instance of the GraphQueryServiceTests class.
    /// </summary>
    public GraphQueryServiceTests()
    {
        _knowledgeGraphPort = Substitute.For<IKnowledgeGraphPort>();
        _logger = Substitute.For<ILogger<GraphQueryService>>();
        _graphQueryService = new GraphQueryService(_knowledgeGraphPort, _logger);
    }

    /// <summary>
    /// Verifies that ExecuteQueryAsync executes valid query successfully.
    /// </summary>
    [Fact]
    public async Task ExecuteQueryAsync_Should_Execute_Valid_Query_Successfully()
    {
        // Arrange
        var query = "MATCH (n) RETURN n";
        var parameters = new Dictionary<string, object> { ["limit"] = 10 };
        var expectedNodes = new List<KnowledgeNode>
        {
            new(
                Id: "node1",
                Label: "CodeNode",
                Properties: new Dictionary<string, object> { ["name"] = "TestNode" },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(query, parameters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(expectedNodes));
        _knowledgeGraphPort.QueryRelationshipsAsync(query, parameters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success([]));

        // Act
        var result = await _graphQueryService.ExecuteQueryAsync(query, parameters, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<GraphQueryResult>(default);
        result.Value.IsSuccess.ShouldBeTrue();
        result.Value.RecordCount.ShouldBe(1);
        result.Value.ExecutionTimeMs.ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Verifies that ExecuteQueryAsync handles empty query.
    /// </summary>
    [Fact]
    public async Task ExecuteQueryAsync_Should_Handle_Empty_Query()
    {
        // Arrange
        var query = "";

        // Act
        var result = await _graphQueryService.ExecuteQueryAsync(query, null, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Query cannot be null or empty");
    }

    /// <summary>
    /// Verifies that ExecuteQueryAsync handles query failure.
    /// </summary>
    [Fact]
    public async Task ExecuteQueryAsync_Should_Handle_Query_Failure()
    {
        // Arrange
        var query = "MATCH (n) RETURN n";
        var expectedError = "Database connection failed";

        _knowledgeGraphPort.QueryNodesAsync(query, null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.WithFailure(expectedError));
        _knowledgeGraphPort.QueryRelationshipsAsync(query, null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure(expectedError));

        // Act
        var result = await _graphQueryService.ExecuteQueryAsync(query, null, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Query execution failed");
    }

    /// <summary>
    /// Verifies that GetNodesAsync retrieves nodes by type.
    /// </summary>
    [Fact]
    public async Task GetNodesAsync_Should_Retrieve_Nodes_By_Type()
    {
        // Arrange
        var nodeType = "CodeNode";
        var expectedNodes = new List<KnowledgeNode>
        {
            new(
                Id: "node1",
                Label: "CodeNode",
                Properties: new Dictionary<string, object> { ["name"] = "TestNode1" },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "node2",
                Label: "CodeNode",
                Properties: new Dictionary<string, object> { ["name"] = "TestNode2" },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(expectedNodes));

        // Act
        var result = await _graphQueryService.GetNodesAsync(nodeType, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value[0].Id.ShouldBe("node1");
        result.Value[0].Type.ShouldBe("CodeNode");
        result.Value[1].Id.ShouldBe("node2");
        result.Value[1].Type.ShouldBe("CodeNode");
    }

    /// <summary>
    /// Verifies that GetNodesAsync handles empty node type.
    /// </summary>
    [Fact]
    public async Task GetNodesAsync_Should_Handle_Empty_NodeType()
    {
        // Arrange
        var nodeType = "";

        // Act
        var result = await _graphQueryService.GetNodesAsync(nodeType, null, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Node type cannot be null or empty");
    }

    /// <summary>
    /// Verifies that GetNodesAsync applies filters.
    /// </summary>
    [Fact]
    public async Task GetNodesAsync_Should_Apply_Filters()
    {
        // Arrange
        var nodeType = "CodeNode";
        var filters = new Dictionary<string, object> { ["language"] = "C#" };
        var expectedNodes = new List<KnowledgeNode>
        {
            new(
                Id: "node1",
                Label: "CodeNode",
                Properties: new Dictionary<string, object> { ["name"] = "TestNode", ["language"] = "C#" },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), filters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(expectedNodes));

        // Act
        var result = await _graphQueryService.GetNodesAsync(nodeType, filters, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].GetProperty<string>("language").ShouldBe("C#");
    }

    /// <summary>
    /// Verifies that GetRelationshipsAsync retrieves relationships by type.
    /// </summary>
    [Fact]
    public async Task GetRelationshipsAsync_Should_Retrieve_Relationships_By_Type()
    {
        // Arrange
        var relationshipType = "DEPENDS_ON";
        var expectedRelationships = new List<KnowledgeRelationship>
        {
            new(
                Id: "rel1",
                RelationshipType: "DEPENDS_ON",
                FromNodeId: "node1",
                ToNodeId: "node2",
                Properties: new Dictionary<string, object> { ["strength"] = 0.8 },
                CreatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _graphQueryService.GetRelationshipsAsync(relationshipType, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Id.ShouldBe("rel1");
        result.Value[0].Type.ShouldBe("DEPENDS_ON");
        result.Value[0].StartNodeId.ShouldBe("node1");
        result.Value[0].EndNodeId.ShouldBe("node2");
    }

    /// <summary>
    /// Verifies that GetRelationshipsAsync handles empty relationship type.
    /// </summary>
    [Fact]
    public async Task GetRelationshipsAsync_Should_Handle_Empty_RelationshipType()
    {
        // Arrange
        var relationshipType = "";

        // Act
        var result = await _graphQueryService.GetRelationshipsAsync(relationshipType, null, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Relationship type cannot be null or empty");
    }

    /// <summary>
    /// Verifies that TraverseAsync performs graph traversal.
    /// </summary>
    [Fact]
    public async Task TraverseAsync_Should_Perform_Graph_Traversal()
    {
        // Arrange
        var startNodeId = "startNode";
        var maxDepth = 2;
        var relationshipTypes = new List<string> { "DEPENDS_ON" };
        var expectedNodes = new List<KnowledgeNode>
        {
            new(
                Id: "startNode",
                Label: "CodeNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "targetNode",
                Label: "CodeNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };
        var expectedRelationships = new List<KnowledgeRelationship>
        {
            new(
                Id: "rel1",
                RelationshipType: "DEPENDS_ON",
                FromNodeId: "startNode",
                ToNodeId: "targetNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(expectedNodes));
        _knowledgeGraphPort.QueryRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _graphQueryService.TraverseAsync(startNodeId, maxDepth, relationshipTypes, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<GraphTraversalResult>(default);
        result.Value.TotalNodesVisited.ShouldBe(2);
        result.Value.TotalRelationshipsTraversed.ShouldBe(1);
        result.Value.Nodes.Count.ShouldBe(2);
        result.Value.Relationships.Count.ShouldBe(1);
    }

    /// <summary>
    /// Verifies that TraverseAsync handles invalid start node ID.
    /// </summary>
    [Fact]
    public async Task TraverseAsync_Should_Handle_Invalid_StartNodeId()
    {
        // Arrange
        var startNodeId = "";

        // Act
        var result = await _graphQueryService.TraverseAsync(startNodeId, 3, null, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Start node ID cannot be null or empty");
    }

    /// <summary>
    /// Verifies that TraverseAsync handles negative max depth.
    /// </summary>
    [Fact]
    public async Task TraverseAsync_Should_Handle_Negative_MaxDepth()
    {
        // Arrange
        var startNodeId = "startNode";
        var maxDepth = -1;

        // Act
        var result = await _graphQueryService.TraverseAsync(startNodeId, maxDepth, null, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Max depth cannot be negative");
    }

    /// <summary>
    /// Verifies that FindShortestPathAsync finds path between nodes.
    /// </summary>
    [Fact]
    public async Task FindShortestPathAsync_Should_Find_Path_Between_Nodes()
    {
        // Arrange
        var startNodeId = "startNode";
        var endNodeId = "endNode";
        var maxDepth = 5;
        var expectedNodes = new List<KnowledgeNode>
        {
            new(
                Id: "startNode",
                Label: "CodeNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "middleNode",
                Label: "CodeNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "endNode",
                Label: "CodeNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };
        var expectedRelationships = new List<KnowledgeRelationship>
        {
            new(
                Id: "rel1",
                RelationshipType: "DEPENDS_ON",
                FromNodeId: "startNode",
                ToNodeId: "middleNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "rel2",
                RelationshipType: "DEPENDS_ON",
                FromNodeId: "middleNode",
                ToNodeId: "endNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(expectedNodes));
        _knowledgeGraphPort.QueryRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _graphQueryService.FindShortestPathAsync(startNodeId, endNodeId, maxDepth, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<GraphPath?>(default);
        result.Value!.Value.Length.ShouldBe(2);
        result.Value.Value.StartNode.Id.ShouldBe("startNode");
        result.Value.Value.EndNode.Id.ShouldBe("endNode");
        result.Value.Value.Nodes.Count.ShouldBe(3);
        result.Value.Value.Relationships.Count.ShouldBe(2);
    }

    /// <summary>
    /// Verifies that FindShortestPathAsync returns null for no path.
    /// </summary>
    [Fact]
    public async Task FindShortestPathAsync_Should_Return_Null_For_No_Path()
    {
        // Arrange
        var startNodeId = "startNode";
        var endNodeId = "unreachableNode";
        var maxDepth = 5;

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success([]));
        _knowledgeGraphPort.QueryRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success([]));

        // Act
        var result = await _graphQueryService.FindShortestPathAsync(startNodeId, endNodeId, maxDepth, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeNull();
    }

    /// <summary>
    /// Verifies that FindShortestPathAsync handles invalid parameters.
    /// </summary>
    [Fact]
    public async Task FindShortestPathAsync_Should_Handle_Invalid_Parameters()
    {
        // Arrange
        var startNodeId = "";
        var endNodeId = "endNode";

        // Act
        var result = await _graphQueryService.FindShortestPathAsync(startNodeId, endNodeId, 5, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Start node ID cannot be null or empty");
    }

    /// <summary>
    /// Verifies that GetStatisticsAsync returns graph statistics.
    /// </summary>
    [Fact]
    public async Task GetStatisticsAsync_Should_Return_Graph_Statistics()
    {
        // Arrange
        var nodeCount = 1000;
        var relationshipCount = 2500;

        _knowledgeGraphPort.GetNodeCountAsync(Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(nodeCount));
        _knowledgeGraphPort.GetRelationshipCountAsync(Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(relationshipCount));
        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success([]));
        _knowledgeGraphPort.QueryRelationshipsAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success([]));

        // Act
        var result = await _graphQueryService.GetStatisticsAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<GraphStatistics>(default);
        result.Value.TotalNodes.ShouldBe(nodeCount);
        result.Value.TotalRelationships.ShouldBe(relationshipCount);
        result.Value.AverageDegree.ShouldBe((double)relationshipCount / nodeCount);
        result.Value.LastUpdated.ShouldNotBe(default(DateTimeOffset));
    }

    /// <summary>
    /// Verifies that GetStatisticsAsync handles node count failure.
    /// </summary>
    [Fact]
    public async Task GetStatisticsAsync_Should_Handle_Node_Count_Failure()
    {
        // Arrange
        var expectedError = "Failed to get node count";

        _knowledgeGraphPort.GetNodeCountAsync(Arg.Any<CancellationToken>())
            .Returns(Result<int>.WithFailure(expectedError));

        // Act
        var result = await _graphQueryService.GetStatisticsAsync(CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    /// <summary>
    /// Verifies that all methods handle cancellation.
    /// </summary>
    [Fact]
    public async Task All_Methods_Should_Handle_Cancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), cts.Token)
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.WithFailure("Cancelled"));
        _knowledgeGraphPort.QueryRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), cts.Token)
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure("Cancelled"));
        _knowledgeGraphPort.GetNodeCountAsync(cts.Token)
            .Returns(Result<int>.WithFailure("Cancelled"));
        _knowledgeGraphPort.GetRelationshipCountAsync(cts.Token)
            .Returns(Result<int>.WithFailure("Cancelled"));

        // Act & Assert
        var executeResult = await _graphQueryService.ExecuteQueryAsync("test", null, cts.Token);
        executeResult.IsFailure.ShouldBeTrue();
        executeResult.Error.ShouldContain("cancelled");

        var nodesResult = await _graphQueryService.GetNodesAsync("test", null, cts.Token);
        nodesResult.IsFailure.ShouldBeTrue();
        nodesResult.Error.ShouldContain("cancelled");

        var relationshipsResult = await _graphQueryService.GetRelationshipsAsync("test", null, cts.Token);
        relationshipsResult.IsFailure.ShouldBeTrue();
        relationshipsResult.Error.ShouldContain("cancelled");

        var traverseResult = await _graphQueryService.TraverseAsync("test", 1, null, cts.Token);
        traverseResult.IsFailure.ShouldBeTrue();
        traverseResult.Error.ShouldContain("cancelled");

        var pathResult = await _graphQueryService.FindShortestPathAsync("start", "end", 1, cts.Token);
        pathResult.IsFailure.ShouldBeTrue();
        pathResult.Error.ShouldContain("cancelled");

        var statsResult = await _graphQueryService.GetStatisticsAsync(cts.Token);
        statsResult.IsFailure.ShouldBeTrue();
        statsResult.Error.ShouldContain("cancelled");
    }

    /// <summary>
    /// Verifies that ExecuteQueryAsync uses ConfigureAwait(false).
    /// </summary>
    [Fact]
    public async Task ExecuteQueryAsync_Should_Use_ConfigureAwait_False()
    {
        // Arrange
        var query = "MATCH (n) RETURN n";
        var expectedNodes = new List<KnowledgeNode>
        {
            new(
                Id: "node1",
                Label: "CodeNode",
                Properties: new Dictionary<string, object>(),
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(query, null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(expectedNodes));
        _knowledgeGraphPort.QueryRelationshipsAsync(query, null, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success([]));

        // Act
        var result = await _graphQueryService.ExecuteQueryAsync(query, null, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        // Verify that ConfigureAwait(false) was used by checking the call was made
        await _knowledgeGraphPort.Received().QueryNodesAsync(query, null, Arg.Any<CancellationToken>());
        await _knowledgeGraphPort.Received().QueryRelationshipsAsync(query, null, Arg.Any<CancellationToken>());
    }
}
