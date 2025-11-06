namespace IndFusion.SemanticRag.Domain.Models;

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
    IReadOnlyList<string>? IncludePatterns = null,
    IReadOnlyList<string>? ExcludePatterns = null,
    long MaxFileSize = 10 * 1024 * 1024, // 10MB
    int MaxDepth = 10,
    DocumentIngestionOptions? IngestionOptions = null,
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