using System.ComponentModel;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Core.Abstractions;

namespace IndFusion.Mcp.Server.Resources;

/// <summary>
/// MCP resource that exposes code metrics for directories, files, classes, and methods.
/// </summary>
[McpServerResourceType]
public class MetricsResourceMcp
{
    private readonly IExxerFactoringService _ExxerFactoringService;

    /// <summary>
    /// Creates a new <see cref="MetricsResourceMcp"/>.
    /// </summary>
    /// <param name="ExxerFactoringService">Service providing metrics computation.</param>
    public MetricsResourceMcp(IExxerFactoringService ExxerFactoringService)
    {
        _ExxerFactoringService = ExxerFactoringService;
    }

    /// <summary>
    /// Reads code metrics for the specified path within a solution.
    /// </summary>
    /// <param name="path">Target path within the solution.</param>
    /// <param name="solutionPath">Absolute path to the solution (.sln).</param>
    /// <returns>Text content with metrics in JSON format.</returns>
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
