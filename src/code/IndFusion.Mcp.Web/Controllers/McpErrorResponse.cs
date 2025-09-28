namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// Error payload returned by MCP API endpoints.
/// </summary>
public class McpErrorResponse
{
    /// <summary>Error message.</summary>
    public string Error { get; set; } = string.Empty;
    /// <summary>Optional error code (protocol/application dependent).</summary>
    public int Code { get; set; }
}
