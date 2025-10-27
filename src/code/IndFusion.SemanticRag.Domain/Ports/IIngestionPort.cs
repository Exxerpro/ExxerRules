using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for content ingestion operations in the semantic RAG system.
/// </summary>
public interface IIngestionPort
{
    /// <summary>
    /// Ingests a single document into the system.
    /// </summary>
    /// <param name="document">The document to ingest.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> IngestDocumentAsync(Document document, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests multiple documents into the system.
    /// </summary>
    /// <param name="documents">The documents to ingest.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> IngestDocumentsAsync(IReadOnlyList<Document> documents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests content from a repository.
    /// </summary>
    /// <param name="repositoryPath">Path to the repository.</param>
    /// <param name="repositoryName">Name of the repository.</param>
    /// <param name="commitHash">Git commit hash.</param>
    /// <param name="options">Ingestion options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the ingestion summary.</returns>
    Task<Result<IngestionSummary>> IngestRepositoryAsync(
        string repositoryPath,
        string repositoryName,
        string commitHash,
        IngestionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests content from a specific file.
    /// </summary>
    /// <param name="filePath">Path to the file.</param>
    /// <param name="repositoryName">Name of the repository.</param>
    /// <param name="commitHash">Git commit hash.</param>
    /// <param name="options">Ingestion options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the ingested document.</returns>
    Task<Result<Document>> IngestFileAsync(
        string filePath,
        string repositoryName,
        string commitHash,
        IngestionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the ingestion status for a repository.
    /// </summary>
    /// <param name="repositoryName">Name of the repository.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the ingestion status.</returns>
    Task<Result<IngestionStatus>> GetIngestionStatusAsync(string repositoryName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of ingested repositories.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the repository list.</returns>
    Task<Result<IReadOnlyList<string>>> GetIngestedRepositoriesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all ingested content for a repository.
    /// </summary>
    /// <param name="repositoryName">Name of the repository.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> RemoveRepositoryAsync(string repositoryName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all ingested content.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> ClearAllAsync(CancellationToken cancellationToken = default);
}

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

/// <summary>
/// Represents the summary of an ingestion operation.
/// </summary>
/// <param name="RepositoryName">Name of the repository.</param>
/// <param name="CommitHash">Git commit hash.</param>
/// <param name="DocumentsProcessed">Number of documents processed.</param>
/// <param name="EmbeddingsGenerated">Number of embeddings generated.</param>
/// <param name="KnowledgeNodesCreated">Number of knowledge graph nodes created.</param>
/// <param name="ProcessingTime">Time taken for processing in milliseconds.</param>
/// <param name="Errors">List of errors encountered during processing.</param>
public record IngestionSummary(
    string RepositoryName,
    string CommitHash,
    int DocumentsProcessed,
    int EmbeddingsGenerated,
    int KnowledgeNodesCreated,
    long ProcessingTime,
    IReadOnlyList<string> Errors)
{
    /// <summary>
    /// Gets the repository name.
    /// </summary>
    public string RepositoryName { get; init; } = RepositoryName;

    /// <summary>
    /// Gets the commit hash.
    /// </summary>
    public string CommitHash { get; init; } = CommitHash;

    /// <summary>
    /// Gets the number of documents processed.
    /// </summary>
    public int DocumentsProcessed { get; init; } = DocumentsProcessed;

    /// <summary>
    /// Gets the number of embeddings generated.
    /// </summary>
    public int EmbeddingsGenerated { get; init; } = EmbeddingsGenerated;

    /// <summary>
    /// Gets the number of knowledge graph nodes created.
    /// </summary>
    public int KnowledgeNodesCreated { get; init; } = KnowledgeNodesCreated;

    /// <summary>
    /// Gets the processing time in milliseconds.
    /// </summary>
    public long ProcessingTime { get; init; } = ProcessingTime;

    /// <summary>
    /// Gets the list of errors.
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = Errors;

    /// <summary>
    /// Gets whether the ingestion was successful.
    /// </summary>
    public bool IsSuccessful => Errors.Count == 0;
}

/// <summary>
/// Represents the ingestion status for a repository.
/// </summary>
/// <param name="RepositoryName">Name of the repository.</param>
/// <param name="LastIngested">Timestamp of last ingestion.</param>
/// <param name="CommitHash">Last ingested commit hash.</param>
/// <param name="DocumentCount">Number of documents ingested.</param>
/// <param name="IsUpToDate">Whether the ingestion is up to date.</param>
public record IngestionStatus(
    string RepositoryName,
    DateTimeOffset LastIngested,
    string CommitHash,
    int DocumentCount,
    bool IsUpToDate)
{
    /// <summary>
    /// Gets the repository name.
    /// </summary>
    public string RepositoryName { get; init; } = RepositoryName;

    /// <summary>
    /// Gets the last ingestion timestamp.
    /// </summary>
    public DateTimeOffset LastIngested { get; init; } = LastIngested;

    /// <summary>
    /// Gets the last ingested commit hash.
    /// </summary>
    public string CommitHash { get; init; } = CommitHash;

    /// <summary>
    /// Gets the document count.
    /// </summary>
    public int DocumentCount { get; init; } = DocumentCount;

    /// <summary>
    /// Gets whether the ingestion is up to date.
    /// </summary>
    public bool IsUpToDate { get; init; } = IsUpToDate;
}
