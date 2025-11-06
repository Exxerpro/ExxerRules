namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Query for semantic search operations.
/// </summary>
/// <param name="Id">Unique identifier for the query.</param>
/// <param name="Text">Text to search for.</param>
/// <param name="Filters">Additional filters to apply.</param>
/// <param name="MaxResults">Maximum number of results to return.</param>
/// <param name="MinScore">Minimum similarity score threshold.</param>
public record SemanticSearchQuery(
    string Id,
    string Text,
    Dictionary<string, object> Filters,
    int MaxResults = 10,
    double MinScore = 0.7
);

