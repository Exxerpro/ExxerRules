using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.ValueObjects;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for vector storage operations in the semantic RAG system.
/// </summary>
public interface IVectorStorePort
{
    /// <summary>
    /// Stores a document embedding in the vector store.
    /// </summary>
    /// <param name="embedding">The embedding to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> StoreEmbeddingAsync(Embedding embedding, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores multiple document embeddings in the vector store.
    /// </summary>
    /// <param name="embeddings">The embeddings to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> StoreEmbeddingsAsync(IReadOnlyList<Embedding> embeddings, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for similar embeddings using vector similarity.
    /// </summary>
    /// <param name="queryVector">The query vector to search with.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="threshold">Minimum similarity threshold.</param>
    /// <param name="filters">Optional filters to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results.</returns>
    Task<Result<IReadOnlyList<SearchResult>>> SearchSimilarAsync(
        EmbeddingVector queryVector,
        int limit = 10,
        float threshold = 0.0f,
        IReadOnlyDictionary<string, object>? filters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for similar embeddings using text query.
    /// </summary>
    /// <param name="query">The text query to search with.</param>
    /// <param name="limit">Maximum number of results to return.</param>
    /// <param name="threshold">Minimum similarity threshold.</param>
    /// <param name="filters">Optional filters to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results.</returns>
    Task<Result<IReadOnlyList<SearchResult>>> SearchByTextAsync(
        string query,
        int limit = 10,
        float threshold = 0.0f,
        IReadOnlyDictionary<string, object>? filters = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an embedding by its ID.
    /// </summary>
    /// <param name="embeddingId">The embedding ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the embedding or null if not found.</returns>
    Task<Result<Embedding?>> GetEmbeddingAsync(string embeddingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves embeddings by document ID.
    /// </summary>
    /// <param name="documentId">The document ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the embeddings for the document.</returns>
    Task<Result<IReadOnlyList<Embedding>>> GetEmbeddingsByDocumentIdAsync(string documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an embedding by its ID.
    /// </summary>
    /// <param name="embeddingId">The embedding ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteEmbeddingAsync(string embeddingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes all embeddings for a specific document.
    /// </summary>
    /// <param name="documentId">The document ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteEmbeddingsByDocumentIdAsync(string documentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of embeddings in the store.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the count.</returns>
    Task<Result<int>> GetEmbeddingCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all embeddings from the store.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> ClearAllEmbeddingsAsync(CancellationToken cancellationToken = default);
}
