namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Query for semantic search operations.
/// </summary>
/// <param name="Id">Unique identifier for the query.</param>
/// <param name="Text">Text to search for.</param>
/// <param name="Filters">Additional filters to apply.</param>
/// <param name="MaxResults">Maximum number of results to return.</param>
/// <param name="MinScore">Minimum similarity score threshold.</param>
public record SemanticSearchQuery(
    string Id,
    string Text,
    Dictionary<string, object> Filters,
    int MaxResults = 10,
    double MinScore = 0.7
);

/// <summary>
/// Result of semantic search operations.
/// </summary>
/// <param name="Id">Unique identifier for the search result.</param>
/// <param name="QueryId">ID of the original query.</param>
/// <param name="Results">Search results.</param>
/// <param name="Document">Primary document for this search result.</param>
/// <param name="TotalCount">Total number of results available.</param>
/// <param name="QueryTime">Time taken to execute the query.</param>
/// <param name="Metadata">Additional search metadata.</param>
public record SemanticSearchResult(
    string Id,
    string QueryId,
    IEnumerable<SearchResultItem> Results,
    SemanticDocument? Document,
    long TotalCount,
    TimeSpan QueryTime,
    Dictionary<string, object> Metadata
);

/// <summary>
/// Individual item in a search result.
/// </summary>
/// <param name="Id">Unique identifier for the result item.</param>
/// <param name="Content">Content of the result item.</param>
/// <param name="Score">Similarity score.</param>
/// <param name="Metadata">Additional metadata for the item.</param>
/// <param name="Source">Source of the result item.</param>
public record SearchResultItem(
    string Id,
    string Content,
    double Score,
    Dictionary<string, object> Metadata,
    string Source
);

/// <summary>
/// Context for semantic operations.
/// </summary>
/// <param name="Id">Unique identifier for the context.</param>
/// <param name="Name">Name of the context.</param>
/// <param name="Description">Description of the context.</param>
/// <param name="Documents">Documents in this context.</param>
/// <param name="Entities">Entities in this context.</param>
/// <param name="Relationships">Relationships in this context.</param>
/// <param name="Properties">Additional context properties.</param>
/// <param name="CreatedAt">When the context was created.</param>
public record SemanticContext(
    string Id,
    string Name,
    string Description,
    IEnumerable<SemanticDocument> Documents,
    IEnumerable<KnowledgeEntity> Entities,
    IEnumerable<EntityRelationship> Relationships,
    Dictionary<string, object> Properties,
    DateTime CreatedAt
);

/// <summary>
/// Configuration for Semantic RAG operations.
/// </summary>
/// <param name="Id">Unique identifier for the configuration.</param>
/// <param name="Name">Name of the configuration.</param>
/// <param name="EmbeddingModel">Model to use for embeddings.</param>
/// <param name="VectorDimensions">Number of dimensions for vectors.</param>
/// <param name="SimilarityThreshold">Default similarity threshold.</param>
/// <param name="MaxResults">Default maximum results.</param>
/// <param name="Properties">Additional configuration properties.</param>
public record SemanticRagConfig(
    string Id,
    string Name,
    string EmbeddingModel,
    int VectorDimensions,
    double SimilarityThreshold,
    int MaxResults,
    Dictionary<string, object> Properties
);
