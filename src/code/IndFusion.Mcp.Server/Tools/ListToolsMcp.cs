using System.ComponentModel;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Core.Abstractions;

namespace IndFusion.Mcp.Server.Tools;

/// <summary>
/// MCP tool that returns the list of available ExxerFactor tools.
/// </summary>
[McpServerToolType]
public class ListToolsMcp
{
    private readonly IExxerFactoringService _ExxerFactoringService;

    /// <summary>
    /// Initializes the tool with the factoring service abstraction.
    /// </summary>
    /// <param name="ExxerFactoringService">Service used to query available tools.</param>
    public ListToolsMcp(IExxerFactoringService ExxerFactoringService)
    {
        _ExxerFactoringService = ExxerFactoringService;
    }

    /// <summary>
    /// Lists tools and returns their names as a newline-delimited string.
    /// </summary>
    /// <returns>Newline-delimited tool names.</returns>
    [McpServerTool, Description("List all available ExxerFactoring tools")]
    public async Task<string> ListToolsCommand()
    {
        var tools = await _ExxerFactoringService.ListAvailableToolsAsync();
        return string.Join('\n', tools.OrderBy(t => t));
    }
}
