namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Metrics about pattern extraction process.
/// </summary>
/// <param name="PatternsExtracted">Number of patterns extracted.</param>
/// <param name="ExtractionTimeMs">Time taken for extraction.</param>
/// <param name="ConfidenceDistribution">Distribution of confidence scores.</param>
/// <param name="PatternTypeDistribution">Distribution by pattern type.</param>
public record ExtractionMetrics(
    int PatternsExtracted,
    long ExtractionTimeMs,
    Dictionary<string, int> ConfidenceDistribution,
    Dictionary<string, int> PatternTypeDistribution
);