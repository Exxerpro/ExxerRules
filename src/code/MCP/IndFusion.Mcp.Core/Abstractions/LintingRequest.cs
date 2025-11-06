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