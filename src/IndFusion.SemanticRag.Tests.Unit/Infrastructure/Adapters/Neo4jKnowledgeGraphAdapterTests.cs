using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.Tests.Unit.Helpers;
using IndFusion.SemanticRag.Tests.Unit.Shared;
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
        // Don't mock DisposeAsync - NSubstitute has trouble with IAsyncDisposable.DisposeAsync()
        // The using statement will call it, but we can verify behavior without mocking it
        // NSubstitute will handle the call automatically without explicit mocking

        _adapter = new Neo4jKnowledgeGraphAdapter(_mockDriver, _mockOptions, _mockLogger);
    }

    [Fact(Timeout = 5000)]
    public async Task StoreNodeAsync_Should_ReturnSuccess_When_ValidNodeProvided()
    {
        // Arrange
        var node = CreateValidKnowledgeNode();
        // Note: StoreNodeAsync calls session.RunAsync() but doesn't use the result, so no need to mock ToListAsync

        // Act
        var result = await _adapter.StoreNodeAsync(node, cancellationToken: CancellationToken.None);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        await _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("MERGE (n:KnowledgeNode {id: $id})")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task StoreNodeAsync_Should_ReturnFailure_When_NodeValidationFails()
    {
        // Arrange
        var invalidNode = CreateValidKnowledgeNode() with { Id = string.Empty };

        // Act
        var result = await _adapter.StoreNodeAsync(invalidNode, cancellationToken: CancellationToken.None);

        // Assert: Use error code assertion instead of fragile string assertion
        result.ShouldFailWith(ErrorCodes.KnowledgeNodeIdRequired);
        await _mockSession.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task StoreNodeAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        var node = CreateValidKnowledgeNode();

        // Act
        var result = await _adapter.StoreNodeAsync(node, cancellationToken: cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
    public async Task StoreNodesAsync_Should_ReturnSuccess_When_ValidNodesProvided()
    {
        // Arrange
        var nodes = new List<KnowledgeNode>
        {
            CreateValidKnowledgeNode("node1"),
            CreateValidKnowledgeNode("node2")
        };
        // Note: StoreNodesAsync doesn't use result cursor, so no need to mock ToListAsync

        // Act
        var result = await _adapter.StoreNodesAsync(nodes, cancellationToken: CancellationToken.None);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        await _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("UNWIND $nodes AS node")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task StoreNodesAsync_Should_ReturnSuccess_When_EmptyListProvided()
    {
        // Arrange
        var emptyNodes = new List<KnowledgeNode>();

        // Act
        var result = await _adapter.StoreNodesAsync(emptyNodes, cancellationToken: CancellationToken.None);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        await _mockSession.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task StoreNodesAsync_Should_ReturnFailure_When_AnyNodeValidationFails()
    {
        // Arrange
        // Create an invalid node directly (with empty Id) instead of using 'with' expression
        // The 'with' expression might not work correctly with records that have both parameter and property with same name
        var invalidNode = new KnowledgeNode(
            Id: string.Empty, // Invalid: empty Id
            Label: "TestLabel",
            Properties: new Dictionary<string, object> { { "key", "value" } },
            CreatedAt: DateTimeOffset.UtcNow,
            UpdatedAt: DateTimeOffset.UtcNow);
        
        var nodes = new List<KnowledgeNode>
        {
            CreateValidKnowledgeNode("node1"),
            invalidNode
        };

        // Act
        var result = await _adapter.StoreNodesAsync(nodes, cancellationToken: CancellationToken.None);

        // Assert: Use error code assertion instead of fragile string assertion
        result.ShouldFailWith(ErrorCodes.KnowledgeNodeIdRequired);
        await _mockSession.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task StoreNodesAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        var nodes = new List<KnowledgeNode>
        {
            CreateValidKnowledgeNode("node1"),
            CreateValidKnowledgeNode("node2")
        };

        // Act
        var result = await _adapter.StoreNodesAsync(nodes, cancellationToken: cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
    public async Task StoreRelationshipAsync_Should_ReturnSuccess_When_ValidRelationshipProvided()
    {
        // Arrange
        var relationship = CreateValidKnowledgeRelationship();
        // Note: StoreRelationshipAsync doesn't use result cursor, so no need to mock ToListAsync

        // Act
        var result = await _adapter.StoreRelationshipAsync(relationship, cancellationToken: CancellationToken.None);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        await _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("MATCH (source:KnowledgeNode {id: $sourceId})")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task StoreRelationshipAsync_Should_ReturnFailure_When_RelationshipValidationFails()
    {
        // Arrange
        var invalidRelationship = CreateValidKnowledgeRelationship() with { Id = string.Empty };

        // Act
        var result = await _adapter.StoreRelationshipAsync(invalidRelationship, cancellationToken: CancellationToken.None);

        // Assert: Use error code assertion instead of fragile string assertion
        result.ShouldFailWith(ErrorCodes.KnowledgeRelationshipIdRequired);
        await _mockSession.DidNotReceive().RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task StoreRelationshipAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        var relationship = CreateValidKnowledgeRelationship();

        // Act
        var result = await _adapter.StoreRelationshipAsync(relationship, cancellationToken: cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
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

        _mockResultCursor.SingleOrDefaultAsync(Arg.Any<CancellationToken>()).Returns(ValueTask.FromResult<IRecord?>(record));

        // Act
        var result = await _adapter.GetNodeByIdAsync(nodeId, cancellationToken: CancellationToken.None);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        result.Value!.Id.ShouldBe(nodeId);
        result.Value!.Label.ShouldBe("TestLabel");
    }

    [Fact(Timeout = 5000)]
    public async Task GetNodeByIdAsync_Should_ReturnFailure_When_NodeNotFound()
    {
        // Arrange
        var nodeId = "non-existent-node";
        _mockResultCursor.SingleOrDefaultAsync(Arg.Any<CancellationToken>()).Returns(ValueTask.FromResult<IRecord?>((IRecord?)null));

        // Act
        var result = await _adapter.GetNodeByIdAsync(nodeId, cancellationToken: CancellationToken.None);

        // Assert: Use error code assertion instead of fragile string assertion
        result.ShouldFailWith(ErrorCodes.EntityNotFound);
    }

    [Fact(Timeout = 5000)]
    public async Task GetNodeByIdAsync_Should_ReturnFailure_When_Neo4jThrowsException()
    {
        // Arrange
        var nodeId = "test-node-id";
        _mockResultCursor.SingleOrDefaultAsync(Arg.Any<CancellationToken>())
            .Returns<ValueTask<IRecord?>>(_ => ValueTask.FromException<IRecord?>(new Exception("Neo4j query failed")));

        // Act
        var result = await _adapter.GetNodeByIdAsync(nodeId, cancellationToken: CancellationToken.None);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.GraphDatabaseError);
    }

    [Fact(Timeout = 5000)]
    public async Task GetNodeByIdAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        var nodeId = "test-node-id";

        // Act
        var result = await _adapter.GetNodeByIdAsync(nodeId, cancellationToken: cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
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

        var cancellationToken = CancellationToken.None;
        _mockResultCursor.ToListAsync(cancellationToken).Returns(Task.FromResult<List<IRecord>>(new List<IRecord> { record }));

        // Act
        var result = await _adapter.GetRelationshipsForNodeAsync(nodeId, cancellationToken: cancellationToken);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        result.Value!.Count.ShouldBe(1);
        result.Value![0].Id.ShouldBe("rel-id");
        result.Value![0].RelationshipType.ShouldBe("RELATES_TO");
    }

    [Fact(Timeout = 5000)]
    public async Task GetRelationshipsForNodeAsync_Should_ReturnEmptyList_When_NodeHasNoRelationships()
    {
        // Arrange
        var nodeId = "test-node-id";
        var cancellationToken = CancellationToken.None;
        _mockResultCursor.ToListAsync(cancellationToken).Returns(Task.FromResult<List<IRecord>>(new List<IRecord>()));

        // Act
        var result = await _adapter.GetRelationshipsForNodeAsync(nodeId, cancellationToken: cancellationToken);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        result.Value!.Count.ShouldBe(0);
    }

    [Fact(Timeout = 5000)]
    public async Task GetRelationshipsForNodeAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        var nodeId = "test-node-id";

        // Act
        var result = await _adapter.GetRelationshipsForNodeAsync(nodeId, cancellationToken: cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteGraphQueryAsync_Should_ReturnResults_When_QueryExecutesSuccessfully()
    {
        // Arrange
        var query = "MATCH (n) RETURN n";
        var record = Substitute.For<IRecord>();
        record.Values.Returns(new Dictionary<string, object> { { "n", "test-value" } });
        var cancellationToken = CancellationToken.None;
        _mockResultCursor.ToListAsync(cancellationToken).Returns(Task.FromResult<List<IRecord>>(new List<IRecord> { record }));

        // Act
        var result = await _adapter.ExecuteGraphQueryAsync(query, cancellationToken: cancellationToken);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        result.Value!.Count.ShouldBe(1);
        result.Value![0]["n"].ShouldBe("test-value");
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteGraphQueryAsync_Should_ReturnFailure_When_QueryFails()
    {
        // Arrange
        var query = "INVALID CYPHER QUERY";
        var cancellationToken = CancellationToken.None;
        _mockResultCursor.ToListAsync(cancellationToken)
            .Returns(Task.FromException<List<IRecord>>(new Exception("Invalid syntax")));

        // Act
        var result = await _adapter.ExecuteGraphQueryAsync(query, cancellationToken: CancellationToken.None);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.CypherQueryFailed);
    }

    [Fact(Timeout = 5000)]
    public async Task ExecuteGraphQueryAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        var query = "MATCH (n) RETURN n";

        // Act
        var result = await _adapter.ExecuteGraphQueryAsync(query, cancellationToken: cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
    public async Task DeleteNodeAsync_Should_ReturnSuccess_When_NodeExists()
    {
        // Arrange
        var nodeId = "test-node-id";
        // Note: DeleteNodeAsync doesn't use result cursor, so no need to mock ToListAsync

        // Act
        var result = await _adapter.DeleteNodeAsync(nodeId, cancellationToken: CancellationToken.None);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        await _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("MATCH (n:KnowledgeNode {id: $id})") && s.Contains("DETACH DELETE n")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task DeleteNodeAsync_Should_ReturnFailure_When_Neo4jThrowsException()
    {
        // Arrange
        var nodeId = "test-node-id";
        _mockSession.RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
            .Returns(Task.FromException<IResultCursor>(new Exception("Delete failed")));

        // Act
        var result = await _adapter.DeleteNodeAsync(nodeId, cancellationToken: CancellationToken.None);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.GraphDatabaseError);
    }

    [Fact(Timeout = 5000)]
    public async Task DeleteNodeAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        var nodeId = "test-node-id";

        // Act
        var result = await _adapter.DeleteNodeAsync(nodeId, cancellationToken: cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
    }

    [Fact(Timeout = 5000)]
    public async Task DeleteRelationshipAsync_Should_ReturnSuccess_When_RelationshipExists()
    {
        // Arrange
        var relationshipId = "test-rel-id";
        // Note: DeleteRelationshipAsync doesn't use result cursor, so no need to mock ToListAsync

        // Act
        var result = await _adapter.DeleteRelationshipAsync(relationshipId, cancellationToken: CancellationToken.None);

        // Assert: Contract-based assertion
        result.ShouldSucceed();
        await _mockSession.Received(1).RunAsync(
            Arg.Is<string>(s => s.Contains("MATCH ()-[r:RELATIONSHIP {id: $id}]-()") && s.Contains("DELETE r")),
            Arg.Any<Dictionary<string, object>>());
    }

    [Fact(Timeout = 5000)]
    public async Task DeleteRelationshipAsync_Should_ReturnFailure_When_Neo4jThrowsException()
    {
        // Arrange
        var relationshipId = "test-rel-id";
        _mockSession.RunAsync(Arg.Any<string>(), Arg.Any<Dictionary<string, object>>())
            .Returns(Task.FromException<IResultCursor>(new Exception("Delete failed")));

        // Act
        var result = await _adapter.DeleteRelationshipAsync(relationshipId, cancellationToken: CancellationToken.None);

        // Assert: Use error code assertion
        result.ShouldFailWith(ErrorCodes.GraphDatabaseError);
    }

    [Fact(Timeout = 5000)]
    public async Task DeleteRelationshipAsync_WithCancellation_ShouldReturnCancelled()
    {
        // ✅ TDD: Test cancellation handling
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();
        
        var relationshipId = "test-rel-id";

        // Act
        var result = await _adapter.DeleteRelationshipAsync(relationshipId, cancellationToken: cancellationTokenSource.Token);

        // ✅ TDD: Assert cancellation contract - result must be a failure with OperationCancelled error code
        result.ShouldBeCancelled();
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
        // ✅ Use fluent builder from TestDataBuilders
        var relationshipResult = TestDataBuilders.CreateValidKnowledgeRelationship(
            id: "test-rel-id",
            fromNodeId: "source-node-id",
            toNodeId: "target-node-id");
        relationshipResult.IsSuccess.ShouldBeTrue();
        return relationshipResult.Value!; // Null-forgiving: IsSuccess guarantees non-null
    }
}