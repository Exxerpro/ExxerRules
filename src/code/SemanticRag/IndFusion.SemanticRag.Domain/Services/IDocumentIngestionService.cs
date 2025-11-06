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