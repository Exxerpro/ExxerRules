namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for embedding generation operations.
/// </summary>
/// <param name="Model">Embedding model to use.</param>
/// <param name="Dimensions">Number of dimensions for the embedding.</param>
/// <param name="Normalize">Whether to normalize the embedding vector.</param>
public record EmbeddingOptions(
    string Model = "text-embedding-ada-002",
    int Dimensions = 1536,
    bool Normalize = true
);