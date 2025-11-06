using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Response from a semantic search operation.
/// </summary>
/// <param name="Results">The search results.</param>
/// <param name="TotalCount">Total number of results found.</param>
/// <param name="Query">The original query.</param>
/// <param name="ProcessingTimeMs">Time taken to process the search in milliseconds.</param>
/// <param name="Context">Additional context about the search.</param>
/// <param name="Suggestions">Search suggestions for refinement.</param>
public readonly record struct SemanticSearchResponse(
    IReadOnlyList<SemanticSearchResult> Results,
    int TotalCount,
    string Query,
    long ProcessingTimeMs,
    SemanticContext? Context,
    IReadOnlyList<string>? Suggestions = null)
{
    /// <summary>
    /// Checks if any results were found.
    /// </summary>
    public bool HasResults => Results.Count > 0;

    /// <summary>
    /// Gets the average relevance score of the results.
    /// </summary>
    public float AverageRelevance => Results.Count > 0 
        ? (float)Results.SelectMany(r => r.Results).Average(item => item.Score) 
        : 0.0f;
}