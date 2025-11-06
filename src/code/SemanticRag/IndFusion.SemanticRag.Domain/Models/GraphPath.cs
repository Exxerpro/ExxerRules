namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a path through the graph.
/// </summary>
/// <param name="Nodes">Nodes in the path.</param>
/// <param name="Relationships">Relationships in the path.</param>
/// <param name="Length">Length of the path (number of relationships).</param>
/// <param name="Weight">Optional weight of the path.</param>
public readonly record struct GraphPath(
    IReadOnlyList<GraphNode> Nodes,
    IReadOnlyList<GraphRelationship> Relationships,
    int Length,
    double Weight = 1.0)
{
    /// <summary>
    /// Gets the start node of the path.
    /// </summary>
    public GraphNode StartNode => Nodes.Count > 0 ? Nodes[0] : throw new InvalidOperationException("Path has no nodes");

    /// <summary>
    /// Gets the end node of the path.
    /// </summary>
    public GraphNode EndNode => Nodes.Count > 0 ? Nodes[^1] : throw new InvalidOperationException("Path has no nodes");

    /// <summary>
    /// Validates the graph path.
    /// </summary>
    /// <returns>A Result indicating whether the path is valid.</returns>
    public Result Validate()
    {
        if (Nodes is null || Nodes.Count == 0)
            return Result.WithFailure("Path must have at least one node");

        if (Relationships is null)
            return Result.WithFailure("Path relationships cannot be null");

        if (Length < 0)
            return Result.WithFailure("Path length cannot be negative");

        if (Weight < 0)
            return Result.WithFailure("Path weight cannot be negative");

        return Result.Success();
    }
}