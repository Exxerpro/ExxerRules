using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for graph node operations.
/// </summary>
/// <param name="Nodes">The graph nodes found.</param>
/// <param name="TotalNodes">Total number of nodes.</param>
/// <param name="NodeType">The node type that was searched.</param>
public readonly record struct GraphNodeResponse(
    IReadOnlyList<GraphNode> Nodes,
    int TotalNodes,
    string NodeType);