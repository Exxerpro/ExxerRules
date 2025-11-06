namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Represents a code entity (class, method, property, etc.).
/// </summary>
/// <param name="Id">Unique identifier for the code entity.</param>
/// <param name="Type">Type of the code entity (CLASS, METHOD, etc.).</param>
/// <param name="Name">Name of the code entity.</param>
/// <param name="FullName">Fully qualified name.</param>
/// <param name="Namespace">Namespace containing the entity.</param>
/// <param name="AccessModifier">Access modifier (public, private, etc.).</param>
/// <param name="Parameters">Method parameters (if applicable).</param>
/// <param name="ReturnType">Return type (if applicable).</param>
/// <param name="Properties">Additional properties.</param>
/// <param name="Embedding">Vector embedding for semantic similarity.</param>
public readonly record struct CodeEntity(
    string Id,
    string Type,
    string Name,
    string FullName,
    string? Namespace,
    string? AccessModifier,
    IReadOnlyList<CodeParameter>? Parameters,
    string? ReturnType,
    IReadOnlyDictionary<string, object> Properties,
    float[]? Embedding)
{
    /// <summary>
    /// Gets the display name for the code entity.
    /// </summary>
    public string DisplayName => !string.IsNullOrWhiteSpace(Namespace) 
        ? $"{Namespace}.{Name}" 
        : Name;
}