namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of code pattern analysis operations.
/// </summary>
/// <param name="Success">Whether the analysis succeeded.</param>
/// <param name="Patterns">Patterns found in the code.</param>
/// <param name="Suggestions">Improvement suggestions.</param>
/// <param name="ConfidenceScores">Confidence scores for patterns.</param>
/// <param name="AnalysisTimeMs">Time taken for analysis.</param>
/// <param name="ErrorDetails">Error details if analysis failed.</param>
public record CodePatternAnalysisResult(
    bool Success,
    IEnumerable<CodePattern> Patterns,
    IEnumerable<ImprovementSuggestion> Suggestions,
    Dictionary<string, double> ConfidenceScores,
    long AnalysisTimeMs,
    string? ErrorDetails = null
);