namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Details about a Fixer001 transformation.
/// </summary>
/// <param name="TransformationType">Type of transformation applied.</param>
/// <param name="TransformationId">Unique identifier for the transformation.</param>
/// <param name="Description">Description of the transformation.</param>
/// <param name="ChangesApplied">Number of changes applied.</param>
/// <param name="FilesAffected">Number of files affected.</param>
/// <param name="Confidence">Confidence in the transformation (0.0-1.0).</param>
/// <param name="DiagnosticId">ID of the diagnostic being fixed.</param>
/// <param name="FixerVersion">Version of the fixer used.</param>
public record Fixer001TransformationDetails(
    string TransformationType,
    string TransformationId,
    string Description,
    int ChangesApplied,
    int FilesAffected,
    double Confidence,
    string DiagnosticId,
    string FixerVersion
);