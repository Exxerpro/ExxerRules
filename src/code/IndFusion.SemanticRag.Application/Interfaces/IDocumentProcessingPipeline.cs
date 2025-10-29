using IndFusion.SemanticRag.Domain.Models;

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