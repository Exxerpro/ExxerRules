using IndQuestResults;
using System;
using System.Collections.Generic;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a relationship between nodes in a knowledge graph.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="Type">Type of the relationship.</param>
/// <param name="FromNodeId">ID of the source node.</param>
/// <param name="ToNodeId">ID of the target node.</param>
/// <param name="Properties">Properties associated with the relationship.</param>
/// <param name="Weight">Weight or strength of the relationship (0.0 to 1.0).</param>
/// <param name="CreatedAt">When the relationship was created.</param>
/// <param name="UpdatedAt">When the relationship was last updated.</param>
public readonly record struct GraphRelationship(
    string Id,
    string Type,
    string FromNodeId,
    string ToNodeId,
    IReadOnlyDictionary<string, object> Properties,
    float Weight = 1.0f,
    DateTimeOffset? CreatedAt = null,
    DateTimeOffset? UpdatedAt = null)
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

        if (string.IsNullOrWhiteSpace(FromNodeId))
            return Result.WithFailure("From node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(ToNodeId))
            return Result.WithFailure("To node ID cannot be null or empty");

        if (Properties is null)
            return Result.WithFailure("Relationship properties cannot be null");

        if (FromNodeId == ToNodeId)
            return Result.WithFailure("From node and To node cannot be the same");

        if (Weight < 0.0f || Weight > 1.0f)
            return Result.WithFailure("Weight must be between 0.0 and 1.0");

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
    /// Checks if the relationship has a specific property.
    /// </summary>
    /// <param name="key">The property key.</param>
    /// <returns>True if the property exists, otherwise false.</returns>
    public bool HasProperty(string key)
    {
        return Properties.ContainsKey(key);
    }

    /// <summary>
    /// Checks if this is a bidirectional relationship.
    /// </summary>
    /// <returns>True if the relationship is bidirectional, otherwise false.</returns>
    public bool IsBidirectional()
    {
        return GetProperty<bool>("bidirectional") || GetProperty<bool>("symmetric");
    }

    /// <summary>
    /// Gets the display name for the relationship.
    /// </summary>
    /// <returns>A formatted string representing the relationship.</returns>
    public string GetDisplayName()
    {
        return $"{FromNodeId} --[{Type}]--> {ToNodeId}";
    }
}