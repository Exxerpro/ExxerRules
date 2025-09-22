using IndFusion.Mcp.Web.Models;
using IndFusion.Mcp.Web.Services;

namespace IndFusion.Mcp.Web.Tests.Integration;

/// <summary>
/// Test IMetricsService implementation used by integration tests.
/// </summary>
public class TestMetricsService : IMetricsService
{
    /// <summary>
    /// Returns a small, fixed metrics snapshot for integration tests.
    /// </summary>
    public Task<MetricsData> GetMetricsDataAsync()
    {
        return Task.FromResult(new MetricsData(10, 1.5, 2, 1024 * 1024 * 100, 5.0));
    }

    /// <summary>
    /// Returns a minimal sequence with a single metric record, ignoring <paramref name="period"/>.
    /// </summary>
    public Task<IEnumerable<PerformanceMetric>> GetPerformanceMetricsAsync(TimeSpan period)
    {
        var metrics = new[] { new PerformanceMetric(DateTime.Now, "test", 1.0, "unit") };
        return Task.FromResult<IEnumerable<PerformanceMetric>>(metrics);
    }
}

