namespace IndFusion.Mcp.Mcp.Web.Models;

public record PerformanceMetric(
    DateTime Timestamp,
    string MetricName,
    double Value,
    string Unit
);
