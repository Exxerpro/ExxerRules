using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for graph statistics operations.
/// </summary>
/// <param name="Statistics">The graph statistics.</param>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="ErrorMessage">Error message if operation failed.</param>
public readonly record struct GraphStatisticsResponse(
    GraphStatistics Statistics,
    bool Success,
    string? ErrorMessage);