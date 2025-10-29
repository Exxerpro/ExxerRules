namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Diagnostic result from an analyzer.
/// </summary>
/// <param name="Id">Unique identifier for the diagnostic.</param>
/// <param name="RuleId">ID of the rule that generated this diagnostic.</param>
/// <param name="Severity">Severity level of the diagnostic.</param>
/// <param name="Message">Diagnostic message.</param>
/// <param name="FilePath">Path to the file where the issue was found.</param>
/// <param name="LineNumber">Line number where the issue was found.</param>
/// <param name="ColumnNumber">Column number where the issue was found.</param>
/// <param name="Properties">Additional diagnostic properties.</param>
public record AnalyzerDiagnostic(
    string Id,
    string RuleId,
    string Severity,
    string Message,
    string FilePath,
    int LineNumber,
    int ColumnNumber,
    Dictionary<string, object> Properties
);

/// <summary>
/// Options for analyzer execution.
/// </summary>
/// <param name="Analyzers">List of analyzer IDs to run.</param>
/// <param name="Severity">Minimum severity level to report.</param>
/// <param name="IncludeWarnings">Whether to include warnings.</param>
/// <param name="MaxConcurrency">Maximum number of concurrent analyzers.</param>
public record AnalyzerExecutionOptions(
    IEnumerable<string> Analyzers,
    string Severity = "Error",
    bool IncludeWarnings = true,
    int MaxConcurrency = 4
);

/// <summary>
/// Result of code fix operations.
/// </summary>
/// <param name="Id">Unique identifier for the fix result.</param>
/// <param name="OriginalCode">Original code before fixes.</param>
/// <param name="FixedCode">Code after applying fixes.</param>
/// <param name="AppliedFixes">List of fixes that were applied.</param>
/// <param name="Success">Whether the fixes were applied successfully.</param>
/// <param name="Errors">Any errors that occurred during fixing.</param>
public record CodeFixResult(
    string Id,
    string OriginalCode,
    string FixedCode,
    IEnumerable<string> AppliedFixes,
    bool Success,
    IEnumerable<string> Errors
);

/// <summary>
/// Options for code fix operations.
/// </summary>
/// <param name="FixIds">List of fix IDs to apply.</param>
/// <param name="DryRun">Whether to perform a dry run without applying fixes.</param>
/// <param name="MaxFixes">Maximum number of fixes to apply.</param>
public record CodeFixOptions(
    IEnumerable<string> FixIds,
    bool DryRun = false,
    int MaxFixes = 10
);

/// <summary>
/// Result of fix validation operations.
/// </summary>
/// <param name="Id">Unique identifier for the validation result.</param>
/// <param name="IsValid">Whether the fixes are valid.</param>
/// <param name="NewIssues">New issues introduced by the fixes.</param>
/// <param name="ResolvedIssues">Issues that were resolved by the fixes.</param>
/// <param name="ValidationErrors">Any validation errors.</param>
public record FixValidationResult(
    string Id,
    bool IsValid,
    IEnumerable<AnalyzerDiagnostic> NewIssues,
    IEnumerable<AnalyzerDiagnostic> ResolvedIssues,
    IEnumerable<string> ValidationErrors
);

/// <summary>
/// Options for fix validation operations.
/// <param name="ValidateSyntax">Whether to validate syntax.</param>
/// <param name="ValidateSemantics">Whether to validate semantics.</param>
/// <param name="RunAnalyzers">Whether to run analyzers for validation.</param>
/// </summary>
public record FixValidationOptions(
    bool ValidateSyntax = true,
    bool ValidateSemantics = true,
    bool RunAnalyzers = true
);

/// <summary>
/// Metadata about an analyzer.
/// </summary>
/// <param name="Id">Unique identifier for the analyzer.</param>
/// <param name="Name">Display name of the analyzer.</param>
/// <param name="Description">Description of what the analyzer does.</param>
/// <param name="Version">Version of the analyzer.</param>
/// <param name="SupportedLanguages">Languages supported by the analyzer.</param>
/// <param name="Rules">Rules provided by the analyzer.</param>
public record AnalyzerMetadata(
    string Id,
    string Name,
    string Description,
    string Version,
    IEnumerable<string> SupportedLanguages,
    IEnumerable<string> Rules
);