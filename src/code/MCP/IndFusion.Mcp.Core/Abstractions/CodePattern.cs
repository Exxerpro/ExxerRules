namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// A code pattern found in analysis.
/// </summary>
/// <param name="Id">Unique identifier for the pattern.</param>
/// <param name="PatternType">Type of the pattern.</param>
/// <param name="Description">Description of the pattern.</param>
/// <param name="CodeSnippet">Code snippet containing the pattern.</param>
/// <param name="Confidence">Confidence in the pattern (0.0-1.0).</param>
/// <param name="Location">Location of the pattern in the code.</param>
/// <param name="Metadata">Additional metadata about the pattern.</param>
public record CodePattern(
    string Id,
    string PatternType,
    string Description,
    string CodeSnippet,
    double Confidence,
    SourceLocation Location,
    Dictionary<string, object> Metadata
);