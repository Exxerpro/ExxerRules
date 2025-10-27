using IndFusion.SemanticRag.Domain.Models;
using System.Text.Json.Serialization;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for ingesting documents into the knowledge base with vector embeddings.
/// </summary>
public interface IDocumentIngestionService
{
    /// <summary>
    /// Ingests a single document into the knowledge base.
    /// </summary>
    /// <param name="input">Document input to ingest.</param>
    /// <param name="options">Ingestion options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ingestion result.</returns>
    Task<DocumentIngestionResult> IngestDocumentAsync(
        DocumentInput input, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests multiple documents in batch.
    /// </summary>
    /// <param name="inputs">List of document inputs.</param>
    /// <param name="options">Ingestion options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of ingestion results.</returns>
    Task<IReadOnlyList<DocumentIngestionResult>> IngestDocumentsAsync(
        IReadOnlyList<DocumentInput> inputs, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Ingests documents from a directory.
    /// </summary>
    /// <param name="directoryPath">Path to the directory.</param>
    /// <param name="options">Ingestion options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of ingestion results.</returns>
    Task<IReadOnlyList<DocumentIngestionResult>> IngestDirectoryAsync(
        string directoryPath, 
        DocumentIngestionOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets ingestion status for a document.
    /// </summary>
    /// <param name="documentId">Document identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ingestion status.</returns>
    Task<DocumentIngestionStatus> GetIngestionStatusAsync(
        string documentId, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Options for document ingestion.
/// </summary>
public record DocumentIngestionOptions
{
    /// <summary>
    /// Whether to generate embeddings for chunks.
    /// </summary>
    public bool GenerateEmbeddings { get; init; } = true;

    /// <summary>
    /// Whether to store in vector database.
    /// </summary>
    public bool StoreInVectorDatabase { get; init; } = true;

    /// <summary>
    /// Whether to store in knowledge graph.
    /// </summary>
    public bool StoreInKnowledgeGraph { get; init; } = true;

    /// <summary>
    /// Source identifier for the documents.
    /// </summary>
    public string? SourceId { get; init; }

    /// <summary>
    /// Tags to associate with the documents.
    /// </summary>
    public IReadOnlyList<string>? Tags { get; init; }

    /// <summary>
    /// Whether to overwrite existing documents.
    /// </summary>
    public bool OverwriteExisting { get; init; } = false;

    /// <summary>
    /// Processing options for document processing.
    /// </summary>
    public DocumentProcessingOptions ProcessingOptions { get; init; } = new();
}

/// <summary>
/// Result of document ingestion.
/// </summary>
public record DocumentIngestionResult
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    public required string DocumentId { get; init; }

    /// <summary>
    /// Whether ingestion was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Number of chunks created.
    /// </summary>
    public required int ChunksCreated { get; init; }

    /// <summary>
    /// Number of embeddings generated.
    /// </summary>
    public required int EmbeddingsGenerated { get; init; }

    /// <summary>
    /// Number of graph nodes created.
    /// </summary>
    public required int GraphNodesCreated { get; init; }

    /// <summary>
    /// Error message if ingestion failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Time taken for ingestion in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }

    /// <summary>
    /// Processing result from document processing.
    /// </summary>
    public DocumentProcessingResult? ProcessingResult { get; init; }
}

/// <summary>
/// Status of document ingestion.
/// </summary>
public record DocumentIngestionStatus
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    public required string DocumentId { get; init; }

    /// <summary>
    /// Current ingestion status.
    /// </summary>
    public required IngestionStatus Status { get; init; }

    /// <summary>
    /// Progress percentage (0-100).
    /// </summary>
    public required int ProgressPercentage { get; init; }

    /// <summary>
    /// Current processing step.
    /// </summary>
    public required string CurrentStep { get; init; }

    /// <summary>
    /// Error message if failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Timestamp when ingestion started.
    /// </summary>
    public required DateTime StartedAt { get; init; }

    /// <summary>
    /// Timestamp when ingestion completed (if applicable).
    /// </summary>
    public DateTime? CompletedAt { get; init; }
}

/// <summary>
/// Ingestion status values.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IngestionStatus
{
    /// <summary>
    /// Ingestion is pending.
    /// </summary>
    Pending,

    /// <summary>
    /// Document is being processed.
    /// </summary>
    Processing,

    /// <summary>
    /// Embeddings are being generated.
    /// </summary>
    GeneratingEmbeddings,

    /// <summary>
    /// Storing in vector database.
    /// </summary>
    StoringInVectorDatabase,

    /// <summary>
    /// Storing in knowledge graph.
    /// </summary>
    StoringInKnowledgeGraph,

    /// <summary>
    /// Ingestion completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Ingestion failed.
    /// </summary>
    Failed
}


