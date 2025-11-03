namespace IndFusion.Tools.Cli.Models;

/// <summary>
/// Represents a request for refactoring operations
/// </summary>
public class RefactoringRequest
{
    /// <summary>
    /// Gets or sets the name of the refactoring tool
    /// </summary>
    public string ToolName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the solution file
    /// </summary>
    public string? SolutionPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the project file
    /// </summary>
    public string? ProjectPath { get; set; }

    /// <summary>
    /// Gets or sets the path to the file to refactor
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Gets or sets the line and column range
    /// </summary>
    public string? Range { get; set; }

    /// <summary>
    /// Gets or sets the starting line number
    /// </summary>
    public int? Line { get; set; }

    /// <summary>
    /// Gets or sets the starting column number
    /// </summary>
    public int? Column { get; set; }

    /// <summary>
    /// Gets or sets the new name for the refactored entity
    /// </summary>
    public string? NewName { get; set; }

    /// <summary>
    /// Gets or sets the name of the method to refactor
    /// </summary>
    public string? MethodName { get; set; }

    /// <summary>
    /// Gets or sets the target location for move operations
    /// </summary>
    public string? TargetLocation { get; set; }

    /// <summary>
    /// Gets or sets whether to perform a dry run
    /// </summary>
    public bool DryRun { get; set; }

    /// <summary>
    /// Gets or sets the output format
    /// </summary>
    public string OutputFormat { get; set; } = "console";
}
