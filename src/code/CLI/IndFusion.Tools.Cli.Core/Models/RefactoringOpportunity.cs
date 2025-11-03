namespace IndFusion.Tools.Cli.Core.Models;

/// <summary>
/// Represents a refactoring opportunity detected in code
/// </summary>
public class RefactoringOpportunity
{
    /// <summary>
    /// Gets or sets the type of refactoring (e.g., ExtractMethod, MoveMethod)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file path containing the opportunity
    /// </summary>
    public string File { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the line number where the opportunity occurs
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the description of the opportunity
    /// </summary>
    public string Description { get; set; } = string.Empty;
}

