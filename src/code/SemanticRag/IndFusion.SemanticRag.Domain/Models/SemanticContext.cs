namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Context for semantic operations.
/// </summary>
/// <param name="Id">Unique identifier for the context.</param>
/// <param name="Name">Name of the context.</param>
/// <param name="Description">Description of the context.</param>
/// <param name="Documents">Documents in this context.</param>
/// <param name="Entities">Entities in this context.</param>
/// <param name="Relationships">Relationships in this context.</param>
/// <param name="Properties">Additional context properties.</param>
/// <param name="CreatedAt">When the context was created.</param>
public record SemanticContext(
    string Id,
    string Name,
    string Description,
    IEnumerable<SemanticDocument> Documents,
    IEnumerable<KnowledgeEntity> Entities,
    IEnumerable<EntityRelationship> Relationships,
    Dictionary<string, object> Properties,
    DateTime CreatedAt
);