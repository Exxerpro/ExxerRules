namespace IndFusion.SemanticRag.Domain.Models;

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
    public IReadOnlyList<DocumentChunk> Chunks { get; init; } = [];

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