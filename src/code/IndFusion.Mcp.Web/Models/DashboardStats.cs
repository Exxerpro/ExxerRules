namespace IndFusion.Mcp.Web.Models;

/// <summary>
/// Aggregate statistics for the dashboard view.
/// </summary>
/// <param name="TotalExxerFactorings">Total number of ExxerFactorings performed.</param>
/// <param name="ActiveSolutions">Number of currently active/loaded solutions.</param>
/// <param name="AvailableTools">Count of tools available.</param>
/// <param name="AverageExecutionTime">Average tool execution time (ms).</param>
/// <param name="SuccessRate">Success rate percentage (0-100).</param>
public record DashboardStats(
    int TotalExxerFactorings,
    int ActiveSolutions,
    int AvailableTools,
    double AverageExecutionTime,
    int SuccessRate
);
