namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of pattern analysis operations.
/// </summary>
/// <param name="Success">Whether the analysis succeeded.</param>
/// <param name="PatternAlignment">Pattern alignment analysis results.</param>
/// <param name="ImprovementSuggestions">Suggestions for pattern improvements.</param>
/// <param name="Metrics">Pattern metrics and statistics.</param>
/// <param name="Report">Detailed analysis report if requested.</param>
/// <param name="ExecutionTimeMs">Time taken for analysis.</param>
/// <param name="ErrorDetails">Error details if analysis failed.</param>
public record PatternAnalysisResult(
    bool Success,
    PatternAlignmentAnalysis PatternAlignment,
    IEnumerable<PatternImprovementSuggestion> ImprovementSuggestions,
    PatternMetrics Metrics,
    string? Report = null,
    long ExecutionTimeMs = 0,
    string? ErrorDetails = null
);