using System.Text.Json.Serialization;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Input document for processing operations.
/// </summary>
/// <param name="Id">Unique identifier for the document.</param>
/// <param name="Name">Name of the document.</param>
/// <param name="Content">Document content as byte array.</param>
/// <param name="MimeType">MIME type of the document.</param>
/// <param name="FilePath">Optional file path.</param>
/// <param name="Metadata">Additional document metadata.</param>
public record DocumentInput(
    string Id,
    string Name,
    byte[] Content,
    string MimeType,
    string? FilePath = null,
    IReadOnlyDictionary<string, object>? Metadata = null
);

/// <summary>
/// Options for document processing operations.
/// </summary>
/// <param name="ExtractMetadata">Whether to extract metadata.</param>
/// <param name="EnableOcr">Whether to enable OCR processing.</param>
/// <param name="OcrLanguage">Language for OCR processing.</param>
/// <param name="EnableChunking">Whether to enable chunking.</param>
/// <param name="ChunkingStrategy">Strategy for chunking documents.</param>
/// <param name="MaxChunkSize">Maximum size for chunks.</param>
/// <param name="CustomSettings">Custom processing settings.</param>
public record DocumentProcessingOptions(
    bool ExtractMetadata = true,
    bool EnableOcr = true,
    string OcrLanguage = "eng",
    bool EnableChunking = true,
    ChunkingStrategy ChunkingStrategy = ChunkingStrategy.Recursive,
    int MaxChunkSize = 1000,
    IReadOnlyDictionary<string, object>? CustomSettings = null
);

/// <summary>
/// Represents different chunking strategies for documents.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChunkingStrategy
{
    /// <summary>
    /// Splits text by a fixed size.
    /// </summary>
    FixedSize,
    /// <summary>
    /// Splits text by sentences.
    /// </summary>
    Sentence,
    /// <summary>
    /// Splits text by paragraphs.
    /// </summary>
    Paragraph,
    /// <summary>
    /// Recursively splits text based on delimiters.
    /// </summary>
    Recursive,
    /// <summary>
    /// Splits text semantically based on content structure.
    /// </summary>
    Semantic,
    /// <summary>
    /// Splits text by lines (useful for code).
    /// </summary>
    LineBased,
    /// <summary>
    /// Splits text by paragraphs.
    /// </summary>
    ParagraphBased
}

/// <summary>
/// Represents the status of a processing operation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProcessingStatus
{
    /// <summary>
    /// Processing is pending.
    /// </summary>
    Pending,
    /// <summary>
    /// Processing is in progress.
    /// </summary>
    InProgress,
    /// <summary>
    /// Processing completed successfully.
    /// </summary>
    Success,
    /// <summary>
    /// Processing failed.
    /// </summary>
    Failed,
    /// <summary>
    /// Processing was cancelled.
    /// </summary>
    Cancelled
}

/// <summary>
/// Result of document processing operations.
/// </summary>
public record DocumentProcessingResult
{
    /// <summary>
    /// Unique identifier for the processing result.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// ID of the original document input.
    /// </summary>
    public required string DocumentId { get; init; }

    /// <summary>
    /// Extracted text content.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Document chunks created during processing.
    /// </summary>
    public IReadOnlyList<DocumentChunk> Chunks { get; init; } = new List<DocumentChunk>();

    /// <summary>
    /// Processing metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Detected document type.
    /// </summary>
    public required DocumentType DocumentType { get; init; }

    /// <summary>
    /// Processing status.
    /// </summary>
    public required ProcessingStatus Status { get; init; }

    /// <summary>
    /// Processing duration in milliseconds.
    /// </summary>
    public long ElapsedMilliseconds { get; init; }

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? ErrorMessage { get; init; }
}

