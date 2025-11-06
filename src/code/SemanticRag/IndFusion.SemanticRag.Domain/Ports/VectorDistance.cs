namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Distance metrics for vector similarity calculation.
/// </summary>
public enum VectorDistance
{
    /// <summary>
    /// Cosine similarity metric.
    /// </summary>
    Cosine,

    /// <summary>
    /// Euclidean distance metric.
    /// </summary>
    Euclidean,

    /// <summary>
    /// Dot product similarity metric.
    /// </summary>
    Dot
}