namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Defines the contract for knowledge RAG services that provide semantic search and retrieval
/// from documentation, ADRs, and linting rules with provenance tracking.
/// </summary>
public interface IKnowledgeRagService
{
    /// <summary>
    /// Performs semantic search across the knowledge base using natural language queries.
    /// </summary>
    /// <param name="request">The semantic search request containing query and search options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing relevant snippets, confidence scores, and provenance metadata.</returns>
    Task<SemanticSearchResult> SearchKnowledgeAsync(SemanticSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes code patterns and provides improvement suggestions.
    /// </summary>
    /// <param name="request">The code pattern analysis request containing code snippet and analysis options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing patterns found, suggestions, and confidence scores.</returns>
    Task<CodePatternAnalysisResult> AnalyzeCodePatternsAsync(CodePatternAnalysisRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs knowledge base operations for entity management.
    /// </summary>
    /// <param name="request">The knowledge base request containing operation details and entity data.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing operation results and affected entities.</returns>
    Task<KnowledgeBaseResult> PerformKnowledgeBaseOperationAsync(KnowledgeBaseRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs RAG query operations for question answering.
    /// </summary>
    /// <param name="request">The RAG query request containing question and context options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing generated answer, sources, and confidence scores.</returns>
    Task<RagQueryResult> QueryRagAsync(RagQueryRequest request, CancellationToken cancellationToken = default);
}