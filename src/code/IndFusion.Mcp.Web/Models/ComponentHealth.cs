namespace IndFusion.Mcp.Web.Models;

/// <summary>
/// Health information for a single system component.
/// </summary>
/// <param name="IsHealthy">True when the component is healthy.</param>
/// <param name="Status">Status text.</param>
/// <param name="LastError">Last error message, if any.</param>
/// <param name="LastChecked">The last time this component was checked.</param>
public record ComponentHealth(
    bool IsHealthy,
    string Status,
    string? LastError = null,
    DateTime LastChecked = default
);
