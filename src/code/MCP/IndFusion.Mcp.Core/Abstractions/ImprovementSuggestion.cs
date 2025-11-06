namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// An improvement suggestion for code.
/// </summary>
/// <param name="Id">Unique identifier for the suggestion.</param>
/// <param name="SuggestionType">Type of suggestion.</param>
/// <param name="Description">Description of the suggestion.</param>
/// <param name="CodeExample">Example code showing the improvement.</param>
/// <param name="Confidence">Confidence in the suggestion (0.0-1.0).</param>
/// <param name="Effort">Estimated effort to implement.</param>
/// <param name="Benefits">Benefits of implementing the suggestion.</param>
public record ImprovementSuggestion(
    string Id,
    string SuggestionType,
    string Description,
    string? CodeExample,
    double Confidence,
    string Effort,
    IEnumerable<string> Benefits
);