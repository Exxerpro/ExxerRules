namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Pattern metrics and statistics.
/// </summary>
/// <param name="TotalPatterns">Total number of patterns found.</param>
/// <param name="PatternDistribution">Distribution of patterns by type.</param>
/// <param name="ComplianceScore">Overall compliance score.</param>
/// <param name="QualityMetrics">Quality metrics for patterns.</param>
public record PatternMetrics(
    int TotalPatterns,
    Dictionary<string, int> PatternDistribution,
    double ComplianceScore,
    Dictionary<string, double> QualityMetrics
);