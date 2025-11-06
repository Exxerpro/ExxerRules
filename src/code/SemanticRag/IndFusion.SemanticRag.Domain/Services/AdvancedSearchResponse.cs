using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Response from an advanced search operation.
/// </summary>
/// <param name="Results">The search results.</param>
/// <param name="TotalCount">Total number of results found.</param>
/// <param name="PageInfo">Pagination information.</param>
/// <param name="Query">The original query.</param>
/// <param name="ProcessingTimeMs">Time taken to process the search in milliseconds.</param>
public readonly record struct AdvancedSearchResponse(
    IReadOnlyList<SemanticSearchResult> Results,
    int TotalCount,
    PageInfo PageInfo,
    string Query,
    long ProcessingTimeMs);