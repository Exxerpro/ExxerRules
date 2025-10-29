using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for document ingestion operations in the Semantic RAG system.
/// This defines the contract for ingesting documents into the knowledge base.
/// </summary>
public interface IDocumentIngestionPort
{
    /// <summary>
    /// Ingests a single document into the knowledge base.
    /// </summary>
    /// <param name="input">Document input to ingest.</param>
    /// <param name="options">Ingestion options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the ingestion result.</returns>
    Task<Result<DocumentIngestionResult>> IngestDocumentAsync(
        DocumentInput input, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests multiple documents in batch.
    /// </summary>
    /// <param name="inputs">List of document inputs.</param>
    /// <param name="options">Ingestion options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the batch ingestion results.</returns>
    Task<Result<IReadOnlyList<DocumentIngestionResult>>> IngestDocumentsAsync(
        IReadOnlyList<DocumentInput> inputs, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests an entire repository into the knowledge base.
    /// </summary>
    /// <param name="repositoryPath">Path to the repository.</param>
    /// <param name="config">Repository ingestion configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the repository ingestion results.</returns>
    Task<Result<RepositoryIngestionResult>> IngestRepositoryAsync(
        string repositoryPath,
        RepositoryIngestionConfig config,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the ingestion status for a document.
    /// </summary>
    /// <param name="documentId">ID of the document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the ingestion status.</returns>
    Task<Result<DocumentIngestionStatus>> GetIngestionStatusAsync(
        string documentId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels an ongoing ingestion operation.
    /// </summary>
    /// <param name="documentId">ID of the document to cancel.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    Task<Result> CancelIngestionAsync(
        string documentId, 
        CancellationToken cancellationToken = default);
}
