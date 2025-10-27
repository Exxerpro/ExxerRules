using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Infrastructure.Adapters;

/// <summary>
/// Unit tests for the Neo4jKnowledgeGraphAdapter.
/// </summary>
public class Neo4jKnowledgeGraphAdapterTests
{
    private readonly IDriver _mockDriver;
    private readonly IAsyncSession _mockSession;
    private readonly IResultCursor _mockResultCursor;
    private readonly ILogger<Neo4jKnowledgeGraphAdapter> _mockLogger;
    private readonly IOptions<Neo4jOptions> _mockOptions;
    private readonly Neo4jKnowledgeGraphAdapter _adapter;

    public Neo4jKnowledgeGraphAdapterTests()
    {
        _mockDriver = Substitute.For<IDriver>();
        _mockSession = Substitute.For<IAsyncSession>();
        _mockResultCursor = Substitute.For<IResultCursor>();
        _mockLogger = Substitute.For<ILogger<Neo4jKnowledgeGraphAdapter>>();
        
        var options = new Neo4jOptions
        {
            Uri = "bolt://localhost:7687",
            Username = "neo4j",
            Password = "password",
            Database = "neo4j"
        };
        _mockOptions = Substitute.For<IOptions<Neo4jOptions>>();
        _mockOptions.Value.Returns(options);

        _mockDriver.AsyncSession(Arg.Any<Action<SessionConfigBuilder>>()).Returns(_mockSession);
        _mockSession.RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
            .Returns(_mockResultCursor);

        _adapter = new Neo4jKnowledgeGraphAdapter(_mockDriver, _mockOptions, _mockLogger);
    }

    [Fact]
    public async Task StoreNodeAsync_Should_ReturnSuccess_When_ValidNodeProvided()
    {
        // Arrange
        var node = CreateValidKnowledgeNode();
        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>()).Returns(new List<IRecord>());

