namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A pattern suggestion with details and confidence.
/// </summary>
/// <param name="Id">Unique identifier for the suggestion.</param>
/// <param name="PatternType">Type of pattern suggested.</param>
/// <param name="Description">Description of the suggested pattern.</param>
/// <param name="CodeExample">Example code showing the pattern.</param>
/// <param name="Confidence">Confidence score (0.0-1.0).</param>
/// <param name="Effort">Estimated effort to implement (Low, Medium, High).</param>
/// <param name="Benefits">Benefits of applying the pattern.</param>
/// <param name="Citations">Source citations for the pattern.</param>
public record PatternSuggestion(
    string Id,
    string PatternType,
    string Description,
    string CodeExample,
    double Confidence,
    string Effort,
    IEnumerable<string> Benefits,
    IEnumerable<PatternCitation> Citations
);