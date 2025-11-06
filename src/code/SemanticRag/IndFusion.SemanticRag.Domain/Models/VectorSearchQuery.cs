namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a search query for vector similarity search.
/// </summary>
/// <param name="Query">The search query text.</param>
/// <param name="Embedding">The query embedding vector.</param>
/// <param name="Limit">Maximum number of results to return.</param>
/// <param name="Threshold">Minimum similarity threshold (0.0 to 1.0).</param>
/// <param name="Filters">Optional metadata filters.</param>
public readonly record struct VectorSearchQuery(
    string Query,
    float[] Embedding,
    int Limit = 10,
    float Threshold = 0.7f,
    IReadOnlyDictionary<string, object>? Filters = null)
{
    /// <summary>
    /// Validates the search query.
    /// </summary>
    /// <returns>A Result indicating whether the query is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Search query cannot be null or empty");

        if (Embedding.Length == 0)
            return Result.WithFailure("Query embedding cannot be empty");

        if (Limit <= 0)
            return Result.WithFailure("Search limit must be greater than 0");

        if (Threshold < 0.0f || Threshold > 1.0f)
            return Result.WithFailure("Similarity threshold must be between 0.0 and 1.0");

        return Result.Success();
    }
}