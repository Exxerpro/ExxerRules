namespace IndFusion.SemanticRag.Domain.Errors;

/// <summary>
/// Error codes for the Semantic RAG domain.
/// These codes provide stable test assertions that are resilient to message wording changes.
/// </summary>
public static class ErrorCodes
{
    // Vector/Embedding Error Codes (VE###)
    /// <summary>
    /// Vector or embedding ID is required but was null or empty.
    /// </summary>
    public const string VectorIdRequired = "VE001";

    /// <summary>
    /// Vector or embedding content is required but was null or empty.
    /// </summary>
    public const string VectorContentRequired = "VE002";

    /// <summary>
    /// Vector embedding array is required but was null or empty.
    /// </summary>
    public const string VectorEmbeddingRequired = "VE003";

    /// <summary>
    /// Vector embedding dimension is invalid (doesn't match expected size).
    /// </summary>
    public const string VectorDimensionInvalid = "VE004";

    // Document Error Codes (DO###)
    /// <summary>
    /// Document ID is required but was null or empty.
    /// </summary>
    public const string DocumentIdRequired = "DO001";

    /// <summary>
    /// Document content is required but was null or empty.
    /// </summary>
    public const string DocumentContentRequired = "DO002";

    /// <summary>
    /// Document commit hash is required but was null or empty.
    /// </summary>
    public const string DocumentCommitHashRequired = "DO003";

    /// <summary>
    /// Document source path is required but was null or empty.
    /// </summary>
    public const string DocumentSourcePathRequired = "DO004";

    /// <summary>
    /// Document commit hash cannot be empty or whitespace.
    /// </summary>
    public const string DocumentCommitHashEmpty = "DO005";

    /// <summary>
    /// Document source path cannot be empty or whitespace.
    /// </summary>
    public const string DocumentSourcePathEmpty = "DO006";

    /// <summary>
    /// Document metadata is required but was null.
    /// </summary>
    public const string DocumentMetadataRequired = "DO007";

    /// <summary>
    /// Document repository is required but was null or empty.
    /// </summary>
    public const string DocumentRepositoryRequired = "DO008";

    // Knowledge Graph Error Codes (KG###)
    /// <summary>
    /// Knowledge node ID is required but was null or empty.
    /// </summary>
    public const string KnowledgeNodeIdRequired = "KG001";

    /// <summary>
    /// Knowledge node label is required but was null or empty.
    /// </summary>
    public const string KnowledgeNodeLabelRequired = "KG002";

    /// <summary>
    /// Knowledge relationship ID is required but was null or empty.
    /// </summary>
    public const string KnowledgeRelationshipIdRequired = "KG003";

    /// <summary>
    /// Knowledge relationship source node ID is required but was null or empty.
    /// </summary>
    public const string KnowledgeRelationshipSourceIdRequired = "KG004";

    /// <summary>
    /// Knowledge relationship target node ID is required but was null or empty.
    /// </summary>
    public const string KnowledgeRelationshipTargetIdRequired = "KG005";

    /// <summary>
    /// Knowledge relationship type is required but was null or empty.
    /// </summary>
    public const string KnowledgeRelationshipTypeRequired = "KG006";

    /// <summary>
    /// Knowledge relationship source and target node IDs cannot be the same.
    /// </summary>
    public const string KnowledgeRelationshipSameNodeIds = "KG007";

    // Semantic Document Error Codes (SD###)
    /// <summary>
    /// Semantic document ID is required but was null or empty.
    /// </summary>
    public const string SemanticDocumentIdRequired = "SD001";

    /// <summary>
    /// Semantic document content is required but was null or empty.
    /// </summary>
    public const string SemanticDocumentContentRequired = "SD002";

    // Configuration Error Codes (CF###)
    /// <summary>
    /// Configuration collection name is required but was null or empty.
    /// </summary>
    public const string ConfigCollectionNameRequired = "CF001";

    /// <summary>
    /// Configuration vector size is invalid (must be greater than 0).
    /// </summary>
    public const string ConfigVectorSizeInvalid = "CF002";

    // Search Error Codes (SE###)
    /// <summary>
    /// Search query is required but was null or empty.
    /// </summary>
    public const string SearchQueryRequired = "SE001";

    /// <summary>
    /// Search query vector is required but was null or empty.
    /// </summary>
    public const string SearchQueryVectorRequired = "SE002";

    /// <summary>
    /// Search limit is invalid (must be greater than 0).
    /// </summary>
    public const string SearchLimitInvalid = "SE003";

    // General Validation Error Codes (GV###)
    /// <summary>
    /// Required parameter is null.
    /// </summary>
    public const string ParameterNull = "GV001";

    /// <summary>
    /// Required parameter is null or empty.
    /// </summary>
    public const string ParameterNullOrEmpty = "GV002";

    /// <summary>
    /// Required parameter is null or whitespace.
    /// </summary>
    public const string ParameterNullOrWhitespace = "GV003";

    /// <summary>
    /// Collection is empty when at least one item is required.
    /// </summary>
    public const string CollectionEmpty = "GV004";

    /// <summary>
    /// Value is out of valid range.
    /// </summary>
    public const string ValueOutOfRange = "GV005";

    // Cancellation Error Codes
    /// <summary>
    /// Operation was cancelled.
    /// </summary>
    public const string OperationCancelled = "GV006";

    // Vector Database Error Codes (VE###)
    /// <summary>
    /// General vector database error occurred.
    /// </summary>
    public const string VectorDatabaseError = "VE005";

    /// <summary>
    /// Collection was not found in the vector database.
    /// </summary>
    public const string CollectionNotFound = "VE006";

    /// <summary>
    /// Vector search operation failed.
    /// </summary>
    public const string VectorSearchFailed = "VE007";

    // Graph Database Error Codes (KG###)
    /// <summary>
    /// General graph database error occurred.
    /// </summary>
    public const string GraphDatabaseError = "KG007";

    /// <summary>
    /// Cypher query execution failed.
    /// </summary>
    public const string CypherQueryFailed = "KG008";

    /// <summary>
    /// Entity was not found in the graph database.
    /// </summary>
    public const string EntityNotFound = "KG009";

    // Embedding Service Error Codes (VE###)
    /// <summary>
    /// General embedding service error occurred.
    /// </summary>
    public const string EmbeddingServiceError = "VE008";

    /// <summary>
    /// Embedding generation failed.
    /// </summary>
    public const string EmbeddingGenerationFailed = "VE009";

    /// <summary>
    /// Model was not found in the embedding service.
    /// </summary>
    public const string ModelNotFound = "VE010";
}

