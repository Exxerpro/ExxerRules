using System;
using System.Collections.Generic;
using System.Linq;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.ValueObjects;

/// <summary>
/// Represents an embedding vector with validation and utility methods.
/// </summary>
/// <param name="Values">The vector values.</param>
public record EmbeddingVector(IReadOnlyList<float> Values)
{
    /// <summary>
    /// Gets the vector values.
    /// </summary>
    public IReadOnlyList<float> Values { get; init; } = Values;

    /// <summary>
    /// Gets the number of dimensions in the vector.
    /// </summary>
    public int Dimensions => Values.Count;

    /// <summary>
    /// Validates the embedding vector.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (Values == null)
            return Result.WithFailure("Vector values cannot be null");

        if (Values.Count == 0)
            return Result.WithFailure("Vector cannot be empty");

        if (Values.Count > 10000)
            return Result.WithFailure("Vector cannot have more than 10,000 dimensions");

        // Check for NaN or Infinity values
        foreach (var value in Values)
        {
            if (float.IsNaN(value))
                return Result.WithFailure("Vector cannot contain NaN values");

            if (float.IsInfinity(value))
                return Result.WithFailure("Vector cannot contain Infinity values");
        }

        return Result.Success();
    }

    /// <summary>
    /// Creates a new EmbeddingVector with validation.
    /// </summary>
    /// <param name="values">The vector values.</param>
    /// <returns>A Result containing the EmbeddingVector or validation error.</returns>
    public static Result<EmbeddingVector> Create(IReadOnlyList<float> values)
    {
        var vector = new EmbeddingVector(values);
        var validation = vector.Validate();
        
        return validation.IsFailure 
            ? Result<EmbeddingVector>.WithFailure(validation.Error!)
            : Result<EmbeddingVector>.Success(vector);
    }

    /// <summary>
    /// Creates a new EmbeddingVector from an array with validation.
    /// </summary>
    /// <param name="values">The vector values array.</param>
    /// <returns>A Result containing the EmbeddingVector or validation error.</returns>
    public static Result<EmbeddingVector> Create(params float[] values)
    {
        return Create(values.ToList());
    }

    /// <summary>
    /// Calculates the magnitude (length) of the vector.
    /// </summary>
    /// <returns>The vector magnitude.</returns>
    public float Magnitude()
    {
        var sum = 0.0f;
        foreach (var value in Values)
        {
            sum += value * value;
        }
        return (float)Math.Sqrt(sum);
    }

    /// <summary>
    /// Normalizes the vector to unit length.
    /// </summary>
    /// <returns>A new normalized EmbeddingVector.</returns>
    public EmbeddingVector Normalize()
    {
        var magnitude = Magnitude();
        if (magnitude == 0.0f)
            return this; // Return original if magnitude is zero

        var normalizedValues = Values.Select(v => v / magnitude).ToArray();
        return new EmbeddingVector(normalizedValues);
    }

    /// <summary>
    /// Calculates the dot product with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The dot product.</returns>
    /// <exception cref="ArgumentException">Thrown when vectors have different dimensions.</exception>
    public float DotProduct(EmbeddingVector other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (Dimensions != other.Dimensions)
            throw new ArgumentException("Vectors must have the same dimensions for dot product calculation");

        var sum = 0.0f;
        for (var i = 0; i < Values.Count; i++)
        {
            sum += Values[i] * other.Values[i];
        }
        return sum;
    }

    /// <summary>
    /// Calculates the cosine similarity with another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The cosine similarity between -1 and 1.</returns>
    /// <exception cref="ArgumentException">Thrown when vectors have different dimensions.</exception>
    public float CosineSimilarity(EmbeddingVector other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (Dimensions != other.Dimensions)
            throw new ArgumentException("Vectors must have the same dimensions for cosine similarity calculation");

        var dotProduct = DotProduct(other);
        var magnitudeA = Magnitude();
        var magnitudeB = other.Magnitude();

        if (magnitudeA == 0.0f || magnitudeB == 0.0f)
            return 0.0f;

        return dotProduct / (magnitudeA * magnitudeB);
    }

    /// <summary>
    /// Calculates the Euclidean distance to another vector.
    /// </summary>
    /// <param name="other">The other vector.</param>
    /// <returns>The Euclidean distance.</returns>
    /// <exception cref="ArgumentException">Thrown when vectors have different dimensions.</exception>
    public float EuclideanDistance(EmbeddingVector other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (Dimensions != other.Dimensions)
            throw new ArgumentException("Vectors must have the same dimensions for distance calculation");

        var sum = 0.0f;
        for (var i = 0; i < Values.Count; i++)
        {
            var diff = Values[i] - other.Values[i];
            sum += diff * diff;
        }
        return (float)Math.Sqrt(sum);
    }

    /// <summary>
    /// Implicit conversion from EmbeddingVector to float array.
    /// </summary>
    /// <param name="vector">The EmbeddingVector to convert.</param>
    /// <returns>The float array.</returns>
    public static implicit operator float[](EmbeddingVector vector) => vector.Values.ToArray();

    /// <summary>
    /// Returns a string representation of the EmbeddingVector.
    /// </summary>
    /// <returns>A string showing the dimensions and first few values.</returns>
    public override string ToString() => $"EmbeddingVector({Dimensions}D): [{string.Join(", ", Values.Take(5))}{(Values.Count > 5 ? "..." : "")}]";
}
