namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a relationship between patterns.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="Type">Type of the relationship.</param>
/// <param name="SourcePatternId">ID of the source pattern.</param>
/// <param name="TargetPatternId">ID of the target pattern.</param>
/// <param name="Properties">Properties of the relationship.</param>
/// <param name="Strength">Strength of the relationship (0.0 to 1.0).</param>
public readonly record struct PatternRelationship(
    string Id,
    string Type,
    string SourcePatternId,
    string TargetPatternId,
    IReadOnlyDictionary<string, object> Properties,
    float Strength = 1.0f)
{
    /// <summary>
    /// Validates the pattern relationship.
    /// </summary>
    /// <returns>A Result indicating whether the relationship is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Relationship ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Type))
            return Result.WithFailure("Relationship type cannot be null or empty");

        if (string.IsNullOrWhiteSpace(SourcePatternId))
            return Result.WithFailure("Source pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(TargetPatternId))
            return Result.WithFailure("Target pattern ID cannot be null or empty");

        if (Strength < 0.0f || Strength > 1.0f)
            return Result.WithFailure("Strength must be between 0.0 and 1.0");

        return Result.Success();
    }
}