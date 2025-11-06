namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the result of a pattern graph query.
/// </summary>
/// <param name="Patterns">Patterns found by the query.</param>
/// <param name="Relationships">Relationships found by the query.</param>
/// <param name="ExecutionTimeMs">Time taken to execute the query.</param>
/// <param name="TotalResults">Total number of results found.</param>
/// <param name="HasMoreResults">Whether there are more results available.</param>
public readonly record struct PatternGraphResult(
    IReadOnlyList<PatternDefinition> Patterns,
    IReadOnlyList<PatternRelationship> Relationships,
    long ExecutionTimeMs,
    int TotalResults,
    bool HasMoreResults)
{
    /// <summary>
    /// Gets the number of patterns found.
    /// </summary>
    public int PatternCount => Patterns.Count;

    /// <summary>
    /// Gets the number of relationships found.
    /// </summary>
    public int RelationshipCount => Relationships.Count;

    /// <summary>
    /// Validates the pattern graph result.
    /// </summary>
    /// <returns>A Result indicating whether the result is valid.</returns>
    public Result Validate()
    {
        if (ExecutionTimeMs < 0)
            return Result.WithFailure("Execution time cannot be negative");

        if (TotalResults < 0)
            return Result.WithFailure("Total results cannot be negative");

        return Result.Success();
    }
}