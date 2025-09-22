namespace IndFusion.Mcp.Web.Models;

/// <summary>
/// Snapshot of overall system health and component-level statuses.
/// </summary>
/// <param name="IsHealthy">True when all critical components are healthy.</param>
/// <param name="Status">Overall status text.</param>
/// <param name="Components">Component-specific health map.</param>
public record SystemHealthStatus(
    bool IsHealthy,
    string Status,
    Dictionary<string, ComponentHealth> Components
);
