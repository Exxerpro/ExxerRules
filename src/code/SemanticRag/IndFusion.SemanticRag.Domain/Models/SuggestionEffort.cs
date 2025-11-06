namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the effort required to implement a suggestion.
/// </summary>
public enum SuggestionEffort
{
    /// <summary>
    /// Low effort - quick fix.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Medium effort - moderate changes required.
    /// </summary>
    Medium = 1,

    /// <summary>
    /// High effort - significant changes required.
    /// </summary>
    High = 2,

    /// <summary>
    /// Very high effort - major refactoring required.
    /// </summary>
    VeryHigh = 3
}