using IndFusion.SemanticRag.Domain.Errors;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a relationship between two knowledge nodes.
/// </summary>
/// <param name="Id">Unique identifier for the relationship.</param>
/// <param name="FromNodeId">ID of the source node.</param>
/// <param name="ToNodeId">ID of the target node.</param>
/// <param name="RelationshipType">Type of the relationship.</param>
/// <param name="Properties">Properties associated with the relationship.</param>
/// <param name="CreatedAt">Timestamp when the relationship was created.</param>
public record KnowledgeRelationship(
    string Id,
    string FromNodeId,
    string ToNodeId,
    string RelationshipType,
    IReadOnlyDictionary<string, object> Properties,
    DateTimeOffset CreatedAt)
{
    /// <summary>
    /// Gets the relationship identifier.
    /// </summary>
    public string Id { get; init; } = Id;

    /// <summary>
    /// Gets the source node identifier.
    /// </summary>
    public string FromNodeId { get; init; } = FromNodeId;

    /// <summary>
    /// Gets the target node identifier.
    /// </summary>
    public string ToNodeId { get; init; } = ToNodeId;

    /// <summary>
    /// Gets the relationship type.
    /// </summary>
    public string RelationshipType { get; init; } = RelationshipType;

    /// <summary>
    /// Gets the relationship properties.
    /// </summary>
    public IReadOnlyDictionary<string, object> Properties { get; init; } = Properties;

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = CreatedAt;

    /// <summary>
    /// Validates the knowledge relationship for basic requirements.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure(ErrorCodes.KnowledgeRelationshipIdRequired);

        if (string.IsNullOrWhiteSpace(FromNodeId))
            return Result.WithFailure(ErrorCodes.KnowledgeRelationshipSourceIdRequired);

        if (string.IsNullOrWhiteSpace(ToNodeId))
            return Result.WithFailure(ErrorCodes.KnowledgeRelationshipTargetIdRequired);

        if (string.IsNullOrWhiteSpace(RelationshipType))
            return Result.WithFailure(ErrorCodes.KnowledgeRelationshipTypeRequired);

        if (FromNodeId == ToNodeId)
            return Result.WithFailure(ErrorCodes.KnowledgeRelationshipSameNodeIds);

        return Result.Success();
    }
}