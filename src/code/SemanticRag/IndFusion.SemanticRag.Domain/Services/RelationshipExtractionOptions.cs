namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Options for relationship extraction.
/// </summary>
/// <param name="RelationshipTypes">Types of relationships to extract.</param>
/// <param name="MinConfidence">Minimum confidence threshold for relationships.</param>
/// <param name="MaxRelationships">Maximum number of relationships to extract.</param>
/// <param name="EnableBidirectional">Whether to extract bidirectional relationships.</param>
/// <param name="IncludeWeight">Whether to calculate relationship weights.</param>
public readonly record struct RelationshipExtractionOptions(
    IReadOnlyList<string> RelationshipTypes,
    float MinConfidence = 0.6f,
    int MaxRelationships = 50,
    bool EnableBidirectional = true,
    bool IncludeWeight = true)
{
    /// <summary>
    /// Default options for general relationship extraction.
    /// </summary>
    public static RelationshipExtractionOptions Default() => new(
        RelationshipTypes: new[] { "RELATES_TO", "CONTAINS", "SIMILAR_TO", "DEPENDS_ON", "IMPLEMENTS" },
        MinConfidence: 0.6f,
        MaxRelationships: 30,
        EnableBidirectional: true,
        IncludeWeight: true);
}