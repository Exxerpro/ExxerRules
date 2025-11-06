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
