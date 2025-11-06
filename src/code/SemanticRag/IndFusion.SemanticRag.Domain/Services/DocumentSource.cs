namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Represents a document source for ingestion.
/// </summary>
/// <param name="Path">Path to the document.</param>
/// <param name="Type">Type of the document (e.g., "code", "markdown", "text").</param>
/// <param name="Metadata">Additional metadata about the source.</param>
public readonly record struct DocumentSource(
    string Path,
    string Type,
    IReadOnlyDictionary<string, object>? Metadata = null)
{
    /// <summary>
    /// Gets the file extension from the path.
    /// </summary>
    public string FileExtension => System.IO.Path.GetExtension(Path).ToLowerInvariant();

    /// <summary>
    /// Gets the file name from the path.
    /// </summary>
    public string FileName => System.IO.Path.GetFileName(Path);

    /// <summary>
    /// Validates the document source.
    /// </summary>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Path))
            return Result.WithFailure("Document path cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Type))
            return Result.WithFailure("Document type cannot be null or empty");

        return Result.Success();
    }
}