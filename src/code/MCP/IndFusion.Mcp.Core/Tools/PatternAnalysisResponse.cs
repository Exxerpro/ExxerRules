using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for pattern analysis operations.
/// </summary>
/// <param name="Analysis">The pattern analysis result.</param>
/// <param name="Success">Whether the analysis was successful.</param>
/// <param name="ErrorMessage">Error message if analysis failed.</param>
public readonly record struct PatternAnalysisResponse(
    PatternAnalysis Analysis,
    bool Success,
    string? ErrorMessage);