namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Details about a safe regex preview.
/// </summary>
/// <param name="PreviewId">Unique identifier for the preview.</param>
/// <param name="Pattern">Regex pattern that would be used.</param>
/// <param name="Replacement">Replacement text that would be used.</param>
/// <param name="EstimatedMatches">Estimated number of matches.</param>
/// <param name="SafetyAssessment">Safety assessment of the transformation.</param>
public record SafeRegexPreviewDetails(
    string PreviewId,
    string Pattern,
    string Replacement,
    int EstimatedMatches,
    string SafetyAssessment
);