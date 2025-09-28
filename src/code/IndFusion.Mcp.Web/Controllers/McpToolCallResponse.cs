namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Standard tool call response content wrapper.
/// </summary>
public class McpToolCallResponse
{
    /// <summary>Response content list.</summary>
    public List<McpContent> Content { get; set; } = new();
}
