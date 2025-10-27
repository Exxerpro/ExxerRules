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

/// <summary>
/// Options for entity extraction.
/// </summary>
/// <param name="EntityTypes">Types of entities to extract.</param>
/// <param name="MinConfidence">Minimum confidence threshold for entities.</param>
/// <param name="MaxEntities">Maximum number of entities to extract.</param>
/// <param name="IncludeContext">Whether to include surrounding context.</param>
/// <param name="EnableNestedExtraction">Whether to extract nested entities.</param>
public readonly record struct EntityExtractionOptions(
    IReadOnlyList<string> EntityTypes,
    float MinConfidence = 0.7f,
    int MaxEntities = 100,
    bool IncludeContext = true,
    bool EnableNestedExtraction = true)
{
    /// <summary>
    /// Default options for general entity extraction.
    /// </summary>
    public static EntityExtractionOptions Default() => new(
        EntityTypes: new[] { "PERSON", "ORGANIZATION", "LOCATION", "CONCEPT", "TECHNOLOGY" },
        MinConfidence: 0.7f,
        MaxEntities: 50,
        IncludeContext: true,
        EnableNestedExtraction: true);

    /// <summary>
    /// Options for code entity extraction.
    /// </summary>
    public static EntityExtractionOptions ForCode() => new(
        EntityTypes: new[] { "CLASS", "METHOD", "INTERFACE", "NAMESPACE", "PROPERTY", "FIELD" },
        MinConfidence: 0.8f,
        MaxEntities: 200,
        IncludeContext: true,
        EnableNestedExtraction: true);
}

/// <summary>
/// Options for relationship extraction.
/// </summary>
/// <param name="RelationshipTypes">Types of relationships to extract.</param>
/// <param name="MinConfidence">Minimum confidence threshold for relationships.</param>
/// <param name="MaxRelationships">Maximum number of relationships to extract.</param>
/// <param name="EnableBidirectional">Whether to extract bidirectional relationships.</param>
/// <param name="IncludeWeight">Whether to calculate relationship weights.</param>
public readonly record struct RelationshipExtractionOptions(
    IReadOnlyList<string> RelationshipTypes,
    float MinConfidence = 0.6f,
    int MaxRelationships = 50,
    bool EnableBidirectional = true,
    bool IncludeWeight = true)
{
    /// <summary>
    /// Default options for general relationship extraction.
    /// </summary>
    public static RelationshipExtractionOptions Default() => new(
        RelationshipTypes: new[] { "RELATES_TO", "CONTAINS", "SIMILAR_TO", "DEPENDS_ON", "IMPLEMENTS" },
        MinConfidence: 0.6f,
        MaxRelationships: 30,
        EnableBidirectional: true,
        IncludeWeight: true);
}

/// <summary>
/// Options for code entity extraction.
/// </summary>
/// <param name="ExtractClasses">Whether to extract classes.</param>
/// <param name="ExtractMethods">Whether to extract methods.</param>
/// <param name="ExtractInterfaces">Whether to extract interfaces.</param>
/// <param name="ExtractProperties">Whether to extract properties.</param>
/// <param name="ExtractFields">Whether to extract fields.</param>
/// <param name="ExtractNamespaces">Whether to extract namespaces.</param>
/// <param name="IncludeAccessModifiers">Whether to include access modifiers.</param>
/// <param name="IncludeParameters">Whether to include method parameters.</param>
/// <param name="IncludeReturnTypes">Whether to include return types.</param>
public readonly record struct CodeExtractionOptions(
    bool ExtractClasses = true,
    bool ExtractMethods = true,
    bool ExtractInterfaces = true,
    bool ExtractProperties = true,
    bool ExtractFields = true,
    bool ExtractNamespaces = true,
    bool IncludeAccessModifiers = true,
    bool IncludeParameters = true,
    bool IncludeReturnTypes = true)
{
    /// <summary>
    /// Default options for comprehensive code extraction.
    /// </summary>
    public static CodeExtractionOptions Comprehensive() => new(
        ExtractClasses: true,
        ExtractMethods: true,
        ExtractInterfaces: true,
        ExtractProperties: true,
        ExtractFields: true,
        ExtractNamespaces: true,
        IncludeAccessModifiers: true,
        IncludeParameters: true,
        IncludeReturnTypes: true);

    /// <summary>
    /// Options for minimal code extraction.
    /// </summary>
    public static CodeExtractionOptions Minimal() => new(
        ExtractClasses: true,
        ExtractMethods: false,
        ExtractInterfaces: false,
        ExtractProperties: false,
        ExtractFields: false,
        ExtractNamespaces: true,
        IncludeAccessModifiers: false,
        IncludeParameters: false,
        IncludeReturnTypes: false);
}

/// <summary>
/// Options for concept extraction.
/// </summary>
/// <param name="MinFrequency">Minimum frequency for concepts.</param>
/// <param name="MaxConcepts">Maximum number of concepts to extract.</param>
/// <param name="IncludeSynonyms">Whether to include synonyms.</param>
/// <param name="IncludeDefinitions">Whether to include concept definitions.</param>
/// <param name="MinLength">Minimum concept length in characters.</param>
/// <param name="MaxLength">Maximum concept length in characters.</param>
public readonly record struct ConceptExtractionOptions(
    int MinFrequency = 2,
    int MaxConcepts = 50,
    bool IncludeSynonyms = true,
    bool IncludeDefinitions = true,
    int MinLength = 3,
    int MaxLength = 50)
{
    /// <summary>
    /// Validates the concept extraction options.
    /// </summary>
    public Result Validate()
    {
        if (MinFrequency < 1)
            return Result.WithFailure("MinFrequency must be at least 1");

        if (MaxConcepts <= 0)
            return Result.WithFailure("MaxConcepts must be greater than 0");

        if (MinLength < 1)
            return Result.WithFailure("MinLength must be at least 1");

        if (MaxLength < MinLength)
            return Result.WithFailure("MaxLength must be greater than or equal to MinLength");

        return Result.Success();
    }
}

