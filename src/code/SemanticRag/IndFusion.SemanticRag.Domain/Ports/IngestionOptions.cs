namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Represents options for content ingestion.
/// </summary>
/// <param name="IncludeFileTypes">File types to include in ingestion.</param>
/// <param name="ExcludeFileTypes">File types to exclude from ingestion.</param>
/// <param name="MaxFileSize">Maximum file size to process in bytes.</param>
/// <param name="GenerateEmbeddings">Whether to generate embeddings during ingestion.</param>
/// <param name="CreateKnowledgeGraph">Whether to create knowledge graph nodes.</param>
/// <param name="ChunkSize">Size of content chunks for processing.</param>
/// <param name="OverlapSize">Overlap size between chunks.</param>
public record IngestionOptions(
    IReadOnlyList<string>? IncludeFileTypes = null,
    IReadOnlyList<string>? ExcludeFileTypes = null,
    long MaxFileSize = 10 * 1024 * 1024, // 10MB default
    bool GenerateEmbeddings = true,
    bool CreateKnowledgeGraph = true,
    int ChunkSize = 1000,
    int OverlapSize = 200)
{
    /// <summary>
    /// Gets the file types to include.
    /// </summary>
    public IReadOnlyList<string>? IncludeFileTypes { get; init; } = IncludeFileTypes;

    /// <summary>
    /// Gets the file types to exclude.
    /// </summary>
    public IReadOnlyList<string>? ExcludeFileTypes { get; init; } = ExcludeFileTypes;

    /// <summary>
    /// Gets the maximum file size in bytes.
    /// </summary>
    public long MaxFileSize { get; init; } = MaxFileSize;

    /// <summary>
    /// Gets whether to generate embeddings.
    /// </summary>
    public bool GenerateEmbeddings { get; init; } = GenerateEmbeddings;

    /// <summary>
    /// Gets whether to create knowledge graph nodes.
    /// </summary>
    public bool CreateKnowledgeGraph { get; init; } = CreateKnowledgeGraph;

    /// <summary>
    /// Gets the chunk size for content processing.
    /// </summary>
    public int ChunkSize { get; init; } = ChunkSize;

    /// <summary>
    /// Gets the overlap size between chunks.
    /// </summary>
    public int OverlapSize { get; init; } = OverlapSize;

    /// <summary>
    /// Validates the ingestion options.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (MaxFileSize <= 0)
            return Result.WithFailure("Max file size must be greater than zero");

        if (ChunkSize <= 0)
            return Result.WithFailure("Chunk size must be greater than zero");

        if (OverlapSize < 0)
            return Result.WithFailure("Overlap size cannot be negative");

        if (OverlapSize >= ChunkSize)
            return Result.WithFailure("Overlap size must be less than chunk size");

        return Result.Success();
    }
}