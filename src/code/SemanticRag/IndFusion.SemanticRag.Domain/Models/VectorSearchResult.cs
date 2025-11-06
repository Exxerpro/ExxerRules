namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a search result from vector similarity search.
/// </summary>
/// <param name="Vector">The matching vector embedding.</param>
/// <param name="Similarity">The similarity score (0.0 to 1.0).</param>
/// <param name="Rank">The rank of this result in the search results.</param>
public readonly record struct VectorSearchResult(
    VectorEmbedding Vector,
    float Similarity,
    int Rank)
{
    /// <summary>
    /// Checks if this result meets the minimum threshold.
    /// </summary>
    /// <param name="threshold">The minimum similarity threshold.</param>
    /// <returns>True if the similarity meets the threshold.</returns>
    public bool MeetsThreshold(float threshold) => Similarity >= threshold;
}