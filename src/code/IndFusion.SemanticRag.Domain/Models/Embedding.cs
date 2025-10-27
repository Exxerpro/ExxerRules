using System;
using System.Collections.Generic;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a vector embedding in the semantic RAG system.
/// </summary>
/// <param name="Id">Unique identifier for the embedding.</param>
/// <param name="DocumentId">ID of the associated document.</param>
/// <param name="Vector">The embedding vector values.</param>
/// <param name="Model">The embedding model used to generate this vector.</param>
/// <param name="Dimensions">Number of dimensions in the vector.</param>
/// <param name="Metadata">Additional metadata associated with the embedding.</param>
/// <param name="CreatedAt">Timestamp when the embedding was created.</param>
public record Embedding(
    string Id,
    string DocumentId,
    IReadOnlyList<float> Vector,
    string Model,
    int Dimensions,
    IReadOnlyDictionary<string, object> Metadata,
    DateTimeOffset CreatedAt)
{
    /// <summary>
    /// Gets the embedding identifier.
    /// </summary>
    public string Id { get; init; } = Id;

    /// <summary>
    /// Gets the associated document identifier.
    /// </summary>
    public string DocumentId { get; init; } = DocumentId;

    /// <summary>
    /// Gets the embedding vector values.
    /// </summary>
    public IReadOnlyList<float> Vector { get; init; } = Vector;

    /// <summary>
    /// Gets the embedding model name.
    /// </summary>
    public string Model { get; init; } = Model;

    /// <summary>
    /// Gets the number of dimensions in the vector.
    /// </summary>
    public int Dimensions { get; init; } = Dimensions;

    /// <summary>
    /// Gets the metadata dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = Metadata;

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = CreatedAt;

    /// <summary>
    /// Validates the embedding for basic requirements.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Embedding ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(DocumentId))
            return Result.WithFailure("Document ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(Model))
            return Result.WithFailure("Embedding model cannot be empty or whitespace");

        if (Vector == null || Vector.Count == 0)
            return Result.WithFailure("Embedding vector cannot be null or empty");

        if (Dimensions <= 0)
            return Result.WithFailure("Embedding dimensions must be greater than zero");

        if (Vector.Count != Dimensions)
            return Result.WithFailure("Vector count must match the specified dimensions");

        return Result.Success();
    }

    /// <summary>
    /// Calculates the cosine similarity between this embedding and another.
    /// </summary>
    /// <param name="other">The other embedding to compare with.</param>
    /// <returns>The cosine similarity value between -1 and 1.</returns>
    public float CalculateCosineSimilarity(Embedding other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (Dimensions != other.Dimensions)
            throw new ArgumentException("Embeddings must have the same dimensions for similarity calculation");

        var dotProduct = 0.0f;
        var magnitudeA = 0.0f;
        var magnitudeB = 0.0f;

        for (var i = 0; i < Vector.Count; i++)
        {
            dotProduct += Vector[i] * other.Vector[i];
            magnitudeA += Vector[i] * Vector[i];
            magnitudeB += other.Vector[i] * other.Vector[i];
        }

        if (magnitudeA == 0.0f || magnitudeB == 0.0f)
            return 0.0f;

        return dotProduct / (float)(Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}
