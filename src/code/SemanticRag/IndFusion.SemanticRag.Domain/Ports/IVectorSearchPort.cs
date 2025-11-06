using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for vector search operations in the Semantic RAG system.
/// This defines the contract for vector similarity search and indexing.
/// </summary>
public interface IVectorSearchPort
{
    /// <summary>
    /// Searches for similar vectors using cosine similarity.
    /// </summary>
    /// <param name="queryVector">Query vector to search with.</param>
    /// <param name="options">Search options including filters and limits.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results.</returns>
    Task<Result<VectorSearchResult>> SearchAsync(
        float[] queryVector,
        VectorSearchOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for similar vectors using multiple query vectors.
    /// </summary>
    /// <param name="queryVectors">Query vectors to search with.</param>
    /// <param name="options">Search options including filters and limits.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results.</returns>
    Task<Result<IReadOnlyList<VectorSearchResult>>> SearchBatchAsync(
        IReadOnlyList<float[]> queryVectors,
        VectorSearchOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indexes a vector embedding for future searches.
    /// </summary>
    /// <param name="embedding">Vector embedding to index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> IndexAsync(
        VectorEmbedding embedding,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Indexes multiple vector embeddings in batch.
    /// </summary>
    /// <param name="embeddings">Vector embeddings to index.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> IndexBatchAsync(
        IReadOnlyList<VectorEmbedding> embeddings,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing vector embedding in the index.
    /// </summary>
    /// <param name="embedding">Updated vector embedding.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> UpdateAsync(
        VectorEmbedding embedding,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a vector embedding from the index.
    /// </summary>
    /// <param name="embeddingId">ID of the embedding to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteAsync(
        string embeddingId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets statistics about the vector index.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing index statistics.</returns>
    Task<Result<VectorIndexStatistics>> GetStatisticsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all vectors from the index.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> ClearAsync(
        CancellationToken cancellationToken = default);
}