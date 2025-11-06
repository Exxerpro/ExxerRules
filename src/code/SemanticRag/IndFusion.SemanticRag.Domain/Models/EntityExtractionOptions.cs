namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for entity extraction operations.
/// </summary>
/// <param name="EntityTypes">Types of entities to extract.</param>
/// <param name="MinConfidence">Minimum confidence threshold.</param>
/// <param name="MaxEntities">Maximum number of entities to extract.</param>
/// <param name="Language">Language for extraction.</param>
public record EntityExtractionOptions(
    IEnumerable<string> EntityTypes,
    double MinConfidence = 0.5,
    int MaxEntities = 100,
    string Language = "en"
);