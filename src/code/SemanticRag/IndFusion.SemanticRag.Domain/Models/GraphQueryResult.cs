namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents the result of a graph query execution.
/// </summary>
/// <param name="Records">The query result records.</param>
/// <param name="ExecutionTimeMs">Time taken to execute the query in milliseconds.</param>
/// <param name="RecordsAffected">Number of records affected by the query.</param>
/// <param name="Success">Whether the query executed successfully.</param>
/// <param name="ErrorMessage">Error message if the query failed.</param>
public readonly record struct GraphQueryResult(
    IReadOnlyList<GraphRecord> Records,
    long ExecutionTimeMs,
    int RecordsAffected,
    bool Success,
    string? ErrorMessage = null)
{
    /// <summary>
    /// Checks if the query was successful.
    /// </summary>
    public bool IsSuccess => Success && ErrorMessage is null;

    /// <summary>
    /// Gets the number of records returned.
    /// </summary>
    public int RecordCount => Records.Count;
}