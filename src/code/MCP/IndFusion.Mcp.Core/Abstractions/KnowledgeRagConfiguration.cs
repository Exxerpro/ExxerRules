namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for the Knowledge RAG service.
/// </summary>
/// <param name="VectorStoreConfig">Configuration for the vector store.</param>
/// <param name="EmbeddingConfig">Configuration for embeddings.</param>
/// <param name="RagConfig">Configuration for RAG operations.</param>
/// <param name="IndexingConfig">Configuration for indexing operations.</param>
/// <param name="Version">Version of the configuration.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record KnowledgeRagConfiguration(
    VectorStoreConfiguration VectorStoreConfig,
    EmbeddingConfiguration EmbeddingConfig,
    RagConfiguration RagConfig,
    IndexingConfiguration IndexingConfig,
    string Version,
    DateTime LastUpdated
);