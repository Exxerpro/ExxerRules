using System;
using System.Collections.Generic;
using System.Linq;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a semantic document in the RAG system.
/// </summary>
/// <param name="Id">Unique identifier for the document.</param>
/// <param name="Content">The text content of the document.</param>
/// <param name="Metadata">Additional metadata about the document.</param>
/// <param name="Embedding">Vector embedding for semantic search.</param>
/// <param name="CreatedAt">When the document was created.</param>
/// <param name="UpdatedAt">When the document was last updated.</param>
public readonly record struct SemanticDocument(
    string Id,
    string Content,
    IReadOnlyDictionary<string, object> Metadata,
    float[]? Embedding,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    /// <summary>
    /// Gets the document type from metadata.
    /// </summary>
    public string DocumentType => Metadata.GetValueOrDefault("type", "unknown").ToString() ?? "unknown";

    /// <summary>
    /// Gets the source of the document from metadata.
    /// </summary>
    public string Source => Metadata.GetValueOrDefault("source", "unknown").ToString() ?? "unknown";

    /// <summary>
    /// Gets the language of the document from metadata.
    /// </summary>
    public string Language => Metadata.GetValueOrDefault("language", "en").ToString() ?? "en";

    /// <summary>
    /// Checks if the document has an embedding.
    /// </summary>
    public bool HasEmbedding => Embedding is not null && Embedding.Length > 0;
}

/// <summary>
/// Represents a semantic search query.
/// </summary>
/// <param name="Query">The search query text.</param>
/// <param name="Filters">Optional filters to apply to the search.</param>
/// <param name="Limit">Maximum number of results to return.</param>
/// <param name="Threshold">Similarity threshold for results (0.0 to 1.0).</param>
/// <param name="IncludeMetadata">Whether to include metadata in results.</param>
public readonly record struct SemanticSearchQuery(
    string Query,
    IReadOnlyDictionary<string, object>? Filters = null,
    int Limit = 10,
    float Threshold = 0.7f,
    bool IncludeMetadata = true)
{
    /// <summary>
    /// Validates the search query parameters.
    /// </summary>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Query cannot be null or empty");

        if (Limit <= 0)
            return Result.WithFailure("Limit must be greater than 0");

        if (Threshold < 0.0f || Threshold > 1.0f)
            return Result.WithFailure("Threshold must be between 0.0 and 1.0");

        return Result.Success();
    }
}

/// <summary>
/// Represents a semantic search result.
/// </summary>
/// <param name="Document">The matching document.</param>
/// <param name="Score">Similarity score (0.0 to 1.0).</param>
/// <param name="Highlights">Text highlights showing why this result matched.</param>
public readonly record struct SemanticSearchResult(
    SemanticDocument Document,
    float Score,
    IReadOnlyList<string> Highlights)
{
    /// <summary>
    /// Checks if this result meets the minimum threshold.
    /// </summary>
    /// <param name="threshold">The minimum score threshold.</param>
    /// <returns>True if the score meets the threshold.</returns>
    public bool MeetsThreshold(float threshold) => Score >= threshold;
}

/// <summary>
/// Represents a knowledge entity in the semantic graph.
/// </summary>
/// <param name="Id">Unique identifier for the entity.</param>
/// <param name="Type">The type of the entity (e.g., "Person", "Concept", "Document").</param>
/// <param name="Name">The name or title of the entity.</param>
/// <param name="Description">Description of the entity.</param>
/// <param name="Properties">Additional properties of the entity.</param>
/// <param name="Embedding">Vector embedding for semantic similarity.</param>
public readonly record struct KnowledgeEntity(
    string Id,
    string Type,
    string Name,
    string? Description,
    IReadOnlyDictionary<string, object> Properties,
    float[]? Embedding)
{
    /// <summary>
    /// Gets the display name for the entity.
    /// </summary>
    public string DisplayName => !string.IsNullOrWhiteSpace(Description) 
        ? $"{Name}: {Description}" 
        : Name;

    /// <summary>
    /// Checks if the entity has an embedding.
    /// </summary>
    public bool HasEmbedding => Embedding is not null && Embedding.Length > 0;
}

/// <summary>
/// Represents a semantic context for RAG operations.
/// </summary>
/// <param name="Documents">Relevant documents for the context.</param>
/// <param name="Entities">Relevant knowledge entities.</param>
/// <param name="Relationships">Relevant relationships between entities.</param>
/// <param name="Query">The original query that generated this context.</param>
/// <param name="Confidence">Overall confidence score for the context (0.0 to 1.0).</param>
public readonly record struct SemanticContext(
    IReadOnlyList<SemanticDocument> Documents,
    IReadOnlyList<KnowledgeEntity> Entities,
    IReadOnlyList<KnowledgeRelationship> Relationships,
    string Query,
    float Confidence)
{
    /// <summary>
    /// Gets the total number of context items.
    /// </summary>
    public int TotalItems => Documents.Count + Entities.Count + Relationships.Count;

    /// <summary>
    /// Checks if the context has sufficient information.
    /// </summary>
    /// <param name="minimumItems">Minimum number of items required.</param>
    /// <returns>True if the context has sufficient items.</returns>
    public bool HasSufficientContext(int minimumItems = 3) => TotalItems >= minimumItems;

    /// <summary>
    /// Gets a summary of the context for display purposes.
    /// </summary>
    public string GetSummary()
    {
        var docCount = Documents.Count;
        var entityCount = Entities.Count;
        var relCount = Relationships.Count;
        
        return $"Context: {docCount} documents, {entityCount} entities, {relCount} relationships (confidence: {Confidence:P1})";
    }
}

/// <summary>
/// Represents configuration for semantic RAG operations.
/// </summary>
/// <param name="MaxDocuments">Maximum number of documents to retrieve.</param>
/// <param name="MaxEntities">Maximum number of entities to retrieve.</param>
/// <param name="SimilarityThreshold">Minimum similarity threshold for results.</param>
/// <param name="EnableGraphTraversal">Whether to enable graph traversal for context.</param>
/// <param name="MaxTraversalDepth">Maximum depth for graph traversal.</param>
/// <param name="EnableHybridSearch">Whether to enable hybrid search (vector + keyword).</param>
public readonly record struct SemanticRagConfig(
    int MaxDocuments = 10,
    int MaxEntities = 5,
    float SimilarityThreshold = 0.7f,
    bool EnableGraphTraversal = true,
    int MaxTraversalDepth = 2,
    bool EnableHybridSearch = true)
{
    /// <summary>
    /// Validates the configuration parameters.
    /// </summary>
    public Result Validate()
    {
        if (MaxDocuments <= 0)
            return Result.WithFailure("MaxDocuments must be greater than 0");

        if (MaxEntities < 0)
            return Result.WithFailure("MaxEntities cannot be negative");

        if (SimilarityThreshold < 0.0f || SimilarityThreshold > 1.0f)
            return Result.WithFailure("SimilarityThreshold must be between 0.0 and 1.0");

        if (MaxTraversalDepth < 0)
            return Result.WithFailure("MaxTraversalDepth cannot be negative");

        return Result.Success();
    }
}