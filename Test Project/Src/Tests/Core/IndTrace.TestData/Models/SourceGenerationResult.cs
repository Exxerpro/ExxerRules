namespace IndTrace.TestData.Models;

/// <summary>
/// Result of generating static test data classes from usage logs.
/// </summary>
internal sealed class SourceGenerationResult
{
    /// <summary>
    /// Whether the generation was successful.
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// List of generated files with their paths.
    /// </summary>
    public List<string> GeneratedFiles { get; set; } = [];

    /// <summary>
    /// Any warnings during generation.
    /// </summary>
    public List<string> Warnings { get; set; } = [];

    /// <summary>
    /// Error message if generation failed.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Number of classes generated.
    /// </summary>
    public int ClassesGenerated { get; set; }

    /// <summary>
    /// Total lines of code generated.
    /// </summary>
    public int LinesOfCodeGenerated { get; set; }
}
