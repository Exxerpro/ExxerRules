using IndFusion.SemanticRag.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.ValueObjects;

/// <summary>
/// Unit tests for EmbeddingVector value object.
/// </summary>
public class EmbeddingVectorTests
{
    [Fact]
    public void Should_CreateEmbeddingVector_When_ValidValuesProvided()
    {
        // Arrange
        var values = new float[] { 0.1f, 0.2f, 0.3f };

        // Act
        var vector = new EmbeddingVector(values);

        // Assert
        vector.Values.ShouldBe(values);
        vector.Dimensions.ShouldBe(3);
    }

    [Fact]
    public void Should_ValidateSuccessfully_When_ValidVector()
    {
        // Arrange
        var vector = CreateValidVector();

        // Act
        var result = vector.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Should_ValidateFailure_When_ValuesIsNull()
    {
        // Arrange
        var vector = new EmbeddingVector(null!);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Vector values cannot be null");
    }

    [Fact]
    public void Should_ValidateFailure_When_ValuesIsEmpty()
    {
        // Arrange
        var vector = new EmbeddingVector(new float[0]);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Vector cannot be empty");
    }

    [Fact]
    public void Should_ValidateFailure_When_ValuesExceedsMaxDimensions()
    {
        // Arrange
        var values = new float[10001]; // 10,001 dimensions
        var vector = new EmbeddingVector(values);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Vector cannot have more than 10,000 dimensions");
    }

    [Fact]
    public void Should_ValidateFailure_When_ValuesContainsNaN()
    {
        // Arrange
        var values = new float[] { 0.1f, float.NaN, 0.3f };
        var vector = new EmbeddingVector(values);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Vector cannot contain NaN values");
    }

    [Fact]
    public void Should_ValidateFailure_When_ValuesContainsInfinity()
    {
        // Arrange
        var values = new float[] { 0.1f, float.PositiveInfinity, 0.3f };
        var vector = new EmbeddingVector(values);

        // Act
        var result = vector.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Vector cannot contain Infinity values");
    }

    [Fact]
    public void Should_CreateSuccessfully_When_ValidValuesProvided()
    {
        // Arrange
        var values = new float[] { 0.1f, 0.2f, 0.3f };

        // Act
        var result = EmbeddingVector.Create(values);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Values.ShouldBe(values);
    }

    [Fact]
    public void Should_CreateSuccessfully_When_ValidArrayProvided()
    {
        // Arrange
        var values = new float[] { 0.1f, 0.2f, 0.3f };

        // Act
        var result = EmbeddingVector.Create(values);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Values.ShouldBe(values);
    }

    [Fact]
    public void Should_CreateFailure_When_InvalidValuesProvided()
    {
        // Arrange
        var values = new float[0];

        // Act
        var result = EmbeddingVector.Create(values);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Should_CalculateMagnitude_When_ValidVector()
    {
        // Arrange
        var vector = new EmbeddingVector(new float[] { 3.0f, 4.0f });

        // Act
        var magnitude = vector.Magnitude();

        // Assert
        magnitude.ShouldBe(5.0f, 0.001f); // 3^2 + 4^2 = 25, sqrt(25) = 5
    }

    [Fact]
    public void Should_CalculateMagnitude_When_ZeroVector()
    {
        // Arrange
        var vector = new EmbeddingVector(new float[] { 0.0f, 0.0f, 0.0f });

        // Act
        var magnitude = vector.Magnitude();

        // Assert
        magnitude.ShouldBe(0.0f);
    }

    [Fact]
    public void Should_NormalizeVector_When_ValidVector()
    {
        // Arrange
        var vector = new EmbeddingVector(new float[] { 3.0f, 4.0f });

        // Act
        var normalized = vector.Normalize();

        // Assert
        normalized.Magnitude().ShouldBe(1.0f, 0.001f);
        normalized.Values[0].ShouldBe(0.6f, 0.001f); // 3/5
        normalized.Values[1].ShouldBe(0.8f, 0.001f); // 4/5
    }

    [Fact]
    public void Should_ReturnOriginalVector_When_NormalizingZeroVector()
    {
        // Arrange
        var vector = new EmbeddingVector(new float[] { 0.0f, 0.0f });

        // Act
        var normalized = vector.Normalize();

        // Assert
        normalized.ShouldBe(vector);
    }

    [Fact]
    public void Should_CalculateDotProduct_When_ValidVectors()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });
        var vector2 = new EmbeddingVector(new float[] { 4.0f, 5.0f, 6.0f });

        // Act
        var dotProduct = vector1.DotProduct(vector2);

