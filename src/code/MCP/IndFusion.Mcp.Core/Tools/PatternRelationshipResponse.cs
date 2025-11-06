using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.Mcp.Core.Tools;

/// <summary>
/// Response model for pattern relationship operations.
/// </summary>
/// <param name="Relationships">The pattern relationships found.</param>
/// <param name="TotalRelationships">Total number of relationships.</param>
/// <param name="PatternId">The pattern ID that was searched.</param>
/// <param name="MaxDepth">Maximum depth used in the search.</param>
public readonly record struct PatternRelationshipResponse(
    IReadOnlyList<PatternRelationship> Relationships,
    int TotalRelationships,
    string PatternId,
    int MaxDepth);