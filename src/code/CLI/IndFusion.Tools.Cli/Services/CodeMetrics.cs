namespace IndFusion.Tools.Cli.Services;

/// <summary>
/// Represents basic code metrics
/// </summary>
public class CodeMetrics
{
    /// <summary>
    /// Gets or sets the lines of code
    /// </summary>
    public int LinesOfCode { get; set; }

    /// <summary>
    /// Gets or sets the cyclomatic complexity
    /// </summary>
    public int CyclomaticComplexity { get; set; }

    /// <summary>
    /// Gets or sets the method count
    /// </summary>
    public int MethodCount { get; set; }

    /// <summary>
    /// Gets or sets the class count
    /// </summary>
    public int ClassCount { get; set; }

    /// <summary>
    /// Gets or sets the interface count
    /// </summary>
    public int InterfaceCount { get; set; }

    /// <summary>
    /// Gets or sets the property count
    /// </summary>
    public int PropertyCount { get; set; }

    /// <summary>
    /// Gets or sets the field count
    /// </summary>
    public int FieldCount { get; set; }
}