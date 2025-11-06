namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for semantic change review.
/// </summary>
/// <param name="OriginalCode">Original code version.</param>
/// <param name="ModifiedCode">Modified code version.</param>
/// <param name="ReviewOptions">Options for the review process.</param>
/// <param name="IncludeDiff">Whether to include detailed diff information.</param>
/// <param name="CheckSemanticDrift">Whether to check for semantic drift.</param>
public record SemanticChangeReviewRequest(
    string OriginalCode,
    string ModifiedCode,
    ChangeReviewOptions ReviewOptions,
    bool IncludeDiff = true,
    bool CheckSemanticDrift = true
);