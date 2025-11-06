using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for pattern suggestion operations.
/// </summary>
/// <param name="Suggestions">The pattern suggestions.</param>
/// <param name="TotalSuggestions">Total number of suggestions.</param>
/// <param name="AnalysisTimeMs">Time taken for analysis in milliseconds.</param>
/// <param name="ConfidenceThreshold">Confidence threshold used.</param>
/// <param name="CategoriesFiltered">Number of categories filtered.</param>
public readonly record struct PatternSuggestionResponse(
    IReadOnlyList<PatternSuggestion> Suggestions,
    int TotalSuggestions,
    long AnalysisTimeMs,
    float ConfidenceThreshold,
    int CategoriesFiltered);