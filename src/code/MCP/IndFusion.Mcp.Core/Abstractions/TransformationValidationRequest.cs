namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for validating code transformations.
/// </summary>
/// <param name="OriginalCode">Original code before transformation.</param>
/// <param name="TransformedCode">Code after transformation.</param>
/// <param name="ValidationCriteria">Criteria for validation.</param>
/// <param name="RunAnalyzers">Whether to run analyzers for validation.</param>
/// <param name="BuildValidation">Whether to perform build validation.</param>
/// <param name="CheckForNewIssues">Whether to check for new issues.</param>
public record TransformationValidationRequest(
    string OriginalCode,
    string TransformedCode,
    ValidationCriteria ValidationCriteria,
    bool RunAnalyzers = true,
    bool BuildValidation = true,
    bool CheckForNewIssues = true
);