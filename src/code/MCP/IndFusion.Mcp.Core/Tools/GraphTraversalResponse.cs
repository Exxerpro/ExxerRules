using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for graph traversal operations.
/// </summary>
/// <param name="TraversalResult">The graph traversal result.</param>
/// <param name="Success">Whether the traversal was successful.</param>
/// <param name="ErrorMessage">Error message if traversal failed.</param>
public readonly record struct GraphTraversalResponse(
    GraphTraversalResult TraversalResult,
    bool Success,
    string? ErrorMessage);