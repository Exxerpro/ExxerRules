namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of external analysis integration.
/// </summary>
/// <param name="Success">Whether the integration succeeded.</param>
/// <param name="ExternalService">External service used.</param>
/// <param name="AnalysisResults">Results from external analysis.</param>
/// <param name="IntegrationMetrics">Metrics for the integration.</param>
/// <param name="ExecutionTimeMs">Time taken for integration.</param>
/// <param name="ErrorDetails">Error details if integration failed.</param>
public record ExternalAnalysisResult(
    bool Success,
    string ExternalService,
    IEnumerable<ExternalAnalysisResult> AnalysisResults,
    IntegrationMetrics IntegrationMetrics,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);