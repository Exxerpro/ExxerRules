using System.ComponentModel;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Mcp.Core.Abstractions;

namespace IndFusion.Mcp.Mcp.Server.Resources;

[McpServerResourceType]
public class MetricsResourceMcp
{
    private readonly IExxerFactoringService _ExxerFactoringService;

    public MetricsResourceMcp(IExxerFactoringService ExxerFactoringService)
    {
        _ExxerFactoringService = ExxerFactoringService;
    }

    [McpServerResource(UriTemplate = "metrics://{+path}")]
    [Description("Return code metrics for directories, files, classes or methods")]
    public async Task<TextResourceContents> ReadMetrics(
        [Description("Target path within the solution")] string path,
        [Description("Absolute path to the solution file (.sln)")] string solutionPath)
    {
        var metricsJson = await _ExxerFactoringService.GetMetricsAsync(solutionPath, path);
        return new TextResourceContents { Text = metricsJson };
    }
}
