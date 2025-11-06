namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Options for entity extraction.
/// </summary>
/// <param name="EntityTypes">Types of entities to extract.</param>
/// <param name="MinConfidence">Minimum confidence threshold for entities.</param>
/// <param name="MaxEntities">Maximum number of entities to extract.</param>
/// <param name="IncludeContext">Whether to include surrounding context.</param>
/// <param name="EnableNestedExtraction">Whether to extract nested entities.</param>
public readonly record struct EntityExtractionOptions(
    IReadOnlyList<string> EntityTypes,
    float MinConfidence = 0.7f,
    int MaxEntities = 100,
    bool IncludeContext = true,
    bool EnableNestedExtraction = true)
{
    /// <summary>
    /// Default options for general entity extraction.
    /// </summary>
    public static EntityExtractionOptions Default() => new(
        EntityTypes: new[] { "PERSON", "ORGANIZATION", "LOCATION", "CONCEPT", "TECHNOLOGY" },
        MinConfidence: 0.7f,
        MaxEntities: 50,
        IncludeContext: true,
        EnableNestedExtraction: true);

    /// <summary>
    /// Options for code entity extraction.
    /// </summary>
    public static EntityExtractionOptions ForCode() => new(
        EntityTypes: new[] { "CLASS", "METHOD", "INTERFACE", "NAMESPACE", "PROPERTY", "FIELD" },
        MinConfidence: 0.8f,
        MaxEntities: 200,
        IncludeContext: true,
        EnableNestedExtraction: true);
}