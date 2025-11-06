namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents a consistency report for a project.
/// </summary>
public record ConsistencyReport
{
    /// <summary>
    /// Overall consistency score (0-1).
    /// </summary>
    public required float ConsistencyScore { get; init; }

    /// <summary>
    /// List of inconsistencies found.
    /// </summary>
    public required IReadOnlyList<Inconsistency> Inconsistencies { get; init; }

    /// <summary>
    /// Pattern family that was analyzed.
    /// </summary>
    public required string PatternFamily { get; init; }

    /// <summary>
    /// Number of files analyzed.
    /// </summary>
    public required int FilesAnalyzed { get; init; }

    /// <summary>
    /// Time taken for analysis in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }
}