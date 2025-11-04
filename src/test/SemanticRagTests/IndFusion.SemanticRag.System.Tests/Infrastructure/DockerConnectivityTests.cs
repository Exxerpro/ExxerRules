using IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Utilities;
using Xunit.Sdk;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure;

/// <summary>
/// Smoke tests to verify Docker connectivity and container availability for system tests.
/// Tests skip gracefully when Docker is unavailable.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="DockerConnectivityTests"/> class.
/// </remarks>
/// <param name="qdrantFixture">The Qdrant container fixture.</param>
/// <param name="neo4jFixture">The Neo4j container fixture.</param>
/// <param name="output">xUnit output helper for logging.</param>
[Collection("System")]
public sealed class DockerConnectivityTests(
    QdrantContainerFixture qdrantFixture,
    Neo4jContainerFixture neo4jFixture,
    ITestOutputHelper output)
{
    private readonly QdrantContainerFixture _qdrantFixture = qdrantFixture;
    private readonly Neo4jContainerFixture _neo4jFixture = neo4jFixture;
    private readonly ILogger<DockerConnectivityTests> _logger = XUnitLogger.CreateLogger<DockerConnectivityTests>(output);

    private static void SkipIfDockerUnavailable()
    {
        if (DockerSkipConditions.ShouldSkipDockerTests)
        {
            Assert.Skip("Docker is not available - system tests require real containers");
        }
    }

    /// <summary>
    /// Verifies that the Neo4j container is available when Docker is running.
    /// This is the first container test, so it has an extended timeout to allow for container startup and health checks.
    /// </summary>
    [Trait("Category", "Smoke")]
    [Fact(Timeout = 120000)] // 120 seconds for first container test (includes startup + health check)
    public async Task Neo4jContainer_ShouldBeAvailable_When_DockerIsRunning()
    {
        SkipIfDockerUnavailable();

        _logger.LogInformation("🧪 Checking Neo4j container availability...");
        
        // Health check: Verify container is available
        _neo4jFixture.IsAvailable.ShouldBeTrue("Neo4j container should be available");
        _neo4jFixture.BoltUri.ShouldNotBeNullOrWhiteSpace("Neo4j Bolt URI should be set");
        _neo4jFixture.Username.ShouldNotBeNullOrWhiteSpace("Neo4j username should be set");
        _neo4jFixture.Password.ShouldNotBeNullOrWhiteSpace("Neo4j password should be set");
        
        // Health check: Verify driver can execute a simple query
        if (_neo4jFixture.Driver != null)
        {
            using var session = _neo4jFixture.Driver.AsyncSession();
            var healthCheckResult = await session.RunAsync("RETURN 1 AS health_check");
            var healthCheckRecord = await healthCheckResult.SingleAsync(TestContext.Current.CancellationToken);
            healthCheckRecord["health_check"].As<int>().ShouldBe(1, "Neo4j health check query should return 1");
        }
        
        _logger.LogInformation("✅ Neo4j container available at {BoltUri}", _neo4jFixture.BoltUri);
    }

    /// <summary>
    /// Verifies that the Qdrant container is available when Docker is running.
    /// </summary>
    [Trait("Category", "Smoke")]
    [Fact(Timeout = 60000)] // 60 seconds (doubled from 30s)
    public async Task QdrantContainer_ShouldBeAvailable_When_DockerIsRunning()
    {
        SkipIfDockerUnavailable();

        _logger.LogInformation("🧪 Checking Qdrant container availability...");
        
        // Health check: Verify container is available
        _qdrantFixture.IsAvailable.ShouldBeTrue("Qdrant container should be available");
        _qdrantFixture.HttpEndpoint.ShouldNotBeNullOrWhiteSpace("Qdrant HTTP endpoint should be set");
        _qdrantFixture.GrpcEndpoint.ShouldNotBeNullOrWhiteSpace("Qdrant gRPC endpoint should be set");
        
        // Health check: Verify client can list collections
        if (_qdrantFixture.Client != null)
        {
            var healthCheckResp = await _qdrantFixture.Client.ListCollectionsAsync(TestContext.Current.CancellationToken);
            healthCheckResp.ShouldNotBeNull("Qdrant health check (list collections) should succeed");
        }
        
        _logger.LogInformation("✅ Qdrant container available at {HttpEndpoint} / {GrpcEndpoint}", _qdrantFixture.HttpEndpoint, _qdrantFixture.GrpcEndpoint);
    }

    /// <summary>
    /// Smoke test: Neo4j driver can execute a trivial query.
    /// </summary>
    [Trait("Category", "Smoke")]
    [Fact(Timeout = 60000)] // 60 seconds (doubled from 30s)
    public async Task Neo4jDriver_ShouldExecuteSimpleQuery_When_DockerIsRunning()
    {
        SkipIfDockerUnavailable();

        _logger.LogInformation("🧪 Executing simple Neo4j query...");
        using var session = _neo4jFixture.Driver!.AsyncSession();
        var result = await session.RunAsync("RETURN 1 AS x");
        var record = await result.SingleAsync(TestContext.Current.CancellationToken);
        record.ShouldNotBeNull();
        record["x"].As<int>().ShouldBe(1);
        _logger.LogInformation("✅ Neo4j simple query succeeded");
    }

    /// <summary>
    /// Smoke test: Qdrant client can list collections.
    /// </summary>
    [Trait("Category", "Smoke")]
    [Fact(Timeout = 60000)] // 60 seconds (doubled from 30s)
    public async Task QdrantClient_ShouldListCollections_When_DockerIsRunning()
    {
        SkipIfDockerUnavailable();

        _logger.LogInformation("🧪 Listing Qdrant collections...");
        var resp = await _qdrantFixture.Client!.ListCollectionsAsync(TestContext.Current.CancellationToken);
        resp.ShouldNotBeNull();
        _logger.LogInformation("✅ Qdrant list collections returned {Count} collections", resp.Count);
    }
}