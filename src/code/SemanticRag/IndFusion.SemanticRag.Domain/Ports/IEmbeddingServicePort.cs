using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for embedding service operations in the semantic RAG system.
/// </summary>
public interface IEmbeddingServicePort
{
    /// <summary>
    /// Generates embeddings for the given text content.
    /// </summary>
    /// <param name="text">The text content to embed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the embedding vector.</returns>
    Task<Result<float[]>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for multiple text contents in batch.
    /// </summary>
    /// <param name="texts">The text contents to embed.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the embedding vectors.</returns>
    Task<Result<IReadOnlyList<float[]>>> GenerateEmbeddingsAsync(
        IReadOnlyList<string> texts, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for the given text content with metadata.
    /// </summary>
    /// <param name="text">The text content to embed.</param>
    /// <param name="metadata">Additional metadata for the embedding.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the vector embedding with metadata.</returns>
    Task<Result<VectorEmbedding>> GenerateEmbeddingWithMetadataAsync(
        string text, 
        Dictionary<string, object> metadata, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the dimension of embeddings produced by this service.
    /// </summary>
    /// <returns>The embedding dimension.</returns>
    int GetEmbeddingDimension();

    /// <summary>
    /// Gets the maximum text length supported by this service.
    /// </summary>
    /// <returns>The maximum text length in characters.</returns>
    int GetMaxTextLength();

    /// <summary>
    /// Validates if the given text is within the supported length limits.
    /// </summary>
    /// <param name="text">The text to validate.</param>
    /// <returns>A Result indicating whether the text is valid.</returns>
    Result ValidateTextLength(string text);

    /// <summary>
    /// Gets the model information for this embedding service.
    /// </summary>
    /// <returns>A Result containing the model information.</returns>
    Task<Result<EmbeddingModelInfo>> GetModelInfoAsync(CancellationToken cancellationToken = default);
}