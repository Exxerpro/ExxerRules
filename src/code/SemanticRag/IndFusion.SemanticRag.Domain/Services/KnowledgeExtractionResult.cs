using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Result of knowledge extraction.
/// </summary>
/// <param name="Entities">Extracted entities.</param>
/// <param name="Relationships">Extracted relationships.</param>
/// <param name="CodeEntities">Extracted code entities.</param>
/// <param name="Concepts">Extracted concepts.</param>
/// <param name="ProcessingTimeMs">Time taken for extraction in milliseconds.</param>
/// <param name="Confidence">Overall confidence score for the extraction.</param>
public readonly record struct KnowledgeExtractionResult(
    IReadOnlyList<KnowledgeEntity> Entities,
    IReadOnlyList<KnowledgeRelationship> Relationships,
    IReadOnlyList<CodeEntity> CodeEntities,
    IReadOnlyList<SemanticConcept> Concepts,
    long ProcessingTimeMs,
    float Confidence)
{
    /// <summary>
    /// Gets the total number of extracted items.
    /// </summary>
    public int TotalItems => Entities.Count + Relationships.Count + CodeEntities.Count + Concepts.Count;

    /// <summary>
    /// Checks if any knowledge was extracted.
    /// </summary>
    public bool HasKnowledge => TotalItems > 0;
}