using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for graph path operations.
/// </summary>
/// <param name="Path">The graph path found.</param>
/// <param name="Success">Whether the path search was successful.</param>
/// <param name="ErrorMessage">Error message if path search failed.</param>
public readonly record struct GraphPathResponse(
    GraphPath? Path,
    bool Success,
    string? ErrorMessage);