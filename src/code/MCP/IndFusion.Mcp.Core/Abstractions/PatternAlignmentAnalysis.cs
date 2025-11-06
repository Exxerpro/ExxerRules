namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Pattern alignment analysis results.
/// </summary>
/// <param name="OverallScore">Overall alignment score (0.0-1.0).</param>
/// <param name="PatternScores">Scores for individual patterns.</param>
/// <param name="AlignmentIssues">Issues found in pattern alignment.</param>
/// <param name="Recommendations">Recommendations for improvement.</param>
public record PatternAlignmentAnalysis(
    double OverallScore,
    Dictionary<string, double> PatternScores,
    IEnumerable<string> AlignmentIssues,
    IEnumerable<string> Recommendations
);