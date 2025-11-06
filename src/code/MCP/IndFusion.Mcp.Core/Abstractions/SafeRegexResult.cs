using IndFusion.Mcp.Core.Models.PatternGraph;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of safe regex transformation operations.
/// </summary>
/// <param name="Success">Whether the transformation succeeded.</param>
/// <param name="TransformationDetails">Details about the transformation.</param>
/// <param name="ValidationResults">Results of validation checks.</param>
/// <param name="DiffPreview">Preview of changes made.</param>
/// <param name="ModifiedFiles">Files that were modified.</param>
/// <param name="ExecutionTimeMs">Time taken for the transformation.</param>
/// <param name="ErrorDetails">Error details if transformation failed.</param>
public record SafeRegexResult(
    bool Success,
    SafeRegexTransformationDetails TransformationDetails,
    IEnumerable<ValidationResult> ValidationResults,
    string? DiffPreview = null,
    IEnumerable<string>? ModifiedFiles = null,
    long ExecutionTimeMs = 0,
    string? ErrorDetails = null
)
{
    /// <summary>
    /// Gets the number of changes applied by the transformation.
    /// </summary>
    public int ChangesApplied => TransformationDetails.ChangesApplied;

    /// <summary>
    /// Gets the number of files affected by the transformation.
    /// </summary>
    public int FilesAffected => TransformationDetails.FilesAffected;
};