namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the result of a graph traversal operation.
/// </summary>
/// <param name="Nodes">Nodes visited during traversal.</param>
/// <param name="Relationships">Relationships traversed.</param>
/// <param name="Paths">Complete paths found during traversal.</param>
/// <param name="MaxDepthReached">Maximum depth reached during traversal.</param>
/// <param name="TotalNodesVisited">Total number of nodes visited.</param>
/// <param name="TotalRelationshipsTraversed">Total number of relationships traversed.</param>
public readonly record struct GraphTraversalResult(
    IReadOnlyList<GraphNode> Nodes,
    IReadOnlyList<GraphRelationship> Relationships,
    IReadOnlyList<GraphPath> Paths,
    int MaxDepthReached,
    int TotalNodesVisited,
    int TotalRelationshipsTraversed)
{
    /// <summary>
    /// Gets the number of unique paths found.
    /// </summary>
    public int PathCount => Paths.Count;

    /// <summary>
    /// Checks if any paths were found.
    /// </summary>
    public bool HasPaths => Paths.Count > 0;
}