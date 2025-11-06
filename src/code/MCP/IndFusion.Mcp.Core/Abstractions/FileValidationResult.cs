namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of file validation operations.
/// </summary>
/// <param name="IsValid">Whether the file transformation is valid.</param>
/// <param name="FilePath">Path to the validated file.</param>
/// <param name="ValidationChecks">Results of individual validation checks.</param>
/// <param name="NewIssues">New issues introduced by the transformation.</param>
/// <param name="ValidationTimeMs">Time taken for validation.</param>
public record FileValidationResult(
    bool IsValid,
    string FilePath,
    IEnumerable<ValidationCheck> ValidationChecks,
    IEnumerable<TransformationIssue> NewIssues,
    long ValidationTimeMs
);