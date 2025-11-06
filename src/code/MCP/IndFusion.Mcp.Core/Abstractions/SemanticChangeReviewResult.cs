namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of semantic change review.
/// </summary>
/// <param name="Success">Whether the review succeeded.</param>
/// <param name="SemanticDiff">Semantic differences found.</param>
/// <param name="DriftAnalysis">Analysis of semantic drift.</param>
/// <param name="FixSuggestions">Suggestions for fixing issues.</param>
/// <param name="ConfidenceScore">Confidence in the analysis (0.0-1.0).</param>
/// <param name="ReviewTimeMs">Time taken for the review.</param>
/// <param name="ErrorDetails">Error details if review failed.</param>
public record SemanticChangeReviewResult(
    bool Success,
    SemanticDiffAnalysis SemanticDiff,
    DriftAnalysis DriftAnalysis,
    IEnumerable<FixSuggestion> FixSuggestions,
    double ConfidenceScore,
    long ReviewTimeMs,
    string? ErrorDetails = null
)
{
    /// <summary>
    /// Gets the changes from semantic diff analysis.
    /// </summary>
    public IEnumerable<StructuralChange> Changes => SemanticDiff.StructuralChanges;
}