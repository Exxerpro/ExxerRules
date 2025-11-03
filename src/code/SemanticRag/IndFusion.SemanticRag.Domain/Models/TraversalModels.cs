using IndQuestResults;
using System;
using System.Collections.Generic;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a node in the graph.
/// </summary>
/// <param name="Id">Unique identifier for the node.</param>
/// <param name="Type">Type of the node.</param>
/// <param name="Properties">Properties of the node.</param>
/// <param name="Labels">Labels associated with the node.</param>
public readonly record struct GraphNode(
    string Id,
    string Type,
    IReadOnlyDictionary<string, object> Properties,
    IReadOnlyList<string> Labels)
{
    /// <summary>
    /// Validates the graph node.
    /// </summary>
    /// <returns>A Result indicating whether the node is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Type))
            return Result.WithFailure("Node type cannot be null or empty");

        if (Properties is null)
            return Result.WithFailure("Node properties cannot be null");

        if (Labels is null)
            return Result.WithFailure("Node labels cannot be null");

        return Result.Success();
    }

    /// <summary>
    /// Gets a property value by key.
    /// </summary>
    /// <typeparam name="T">The expected type of the property value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The property value if found, otherwise default value.</returns>
    public T? GetProperty<T>(string key)
    {
        if (Properties.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }

    /// <summary>
    /// Checks if the node has a specific label.
    /// </summary>
    /// <param name="label">The label to check for.</param>
    /// <returns>True if the node has the label, otherwise false.</returns>
    public bool HasLabel(string label) => Labels.Contains(label, StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// Represents a relationship between two nodes in the graph.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="Type">Type of the relationship.</param>
/// <param name="StartNodeId">ID of the start node.</param>
/// <param name="EndNodeId">ID of the end node.</param>
/// <param name="Properties">Properties of the relationship.</param>
public readonly record struct GraphRelationship(
    string Id,
    string Type,
    string StartNodeId,
    string EndNodeId,
    IReadOnlyDictionary<string, object> Properties)
{
    /// <summary>
    /// Validates the graph relationship.
    /// </summary>
    /// <returns>A Result indicating whether the relationship is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Relationship ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Type))
            return Result.WithFailure("Relationship type cannot be null or empty");

        if (string.IsNullOrWhiteSpace(StartNodeId))
            return Result.WithFailure("Start node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(EndNodeId))
            return Result.WithFailure("End node ID cannot be null or empty");

        if (Properties is null)
            return Result.WithFailure("Relationship properties cannot be null");

        return Result.Success();
    }

    /// <summary>
    /// Gets a property value by key.
    /// </summary>
    /// <typeparam name="T">The expected type of the property value.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>The property value if found, otherwise default value.</returns>
    public T? GetProperty<T>(string key)
    {
        if (Properties.TryGetValue(key, out var value) && value is T typedValue)
            return typedValue;

        return default;
    }
}

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

/// <summary>
/// Represents statistics about the graph structure.
/// </summary>
/// <param name="TotalNodes">Total number of nodes in the graph.</param>
/// <param name="TotalRelationships">Total number of relationships in the graph.</param>
/// <param name="NodeTypes">Count of nodes by type.</param>
/// <param name="RelationshipTypes">Count of relationships by type.</param>
/// <param name="AverageDegree">Average degree of nodes.</param>
/// <param name="MaxDegree">Maximum degree of any node.</param>
/// <param name="ConnectedComponents">Number of connected components.</param>
/// <param name="LastUpdated">When the statistics were last updated.</param>
public readonly record struct GraphStatistics(
    int TotalNodes,
    int TotalRelationships,
    IReadOnlyDictionary<string, int> NodeTypes,
    IReadOnlyDictionary<string, int> RelationshipTypes,
    double AverageDegree,
    int MaxDegree,
    int ConnectedComponents,
    DateTimeOffset LastUpdated)
{
    /// <summary>
    /// Gets the density of the graph (relationships / possible relationships).
    /// </summary>
    public double Density
    {
        get
        {
            if (TotalNodes <= 1)
                return 0.0;

            var maxPossibleRelationships = TotalNodes * (TotalNodes - 1);
            return (double)TotalRelationships / maxPossibleRelationships;
        }
    }

    /// <summary>
    /// Gets the most common node type.
    /// </summary>
    public string? MostCommonNodeType
    {
        get
        {
            if (NodeTypes.Count == 0)
                return null;

            return NodeTypes.MaxBy(kvp => kvp.Value).Key;
        }
    }

    /// <summary>
    /// Gets the most common relationship type.
    /// </summary>
    public string? MostCommonRelationshipType
    {
        get
        {
            if (RelationshipTypes.Count == 0)
                return null;

            return RelationshipTypes.MaxBy(kvp => kvp.Value).Key;
        }
    }
}
