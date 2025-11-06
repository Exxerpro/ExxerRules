namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Remediation suggestion for fixing a violation.
/// </summary>
/// <param name="Type">Type of suggestion (CodeFix, Refactor, Documentation, Suppression).</param>
/// <param name="Description">Description of the suggested fix.</param>
/// <param name="CodeExample">Example code showing the fix.</param>
/// <param name="Confidence">Confidence in the suggestion (0.0-1.0).</param>
/// <param name="Effort">Estimated effort to implement (Low, Medium, High).</param>
public record RemediationSuggestion(
    string Type,
    string Description,
    string? CodeExample,
    double Confidence,
    string Effort
);