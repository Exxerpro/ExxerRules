namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Result of repository ingestion operations.
/// </summary>
/// <param name="ProcessedDocuments">Documents that were successfully processed.</param>
/// <param name="TotalDocuments">Total number of documents found.</param>
/// <param name="ExtractedKnowledge">Knowledge extracted from documents.</param>
/// <param name="ProcessingTimeMs">Time taken for processing in milliseconds.</param>
/// <param name="Success">Whether the ingestion was successful.</param>
public record RepositoryIngestionResult(
    IReadOnlyList<SemanticDocument> ProcessedDocuments,
    int TotalDocuments,
    IReadOnlyList<KnowledgeExtractionResult> ExtractedKnowledge,
    long ProcessingTimeMs,
    bool Success
);