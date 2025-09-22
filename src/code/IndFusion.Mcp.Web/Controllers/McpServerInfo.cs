namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Describes high level MCP server information for clients.
/// </summary>
public class McpServerInfo
{
    /// <summary>Human friendly server name.</summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>Semantic version of the server implementation.</summary>
    public string Version { get; set; } = string.Empty;
    /// <summary>Supported MCP protocol version.</summary>
    public string ProtocolVersion { get; set; } = string.Empty;
    /// <summary>Advertised server capabilities.</summary>
    public McpCapabilities? Capabilities { get; set; }
}
