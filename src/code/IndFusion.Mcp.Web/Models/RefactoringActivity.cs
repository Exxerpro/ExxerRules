namespace IndFusion.Mcp.Web.Models;

/// <summary>
/// Describes a single ExxerFactoring activity for audit/telemetry.
/// </summary>
/// <param name="Timestamp">UTC timestamp of the activity.</param>
/// <param name="ToolName">Tool invoked.</param>
/// <param name="ProjectName">Project name or identifier.</param>
/// <param name="Success">Whether the activity succeeded.</param>
/// <param name="Duration">Execution time.</param>
/// <param name="ErrorMessage">Optional error details when failed.</param>
public record ExxerFactoringActivity(
    DateTime Timestamp,
    string ToolName,
    string ProjectName,
    bool Success,
    TimeSpan Duration,
    string? ErrorMessage = null
);
