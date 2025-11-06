namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Knowledge base update from pattern extraction.
/// </summary>
/// <param name="UpdateType">Type of update (Add, Update, Delete).</param>
/// <param name="EntityId">ID of the entity being updated.</param>
/// <param name="EntityType">Type of the entity.</param>
/// <param name="Changes">Changes made to the entity.</param>
/// <param name="Timestamp">When the update was made.</param>
public record KnowledgeBaseUpdate(
    string UpdateType,
    string EntityId,
    string EntityType,
    Dictionary<string, object> Changes,
    DateTime Timestamp
);