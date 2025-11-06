namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// An item from external knowledge.
/// </summary>
/// <param name="ItemId">Unique identifier for the item.</param>
/// <param name="ItemType">Type of the knowledge item.</param>
/// <param name="Content">Content of the item.</param>
/// <param name="Metadata">Metadata about the item.</param>
/// <param name="Relevance">Relevance score (0.0-1.0).</param>
public record ExternalKnowledgeItem(
    string ItemId,
    string ItemType,
    string Content,
    Dictionary<string, object> Metadata,
    double Relevance
);