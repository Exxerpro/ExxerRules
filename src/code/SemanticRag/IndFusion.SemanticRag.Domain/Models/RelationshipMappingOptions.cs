namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for relationship mapping operations.
/// </summary>
/// <param name="RelationshipTypes">Types of relationships to map.</param>
/// <param name="MinConfidence">Minimum confidence threshold.</param>
/// <param name="MaxRelationships">Maximum number of relationships to map.</param>
public record RelationshipMappingOptions(
    IEnumerable<string> RelationshipTypes,
    double MinConfidence = 0.5,
    int MaxRelationships = 50
);