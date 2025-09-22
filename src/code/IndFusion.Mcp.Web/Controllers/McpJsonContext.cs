using System.Text.Json;
using System.Text.Json.Serialization;

namespace IndFusion.Mcp.Web.Controllers;

/// <summary>
/// System.Text.Json source generation context for MCP HTTP API types.
/// </summary>
[JsonSerializable(typeof(McpToolCallRequest))]
[JsonSerializable(typeof(McpToolCallResponse))]
[JsonSerializable(typeof(McpContent))]
[JsonSerializable(typeof(McpErrorResponse))]
[JsonSerializable(typeof(McpListToolsResponse))]
[JsonSerializable(typeof(McpTool))]
[JsonSerializable(typeof(McpServerInfo))]
[JsonSerializable(typeof(McpCapabilities))]
[JsonSerializable(typeof(McpToolsCapability))]
[JsonSerializable(typeof(McpResourcesCapability))]
[JsonSerializable(typeof(List<McpTool>))]
[JsonSerializable(typeof(List<McpContent>))]
[JsonSerializable(typeof(Dictionary<string, JsonElement>))]
public partial class McpJsonContext : JsonSerializerContext { }

/// <summary>
/// System.Text.Json source generation context for MCP HTTP API types.
/// </summary>
public partial class McpJsonContext { }
