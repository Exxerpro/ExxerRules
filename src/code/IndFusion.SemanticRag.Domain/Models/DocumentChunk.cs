namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a chunk of a document for processing and analysis.
/// </summary>
/// <param name="Id">Unique identifier for the chunk.</param>
/// <param name="DocumentId">ID of the parent document.</param>
/// <param name="Content">Text content of the chunk.</param>
/// <param name="ChunkIndex">Index of this chunk within the document.</param>
/// <param name="StartPosition">Starting position in the original document.</param>
/// <param name="EndPosition">Ending position in the original document.</param>
/// <param name="Metadata">Additional metadata for the chunk.</param>
public record DocumentChunk(
    string Id,
    string DocumentId,
    string Content,
    int ChunkIndex,
    int StartPosition,
    int EndPosition,
    Dictionary<string, object> Metadata
)
{
    /// <summary>
    /// Gets the length of the chunk content.
    /// </summary>
    public int Length => Content.Length;

    /// <summary>
    /// Gets the chunk size in characters.
    /// </summary>
    public int Size => EndPosition - StartPosition;

    /// <summary>
    /// Validates the document chunk for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the chunk is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Chunk ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(DocumentId))
            return Result.WithFailure("Document ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Content))
            return Result.WithFailure("Chunk content cannot be null or empty");

        if (ChunkIndex < 0)
            return Result.WithFailure("Chunk index cannot be negative");

        if (StartPosition < 0)
            return Result.WithFailure("Start position cannot be negative");

        if (EndPosition < StartPosition)
            return Result.WithFailure("End position cannot be before start position");

        if (Metadata is null)
            return Result.WithFailure("Chunk metadata cannot be null");

        return Result.Success();
    }
}
