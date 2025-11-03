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

/// <summary>
/// Result of Fixer001 transformation operations.
/// </summary>
/// <param name="Success">Whether the transformation succeeded.</param>
/// <param name="TransformationDetails">Details about the transformation.</param>
/// <param name="ValidationResults">Results of validation checks.</param>
/// <param name="DiffPreview">Preview of changes made.</param>
/// <param name="ModifiedFiles">Files that were modified.</param>
/// <param name="ExecutionTimeMs">Time taken for the transformation.</param>
/// <param name="ErrorDetails">Error details if transformation failed.</param>
public record Fixer001Result(
    bool Success,
    Fixer001TransformationDetails TransformationDetails,
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

/// <summary>
/// Result of regex pattern validation.
/// </summary>
/// <param name="IsValid">Whether the pattern is valid.</param>
/// <param name="SafetyScore">Safety score from 0.0 to 1.0.</param>
/// <param name="Issues">Issues found with the pattern.</param>
/// <param name="Warnings">Warnings about the pattern.</param>
/// <param name="PerformanceImpact">Estimated performance impact.</param>
/// <param name="ValidationTimeMs">Time taken for validation.</param>
public record RegexValidationResult(
    bool IsValid,
    double SafetyScore,
    IEnumerable<RegexIssue> Issues,
    IEnumerable<RegexWarning> Warnings,
    string PerformanceImpact,
    long ValidationTimeMs
);

/// <summary>
/// Result of safe regex preview operations.
/// </summary>
/// <param name="Success">Whether the preview succeeded.</param>
/// <param name="PreviewDetails">Details about the preview.</param>
/// <param name="EstimatedChanges">Estimated number of changes.</param>
/// <param name="AffectedFiles">Files that would be affected.</param>
/// <param name="PreviewTimeMs">Time taken for the preview.</param>
/// <param name="ErrorDetails">Error details if preview failed.</param>
public record SafeRegexPreviewResult(
    bool Success,
    SafeRegexPreviewDetails PreviewDetails,
    int EstimatedChanges,
    IEnumerable<string> AffectedFiles,
    long PreviewTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of Fixer001 preview operations.
/// </summary>
/// <param name="Success">Whether the preview succeeded.</param>
/// <param name="PreviewDetails">Details about the preview.</param>
/// <param name="EstimatedChanges">Estimated number of changes.</param>
/// <param name="AffectedFiles">Files that would be affected.</param>
/// <param name="PreviewTimeMs">Time taken for the preview.</param>
/// <param name="ErrorDetails">Error details if preview failed.</param>
public record Fixer001PreviewResult(
    bool Success,
    Fixer001PreviewDetails PreviewDetails,
    int EstimatedChanges,
    IEnumerable<string> AffectedFiles,
    long PreviewTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of Fixer001 validation operations.
/// </summary>
/// <param name="IsReady">Whether Fixer001 is ready to be applied.</param>
/// <param name="ReadinessScore">Readiness score from 0.0 to 1.0.</param>
/// <param name="Issues">Issues preventing application.</param>
/// <param name="Warnings">Warnings about the application.</param>
/// <param name="ValidationTimeMs">Time taken for validation.</param>
public record Fixer001ValidationResult(
    bool IsReady,
    double ReadinessScore,
    IEnumerable<Fixer001Issue> Issues,
    IEnumerable<Fixer001Warning> Warnings,
    long ValidationTimeMs
);

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

/// <summary>
/// Issue found with a regex pattern.
/// </summary>
/// <param name="IssueId">Unique identifier for the issue.</param>
/// <param name="IssueType">Type of issue.</param>
/// <param name="Severity">Severity of the issue.</param>
/// <param name="Message">Description of the issue.</param>
/// <param name="SuggestedFix">Suggested fix for the issue.</param>
public record RegexIssue(
    string IssueId,
    string IssueType,
    string Severity,
    string Message,
    string? SuggestedFix
);

/// <summary>
/// Warning about a regex pattern.
/// </summary>
/// <param name="WarningId">Unique identifier for the warning.</param>
/// <param name="WarningType">Type of warning.</param>
/// <param name="Message">Description of the warning.</param>
/// <param name="Recommendation">Recommendation for addressing the warning.</param>
public record RegexWarning(
    string WarningId,
    string WarningType,
    string Message,
    string? Recommendation
);

/// <summary>
/// Issue preventing Fixer001 application.
/// </summary>
/// <param name="IssueId">Unique identifier for the issue.</param>
/// <param name="IssueType">Type of issue.</param>
/// <param name="Severity">Severity of the issue.</param>
/// <param name="Message">Description of the issue.</param>
/// <param name="SuggestedFix">Suggested fix for the issue.</param>
public record Fixer001Issue(
    string IssueId,
    string IssueType,
    string Severity,
    string Message,
    string? SuggestedFix
);

/// <summary>
/// Warning about Fixer001 application.
/// </summary>
/// <param name="WarningId">Unique identifier for the warning.</param>
/// <param name="WarningType">Type of warning.</param>
/// <param name="Message">Description of the warning.</param>
/// <param name="Recommendation">Recommendation for addressing the warning.</param>
public record Fixer001Warning(
    string WarningId,
    string WarningType,
    string Message,
    string? Recommendation
);

/// <summary>
/// Request for build validation operations.
/// </summary>
/// <param name="SolutionPath">Path to the solution file.</param>
/// <param name="TransformedFiles">Files that have been transformed.</param>
/// <param name="ValidationOptions">Options for validation.</param>
/// <param name="RunAnalyzers">Whether to run analyzers for validation.</param>
/// <param name="BuildValidation">Whether to perform build validation.</param>
/// <param name="CheckForNewIssues">Whether to check for new issues.</param>
public record BuildValidationRequest(
    string SolutionPath,
    IEnumerable<TransformedFile> TransformedFiles,
    TransformationValidationOptions ValidationOptions,
    bool RunAnalyzers = true,
    bool BuildValidation = true,
    bool CheckForNewIssues = true
);

/// <summary>
/// Represents a transformed file.
/// </summary>
/// <param name="FilePath">Path to the file.</param>
/// <param name="OriginalContent">Original content of the file.</param>
/// <param name="TransformedContent">Transformed content of the file.</param>
/// <param name="TransformationType">Type of transformation applied.</param>
public record TransformedFile(
    string FilePath,
    string OriginalContent,
    string TransformedContent,
    string TransformationType
);

/// <summary>
/// Represents a temporary workspace for validation.
/// </summary>
/// <param name="WorkspacePath">Path to the temporary workspace.</param>
/// <param name="OriginalSolutionPath">Path to the original solution.</param>
/// <param name="CreatedAt">When the workspace was created.</param>
/// <param name="ExpiresAt">When the workspace expires.</param>
public record TemporaryWorkspace(
    string WorkspacePath,
    string OriginalSolutionPath,
    DateTime CreatedAt,
    DateTime ExpiresAt
);
