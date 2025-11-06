namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Details about a Fixer001 preview.
/// </summary>
/// <param name="PreviewId">Unique identifier for the preview.</param>
/// <param name="DiagnosticId">ID of the diagnostic that would be fixed.</param>
/// <param name="EstimatedFixes">Estimated number of fixes.</param>
/// <param name="ReadinessAssessment">Readiness assessment of the transformation.</param>
public record Fixer001PreviewDetails(
    string PreviewId,
    string DiagnosticId,
    int EstimatedFixes,
    string ReadinessAssessment
);