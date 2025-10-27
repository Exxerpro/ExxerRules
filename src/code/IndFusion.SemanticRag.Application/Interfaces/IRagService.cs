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

/// <summary>
/// Represents a RAG response with generated answer and source documents.
/// </summary>
public record RagResponse
{
    /// <summary>
    /// The generated answer to the query.
    /// </summary>
    public required string Answer { get; init; }

    /// <summary>
    /// Source documents used to generate the answer.
    /// </summary>
    public required IReadOnlyList<RagSource> Sources { get; init; }

    /// <summary>
    /// Confidence score for the answer (0-1).
    /// </summary>
    public required float Confidence { get; init; }

    /// <summary>
    /// Time taken to generate the response in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }

    /// <summary>
    /// The original query.
    /// </summary>
    public required string Query { get; init; }
}

/// <summary>
/// Represents a source document used in RAG generation.
/// </summary>
public record RagSource
{
    /// <summary>
    /// Unique identifier for the source.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Source content.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Relevance score (0-1).
    /// </summary>
    public required float RelevanceScore { get; init; }

    /// <summary>
    /// Source metadata.
    /// </summary>
    public required Dictionary<string, object> Metadata { get; init; }

    /// <summary>
    /// Source type (e.g., "code", "documentation", "pattern").
    /// </summary>
    public required string Type { get; init; }
}



