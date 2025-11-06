namespace IndFusion.SemanticRag.WebAPI.Controllers;

/// <summary>
/// Request model for document processing.
/// </summary>
public record DocumentProcessingRequest
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Document name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Document content as base64 string.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// File path (if available).
    /// </summary>
    public string? FilePath { get; init; }

    /// <summary>
    /// MIME type.
    /// </summary>
    public string? MimeType { get; init; }
}