namespace IndFusion.SemanticRag.Domain.Models;

// Note: GraphPath is now defined in TraversalModels.cs
// This file is kept for backward compatibility but the duplicate GraphPath definition has been removed

/// <summary>
/// Statistics about the knowledge graph.
/// </summary>
/// <param name="TotalNodes">Total number of nodes.</param>
/// <param name="TotalRelationships">Total number of relationships.</param>
/// <param name="NodeTypes">Count of nodes by type.</param>
/// <param name="RelationshipTypes">Count of relationships by type.</param>
/// <param name="LastUpdated">When statistics were last updated.</param>
public record KnowledgeGraphStatistics(
    long TotalNodes,
    long TotalRelationships,
    Dictionary<string, long> NodeTypes,
    Dictionary<string, long> RelationshipTypes,
    DateTime LastUpdated
);