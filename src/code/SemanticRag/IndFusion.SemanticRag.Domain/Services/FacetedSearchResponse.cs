using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Response from a faceted search operation.
/// </summary>
/// <param name="Results">The search results.</param>
/// <param name="Facets">Available facets and their counts.</param>
/// <param name="TotalCount">Total number of results found.</param>
/// <param name="Query">The original query.</param>
/// <param name="ProcessingTimeMs">Time taken to process the search in milliseconds.</param>
public readonly record struct FacetedSearchResponse(
    IReadOnlyList<SemanticSearchResult> Results,
    IReadOnlyDictionary<string, IReadOnlyList<FacetValue>> Facets,
    int TotalCount,
    string Query,
    long ProcessingTimeMs);