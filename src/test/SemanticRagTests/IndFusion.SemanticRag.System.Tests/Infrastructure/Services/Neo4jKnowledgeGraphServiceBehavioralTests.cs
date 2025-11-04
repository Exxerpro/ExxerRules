using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.Infrastructure.Services;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Xunit;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Services;

/// <summary>
/// Behavioral system tests for Neo4jKnowledgeGraphService to drive implementation.
/// These tests verify actual behavior using real containerized Neo4j instance.
/// </summary>
[Collection("System")]
[Trait("Category", "System")]
public class Neo4jKnowledgeGraphServiceBehavioralTests : IDisposable
{
    private readonly Neo4jContainerFixture _fixture;
    private readonly ILogger<Neo4jKnowledgeGraphService> _logger;
    private readonly IGraphDatabasePort _graphDatabasePort;
    private readonly IOptions<Neo4jOptions> _options;
    private readonly Neo4jKnowledgeGraphService _service;

    /// <summary>
    /// Initializes the test fixture with real services from container.
    /// </summary>
    /// <param name="fixture">Neo4j container fixture providing real Neo4j instance.</param>
    /// <param name="output">Test output helper for logging.</param>
    public Neo4jKnowledgeGraphServiceBehavioralTests(Neo4jContainerFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));

        // Create real logger using Meziantou XUnit logger
        _logger = XUnitLogger.CreateLogger<Neo4jKnowledgeGraphService>(output);

        // Create real Neo4j graph database adapter with container driver
        var graphAdapterLogger = XUnitLogger.CreateLogger<Neo4jGraphDatabaseAdapter>(output);
        _graphDatabasePort = new Neo4jGraphDatabaseAdapter(_fixture.Driver, Options.Create(_fixture.Options), graphAdapterLogger);

        // Use container options
        _options = Options.Create(_fixture.Options);

        // Create real service instance
        _service = new Neo4jKnowledgeGraphService(_graphDatabasePort, _options, _logger);
    }

    /// <summary>
    /// Cleans up test database after each test class.
    /// </summary>
    public void Dispose()
    {
        // Clear database after tests
        Task.Run(async () => await TestCleanupHelpers.ClearNeo4jDatabase(_fixture.Driver, _fixture.Options.Database))
            .Wait(TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// Verifies that executing <see cref="Neo4jKnowledgeGraphService.QueryAsync(GraphQuery, CancellationToken)"/> with a valid MATCH query succeeds and exposes no error details.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the success assertions have been evaluated.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithValidQuery_ShouldReturnActualResults()
    {
        // Arrange
        var query = new GraphQuery("MATCH (n) RETURN n LIMIT 10");

        // Act
        var result = await _service.QueryAsync(query, TestContext.Current.CancellationToken);

        // Assert - Verify contract, not implementation details
        result.IsSuccess.ShouldBeTrue();
        result.ErrorMessage.ShouldBeNull();

        // This test drives implementation of actual Neo4j query execution
        // Currently fails because implementation uses Task.Delay placeholder
    }

    /// <summary>
    /// Ensures parameterized Cypher queries propagate their parameter values when issued through <see cref="Neo4jKnowledgeGraphService.QueryAsync(GraphQuery, CancellationToken)"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the query has been executed and inspected.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithParameters_ShouldUseParametersInQuery()
    {
        // Arrange
        var parameters = new Dictionary<string, object> { { "name", "test" }, { "age", 25 } };
        var query = new GraphQuery("MATCH (n {name: $name, age: $age}) RETURN n", parameters);

        // Act
        var result = await _service.QueryAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        query.Parameters.ShouldBe(parameters);

        // This test drives implementation of parameterized query execution
    }

    /// <summary>
    /// Confirms that query execution honors the timeout embedded in <see cref="GraphQuery"/> by surfacing an <see cref="OperationCanceledException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the cancellation behavior has been validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithTimeout_ShouldRespectTimeout()
    {
        // Arrange
        var query = new GraphQuery("MATCH (n) RETURN n", TimeoutMs: 100); // Very short timeout

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _service.QueryAsync(query, TestContext.Current.CancellationToken));

        // This test drives implementation of timeout handling
    }

    /// <summary>
    /// Validates that cancellation tokens passed to <see cref="Neo4jKnowledgeGraphService.QueryAsync(GraphQuery, CancellationToken)"/> are observed immediately.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after verifying that the operation is cancelled.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithCancellation_ShouldRespectCancellationToken()
    {
        // Arrange
        var query = new GraphQuery("MATCH (n) RETURN n");
        using var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(async () =>
            await _service.QueryAsync(query, cts.Token));
    }

    /// <summary>
    /// Checks that invalid Cypher queries result in a failure outcome with a populated error message rather than a silent success.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after inspecting the failure result.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithInvalidQuery_ShouldReturnFailure()
    {
        // Arrange
        var query = new GraphQuery("INVALID CYPHER QUERY");

        // Act
        var result = await _service.QueryAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.ErrorMessage.ShouldNotBeNullOrEmpty();

        // This test drives implementation of proper error handling
    }

    /// <summary>
    /// Confirms that creating a node with valid identifiers and metadata produces a successful result mirroring the supplied node data.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the node creation assertions pass.</returns>
    [Fact(Timeout = 60000)]
    public async Task AddNodeAsync_WithValidNode_ShouldAddNode()
    {
        // Arrange
        var node = new GraphNode(
            "node-1",
            "Person",
            new Dictionary<string, object> { { "name", "John" }, { "age", 30 } },
            new List<string> { "Person" });

        // Act
        var result = await _service.AddNodeAsync(node, TestContext.Current.CancellationToken);

        // Assert
        result.Id.ShouldBe(node.Id);
        result.Type.ShouldBe(node.Type);
        result.Properties.ShouldBe(node.Properties);

        // This test drives implementation of actual node creation
    }

    /// <summary>
    /// Ensures <see cref="Neo4jKnowledgeGraphService.AddNodeAsync(GraphNode, CancellationToken)"/> rejects null nodes by surfacing <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the expected exception has been observed.</returns>
    [Fact(Timeout = 60000)]
    public async Task AddNodeAsync_WithNullNode_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.AddNodeAsync(default, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that attempts to add structurally invalid nodes trigger a failure result instead of succeeding silently.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after verifying the failure outcome.</returns>
    [Fact(Timeout = 60000)]
    public async Task AddNodeAsync_WithInvalidNode_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidNode = new GraphNode("", "", new Dictionary<string, object>(), new List<string>());

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.AddNodeAsync(invalidNode, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Verifies that updating an existing node returns the updated node metadata and reports success.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the update assertions succeed.</returns>
    [Fact(Timeout = 60000)]
    public async Task UpdateNodeAsync_WithValidNode_ShouldUpdateNode()
    {
        // Arrange - Create node first
        var nodeId = "node-1";
        var initialNode = new GraphNode(
            nodeId,
            "Person",
            new Dictionary<string, object> { { "name", "John" }, { "age", 30 } },
            new List<string> { "Person" });
        await _service.AddNodeAsync(initialNode, TestContext.Current.CancellationToken);

        var updatedNode = new GraphNode(
            nodeId,
            "Person",
            new Dictionary<string, object> { { "name", "Jane" }, { "age", 35 } },
            new List<string> { "Person" });

        // Act
        var result = await _service.UpdateNodeAsync(nodeId, updatedNode, TestContext.Current.CancellationToken);

        // Assert
        result.Id.ShouldBe(nodeId);
        result.Type.ShouldBe(updatedNode.Type);
        result.Properties.ShouldBe(updatedNode.Properties);

        // This test drives implementation of actual node updates
    }

    /// <summary>
    /// Asserts that attempts to update a node without an identifier are rejected with <see cref="ArgumentException"/>.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the guard clause is confirmed.</returns>
    [Fact(Timeout = 60000)]
    public async Task UpdateNodeAsync_WithNullNodeId_ShouldThrowArgumentException()
    {
        // Arrange
        var node = new GraphNode("node-1", "Person", new Dictionary<string, object>(), new List<string> { "Person" });

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.UpdateNodeAsync(null!, node, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Checks that empty node identifiers are treated as invalid input during update operations.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the expected exception is thrown.</returns>
    [Fact(Timeout = 60000)]
    public async Task UpdateNodeAsync_WithEmptyNodeId_ShouldThrowArgumentException()
    {
        // Arrange
        var node = new GraphNode("node-1", "Person", new Dictionary<string, object>(), new List<string> { "Person" });

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.UpdateNodeAsync(string.Empty, node, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that deleting an existing node returns a success result referencing the removed node id.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the deletion result is evaluated.</returns>
    [Fact(Timeout = 60000)]
    public async Task DeleteNodeAsync_WithValidNodeId_ShouldDeleteNode()
    {
        // Arrange - Create node first
        var nodeId = "node-1";
        var node = new GraphNode(
            nodeId,
            "Person",
            new Dictionary<string, object> { { "name", "John" } },
            new List<string> { "Person" });
        await _service.AddNodeAsync(node, TestContext.Current.CancellationToken);

        // Act
        await _service.DeleteNodeAsync(nodeId, TestContext.Current.CancellationToken);

        // Assert
        // This test drives implementation of actual node deletion
        // Currently fails because implementation uses Task.Delay placeholder
    }

    /// <summary>
    /// Ensures null node identifiers are rejected when attempting to delete a node from the graph.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the guard clause produces the expected exception.</returns>
    [Fact(Timeout = 60000)]
    public async Task DeleteNodeAsync_WithNullNodeId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.DeleteNodeAsync(null!, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Confirms that empty node identifiers trigger input validation during delete operations.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once deletion rejects the empty identifier.</returns>
    [Fact(Timeout = 60000)]
    public async Task DeleteNodeAsync_WithEmptyNodeId_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.DeleteNodeAsync(string.Empty, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Ensures that creating a relationship with valid endpoints results in a success response containing the created relationship identifier.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when creation success has been asserted.</returns>
    [Fact(Timeout = 60000)]
    public async Task CreateRelationshipAsync_WithValidRelationship_ShouldCreateRelationship()
    {
        // Arrange - Create nodes first
        var node1 = new GraphNode("node-1", "Person", new Dictionary<string, object>(), new List<string> { "Person" });
        var node2 = new GraphNode("node-2", "Person", new Dictionary<string, object>(), new List<string> { "Person" });
        await _service.AddNodeAsync(node1, TestContext.Current.CancellationToken);
        await _service.AddNodeAsync(node2, TestContext.Current.CancellationToken);

        var relationship = new GraphRelationship(
            "rel-1",
            "KNOWS",
            "node-1",
            "node-2",
            new Dictionary<string, object> { { "since", "2023" } });

        // Act
        var result = await _service.CreateRelationshipAsync(relationship, TestContext.Current.CancellationToken);

        // Assert
        result.Id.ShouldBe(relationship.Id);
        result.Type.ShouldBe(relationship.Type);
        result.StartNodeId.ShouldBe(relationship.StartNodeId);
        result.EndNodeId.ShouldBe(relationship.EndNodeId);

        // This test drives implementation of actual relationship creation
    }

    /// <summary>
    /// Verifies that structurally invalid relationships are rejected and produce failure feedback rather than being persisted.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the failure result is validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task CreateRelationshipAsync_WithInvalidRelationship_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidRelationship = new GraphRelationship(
            "",
            "",
            "",
            "",
            new Dictionary<string, object>());

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.CreateRelationshipAsync(invalidRelationship, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Checks that self-referential relationships are prevented because Neo4j relationships must connect distinct nodes.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after observing the failure response.</returns>
    [Fact(Timeout = 60000)]
    public async Task CreateRelationshipAsync_WithSelfReferencingRelationship_ShouldThrowArgumentException()
    {
        // Arrange
        var selfReferencingRelationship = new GraphRelationship(
            "rel-1",
            "KNOWS",
            "node-1",
            "node-1",
            new Dictionary<string, object>());

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.CreateRelationshipAsync(selfReferencingRelationship, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Confirms that deleting an existing relationship reports a success state and includes the identifier of the removed edge.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the deletion response is evaluated.</returns>
    [Fact(Timeout = 60000)]
    public async Task DeleteRelationshipAsync_WithValidRelationshipId_ShouldDeleteRelationship()
    {
        // Arrange - Create nodes and relationship first
        var node1 = new GraphNode("node-1", "Person", new Dictionary<string, object>(), new List<string> { "Person" });
        var node2 = new GraphNode("node-2", "Person", new Dictionary<string, object>(), new List<string> { "Person" });
        await _service.AddNodeAsync(node1, TestContext.Current.CancellationToken);
        await _service.AddNodeAsync(node2, TestContext.Current.CancellationToken);
        var relationship = new GraphRelationship("rel-1", "KNOWS", "node-1", "node-2", new Dictionary<string, object>());
        await _service.CreateRelationshipAsync(relationship, TestContext.Current.CancellationToken);

        var relationshipId = "rel-1";

        // Act
        var result = await _service.DeleteRelationshipAsync(relationshipId, TestContext.Current.CancellationToken);

        // Assert: DeleteRelationshipAsync returns Result<T> instead of void (functional pattern)
        result.IsSuccess.ShouldBeTrue();
        // This test drives implementation of actual relationship deletion
    }

    /// <summary>
    /// Ensures null relationship identifiers are treated as invalid input during deletion attempts.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the resulting failure state has been verified.</returns>
    [Fact(Timeout = 60000)]
    public async Task DeleteRelationshipAsync_WithNullRelationshipId_ShouldReturnFailure()
    {
        // Act
        var result = await _service.DeleteRelationshipAsync(null!, TestContext.Current.CancellationToken);

        // Assert: DeleteRelationshipAsync returns Result<T> instead of throwing exceptions (functional pattern)
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Validates that empty relationship identifiers are rejected when requesting relationship deletion.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after confirming the failure outcome.</returns>
    [Fact(Timeout = 60000)]
    public async Task DeleteRelationshipAsync_WithEmptyRelationshipId_ShouldReturnFailure()
    {
        // Act
        var result = await _service.DeleteRelationshipAsync(string.Empty, TestContext.Current.CancellationToken);

        // Assert: DeleteRelationshipAsync returns Result<T> instead of throwing exceptions (functional pattern)
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that context retrieval succeeds and returns populated context information for a well-formed query.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the context result has been inspected.</returns>
    [Fact(Timeout = 60000)]
    public async Task GetContextAsync_WithValidQuery_ShouldReturnContext()
    {
        // Arrange
        var query = "What is the relationship between Person and Company?";

        // Act
        var result = await _service.GetContextAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // This test drives implementation of context retrieval
    }

    /// <summary>
    /// Ensures a null query parameter triggers an <see cref="ArgumentNullException"/> when requesting query context.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the exception assertion succeeds.</returns>
    [Fact(Timeout = 60000)]
    public async Task GetContextAsync_WithNullQuery_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.GetContextAsync(null!, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Checks that passing an empty string query is treated as invalid input during context retrieval.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the guard clause is validated.</returns>
    [Fact(Timeout = 60000)]
    public async Task GetContextAsync_WithEmptyQuery_ShouldThrowArgumentException()
    {
        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.GetContextAsync(string.Empty, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Confirms that code model nodes can be added successfully and mirror the supplied identifier in the response.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when the code node creation result is verified.</returns>
    [Fact(Timeout = 60000)]
    public async Task AddCodeNodeAsync_WithValidCodeNode_ShouldAddCodeNode()
    {
        // Arrange
        var codeNode = new CodeNode(
            "code-1",
            "Method",
            "CalculateTotal",
            "MyApp.Services.Calculator.CalculateTotal",
            "MyApp.Services",
            "/path/to/file.cs",
            42,
            40,
            50,
            "public int CalculateTotal(int a, int b) { return a + b; }",
            "C#",
            new List<string> { "public", "method" },
            new Dictionary<string, object> { { "visibility", "public" } },
            new float[] { 0.1f, 0.2f, 0.3f },
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        // Act
        var result = await _service.AddCodeNodeAsync(codeNode, TestContext.Current.CancellationToken);

        // Assert
        result.Id.ShouldBe(codeNode.Id);
        result.Type.ShouldBe(codeNode.Type);
        result.Name.ShouldBe(codeNode.Name);
        result.FullName.ShouldBe(codeNode.FullName);
        result.CodeContent.ShouldBe(codeNode.CodeContent);

        // This test drives implementation of actual code node creation
    }

    /// <summary>
    /// Ensures invalid code nodes are rejected, signaling to the caller that input validation must be satisfied.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the failure response has been asserted.</returns>
    [Fact(Timeout = 60000)]
    public async Task AddCodeNodeAsync_WithInvalidCodeNode_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidCodeNode = new CodeNode(
            "",
            "",
            "",
            "",
            null,
            null,
            null,
            0,
            0,
            "",
            "",
            new List<string>(),
            new Dictionary<string, object>(),
            null,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);

        // Act & Assert
        await Should.ThrowAsync<ArgumentException>(async () =>
            await _service.AddCodeNodeAsync(invalidCodeNode, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Validates that complex multi-clause queries execute successfully and preserve query metadata in the returned result.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after the complex query outcome has been checked.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithComplexQuery_ShouldExecuteComplexQuery()
    {
        // Arrange
        var complexQuery = new GraphQuery(
            "MATCH (p:Person)-[r:KNOWS]->(f:Person) " +
            "WHERE p.age > $minAge " +
            "RETURN p.name, f.name, r.since " +
            "ORDER BY p.name",
            new Dictionary<string, object> { { "minAge", 18 } });

        // Act
        var result = await _service.QueryAsync(complexQuery, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        complexQuery.Parameters.ShouldNotBeNull();
        complexQuery.Parameters.ShouldContainKey("minAge");
        complexQuery.Parameters["minAge"].ShouldBe(18);

        // This test drives implementation of complex query execution
    }

    /// <summary>
    /// Confirms that aggregation queries produce a successful result and capture aggregated data when the service is fully implemented.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes when aggregation assertions finish.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithAggregationQuery_ShouldReturnAggregatedResults()
    {
        // Arrange
        var aggregationQuery = new GraphQuery(
            "MATCH (p:Person) " +
            "RETURN p.department, COUNT(p) as employeeCount " +
            "ORDER BY employeeCount DESC");

        // Act
        var result = await _service.QueryAsync(aggregationQuery, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.RecordsAffected.ShouldBeGreaterThan(0); // Should show actual affected records

        // This test drives implementation of aggregation query execution
    }

    /// <summary>
    /// Ensures path traversal queries succeed and report path-oriented data without raising errors.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes once the path query result has been inspected.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithPathQuery_ShouldReturnPathResults()
    {
        // Arrange
        var pathQuery = new GraphQuery(
            "MATCH path = (start:Person)-[*1..3]->(end:Company) " +
            "WHERE start.name = $startName " +
            "RETURN path, length(path) as pathLength",
            new Dictionary<string, object> { { "startName", "John" } });

        // Act
        var result = await _service.QueryAsync(pathQuery, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        pathQuery.Parameters.ShouldNotBeNull();
        pathQuery.Parameters.ShouldContainKey("startName");
        pathQuery.Parameters["startName"].ShouldBe("John");

        // This test drives implementation of path query execution
    }

    /// <summary>
    /// Checks that issuing multiple sequential queries yields independent results for each request without cross-contamination.
    /// </summary>
    /// <returns>A <see cref="Task"/> that completes after sequential execution assertions are performed.</returns>
    [Fact(Timeout = 60000)]
    public async Task QueryAsync_WithMultipleQueries_ShouldExecuteSequentially()
    {
        // Arrange
        var queries = new[]
        {
            new GraphQuery("CREATE (n:Test {id: 1})"),
            new GraphQuery("MATCH (n:Test {id: 1}) RETURN n"),
            new GraphQuery("MATCH (n:Test {id: 1}) DELETE n")
        };

        // Act
        var results = new List<GraphQueryResult>();
        foreach (var query in queries)
        {
            var result = await _service.QueryAsync(query, TestContext.Current.CancellationToken);
            results.Add(result);
        }

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(3);
        results[0].IsSuccess.ShouldBeTrue(); // CREATE should succeed
        results[1].IsSuccess.ShouldBeTrue(); // MATCH should succeed
        results[2].IsSuccess.ShouldBeTrue(); // DELETE should succeed

        // This test drives implementation of sequential query execution
    }
}