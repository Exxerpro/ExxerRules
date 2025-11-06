namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents a source document used in RAG generation.
/// </summary>
public record RagSource
{
    /// <summary>
    /// Unique identifier for the source.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Source content.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Relevance score (0-1).
    /// </summary>
    public required float RelevanceScore { get; init; }

    /// <summary>
    /// Source metadata.
    /// </summary>
    public required Dictionary<string, object> Metadata { get; init; }

    /// <summary>
    /// Source type (e.g., "code", "documentation", "pattern").
    /// </summary>
    public required string Type { get; init; }
}