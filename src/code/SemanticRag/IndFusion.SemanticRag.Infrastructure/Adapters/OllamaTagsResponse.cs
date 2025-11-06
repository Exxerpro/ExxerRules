namespace IndFusion.SemanticRag.Infrastructure.Adapters;

/// <summary>
/// Response model for Ollama tags API.
/// </summary>
internal record OllamaTagsResponse(
    OllamaModel[] Models);