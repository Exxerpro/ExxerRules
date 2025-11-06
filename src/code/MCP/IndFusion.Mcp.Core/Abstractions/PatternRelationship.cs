namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Pattern relationship discovered in the graph.
/// </summary>
/// <param name="SourcePattern">Source pattern ID.</param>
/// <param name="TargetPattern">Target pattern ID.</param>
/// <param name="RelationshipType">Type of relationship.</param>
/// <param name="Strength">Strength of the relationship (0.0-1.0).</param>
/// <param name="Context">Context of the relationship.</param>
public record PatternRelationship(
    string SourcePattern,
    string TargetPattern,
    string RelationshipType,
    double Strength,
    Dictionary<string, object> Context
);