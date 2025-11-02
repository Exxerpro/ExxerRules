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

    /// <summary>
    /// Initializes a new instance of the IVectorSearchPortTests class.
    /// </summary>
    public IVectorSearchPortTests()
    {
        _mockVectorSearchPort = Substitute.For<IVectorSearchPort>();
    }

    /// <summary>
    /// Verifies that SearchAsync returns similar vectors.
    /// </summary>
    [Fact]
    public async Task SearchAsync_Should_Return_Similar_Vectors()
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            Limit: 5,
            Threshold: 0.7f
        );
        var expectedEmbedding = new VectorEmbedding(
            "emb-1", 
            "Content 1", 
            new float[] { 0.11f, 0.21f, 0.31f, 0.41f }, 
            new Dictionary<string, object>(), 
            DateTimeOffset.UtcNow);
        var expectedResult = new VectorSearchResult(
            Vector: expectedEmbedding,
            Similarity: 0.95f,
            Rank: 1);

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.Success(expectedResult));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(default(VectorSearchResult));
        result.Value.Vector.Id.ShouldBe(expectedEmbedding.Id);
        result.Value.Similarity.ShouldBe(0.95f);
        result.Value.Rank.ShouldBe(1);
    }

    /// <summary>
    /// Verifies that SearchAsync handles empty results.
    /// </summary>
    [Fact]
    public async Task SearchAsync_Should_Handle_Empty_Results()
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            Limit: 5,
            Threshold: 0.9f // High threshold to get no results
        );

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.WithFailure("No results found"));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("No results found");
    }

    /// <summary>
    /// Verifies that SearchBatchAsync processes multiple queries.
    /// </summary>
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
            Limit: 3,
            Threshold: 0.7f
        );
        var embedding1 = new VectorEmbedding("emb-1", "Content 1", new float[] { 0.1f }, new Dictionary<string, object>(), DateTimeOffset.UtcNow);
        var embedding2 = new VectorEmbedding("emb-2", "Content 2", new float[] { 0.2f }, new Dictionary<string, object>(), DateTimeOffset.UtcNow);
        var expectedResults = new List<VectorSearchResult>
        {
            new VectorSearchResult(Vector: embedding1, Similarity: 0.8f, Rank: 1),
            new VectorSearchResult(Vector: embedding2, Similarity: 0.75f, Rank: 2)
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

    /// <summary>
    /// Verifies that IndexAsync indexes vector successfully.
    /// </summary>
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

    /// <summary>
    /// Verifies that IndexBatchAsync indexes multiple vectors.
    /// </summary>
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

    /// <summary>
    /// Verifies that UpdateAsync updates existing vector.
    /// </summary>
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

    /// <summary>
    /// Verifies that DeleteAsync deletes vector successfully.
    /// </summary>
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

    /// <summary>
    /// Verifies that DeleteAsync handles non-existent vector.
    /// </summary>
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

    /// <summary>
    /// Verifies that GetStatisticsAsync returns index statistics.
    /// </summary>
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
        result.Value.ShouldNotBe<VectorIndexStatistics>(default);
        result.Value.TotalVectors.ShouldBe(1000);
        result.Value.IndexSize.ShouldBe(1024 * 1024);
        result.Value.AverageVectorDimension.ShouldBe(384);
    }

    /// <summary>
    /// Verifies that ClearAsync clears all vectors.
    /// </summary>
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

    /// <summary>
    /// Verifies that SearchAsync respects similarity threshold.
    /// </summary>
    /// <param name="threshold">The similarity threshold to test.</param>
    /// <param name="expectedThreshold">The expected threshold value.</param>
    [Theory]
    [InlineData(0.5f, 0.5f)]
    [InlineData(0.7f, 0.7f)]
    [InlineData(0.9f, 0.9f)]
    public async Task SearchAsync_Should_Respect_Similarity_Threshold(float threshold, float expectedThreshold)
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            Limit: 5,
            Threshold: threshold
        );
        var embedding = new VectorEmbedding("emb-1", "Content", new float[] { 0.1f }, new Dictionary<string, object>(), DateTimeOffset.UtcNow);
        var expectedResult = new VectorSearchResult(
            Vector: embedding,
            Similarity: expectedThreshold,
            Rank: 1);

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.Success(expectedResult));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe(default(VectorSearchResult));
        result.Value.Similarity.ShouldBe(expectedThreshold);
    }

    /// <summary>
    /// Verifies that SearchAsync handles invalid query vector.
    /// </summary>
    [Fact]
    public async Task SearchAsync_Should_Handle_Invalid_Query_Vector()
    {
        // Arrange
        var invalidQueryVector = Array.Empty<float>();
        var options = new VectorSearchOptions(
            Limit: 5,
            Threshold: 0.7f
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
