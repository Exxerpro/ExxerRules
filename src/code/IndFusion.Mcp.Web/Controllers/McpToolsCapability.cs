namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Capability flags related to MCP tools exposure.
/// </summary>
public class McpToolsCapability
{
    /// <summary>
    /// Indicates the tools list has changed and clients may refresh cached tool metadata.
    /// </summary>
    public bool ListChanged { get; set; } = false;
}
