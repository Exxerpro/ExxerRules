namespace ExxerAI.Nexus.Integration.Test.Infrastructure;

/// <summary>
/// Simple smoke tests to verify Docker connectivity and container availability.
/// These tests PASS when Docker Desktop is running and containers start successfully.
/// These tests FAIL/SKIP when Docker Desktop is not running.
/// Use these to validate Docker infrastructure before running complex integration tests.
/// </summary>
[Collection("Nexus Integration")]
public sealed class DockerConnectivityTests
{
    private readonly QdrantContainerFixture _qdrantFixture;
    private readonly Neo4jContainerFixture _neo4jFixture;
    private readonly ILogger<DockerConnectivityTests> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerConnectivityTests"/> class.
    /// </summary>
    /// <param name="qdrantFixture">The Qdrant container fixture</param>
    /// <param name="neo4jFixture">The Neo4j container fixture</param>
    public DockerConnectivityTests(
        QdrantContainerFixture qdrantFixture,
        Neo4jContainerFixture neo4jFixture)
    {
        _qdrantFixture = qdrantFixture;
        _neo4jFixture = neo4jFixture;
        _logger = XUnitLogger.CreateLogger<DockerConnectivityTests>();
    }

    /// <summary>
    /// Smoke test: Verifies that Qdrant container is available and client can connect.
    /// ✅ PASS: Docker is running, Qdrant container started successfully
    /// ❌ FAIL/SKIP: Docker is not running or Qdrant container failed to start
    /// </summary>
    [Fact(Timeout = 30_000)]
    public void QdrantContainer_ShouldBeAvailable_When_DockerIsRunning()
    {
        // Arrange
        _logger.LogInformation("🧪 Testing Qdrant container availability...");

        // Act & Assert
        _qdrantFixture.IsAvailable.ShouldBeTrue("Qdrant container should be available when Docker is running");
        _qdrantFixture.Client.ShouldNotBeNull("Qdrant client should be initialized");
        _qdrantFixture.Hostname.ShouldNotBeNullOrWhiteSpace("Qdrant hostname should be set");
        _qdrantFixture.HttpPort.ShouldBeGreaterThan(0, "Qdrant HTTP port should be mapped");

        _logger.LogInformation("✅ Qdrant container is available at {Hostname}:{Port}",
            _qdrantFixture.Hostname, _qdrantFixture.HttpPort);
    }

    /// <summary>
    /// Smoke test: Verifies that Neo4j container is available and driver can connect.
    /// ✅ PASS: Docker is running, Neo4j container started successfully
    /// ❌ FAIL/SKIP: Docker is not running or Neo4j container failed to start
    /// </summary>
    [Fact(Timeout = 30_000)]
    public void Neo4jContainer_ShouldBeAvailable_When_DockerIsRunning()
    {
        // Arrange
        _logger.LogInformation("🧪 Testing Neo4j container availability...");

        // Act & Assert
        _neo4jFixture.IsAvailable.ShouldBeTrue("Neo4j container should be available when Docker is running");
        _neo4jFixture.Driver.ShouldNotBeNull("Neo4j driver should be initialized");
        _neo4jFixture.Hostname.ShouldNotBeNullOrWhiteSpace("Neo4j hostname should be set");
        _neo4jFixture.BoltPort.ShouldBeGreaterThan(0, "Neo4j Bolt port should be mapped");
        _neo4jFixture.ConnectionString.ShouldNotBeNullOrWhiteSpace("Neo4j connection string should be set");

        _logger.LogInformation("✅ Neo4j container is available at {ConnectionString}",
            _neo4jFixture.ConnectionString);
    }

    /// <summary>
    /// Smoke test: Verifies Qdrant client can perform a basic health check operation.
    /// ✅ PASS: Docker running, Qdrant responds to API calls
    /// ❌ FAIL/SKIP: Docker not running or Qdrant not responding
    /// </summary>
    [Fact(Timeout = 30_000)]
    public async Task QdrantClient_ShouldRespondToHealthCheck_When_DockerIsRunning()
    {
        // Arrange
        _logger.LogInformation("🧪 Testing Qdrant client health check...");
        var client = _qdrantFixture.Client;

        // Act
        var collections = await client.ListCollectionsAsync();

        // Assert
        collections.ShouldNotBeNull("Qdrant should return collections list (even if empty)");
        _logger.LogInformation("✅ Qdrant health check passed - {Count} collections found",
            collections.Count);
    }

