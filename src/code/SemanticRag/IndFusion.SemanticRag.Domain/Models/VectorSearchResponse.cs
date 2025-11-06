namespace IndFusion.SemanticRag.Domain.Models;

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