namespace IndFusion.Tools.Cli.Tools;

/// <summary>
/// MCP tools to unload solutions and clear solution cache used by refactoring helpers.
/// </summary>
[McpServerToolType]
public static class UnloadSolutionTool
{
    /// <summary>
    /// Unloads the specified solution and removes it from the in-memory cache.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the .sln file.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message.</returns>
    [McpServerTool, Description("Unload a solution and remove it from the cache")]
    public static string UnloadSolution(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        CancellationToken cancellationToken = default)
    {
        if (ExxerFactoringHelpers.SolutionCache.TryGetValue(solutionPath, out _))
        {
            ExxerFactoringHelpers.SolutionCache.Remove(solutionPath);
            return $"Unloaded solution '{Path.GetFileName(solutionPath)}' from cache";
        }

        return $"Solution '{Path.GetFileName(solutionPath)}' was not loaded";
    }

    /// <summary>
    /// Clears all cached solutions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message.</returns>
    [McpServerTool, Description("Clear all cached solutions")]
    public static string ClearSolutionCache(
        CancellationToken cancellationToken = default)
    {
        ExxerFactoringHelpers.ClearAllCaches();
        return "Cleared all cached solutions";
    }
}