    /// <summary>
    /// Smoke test: Verifies Neo4j driver can execute a simple query.
    /// ✅ PASS: Docker running, Neo4j responds to Cypher queries
    /// ❌ FAIL/SKIP: Docker not running or Neo4j not responding
    /// </summary>
    [Fact(Timeout = 30_000)]
    public async Task Neo4jDriver_ShouldExecuteSimpleQuery_When_DockerIsRunning()
    {
        // Arrange
        _logger.LogInformation("🧪 Testing Neo4j driver with simple query...");
        var driver = _neo4jFixture.Driver;

        // Act
        await using var session = driver.AsyncSession();
        var cursor = await session.RunAsync("RETURN 1 AS testValue, 'Docker is working!' AS message");
        var result = await cursor.SingleAsync();

        // Assert
        result.ShouldNotBeNull("Neo4j should return query result");
        result["testValue"].As<int>().ShouldBe(1, "Query should return expected value");
        result["message"].As<string>().ShouldBe("Docker is working!", "Query should return expected message");

        _logger.LogInformation("✅ Neo4j query executed successfully: {Message}",
            result["message"].As<string>());
    }

    /// <summary>
    /// Smoke test: Verifies Qdrant client can list collections (simple read operation).
    /// ✅ PASS: Docker running, Qdrant responds to list request
    /// ❌ FAIL/SKIP: Docker not running or Qdrant not responding
    /// </summary>
    [Fact(Timeout = 30_000)]
    public async Task QdrantClient_ShouldListCollections_When_DockerIsRunning()
    {
        // Arrange
        _logger.LogInformation("🧪 Testing Qdrant list collections...");
        var client = _qdrantFixture.Client;

        // Act
        var collections = await client.ListCollectionsAsync();

        // Assert
        collections.ShouldNotBeNull("Qdrant should return collections list (even if empty)");
        _logger.LogInformation("✅ Qdrant list collections succeeded - {Count} collections found",
            collections.Count);
    }

    /// <summary>
    /// Smoke test: Creates a test node in Neo4j and verifies it exists.
    /// ✅ PASS: Docker running, can create and query nodes
    /// ❌ FAIL/SKIP: Docker not running or Neo4j operations fail
    /// </summary>
    [Fact(Timeout = 30_000)]
    public async Task Neo4jDriver_ShouldCreateAndQueryNode_When_DockerIsRunning()
    {
        // Arrange
        _logger.LogInformation("🧪 Testing Neo4j node creation...");
        var driver = _neo4jFixture.Driver;
        var testNodeId = Guid.NewGuid().ToString("N");

        try
        {
            await using var session = driver.AsyncSession();

            // Act - Create node
            await session.RunAsync(
                "CREATE (n:SmokeTest {id: $id, message: $message})",
                new { id = testNodeId, message = "Docker connectivity test" });

            // Act - Query node
            var cursor = await session.RunAsync(
                "MATCH (n:SmokeTest {id: $id}) RETURN n.message AS message",
                new { id = testNodeId });

            var result = await cursor.SingleAsync();

            // Assert
            result.ShouldNotBeNull("Neo4j should return created node");
            result["message"].As<string>().ShouldBe("Docker connectivity test",
                "Node should contain expected message");

            _logger.LogInformation("✅ Successfully created and queried Neo4j node with ID '{NodeId}'",
                testNodeId);
        }
        finally
        {
            // Cleanup - Delete test node
            try
            {
                await using var session = driver.AsyncSession();
                await session.RunAsync(
                    "MATCH (n:SmokeTest {id: $id}) DELETE n",
                    new { id = testNodeId });

                _logger.LogInformation("🧹 Cleaned up test node with ID '{NodeId}'", testNodeId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Failed to cleanup test node (non-fatal)");
            }
        }
    }
}
