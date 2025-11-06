namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents similarity between two patterns.
/// </summary>
/// <param name="PatternId">ID of the similar pattern.</param>
/// <param name="SimilarityScore">Similarity score (0.0 to 1.0).</param>
/// <param name="SimilarityType">Type of similarity (semantic, structural, etc.).</param>
/// <param name="CommonElements">Elements common between the patterns.</param>
public readonly record struct PatternSimilarity(
    string PatternId,
    float SimilarityScore,
    string SimilarityType,
    IReadOnlyList<string> CommonElements)
{
    /// <summary>
    /// Validates the pattern similarity.
    /// </summary>
    /// <returns>A Result indicating whether the similarity is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(PatternId))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (SimilarityScore < 0.0f || SimilarityScore > 1.0f)
            return Result.WithFailure("Similarity score must be between 0.0 and 1.0");

        if (string.IsNullOrWhiteSpace(SimilarityType))
            return Result.WithFailure("Similarity type cannot be null or empty");

        return Result.Success();
    }
}