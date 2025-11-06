namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Configuration for RAG operations.
/// </summary>
/// <param name="MaxContextLength">Maximum context length for queries.</param>
/// <param name="Temperature">Temperature for response generation.</param>
/// <param name="MaxTokens">Maximum tokens in responses.</param>
/// <param name="TopK">Number of top results to consider.</param>
/// <param name="SimilarityThreshold">Minimum similarity threshold.</param>
public record RagConfiguration(
    int MaxContextLength,
    double Temperature,
    int MaxTokens,
    int TopK,
    double SimilarityThreshold
);