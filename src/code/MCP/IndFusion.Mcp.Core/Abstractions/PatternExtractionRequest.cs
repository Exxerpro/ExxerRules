namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for pattern extraction operations.
/// </summary>
/// <param name="SourceCode">Source code to extract patterns from.</param>
/// <param name="PatternTypes">Types of patterns to extract.</param>
/// <param name="Metadata">Metadata about the source code.</param>
/// <param name="ConfidenceThreshold">Minimum confidence for extracted patterns.</param>
/// <param name="AddToKnowledgeBase">Whether to add extracted patterns to knowledge base.</param>
public record PatternExtractionRequest(
    string SourceCode,
    IEnumerable<string> PatternTypes,
    Dictionary<string, object> Metadata,
    double ConfidenceThreshold = 0.8,
    bool AddToKnowledgeBase = true
);