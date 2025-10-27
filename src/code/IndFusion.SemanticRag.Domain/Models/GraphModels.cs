using System.Diagnostics.CodeAnalysis;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a knowledge graph node.
/// </summary>
/// <param name="Id">Unique identifier for the node.</param>
/// <param name="Label">The node label/type.</param>
/// <param name="Properties">Properties associated with the node.</param>
public readonly record struct GraphNode(
    string Id,
    string Label,
    Dictionary<string, object> Properties)
{
    /// <summary>
    /// Validates the graph node for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the node is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Label))
            return Result.WithFailure("Node label cannot be null or empty");

        if (Properties is null)
            return Result.WithFailure("Node properties cannot be null");

        return Result.Success();
    }
}

/// <summary>
/// Represents a knowledge graph edge/relationship.
/// </summary>
/// <param name="Id">Unique identifier for the edge.</param>
/// <param name="SourceNodeId">ID of the source node.</param>
/// <param name="TargetNodeId">ID of the target node.</param>
/// <param name="Relationship">The type of relationship.</param>
/// <param name="Properties">Properties associated with the edge.</param>
public readonly record struct GraphEdge(
    string Id,
    string SourceNodeId,
    string TargetNodeId,
    string Relationship,
    Dictionary<string, object> Properties)
{
    /// <summary>
    /// Validates the graph edge for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the edge is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Edge ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(SourceNodeId))
            return Result.WithFailure("Source node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(TargetNodeId))
            return Result.WithFailure("Target node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Relationship))
            return Result.WithFailure("Edge relationship cannot be null or empty");

        if (Properties is null)
            return Result.WithFailure("Edge properties cannot be null");

        return Result.Success();
    }
}