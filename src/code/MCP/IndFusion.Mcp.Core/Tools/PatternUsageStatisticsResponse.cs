using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for pattern usage statistics operations.
/// </summary>
/// <param name="Statistics">The pattern usage statistics.</param>
/// <param name="Success">Whether the operation was successful.</param>
/// <param name="ErrorMessage">Error message if operation failed.</param>
public readonly record struct PatternUsageStatisticsResponse(
    PatternUsageStatistics Statistics,
    bool Success,
    string? ErrorMessage);