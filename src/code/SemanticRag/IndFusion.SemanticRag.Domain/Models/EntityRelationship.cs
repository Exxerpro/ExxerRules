namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Relationship between extracted entities.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="SourceEntityId">ID of the source entity.</param>
/// <param name="TargetEntityId">ID of the target entity.</param>
/// <param name="RelationshipType">Type of relationship.</param>
/// <param name="Confidence">Confidence score for the relationship.</param>
/// <param name="Properties">Additional relationship properties.</param>
public record EntityRelationship(
    string Id,
    string SourceEntityId,
    string TargetEntityId,
    string RelationshipType,
    double Confidence,
    Dictionary<string, object> Properties
);