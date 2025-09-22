using IndFusion.Mcp.Web.Models;
using IndFusion.Mcp.Web.Services;

namespace IndFusion.Mcp.Web.Tests;

/// <summary>
/// Mock implementation of IMetricsService returning predictable metrics for tests.
/// </summary>
public class MockMetricsService : IMetricsService
{
    /// <summary>
    /// Returns a fixed MetricsData snapshot for assertions.
    /// </summary>
    public Task<MetricsData> GetMetricsDataAsync()
    {
        return Task.FromResult(new MetricsData(
            RequestsPerMinute: 156,
            AverageResponseTime: 1.85,
            ActiveConnections: 4,
            MemoryUsage: 245 * 1024 * 1024, // Convert MB to bytes
            CpuUsage: 12.3
        ));
    }

    /// <summary>
    /// Returns a small series of performance metrics independent of <paramref name="period"/>.
    /// </summary>
    public Task<IEnumerable<PerformanceMetric>> GetPerformanceMetricsAsync(TimeSpan period)
    {
        var now = DateTime.Now;
        var metrics = new[]
        {
            new PerformanceMetric(now.AddMinutes(-5), "CPU Usage", 12.3, "%"),
            new PerformanceMetric(now.AddMinutes(-5), "Memory Usage", 245.7, "MB"),
            new PerformanceMetric(now.AddMinutes(-5), "Disk IO", 15.2, "ops/sec"),
            new PerformanceMetric(now.AddMinutes(-5), "Network Throughput", 8.9, "Mbps")
        };

        return Task.FromResult<IEnumerable<PerformanceMetric>>(metrics);
    }
}

