namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Options for document processing operations.
/// </summary>
/// <param name="ExtractMetadata">Whether to extract metadata.</param>
/// <param name="EnableOcr">Whether to enable OCR processing.</param>
/// <param name="OcrLanguage">Language for OCR processing.</param>
/// <param name="EnableChunking">Whether to enable chunking.</param>
/// <param name="ChunkingStrategy">Strategy for chunking documents.</param>
/// <param name="MaxChunkSize">Maximum size for chunks.</param>
/// <param name="CustomSettings">Custom processing settings.</param>
public record DocumentProcessingOptions(
    bool ExtractMetadata = true,
    bool EnableOcr = true,
    string OcrLanguage = "eng",
    bool EnableChunking = true,
    ChunkingStrategy ChunkingStrategy = ChunkingStrategy.Recursive,
    int MaxChunkSize = 1000,
    IReadOnlyDictionary<string, object>? CustomSettings = null
);