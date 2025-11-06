namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for indexing operations.
/// </summary>
/// <param name="ChunkSize">Size of text chunks for indexing.</param>
/// <param name="ChunkOverlap">Overlap between chunks.</param>
/// <param name="IndexingBatchSize">Batch size for indexing operations.</param>
/// <param name="UpdateFrequency">How often to update the index.</param>
public record IndexingConfiguration(
    int ChunkSize,
    int ChunkOverlap,
    int IndexingBatchSize,
    TimeSpan UpdateFrequency
);