using System;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;
using IndQuestResults.Operations;

namespace IndFusion.SemanticRag.Domain.Builders;

/// <summary>
/// Factory builder for creating <see cref="VectorEmbedding"/> instances with validation.
/// </summary>
public static class VectorEmbeddingBuilder
{
	/// <summary>
	/// Builds a <see cref="VectorEmbedding"/> with validation.
	/// </summary>
	/// <param name="id">Unique identifier for the embedding.</param>
	/// <param name="content">The text content that was embedded.</param>
	/// <param name="embedding">The vector embedding values.</param>
	/// <param name="metadata">Additional metadata associated with the vector.</param>
	/// <param name="createdAt">When the vector was created.</param>
	/// <returns>A Result containing the created VectorEmbedding or validation error.</returns>
	public static Result<VectorEmbedding> Build(
		string id,
		string content,
		float[] embedding,
		System.Collections.Generic.IReadOnlyDictionary<string, object> metadata,
		DateTimeOffset createdAt)
	{
		var validation = ResultExtensions.ValidateNotNull(
			(id, nameof(id)),
			(content, nameof(content)),
			(embedding, nameof(embedding)),
			(metadata, nameof(metadata))
		);

		if (validation.IsFailure)
			return Result<VectorEmbedding>.WithFailure(validation.Error!);

		// Validate string parameters are not empty
		if (string.IsNullOrWhiteSpace(id))
			return Result<VectorEmbedding>.WithFailure(ErrorCodes.VectorIdRequired);

		if (string.IsNullOrWhiteSpace(content))
			return Result<VectorEmbedding>.WithFailure(ErrorCodes.VectorContentRequired);

		// Validate embedding array
		if (embedding.Length == 0)
			return Result<VectorEmbedding>.WithFailure(ErrorCodes.VectorEmbeddingRequired);

		var vectorEmbedding = new VectorEmbedding(
			Id: id,
			Content: content,
			Embedding: embedding,
			Metadata: metadata,
			CreatedAt: createdAt
		);

		return Result<VectorEmbedding>.Success(vectorEmbedding);
	}
}

