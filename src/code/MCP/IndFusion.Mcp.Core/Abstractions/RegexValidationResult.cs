namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of regex pattern validation.
/// </summary>
/// <param name="IsValid">Whether the pattern is valid.</param>
/// <param name="SafetyScore">Safety score from 0.0 to 1.0.</param>
/// <param name="Issues">Issues found with the pattern.</param>
/// <param name="Warnings">Warnings about the pattern.</param>
/// <param name="PerformanceImpact">Estimated performance impact.</param>
/// <param name="ValidationTimeMs">Time taken for validation.</param>
public record RegexValidationResult(
    bool IsValid,
    double SafetyScore,
    IEnumerable<RegexIssue> Issues,
    IEnumerable<RegexWarning> Warnings,
    string PerformanceImpact,
    long ValidationTimeMs
);