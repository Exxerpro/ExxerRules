namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Advanced search request with complex filtering and sorting.
/// </summary>
/// <param name="Query">The search query.</param>
/// <param name="Filters">Complex filters to apply.</param>
/// <param name="Sorting">How to sort the results.</param>
/// <param name="Pagination">Pagination parameters.</param>
/// <param name="Options">Additional search options.</param>
public readonly record struct AdvancedSearchRequest(
    string Query,
    IReadOnlyList<SearchFilter> Filters,
    IReadOnlyList<SearchSort> Sorting,
    PaginationOptions Pagination,
    SemanticSearchOptions Options);