using System.Text.Json;

namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Request payload for invoking an MCP tool via HTTP.
/// </summary>
public class McpToolCallRequest
{
    /// <summary>Name of the tool to call.</summary>
    public string ToolName { get; set; } = string.Empty;
    /// <summary>Arbitrary JSON parameters for the tool call.</summary>
    public Dictionary<string, JsonElement>? Parameters { get; set; }
}
