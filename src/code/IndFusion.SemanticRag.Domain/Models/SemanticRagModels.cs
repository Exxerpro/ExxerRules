namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a vector embedding with associated metadata.
/// </summary>
/// <param name="Id">Unique identifier for the vector.</param>
/// <param name="Vector">The embedding vector values.</param>
/// <param name="Metadata">Associated metadata including source information.</param>
/// <param name="CreatedAt">When the vector was created.</param>
/// <param name="UpdatedAt">When the vector was last updated.</param>
public record VectorEmbedding(
    string Id,
    float[] Vector,
    Dictionary<string, object> Metadata,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Options for vector similarity search.
/// </summary>
/// <param name="MaxResults">Maximum number of results to return.</param>
/// <param name="SimilarityThreshold">Minimum similarity score threshold.</param>
/// <param name="MetadataFilters">Filters to apply to metadata.</param>
/// <param name="IncludeMetadata">Whether to include metadata in results.</param>
public record VectorSearchOptions(
    int MaxResults = 10,
    double SimilarityThreshold = 0.7,
    Dictionary<string, object>? MetadataFilters = null,
    bool IncludeMetadata = true
);

/// <summary>
/// Result of a vector similarity search.
/// </summary>
/// <param name="VectorId">ID of the similar vector.</param>
/// <param name="SimilarityScore">Similarity score (0.0-1.0).</param>
/// <param name="Metadata">Associated metadata.</param>
public record VectorSearchResult(
    string VectorId,
    double SimilarityScore,
    Dictionary<string, object> Metadata
);

/// <summary>
/// Result of vector store operations.
/// </summary>
/// <param name="Success">Whether the operation succeeded.</param>
/// <param name="Message">Success or error message.</param>
/// <param name="AffectedCount">Number of vectors affected by the operation.</param>
/// <param name="ErrorDetails">Detailed error information if operation failed.</param>
public record VectorStoreResult(
    bool Success,
    string Message,
    int AffectedCount = 0,
    string? ErrorDetails = null
);

/// <summary>
/// Statistics about the vector store.
/// </summary>
/// <param name="TotalVectors">Total number of vectors stored.</param>
/// <param name="VectorDimensions">Dimension count of vectors.</param>
/// <param name="StorageSizeBytes">Storage size in bytes.</param>
/// <param name="LastUpdated">When statistics were last updated.</param>
public record VectorStoreStatistics(
    long TotalVectors,
    int VectorDimensions,
    long StorageSizeBytes,
    DateTime LastUpdated
);

/// <summary>
/// Represents a node in the knowledge graph.
/// </summary>
/// <param name="Id">Unique identifier for the node.</param>
/// <param name="Label">Node label/type.</param>
/// <param name="Properties">Node properties.</param>
/// <param name="CreatedAt">When the node was created.</param>
/// <param name="UpdatedAt">When the node was last updated.</param>
public record GraphNode(
    string Id,
    string Label,
    Dictionary<string, object> Properties,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Represents a relationship between nodes in the knowledge graph.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="StartNodeId">ID of the starting node.</param>
/// <param name="EndNodeId">ID of the ending node.</param>
/// <param name="RelationshipType">Type of the relationship.</param>
/// <param name="Properties">Relationship properties.</param>
/// <param name="CreatedAt">When the relationship was created.</param>
public record GraphRelationship(
    string Id,
    string StartNodeId,
    string EndNodeId,
    string RelationshipType,
    Dictionary<string, object> Properties,
    DateTime CreatedAt
);

/// <summary>
/// Result of knowledge graph operations.
/// </summary>
/// <param name="Success">Whether the operation succeeded.</param>
/// <param name="Message">Success or error message.</param>
/// <param name="AffectedCount">Number of nodes/relationships affected.</param>
/// <param name="ErrorDetails">Detailed error information if operation failed.</param>
public record KnowledgeGraphResult(
    bool Success,
    string Message,
    int AffectedCount = 0,
    string? ErrorDetails = null
);

/// <summary>
/// Result of a graph query execution.
/// </summary>
/// <param name="Nodes">Nodes returned by the query.</param>
/// <param name="Relationships">Relationships returned by the query.</param>
/// <param name="ExecutionTimeMs">Query execution time in milliseconds.</param>
/// <param name="ErrorDetails">Error details if query failed.</param>
public record GraphQueryResult(
    IEnumerable<GraphNode> Nodes,
    IEnumerable<GraphRelationship> Relationships,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Represents a path through the knowledge graph.
/// </summary>
/// <param name="Nodes">Nodes in the path.</param>
/// <param name="Relationships">Relationships connecting the nodes.</param>
/// <param name="TotalWeight">Total weight/cost of the path.</param>
public record GraphPath(
    IEnumerable<GraphNode> Nodes,
    IEnumerable<GraphRelationship> Relationships,
    double TotalWeight
);

/// <summary>
/// Statistics about the knowledge graph.
/// </summary>
/// <param name="TotalNodes">Total number of nodes.</param>
/// <param name="TotalRelationships">Total number of relationships.</param>
/// <param name="NodeTypes">Count of nodes by type.</param>
/// <param name="RelationshipTypes">Count of relationships by type.</param>
/// <param name="LastUpdated">When statistics were last updated.</param>
public record KnowledgeGraphStatistics(
    long TotalNodes,
    long TotalRelationships,
    Dictionary<string, long> NodeTypes,
    Dictionary<string, long> RelationshipTypes,
    DateTime LastUpdated
);

/// <summary>
/// Input document for processing.
/// </summary>
/// <param name="Id">Unique identifier for the document.</param>
/// <param name="Content">Document content (text or binary).</param>
/// <param name="ContentType">MIME type of the content.</param>
/// <param name="Metadata">Document metadata.</param>
/// <param name="SourcePath">Path to the source document.</param>
public record DocumentInput(
    string Id,
    byte[] Content,
    string ContentType,
    Dictionary<string, object> Metadata,
    string? SourcePath = null
);

/// <summary>
/// Options for document processing.
/// </summary>
/// <param name="EnableOCR">Whether to enable OCR processing.</param>
/// <param name="OcrLanguage">Language for OCR processing.</param>
/// <param name="ExtractEntities">Whether to extract entities.</param>
/// <param name="GenerateEmbeddings">Whether to generate embeddings.</param>
/// <param name="QualityThreshold">Minimum quality threshold for processing.</param>
public record DocumentProcessingOptions(
    bool EnableOCR = true,
    string OcrLanguage = "en",
    bool ExtractEntities = true,
    bool GenerateEmbeddings = true,
    double QualityThreshold = 0.8
);

/// <summary>
/// Processed document with extracted content and metadata.
/// </summary>
/// <param name="DocumentId">ID of the original document.</param>
/// <param name="ExtractedText">Text extracted from the document.</param>
/// <param name="Entities">Entities extracted from the text.</param>
/// <param name="Embeddings">Generated embeddings.</param>
/// <param name="ProcessingMetadata">Metadata about the processing.</param>
/// <param name="ProcessedAt">When the document was processed.</param>
public record ProcessedDocument(
    string DocumentId,
    string ExtractedText,
    IEnumerable<ExtractedEntity> Entities,
    IEnumerable<VectorEmbedding> Embeddings,
    Dictionary<string, object> ProcessingMetadata,
    DateTime ProcessedAt
);

/// <summary>
/// Entity extracted from text content.
/// </summary>
/// <param name="Id">Unique identifier for the entity.</param>
/// <param name="Text">Text of the entity.</param>
/// <param name="Type">Type/category of the entity.</param>
/// <param name="Confidence">Confidence score (0.0-1.0).</param>
/// <param name="StartPosition">Start position in the text.</param>
/// <param name="EndPosition">End position in the text.</param>
/// <param name="Properties">Additional entity properties.</param>
public record ExtractedEntity(
    string Id,
    string Text,
    string Type,
    double Confidence,
    int StartPosition,
    int EndPosition,
    Dictionary<string, object> Properties
);

/// <summary>
/// Options for entity extraction.
/// </summary>
/// <param name="EntityTypes">Types of entities to extract.</param>
/// <param name="ConfidenceThreshold">Minimum confidence threshold.</param>
/// <param name="MaxEntities">Maximum number of entities to extract.</param>
/// <param name="IncludeProperties">Whether to include additional properties.</param>
public record EntityExtractionOptions(
    IEnumerable<string> EntityTypes,
    double ConfidenceThreshold = 0.7,
    int MaxEntities = 100,
    bool IncludeProperties = true
);

/// <summary>
/// Relationship between extracted entities.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="SourceEntityId">ID of the source entity.</param>
/// <param name="TargetEntityId">ID of the target entity.</param>
/// <param name="RelationshipType">Type of the relationship.</param>
/// <param name="Confidence">Confidence score (0.0-1.0).</param>
/// <param name="Properties">Additional relationship properties.</param>
public record EntityRelationship(
    string Id,
    string SourceEntityId,
    string TargetEntityId,
    string RelationshipType,
    double Confidence,
    Dictionary<string, object> Properties
);

/// <summary>
/// Options for relationship mapping.
/// </summary>
/// <param name="RelationshipTypes">Types of relationships to map.</param>
/// <param name="ConfidenceThreshold">Minimum confidence threshold.</param>
/// <param name="MaxRelationships">Maximum number of relationships to map.</param>
public record RelationshipMappingOptions(
    IEnumerable<string> RelationshipTypes,
    double ConfidenceThreshold = 0.6,
    int MaxRelationships = 50
);

/// <summary>
/// Options for embedding generation.
/// </summary>
/// <param name="Model">Embedding model to use.</param>
/// <param name="Dimensions">Number of dimensions for the embedding.</param>
/// <param name="Normalize">Whether to normalize the embedding.</param>
public record EmbeddingOptions(
    string Model = "text-embedding-3-large",
    int Dimensions = 1536,
    bool Normalize = true
);