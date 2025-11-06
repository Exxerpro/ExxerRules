namespace IndFusion.SemanticRag.Domain.Models;

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