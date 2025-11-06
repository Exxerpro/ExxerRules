namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A source used by the RAG system.
/// </summary>
/// <param name="Id">Unique identifier for the source.</param>
/// <param name="SourceType">Type of the source.</param>
/// <param name="Content">Content from the source.</param>
/// <param name="Relevance">Relevance to the query (0.0-1.0).</param>
/// <param name="Location">Location of the source.</param>
/// <param name="Metadata">Additional metadata about the source.</param>
public record RagSource(
    string Id,
    string SourceType,
    string Content,
    double Relevance,
    SourceLocation Location,
    Dictionary<string, object> Metadata
);