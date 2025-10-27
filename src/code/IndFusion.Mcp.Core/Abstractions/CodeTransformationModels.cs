namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for applying Fixer001 transformations.
/// </summary>
/// <param name="DiagnosticId">ID of the diagnostic to fix.</param>
/// <param name="TargetFiles">Files to apply the fix to.</param>
/// <param name="ValidationOptions">Options for validating the transformation.</param>
/// <param name="DryRun">Whether to perform a dry run without applying changes.</param>
/// <param name="BackupOriginal">Whether to backup original files.</param>
/// <param name="MaxFixesPerFile">Maximum number of fixes to apply per file.</param>
public record Fixer001Request(
    string DiagnosticId,
    IEnumerable<string> TargetFiles,
    TransformationValidationOptions ValidationOptions,
    bool DryRun = false,
    bool BackupOriginal = true,
    int MaxFixesPerFile = 10
);

/// <summary>
/// Request for safe regex transformations.
/// </summary>
/// <param name="Pattern">Regex pattern to match.</param>
/// <param name="Replacement">Replacement text.</param>
/// <param name="TargetFiles">Files to apply the transformation to.</param>
/// <param name="ValidationOptions">Options for validating the transformation.</param>
/// <param name="DryRun">Whether to perform a dry run.</param>
/// <param name="CaseSensitive">Whether the pattern matching is case sensitive.</param>
/// <param name="Multiline">Whether to use multiline matching.</param>
public record SafeRegexRequest(
    string Pattern,
    string Replacement,
    IEnumerable<string> TargetFiles,
    TransformationValidationOptions ValidationOptions,
    bool DryRun = false,
    bool CaseSensitive = true,
    bool Multiline = false
);

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

/// <summary>
/// Request for semantic change review.
/// </summary>
/// <param name="OriginalCode">Original code version.</param>
/// <param name="ModifiedCode">Modified code version.</param>
/// <param name="ReviewOptions">Options for the review process.</param>
/// <param name="IncludeDiff">Whether to include detailed diff information.</param>
/// <param name="CheckSemanticDrift">Whether to check for semantic drift.</param>
public record SemanticChangeReviewRequest(
    string OriginalCode,
    string ModifiedCode,
    ChangeReviewOptions ReviewOptions,
    bool IncludeDiff = true,
    bool CheckSemanticDrift = true
);

/// <summary>
/// Result of code transformation operations.
/// </summary>
/// <param name="Success">Whether the transformation succeeded.</param>
/// <param name="TransformationDetails">Details about the transformation.</param>
/// <param name="ValidationResults">Results of validation checks.</param>
/// <param name="DiffPreview">Preview of changes made.</param>
/// <param name="ModifiedFiles">Files that were modified.</param>
/// <param name="ExecutionTimeMs">Time taken for the transformation.</param>
/// <param name="ErrorDetails">Error details if transformation failed.</param>
public record CodeTransformationResult(
    bool Success,
    TransformationDetails TransformationDetails,
    IEnumerable<ValidationResult> ValidationResults,
    string? DiffPreview = null,
    IEnumerable<string>? ModifiedFiles = null,
    long ExecutionTimeMs = 0,
    string? ErrorDetails = null
);

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

/// <summary>
/// Result of semantic change review.
/// </summary>
/// <param name="Success">Whether the review succeeded.</param>
/// <param name="SemanticDiff">Semantic differences found.</param>
/// <param name="DriftAnalysis">Analysis of semantic drift.</param>
/// <param name="FixSuggestions">Suggestions for fixing issues.</param>
/// <param name="ConfidenceScore">Confidence in the analysis (0.0-1.0).</param>
/// <param name="ReviewTimeMs">Time taken for the review.</param>
/// <param name="ErrorDetails">Error details if review failed.</param>
public record SemanticChangeReviewResult(
    bool Success,
    SemanticDiffAnalysis SemanticDiff,
    DriftAnalysis DriftAnalysis,
    IEnumerable<FixSuggestion> FixSuggestions,
    double ConfidenceScore,
    long ReviewTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Configuration for Fixer001.
/// </summary>
/// <param name="SolutionPath">Path to the solution file.</param>
/// <param name="AvailableTransformations">Available transformations.</param>
/// <param name="DefaultSettings">Default settings for transformations.</param>
/// <param name="Version">Version of Fixer001.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record Fixer001Configuration(
    string SolutionPath,
    IEnumerable<TransformationInfo> AvailableTransformations,
    Dictionary<string, object> DefaultSettings,
    string Version,
    DateTime LastUpdated
);

