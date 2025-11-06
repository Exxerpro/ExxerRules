using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;

namespace IndFusion.SemanticRag.Application.Services;

/// <summary>
/// Options for comprehensive search operations.
/// </summary>
/// <param name="SearchOptions">Basic search options.</param>
/// <param name="RagConfig">RAG configuration.</param>
/// <param name="EnableKnowledgeExtraction">Whether to extract knowledge from results.</param>
/// <param name="MaxResultsForExtraction">Maximum number of results to extract knowledge from.</param>
/// <param name="ExtractionOptions">Knowledge extraction options.</param>
/// <param name="EnableContextRetrieval">Whether to retrieve additional context.</param>
public readonly record struct ComprehensiveSearchOptions(
    SemanticSearchOptions SearchOptions,
    SemanticRagConfig RagConfig,
    bool EnableKnowledgeExtraction = true,
    int MaxResultsForExtraction = 5,
    ComprehensiveExtractionOptions ExtractionOptions = default,
    bool EnableContextRetrieval = true)
{
    /// <summary>
    /// Default comprehensive search options.
    /// </summary>
    public static ComprehensiveSearchOptions Default() => new(
        SearchOptions: new SemanticSearchOptions(),
        RagConfig: new SemanticRagConfig(
            Id: "default",
            Name: "Default Configuration",
            EmbeddingModel: "text-embedding-ada-002",
            VectorDimensions: 1536,
            SimilarityThreshold: 0.7,
            MaxResults: 10,
            Properties: []
        ),
        EnableKnowledgeExtraction: true,
        MaxResultsForExtraction: 5,
        ExtractionOptions: ComprehensiveExtractionOptions.Default(),
        EnableContextRetrieval: true);
}