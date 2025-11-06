namespace IndFusion.SemanticRag.Domain.Models;

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