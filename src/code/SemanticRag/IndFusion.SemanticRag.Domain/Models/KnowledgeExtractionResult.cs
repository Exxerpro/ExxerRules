namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Result of knowledge extraction operations.
/// </summary>
/// <param name="DocumentId">ID of the document from which knowledge was extracted.</param>
/// <param name="Entities">Entities extracted from the document.</param>
/// <param name="Relationships">Relationships mapped from the document.</param>
/// <param name="Summary">Summary of the extracted knowledge.</param>
/// <param name="Confidence">Overall confidence score for the extraction.</param>
/// <param name="Metadata">Additional extraction metadata.</param>
public record KnowledgeExtractionResult(
    string DocumentId,
    IReadOnlyList<KnowledgeEntity> Entities,
    IReadOnlyList<KnowledgeRelationship> Relationships,
    string Summary,
    double Confidence,
    Dictionary<string, object> Metadata
);