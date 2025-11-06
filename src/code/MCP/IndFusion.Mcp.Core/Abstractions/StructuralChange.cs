namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Structural change detected in code.
/// </summary>
/// <param name="ChangeType">Type of structural change.</param>
/// <param name="ElementName">Name of the changed element.</param>
/// <param name="ChangeDescription">Description of the change.</param>
/// <param name="Impact">Impact of the change.</param>
public record StructuralChange(
    string ChangeType,
    string ElementName,
    string ChangeDescription,
    string Impact
);