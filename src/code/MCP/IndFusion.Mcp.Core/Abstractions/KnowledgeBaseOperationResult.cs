namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of a knowledge base operation.
/// </summary>
/// <param name="OperationType">Type of operation performed.</param>
/// <param name="EntityId">ID of the entity operated on.</param>
/// <param name="EntityType">Type of the entity.</param>
/// <param name="Changes">Changes made to the entity.</param>
/// <param name="Timestamp">When the operation was performed.</param>
public record KnowledgeBaseOperationResult(
    string OperationType,
    string EntityId,
    string EntityType,
    Dictionary<string, object> Changes,
    DateTime Timestamp
);