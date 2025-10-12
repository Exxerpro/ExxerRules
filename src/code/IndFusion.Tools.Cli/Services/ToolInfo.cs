namespace IndFusion.Tools.Cli.Services;

/// <summary>
/// Represents information about a refactoring tool
/// </summary>
public class ToolInfo
{
    /// <summary>
    /// Gets or sets the type of the tool
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the tool
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Initializes a new instance of the ToolInfo class
    /// </summary>
    /// <param name="type">The type of the tool</param>
    /// <param name="description">The description of the tool</param>
    public ToolInfo(string type, string description)
    {
        Type = type;
        Description = description;
    }
}