        // Assert
        dotProduct.ShouldBe(32.0f); // 1*4 + 2*5 + 3*6 = 4 + 10 + 18 = 32
    }

    [Fact]
    public void Should_ThrowException_When_DotProductWithNullVector()
    {
        // Arrange
        var vector = CreateValidVector();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => vector.DotProduct(null!));
    }

    [Fact]
    public void Should_ThrowException_When_DotProductWithDifferentDimensions()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 1.0f, 2.0f });
        var vector2 = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });

        // Act & Assert
        Should.Throw<ArgumentException>(() => vector1.DotProduct(vector2));
    }

    [Fact]
    public void Should_CalculateCosineSimilarity_When_ValidVectors()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 1.0f, 0.0f });
        var vector2 = new EmbeddingVector(new float[] { 0.0f, 1.0f });

        // Act
        var similarity = vector1.CosineSimilarity(vector2);

        // Assert
        similarity.ShouldBe(0.0f, 0.001f); // Perpendicular vectors have 0 similarity
    }

    [Fact]
    public void Should_CalculateCosineSimilarity_When_IdenticalVectors()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });
        var vector2 = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });

        // Act
        var similarity = vector1.CosineSimilarity(vector2);

        // Assert
        similarity.ShouldBe(1.0f, 0.001f); // Identical vectors have similarity 1
    }

    [Fact]
    public void Should_CalculateCosineSimilarity_When_ZeroVectors()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 0.0f, 0.0f });
        var vector2 = new EmbeddingVector(new float[] { 1.0f, 2.0f });

        // Act
        var similarity = vector1.CosineSimilarity(vector2);

        // Assert
        similarity.ShouldBe(0.0f); // Zero vector has 0 similarity with any vector
    }

    [Fact]
    public void Should_ThrowException_When_CosineSimilarityWithNullVector()
    {
        // Arrange
        var vector = CreateValidVector();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => vector.CosineSimilarity(null!));
    }

    [Fact]
    public void Should_ThrowException_When_CosineSimilarityWithDifferentDimensions()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 1.0f, 2.0f });
        var vector2 = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });

        // Act & Assert
        Should.Throw<ArgumentException>(() => vector1.CosineSimilarity(vector2));
    }

    [Fact]
    public void Should_CalculateEuclideanDistance_When_ValidVectors()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 0.0f, 0.0f });
        var vector2 = new EmbeddingVector(new float[] { 3.0f, 4.0f });

        // Act
        var distance = vector1.EuclideanDistance(vector2);

        // Assert
        distance.ShouldBe(5.0f, 0.001f); // sqrt((3-0)^2 + (4-0)^2) = sqrt(9+16) = 5
    }

    [Fact]
    public void Should_CalculateEuclideanDistance_When_IdenticalVectors()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });
        var vector2 = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });

        // Act
        var distance = vector1.EuclideanDistance(vector2);

        // Assert
        distance.ShouldBe(0.0f, 0.001f); // Identical vectors have distance 0
    }

    [Fact]
    public void Should_ThrowException_When_EuclideanDistanceWithNullVector()
    {
        // Arrange
        var vector = CreateValidVector();

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => vector.EuclideanDistance(null!));
    }

    [Fact]
    public void Should_ThrowException_When_EuclideanDistanceWithDifferentDimensions()
    {
        // Arrange
        var vector1 = new EmbeddingVector(new float[] { 1.0f, 2.0f });
        var vector2 = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });

        // Act & Assert
        Should.Throw<ArgumentException>(() => vector1.EuclideanDistance(vector2));
    }

    [Fact]
    public void Should_ImplicitlyConvertToFloatArray()
    {
        // Arrange
        var vector = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f });

        // Act
        float[] array = vector;

        // Assert
        array.ShouldBe(new float[] { 1.0f, 2.0f, 3.0f });
    }

    [Fact]
    public void Should_ReturnStringRepresentation_When_ToStringCalled()
    {
        // Arrange
        var vector = new EmbeddingVector(new float[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f, 6.0f });

        // Act
        var result = vector.ToString();

        // Assert
        result.ShouldContain("EmbeddingVector(6D)");
        result.ShouldContain("1, 2, 3, 4, 5");
        result.ShouldContain("...");
    }

    [Fact]
    public void Should_ReturnStringRepresentation_When_ShortVector()
    {
        // Arrange
        var vector = new EmbeddingVector(new float[] { 1.0f, 2.0f });

        // Act
        var result = vector.ToString();

        // Assert
        result.ShouldContain("EmbeddingVector(2D)");
        result.ShouldContain("1, 2");
        result.ShouldNotContain("...");
    }

    private static EmbeddingVector CreateValidVector()
    {
        return new EmbeddingVector(new float[] { 0.1f, 0.2f, 0.3f });
    }
}
