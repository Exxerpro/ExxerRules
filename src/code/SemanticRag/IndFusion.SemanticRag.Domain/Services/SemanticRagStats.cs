namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Statistics about the semantic RAG system.
/// </summary>
/// <param name="TotalDocuments">Total number of indexed documents.</param>
/// <param name="TotalEntities">Total number of knowledge entities.</param>
/// <param name="TotalRelationships">Total number of relationships.</param>
/// <param name="LastIndexedAt">When the last document was indexed.</param>
/// <param name="AverageDocumentSize">Average size of documents in characters.</param>
/// <param name="EmbeddingDimension">Dimension of the embedding vectors.</param>
public readonly record struct SemanticRagStats(
    int TotalDocuments,
    int TotalEntities,
    int TotalRelationships,
    DateTimeOffset? LastIndexedAt,
    double AverageDocumentSize,
    int EmbeddingDimension);