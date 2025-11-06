namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Extracted pattern from source code.
/// </summary>
/// <param name="Id">Unique identifier for the extracted pattern.</param>
/// <param name="PatternType">Type of the extracted pattern.</param>
/// <param name="CodeSnippet">Code snippet containing the pattern.</param>
/// <param name="Confidence">Confidence in the extraction (0.0-1.0).</param>
/// <param name="Metadata">Additional metadata about the pattern.</param>
/// <param name="SourceLocation">Location in source code where pattern was found.</param>
public record ExtractedPattern(
    string Id,
    string PatternType,
    string CodeSnippet,
    double Confidence,
    Dictionary<string, object> Metadata,
    SourceLocation SourceLocation
);