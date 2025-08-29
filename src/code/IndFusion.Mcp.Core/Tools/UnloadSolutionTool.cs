using System.ComponentModel;
using ModelContextProtocol.Server;

namespace IndFusion.Mcp.Mcp.Core.Tools;

/// <summary>
/// Tools to unload a solution from cache and clear all caches.
/// </summary>
[McpServerToolType]
public static class UnloadSolutionTool
{
    /// <summary>
    /// Unloads the specified solution from the cache if present.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message indicating the outcome.</returns>
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
    /// Clears all solution, syntax and semantic caches.
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
