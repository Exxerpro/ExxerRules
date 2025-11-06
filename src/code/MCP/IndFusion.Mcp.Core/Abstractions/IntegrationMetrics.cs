namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Metrics for external service integration.
/// </summary>
/// <param name="RequestCount">Number of requests made.</param>
/// <param name="SuccessCount">Number of successful requests.</param>
/// <param name="FailureCount">Number of failed requests.</param>
/// <param name="AverageResponseTimeMs">Average response time.</param>
/// <param name="TotalDataTransferred">Total data transferred.</param>
public record IntegrationMetrics(
    int RequestCount,
    int SuccessCount,
    int FailureCount,
    long AverageResponseTimeMs,
    long TotalDataTransferred
);