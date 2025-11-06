namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Configuration for repository ingestion.
/// </summary>
/// <param name="IncludePatterns">File patterns to include (e.g., "*.cs", "*.md").</param>
/// <param name="ExcludePatterns">File patterns to exclude (e.g., "*/bin/*", "*/obj/*").</param>
/// <param name="MaxFileSize">Maximum file size to process in bytes.</param>
/// <param name="ExtractCodeEntities">Whether to extract code entities (classes, methods, etc.).</param>
/// <param name="ExtractComments">Whether to extract and process comments.</param>
/// <param name="ProcessDependencies">Whether to process dependency relationships.</param>
/// <param name="MaxDepth">Maximum directory depth to traverse.</param>
public readonly record struct RepositoryIngestionConfig(
    IReadOnlyList<string> IncludePatterns,
    IReadOnlyList<string> ExcludePatterns,
    long MaxFileSize = 1024 * 1024, // 1MB
    bool ExtractCodeEntities = true,
    bool ExtractComments = true,
    bool ProcessDependencies = true,
    int MaxDepth = 10)
{
    /// <summary>
    /// Default configuration for C# repositories.
    /// </summary>
    public static RepositoryIngestionConfig ForCSharpRepository() => new(
        IncludePatterns: new[] { "*.cs", "*.csproj", "*.sln", "*.md", "*.json", "*.xml" },
        ExcludePatterns: new[] { "*/bin/*", "*/obj/*", "*/packages/*", "*/node_modules/*" },
        MaxFileSize: 2 * 1024 * 1024, // 2MB
        ExtractCodeEntities: true,
        ExtractComments: true,
        ProcessDependencies: true,
        MaxDepth: 15);

    /// <summary>
    /// Default configuration for general documentation.
    /// </summary>
    public static RepositoryIngestionConfig ForDocumentation() => new(
        IncludePatterns: new[] { "*.md", "*.txt", "*.rst", "*.adoc" },
        ExcludePatterns: new[] { "*/node_modules/*", "*/vendor/*" },
        MaxFileSize: 512 * 1024, // 512KB
        ExtractCodeEntities: false,
        ExtractComments: false,
        ProcessDependencies: false,
        MaxDepth: 5);

    /// <summary>
    /// Validates the configuration.
    /// </summary>
    public Result Validate()
    {
        if (IncludePatterns.Count == 0)
            return Result.WithFailure("At least one include pattern must be specified");

        if (MaxFileSize <= 0)
            return Result.WithFailure("MaxFileSize must be greater than 0");

        if (MaxDepth < 0)
            return Result.WithFailure("MaxDepth cannot be negative");

        return Result.Success();
    }
}