namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of external linting integration.
/// </summary>
/// <param name="Success">Whether the integration succeeded.</param>
/// <param name="ExternalService">External service used.</param>
/// <param name="LintingResults">Results from external linting.</param>
/// <param name="IntegrationMetrics">Metrics for the integration.</param>
/// <param name="ExecutionTimeMs">Time taken for integration.</param>
/// <param name="ErrorDetails">Error details if integration failed.</param>
public record ExternalLintingResult(
    bool Success,
    string ExternalService,
    IEnumerable<ExternalLintingIssue> LintingResults,
    IntegrationMetrics IntegrationMetrics,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);