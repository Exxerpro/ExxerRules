namespace IndFusion.SemanticRag.Infrastructure.Adapters;

/// <summary>
/// Response model for Ollama embedding API.
/// </summary>
internal record OllamaEmbeddingResponse(
    [property: System.Text.Json.Serialization.JsonPropertyName("embedding")] float[] Embedding);