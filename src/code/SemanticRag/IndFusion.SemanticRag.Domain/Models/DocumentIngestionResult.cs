namespace IndFusion.SemanticRag.Domain.Models;

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