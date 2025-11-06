namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Information about an MCP tool.
/// </summary>
/// <param name="Name">Name of the tool.</param>
/// <param name="Description">Description of the tool.</param>
/// <param name="RequiredServices">Services required by the tool.</param>
/// <param name="Parameters">Parameters accepted by the tool.</param>
/// <param name="IsEnabled">Whether the tool is enabled.</param>
public record McpToolInfo(
    string Name,
    string Description,
    IEnumerable<string> RequiredServices,
    Dictionary<string, object> Parameters,
    bool IsEnabled
);