namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Comprehensive options for knowledge extraction.
/// </summary>
/// <param name="EntityOptions">Entity extraction options.</param>
/// <param name="RelationshipOptions">Relationship extraction options.</param>
/// <param name="CodeOptions">Code extraction options.</param>
/// <param name="ConceptOptions">Concept extraction options.</param>
/// <param name="EnableParallelProcessing">Whether to enable parallel processing.</param>
/// <param name="MaxProcessingTimeMs">Maximum processing time in milliseconds.</param>
public readonly record struct ComprehensiveExtractionOptions(
    EntityExtractionOptions EntityOptions,
    RelationshipExtractionOptions RelationshipOptions,
    CodeExtractionOptions CodeOptions,
    ConceptExtractionOptions ConceptOptions,
    bool EnableParallelProcessing = true,
    int MaxProcessingTimeMs = 30000)
{
    /// <summary>
    /// Default comprehensive extraction options.
    /// </summary>
    public static ComprehensiveExtractionOptions Default() => new(
        EntityOptions: EntityExtractionOptions.Default(),
        RelationshipOptions: RelationshipExtractionOptions.Default(),
        CodeOptions: CodeExtractionOptions.Comprehensive(),
        ConceptOptions: new ConceptExtractionOptions(),
        EnableParallelProcessing: true,
        MaxProcessingTimeMs: 30000);
}