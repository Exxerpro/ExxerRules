namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Suggestion for fixing issues.
/// </summary>
/// <param name="SuggestionId">Unique identifier for the suggestion.</param>
/// <param name="SuggestionType">Type of suggestion.</param>
/// <param name="Description">Description of the suggestion.</param>
/// <param name="CodeExample">Example code showing the fix.</param>
/// <param name="Confidence">Confidence in the suggestion (0.0-1.0).</param>
/// <param name="Effort">Estimated effort to implement.</param>
public record FixSuggestion(
    string SuggestionId,
    string SuggestionType,
    string Description,
    string? CodeExample,
    double Confidence,
    string Effort
);