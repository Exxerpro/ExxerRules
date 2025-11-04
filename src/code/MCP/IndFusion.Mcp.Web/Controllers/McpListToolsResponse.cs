namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Response envelope for tool listing.
/// </summary>
public class McpListToolsResponse
{
    /// <summary>Collection of available tools.</summary>
    public List<McpTool> Tools { get; set; } = [];
}
