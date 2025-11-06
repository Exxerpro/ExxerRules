namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for document ingestion operations.
/// </summary>
/// <param name="EnableVectorIndexing">Whether to enable vector indexing.</param>
/// <param name="EnableKnowledgeGraph">Whether to enable knowledge graph creation.</param>
/// <param name="EnableEntityExtraction">Whether to enable entity extraction.</param>
/// <param name="EnableRelationshipMapping">Whether to enable relationship mapping.</param>
/// <param name="ProcessingOptions">Document processing options.</param>
/// <param name="CustomSettings">Custom ingestion settings.</param>
public record DocumentIngestionOptions(
    bool EnableVectorIndexing = true,
    bool EnableKnowledgeGraph = true,
    bool EnableEntityExtraction = true,
    bool EnableRelationshipMapping = true,
    DocumentProcessingOptions? ProcessingOptions = null,
    IReadOnlyDictionary<string, object>? CustomSettings = null)
{
    /// <summary>
    /// Default document ingestion options.
    /// </summary>
    public static DocumentIngestionOptions Default() => new(
        ProcessingOptions: new DocumentProcessingOptions()
    );
}
