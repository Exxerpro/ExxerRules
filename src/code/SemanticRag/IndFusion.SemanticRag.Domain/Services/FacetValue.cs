namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Represents a facet value with its count.
/// </summary>
/// <param name="Value">The facet value.</param>
/// <param name="Count">Number of documents with this value.</param>
public readonly record struct FacetValue(string Value, int Count);