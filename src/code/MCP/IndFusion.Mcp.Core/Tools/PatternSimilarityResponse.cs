using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for pattern similarity operations.
/// </summary>
/// <param name="Similarities">The similar patterns found.</param>
/// <param name="TotalSimilarities">Total number of similar patterns.</param>
/// <param name="PatternId">The pattern ID that was searched.</param>
/// <param name="SimilarityThreshold">Similarity threshold used.</param>
public readonly record struct PatternSimilarityResponse(
    IReadOnlyList<PatternSimilarity> Similarities,
    int TotalSimilarities,
    string PatternId,
    float SimilarityThreshold);