using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;
using IndFusion.SemanticRag.Domain.Models;

namespace IndFusion.SemanticRag.Domain.Services;

/// <summary>
/// Service for extracting knowledge entities and relationships from documents.
/// </summary>
public interface IKnowledgeExtractionService
{
    /// <summary>
    /// Extracts knowledge entities from a document.
    /// </summary>
    /// <param name="document">The document to extract entities from.</param>
    /// <param name="options">Extraction options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the extracted entities.</returns>
    Task<Result<IReadOnlyList<KnowledgeEntity>>> ExtractEntitiesAsync(
        SemanticDocument document,
        EntityExtractionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts relationships between entities in a document.
    /// </summary>
    /// <param name="document">The document to extract relationships from.</param>
    /// <param name="entities">The entities found in the document.</param>
    /// <param name="options">Extraction options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the extracted relationships.</returns>
    Task<Result<IReadOnlyList<KnowledgeRelationship>>> ExtractRelationshipsAsync(
        SemanticDocument document,
        IReadOnlyList<KnowledgeEntity> entities,
        RelationshipExtractionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts code entities from a code document.
    /// </summary>
    /// <param name="document">The code document to extract entities from.</param>
    /// <param name="options">Code extraction options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the extracted code entities.</returns>
    Task<Result<IReadOnlyList<CodeEntity>>> ExtractCodeEntitiesAsync(
        SemanticDocument document,
        CodeExtractionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extracts semantic concepts from a document.
    /// </summary>
    /// <param name="document">The document to extract concepts from.</param>
    /// <param name="options">Concept extraction options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the extracted concepts.</returns>
    Task<Result<IReadOnlyList<SemanticConcept>>> ExtractConceptsAsync(
        SemanticDocument document,
        ConceptExtractionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs comprehensive knowledge extraction on a document.
    /// </summary>
    /// <param name="document">The document to extract knowledge from.</param>
    /// <param name="options">Comprehensive extraction options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing all extracted knowledge.</returns>
    Task<Result<KnowledgeExtractionResult>> ExtractKnowledgeAsync(
        SemanticDocument document,
        ComprehensiveExtractionOptions options,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates extracted entities for consistency and quality.
    /// </summary>
    /// <param name="entities">The entities to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing validation results.</returns>
    Task<Result<EntityValidationResult>> ValidateEntitiesAsync(
        IReadOnlyList<KnowledgeEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the supported entity types for extraction.
    /// </summary>
    /// <returns>A list of supported entity types.</returns>
    IReadOnlyList<string> GetSupportedEntityTypes();

    /// <summary>
    /// Gets the supported relationship types for extraction.
    /// </summary>
    /// <returns>A list of supported relationship types.</returns>
    IReadOnlyList<string> GetSupportedRelationshipTypes();
}
