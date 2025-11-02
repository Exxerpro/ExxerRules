using System.Collections.Generic;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for vector search operations in the Semantic RAG system.
/// This is the application layer interface that coordinates domain services.
/// </summary>
public interface IVectorSearchService : IVectorSearchPort
{
    /// <summary>
    /// Searches for similar documents using a query string.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="options">Search options including filters and limits.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A VectorSearchResponse containing the search results.</returns>
    Task<VectorSearchResponse> SearchSimilarAsync(
        string query,
        VectorSearchOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Stores a document in the vector database for future retrieval.
    /// </summary>
    /// <param name="id">The document ID.</param>
    /// <param name="content">The document content.</param>
    /// <param name="metadata">Additional metadata for the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task StoreDocumentAsync(
        string id,
        string content,
        Dictionary<string, object> metadata,
        CancellationToken cancellationToken = default);
}