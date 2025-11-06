namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Represents a semantic concept.
/// </summary>
/// <param name="Id">Unique identifier for the concept.</param>
/// <param name="Name">Name of the concept.</param>
/// <param name="Definition">Definition of the concept.</param>
/// <param name="Synonyms">Synonyms for the concept.</param>
/// <param name="Frequency">Frequency of the concept in the document.</param>
/// <param name="Context">Context where the concept was found.</param>
/// <param name="Embedding">Vector embedding for semantic similarity.</param>
public readonly record struct SemanticConcept(
    string Id,
    string Name,
    string? Definition,
    IReadOnlyList<string> Synonyms,
    int Frequency,
    string? Context,
    float[]? Embedding);