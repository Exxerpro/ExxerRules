namespace IndFusion.Mcp.Core.Abstractions;

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