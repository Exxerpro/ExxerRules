namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Result of knowledge base operations.
/// </summary>
/// <param name="Success">Whether the operation succeeded.</param>
/// <param name="OperationResult">Result of the knowledge base operation.</param>
/// <param name="EntitiesAffected">Number of entities affected.</param>
/// <param name="OperationTimeMs">Time taken for the operation.</param>
/// <param name="ErrorDetails">Error details if operation failed.</param>
public record KnowledgeBaseResult(
    bool Success,
    KnowledgeBaseOperationResult OperationResult,
    int EntitiesAffected,
    long OperationTimeMs,
    string? ErrorDetails = null
);