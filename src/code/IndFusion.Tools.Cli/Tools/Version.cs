namespace IndFusion.Tools.Mcp.App.Tools;

/// <summary>
/// Tool that reports the running assembly version and build timestamp.
/// </summary>
[McpServerToolType]
public static class VersionTool
{
    /// <summary>
    /// Returns version and build time information for diagnostics.
    /// </summary>
    [McpServerTool, Description("Show the current version and build timestamp")]
    public static string Version()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "unknown";
        var buildTime = File.GetLastWriteTimeUtc(assembly.Location).ToString("u");
        return $"Version: {version} (Build {buildTime})";
    }
}
