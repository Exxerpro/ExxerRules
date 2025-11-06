namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a query for pattern graph operations.
/// </summary>
/// <param name="Query">The query string.</param>
/// <param name="Parameters">Optional parameters for the query.</param>
/// <param name="MaxResults">Maximum number of results to return.</param>
/// <param name="TimeoutMs">Query timeout in milliseconds.</param>
public readonly record struct PatternGraphQuery(
    string Query,
    IReadOnlyDictionary<string, object>? Parameters = null,
    int MaxResults = 100,
    int TimeoutMs = 30000)
{
    /// <summary>
    /// Validates the pattern graph query.
    /// </summary>
    /// <returns>A Result indicating whether the query is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Query cannot be null or empty");

        if (MaxResults <= 0)
            return Result.WithFailure("Max results must be greater than 0");

        if (TimeoutMs <= 0)
            return Result.WithFailure("Timeout must be greater than 0");

        return Result.Success();
    }
}