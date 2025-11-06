using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// Result of question answering operations.
/// </summary>
/// <param name="Question">The original question.</param>
/// <param name="Answer">The generated answer.</param>
/// <param name="SupportingDocuments">Documents used to generate the answer.</param>
/// <param name="SupportingEntities">Entities used to generate the answer.</param>
/// <param name="SupportingRelationships">Relationships used to generate the answer.</param>
/// <param name="Confidence">Confidence score for the answer (0.0 to 1.0).</param>
/// <param name="ProcessingTimeMs">Time taken for processing in milliseconds.</param>
public readonly record struct QuestionAnswerResult(
    string Question,
    string Answer,
    IReadOnlyList<SemanticDocument> SupportingDocuments,
    IReadOnlyList<KnowledgeEntity> SupportingEntities,
    IReadOnlyList<KnowledgeRelationship> SupportingRelationships,
    float Confidence,
    long ProcessingTimeMs);