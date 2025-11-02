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

    /// <inheritdoc />
    public Task<Result<VectorSearchResult>> SearchAsync(
        float[] queryVector,
        VectorSearchOptions options,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching for similar vectors with query vector of dimension: {Dimension}", queryVector.Length);

            // Convert VectorSearchOptions to VectorSearchQuery for internal use
            var query = new VectorSearchQuery(
                Query: string.Empty, // VectorSearchOptions doesn't have a query string
                Embedding: queryVector,
                Limit: options.Limit,
                Threshold: options.Threshold,
                Filters: options.Filters);

            // Use existing SearchSimilarVectorsAsync logic
            var result = SearchSimilarVectorsAsync(query, cancellationToken).Result;
            if (result.IsFailure)
            {
                return Task.FromResult(Result<VectorSearchResult>.WithFailure(result.Error ?? "Failed to search vectors", default));
            }

            // Return first result (SearchAsync returns single result, not list)
            if (result.Value == null || result.Value.Count == 0)
            {
                _logger.LogWarning("No similar vectors found");
                return Task.FromResult(Result<VectorSearchResult>.WithFailure("No similar vectors found", default));
            }

            var firstResult = result.Value[0];
            return Task.FromResult(Result<VectorSearchResult>.Success(firstResult));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search vectors");
            return Task.FromResult(Result<VectorSearchResult>.WithFailure($"Failed to search vectors: {ex.Message}", default));
        }
    }

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<VectorSearchResult>>> SearchBatchAsync(
        IReadOnlyList<float[]> queryVectors,
        VectorSearchOptions options,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching with {Count} query vectors", queryVectors.Count);

            var allResults = new List<VectorSearchResult>();

            foreach (var queryVector in queryVectors)
            {
                var query = new VectorSearchQuery(
                    Query: string.Empty,
                    Embedding: queryVector,
                    Limit: options.Limit,
                    Threshold: options.Threshold,
                    Filters: options.Filters);

                var result = SearchSimilarVectorsAsync(query, cancellationToken).Result;
                if (result.IsSuccess && result.Value != null)
                {
                    allResults.AddRange(result.Value);
                }
            }

            _logger.LogInformation("Found {Count} total results across {VectorCount} query vectors", allResults.Count, queryVectors.Count);
            return Task.FromResult(Result<IReadOnlyList<VectorSearchResult>>.Success(allResults));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to search vectors in batch");
            return Task.FromResult(Result<IReadOnlyList<VectorSearchResult>>.WithFailure($"Failed to search vectors: {ex.Message}", Array.Empty<VectorSearchResult>()));
        }
    }

    /// <inheritdoc />
    public Task<Result> IndexAsync(VectorEmbedding embedding, CancellationToken cancellationToken = default)
    {
        // Use existing StoreVectorAsync logic
        return StoreVectorAsync(embedding, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result> IndexBatchAsync(IReadOnlyList<VectorEmbedding> embeddings, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Indexing {Count} embeddings in batch", embeddings.Count);

            foreach (var embedding in embeddings)
            {
                var result = StoreVectorAsync(embedding, cancellationToken).Result;
                if (result.IsFailure)
                {
                    _logger.LogError("Failed to index embedding: {EmbeddingId}, Error: {Error}", embedding.Id, result.Error);
                    return Task.FromResult(Result.WithFailure($"Failed to index embedding {embedding.Id}: {result.Error}"));
                }
            }

            _logger.LogInformation("Successfully indexed {Count} embeddings", embeddings.Count);
            return Task.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to index embeddings in batch");
            return Task.FromResult(Result.WithFailure($"Failed to index embeddings: {ex.Message}"));
        }
    }

    /// <inheritdoc />
    public Task<Result> UpdateAsync(VectorEmbedding embedding, CancellationToken cancellationToken = default)
    {
        // Update is same as index (upsert behavior)
        return IndexAsync(embedding, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result> DeleteAsync(string embeddingId, CancellationToken cancellationToken = default)
    {
        // Use existing DeleteVectorAsync logic
        return DeleteVectorAsync(embeddingId, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Result<VectorIndexStatistics>> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting vector index statistics");

            var countResult = GetVectorCountAsync(cancellationToken).Result;
            if (countResult.IsFailure)
            {
                return Task.FromResult(Result<VectorIndexStatistics>.WithFailure(countResult.Error ?? "Failed to get vector count", default));
            }

            var totalVectors = countResult.Value;
            
            // Calculate average vector dimension
            var totalDimension = 0;
            var vectorCount = 0;
            foreach (var vector in _vectors.Values)
            {
                totalDimension += vector.Dimension;
                vectorCount++;
            }

            var averageDimension = vectorCount > 0 ? totalDimension / vectorCount : 0;

            // Calculate index size (approximate - in-memory dictionary size)
            // This is a rough estimate: each vector embedding takes up space
            var indexSize = totalVectors * averageDimension * sizeof(float); // Rough estimate

            var stats = new VectorIndexStatistics(
                TotalVectors: totalVectors,
                IndexSize: indexSize,
                LastUpdated: DateTimeOffset.UtcNow,
                AverageVectorDimension: averageDimension);

            _logger.LogInformation("Vector index statistics: {TotalVectors} vectors, {AverageDimension} avg dimension", totalVectors, averageDimension);
            return Task.FromResult(Result<VectorIndexStatistics>.Success(stats));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get vector index statistics");
            return Task.FromResult(Result<VectorIndexStatistics>.WithFailure($"Failed to get statistics: {ex.Message}", default));
        }
    }

    /// <inheritdoc />
    public Task<Result> ClearAsync(CancellationToken cancellationToken = default)
    {
        // Use existing ClearAllVectorsAsync logic
        return ClearAllVectorsAsync(cancellationToken);
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