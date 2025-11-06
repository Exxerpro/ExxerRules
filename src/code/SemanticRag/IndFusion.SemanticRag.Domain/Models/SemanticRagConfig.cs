namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Configuration for Semantic RAG operations.
/// </summary>
/// <param name="Id">Unique identifier for the configuration.</param>
/// <param name="Name">Name of the configuration.</param>
/// <param name="EmbeddingModel">Model to use for embeddings.</param>
/// <param name="VectorDimensions">Number of dimensions for vectors.</param>
/// <param name="SimilarityThreshold">Default similarity threshold.</param>
/// <param name="MaxResults">Default maximum results.</param>
/// <param name="Properties">Additional configuration properties.</param>
public record SemanticRagConfig(
    string Id,
    string Name,
    string EmbeddingModel,
    int VectorDimensions,
    double SimilarityThreshold,
    int MaxResults,
    Dictionary<string, object> Properties
);