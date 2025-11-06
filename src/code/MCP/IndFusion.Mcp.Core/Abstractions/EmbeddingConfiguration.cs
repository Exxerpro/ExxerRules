namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for embeddings.
/// </summary>
/// <param name="Provider">Embedding provider.</param>
/// <param name="Model">Model to use for embeddings.</param>
/// <param name="MaxTokens">Maximum tokens per embedding.</param>
/// <param name="BatchSize">Batch size for embedding operations.</param>
public record EmbeddingConfiguration(
    string Provider,
    string Model,
    int MaxTokens,
    int BatchSize
);