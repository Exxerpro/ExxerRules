namespace IndFusion.Mcp.Core.Abstractions;

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