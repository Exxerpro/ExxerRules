namespace IndFusion.SemanticRag.Infrastructure.Adapters;

/// <summary>
/// Model information from Ollama tags API.
/// </summary>
internal record OllamaModel(
    string Name,
    DateTime ModifiedAt,
    long Size);