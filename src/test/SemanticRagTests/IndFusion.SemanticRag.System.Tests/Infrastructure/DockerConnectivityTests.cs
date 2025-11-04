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
    /// </summary>
    [Trait("Category", "Smoke")]
    [Fact(Timeout = 30000)]
    public void Neo4jContainer_ShouldBeAvailable_When_DockerIsRunning()
    {
        SkipIfDockerUnavailable();

        _logger.LogInformation("🧪 Checking Neo4j container availability...");
        _neo4jFixture.IsAvailable.ShouldBeTrue();
        _neo4jFixture.BoltUri.ShouldNotBeNullOrWhiteSpace();
        _neo4jFixture.Username.ShouldNotBeNullOrWhiteSpace();
        _neo4jFixture.Password.ShouldNotBeNullOrWhiteSpace();
        _logger.LogInformation("✅ Neo4j container available at {BoltUri}", _neo4jFixture.BoltUri);
    }

    /// <summary>
    /// Verifies that the Qdrant container is available when Docker is running.
    /// </summary>
    [Trait("Category", "Smoke")]
    [Fact(Timeout = 30000)]
    public void QdrantContainer_ShouldBeAvailable_When_DockerIsRunning()
    {
        SkipIfDockerUnavailable();

        _logger.LogInformation("🧪 Checking Qdrant container availability...");
        _qdrantFixture.IsAvailable.ShouldBeTrue();
        _qdrantFixture.HttpEndpoint.ShouldNotBeNullOrWhiteSpace();
        _qdrantFixture.GrpcEndpoint.ShouldNotBeNullOrWhiteSpace();
        _logger.LogInformation("✅ Qdrant container available at {HttpEndpoint} / {GrpcEndpoint}", _qdrantFixture.HttpEndpoint, _qdrantFixture.GrpcEndpoint);
    }

    /// <summary>
    /// Smoke test: Neo4j driver can execute a trivial query.
    /// </summary>
    [Trait("Category", "Smoke")]
    [Fact(Timeout = 30000)]
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
    [Fact(Timeout = 30000)]
    public async Task QdrantClient_ShouldListCollections_When_DockerIsRunning()
    {
        SkipIfDockerUnavailable();

        _logger.LogInformation("🧪 Listing Qdrant collections...");
        var resp = await _qdrantFixture.Client!.ListCollectionsAsync(TestContext.Current.CancellationToken);
        resp.ShouldNotBeNull();
        _logger.LogInformation("✅ Qdrant list collections returned {Count} collections", resp.Count);
    }
}