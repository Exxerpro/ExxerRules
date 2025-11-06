using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Result of entity validation.
/// </summary>
/// <param name="ValidEntities">Entities that passed validation.</param>
/// <param name="InvalidEntities">Entities that failed validation.</param>
/// <param name="ValidationErrors">Validation error messages.</param>
/// <param name="OverallQuality">Overall quality score (0.0 to 1.0).</param>
public readonly record struct EntityValidationResult(
    IReadOnlyList<KnowledgeEntity> ValidEntities,
    IReadOnlyList<KnowledgeEntity> InvalidEntities,
    IReadOnlyList<string> ValidationErrors,
    float OverallQuality);