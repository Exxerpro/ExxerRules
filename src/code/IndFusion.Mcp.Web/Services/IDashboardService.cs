using IndFusion.Mcp.Web.Models;

namespace IndFusion.Mcp.Web.Services;

/// <summary>
/// Abstraction for retrieving dashboard and health data for the web UI.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Returns current dashboard statistics.
    /// </summary>
    Task<DashboardStats> GetDashboardStatsAsync();

    /// <summary>
    /// Returns recent ExxerFactoring activities (most recent first).
    /// </summary>
    /// <param name="count">Maximum number of activities.</param>
    Task<IEnumerable<ExxerFactoringActivity>> GetRecentActivitiesAsync(int count = 20);

    /// <summary>
    /// Returns the current system health snapshot.
    /// </summary>
    Task<SystemHealthStatus> GetSystemHealthAsync();
}