/// <summary>
/// Represents different document types that can be processed.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentType
{
    /// <summary>
    /// Unknown document type.
    /// </summary>
    Unknown,

    /// <summary>
    /// Plain text document.
    /// </summary>
    Text,

    /// <summary>
    /// Markdown document.
    /// </summary>
    Markdown,

    /// <summary>
    /// PDF document.
    /// </summary>
    Pdf,

    /// <summary>
    /// Microsoft Word document.
    /// </summary>
    Word,

    /// <summary>
    /// Microsoft Excel spreadsheet.
    /// </summary>
    Excel,

    /// <summary>
    /// PowerPoint presentation.
    /// </summary>
    PowerPoint,

    /// <summary>
    /// Image file (PNG, JPG, etc.).
    /// </summary>
    Image,

    /// <summary>
    /// C# code file.
    /// </summary>
    CSharpCode,

    /// <summary>
    /// TypeScript/JavaScript code file.
    /// </summary>
    TypeScriptCode,

    /// <summary>
    /// Python code file.
    /// </summary>
    PythonCode,

    /// <summary>
    /// HTML document.
    /// </summary>
    Html,

    /// <summary>
    /// XML document.
    /// </summary>
    Xml,

    /// <summary>
    /// JSON document.
    /// </summary>
    Json,

    /// <summary>
    /// CSV file.
    /// </summary>
    Csv
}

/// <summary>
/// Result of document processing operations.
/// </summary>
/// <param name="Id">Unique identifier for the processed document.</param>
/// <param name="OriginalId">ID of the original input document.</param>
/// <param name="ExtractedText">Text extracted from the document.</param>
/// <param name="Metadata">Processing metadata and results.</param>
/// <param name="ProcessedAt">When the document was processed.</param>
public record ProcessedDocument(
    string Id,
    string OriginalId,
    string ExtractedText,
    Dictionary<string, object> Metadata,
    DateTime ProcessedAt
);

/// <summary>
/// Entity extracted from text content.
/// </summary>
/// <param name="Id">Unique identifier for the entity.</param>
/// <param name="Text">The text that was identified as an entity.</param>
/// <param name="Type">Type of entity (person, organization, location, etc.).</param>
/// <param name="Confidence">Confidence score for the extraction.</param>
/// <param name="StartPosition">Start position in the original text.</param>
/// <param name="EndPosition">End position in the original text.</param>
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
/// Options for entity extraction operations.
/// </summary>
/// <param name="EntityTypes">Types of entities to extract.</param>
/// <param name="MinConfidence">Minimum confidence threshold.</param>
/// <param name="MaxEntities">Maximum number of entities to extract.</param>
/// <param name="Language">Language for extraction.</param>
public record EntityExtractionOptions(
    IEnumerable<string> EntityTypes,
    double MinConfidence = 0.5,
    int MaxEntities = 100,
    string Language = "en"
);

/// <summary>
/// Relationship between extracted entities.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="SourceEntityId">ID of the source entity.</param>
/// <param name="TargetEntityId">ID of the target entity.</param>
/// <param name="RelationshipType">Type of relationship.</param>
/// <param name="Confidence">Confidence score for the relationship.</param>
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
/// Options for relationship mapping operations.
/// </summary>
/// <param name="RelationshipTypes">Types of relationships to map.</param>
/// <param name="MinConfidence">Minimum confidence threshold.</param>
/// <param name="MaxRelationships">Maximum number of relationships to map.</param>
public record RelationshipMappingOptions(
    IEnumerable<string> RelationshipTypes,
    double MinConfidence = 0.5,
    int MaxRelationships = 50
);

/// <summary>
/// Options for embedding generation operations.
/// </summary>
/// <param name="Model">Embedding model to use.</param>
/// <param name="Dimensions">Number of dimensions for the embedding.</param>
/// <param name="Normalize">Whether to normalize the embedding vector.</param>
public record EmbeddingOptions(
    string Model = "text-embedding-ada-002",
    int Dimensions = 1536,
    bool Normalize = true
);
