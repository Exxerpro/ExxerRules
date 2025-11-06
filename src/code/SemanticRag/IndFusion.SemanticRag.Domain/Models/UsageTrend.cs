namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents usage trend over time.
/// </summary>
public enum UsageTrend
{
    /// <summary>
    /// Usage is increasing.
    /// </summary>
    Increasing,

    /// <summary>
    /// Usage is decreasing.
    /// </summary>
    Decreasing,

    /// <summary>
    /// Usage is stable.
    /// </summary>
    Stable,

    /// <summary>
    /// Usage trend is unknown.
    /// </summary>
    Unknown
}