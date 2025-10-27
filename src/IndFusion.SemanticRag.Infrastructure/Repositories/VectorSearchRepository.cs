using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using Microsoft.Extensions.Logging;
using IndQuestResults;

namespace IndFusion.SemanticRag.Infrastructure.Repositories;

/// <summary>
/// In-memory implementation of the vector search repository for testing and development.
/// </summary>
public class VectorSearchRepository : IVectorSearchPort
{
    private readonly Dictionary<string, VectorEmbedding> _vectors = new();
    private readonly ILogger<VectorSearchRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the VectorSearchRepository class.
    /// </summary>
    /// <param name="logger">The logger for this repository.</param>
    public VectorSearchRepository(ILogger<VectorSearchRepository> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Stores a vector embedding in the repository.
    /// </summary>
    /// <param name="vector">The vector embedding to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Task<Result> StoreVectorAsync(VectorEmbedding vector, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Storing vector with ID: {VectorId}", vector.Id);
            
            _vectors[vector.Id] = vector;
            
            _logger.LogInformation("Successfully stored vector with ID: {VectorId}", vector.Id);
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to store vector with ID: {VectorId}", vector.Id);
            return Task.FromResult(Result.WithFailure($"Failed to store vector: {ex.Message}"));
        }
    }

    /// <summary>
    /// Searches for similar vectors using the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results.</returns>
    public Task<Result<IReadOnlyList<VectorSearchResult>>> SearchSimilarVectorsAsync(
        VectorSearchQuery query, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching for similar vectors with query: {Query}", query.Query);
            
            var results = new List<VectorSearchResult>();
            var rank = 0;
            
            foreach (var vector in _vectors.Values)
            {
                // Simple cosine similarity calculation for demonstration
                var similarity = CalculateCosineSimilarity(query.Embedding, vector.Embedding);
                
                if (similarity >= query.Threshold)
                {
                    results.Add(new VectorSearchResult(vector, similarity, rank++));
                }
            }
            
            // Sort by similarity (descending) and take the limit
            var sortedResults = results
                .OrderByDescending(r => r.Similarity)
                .Take(query.Limit)
                .ToList();
            
            _logger.LogInformation("Found {Count} similar vectors for query: {Query}", 
                sortedResults.Count, query.Query);
            
            return Task.FromResult(Result<IReadOnlyList<VectorSearchResult>>.Success(sortedResults));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search similar vectors for query: {Query}", query.Query);
            return Task.FromResult(Result<IReadOnlyList<VectorSearchResult>>.WithFailure($"Failed to search vectors: {ex.Message}"));
        }
    }

    /// <summary>
    /// Retrieves a vector by its ID.
    /// </summary>
    /// <param name="id">The vector ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the vector or failure.</returns>
    public Task<Result<VectorEmbedding>> GetVectorByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving vector with ID: {VectorId}", id);
            
            if (_vectors.TryGetValue(id, out var vector))
            {
                _logger.LogInformation("Successfully retrieved vector with ID: {VectorId}", id);
                return Task.FromResult(Result<VectorEmbedding>.Success(vector));
            }
            
            _logger.LogWarning("Vector with ID: {VectorId} not found", id);
            return Task.FromResult(Result<VectorEmbedding>.WithFailure($"Vector with ID '{id}' not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve vector with ID: {VectorId}", id);
            return Task.FromResult(Result<VectorEmbedding>.WithFailure($"Failed to retrieve vector: {ex.Message}"));
        }
    }

    /// <summary>
    /// Deletes a vector by its ID.
    /// </summary>
    /// <param name="id">The vector ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Task<Result> DeleteVectorAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting vector with ID: {VectorId}", id);
            
            if (_vectors.Remove(id))
            {
                _logger.LogInformation("Successfully deleted vector with ID: {VectorId}", id);
                return Task.FromResult(Result.Success());
            }
            
            _logger.LogWarning("Vector with ID: {VectorId} not found for deletion", id);
            return Task.FromResult(Result.WithFailure($"Vector with ID '{id}' not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete vector with ID: {VectorId}", id);
            return Task.FromResult(Result.WithFailure($"Failed to delete vector: {ex.Message}"));
        }
    }

    /// <summary>
    /// Gets the total count of vectors in the repository.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the count.</returns>
    public Task<Result<int>> GetVectorCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var count = _vectors.Count;
            _logger.LogInformation("Repository contains {Count} vectors", count);
            return Task.FromResult(Result<int>.Success(count));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get vector count");
            return Task.FromResult(Result<int>.WithFailure($"Failed to get vector count: {ex.Message}"));
        }
    }

    /// <summary>
    /// Clears all vectors from the repository.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public Task<Result> ClearAllVectorsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Clearing all vectors from repository");
            _vectors.Clear();
            _logger.LogInformation("Successfully cleared all vectors from repository");
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear vectors from repository");
            return Task.FromResult(Result.WithFailure($"Failed to clear vectors: {ex.Message}"));
        }
    }

    /// <summary>
    /// Calculates cosine similarity between two vectors.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The cosine similarity score.</returns>
    private static float CalculateCosineSimilarity(float[] vector1, float[] vector2)
    {
        if (vector1.Length != vector2.Length)
            return 0.0f;

        var dotProduct = 0.0f;
        var magnitude1 = 0.0f;
        var magnitude2 = 0.0f;

        for (var i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            magnitude1 += vector1[i] * vector1[i];
            magnitude2 += vector2[i] * vector2[i];
        }

        if (magnitude1 == 0.0f || magnitude2 == 0.0f)
            return 0.0f;

        return dotProduct / (float)(Math.Sqrt(magnitude1) * Math.Sqrt(magnitude2));
    }
}