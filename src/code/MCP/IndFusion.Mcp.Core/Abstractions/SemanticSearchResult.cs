namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of semantic search operations.
/// </summary>
/// <param name="Success">Whether the search succeeded.</param>
/// <param name="Results">Search results.</param>
/// <param name="TotalMatches">Total number of matches found.</param>
/// <param name="SearchTimeMs">Time taken for the search.</param>
/// <param name="QueryProcessed">The processed query used for search.</param>
/// <param name="ErrorDetails">Error details if search failed.</param>
public record SemanticSearchResult(
    bool Success,
    IEnumerable<SearchResult> Results,
    int TotalMatches,
    long SearchTimeMs,
    string QueryProcessed,
    string? ErrorDetails = null
);