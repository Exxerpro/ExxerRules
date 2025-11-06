namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Criteria for validation.
/// </summary>
/// <param name="MaxNewIssues">Maximum number of new issues allowed.</param>
/// <param name="SeverityThreshold">Minimum severity threshold.</param>
/// <param name="RequiredChecks">Required validation checks.</param>
/// <param name="CustomRules">Custom validation rules.</param>
/// <param name="CheckSyntax">Whether to check syntax validity.</param>
/// <param name="CheckBuild">Whether to check build success.</param>
/// <param name="CheckAnalyzers">Whether to run analyzer checks.</param>
/// <param name="MaxIssues">Maximum number of issues allowed (alias for MaxNewIssues for backward compatibility).</param>
public record ValidationCriteria(
    int MaxNewIssues = 0,
    string SeverityThreshold = "Error",
    IEnumerable<string> RequiredChecks = null!,
    Dictionary<string, object> CustomRules = null!,
    bool CheckSyntax = true,
    bool CheckBuild = true,
    bool CheckAnalyzers = true,
    int MaxIssues = 0
)
{
    /// <summary>
    /// Gets the maximum issues count, using MaxIssues if specified, otherwise MaxNewIssues.
    /// </summary>
    public int MaxIssuesCount => MaxIssues > 0 ? MaxIssues : MaxNewIssues;
}