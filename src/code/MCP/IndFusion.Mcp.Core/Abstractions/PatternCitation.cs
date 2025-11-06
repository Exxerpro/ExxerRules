namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Citation for a pattern suggestion.
/// </summary>
/// <param name="Source">Source of the citation (document, code, etc.).</param>
/// <param name="Url">URL to the source if available.</param>
/// <param name="Confidence">Confidence in the citation (0.0-1.0).</param>
/// <param name="Relevance">Relevance to the current context (0.0-1.0).</param>
public record PatternCitation(
    string Source,
    string? Url,
    double Confidence,
    double Relevance
);