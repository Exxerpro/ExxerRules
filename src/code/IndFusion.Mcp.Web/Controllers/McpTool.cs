namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Describes a tool surfaced by the MCP HTTP API.
/// </summary>
public class McpTool
{
    /// <summary>Tool name (kebab-case).</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Human-readable description.</summary>
    public string Description { get; set; } = string.Empty;
    /// <summary>JSON schema for input parameters (anonymous object).</summary>
    public object? InputSchema { get; set; }
}
