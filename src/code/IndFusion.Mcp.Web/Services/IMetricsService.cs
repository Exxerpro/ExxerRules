using IndFusion.Mcp.Mcp.Web.Models;

namespace IndFusion.Mcp.Mcp.Web.Services;

public interface IMetricsService
{
    Task<MetricsData> GetMetricsDataAsync();
    Task<IEnumerable<PerformanceMetric>> GetPerformanceMetricsAsync(TimeSpan period);
}
