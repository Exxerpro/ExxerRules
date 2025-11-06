namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of pattern graph query operations.
/// </summary>
/// <param name="Success">Whether the query succeeded.</param>
/// <param name="GraphResults">Graph traversal results.</param>
/// <param name="Relationships">Discovered relationships.</param>
/// <param name="PatternInsights">Pattern insights from the graph.</param>
/// <param name="QueryExecutionTimeMs">Time taken to execute the query.</param>
/// <param name="ErrorDetails">Error details if query failed.</param>
public record PatternGraphResult(
    bool Success,
    IEnumerable<GraphTraversalResult> GraphResults,
    IEnumerable<PatternRelationship> Relationships,
    IEnumerable<PatternInsight> PatternInsights,
    long QueryExecutionTimeMs,
    string? ErrorDetails = null
);