using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;

namespace IndFusion.SemanticRag.Tests.Interfaces;

/// <summary>
/// ITDD tests for IVectorSearchPort interface contracts.
/// These tests verify that any implementation satisfies the interface contract using mocks.
/// </summary>
public class IVectorSearchPortTests
{
    private readonly IVectorSearchPort _mockVectorSearchPort;

    public IVectorSearchPortTests()
    {
        _mockVectorSearchPort = Substitute.For<IVectorSearchPort>();
    }

    [Fact]
    public async Task SearchAsync_Should_Return_Similar_Vectors()
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            QueryVector: queryVector,
            MaxResults: 5,
            SimilarityThreshold: 0.7
        );
        var expectedResult = new VectorSearchResult(
            Id: "search-1",
            QueryId: "query-1",
            Results: new List<VectorEmbedding>
            {
                new("emb-1", "Content 1", new float[] { 0.11f, 0.21f, 0.31f, 0.41f }, 
                    new Dictionary<string, object>(), DateTimeOffset.UtcNow),
                new("emb-2", "Content 2", new float[] { 0.12f, 0.22f, 0.32f, 0.42f }, 
                    new Dictionary<string, object>(), DateTimeOffset.UtcNow)
            },
            TotalCount: 2,
            QueryTime: TimeSpan.FromMilliseconds(50),
            Metadata: new Dictionary<string, object>()
        );

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.Success(expectedResult));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(expectedResult.Id);
        result.Value.Results.Count.ShouldBe(2);
        result.Value.TotalCount.ShouldBe(2);
        result.Value.QueryTime.ShouldBe(expectedResult.QueryTime);
    }

    [Fact]
    public async Task SearchAsync_Should_Handle_Empty_Results()
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            QueryVector: queryVector,
            MaxResults: 5,
            SimilarityThreshold: 0.9 // High threshold to get no results
        );
        var expectedResult = new VectorSearchResult(
            Id: "search-empty",
            QueryId: "query-empty",
            Results: new List<VectorEmbedding>(),
            TotalCount: 0,
            QueryTime: TimeSpan.FromMilliseconds(10),
            Metadata: new Dictionary<string, object>()
        );

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.Success(expectedResult));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Results.Count.ShouldBe(0);
        result.Value.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task SearchBatchAsync_Should_Process_Multiple_Queries()
    {
        // Arrange
        var queryVectors = new List<float[]>
        {
            new float[] { 0.1f, 0.2f, 0.3f, 0.4f },
            new float[] { 0.5f, 0.6f, 0.7f, 0.8f }
        };
        var options = new VectorSearchOptions(
            QueryVector: queryVectors[0], // Will be overridden in batch
            MaxResults: 3,
            SimilarityThreshold: 0.7
        );
        var expectedResults = new List<VectorSearchResult>
        {
            new("search-1", "query-1", new List<VectorEmbedding>(), 0, TimeSpan.FromMilliseconds(50), new Dictionary<string, object>()),
            new("search-2", "query-2", new List<VectorEmbedding>(), 0, TimeSpan.FromMilliseconds(60), new Dictionary<string, object>())
        };

        _mockVectorSearchPort.SearchBatchAsync(queryVectors, options, CancellationToken.None)
            .Returns(Result<IReadOnlyList<VectorSearchResult>>.Success(expectedResults));

        // Act
        var result = await _mockVectorSearchPort.SearchBatchAsync(queryVectors, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task IndexAsync_Should_Index_Vector_Successfully()
    {
        // Arrange
        var embedding = new VectorEmbedding(
            Id: "emb-1",
            Content: "Test content",
            Embedding: new float[] { 0.1f, 0.2f, 0.3f, 0.4f },
            Metadata: new Dictionary<string, object> { ["source"] = "test" },
            CreatedAt: DateTimeOffset.UtcNow
        );

        _mockVectorSearchPort.IndexAsync(embedding, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.IndexAsync(embedding, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task IndexBatchAsync_Should_Index_Multiple_Vectors()
    {
        // Arrange
        var embeddings = new List<VectorEmbedding>
        {
            new("emb-1", "Content 1", new float[] { 0.1f, 0.2f }, new Dictionary<string, object>(), DateTimeOffset.UtcNow),
            new("emb-2", "Content 2", new float[] { 0.3f, 0.4f }, new Dictionary<string, object>(), DateTimeOffset.UtcNow)
        };

        _mockVectorSearchPort.IndexBatchAsync(embeddings, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.IndexBatchAsync(embeddings, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Existing_Vector()
    {
        // Arrange
        var embedding = new VectorEmbedding(
            Id: "emb-1",
            Content: "Updated content",
            Embedding: new float[] { 0.5f, 0.6f, 0.7f, 0.8f },
            Metadata: new Dictionary<string, object> { ["updated"] = true },
            CreatedAt: DateTimeOffset.UtcNow
        );

        _mockVectorSearchPort.UpdateAsync(embedding, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.UpdateAsync(embedding, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Vector_Successfully()
    {
        // Arrange
        var embeddingId = "emb-1";

        _mockVectorSearchPort.DeleteAsync(embeddingId, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.DeleteAsync(embeddingId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteAsync_Should_Handle_Non_Existent_Vector()
    {
        // Arrange
        var embeddingId = "non-existent";

        _mockVectorSearchPort.DeleteAsync(embeddingId, CancellationToken.None)
            .Returns(Result.WithFailure("Vector not found"));

        // Act
        var result = await _mockVectorSearchPort.DeleteAsync(embeddingId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Vector not found");
    }

    [Fact]
    public async Task GetStatisticsAsync_Should_Return_Index_Statistics()
    {
        // Arrange
        var expectedStats = new VectorIndexStatistics(
            TotalVectors: 1000,
            IndexSize: 1024 * 1024, // 1MB
            LastUpdated: DateTimeOffset.UtcNow,
            AverageVectorDimension: 384
        );

        _mockVectorSearchPort.GetStatisticsAsync(CancellationToken.None)
            .Returns(Result<VectorIndexStatistics>.Success(expectedStats));

        // Act
        var result = await _mockVectorSearchPort.GetStatisticsAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalVectors.ShouldBe(1000);
        result.Value.IndexSize.ShouldBe(1024 * 1024);
        result.Value.AverageVectorDimension.ShouldBe(384);
    }

    [Fact]
    public async Task ClearAsync_Should_Clear_All_Vectors()
    {
        // Arrange
        _mockVectorSearchPort.ClearAsync(CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.ClearAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0.5f, 0.5f)]
    [InlineData(0.7f, 0.7f)]
    [InlineData(0.9f, 0.9f)]
    public async Task SearchAsync_Should_Respect_Similarity_Threshold(float threshold, float expectedThreshold)
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            QueryVector: queryVector,
            MaxResults: 5,
            SimilarityThreshold: threshold
        );
        var expectedResult = new VectorSearchResult(
            Id: "search-threshold",
            QueryId: "query-threshold",
            Results: new List<VectorEmbedding>(),
            TotalCount: 0,
            QueryTime: TimeSpan.FromMilliseconds(10),
            Metadata: new Dictionary<string, object> { ["threshold"] = expectedThreshold }
        );

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.Success(expectedResult));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Metadata["threshold"].ShouldBe(expectedThreshold);
    }

    [Fact]
    public async Task SearchAsync_Should_Handle_Invalid_Query_Vector()
    {
        // Arrange
        var invalidQueryVector = Array.Empty<float>();
        var options = new VectorSearchOptions(
            QueryVector: invalidQueryVector,
            MaxResults: 5,
            SimilarityThreshold: 0.7
        );

        _mockVectorSearchPort.SearchAsync(invalidQueryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.WithFailure("Invalid query vector"));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(invalidQueryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Invalid query vector");
    }
}
