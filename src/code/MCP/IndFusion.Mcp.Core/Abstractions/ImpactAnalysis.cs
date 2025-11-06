namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Analysis of the impact of changes.
/// </summary>
/// <param name="OverallImpact">Overall impact assessment.</param>
/// <param name="AffectedComponents">Components affected by changes.</param>
/// <param name="RiskLevel">Risk level of the changes.</param>
/// <param name="MitigationStrategies">Strategies to mitigate risks.</param>
public record ImpactAnalysis(
    string OverallImpact,
    IEnumerable<string> AffectedComponents,
    string RiskLevel,
    IEnumerable<string> MitigationStrategies
);