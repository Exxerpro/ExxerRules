namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Graph traversal result from pattern graph queries.
/// </summary>
/// <param name="NodeId">ID of the traversed node.</param>
/// <param name="NodeType">Type of the node.</param>
/// <param name="Properties">Node properties.</param>
/// <param name="Relationships">Relationships from this node.</param>
/// <param name="Depth">Depth of traversal.</param>
public record GraphTraversalResult(
    string NodeId,
    string NodeType,
    Dictionary<string, object> Properties,
    IEnumerable<string> Relationships,
    int Depth
);