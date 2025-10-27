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
    /// <param name="request">The RAG request containing query, context, and retrieval preferences.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing relevant snippets, confidence scores, and provenance metadata.</returns>
    Task<KnowledgeRagResult> SearchKnowledgeAsync(KnowledgeRagRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches vector embeddings using semantic similarity with metadata filtering.
    /// </summary>
    /// <param name="request">The vector search request containing query text and search options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing similar vectors, similarity scores, and metadata.</returns>
    Task<VectorSearchResult> SearchVectorsAsync(VectorSearchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries the knowledge graph for relationships, patterns, and entity connections.
    /// </summary>
    /// <param name="request">The graph query request containing query parameters and filters.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing graph traversal results, relationships, and entity data.</returns>
    Task<KnowledgeGraphResult> QueryKnowledgeGraphAsync(KnowledgeGraphRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes documents using OCR and LLM services for entity extraction and relationship mapping.
    /// </summary>
    /// <param name="request">The document processing request containing document input and processing options.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing extracted entities, relationships, and processed content.</returns>
    Task<DocumentProcessingResult> ProcessDocumentAsync(DocumentProcessingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Looks up API surfaces, usage examples, and syntactic changes for referenced packages.
    /// </summary>
    /// <param name="request">The dependency lookup request containing package information and version details.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing API documentation, usage examples, and breaking change information.</returns>
    Task<DependencyApiResult> LookupDependencyApiAsync(DependencyApiRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reports drift or pattern adoption variance across multiple repositories.
    /// </summary>
    /// <param name="request">The cross-repo consistency request containing pattern ID and time window.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing drift analysis, consistency metrics, and remediation recommendations.</returns>
    Task<CrossRepoConsistencyResult> AnalyzeCrossRepoConsistencyAsync(CrossRepoConsistencyRequest request, CancellationToken cancellationToken = default);
}