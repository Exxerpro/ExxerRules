namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of pattern suggestion operations.
/// </summary>
/// <param name="Success">Whether the operation succeeded.</param>
/// <param name="Suggestions">Collection of pattern suggestions.</param>
/// <param name="ConfidenceScores">Confidence scores for suggestions.</param>
/// <param name="Citations">Source citations for suggestions.</param>
/// <param name="ExecutionTimeMs">Time taken to generate suggestions.</param>
/// <param name="ErrorDetails">Error details if operation failed.</param>
public record PatternSuggestionResult(
    bool Success,
    IEnumerable<PatternSuggestion> Suggestions,
    Dictionary<string, double> ConfidenceScores,
    IEnumerable<PatternCitation> Citations,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);