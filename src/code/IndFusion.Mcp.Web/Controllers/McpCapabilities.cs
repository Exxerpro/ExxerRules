namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Container for MCP capabilities exposed by the server (tools/resources, etc.).
/// </summary>
public class McpCapabilities
{
    /// <summary>Tool-related capabilities.</summary>
    public McpToolsCapability? Tools { get; set; }
    /// <summary>Resource-related capabilities.</summary>
    public McpResourcesCapability? Resources { get; set; }
}