/// <summary>
/// Options for transformation validation.
/// </summary>
/// <param name="RunAnalyzers">Whether to run analyzers.</param>
/// <param name="BuildValidation">Whether to perform build validation.</param>
/// <param name="CheckForNewIssues">Whether to check for new issues.</param>
/// <param name="TimeoutMs">Timeout for validation in milliseconds.</param>
/// <param name="SeverityThreshold">Minimum severity threshold for issues.</param>
public record TransformationValidationOptions(
    bool RunAnalyzers = true,
    bool BuildValidation = true,
    bool CheckForNewIssues = true,
    int TimeoutMs = 30000,
    string SeverityThreshold = "Warning"
);

/// <summary>
/// Criteria for validation.
/// </summary>
/// <param name="MaxNewIssues">Maximum number of new issues allowed.</param>
/// <param name="SeverityThreshold">Minimum severity threshold.</param>
/// <param name="RequiredChecks">Required validation checks.</param>
/// <param name="CustomRules">Custom validation rules.</param>
public record ValidationCriteria(
    int MaxNewIssues = 0,
    string SeverityThreshold = "Error",
    IEnumerable<string> RequiredChecks = null!,
    Dictionary<string, object> CustomRules = null!
);

/// <summary>
/// Options for change review.
/// </summary>
/// <param name="IncludeMetrics">Whether to include code metrics.</param>
/// <param name="CheckPerformance">Whether to check for performance impact.</param>
/// <param name="CheckSecurity">Whether to check for security issues.</param>
/// <param name="CheckMaintainability">Whether to check for maintainability.</param>
public record ChangeReviewOptions(
    bool IncludeMetrics = true,
    bool CheckPerformance = true,
    bool CheckSecurity = true,
    bool CheckMaintainability = true
);

/// <summary>
/// Details about a transformation.
/// </summary>
/// <param name="TransformationType">Type of transformation applied.</param>
/// <param name="TransformationId">Unique identifier for the transformation.</param>
/// <param name="Description">Description of the transformation.</param>
/// <param name="ChangesApplied">Number of changes applied.</param>
/// <param name="FilesAffected">Number of files affected.</param>
/// <param name="Confidence">Confidence in the transformation (0.0-1.0).</param>
public record TransformationDetails(
    string TransformationType,
    string TransformationId,
    string Description,
    int ChangesApplied,
    int FilesAffected,
    double Confidence
);

/// <summary>
/// Result of a validation check.
/// </summary>
/// <param name="CheckName">Name of the validation check.</param>
/// <param name="Passed">Whether the check passed.</param>
/// <param name="Message">Message describing the result.</param>
/// <param name="Details">Additional details about the check.</param>
public record ValidationResult(
    string CheckName,
    bool Passed,
    string Message,
    Dictionary<string, object> Details
);

/// <summary>
/// Individual validation check result.
/// </summary>
/// <param name="CheckType">Type of validation check.</param>
/// <param name="Status">Status of the check (Pass, Fail, Warning).</param>
/// <param name="Message">Message describing the result.</param>
/// <param name="Severity">Severity of any issues found.</param>
public record ValidationCheck(
    string CheckType,
    string Status,
    string Message,
    string Severity
);

/// <summary>
/// Issue introduced by a transformation.
/// </summary>
/// <param name="IssueId">Unique identifier for the issue.</param>
/// <param name="IssueType">Type of issue.</param>
/// <param name="Severity">Severity of the issue.</param>
/// <param name="Message">Description of the issue.</param>
/// <param name="Location">Location of the issue.</param>
/// <param name="SuggestedFix">Suggested fix for the issue.</param>
public record TransformationIssue(
    string IssueId,
    string IssueType,
    string Severity,
    string Message,
    SourceLocation Location,
    string? SuggestedFix
);

