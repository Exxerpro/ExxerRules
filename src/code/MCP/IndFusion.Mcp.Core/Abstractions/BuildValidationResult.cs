namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of build validation operations.
/// </summary>
/// <param name="IsValid">Whether the build is valid.</param>
/// <param name="BuildSuccess">Whether the build succeeded.</param>
/// <param name="ValidationChecks">Results of individual validation checks.</param>
/// <param name="NewIssues">New issues introduced by the transformation.</param>
/// <param name="AnalyzerResults">Results from analyzer validation.</param>
/// <param name="ValidationTimeMs">Time taken for validation.</param>
public record BuildValidationResult(
    bool IsValid,
    bool BuildSuccess,
    IEnumerable<ValidationCheck> ValidationChecks,
    IEnumerable<TransformationIssue> NewIssues,
    IEnumerable<AnalyzerResult> AnalyzerResults,
    long ValidationTimeMs
);