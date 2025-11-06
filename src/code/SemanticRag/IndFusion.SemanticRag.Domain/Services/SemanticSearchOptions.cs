namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Options for semantic search operations.
/// </summary>
/// <param name="MaxResults">Maximum number of results to return.</param>
/// <param name="SimilarityThreshold">Minimum similarity threshold for results.</param>
/// <param name="IncludeHighlights">Whether to include text highlights.</param>
/// <param name="IncludeMetadata">Whether to include document metadata.</param>
/// <param name="EnableQueryExpansion">Whether to enable automatic query expansion.</param>
/// <param name="EnableContextRetrieval">Whether to enable context retrieval.</param>
/// <param name="SortBy">How to sort the results.</param>
/// <param name="Filters">Additional filters to apply.</param>
public readonly record struct SemanticSearchOptions(
    int MaxResults = 10,
    float SimilarityThreshold = 0.7f,
    bool IncludeHighlights = true,
    bool IncludeMetadata = true,
    bool EnableQueryExpansion = true,
    bool EnableContextRetrieval = true,
    SearchSortBy SortBy = SearchSortBy.Relevance,
    IReadOnlyDictionary<string, object>? Filters = null)
{
    /// <summary>
    /// Validates the search options.
    /// </summary>
    public Result Validate()
    {
        if (MaxResults <= 0)
            return Result.WithFailure("MaxResults must be greater than 0");

        if (SimilarityThreshold < 0.0f || SimilarityThreshold > 1.0f)
            return Result.WithFailure("SimilarityThreshold must be between 0.0 and 1.0");

        return Result.Success();
    }
}