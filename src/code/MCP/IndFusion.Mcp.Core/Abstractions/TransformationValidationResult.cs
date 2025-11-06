namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of transformation validation.
/// </summary>
/// <param name="IsValid">Whether the transformation is valid.</param>
/// <param name="ValidationChecks">Results of individual validation checks.</param>
/// <param name="NewIssues">New issues introduced by the transformation.</param>
/// <param name="BuildSuccess">Whether the code builds successfully.</param>
/// <param name="AnalyzerResults">Results from analyzer validation.</param>
/// <param name="ValidationTimeMs">Time taken for validation.</param>
public record TransformationValidationResult(
    bool IsValid,
    IEnumerable<ValidationCheck> ValidationChecks,
    IEnumerable<TransformationIssue> NewIssues,
    bool BuildSuccess,
    IEnumerable<AnalyzerResult> AnalyzerResults,
    long ValidationTimeMs
);