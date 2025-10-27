namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for analyzer execution.
/// </summary>
/// <param name="RuleIds">Specific rule IDs to check, if null checks all rules.</param>
/// <param name="SeverityFilter">Minimum severity level to include.</param>
/// <param name="IncludeSuppressed">Whether to include suppressed diagnostics.</param>
/// <param name="MaxDiagnostics">Maximum number of diagnostics to return.</param>
/// <param name="TimeoutMs">Timeout for analyzer execution in milliseconds.</param>
public record AnalyzerExecutionOptions(
    IEnumerable<string>? RuleIds = null,
    string SeverityFilter = "Warning",
    bool IncludeSuppressed = false,
    int MaxDiagnostics = 1000,
    int TimeoutMs = 30000
);

/// <summary>
/// Diagnostic from an analyzer.
/// </summary>
/// <param name="Id">Unique identifier for the diagnostic.</param>
/// <param name="RuleId">Rule ID that generated the diagnostic.</param>
/// <param name="Severity">Severity level of the diagnostic.</param>
/// <param name="Message">Diagnostic message.</param>
/// <param name="FilePath">Path to the file containing the issue.</param>
/// <param name="Line">Line number (1-based).</param>
/// <param name="Column">Column number (1-based).</param>
/// <param name="CodeSnippet">Code snippet around the issue.</param>
/// <param name="IsSuppressed">Whether the diagnostic is suppressed.</param>
/// <param name="Properties">Additional diagnostic properties.</param>
public record AnalyzerDiagnostic(
    string Id,
    string RuleId,
    string Severity,
    string Message,
    string FilePath,
    int Line,
    int Column,
    string CodeSnippet,
    bool IsSuppressed,
    Dictionary<string, object> Properties
);

/// <summary>
/// Options for applying code fixes.
/// </summary>
/// <param name="DryRun">Whether to perform a dry run without applying changes.</param>
/// <param name="ValidateAfterFix">Whether to validate fixes after application.</param>
/// <param name="BackupOriginal">Whether to backup original files.</param>
/// <param name="MaxFixesPerFile">Maximum number of fixes to apply per file.</param>
/// <param name="TimeoutMs">Timeout for fix application in milliseconds.</param>
public record CodeFixOptions(
    bool DryRun = false,
    bool ValidateAfterFix = true,
    bool BackupOriginal = true,
    int MaxFixesPerFile = 10,
    int TimeoutMs = 60000
);

/// <summary>
/// Result of applying code fixes.
/// </summary>
/// <param name="Success">Whether fixes were applied successfully.</param>
/// <param name="AppliedFixes">Number of fixes applied.</param>
/// <param name="ModifiedFiles">Files that were modified.</param>
/// <param name="ValidationResults">Results of post-fix validation.</param>
/// <param name="ErrorDetails">Error details if application failed.</param>
public record CodeFixResult(
    bool Success,
    int AppliedFixes,
    IEnumerable<string> ModifiedFiles,
    IEnumerable<FixValidationResult> ValidationResults,
    string? ErrorDetails = null
);

/// <summary>
/// Options for validating fixes.
/// </summary>
/// <param name="RunAnalyzers">Whether to run analyzers for validation.</param>
/// <param name="BuildValidation">Whether to perform build validation.</param>
/// <param name="CheckForNewIssues">Whether to check for new issues introduced.</param>
/// <param name="TimeoutMs">Timeout for validation in milliseconds.</param>
public record FixValidationOptions(
    bool RunAnalyzers = true,
    bool BuildValidation = true,
    bool CheckForNewIssues = true,
    int TimeoutMs = 30000
);

/// <summary>
/// Result of fix validation.
/// </summary>
/// <param name="IsValid">Whether the fixes are valid.</param>
/// <param name="NewIssues">New issues introduced by the fixes.</param>
/// <param name="BuildSuccess">Whether the code builds successfully.</param>
/// <param name="ValidationDetails">Detailed validation information.</param>
public record FixValidationResult(
    bool IsValid,
    IEnumerable<AnalyzerDiagnostic> NewIssues,
    bool BuildSuccess,
    Dictionary<string, object> ValidationDetails
);

/// <summary>
/// Metadata about an analyzer.
/// </summary>
/// <param name="Id">Unique identifier for the analyzer.</param>
/// <param name="Name">Display name of the analyzer.</param>
/// <param name="Description">Description of what the analyzer does.</param>
/// <param name="Version">Version of the analyzer.</param>
/// <param name="SupportedLanguages">Programming languages supported.</param>
/// <param name="RuleCount">Number of rules in the analyzer.</param>
/// <param name="IsEnabled">Whether the analyzer is enabled.</param>
/// <param name="Properties">Additional analyzer properties.</param>
public record AnalyzerMetadata(
    string Id,
    string Name,
    string Description,
    string Version,
    IEnumerable<string> SupportedLanguages,
    int RuleCount,
    bool IsEnabled,
    Dictionary<string, object> Properties
);