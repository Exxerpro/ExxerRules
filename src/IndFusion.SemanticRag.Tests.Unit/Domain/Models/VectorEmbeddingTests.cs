using IndFusion.SemanticRag.Domain.Models;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Models;

/// <summary>
/// Unit tests for VectorEmbedding domain model.
/// </summary>
public class VectorEmbeddingTests
{
    [Fact]
    public void Should_CreateValidVectorEmbedding_When_AllParametersAreValid()
    {
        // Arrange
        var id = "test-id";
        var content = "test content";
        var embedding = new float[] { 0.1f, 0.2f, 0.3f };
        var metadata = new Dictionary<string, object> { ["type"] = "text" };
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var vector = new VectorEmbedding(id, content, embedding, metadata, createdAt);

        // Assert
        vector.Id.ShouldBe(id);
        vector.Content.ShouldBe(content);
        vector.Embedding.ShouldBe(embedding);
        vector.Metadata.ShouldBe(metadata);
        vector.CreatedAt.ShouldBe(createdAt);
        vector.Dimension.ShouldBe(3);
    }

    [Fact]
    public void Should_ValidateSuccessfully_When_VectorIsValid()
    {
        // Arrange
        var vector = new VectorEmbedding(
            "test-id",
            "test content",
            new float[] { 0.1f, 0.2f, 0.3f },
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Theory]
    [InlineData("", "content", "Vector ID cannot be null or empty")]
    [InlineData("   ", "content", "Vector ID cannot be null or empty")]
    [InlineData(null, "content", "Vector ID cannot be null or empty")]
    public void Should_ValidateFailure_When_IdIsInvalid(string? id, string content, string expectedError)
    {
        // Arrange
        var vector = new VectorEmbedding(
            id!,
            content,
            new float[] { 0.1f, 0.2f, 0.3f },
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    [Theory]
    [InlineData("id", "", "Vector content cannot be null or empty")]
    [InlineData("id", "   ", "Vector content cannot be null or empty")]
    [InlineData("id", null, "Vector content cannot be null or empty")]
    public void Should_ValidateFailure_When_ContentIsInvalid(string id, string? content, string expectedError)
    {
        // Arrange
        var vector = new VectorEmbedding(
            id,
            content!,
            new float[] { 0.1f, 0.2f, 0.3f },
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    [Fact]
    public void Should_ValidateFailure_When_EmbeddingIsEmpty()
    {
        // Arrange
        var vector = new VectorEmbedding(
            "test-id",
            "test content",
            Array.Empty<float>(),
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Vector embedding cannot be empty");
    }

    [Fact]
    public void Should_ValidateFailure_When_MetadataIsNull()
    {
        // Arrange
        var vector = new VectorEmbedding(
            "test-id",
            "test content",
            new float[] { 0.1f, 0.2f, 0.3f },
            null!,
            DateTimeOffset.UtcNow);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Vector metadata cannot be null");
    }
}