using System;
using System.Collections.Generic;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a node in the knowledge graph.
/// </summary>
/// <param name="Id">Unique identifier for the node.</param>
/// <param name="Label">The node label or type.</param>
/// <param name="Properties">Properties associated with the node.</param>
/// <param name="CreatedAt">Timestamp when the node was created.</param>
/// <param name="UpdatedAt">Timestamp when the node was last updated.</param>
public record KnowledgeNode(
    string Id,
    string Label,
    IReadOnlyDictionary<string, object> Properties,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    /// <summary>
    /// Gets the node identifier.
    /// </summary>
    public string Id { get; init; } = Id;

    /// <summary>
    /// Gets the node label.
    /// </summary>
    public string Label { get; init; } = Label;

    /// <summary>
    /// Gets the node properties.
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties { get; init; } = Properties;

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = CreatedAt;

    /// <summary>
    /// Gets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; } = UpdatedAt;

    /// <summary>
    /// Validates the knowledge node for basic requirements.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Node ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(Label))
            return Result.WithFailure("Node label cannot be empty or whitespace");

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
    /// Checks if the node has a specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>True if the property exists, otherwise false.</returns>
    public bool HasProperty(string key)
    {
        return Properties.ContainsKey(key);
    }
}

/// <summary>
/// Represents a relationship between two knowledge nodes.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="FromNodeId">ID of the source node.</param>
/// <param name="ToNodeId">ID of the target node.</param>
/// <param name="RelationshipType">Type of the relationship.</param>
/// <param name="Properties">Properties associated with the relationship.</param>
/// <param name="CreatedAt">Timestamp when the relationship was created.</param>
public record KnowledgeRelationship(
    string Id,
    string FromNodeId,
    string ToNodeId,
    string RelationshipType,
    IReadOnlyDictionary<string, object> Properties,
    DateTimeOffset CreatedAt)
{
    /// <summary>
    /// Gets the relationship identifier.
    /// </summary>
    public string Id { get; init; } = Id;

    /// <summary>
    /// Gets the source node identifier.
    /// </summary>
    public string FromNodeId { get; init; } = FromNodeId;

    /// <summary>
    /// Gets the target node identifier.
    /// </summary>
    public string ToNodeId { get; init; } = ToNodeId;

    /// <summary>
    /// Gets the relationship type.
    /// </summary>
    public string RelationshipType { get; init; } = RelationshipType;

    /// <summary>
    /// Gets the relationship properties.
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties { get; init; } = Properties;

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = CreatedAt;

    /// <summary>
    /// Validates the knowledge relationship for basic requirements.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Relationship ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(FromNodeId))
            return Result.WithFailure("From node ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(ToNodeId))
            return Result.WithFailure("To node ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(RelationshipType))
            return Result.WithFailure("Relationship type cannot be empty or whitespace");

        if (FromNodeId == ToNodeId)
            return Result.WithFailure("From node and To node cannot be the same");

        return Result.Success();
    }
}
