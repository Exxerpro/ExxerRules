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

/// <summary>
/// Represents a search query for vector similarity search.
/// </summary>
/// <param name="Query">The search query text.</param>
/// <param name="Embedding">The query embedding vector.</param>
/// <param name="Limit">Maximum number of results to return.</param>
/// <param name="Threshold">Minimum similarity threshold (0.0 to 1.0).</param>
/// <param name="Filters">Optional metadata filters.</param>
public readonly record struct VectorSearchQuery(
    string Query,
    float[] Embedding,
    int Limit = 10,
    float Threshold = 0.7f,
    IReadOnlyDictionary<string, object>? Filters = null)
{
    /// <summary>
    /// Validates the search query.
    /// </summary>
    /// <returns>A Result indicating whether the query is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Search query cannot be null or empty");

        if (Embedding.Length == 0)
            return Result.WithFailure("Query embedding cannot be empty");

        if (Limit <= 0)
            return Result.WithFailure("Search limit must be greater than 0");

        if (Threshold < 0.0f || Threshold > 1.0f)
            return Result.WithFailure("Similarity threshold must be between 0.0 and 1.0");

        return Result.Success();
    }
}

/// <summary>
/// Represents a search result from vector similarity search.
/// </summary>
/// <param name="Vector">The matching vector embedding.</param>
/// <param name="Similarity">The similarity score (0.0 to 1.0).</param>
/// <param name="Rank">The rank of this result in the search results.</param>
public readonly record struct VectorSearchResult(
    VectorEmbedding Vector,
    float Similarity,
    int Rank)
{
    /// <summary>
    /// Checks if this result meets the minimum threshold.
    /// </summary>
    /// <param name="threshold">The minimum similarity threshold.</param>
    /// <returns>True if the similarity meets the threshold.</returns>
    public bool MeetsThreshold(float threshold) => Similarity >= threshold;
}

/// <summary>
/// Represents options for vector search operations.
/// </summary>
/// <param name="Limit">Maximum number of results to return.</param>
/// <param name="Threshold">Minimum similarity threshold (0.0 to 1.0).</param>
/// <param name="IncludeMetadata">Whether to include metadata in results.</param>
/// <param name="IncludeEmbedding">Whether to include the embedding vector in results.</param>
/// <param name="Filters">Optional metadata filters to apply.</param>
/// <param name="TimeoutMs">Search timeout in milliseconds.</param>
public readonly record struct VectorSearchOptions(
    int Limit = 10,
    float Threshold = 0.7f,
    bool IncludeMetadata = true,
    bool IncludeEmbedding = false,
    IReadOnlyDictionary<string, object>? Filters = null,
    int TimeoutMs = 5000)
{
    /// <summary>
    /// Validates the vector search options.
    /// </summary>
    /// <returns>A Result indicating whether the options are valid.</returns>
    public Result Validate()
    {
        if (Limit <= 0)
            return Result.WithFailure("Limit must be greater than 0");

        if (Threshold < 0.0f || Threshold > 1.0f)
            return Result.WithFailure("Threshold must be between 0.0 and 1.0");

        if (TimeoutMs <= 0)
            return Result.WithFailure("Timeout must be greater than 0");

        return Result.Success();
    }

    /// <summary>
    /// Default options for general vector search.
    /// </summary>
    public static VectorSearchOptions Default() => new(
        Limit: 10,
        Threshold: 0.7f,
        IncludeMetadata: true,
        IncludeEmbedding: false,
        Filters: null,
        TimeoutMs: 5000);

    /// <summary>
    /// Options for high-precision search.
    /// </summary>
    public static VectorSearchOptions HighPrecision() => new(
        Limit: 5,
        Threshold: 0.9f,
        IncludeMetadata: true,
        IncludeEmbedding: false,
        Filters: null,
        TimeoutMs: 10000);

    /// <summary>
    /// Options for broad search with many results.
    /// </summary>
    public static VectorSearchOptions Broad() => new(
        Limit: 50,
        Threshold: 0.5f,
        IncludeMetadata: true,
        IncludeEmbedding: false,
        Filters: null,
        TimeoutMs: 3000);
}

/// <summary>
/// Represents a response from a vector search operation.
/// </summary>
/// <param name="Results">The search results.</param>
/// <param name="TotalCount">Total number of results found.</param>
/// <param name="Query">The original search query.</param>
/// <param name="ProcessingTimeMs">Time taken to process the search in milliseconds.</param>
/// <param name="SearchOptions">The options used for the search.</param>
/// <param name="Success">Whether the search was successful.</param>
/// <param name="ErrorMessage">Error message if the search failed.</param>
public readonly record struct VectorSearchResponse(
    IReadOnlyList<VectorSearchResult> Results,
    int TotalCount,
    string Query,
    long ProcessingTimeMs,
    VectorSearchOptions SearchOptions,
    bool Success = true,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Checks if the search was successful.
    /// </summary>
    public bool IsSuccess => Success && ErrorMessage is null;

    /// <summary>
    /// Checks if any results were found.
    /// </summary>
    public bool HasResults => Results.Count > 0;

    /// <summary>
    /// Gets the average similarity score of the results.
    /// </summary>
    public float AverageSimilarity => Results.Count > 0 
        ? Results.Average(r => r.Similarity) 
        : 0.0f;

    /// <summary>
    /// Gets the highest similarity score among the results.
    /// </summary>
    public float MaxSimilarity => Results.Count > 0 
        ? Results.Max(r => r.Similarity) 
        : 0.0f;

    /// <summary>
    /// Gets the lowest similarity score among the results.
    /// </summary>
    public float MinSimilarity => Results.Count > 0 
        ? Results.Min(r => r.Similarity) 
        : 0.0f;

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    /// <param name="results">The search results.</param>
    /// <param name="totalCount">Total number of results found.</param>
    /// <param name="query">The original search query.</param>
    /// <param name="processingTimeMs">Time taken to process the search.</param>
    /// <param name="searchOptions">The options used for the search.</param>
    /// <returns>A successful vector search response.</returns>
    public static VectorSearchResponse CreateSuccess(
        IReadOnlyList<VectorSearchResult> results,
        int totalCount,
        string query,
        long processingTimeMs,
        VectorSearchOptions searchOptions) => new(
            Results: results,
            TotalCount: totalCount,
            Query: query,
            ProcessingTimeMs: processingTimeMs,
            SearchOptions: searchOptions,
            Success: true,
            ErrorMessage: null);

    /// <summary>
    /// Creates a failed response.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    /// <param name="query">The original search query.</param>
    /// <param name="searchOptions">The options used for the search.</param>
    /// <returns>A failed vector search response.</returns>
    public static VectorSearchResponse CreateFailure(
        string errorMessage,
        string query,
        VectorSearchOptions searchOptions) => new(
            Results: Array.Empty<VectorSearchResult>(),
            TotalCount: 0,
            Query: query,
            ProcessingTimeMs: 0,
            SearchOptions: searchOptions,
            Success: false,
            ErrorMessage: errorMessage);
}