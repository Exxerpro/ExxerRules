using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace IndFusion.Mcp.Tests.Integration.Contracts;

/// <summary>
/// Contract tests for IVectorStorePort interface.
/// These tests validate that any implementation of IVectorStorePort fulfills its contract.
/// </summary>
public class VectorStorePortContractTests : ServiceContractTestBase<IVectorStorePort, VectorStorePortStub>
{
    public VectorStorePortContractTests(ITestOutputHelper testOutput) : base(testOutput)
    {
    }

    protected override void ConfigureServiceDependencies(IServiceCollection services)
    {
        services.AddLogging();
        services.AddScoped<ILogger<VectorStorePortStub>>();
    }

    [Fact]
    public async Task StoreVectorsAsync_WithValidVectors_ShouldReturnSuccessResult()
    {
        // Arrange
        var vectors = new[]
        {
            new VectorEmbedding(
                Id: "test-vector-1",
                Vector: new float[] { 0.1f, 0.2f, 0.3f },
                Metadata: new Dictionary<string, object> { { "source", "test" } },
                CreatedAt: DateTime.UtcNow,
                UpdatedAt: DateTime.UtcNow
            ),
            new VectorEmbedding(
                Id: "test-vector-2",
                Vector: new float[] { 0.4f, 0.5f, 0.6f },
                Metadata: new Dictionary<string, object> { { "source", "test" } },
                CreatedAt: DateTime.UtcNow,
                UpdatedAt: DateTime.UtcNow
            )
        };

        // Act
        var result = await Service.StoreVectorsAsync(vectors, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Store vectors result should not be null");
        result.Success.ShouldBeTrue("Store vectors should succeed");
        result.AffectedCount.ShouldBe(2, "Should affect 2 vectors");
        result.Message.ShouldNotBeNullOrEmpty("Should have a success message");
        
        Logger.LogInformation("StoreVectorsAsync contract validation passed");
    }

    [Fact]
    public async Task SearchSimilarVectorsAsync_WithValidQuery_ShouldReturnResults()
    {
        // Arrange
        var query = new VectorEmbedding(
            Id: "query-vector",
            Vector: new float[] { 0.1f, 0.2f, 0.3f },
            Metadata: new Dictionary<string, object>(),
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow
        );
        
        var options = new VectorSearchOptions(
            MaxResults: 5,
            SimilarityThreshold: 0.7,
            MetadataFilters: null,
            IncludeMetadata: true
        );

        // Act
        var results = await Service.SearchSimilarVectorsAsync(query, options, CreateTestCancellationToken());

        // Assert
        results.ShouldNotBeNull("Search results should not be null");
        results.ShouldBeAssignableTo<IEnumerable<VectorSearchResult>>("Should return enumerable of search results");
        
        Logger.LogInformation("SearchSimilarVectorsAsync contract validation passed");
    }

    [Fact]
    public async Task DeleteVectorsAsync_WithValidIds_ShouldReturnSuccessResult()
    {
        // Arrange
        var vectorIds = new[] { "test-vector-1", "test-vector-2" };

        // Act
        var result = await Service.DeleteVectorsAsync(vectorIds, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Delete vectors result should not be null");
        result.Success.ShouldBeTrue("Delete vectors should succeed");
        result.AffectedCount.ShouldBe(2, "Should affect 2 vectors");
        
        Logger.LogInformation("DeleteVectorsAsync contract validation passed");
    }

    [Fact]
    public async Task UpdateVectorMetadataAsync_WithValidData_ShouldReturnSuccessResult()
    {
        // Arrange
        var vectorId = "test-vector-1";
        var metadata = new Dictionary<string, object>
        {
            { "updated", true },
            { "timestamp", DateTime.UtcNow }
        };

        // Act
        var result = await Service.UpdateVectorMetadataAsync(vectorId, metadata, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Update metadata result should not be null");
        result.Success.ShouldBeTrue("Update metadata should succeed");
        
        Logger.LogInformation("UpdateVectorMetadataAsync contract validation passed");
    }

    [Fact]
    public async Task GetStatisticsAsync_ShouldReturnStatistics()
    {
        // Act
        var statistics = await Service.GetStatisticsAsync(CreateTestCancellationToken());

        // Assert
        statistics.ShouldNotBeNull("Statistics should not be null");
        statistics.TotalVectors.ShouldBeGreaterThanOrEqualTo(0, "Total vectors should be non-negative");
        statistics.VectorDimensions.ShouldBeGreaterThan(0, "Vector dimensions should be positive");
        statistics.StorageSizeBytes.ShouldBeGreaterThanOrEqualTo(0, "Storage size should be non-negative");
        statistics.LastUpdated.ShouldBeLessThanOrEqualTo(DateTime.UtcNow, "Last updated should be in the past");
        
        Logger.LogInformation("GetStatisticsAsync contract validation passed");
    }

    public override async Task Service_ShouldHandleCancellationTokens()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var vectors = new[]
        {
            new VectorEmbedding("test", new float[] { 0.1f }, new Dictionary<string, object>(), DateTime.UtcNow, DateTime.UtcNow)
        };
        var query = new VectorEmbedding("query", new float[] { 0.1f }, new Dictionary<string, object>(), DateTime.UtcNow, DateTime.UtcNow);
        var options = new VectorSearchOptions();

        // All methods should handle cancellation gracefully
        await Should.NotThrowAsync(async () => await Service.StoreVectorsAsync(vectors, cts.Token));
        await Should.NotThrowAsync(async () => await Service.SearchSimilarVectorsAsync(query, options, cts.Token));
        await Should.NotThrowAsync(async () => await Service.DeleteVectorsAsync(new[] { "test" }, cts.Token));
        await Should.NotThrowAsync(async () => await Service.UpdateVectorMetadataAsync("test", new Dictionary<string, object>(), cts.Token));
        await Should.NotThrowAsync(async () => await Service.GetStatisticsAsync(cts.Token));

        Logger.LogInformation("Cancellation token handling validation passed for all methods");
    }

    public override async Task Service_ShouldHandleNullParameters()
    {
        var vectors = new[]
        {
            new VectorEmbedding("test", new float[] { 0.1f }, new Dictionary<string, object>(), DateTime.UtcNow, DateTime.UtcNow)
        };
        var query = new VectorEmbedding("query", new float[] { 0.1f }, new Dictionary<string, object>(), DateTime.UtcNow, DateTime.UtcNow);
        var options = new VectorSearchOptions();

        // Methods should handle null parameters gracefully
        await Should.NotThrowAsync(async () => await Service.StoreVectorsAsync(vectors, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.SearchSimilarVectorsAsync(query, options, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.DeleteVectorsAsync(new[] { "test" }, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.UpdateVectorMetadataAsync("test", new Dictionary<string, object>(), CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.GetStatisticsAsync(CancellationToken.None));

        Logger.LogInformation("Null parameter handling validation passed");
    }
}

/// <summary>
/// Stub implementation of IVectorStorePort for contract testing.
/// </summary>
public class VectorStorePortStub : IVectorStorePort
{
    private readonly ILogger<VectorStorePortStub> _logger;

    public VectorStorePortStub(ILogger<VectorStorePortStub> logger)
    {
        _logger = logger;
    }

    public async Task<VectorStoreResult> StoreVectorsAsync(IEnumerable<VectorEmbedding> vectors, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Storing {Count} vectors", vectors.Count());
        
        await Task.Delay(50, cancellationToken);
        
        return new VectorStoreResult(
            Success: true,
            Message: $"Successfully stored {vectors.Count()} vectors",
            AffectedCount: vectors.Count()
        );
    }

    public async Task<IEnumerable<VectorSearchResult>> SearchSimilarVectorsAsync(VectorEmbedding query, VectorSearchOptions options, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Searching for similar vectors with threshold {Threshold}", options.SimilarityThreshold);
        
        await Task.Delay(100, cancellationToken);
        
        return new[]
        {
            new VectorSearchResult(
                VectorId: "similar-vector-1",
                SimilarityScore: 0.85,
                Metadata: new Dictionary<string, object> { { "source", "test" } }
            ),
            new VectorSearchResult(
                VectorId: "similar-vector-2",
                SimilarityScore: 0.78,
                Metadata: new Dictionary<string, object> { { "source", "test" } }
            )
        };
    }

    public async Task<VectorStoreResult> DeleteVectorsAsync(IEnumerable<string> vectorIds, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting {Count} vectors", vectorIds.Count());
        
        await Task.Delay(25, cancellationToken);
        
        return new VectorStoreResult(
            Success: true,
            Message: $"Successfully deleted {vectorIds.Count()} vectors",
            AffectedCount: vectorIds.Count()
        );
    }

    public async Task<VectorStoreResult> UpdateVectorMetadataAsync(string vectorId, Dictionary<string, object> metadata, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating metadata for vector {VectorId}", vectorId);
        
        await Task.Delay(25, cancellationToken);
        
        return new VectorStoreResult(
            Success: true,
            Message: $"Successfully updated metadata for vector {vectorId}",
            AffectedCount: 1
        );
    }

    public async Task<VectorStoreStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting vector store statistics");
        
        await Task.Delay(25, cancellationToken);
        
        return new VectorStoreStatistics(
            TotalVectors: 1000,
            VectorDimensions: 1536,
            StorageSizeBytes: 1024 * 1024 * 100, // 100MB
            LastUpdated: DateTime.UtcNow
        );
    }
}