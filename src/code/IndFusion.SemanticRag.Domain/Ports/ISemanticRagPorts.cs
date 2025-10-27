namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for vector storage operations in the Semantic RAG system.
/// This defines the contract for storing and retrieving vector embeddings with metadata.
/// </summary>
public interface IVectorStorePort
{
    /// <summary>
    /// Stores vector embeddings with associated metadata.
    /// </summary>
    /// <param name="vectors">Collection of vectors to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<VectorStoreResult> StoreVectorsAsync(IEnumerable<VectorEmbedding> vectors, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for similar vectors using semantic similarity.
    /// </summary>
    /// <param name="query">Query vector for similarity search.</param>
    /// <param name="options">Search options including filters and limits.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of similar vectors with similarity scores.</returns>
    Task<IEnumerable<VectorSearchResult>> SearchSimilarVectorsAsync(VectorEmbedding query, VectorSearchOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes vectors by their identifiers.
    /// </summary>
    /// <param name="vectorIds">Collection of vector IDs to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<VectorStoreResult> DeleteVectorsAsync(IEnumerable<string> vectorIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates vector metadata without changing the embedding.
    /// </summary>
    /// <param name="vectorId">ID of the vector to update.</param>
    /// <param name="metadata">New metadata to associate with the vector.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<VectorStoreResult> UpdateVectorMetadataAsync(string vectorId, Dictionary<string, object> metadata, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets vector statistics for monitoring and analytics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Vector store statistics.</returns>
    Task<VectorStoreStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Port for knowledge graph operations in the Semantic RAG system.
/// This defines the contract for storing and querying graph relationships and patterns.
/// </summary>
public interface IKnowledgeGraphPort
{
    /// <summary>
    /// Creates or updates nodes in the knowledge graph.
    /// </summary>
    /// <param name="nodes">Collection of nodes to create or update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<KnowledgeGraphResult> UpsertNodesAsync(IEnumerable<GraphNode> nodes, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates or updates relationships between nodes.
    /// </summary>
    /// <param name="relationships">Collection of relationships to create or update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result indicating success or failure.</returns>
    Task<KnowledgeGraphResult> UpsertRelationshipsAsync(IEnumerable<GraphRelationship> relationships, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries the knowledge graph using Cypher-like syntax.
    /// </summary>
    /// <param name="query">Graph query to execute.</param>
    /// <param name="parameters">Query parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Query results containing nodes and relationships.</returns>
    Task<GraphQueryResult> ExecuteQueryAsync(string query, Dictionary<string, object>? parameters = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds shortest paths between nodes.
    /// </summary>
    /// <param name="startNodeId">ID of the starting node.</param>
    /// <param name="endNodeId">ID of the ending node.</param>
    /// <param name="maxDepth">Maximum path depth to search.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of shortest paths.</returns>
    Task<IEnumerable<GraphPath>> FindShortestPathsAsync(string startNodeId, string endNodeId, int maxDepth = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets graph statistics for monitoring and analytics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Knowledge graph statistics.</returns>
    Task<KnowledgeGraphStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Port for document processing operations in the Semantic RAG system.
/// This defines the contract for OCR, entity extraction, and content processing.
/// </summary>
public interface IDocumentProcessingPort
{
    /// <summary>
    /// Processes a document using OCR and extracts text content.
    /// </summary>
    /// <param name="document">Document to process.</param>
    /// <param name="options">Processing options including OCR settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processed document with extracted text and metadata.</returns>
    Task<ProcessedDocument> ProcessDocumentAsync(DocumentInput document, DocumentProcessingOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts entities from text content using LLM services.
    /// </summary>
    /// <param name="text">Text content to analyze.</param>
    /// <param name="options">Entity extraction options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of extracted entities with confidence scores.</returns>
    Task<IEnumerable<ExtractedEntity>> ExtractEntitiesAsync(string text, EntityExtractionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Maps relationships between extracted entities.
    /// </summary>
    /// <param name="entities">Collection of entities to analyze.</param>
    /// <param name="options">Relationship mapping options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of mapped relationships.</returns>
    Task<IEnumerable<EntityRelationship>> MapEntityRelationshipsAsync(IEnumerable<ExtractedEntity> entities, RelationshipMappingOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for text content.
    /// </summary>
    /// <param name="text">Text content to embed.</param>
    /// <param name="options">Embedding generation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated vector embedding.</returns>
    Task<VectorEmbedding> GenerateEmbeddingAsync(string text, EmbeddingOptions options, CancellationToken cancellationToken = default);
}

/// <summary>
/// Port for analyzer integration in the Semantic RAG system.
/// This defines the contract for running analyzers and processing diagnostics.
/// </summary>
public interface IAnalyzerIntegrationPort
{
    /// <summary>
    /// Runs analyzers on a solution and returns diagnostics.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file.</param>
    /// <param name="options">Analyzer execution options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of analyzer diagnostics.</returns>
    Task<IEnumerable<AnalyzerDiagnostic>> RunAnalyzersAsync(string solutionPath, AnalyzerExecutionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies code fixes for specific diagnostics.
    /// </summary>
    /// <param name="diagnostics">Diagnostics to fix.</param>
    /// <param name="options">Fix application options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing applied fixes and validation results.</returns>
    Task<CodeFixResult> ApplyCodeFixesAsync(IEnumerable<AnalyzerDiagnostic> diagnostics, CodeFixOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that applied fixes don't introduce new issues.
    /// </summary>
    /// <param name="originalCode">Original code before fixes.</param>
    /// <param name="fixedCode">Code after applying fixes.</param>
    /// <param name="options">Validation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validation result with any new issues found.</returns>
    Task<FixValidationResult> ValidateFixesAsync(string originalCode, string fixedCode, FixValidationOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available analyzers and their capabilities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of available analyzers with metadata.</returns>
    Task<IEnumerable<AnalyzerMetadata>> GetAvailableAnalyzersAsync(CancellationToken cancellationToken = default);
}