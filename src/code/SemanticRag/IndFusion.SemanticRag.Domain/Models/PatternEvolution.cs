namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the evolution of a pattern over time.
/// </summary>
/// <param name="PatternId">ID of the pattern.</param>
/// <param name="Version">Version of the pattern.</param>
/// <param name="ChangeType">Type of change made.</param>
/// <param name="ChangeDescription">Description of the change.</param>
/// <param name="ChangedAt">When the change was made.</param>
/// <param name="ChangedBy">Who made the change.</param>
public readonly record struct PatternEvolution(
    string PatternId,
    string Version,
    PatternChangeType ChangeType,
    string ChangeDescription,
    DateTimeOffset ChangedAt,
    string? ChangedBy = null)
{
    /// <summary>
    /// Validates the pattern evolution.
    /// </summary>
    /// <returns>A Result indicating whether the evolution is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(PatternId))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Version))
            return Result.WithFailure("Version cannot be null or empty");

        if (string.IsNullOrWhiteSpace(ChangeDescription))
            return Result.WithFailure("Change description cannot be null or empty");

        return Result.Success();
    }
}