namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for pattern graph query operations.
/// </summary>
/// <param name="Query">Graph query to execute.</param>
/// <param name="RepositoryScope">Repository scope for the query.</param>
/// <param name="MaxDepth">Maximum traversal depth.</param>
/// <param name="IncludeMetadata">Whether to include node/edge metadata.</param>
/// <param name="Filters">Additional query filters.</param>
public record PatternGraphRequest(
    string Query,
    string RepositoryScope,
    int MaxDepth = 10,
    bool IncludeMetadata = true,
    Dictionary<string, object>? Filters = null
);