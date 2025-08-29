using System.ComponentModel;
using System.Reflection;
using ModelContextProtocol.Server;

namespace ExxerFactor.Mcp.Core.Tools;

/// <summary>
/// Displays the current assembly version and build timestamp.
/// </summary>
[McpServerToolType]
public static class VersionTool
{
    /// <summary>
    /// Returns version and build time information.
    /// </summary>
    /// <returns>A string with version and build timestamp.</returns>
    [McpServerTool, Description("Show the current version and build timestamp")]
    public static string Version()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "unknown";
        var buildTime = File.GetLastWriteTimeUtc(assembly.Location).ToString("u");
        return $"Version: {version} (Build {buildTime})";
    }
}