using IndFusion.Mcp.Web.Models;

namespace IndFusion.Mcp.Web.Services;

public interface IMetricsService
{
    Task<MetricsData> GetMetricsDataAsync();
    Task<IEnumerable<PerformanceMetric>> GetPerformanceMetricsAsync(TimeSpan period);
}
