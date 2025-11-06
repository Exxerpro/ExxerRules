namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the severity level of a pattern violation.
/// </summary>
public enum PatternSeverity
{
    /// <summary>
    /// Information level - minor issues or suggestions.
    /// </summary>
    Info = 0,

    /// <summary>
    /// Warning level - potential issues that should be addressed.
    /// </summary>
    Warning = 1,

    /// <summary>
    /// Error level - issues that should be fixed.
    /// </summary>
    Error = 2,

    /// <summary>
    /// Critical level - serious issues that must be fixed immediately.
    /// </summary>
    Critical = 3
}