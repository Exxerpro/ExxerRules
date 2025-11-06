namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// Result of system health checks.
/// </summary>
/// <param name="IsHealthy">Whether the system is healthy.</param>
/// <param name="TotalDocuments">Total number of indexed documents.</param>
/// <param name="TotalEntities">Total number of knowledge entities.</param>
/// <param name="TotalRelationships">Total number of relationships.</param>
/// <param name="LastIndexedAt">When the last document was indexed.</param>
/// <param name="AverageDocumentSize">Average size of documents in characters.</param>
/// <param name="EmbeddingDimension">Dimension of the embedding vectors.</param>
/// <param name="HealthScore">Overall health score (0.0 to 1.0).</param>
public readonly record struct SystemHealthResult(
    bool IsHealthy,
    int TotalDocuments,
    int TotalEntities,
    int TotalRelationships,
    DateTimeOffset? LastIndexedAt,
    double AverageDocumentSize,
    int EmbeddingDimension,
    float HealthScore);