using Microsoft.Extensions.Logging;
using IndFusion.Mcp.Core.Abstractions;

using IndFusion.Mcp.Web.Models;

namespace IndFusion.Mcp.Web.Services;

/// <summary>
/// Provides dashboard data for the web UI (aggregate stats, recent activity, health).
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IExxerFactoringService _ExxerFactoringService;
    private readonly ILogger<DashboardService> _logger;

    /// <summary>
    /// Initializes a new instance of <see cref="DashboardService"/>.
    /// </summary>
    /// <param name="ExxerFactoringService">Service for querying MCP tools and metrics.</param>
    /// <param name="logger">Logger instance.</param>
    public DashboardService(IExxerFactoringService ExxerFactoringService, ILogger<DashboardService> logger)
    {
        _ExxerFactoringService = ExxerFactoringService;
        _logger = logger;
    }

    /// <summary>
    /// Returns current dashboard statistics.
    /// </summary>
    public async Task<DashboardStats> GetDashboardStatsAsync()
    {
        try
        {
            var tools = await _ExxerFactoringService.ListAvailableToolsAsync();

            return new DashboardStats(
                TotalExxerFactorings: 0, // TODO: Implement tracking
                ActiveSolutions: 0, // TODO: Implement tracking
                AvailableTools: tools.Count(),
                AverageExecutionTime: 0.0, // TODO: Implement tracking
                SuccessRate: 95 // TODO: Implement tracking
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard stats");
            return new DashboardStats(0, 0, 0, 0.0, 0);
        }
    }

    /// <summary>
    /// Returns the most recent ExxerFactoring activities.
    /// </summary>
    /// <param name="count">Maximum number of activities.</param>
    public async Task<IEnumerable<ExxerFactoringActivity>> GetRecentActivitiesAsync(int count = 20)
    {
        // TODO: Implement activity tracking
        await Task.Delay(10);

        return new[]
        {
            new ExxerFactoringActivity(
                Timestamp: DateTime.Now.AddMinutes(-5),
                ToolName: "extract-method",
                ProjectName: "SampleProject",
                Success: true,
                Duration: TimeSpan.FromSeconds(2.3)
            ),
            new ExxerFactoringActivity(
                Timestamp: DateTime.Now.AddMinutes(-12),
                ToolName: "move-method",
                ProjectName: "AnotherProject",
                Success: false,
                Duration: TimeSpan.FromSeconds(1.8),
                ErrorMessage: "Target class not found"
            )
        };
    }

    /// <summary>
    /// Returns the current system health snapshot.
    /// </summary>
    public async Task<SystemHealthStatus> GetSystemHealthAsync()
    {
        await Task.Delay(10);

        var components = new Dictionary<string, ComponentHealth>
        {
            ["ExxerFactoringService"] = new ComponentHealth(true, "Healthy", LastChecked: DateTime.Now),
            ["McpServer"] = new ComponentHealth(true, "Healthy", LastChecked: DateTime.Now),
            ["Database"] = new ComponentHealth(true, "Healthy", LastChecked: DateTime.Now),
            ["Logging"] = new ComponentHealth(true, "Healthy", LastChecked: DateTime.Now)
        };

        return new SystemHealthStatus(
            IsHealthy: components.Values.All(c => c.IsHealthy),
            Status: "All systems operational",
            Components: components
        );
    }
}
