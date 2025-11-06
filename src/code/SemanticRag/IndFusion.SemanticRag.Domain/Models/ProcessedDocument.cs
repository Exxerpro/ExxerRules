namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Result of document processing operations.
/// </summary>
/// <param name="Id">Unique identifier for the processed document.</param>
/// <param name="OriginalId">ID of the original input document.</param>
/// <param name="ExtractedText">Text extracted from the document.</param>
/// <param name="Metadata">Processing metadata and results.</param>
/// <param name="ProcessedAt">When the document was processed.</param>
public record ProcessedDocument(
    string Id,
    string OriginalId,
    string ExtractedText,
    Dictionary<string, object> Metadata,
    DateTime ProcessedAt
);