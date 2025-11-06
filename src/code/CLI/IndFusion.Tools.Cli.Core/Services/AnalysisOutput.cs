namespace IndFusion.Tools.Cli.Core.Services;

/// <summary>
/// Represents the output of code analysis in different formats
/// </summary>
public class AnalysisOutput
{
    /// <summary>
    /// Gets or sets the JSON output
    /// </summary>
    public string? JsonOutput { get; set; }

    /// <summary>
    /// Gets or sets the CSV output
    /// </summary>
    public string? CsvOutput { get; set; }

    /// <summary>
    /// Gets or sets the Markdown output
    /// </summary>
    public string? MarkdownOutput { get; set; }

    /// <summary>
    /// Gets or sets the console output
    /// </summary>
    public string? ConsoleOutput { get; set; }
}