/// <summary>
/// Comprehensive options for knowledge extraction.
/// </summary>
/// <param name="EntityOptions">Entity extraction options.</param>
/// <param name="RelationshipOptions">Relationship extraction options.</param>
/// <param name="CodeOptions">Code extraction options.</param>
/// <param name="ConceptOptions">Concept extraction options.</param>
/// <param name="EnableParallelProcessing">Whether to enable parallel processing.</param>
/// <param name="MaxProcessingTimeMs">Maximum processing time in milliseconds.</param>
public readonly record struct ComprehensiveExtractionOptions(
    EntityExtractionOptions EntityOptions,
    RelationshipExtractionOptions RelationshipOptions,
    CodeExtractionOptions CodeOptions,
    ConceptExtractionOptions ConceptOptions,
    bool EnableParallelProcessing = true,
    int MaxProcessingTimeMs = 30000)
{
    /// <summary>
    /// Default comprehensive extraction options.
    /// </summary>
    public static ComprehensiveExtractionOptions Default() => new(
        EntityOptions: EntityExtractionOptions.Default(),
        RelationshipOptions: RelationshipExtractionOptions.Default(),
        CodeOptions: CodeExtractionOptions.Comprehensive(),
        ConceptOptions: new ConceptExtractionOptions(),
        EnableParallelProcessing: true,
        MaxProcessingTimeMs: 30000);
}

/// <summary>
/// Result of knowledge extraction.
/// </summary>
/// <param name="Entities">Extracted entities.</param>
/// <param name="Relationships">Extracted relationships.</param>
/// <param name="CodeEntities">Extracted code entities.</param>
/// <param name="Concepts">Extracted concepts.</param>
/// <param name="ProcessingTimeMs">Time taken for extraction in milliseconds.</param>
/// <param name="Confidence">Overall confidence score for the extraction.</param>
public readonly record struct KnowledgeExtractionResult(
    IReadOnlyList<KnowledgeEntity> Entities,
    IReadOnlyList<KnowledgeRelationship> Relationships,
    IReadOnlyList<CodeEntity> CodeEntities,
    IReadOnlyList<SemanticConcept> Concepts,
    long ProcessingTimeMs,
    float Confidence)
{
    /// <summary>
    /// Gets the total number of extracted items.
    /// </summary>
    public int TotalItems => Entities.Count + Relationships.Count + CodeEntities.Count + Concepts.Count;

    /// <summary>
    /// Checks if any knowledge was extracted.
    /// </summary>
    public bool HasKnowledge => TotalItems > 0;
}

/// <summary>
/// Represents a code entity (class, method, property, etc.).
/// </summary>
/// <param name="Id">Unique identifier for the code entity.</param>
/// <param name="Type">Type of the code entity (CLASS, METHOD, etc.).</param>
/// <param name="Name">Name of the code entity.</param>
/// <param name="FullName">Fully qualified name.</param>
/// <param name="Namespace">Namespace containing the entity.</param>
/// <param name="AccessModifier">Access modifier (public, private, etc.).</param>
/// <param name="Parameters">Method parameters (if applicable).</param>
/// <param name="ReturnType">Return type (if applicable).</param>
/// <param name="Properties">Additional properties.</param>
/// <param name="Embedding">Vector embedding for semantic similarity.</param>
public readonly record struct CodeEntity(
    string Id,
    string Type,
    string Name,
    string FullName,
    string? Namespace,
    string? AccessModifier,
    IReadOnlyList<CodeParameter>? Parameters,
    string? ReturnType,
    IReadOnlyDictionary<string, object> Properties,
    float[]? Embedding)
{
    /// <summary>
    /// Gets the display name for the code entity.
    /// </summary>
    public string DisplayName => !string.IsNullOrWhiteSpace(Namespace) 
        ? $"{Namespace}.{Name}" 
        : Name;
}

/// <summary>
/// Represents a code parameter.
/// </summary>
/// <param name="Name">Parameter name.</param>
/// <param name="Type">Parameter type.</param>
/// <param name="IsOptional">Whether the parameter is optional.</param>
/// <param name="DefaultValue">Default value (if any).</param>
public readonly record struct CodeParameter(
    string Name,
    string Type,
    bool IsOptional = false,
    string? DefaultValue = null);

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

/// <summary>
/// Result of entity validation.
/// </summary>
/// <param name="ValidEntities">Entities that passed validation.</param>
/// <param name="InvalidEntities">Entities that failed validation.</param>
/// <param name="ValidationErrors">Validation error messages.</param>
/// <param name="OverallQuality">Overall quality score (0.0 to 1.0).</param>
public readonly record struct EntityValidationResult(
    IReadOnlyList<KnowledgeEntity> ValidEntities,
    IReadOnlyList<KnowledgeEntity> InvalidEntities,
    IReadOnlyList<string> ValidationErrors,
    float OverallQuality);