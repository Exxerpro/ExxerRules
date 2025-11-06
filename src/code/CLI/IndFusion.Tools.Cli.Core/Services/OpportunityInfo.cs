namespace IndFusion.Tools.Cli.Core.Services;

/// <summary>
/// Represents a refactoring opportunity
/// </summary>
public class OpportunityInfo
{
    /// <summary>
    /// Gets or sets the type of refactoring
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the line number
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; } = string.Empty;
}