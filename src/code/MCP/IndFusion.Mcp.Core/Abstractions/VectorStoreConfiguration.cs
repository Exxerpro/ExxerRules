namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for the vector store.
/// </summary>
/// <param name="Provider">Vector store provider.</param>
/// <param name="ConnectionString">Connection string for the vector store.</param>
/// <param name="IndexName">Name of the index.</param>
/// <param name="Dimensions">Number of dimensions for vectors.</param>
/// <param name="SimilarityMetric">Similarity metric to use.</param>
public record VectorStoreConfiguration(
    string Provider,
    string ConnectionString,
    string IndexName,
    int Dimensions,
    string SimilarityMetric
);