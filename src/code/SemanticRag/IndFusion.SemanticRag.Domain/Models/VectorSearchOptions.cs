namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents options for vector search operations.
/// </summary>
/// <param name="Limit">Maximum number of results to return.</param>
/// <param name="Threshold">Minimum similarity threshold (0.0 to 1.0).</param>
/// <param name="IncludeMetadata">Whether to include metadata in results.</param>
/// <param name="IncludeEmbedding">Whether to include the embedding vector in results.</param>
/// <param name="Filters">Optional metadata filters to apply.</param>
/// <param name="TimeoutMs">Search timeout in milliseconds.</param>
public readonly record struct VectorSearchOptions(
    int Limit = 10,
    float Threshold = 0.7f,
    bool IncludeMetadata = true,
    bool IncludeEmbedding = false,
    IReadOnlyDictionary<string, object>? Filters = null,
    int TimeoutMs = 5000)
{
    /// <summary>
    /// Validates the vector search options.
    /// </summary>
    /// <returns>A Result indicating whether the options are valid.</returns>
    public Result Validate()
    {
        if (Limit <= 0)
            return Result.WithFailure("Limit must be greater than 0");

        if (Threshold < 0.0f || Threshold > 1.0f)
            return Result.WithFailure("Threshold must be between 0.0 and 1.0");

        if (TimeoutMs <= 0)
            return Result.WithFailure("Timeout must be greater than 0");

        return Result.Success();
    }

    /// <summary>
    /// Default options for general vector search.
    /// </summary>
    public static VectorSearchOptions Default() => new(
        Limit: 10,
        Threshold: 0.7f,
        IncludeMetadata: true,
        IncludeEmbedding: false,
        Filters: null,
        TimeoutMs: 5000);

    /// <summary>
    /// Options for high-precision search.
    /// </summary>
    public static VectorSearchOptions HighPrecision() => new(
        Limit: 5,
        Threshold: 0.9f,
        IncludeMetadata: true,
        IncludeEmbedding: false,
        Filters: null,
        TimeoutMs: 10000);

    /// <summary>
    /// Options for broad search with many results.
    /// </summary>
    public static VectorSearchOptions Broad() => new(
        Limit: 50,
        Threshold: 0.5f,
        IncludeMetadata: true,
        IncludeEmbedding: false,
        Filters: null,
        TimeoutMs: 3000);
}