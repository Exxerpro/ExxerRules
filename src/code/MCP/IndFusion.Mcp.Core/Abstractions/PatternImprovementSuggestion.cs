namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Improvement suggestion for pattern alignment.
/// </summary>
/// <param name="Type">Type of improvement suggestion.</param>
/// <param name="Description">Description of the improvement.</param>
/// <param name="Priority">Priority level (Low, Medium, High).</param>
/// <param name="EstimatedEffort">Estimated effort to implement.</param>
/// <param name="ExpectedBenefit">Expected benefit from the improvement.</param>
public record PatternImprovementSuggestion(
    string Type,
    string Description,
    string Priority,
    string EstimatedEffort,
    string ExpectedBenefit
);