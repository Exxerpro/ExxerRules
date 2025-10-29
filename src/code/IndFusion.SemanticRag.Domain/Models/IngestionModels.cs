using System.Text.Json.Serialization;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for document ingestion operations.
/// </summary>
/// <param name="EnableVectorIndexing">Whether to enable vector indexing.</param>
/// <param name="EnableKnowledgeGraph">Whether to enable knowledge graph creation.</param>
/// <param name="EnableEntityExtraction">Whether to enable entity extraction.</param>
/// <param name="EnableRelationshipMapping">Whether to enable relationship mapping.</param>
/// <param name="ProcessingOptions">Document processing options.</param>
/// <param name="CustomSettings">Custom ingestion settings.</param>
public record DocumentIngestionOptions(
    bool EnableVectorIndexing = true,
    bool EnableKnowledgeGraph = true,
    bool EnableEntityExtraction = true,
    bool EnableRelationshipMapping = true,
    DocumentProcessingOptions ProcessingOptions = default,
    IReadOnlyDictionary<string, object>? CustomSettings = null
)
{
    /// <summary>
    /// Default document ingestion options.
    /// </summary>
    public static DocumentIngestionOptions Default() => new(
        ProcessingOptions: new DocumentProcessingOptions()
    );
}

/// <summary>
/// Result of document ingestion operations.
/// </summary>
public record DocumentIngestionResult
{
    /// <summary>
    /// Unique identifier for the ingestion result.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// ID of the original document input.
    /// </summary>
    public required string DocumentId { get; init; }

    /// <summary>
    /// Status of the ingestion operation.
    /// </summary>
    public required IngestionStatus Status { get; init; }

    /// <summary>
    /// Processing result from document processing.
    /// </summary>
    public DocumentProcessingResult? ProcessingResult { get; init; }

    /// <summary>
    /// Vector embeddings created during ingestion.
    /// </summary>
    public IReadOnlyList<VectorEmbedding>? VectorEmbeddings { get; init; }

    /// <summary>
    /// Knowledge entities extracted during ingestion.
    /// </summary>
    public IReadOnlyList<KnowledgeEntity>? ExtractedEntities { get; init; }

    /// <summary>
    /// Knowledge relationships mapped during ingestion.
    /// </summary>
    public IReadOnlyList<KnowledgeRelationship>? MappedRelationships { get; init; }

    /// <summary>
    /// Ingestion metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }

    /// <summary>
    /// Time when ingestion started.
    /// </summary>
    public DateTimeOffset StartedAt { get; init; }

    /// <summary>
    /// Time when ingestion completed.
    /// </summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>
    /// Duration of the ingestion operation in milliseconds.
    /// </summary>
    public long DurationMs { get; init; }

    /// <summary>
    /// Error message if ingestion failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Progress percentage (0-100).
    /// </summary>
    public int ProgressPercentage { get; init; }
}

/// <summary>
/// Status of document ingestion operations.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IngestionStatus
{
    /// <summary>
    /// Ingestion is pending.
    /// </summary>
    Pending,

    /// <summary>
    /// Ingestion is in progress.
    /// </summary>
    Processing,

    /// <summary>
    /// Ingestion completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Ingestion failed.
    /// </summary>
    Failed,

    /// <summary>
    /// Ingestion was cancelled.
    /// </summary>
    Cancelled
}

/// <summary>
/// Status information for document ingestion.
/// </summary>
public record DocumentIngestionStatus
{
    /// <summary>
    /// ID of the document.
    /// </summary>
    public required string DocumentId { get; init; }

    /// <summary>
    /// Current status of the ingestion.
    /// </summary>
    public required IngestionStatus Status { get; init; }

    /// <summary>
    /// Progress percentage (0-100).
    /// </summary>
    public required int ProgressPercentage { get; init; }

    /// <summary>
    /// Current processing stage description.
    /// </summary>
    public required string CurrentStage { get; init; }

    /// <summary>
    /// Time when ingestion started.
    /// </summary>
    public required DateTimeOffset StartedAt { get; init; }

    /// <summary>
    /// Time when ingestion completed (if completed).
    /// </summary>
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>
    /// Error message if ingestion failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Additional status metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Configuration for repository ingestion operations.
/// </summary>
/// <param name="IncludePatterns">File patterns to include in ingestion.</param>
/// <param name="ExcludePatterns">File patterns to exclude from ingestion.</param>
/// <param name="MaxFileSize">Maximum file size to process.</param>
/// <param name="MaxDepth">Maximum directory depth to traverse.</param>
/// <param name="IngestionOptions">Document ingestion options.</param>
/// <param name="CustomSettings">Custom repository settings.</param>
public record RepositoryIngestionConfig(
    IReadOnlyList<string> IncludePatterns = default,
    IReadOnlyList<string> ExcludePatterns = default,
    long MaxFileSize = 10 * 1024 * 1024, // 10MB
    int MaxDepth = 10,
    DocumentIngestionOptions IngestionOptions = default,
    IReadOnlyDictionary<string, object>? CustomSettings = null
)
{
    /// <summary>
    /// Default repository ingestion configuration.
    /// </summary>
    public static RepositoryIngestionConfig Default() => new(
        IncludePatterns: new[] { "*.cs", "*.ts", "*.js", "*.py", "*.md", "*.txt" },
        ExcludePatterns: new[] { "*/bin/*", "*/obj/*", "*/node_modules/*", "*/.git/*" },
        IngestionOptions: DocumentIngestionOptions.Default()
    );

    /// <summary>
    /// Validates the repository ingestion configuration.
    /// </summary>
    /// <returns>A Result indicating whether the configuration is valid.</returns>
    public Result Validate()
    {
        if (MaxFileSize <= 0)
            return Result.WithFailure("MaxFileSize must be greater than zero");

        if (MaxDepth <= 0)
            return Result.WithFailure("MaxDepth must be greater than zero");

        if (IncludePatterns is null || IncludePatterns.Count == 0)
            return Result.WithFailure("IncludePatterns cannot be null or empty");

        return Result.Success();
    }
}

/// <summary>
/// Result of repository ingestion operations.
/// </summary>
/// <param name="ProcessedDocuments">Documents that were successfully processed.</param>
/// <param name="TotalDocuments">Total number of documents found.</param>
/// <param name="ExtractedKnowledge">Knowledge extracted from documents.</param>
/// <param name="ProcessingTimeMs">Time taken for processing in milliseconds.</param>
/// <param name="Success">Whether the ingestion was successful.</param>
public record RepositoryIngestionResult(
    IReadOnlyList<SemanticDocument> ProcessedDocuments,
    int TotalDocuments,
    IReadOnlyList<KnowledgeExtractionResult> ExtractedKnowledge,
    long ProcessingTimeMs,
    bool Success
);

/// <summary>
/// Result of knowledge extraction operations.
/// </summary>
/// <param name="DocumentId">ID of the document from which knowledge was extracted.</param>
/// <param name="Entities">Entities extracted from the document.</param>
/// <param name="Relationships">Relationships mapped from the document.</param>
/// <param name="Summary">Summary of the extracted knowledge.</param>
/// <param name="Confidence">Overall confidence score for the extraction.</param>
/// <param name="Metadata">Additional extraction metadata.</param>
public record KnowledgeExtractionResult(
    string DocumentId,
    IReadOnlyList<KnowledgeEntity> Entities,
    IReadOnlyList<KnowledgeRelationship> Relationships,
    string Summary,
    double Confidence,
    Dictionary<string, object> Metadata
);
