using IndFusion.Mcp.Web.Controllers;

namespace IndFusion.Mcp.Web.Tests.Controllers;

/// <summary>
/// Builders for common McpController request payloads used in tests.
/// </summary>
public static class McpTestDataBuilder
{
    /// <summary>
    /// Constructs a McpToolCallRequest with provided tool and optional parameters.
    /// </summary>
    public static McpToolCallRequest CreateToolCallRequest(string toolName, Dictionary<string, JsonElement>? parameters = null)
    {
        return new McpToolCallRequest
        {
            ToolName = toolName,
            Parameters = parameters ?? []
        };
    }

    /// <summary>
    /// Creates a parameter dictionary by serializing values to JsonElement.
    /// </summary>
    public static Dictionary<string, JsonElement> CreateParameters(params (string key, object value)[] parameters)
    {
        var result = new Dictionary<string, JsonElement>();
        foreach (var (key, value) in parameters)
        {
            var json = JsonSerializer.Serialize(value);
            result[key] = JsonSerializer.Deserialize<JsonElement>(json);
        }
        return result;
    }
}
