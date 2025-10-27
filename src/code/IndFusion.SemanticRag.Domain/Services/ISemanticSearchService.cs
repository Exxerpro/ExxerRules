using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// High-level semantic search service that provides intelligent search capabilities.
/// </summary>
public interface ISemanticSearchService
{
    /// <summary>
    /// Performs intelligent semantic search with automatic query expansion and context.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="options">Search options and filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results with context.</returns>
    Task<Result<SemanticSearchResponse>> SearchAsync(
        string query,
        SemanticSearchOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs hybrid search combining semantic and keyword search.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="options">Search options and filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the hybrid search results.</returns>
    Task<Result<SemanticSearchResponse>> HybridSearchAsync(
        string query,
        SemanticSearchOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds similar documents to a given document.
    /// </summary>
    /// <param name="documentId">ID of the document to find similarities for.</param>
    /// <param name="limit">Maximum number of similar documents to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing similar documents.</returns>
    Task<Result<IReadOnlyList<SemanticSearchResult>>> FindSimilarDocumentsAsync(
        string documentId,
        int limit = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs faceted search with category filters.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="facets">Facet filters to apply.</param>
    /// <param name="options">Search options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing faceted search results.</returns>
    Task<Result<FacetedSearchResponse>> FacetedSearchAsync(
        string query,
        IReadOnlyList<SearchFacet> facets,
        SemanticSearchOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets search suggestions based on partial query.
    /// </summary>
    /// <param name="partialQuery">Partial query text.</param>
    /// <param name="limit">Maximum number of suggestions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing search suggestions.</returns>
    Task<Result<IReadOnlyList<string>>> GetSuggestionsAsync(
        string partialQuery,
        int limit = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs advanced search with complex filters and sorting.
    /// </summary>
    /// <param name="request">Advanced search request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the advanced search results.</returns>
    Task<Result<AdvancedSearchResponse>> AdvancedSearchAsync(
        AdvancedSearchRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Options for semantic search operations.
/// </summary>
/// <param name="MaxResults">Maximum number of results to return.</param>
/// <param name="SimilarityThreshold">Minimum similarity threshold for results.</param>
/// <param name="IncludeHighlights">Whether to include text highlights.</param>
/// <param name="IncludeMetadata">Whether to include document metadata.</param>
/// <param name="EnableQueryExpansion">Whether to enable automatic query expansion.</param>
/// <param name="EnableContextRetrieval">Whether to enable context retrieval.</param>
/// <param name="SortBy">How to sort the results.</param>
/// <param name="Filters">Additional filters to apply.</param>
public readonly record struct SemanticSearchOptions(
    int MaxResults = 10,
    float SimilarityThreshold = 0.7f,
    bool IncludeHighlights = true,
    bool IncludeMetadata = true,
    bool EnableQueryExpansion = true,
    bool EnableContextRetrieval = true,
    SearchSortBy SortBy = SearchSortBy.Relevance,
    IReadOnlyDictionary<string, object>? Filters = null)
{
    /// <summary>
    /// Validates the search options.
    /// </summary>
    public Result Validate()
    {
        if (MaxResults <= 0)
            return Result.WithFailure("MaxResults must be greater than 0");

        if (SimilarityThreshold < 0.0f || SimilarityThreshold > 1.0f)
            return Result.WithFailure("SimilarityThreshold must be between 0.0 and 1.0");

        return Result.Success();
    }
}

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
        ? Results.Average(r => r.Score) 
        : 0.0f;
}

/// <summary>
/// Represents a search facet for filtering.
/// </summary>
/// <param name="Name">Name of the facet (e.g., "type", "language", "source").</param>
/// <param name="Values">Values to filter by.</param>
/// <param name="Operator">How to combine multiple values.</param>
public readonly record struct SearchFacet(
    string Name,
    IReadOnlyList<string> Values,
    FacetOperator Operator = FacetOperator.Or)
{
    /// <summary>
    /// Validates the search facet.
    /// </summary>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            return Result.WithFailure("Facet name cannot be null or empty");

        if (Values.Count == 0)
            return Result.WithFailure("At least one facet value must be specified");

        return Result.Success();
    }
}

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

/// <summary>
/// Represents a facet value with its count.
/// </summary>
/// <param name="Value">The facet value.</param>
/// <param name="Count">Number of documents with this value.</param>
public readonly record struct FacetValue(string Value, int Count);

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

/// <summary>
/// Represents a search filter.
/// </summary>
/// <param name="Field">Field to filter on.</param>
/// <param name="Operator">Filter operator.</param>
/// <param name="Value">Value to filter by.</param>
public readonly record struct SearchFilter(string Field, FilterOperator Operator, object Value);

/// <summary>
/// Represents a search sort option.
/// </summary>
/// <param name="Field">Field to sort by.</param>
/// <param name="Direction">Sort direction.</param>
public readonly record struct SearchSort(string Field, SortDirection Direction);

/// <summary>
/// Pagination options for search results.
/// </summary>
/// <param name="Page">Page number (1-based).</param>
/// <param name="PageSize">Number of results per page.</param>
public readonly record struct PaginationOptions(int Page = 1, int PageSize = 10);

/// <summary>
/// Information about pagination.
/// </summary>
/// <param name="CurrentPage">Current page number.</param>
/// <param name="PageSize">Results per page.</param>
/// <param name="TotalPages">Total number of pages.</param>
/// <param name="TotalCount">Total number of results.</param>
public readonly record struct PageInfo(int CurrentPage, int PageSize, int TotalPages, int TotalCount);

/// <summary>
/// How to sort search results.
/// </summary>
public enum SearchSortBy
{
    /// <summary>
    /// Sort by relevance score.
    /// </summary>
    Relevance,

    /// <summary>
    /// Sort by creation date (newest first).
    /// </summary>
    DateCreated,

    /// <summary>
    /// Sort by last modified date (newest first).
    /// </summary>
    DateModified,

    /// <summary>
    /// Sort by document title.
    /// </summary>
    Title
}

/// <summary>
/// How to combine facet values.
/// </summary>
public enum FacetOperator
{
    /// <summary>
    /// OR operation - match any of the values.
    /// </summary>
    Or,

    /// <summary>
    /// AND operation - match all of the values.
    /// </summary>
    And
}

/// <summary>
/// Filter operators for advanced search.
/// </summary>
public enum FilterOperator
{
    /// <summary>
    /// Equals.
    /// </summary>
    Equals,

    /// <summary>
    /// Not equals.
    /// </summary>
    NotEquals,

    /// <summary>
    /// Contains.
    /// </summary>
    Contains,

    /// <summary>
    /// Greater than.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Less than.
    /// </summary>
    LessThan,

    /// <summary>
    /// In list.
    /// </summary>
    In,

    /// <summary>
    /// Not in list.
    /// </summary>
    NotIn
}

/// <summary>
/// Sort direction.
/// </summary>
public enum SortDirection
{
    /// <summary>
    /// Ascending order.
    /// </summary>
    Ascending,

    /// <summary>
    /// Descending order.
    /// </summary>
    Descending
}