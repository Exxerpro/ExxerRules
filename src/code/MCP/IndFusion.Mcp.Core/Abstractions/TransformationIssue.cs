namespace IndFusion.Mcp.Core.Abstractions;

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