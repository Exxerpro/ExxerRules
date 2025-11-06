namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A search result from semantic search.
/// </summary>
/// <param name="Id">Unique identifier for the result.</param>
/// <param name="Content">Content of the search result.</param>
/// <param name="Similarity">Similarity score (0.0-1.0).</param>
/// <param name="Source">Source of the result.</param>
/// <param name="Metadata">Additional metadata.</param>
/// <param name="Location">Location of the result in the codebase.</param>
public record SearchResult(
    string Id,
    string Content,
    double Similarity,
    string Source,
    Dictionary<string, object> Metadata,
    SourceLocation Location
);