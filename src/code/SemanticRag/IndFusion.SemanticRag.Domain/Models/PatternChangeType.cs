namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the type of change made to a pattern.
/// </summary>
public enum PatternChangeType
{
    /// <summary>
    /// Pattern was created.
    /// </summary>
    Created,

    /// <summary>
    /// Pattern was updated.
    /// </summary>
    Updated,

    /// <summary>
    /// Pattern was deprecated.
    /// </summary>
    Deprecated,

    /// <summary>
    /// Pattern was removed.
    /// </summary>
    Removed,

    /// <summary>
    /// Pattern severity was changed.
    /// </summary>
    SeverityChanged,

    /// <summary>
    /// Pattern category was changed.
    /// </summary>
    CategoryChanged
}