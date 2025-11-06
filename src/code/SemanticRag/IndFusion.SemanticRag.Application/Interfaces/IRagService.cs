using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for Retrieval-Augmented Generation (RAG) operations.
/// </summary>
public interface IRagService
{
    /// <summary>
    /// Performs a RAG query by retrieving relevant context and generating a response.
    /// </summary>
    /// <param name="query">The user query.</param>
    /// <param name="context">Additional context for the query.</param>
    /// <param name="maxResults">Maximum number of relevant documents to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>RAG response with generated answer and sources.</returns>
    Task<RagResponse> QueryAsync(
        string query,
        string? context = null,
        int maxResults = 5,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a semantic search to find relevant documents.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="maxResults">Maximum number of results.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search results.</returns>
    Task<VectorSearchResponse> SearchRelevantDocumentsAsync(
        string query,
        int maxResults = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a response using the LLM with provided context.
    /// </summary>
    /// <param name="query">The user query.</param>
    /// <param name="context">Context documents to use for generation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated response.</returns>
    Task<string> GenerateResponseAsync(
        string query,
        IReadOnlyList<string> context,
        CancellationToken cancellationToken = default);
}