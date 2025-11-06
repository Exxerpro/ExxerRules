namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of external knowledge integration.
/// </summary>
/// <param name="Success">Whether the integration succeeded.</param>
/// <param name="ExternalService">External service used.</param>
/// <param name="KnowledgeResults">Results from external knowledge.</param>
/// <param name="IntegrationMetrics">Metrics for the integration.</param>
/// <param name="ExecutionTimeMs">Time taken for integration.</param>
/// <param name="ErrorDetails">Error details if integration failed.</param>
public record ExternalKnowledgeResult(
    bool Success,
    string ExternalService,
    IEnumerable<ExternalKnowledgeItem> KnowledgeResults,
    IntegrationMetrics IntegrationMetrics,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);