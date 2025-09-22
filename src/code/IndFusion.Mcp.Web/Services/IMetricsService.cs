using IndFusion.Mcp.Web.Models;

namespace IndFusion.Mcp.Web.Services;

/// <summary>
/// Abstraction for collecting metrics and performance series for the web UI.
/// </summary>
public interface IMetricsService
{
    /// <summary>
    /// Returns a snapshot of key metrics.
    /// </summary>
    Task<MetricsData> GetMetricsDataAsync();

    /// <summary>
    /// Returns a series of performance metrics for the given period.
    /// </summary>
    /// <param name="period">Period window to simulate/aggregate.</param>
    Task<IEnumerable<PerformanceMetric>> GetPerformanceMetricsAsync(TimeSpan period);
}
