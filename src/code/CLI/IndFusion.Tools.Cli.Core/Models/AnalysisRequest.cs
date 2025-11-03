namespace IndFusion.Tools.Cli.Core.Models;

/// <summary>
/// Represents a request for code analysis
/// </summary>
public class AnalysisRequest
{
    /// <summary>
    /// Gets or sets the type of analysis to perform
    /// </summary>
    public string AnalysisType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the solution file
    /// </summary>
    public string? SolutionPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the project file
    /// </summary>
    public string? ProjectPath { get; set; }

    /// <summary>
    /// Gets or sets the output format
    /// </summary>
    public string OutputFormat { get; set; } = "console";

    /// <summary>
    /// Gets or sets the output file path
    /// </summary>
    public string? OutputFile { get; set; }
}
