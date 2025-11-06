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
