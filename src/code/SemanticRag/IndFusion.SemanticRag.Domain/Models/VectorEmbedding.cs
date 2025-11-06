using IndQuestResults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a vector embedding with metadata.
/// </summary>
/// <param name="Id">Unique identifier for the embedding.</param>
/// <param name="Content">The text content that was embedded.</param>
/// <param name="Embedding">The embedding vector.</param>
/// <param name="Metadata">Additional metadata about the embedding.</param>
/// <param name="CreatedAt">When the embedding was created.</param>
public readonly record struct VectorEmbedding(
    string Id,
    string Content,
    float[] Embedding,
    IReadOnlyDictionary<string, object> Metadata,
    DateTimeOffset CreatedAt)
{
    /// <summary>
    /// Gets the dimension of the embedding vector.
    /// </summary>
    public int Dimension => Embedding.Length;

    /// <summary>
    /// Validates the vector embedding.
    /// </summary>
    /// <returns>A Result indicating whether the embedding is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Embedding ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Content))
            return Result.WithFailure("Embedding content cannot be null or empty");

        if (Embedding.Length == 0)
            return Result.WithFailure("Embedding vector cannot be empty");

        if (Metadata is null)
            return Result.WithFailure("Embedding metadata cannot be null");

        return Result.Success();
    }
}
