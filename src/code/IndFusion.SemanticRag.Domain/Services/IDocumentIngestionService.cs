using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Service for ingesting and processing documents for semantic RAG.
/// </summary>
public interface IDocumentIngestionService
{
    /// <summary>
    /// Ingests a single document from various sources.
    /// </summary>
    /// <param name="source">The source of the document.</param>
    /// <param name="content">The document content.</param>
    /// <param name="metadata">Additional metadata about the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the processed document.</returns>
    Task<Result<SemanticDocument>> IngestDocumentAsync(
        DocumentSource source,
        string content,
        IReadOnlyDictionary<string, object>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests multiple documents in batch.
    /// </summary>
    /// <param name="sources">The document sources to ingest.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the processed documents.</returns>
    Task<Result<IReadOnlyList<SemanticDocument>>> IngestDocumentsAsync(
        IReadOnlyList<DocumentSource> sources,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests documents from a repository or codebase.
    /// </summary>
    /// <param name="repositoryPath">Path to the repository.</param>
    /// <param name="config">Configuration for repository ingestion.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the processed documents.</returns>
    Task<Result<IReadOnlyList<SemanticDocument>>> IngestRepositoryAsync(
        string repositoryPath,
        RepositoryIngestionConfig config,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts knowledge entities from a document.
    /// </summary>
    /// <param name="document">The document to extract entities from.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the extracted entities.</returns>
    Task<Result<IReadOnlyList<KnowledgeEntity>>> ExtractEntitiesAsync(
        SemanticDocument document,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts relationships between entities in a document.
    /// </summary>
    /// <param name="document">The document to extract relationships from.</param>
    /// <param name="entities">The entities found in the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the extracted relationships.</returns>
    Task<Result<IReadOnlyList<KnowledgeRelationship>>> ExtractRelationshipsAsync(
        SemanticDocument document,
        IReadOnlyList<KnowledgeEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the supported file types for ingestion.
    /// </summary>
    /// <returns>A list of supported file extensions.</returns>
    IReadOnlyList<string> GetSupportedFileTypes();

    /// <summary>
    /// Validates if a file can be ingested.
    /// </summary>
    /// <param name="filePath">Path to the file to validate.</param>
    /// <returns>A Result indicating if the file can be ingested.</returns>
    Result ValidateFile(string filePath);
}

/// <summary>
/// Represents a document source for ingestion.
/// </summary>
/// <param name="Path">Path to the document.</param>
/// <param name="Type">Type of the document (e.g., "code", "markdown", "text").</param>
/// <param name="Metadata">Additional metadata about the source.</param>
public readonly record struct DocumentSource(
    string Path,
    string Type,
    IReadOnlyDictionary<string, object>? Metadata = null)
{
    /// <summary>
    /// Gets the file extension from the path.
    /// </summary>
    public string FileExtension => System.IO.Path.GetExtension(Path).ToLowerInvariant();

    /// <summary>
    /// Gets the file name from the path.
    /// </summary>
    public string FileName => System.IO.Path.GetFileName(Path);

    /// <summary>
    /// Validates the document source.
    /// </summary>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Path))
            return Result.WithFailure("Document path cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Type))
            return Result.WithFailure("Document type cannot be null or empty");

        return Result.Success();
    }
}

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