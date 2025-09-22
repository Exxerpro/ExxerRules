namespace IndFusion.Mcp.Web.Models;

/// <summary>
/// Snapshot of key runtime metrics for the application.
/// </summary>
/// <param name="RequestsPerMinute">Throughput measured in requests per minute.</param>
/// <param name="AverageResponseTime">Average response time in milliseconds.</param>
/// <param name="ActiveConnections">Number of active connections.</param>
/// <param name="MemoryUsage">Working set in bytes.</param>
/// <param name="CpuUsage">Estimated CPU usage percentage (0-100).</param>
public record MetricsData(
    int RequestsPerMinute,
    double AverageResponseTime,
    int ActiveConnections,
    long MemoryUsage,
    double CpuUsage
);
