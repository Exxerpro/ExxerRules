namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of a linting operation containing violations and policy recommendations.
/// </summary>
/// <param name="Success">Indicates whether the operation completed successfully.</param>
/// <param name="Violations">List of detected violations with details and policy recommendations.</param>
/// <param name="Summary">Summary statistics of the linting results.</param>
/// <param name="PolicyDecisions">Policy decisions made during the linting process.</param>
/// <param name="ExecutionTimeMs">Time taken to execute the linting operation in milliseconds.</param>
/// <param name="ErrorDetails">Error details if the operation failed.</param>
public record LintingResult(
    bool Success,
    IEnumerable<LintingViolation> Violations,
    LintingSummary Summary,
    IEnumerable<PolicyDecision> PolicyDecisions,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);