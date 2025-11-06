namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for code pattern analysis operations.
/// </summary>
/// <param name="CodeSnippet">Code snippet to analyze.</param>
/// <param name="AnalysisType">Type of analysis to perform.</param>
/// <param name="Context">Additional context for analysis.</param>
/// <param name="IncludeSuggestions">Whether to include improvement suggestions.</param>
/// <param name="ConfidenceThreshold">Minimum confidence threshold for results.</param>
public record CodePatternAnalysisRequest(
    string CodeSnippet,
    string AnalysisType,
    Dictionary<string, object>? Context = null,
    bool IncludeSuggestions = true,
    double ConfidenceThreshold = 0.8
);