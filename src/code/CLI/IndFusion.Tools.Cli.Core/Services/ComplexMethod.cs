namespace IndFusion.Tools.Cli.Core.Services;

/// <summary>
/// Represents a complex method
/// </summary>
public class ComplexMethod
{
    /// <summary>
    /// Gets or sets the method name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the line number
    /// </summary>
    public int Line { get; set; }

    /// <summary>
    /// Gets or sets the complexity value
    /// </summary>
    public int Complexity { get; set; }
}