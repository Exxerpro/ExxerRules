namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the impact of implementing a suggestion.
/// </summary>
public enum SuggestionImpact
{
    /// <summary>
    /// Low impact - minor improvement.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium impact - moderate improvement.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High impact - significant improvement.
    /// </summary>
    High = 2,

    /// <summary>
    /// Very high impact - major improvement.
    /// </summary>
    VeryHigh = 3
}