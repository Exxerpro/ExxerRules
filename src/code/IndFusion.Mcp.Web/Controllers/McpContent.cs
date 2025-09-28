namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Content item returned by MCP endpoints (e.g., text content).
/// </summary>
public class McpContent
{
    /// <summary>Content type discriminator (e.g., "text").</summary>
    public string Type { get; set; } = string.Empty;
    /// <summary>Content text payload.</summary>
    public string Text { get; set; } = string.Empty;
}
