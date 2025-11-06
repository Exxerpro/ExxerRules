namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Input document for processing operations.
/// </summary>
/// <param name="Id">Unique identifier for the document.</param>
/// <param name="Name">Name of the document.</param>
/// <param name="Content">Document content as byte array.</param>
/// <param name="MimeType">MIME type of the document.</param>
/// <param name="FilePath">Optional file path.</param>
/// <param name="Metadata">Additional document metadata.</param>
public record DocumentInput(
    string Id,
    string Name,
    byte[] Content,
    string MimeType,
    string? FilePath = null,
    IReadOnlyDictionary<string, object>? Metadata = null
);
