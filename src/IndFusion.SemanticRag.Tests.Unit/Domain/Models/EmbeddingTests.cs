using IndFusion.SemanticRag.Domain.Models;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Models;

/// <summary>
/// Unit tests for Embedding domain model.
/// </summary>
public class EmbeddingTests
{
    [Fact(Timeout = 5000)]
    public void Should_CreateEmbedding_When_ValidParametersProvided()
    {
        // Arrange
        var id = "emb-123";
        var documentId = "doc-123";
        var vector = new float[] { 0.1f, 0.2f, 0.3f };
        var model = "test-model";
        var dimensions = 3;
        var metadata = new Dictionary<string, object> { ["key"] = "value" };
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var embedding = new Embedding(
            id, documentId, vector, model, dimensions, metadata, createdAt);

        // Assert
        embedding.Id.ShouldBe(id);
        embedding.DocumentId.ShouldBe(documentId);
        embedding.Vector.ShouldBe(vector);
        embedding.Model.ShouldBe(model);
        embedding.Dimensions.ShouldBe(dimensions);
        embedding.Metadata.ShouldBe(metadata);
        embedding.CreatedAt.ShouldBe(createdAt);
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateSuccessfully_When_ValidEmbedding()
    {
        // Arrange
        var embedding = CreateValidEmbedding();

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Theory(Timeout = 5000)]
    [InlineData("", "DocumentId", "Model")]
    [InlineData("Id", "", "Model")]
    [InlineData("Id", "DocumentId", "")]
    [InlineData("   ", "DocumentId", "Model")]
    [InlineData("Id", "   ", "Model")]
    [InlineData("Id", "DocumentId", "   ")]
    public void Should_ValidateFailure_When_RequiredFieldsAreEmptyOrWhitespace(
        string id, string documentId, string model)
    {
        // Arrange
        var embedding = new Embedding(
            id, documentId, new float[] { 0.1f, 0.2f, 0.3f }, model, 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateFailure_When_IdIsNull()
    {
        // Arrange
        var embedding = new Embedding(
            null!, "DocumentId", new float[] { 0.1f, 0.2f, 0.3f }, "Model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateFailure_When_DocumentIdIsNull()
    {
        // Arrange
        var embedding = new Embedding(
            "Id", null!, new float[] { 0.1f, 0.2f, 0.3f }, "Model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateFailure_When_VectorIsNull()
    {
        // Arrange
        var embedding = new Embedding(
            "Id", "DocumentId", null!, "Model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateFailure_When_VectorIsEmpty()
    {
        // Arrange
        var embedding = new Embedding(
            "Id", "DocumentId", new float[0], "Model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateFailure_When_DimensionsIsZero()
    {
        // Arrange
        var embedding = new Embedding(
            "Id", "DocumentId", new float[] { 0.1f, 0.2f, 0.3f }, "Model", 0,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateFailure_When_DimensionsIsNegative()
    {
        // Arrange
        var embedding = new Embedding(
            "Id", "DocumentId", new float[] { 0.1f, 0.2f, 0.3f }, "Model", -1,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void Should_ValidateFailure_When_VectorCountDoesNotMatchDimensions()
    {
        // Arrange
        var embedding = new Embedding(
            "Id", "DocumentId", new float[] { 0.1f, 0.2f, 0.3f }, "Model", 5,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var result = embedding.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public void Should_CalculateCosineSimilarity_When_ValidEmbeddings()
    {
        // Arrange
        var embedding1 = new Embedding(
            "emb1", "doc1", new float[] { 1.0f, 0.0f, 0.0f }, "model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        var embedding2 = new Embedding(
            "emb2", "doc2", new float[] { 0.0f, 1.0f, 0.0f }, "model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var similarity = embedding1.CalculateCosineSimilarity(embedding2);

        // Assert
        similarity.ShouldBe(0.0f); // Perpendicular vectors have 0 similarity
    }

    [Fact(Timeout = 5000)]
    public void Should_CalculateCosineSimilarity_When_IdenticalEmbeddings()
    {
        // Arrange
        var vector = new float[] { 1.0f, 2.0f, 3.0f };
        var embedding1 = new Embedding(
            "emb1", "doc1", vector, "model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        var embedding2 = new Embedding(
            "emb2", "doc2", vector, "model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act
        var similarity = embedding1.CalculateCosineSimilarity(embedding2);

        // Assert
        similarity.ShouldBe(1.0f, 0.001f); // Identical vectors have similarity 1
    }

    [Fact(Timeout = 5000)]
    public void Should_ThrowException_When_CalculateCosineSimilarityWithNullEmbedding()
    {
        // Arrange
        var embedding = CreateValidEmbedding();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => embedding.CalculateCosineSimilarity(null!));
    }

    [Fact(Timeout = 5000)]
    public void Should_ThrowException_When_CalculateCosineSimilarityWithDifferentDimensions()
    {
        // Arrange
        var embedding1 = new Embedding(
            "emb1", "doc1", new float[] { 1.0f, 0.0f }, "model", 2,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        var embedding2 = new Embedding(
            "emb2", "doc2", new float[] { 1.0f, 0.0f, 0.0f }, "model", 3,
            new Dictionary<string, object>(), DateTimeOffset.UtcNow);

        // Act & Assert
        Should.Throw<ArgumentException>(() => embedding1.CalculateCosineSimilarity(embedding2));
    }

    private static Embedding CreateValidEmbedding()
    {
        return new Embedding(
            "emb-123",
            "doc-123",
            new float[] { 0.1f, 0.2f, 0.3f },
            "test-model",
            3,
            new Dictionary<string, object> { ["key"] = "value" },
            DateTimeOffset.UtcNow);
    }
}
