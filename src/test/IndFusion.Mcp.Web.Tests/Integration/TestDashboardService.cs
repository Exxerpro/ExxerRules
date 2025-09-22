using IndFusion.Mcp.Web.Models;
using IndFusion.Mcp.Web.Services;

namespace IndFusion.Mcp.Web.Tests.Integration;

/// <summary>
/// Test IDashboardService used by integration tests.
/// </summary>
public class TestDashboardService : IDashboardService
{
    /// <summary>
    /// Returns a small dashboard snapshot for tests.
    /// </summary>
    public Task<DashboardStats> GetDashboardStatsAsync()
    {
        return Task.FromResult(new DashboardStats(10, 2, 5, 1.5, 95));
    }

    /// <summary>
    /// Returns a single mock activity honoring the count parameter.
    /// </summary>
    public Task<IEnumerable<ExxerFactoringActivity>> GetRecentActivitiesAsync(int count = 20)
    {
        var activities = new[]
        {
            new ExxerFactoringActivity(DateTime.Now, "test-tool", "TestProject", true, TimeSpan.FromSeconds(1))
        };
        return Task.FromResult<IEnumerable<ExxerFactoringActivity>>(activities);
    }

    /// <summary>
    /// Returns a healthy system status for tests.
    /// </summary>
    public Task<SystemHealthStatus> GetSystemHealthAsync()
    {
        var components = new Dictionary<string, ComponentHealth>
        {
            ["Test"] = new ComponentHealth(true, "Healthy", null, DateTime.Now)
        };
        return Task.FromResult(new SystemHealthStatus(true, "Healthy", components));
    }
}

