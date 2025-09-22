using IndFusion.Mcp.Web.Models;

namespace IndFusion.Mcp.Web.Tests.Services;

/// <summary>
/// Helper for building sample dashboard data with sensible defaults for tests.
/// </summary>
public static class DashboardTestDataBuilder
{
    /// <summary>
    /// Creates a DashboardStats record populated with provided or default values.
    /// </summary>
    public static DashboardStats CreateDashboardStats(
        int totalExxerFactorings = 100,
        int activeSolutions = 5,
        int availableTools = 15,
        double averageExecutionTime = 2.5,
        int successRate = 92)
    {
        return new DashboardStats(
            totalExxerFactorings,
            activeSolutions,
            availableTools,
            averageExecutionTime,
            successRate
        );
    }

    /// <summary>
    /// Creates a sample refactoring activity for assertions.
    /// </summary>
    public static ExxerFactoringActivity CreateActivity(
        string toolName = "test-tool",
        string projectName = "TestProject",
        bool success = true,
        DateTime? timestamp = null,
        TimeSpan? duration = null,
        string? errorMessage = null)
    {
        return new ExxerFactoringActivity(
            timestamp ?? DateTime.Now,
            toolName,
            projectName,
            success,
            duration ?? TimeSpan.FromSeconds(1.5),
            errorMessage
        );
    }

    /// <summary>
    /// Creates a SystemHealthStatus snapshot for tests; includes a default component.
    /// </summary>
    public static SystemHealthStatus CreateHealthStatus(
        bool isHealthy = true,
        string status = "All systems operational",
        Dictionary<string, ComponentHealth>? components = null)
    {
        return new SystemHealthStatus(
            isHealthy,
            status,
            components ?? new Dictionary<string, ComponentHealth>
            {
                ["TestComponent"] = new ComponentHealth(true, "Healthy", null, DateTime.Now)
            }
        );
    }
}
