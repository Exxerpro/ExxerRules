namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// An issue from external linting.
/// </summary>
/// <param name="IssueId">Unique identifier for the issue.</param>
/// <param name="Severity">Severity of the issue.</param>
/// <param name="Message">Description of the issue.</param>
/// <param name="Location">Location of the issue.</param>
/// <param name="RuleId">ID of the rule that generated the issue.</param>
/// <param name="SuggestedFix">Suggested fix for the issue.</param>
public record ExternalLintingIssue(
    string IssueId,
    string Severity,
    string Message,
    SourceLocation Location,
    string RuleId,
    string? SuggestedFix
);