namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Pattern insight derived from graph analysis.
/// </summary>
/// <param name="InsightType">Type of insight.</param>
/// <param name="Description">Description of the insight.</param>
/// <param name="Confidence">Confidence in the insight (0.0-1.0).</param>
/// <param name="SupportingEvidence">Evidence supporting the insight.</param>
/// <param name="Recommendations">Recommendations based on the insight.</param>
public record PatternInsight(
    string InsightType,
    string Description,
    double Confidence,
    IEnumerable<string> SupportingEvidence,
    IEnumerable<string> Recommendations
);