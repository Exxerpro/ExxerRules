namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a knowledge entity extracted from documents.
/// </summary>
/// <param name="Id">Unique identifier for the entity.</param>
/// <param name="Name">Entity name.</param>
/// <param name="Type">Entity type (person, organization, concept, etc.).</param>
/// <param name="Description">Entity description.</param>
/// <param name="Properties">Additional entity properties.</param>
/// <param name="Confidence">Confidence score for the extraction.</param>
/// <param name="CreatedAt">When the entity was extracted.</param>
public record KnowledgeEntity(
    string Id,
    string Name,
    string Type,
    string Description,
    Dictionary<string, object> Properties,
    double Confidence,
    DateTime CreatedAt
);