namespace IndFusion.Mcp.Core.Abstractions;

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