namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Capability flags related to MCP resources exposure.
/// </summary>
public class McpResourcesCapability
{
    /// <summary>Indicates subscription to resource change notifications is supported.</summary>
    public bool Subscribe { get; set; } = false;
    /// <summary>Signals the resource list has changed and clients may refresh.</summary>
    public bool ListChanged { get; set; } = false;
}
