namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result from analyzer validation.
/// </summary>
/// <param name="AnalyzerName">Name of the analyzer.</param>
/// <param name="IssuesFound">Number of issues found.</param>
/// <param name="SeverityDistribution">Distribution of issues by severity.</param>
/// <param name="ExecutionTimeMs">Time taken for analysis.</param>
public record AnalyzerResult(
    string AnalyzerName,
    int IssuesFound,
    Dictionary<string, int> SeverityDistribution,
    long ExecutionTimeMs
);