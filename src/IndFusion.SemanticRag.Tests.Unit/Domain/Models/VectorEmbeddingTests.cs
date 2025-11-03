using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using IndQuestResults;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Models;

/// <summary>
/// Unit tests for VectorEmbedding domain model.
/// </summary>
public class VectorEmbeddingTests
{
    [Fact(Timeout = 5000)]
    public void Should_CreateValidVectorEmbedding_When_AllParametersAreValid()
    {
        // ✅ Use fluent builder from TestDataBuilders
        var vectorResult = TestDataBuilders.CreateValidVectorEmbedding(
            id: "test-id",
            content: "test content",
            embeddingSize: 3);
        vectorResult.IsSuccess.ShouldBeTrue();
        var vector = vectorResult.Value;

        // Assert
        vector.Id.ShouldBe("test-id");
        vector.Content.ShouldBe("test content");
        vector.Embedding.Length.ShouldBe(3);
        vector.Metadata.ShouldNotBeNull();
        vector.Dimension.ShouldBe(3);
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateSuccessfully_When_VectorIsValid()
    {
        // ✅ Use fluent builder from TestDataBuilders
        var vectorResult = TestDataBuilders.CreateValidVectorEmbedding(
            id: "test-id",
            content: "test content",
            embeddingSize: 3);
        vectorResult.IsSuccess.ShouldBeTrue();
        var vector = vectorResult.Value;

        // Act
        var result = vector.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();
    }

    [Theory(Timeout = 5000)]
    [InlineData("", "content")]
    [InlineData("   ", "content")]
    [InlineData(null, "content")]
    public void Should_ValidateFailure_When_IdIsInvalid(string? id, string content)
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
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Theory(Timeout = 5000)]
    [InlineData("id", "")]
    [InlineData("id", "   ")]
    [InlineData("id", null)]
    public void Should_ValidateFailure_When_ContentIsInvalid(string id, string? content)
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
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
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
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
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
        result.Error.ShouldNotBeNullOrEmpty();
    }
}