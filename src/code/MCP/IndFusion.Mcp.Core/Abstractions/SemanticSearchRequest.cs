namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for semantic code search operations.
/// </summary>
/// <param name="Query">Semantic search query.</param>
/// <param name="RepositoryScope">Scope of the repository to search.</param>
/// <param name="MaxResults">Maximum number of results to return.</param>
/// <param name="SimilarityThreshold">Minimum similarity threshold for results.</param>
/// <param name="IncludeMetadata">Whether to include metadata in results.</param>
/// <param name="SearchOptions">Additional search options.</param>
public record SemanticSearchRequest(
    string Query,
    string RepositoryScope,
    int MaxResults = 10,
    double SimilarityThreshold = 0.7,
    bool IncludeMetadata = true,
    Dictionary<string, object>? SearchOptions = null
);