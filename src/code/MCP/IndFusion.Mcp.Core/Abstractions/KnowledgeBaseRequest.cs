namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for knowledge base operations.
/// </summary>
/// <param name="Operation">Type of knowledge base operation.</param>
/// <param name="EntityId">ID of the entity to operate on.</param>
/// <param name="EntityType">Type of the entity.</param>
/// <param name="Data">Data for the operation.</param>
/// <param name="Metadata">Additional metadata.</param>
public record KnowledgeBaseRequest(
    string Operation,
    string EntityId,
    string EntityType,
    Dictionary<string, object> Data,
    Dictionary<string, object>? Metadata = null
);