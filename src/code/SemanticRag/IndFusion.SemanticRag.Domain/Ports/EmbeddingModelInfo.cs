namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Represents information about the embedding model.
/// </summary>
/// <param name="ModelName">The name of the model.</param>
/// <param name="Version">The version of the model.</param>
/// <param name="Dimension">The embedding dimension.</param>
/// <param name="MaxTextLength">The maximum text length supported.</param>
/// <param name="SupportedLanguages">Languages supported by the model.</param>
public readonly record struct EmbeddingModelInfo(
    string ModelName,
    string Version,
    int Dimension,
    int MaxTextLength,
    IReadOnlyList<string> SupportedLanguages);