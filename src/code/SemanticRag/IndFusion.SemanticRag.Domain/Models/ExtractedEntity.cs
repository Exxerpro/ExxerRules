namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Entity extracted from text content.
/// </summary>
/// <param name="Id">Unique identifier for the entity.</param>
/// <param name="Text">The text that was identified as an entity.</param>
/// <param name="Type">Type of entity (person, organization, location, etc.).</param>
/// <param name="Confidence">Confidence score for the extraction.</param>
/// <param name="StartPosition">Start position in the original text.</param>
/// <param name="EndPosition">End position in the original text.</param>
/// <param name="Properties">Additional entity properties.</param>
public record ExtractedEntity(
    string Id,
    string Text,
    string Type,
    double Confidence,
    int StartPosition,
    int EndPosition,
    Dictionary<string, object> Properties
);