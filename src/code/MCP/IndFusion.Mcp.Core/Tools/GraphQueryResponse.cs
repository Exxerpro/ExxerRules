using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for graph query operations.
/// </summary>
/// <param name="Result">The graph query result.</param>
/// <param name="Success">Whether the query was successful.</param>
/// <param name="ErrorMessage">Error message if query failed.</param>
public readonly record struct GraphQueryResponse(
    GraphQueryResult Result,
    bool Success,
    string? ErrorMessage);