/// <summary>
/// Result from analyzer validation.
/// </summary>
/// <param name="AnalyzerName">Name of the analyzer.</param>
/// <param name="IssuesFound">Number of issues found.</param>
/// <param name="SeverityDistribution">Distribution of issues by severity.</param>
/// <param name="ExecutionTimeMs">Time taken for analysis.</param>
public record AnalyzerResult(
    string AnalyzerName,
    int IssuesFound,
    Dictionary<string, int> SeverityDistribution,
    long ExecutionTimeMs
);

/// <summary>
/// Semantic diff analysis.
/// </summary>
/// <param name="StructuralChanges">Structural changes detected.</param>
/// <param name="BehavioralChanges">Behavioral changes detected.</param>
/// <param name="ImpactAnalysis">Analysis of the impact of changes.</param>
/// <param name="ConfidenceScore">Confidence in the analysis (0.0-1.0).</param>
public record SemanticDiffAnalysis(
    IEnumerable<StructuralChange> StructuralChanges,
    IEnumerable<BehavioralChange> BehavioralChanges,
    ImpactAnalysis ImpactAnalysis,
    double ConfidenceScore
);

/// <summary>
/// Analysis of semantic drift.
/// </summary>
/// <param name="DriftDetected">Whether drift was detected.</param>
/// <param name="DriftType">Type of drift detected.</param>
/// <param name="DriftSeverity">Severity of the drift.</param>
/// <param name="AffectedAreas">Areas affected by the drift.</param>
/// <param name="Recommendations">Recommendations to address drift.</param>
public record DriftAnalysis(
    bool DriftDetected,
    string DriftType,
    string DriftSeverity,
    IEnumerable<string> AffectedAreas,
    IEnumerable<string> Recommendations
);

/// <summary>
/// Suggestion for fixing issues.
/// </summary>
/// <param name="SuggestionId">Unique identifier for the suggestion.</param>
/// <param name="SuggestionType">Type of suggestion.</param>
/// <param name="Description">Description of the suggestion.</param>
/// <param name="CodeExample">Example code showing the fix.</param>
/// <param name="Confidence">Confidence in the suggestion (0.0-1.0).</param>
/// <param name="Effort">Estimated effort to implement.</param>
public record FixSuggestion(
    string SuggestionId,
    string SuggestionType,
    string Description,
    string? CodeExample,
    double Confidence,
    string Effort
);

/// <summary>
/// Information about a transformation.
/// </summary>
/// <param name="Id">Unique identifier for the transformation.</param>
/// <param name="Name">Name of the transformation.</param>
/// <param name="Description">Description of what the transformation does.</param>
/// <param name="SupportedLanguages">Programming languages supported.</param>
/// <param name="IsEnabled">Whether the transformation is enabled.</param>
/// <param name="Parameters">Parameters required for the transformation.</param>
public record TransformationInfo(
    string Id,
    string Name,
    string Description,
    IEnumerable<string> SupportedLanguages,
    bool IsEnabled,
    Dictionary<string, object> Parameters
);

/// <summary>
/// Structural change detected in code.
/// </summary>
/// <param name="ChangeType">Type of structural change.</param>
/// <param name="ElementName">Name of the changed element.</param>
/// <param name="ChangeDescription">Description of the change.</param>
/// <param name="Impact">Impact of the change.</param>
public record StructuralChange(
    string ChangeType,
    string ElementName,
    string ChangeDescription,
    string Impact
);

/// <summary>
/// Behavioral change detected in code.
/// </summary>
/// <param name="BehaviorType">Type of behavioral change.</param>
/// <param name="ChangeDescription">Description of the behavioral change.</param>
/// <param name="Impact">Impact of the behavioral change.</param>
/// <param name="Confidence">Confidence in detecting the change (0.0-1.0).</param>
public record BehavioralChange(
    string BehaviorType,
    string ChangeDescription,
    string Impact,
    double Confidence
);

/// <summary>
/// Analysis of the impact of changes.
/// </summary>
/// <param name="OverallImpact">Overall impact assessment.</param>
/// <param name="AffectedComponents">Components affected by changes.</param>
/// <param name="RiskLevel">Risk level of the changes.</param>
/// <param name="MitigationStrategies">Strategies to mitigate risks.</param>
public record ImpactAnalysis(
    string OverallImpact,
    IEnumerable<string> AffectedComponents,
    string RiskLevel,
    IEnumerable<string> MitigationStrategies
);