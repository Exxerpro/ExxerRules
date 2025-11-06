namespace IndFusion.SemanticRag.WebAPI.Controllers;

/// <summary>
/// Result model for document storage operations.
/// </summary>
public record DocumentStorageResult
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Whether the operation was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Result message.
    /// </summary>
    public required string Message { get; init; }
}