namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Result of semantic search operations.
/// </summary>
/// <param name="Id">Unique identifier for the search result.</param>
/// <param name="QueryId">ID of the original query.</param>
/// <param name="Results">Search results.</param>
/// <param name="Document">Primary document for this search result.</param>
/// <param name="TotalCount">Total number of results available.</param>
/// <param name="QueryTime">Time taken to execute the query.</param>
/// <param name="Metadata">Additional search metadata.</param>
public record SemanticSearchResult(
    string Id,
    string QueryId,
    IEnumerable<SearchResultItem> Results,
    SemanticDocument? Document,
    long TotalCount,
    TimeSpan QueryTime,
    Dictionary<string, object> Metadata
);