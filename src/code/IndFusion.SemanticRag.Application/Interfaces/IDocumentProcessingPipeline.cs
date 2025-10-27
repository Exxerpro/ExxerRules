using IndFusion.SemanticRag.Domain.Models;
using System.Text.Json.Serialization;

namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Service for processing various document types including code files, PDFs, and images.
/// </summary>
public interface IDocumentProcessingPipeline
{
    /// <summary>
    /// Processes a document and extracts text content with metadata.
    /// </summary>
    /// <param name="input">Document input to process.</param>
    /// <param name="options">Processing options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processing result with extracted content.</returns>
    Task<DocumentProcessingResult> ProcessDocumentAsync(
        DocumentInput input, 
        DocumentProcessingOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Processes multiple documents in batch.
    /// </summary>
    /// <param name="inputs">List of document inputs.</param>
    /// <param name="options">Processing options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of processing results.</returns>
    Task<IReadOnlyList<DocumentProcessingResult>> ProcessDocumentsAsync(
        IReadOnlyList<DocumentInput> inputs, 
        DocumentProcessingOptions options, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects the document type from the input.
    /// </summary>
    /// <param name="input">Document input to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detected document type.</returns>
    Task<DocumentType> DetectDocumentTypeAsync(
        DocumentInput input, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets supported document types.
    /// </summary>
    /// <returns>List of supported document types.</returns>
    IReadOnlyList<DocumentType> GetSupportedDocumentTypes();
}

/// <summary>
/// Represents a document input for processing.
/// </summary>
public record DocumentInput
{
    /// <summary>
    /// Unique identifier for the document.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Document name or title.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Document content as bytes.
    /// </summary>
    public required byte[] Content { get; init; }

    /// <summary>
    /// Document file path (if available).
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// Document MIME type.
    /// </summary>
    public string? MimeType { get; init; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Options for document processing.
/// </summary>
public record DocumentProcessingOptions
{
    /// <summary>
    /// Whether to extract metadata.
    /// </summary>
    public bool ExtractMetadata { get; init; } = true;

    /// <summary>
    /// Whether to perform OCR on images.
    /// </summary>
    public bool EnableOcr { get; init; } = true;

    /// <summary>
    /// Language for OCR processing.
    /// </summary>
    public string OcrLanguage { get; init; } = "eng";

    /// <summary>
    /// Maximum file size in bytes.
    /// </summary>
    public long MaxFileSize { get; init; } = 50 * 1024 * 1024; // 50MB

    /// <summary>
    /// Chunking strategy for large documents.
    /// </summary>
    public ChunkingStrategy ChunkingStrategy { get; init; } = ChunkingStrategy.Semantic;

    /// <summary>
    /// Maximum chunk size in characters.
    /// </summary>
    public int MaxChunkSize { get; init; } = 1000;
}

/// <summary>
/// Result of document processing.
/// </summary>
public record DocumentProcessingResult
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    public required string DocumentId { get; init; }

    /// <summary>
    /// Extracted text content.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Document chunks for processing.
    /// </summary>
    public required IReadOnlyList<DocumentChunk> Chunks { get; init; }

    /// <summary>
    /// Extracted metadata.
    /// </summary>
    public required Dictionary<string, object> Metadata { get; init; }

    /// <summary>
    /// Detected document type.
    /// </summary>
    public required DocumentType DocumentType { get; init; }

    /// <summary>
    /// Processing status.
    /// </summary>
    public required ProcessingStatus Status { get; init; }

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Time taken for processing in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }
}

/// <summary>
/// Represents a chunk of a processed document.
/// </summary>
public record DocumentChunk
{
    /// <summary>
    /// Unique identifier for the chunk.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Chunk content.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Chunk index within the document.
    /// </summary>
    public required int Index { get; init; }

    /// <summary>
    /// Start position in the original document.
    /// </summary>
    public required int StartPosition { get; init; }

    /// <summary>
    /// End position in the original document.
    /// </summary>
    public required int EndPosition { get; init; }

    /// <summary>
    /// Chunk metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Document types supported by the processing pipeline.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DocumentType
{
    /// <summary>
    /// C# source code file.
    /// </summary>
    CSharpCode,

    /// <summary>
    /// TypeScript/JavaScript code file.
    /// </summary>
    TypeScriptCode,

    /// <summary>
    /// Python source code file.
    /// </summary>
    PythonCode,

    /// <summary>
    /// Markdown documentation.
    /// </summary>
    Markdown,

    /// <summary>
    /// PDF document.
    /// </summary>
    Pdf,

    /// <summary>
    /// Image file (PNG, JPG, etc.).
    /// </summary>
    Image,

    /// <summary>
    /// Plain text file.
    /// </summary>
    Text,

    /// <summary>
    /// Unknown or unsupported type.
    /// </summary>
    Unknown
}

/// <summary>
/// Processing status for documents.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ProcessingStatus
{
    /// <summary>
    /// Processing completed successfully.
    /// </summary>
    Success,

    /// <summary>
    /// Processing failed with error.
    /// </summary>
    Failed,

    /// <summary>
    /// Processing is in progress.
    /// </summary>
    Processing,

    /// <summary>
    /// Document type not supported.
    /// </summary>
    Unsupported
}

/// <summary>
/// Chunking strategies for document processing.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ChunkingStrategy
{
    /// <summary>
    /// Fixed-size chunks.
    /// </summary>
    FixedSize,

    /// <summary>
    /// Semantic chunks based on content structure.
    /// </summary>
    Semantic,

    /// <summary>
    /// Line-based chunks for code files.
    /// </summary>
    LineBased,

    /// <summary>
    /// Paragraph-based chunks for text documents.
    /// </summary>
    ParagraphBased
}


