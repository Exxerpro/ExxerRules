namespace IndFusion.SemanticRag.WebAPI.Controllers;

/// <summary>
/// Request model for document storage.
/// </summary>
public record DocumentStorageRequest
{
    /// <summary>
    /// Unique identifier for the document.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Document content.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Document metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}