namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Individual item in a search result.
/// </summary>
/// <param name="Id">Unique identifier for the result item.</param>
/// <param name="Content">Content of the result item.</param>
/// <param name="Score">Similarity score.</param>
/// <param name="Metadata">Additional metadata for the item.</param>
/// <param name="Source">Source of the result item.</param>
public record SearchResultItem(
    string Id,
    string Content,
    double Score,
    Dictionary<string, object> Metadata,
    string Source
);