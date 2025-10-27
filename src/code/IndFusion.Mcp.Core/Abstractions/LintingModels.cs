namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for running linting operations on a solution.
/// </summary>
/// <param name="SolutionPath">Absolute path to the target solution file (.sln).</param>
/// <param name="Scope">Optional file or directory scope within the solution.</param>
/// <param name="SeverityFilter">Minimum severity level to include (Error, Warning, Info, Hint).</param>
/// <param name="RuleIds">Optional specific rule IDs to check, if null checks all rules.</param>
/// <param name="IncludePolicyRecommendations">Whether to include policy recommendations for violations.</param>
/// <param name="OutputFormat">Desired output format (Json, Xml, Text).</param>
public record LintingRequest(
    string SolutionPath,
    string? Scope = null,
    string SeverityFilter = "Warning",
    IEnumerable<string>? RuleIds = null,
    bool IncludePolicyRecommendations = true,
    string OutputFormat = "Json"
);

/// <summary>
/// Request for starting a linting watcher.
/// </summary>
/// <param name="SolutionPath">Absolute path to the target solution file (.sln).</param>
/// <param name="WatchPatterns">File patterns to watch for changes.</param>
/// <param name="DebounceMs">Debounce delay in milliseconds before triggering linting.</param>
/// <param name="AutoFix">Whether to automatically apply fixes when possible.</param>
/// <param name="NotificationEndpoint">Optional endpoint to receive violation notifications.</param>
public record LintingWatchRequest(
    string SolutionPath,
    IEnumerable<string> WatchPatterns,
    int DebounceMs = 1000,
    bool AutoFix = false,
    string? NotificationEndpoint = null
);

/// <summary>
/// Result of a linting operation containing violations and policy recommendations.
/// </summary>
/// <param name="Success">Indicates whether the operation completed successfully.</param>
/// <param name="Violations">List of detected violations with details and policy recommendations.</param>
/// <param name="Summary">Summary statistics of the linting results.</param>
/// <param name="PolicyDecisions">Policy decisions made during the linting process.</param>
/// <param name="ExecutionTimeMs">Time taken to execute the linting operation in milliseconds.</param>
/// <param name="ErrorDetails">Error details if the operation failed.</param>
public record LintingResult(
    bool Success,
    IEnumerable<LintingViolation> Violations,
    LintingSummary Summary,
    IEnumerable<PolicyDecision> PolicyDecisions,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Represents a single linting violation with details and remediation suggestions.
/// </summary>
/// <param name="RuleId">The EXXER rule identifier that was violated.</param>
/// <param name="Severity">Severity level of the violation.</param>
/// <param name="Message">Human-readable description of the violation.</param>
/// <param name="FilePath">Path to the file containing the violation.</param>
/// <param name="Line">Line number where the violation occurs (1-based).</param>
/// <param name="Column">Column number where the violation occurs (1-based).</param>
/// <param name="CodeSnippet">Code snippet around the violation location.</param>
/// <param name="PolicyRecommendation">Policy recommendation for this violation.</param>
/// <param name="RemediationSuggestions">Suggested fixes for the violation.</param>
/// <param name="ConfidenceScore">Confidence score for the violation detection (0.0-1.0).</param>
public record LintingViolation(
    string RuleId,
    string Severity,
    string Message,
    string FilePath,
    int Line,
    int Column,
    string CodeSnippet,
    PolicyRecommendation PolicyRecommendation,
    IEnumerable<RemediationSuggestion> RemediationSuggestions,
    double ConfidenceScore
);

/// <summary>
/// Summary statistics for linting results.
/// </summary>
/// <param name="TotalViolations">Total number of violations found.</param>
/// <param name="ErrorCount">Number of error-level violations.</param>
/// <param name="WarningCount">Number of warning-level violations.</param>
/// <param name="InfoCount">Number of info-level violations.</param>
/// <param name="HintCount">Number of hint-level violations.</param>
/// <param name="FilesAnalyzed">Number of files analyzed.</param>
/// <param name="RulesChecked">Number of rules that were checked.</param>
public record LintingSummary(
    int TotalViolations,
    int ErrorCount,
    int WarningCount,
    int InfoCount,
    int HintCount,
    int FilesAnalyzed,
    int RulesChecked
);

/// <summary>
/// Policy recommendation for a specific violation.
/// </summary>
/// <param name="Action">Recommended action (Fix, Suppress, Ignore, Escalate).</param>
/// <param name="Reason">Reason for the recommendation.</param>
/// <param name="Confidence">Confidence in the recommendation (0.0-1.0).</param>
/// <param name="AutoFixable">Whether this violation can be automatically fixed.</param>
/// <param name="EstimatedEffort">Estimated effort to fix (Low, Medium, High).</param>
public record PolicyRecommendation(
    string Action,
    string Reason,
    double Confidence,
    bool AutoFixable,
    string EstimatedEffort
);

/// <summary>
/// Policy decision made during linting process.
/// </summary>
/// <param name="RuleId">Rule ID for which the decision was made.</param>
/// <param name="Decision">Decision made (Enforce, Suppress, Custom).</param>
/// <param name="Reason">Reason for the decision.</param>
/// <param name="Timestamp">When the decision was made.</param>
public record PolicyDecision(
    string RuleId,
    string Decision,
    string Reason,
    DateTime Timestamp
);

/// <summary>
/// Remediation suggestion for fixing a violation.
/// </summary>
/// <param name="Type">Type of suggestion (CodeFix, Refactor, Documentation, Suppression).</param>
/// <param name="Description">Description of the suggested fix.</param>
/// <param name="CodeExample">Example code showing the fix.</param>
/// <param name="Confidence">Confidence in the suggestion (0.0-1.0).</param>
/// <param name="Effort">Estimated effort to implement (Low, Medium, High).</param>
public record RemediationSuggestion(
    string Type,
    string Description,
    string? CodeExample,
    double Confidence,
    string Effort
);

/// <summary>
/// Linting policy configuration for a solution.
/// </summary>
/// <param name="SolutionPath">Path to the solution file.</param>
/// <param name="RuleSeverities">Severity settings for specific rules.</param>
/// <param name="GlobalSettings">Global policy settings.</param>
/// <param name="LastUpdated">When the policy was last updated.</param>
/// <param name="Version">Policy version identifier.</param>
public record LintingPolicy(
    string SolutionPath,
    Dictionary<string, string> RuleSeverities,
    Dictionary<string, object> GlobalSettings,
    DateTime LastUpdated,
    string Version
);