namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of pattern extraction operations.
/// </summary>
/// <param name="Success">Whether the extraction succeeded.</param>
/// <param name="ExtractedPatterns">Patterns extracted from the source code.</param>
/// <param name="KnowledgeBaseUpdates">Updates made to the knowledge base.</param>
/// <param name="ExtractionMetrics">Metrics about the extraction process.</param>
/// <param name="ExecutionTimeMs">Time taken for extraction.</param>
/// <param name="ErrorDetails">Error details if extraction failed.</param>
public record PatternExtractionResult(
    bool Success,
    IEnumerable<ExtractedPattern> ExtractedPatterns,
    IEnumerable<KnowledgeBaseUpdate> KnowledgeBaseUpdates,
    ExtractionMetrics ExtractionMetrics,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);