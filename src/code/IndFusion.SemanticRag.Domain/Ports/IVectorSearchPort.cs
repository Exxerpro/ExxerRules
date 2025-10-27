using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port interface for vector search operations in the hexagonal architecture.
/// </summary>
public interface IVectorSearchPort
{
    /// <summary>
    /// Stores a vector embedding in the vector database.
    /// </summary>
    /// <param name="vector">The vector embedding to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> StoreVectorAsync(VectorEmbedding vector, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for similar vectors using the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results.</returns>
    Task<Result<IReadOnlyList<VectorSearchResult>>> SearchSimilarVectorsAsync(
        VectorSearchQuery query, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a vector by its ID.
    /// </summary>
    /// <param name="id">The vector ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the vector or failure.</returns>
    Task<Result<VectorEmbedding>> GetVectorByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a vector by its ID.
    /// </summary>
    /// <param name="id">The vector ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> DeleteVectorAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of vectors in the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the count.</returns>
    Task<Result<int>> GetVectorCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all vectors from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> ClearAllVectorsAsync(CancellationToken cancellationToken = default);
}