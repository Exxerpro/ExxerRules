namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Details about a safe regex transformation.
/// </summary>
/// <param name="TransformationType">Type of transformation applied.</param>
/// <param name="TransformationId">Unique identifier for the transformation.</param>
/// <param name="Description">Description of the transformation.</param>
/// <param name="ChangesApplied">Number of changes applied.</param>
/// <param name="FilesAffected">Number of files affected.</param>
/// <param name="Confidence">Confidence in the transformation (0.0-1.0).</param>
/// <param name="Pattern">Regex pattern used.</param>
/// <param name="Replacement">Replacement text used.</param>
public record SafeRegexTransformationDetails(
    string TransformationType,
    string TransformationId,
    string Description,
    int ChangesApplied,
    int FilesAffected,
    double Confidence,
    string Pattern,
    string Replacement
);