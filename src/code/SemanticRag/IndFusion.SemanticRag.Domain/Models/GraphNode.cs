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

