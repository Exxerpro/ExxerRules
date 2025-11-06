namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a collection of search results with metadata.
/// </summary>
/// <param name="Results">The search results.</param>
/// <param name="TotalCount">Total number of results available.</param>
/// <param name="Query">The original search query.</param>
/// <param name="SearchTime">Time taken to perform the search in milliseconds.</param>
/// <param name="Metadata">Additional search metadata.</param>
public record SearchResultCollection(
    IReadOnlyList<SearchResult> Results,
    int TotalCount,
    string Query,
    long SearchTime,
    IReadOnlyDictionary<string, object> Metadata)
{
    /// <summary>
    /// Gets the search results.
    /// </summary>
    public IReadOnlyList<SearchResult> Results { get; init; } = Results;

    /// <summary>
    /// Gets the total count of available results.
    /// </summary>
    public int TotalCount { get; init; } = TotalCount;

    /// <summary>
    /// Gets the original search query.
    /// </summary>
    public string Query { get; init; } = Query;

    /// <summary>
    /// Gets the search time in milliseconds.
    /// </summary>
    public long SearchTime { get; init; } = SearchTime;

    /// <summary>
    /// Gets the search metadata.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = Metadata;

    /// <summary>
    /// Gets the number of results in this collection.
    /// </summary>
    public int Count => Results.Count;

    /// <summary>
    /// Gets a value indicating whether there are any results.
    /// </summary>
    public bool HasResults => Results.Count > 0;

    /// <summary>
    /// Validates the search result collection for basic requirements.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Query cannot be empty or whitespace");

        if (TotalCount < 0)
            return Result.WithFailure("Total count cannot be negative");

        if (SearchTime < 0)
            return Result.WithFailure("Search time cannot be negative");

        if (Results.Count > TotalCount)
            return Result.WithFailure("Results count cannot exceed total count");

        // Validate each result
        foreach (var result in Results)
        {
            var resultValidation = result.Validate();
            if (resultValidation.IsFailure)
                return resultValidation;
        }

        return Result.Success();
    }
}