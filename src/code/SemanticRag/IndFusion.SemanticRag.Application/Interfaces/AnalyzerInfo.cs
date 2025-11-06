namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents information about an analyzer.
/// </summary>
public record AnalyzerInfo
{
    /// <summary>
    /// Unique identifier for the analyzer.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Name of the analyzer.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Description of what the analyzer does.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Category of the analyzer.
    /// </summary>
    public required string Category { get; init; }

    /// <summary>
    /// Whether the analyzer is enabled.
    /// </summary>
    public required bool IsEnabled { get; init; }
}