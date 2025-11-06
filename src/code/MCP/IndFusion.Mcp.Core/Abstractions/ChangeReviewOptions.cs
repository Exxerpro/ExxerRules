namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Options for change review.
/// </summary>
/// <param name="IncludeMetrics">Whether to include code metrics.</param>
/// <param name="CheckPerformance">Whether to check for performance impact.</param>
/// <param name="CheckSecurity">Whether to check for security issues.</param>
/// <param name="CheckMaintainability">Whether to check for maintainability.</param>
public record ChangeReviewOptions(
    bool IncludeMetrics = true,
    bool CheckPerformance = true,
    bool CheckSecurity = true,
    bool CheckMaintainability = true
);