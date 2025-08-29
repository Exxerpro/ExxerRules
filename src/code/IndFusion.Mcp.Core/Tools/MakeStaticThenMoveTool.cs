using System.ComponentModel;
using ModelContextProtocol.Server;
using IndFusion.Mcp.Mcp.Core.Move;

namespace IndFusion.Mcp.Mcp.Core.Tools;

/// <summary>
/// Converts an instance method to static (injecting instance if needed) and moves it to a target class.
/// </summary>
[McpServerToolType]
public static class MakeStaticThenMoveTool
{
    /// <summary>
    /// Converts an instance method to static and moves it to the specified target class/file.
    /// </summary>
    /// <param name="solutionPath">Absolute path to the solution file (.sln).</param>
    /// <param name="filePath">Path to the C# file containing the method.</param>
    /// <param name="methodName">Name of the method to convert and move.</param>
    /// <param name="targetClass">Name of the target class.</param>
    /// <param name="instanceParameterName">Optional name for the injected instance parameter.</param>
    /// <param name="targetFilePath">Optional path to the target file.</param>
    /// <param name="progress">Optional progress reporter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status message for the operation.</returns>
    [McpServerTool, Description("Convert an instance method to static and move it to another class (preferred for large C# file ExxerFactoring)")]
    public static async Task<string> MakeStaticThenMove(
        [Description("Absolute path to the solution file (.sln)")] string solutionPath,
        [Description("Path to the C# file containing the method")] string filePath,
        [Description("Name of the method to convert and move")] string methodName,
        [Description("Name of the target class")] string targetClass,
        [Description("Name for the instance parameter (optional)")] string instanceParameterName = "instance",
        [Description("Path to the target file (optional, will create if doesn't exist or unspecified)")] string? targetFilePath = null,
        IProgress<string>? progress = null,
        CancellationToken cancellationToken = default)
    {
        await ConvertToStaticWithInstanceTool.ConvertToStaticWithInstance(
            solutionPath,
            filePath,
            methodName,
            instanceParameterName);

        return await MoveMethodTool.MoveStaticMethod(
            solutionPath,
            filePath,
            methodName,
            targetClass,
            targetFilePath,
            progress,
            cancellationToken);
    }
}
