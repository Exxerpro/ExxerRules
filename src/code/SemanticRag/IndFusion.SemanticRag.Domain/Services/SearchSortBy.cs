namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// How to sort search results.
/// </summary>
public enum SearchSortBy
{
    /// <summary>
    /// Sort by relevance score.
    /// </summary>
    Relevance,

    /// <summary>
    /// Sort by creation date (newest first).
    /// </summary>
    DateCreated,

    /// <summary>
    /// Sort by last modified date (newest first).
    /// </summary>
    DateModified,

    /// <summary>
    /// Sort by document title.
    /// </summary>
    Title
}