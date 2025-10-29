using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port for document processing operations in the Semantic RAG system.
/// This defines the contract for OCR, entity extraction, and content processing.
/// </summary>
public interface IDocumentProcessingPort
{
    /// <summary>
    /// Processes a document using OCR and extracts text content.
    /// </summary>
    /// <param name="document">Document to process.</param>
    /// <param name="options">Processing options including OCR settings.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processed document with extracted text and metadata.</returns>
    Task<ProcessedDocument> ProcessDocumentAsync(DocumentInput document, DocumentProcessingOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts entities from text content using LLM services.
    /// </summary>
    /// <param name="text">Text content to analyze.</param>
    /// <param name="options">Entity extraction options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of extracted entities with confidence scores.</returns>
    Task<IEnumerable<ExtractedEntity>> ExtractEntitiesAsync(string text, EntityExtractionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Maps relationships between extracted entities.
    /// </summary>
    /// <param name="entities">Collection of entities to analyze.</param>
    /// <param name="options">Relationship mapping options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of mapped relationships.</returns>
    Task<IEnumerable<EntityRelationship>> MapEntityRelationshipsAsync(IEnumerable<ExtractedEntity> entities, RelationshipMappingOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates embeddings for text content.
    /// </summary>
    /// <param name="text">Text content to embed.</param>
    /// <param name="options">Embedding generation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated vector embedding.</returns>
    Task<VectorEmbedding> GenerateEmbeddingAsync(string text, EmbeddingOptions options, CancellationToken cancellationToken = default);
}

/// <summary>
/// Port for analyzer integration in the Semantic RAG system.
/// This defines the contract for running analyzers and processing diagnostics.
/// </summary>
public interface IAnalyzerIntegrationPort
{
    /// <summary>
    /// Runs analyzers on a solution and returns diagnostics.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file.</param>
    /// <param name="options">Analyzer execution options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of analyzer diagnostics.</returns>
    Task<IEnumerable<AnalyzerDiagnostic>> RunAnalyzersAsync(string solutionPath, AnalyzerExecutionOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies code fixes for specific diagnostics.
    /// </summary>
    /// <param name="diagnostics">Diagnostics to fix.</param>
    /// <param name="options">Fix application options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result containing applied fixes and validation results.</returns>
    Task<CodeFixResult> ApplyCodeFixesAsync(IEnumerable<AnalyzerDiagnostic> diagnostics, CodeFixOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates that applied fixes don't introduce new issues.
    /// </summary>
    /// <param name="originalCode">Original code before fixes.</param>
    /// <param name="fixedCode">Code after applying fixes.</param>
    /// <param name="options">Validation options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Validation result with any new issues found.</returns>
    Task<FixValidationResult> ValidateFixesAsync(string originalCode, string fixedCode, FixValidationOptions options, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets available analyzers and their capabilities.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Collection of available analyzers with metadata.</returns>
    Task<IEnumerable<AnalyzerMetadata>> GetAvailableAnalyzersAsync(CancellationToken cancellationToken = default);
}