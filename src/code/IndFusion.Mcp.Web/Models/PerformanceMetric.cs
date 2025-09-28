namespace IndFusion.Mcp.Web.Models;

/// <summary>
/// Represents a single performance measurement at a point in time.
/// </summary>
/// <param name="Timestamp">UTC timestamp of the measurement.</param>
/// <param name="MetricName">Name of the metric (e.g., ResponseTime).</param>
/// <param name="Value">Numeric metric value.</param>
/// <param name="Unit">Unit of measure (e.g., ms, req/min).</param>
public record PerformanceMetric(
    DateTime Timestamp,
    string MetricName,
    double Value,
    string Unit
);
