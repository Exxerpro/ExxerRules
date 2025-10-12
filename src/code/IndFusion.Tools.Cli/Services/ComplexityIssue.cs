namespace IndFusion.Tools.Cli.Services;

/// <summary>
/// Represents a complexity issue
/// </summary>
public class ComplexityIssue
{
    /// <summary>
    /// Gets or sets the file path
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the method name
    /// </summary>
    public string MethodName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the line number
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the complexity value
    /// </summary>
    public int Complexity { get; set; }

    /// <summary>
    /// Gets or sets the issue description
    /// </summary>
    public string Issue { get; set; } = string.Empty;
}