        // Act
        var result = await _adapter.StoreNodeAsync(node);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("MERGE (n:KnowledgeNode {id: $id})")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task StoreNodeAsync_Should_ReturnFailure_When_NodeValidationFails()
    {
        // Arrange
        var invalidNode = CreateValidKnowledgeNode() with { Id = string.Empty };

        // Act
        var result = await _adapter.StoreNodeAsync(invalidNode);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("KnowledgeNode ID cannot be empty or whitespace");
        _mockSession.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task StoreNodeAsync_Should_ReturnFailure_When_Neo4jThrowsException()
    {
        // Arrange
        var node = CreateValidKnowledgeNode();
        _mockSession.RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
            .Returns(Task.FromException<IResultCursor>(new Exception("Neo4j connection failed")));

        // Act
        var result = await _adapter.StoreNodeAsync(node);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Failed to store node");
        result.Error!.ShouldContain("Neo4j connection failed");
    }

    [Fact]
    public async Task StoreNodesAsync_Should_ReturnSuccess_When_ValidNodesProvided()
    {
        // Arrange
        var nodes = new List<KnowledgeNode>
        {
            CreateValidKnowledgeNode("node1"),
            CreateValidKnowledgeNode("node2")
        };
        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>()).Returns(new List<IRecord>());

        // Act
        var result = await _adapter.StoreNodesAsync(nodes);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("UNWIND $nodes AS node")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task StoreNodesAsync_Should_ReturnSuccess_When_EmptyListProvided()
    {
        // Arrange
        var emptyNodes = new List<KnowledgeNode>();

        // Act
        var result = await _adapter.StoreNodesAsync(emptyNodes);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _mockSession.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task StoreNodesAsync_Should_ReturnFailure_When_AnyNodeValidationFails()
    {
        // Arrange
        var nodes = new List<KnowledgeNode>
        {
            CreateValidKnowledgeNode("node1"),
            CreateValidKnowledgeNode("node2") with { Id = string.Empty }
        };

        // Act
        var result = await _adapter.StoreNodesAsync(nodes);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("KnowledgeNode ID cannot be empty or whitespace");
        _mockSession.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task StoreRelationshipAsync_Should_ReturnSuccess_When_ValidRelationshipProvided()
    {
        // Arrange
        var relationship = CreateValidKnowledgeRelationship();
        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>()).Returns(new List<IRecord>());

        // Act
        var result = await _adapter.StoreRelationshipAsync(relationship);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("MATCH (source:KnowledgeNode {id: $sourceId})")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task StoreRelationshipAsync_Should_ReturnFailure_When_RelationshipValidationFails()
    {
        // Arrange
        var invalidRelationship = CreateValidKnowledgeRelationship() with { Id = string.Empty };

        // Act
        var result = await _adapter.StoreRelationshipAsync(invalidRelationship);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("KnowledgeRelationship ID cannot be empty or whitespace");
        _mockSession.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task GetNodeByIdAsync_Should_ReturnNode_When_NodeExists()
    {
        // Arrange
        var nodeId = "test-node-id";
        var record = Substitute.For<IRecord>();
        record["id"].Returns(nodeId);
        record["label"].Returns("TestLabel");
        record["properties"].Returns(new Dictionary<string, object> { { "key", "value" } });
        record["createdAt"].Returns(DateTimeOffset.UtcNow);
        record["updatedAt"].Returns(DateTimeOffset.UtcNow);

        _mockResultCursor.SingleOrDefaultAsync(Arg.Any<CancellationToken>()).Returns(record);

        // Act
        var result = await _adapter.GetNodeByIdAsync(nodeId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Id.ShouldBe(nodeId);
        result.Value!.Label.ShouldBe("TestLabel");
    }

    [Fact]
    public async Task GetNodeByIdAsync_Should_ReturnFailure_When_NodeNotFound()
    {
        // Arrange
        var nodeId = "non-existent-node";
        _mockResultCursor.SingleOrDefaultAsync(Arg.Any<CancellationToken>()).Returns((IRecord?)null);

        // Act
        var result = await _adapter.GetNodeByIdAsync(nodeId);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain($"Node not found: {nodeId}");
    }

    [Fact]
    public async Task GetNodeByIdAsync_Should_ReturnFailure_When_Neo4jThrowsException()
    {
        // Arrange
        var nodeId = "test-node-id";
        _mockResultCursor.SingleOrDefaultAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IResultCursor>(new Exception("Neo4j query failed")));

        // Act
        var result = await _adapter.GetNodeByIdAsync(nodeId);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Failed to retrieve node");
        result.Error!.ShouldContain("Neo4j query failed");
    }

    [Fact]
    public async Task GetRelationshipsForNodeAsync_Should_ReturnRelationships_When_NodeHasRelationships()
    {
        // Arrange
        var nodeId = "test-node-id";
        var record = Substitute.For<IRecord>();
        record["id"].Returns("rel-id");
        record["type"].Returns("RELATES_TO");
        record["properties"].Returns(new Dictionary<string, object> { { "key", "value" } });
        record["createdAt"].Returns(DateTimeOffset.UtcNow);
        record["sourceId"].Returns("source-id");
        record["targetId"].Returns("target-id");

        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>()).Returns(new List<IRecord> { record });

        // Act
        var result = await _adapter.GetRelationshipsForNodeAsync(nodeId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
        result.Value![0].Id.ShouldBe("rel-id");
        result.Value![0].RelationshipType.ShouldBe("RELATES_TO");
    }

    [Fact]
    public async Task GetRelationshipsForNodeAsync_Should_ReturnEmptyList_When_NodeHasNoRelationships()
    {
        // Arrange
        var nodeId = "test-node-id";
        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>()).Returns(new List<IRecord>());

        // Act
        var result = await _adapter.GetRelationshipsForNodeAsync(nodeId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ExecuteGraphQueryAsync_Should_ReturnResults_When_QueryExecutesSuccessfully()
    {
        // Arrange
        var query = "MATCH (n) RETURN n";
        var record = Substitute.For<IRecord>();
        record.Values.Returns(new Dictionary<string, object> { { "n", "test-value" } });
        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>()).Returns(new List<IRecord> { record });

        // Act
        var result = await _adapter.ExecuteGraphQueryAsync(query);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value!.Count.ShouldBe(1);
        result.Value![0]["n"].ShouldBe("test-value");
    }

    [Fact]
    public async Task ExecuteGraphQueryAsync_Should_ReturnFailure_When_QueryFails()
    {
        // Arrange
        var query = "INVALID CYPHER QUERY";
        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IResultCursor>(new Exception("Invalid syntax")));

        // Act
        var result = await _adapter.ExecuteGraphQueryAsync(query);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Failed to execute graph query");
        result.Error!.ShouldContain("Invalid syntax");
    }

    [Fact]
    public async Task DeleteNodeAsync_Should_ReturnSuccess_When_NodeExists()
    {
        // Arrange
        var nodeId = "test-node-id";
        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>()).Returns(new List<IRecord>());

        // Act
        var result = await _adapter.DeleteNodeAsync(nodeId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("MATCH (n:KnowledgeNode {id: $id})") && s.Contains("DETACH DELETE n")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task DeleteNodeAsync_Should_ReturnFailure_When_Neo4jThrowsException()
    {
        // Arrange
        var nodeId = "test-node-id";
        _mockSession.RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
            .Returns(Task.FromException<IResultCursor>(new Exception("Delete failed")));

        // Act
        var result = await _adapter.DeleteNodeAsync(nodeId);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Failed to delete node");
        result.Error!.ShouldContain("Delete failed");
    }

    [Fact]
    public async Task DeleteRelationshipAsync_Should_ReturnSuccess_When_RelationshipExists()
    {
        // Arrange
        var relationshipId = "test-rel-id";
        _mockResultCursor.ToListAsync(Arg.Any<CancellationToken>()).Returns(new List<IRecord>());

        // Act
        var result = await _adapter.DeleteRelationshipAsync(relationshipId);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("MATCH ()-[r:RELATIONSHIP {id: $id}]-()") && s.Contains("DELETE r")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact]
    public async Task DeleteRelationshipAsync_Should_ReturnFailure_When_Neo4jThrowsException()
    {
        // Arrange
        var relationshipId = "test-rel-id";
        _mockSession.RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
            .Returns(Task.FromException<IResultCursor>(new Exception("Delete failed")));

        // Act
        var result = await _adapter.DeleteRelationshipAsync(relationshipId);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Failed to delete relationship");
        result.Error!.ShouldContain("Delete failed");
    }

    private static KnowledgeNode CreateValidKnowledgeNode(string id = "test-node-id")
    {
        return new KnowledgeNode(
            id,
            "TestLabel",
            new Dictionary<string, object> { { "key", "value" } },
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow
        );
    }

    private static KnowledgeRelationship CreateValidKnowledgeRelationship()
    {
        return new KnowledgeRelationship(
            "test-rel-id",
            "source-node-id",
            "target-node-id",
            "RELATES_TO",
            new Dictionary<string, object> { { "key", "value" } },
            DateTimeOffset.UtcNow
        );
    }
}