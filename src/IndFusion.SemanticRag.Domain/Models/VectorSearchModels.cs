using System.Diagnostics.CodeAnalysis;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a vector embedding with metadata.
/// </summary>
/// <param name="Id">Unique identifier for the vector.</param>
/// <param name="Content">The original content that was embedded.</param>
/// <param name="Embedding">The vector embedding values.</param>
/// <param name="Metadata">Additional metadata associated with the vector.</param>
/// <param name="CreatedAt">When the vector was created.</param>
public readonly record struct VectorEmbedding(
    string Id,
    string Content,
    float[] Embedding,
    Dictionary<string, object> Metadata,
    DateTimeOffset CreatedAt)
{
    /// <summary>
    /// Gets the dimension of the embedding vector.
    /// </summary>
    public int Dimension => Embedding.Length;

    /// <summary>
    /// Validates the vector embedding for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the vector is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Vector ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Content))
            return Result.WithFailure("Vector content cannot be null or empty");

        if (Embedding.Length == 0)
            return Result.WithFailure("Vector embedding cannot be empty");

        if (Metadata is null)
            return Result.WithFailure("Vector metadata cannot be null");

        return Result.Success();
    }
}

/// <summary>
/// Represents a search query for vector similarity search.
/// </summary>
/// <param name="Query">The search query text.</param>
/// <param name="Embedding">The query embedding vector.</param>
/// <param name="Limit">Maximum number of results to return.</param>
/// <param name="Threshold">Minimum similarity threshold (0.0 to 1.0).</param>
/// <param name="Filters">Optional metadata filters.</param>
public readonly record struct VectorSearchQuery(
    string Query,
    float[] Embedding,
    int Limit = 10,
    float Threshold = 0.7f,
    Dictionary<string, object>? Filters = null)
{
    /// <summary>
    /// Validates the search query for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the query is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Search query cannot be null or empty");

        if (Embedding.Length == 0)
            return Result.WithFailure("Query embedding cannot be empty");

        if (Limit <= 0)
            return Result.WithFailure("Search limit must be greater than 0");

        if (Threshold < 0.0f || Threshold > 1.0f)
            return Result.WithFailure("Similarity threshold must be between 0.0 and 1.0");

        return Result.Success();
    }
}

/// <summary>
/// Represents a search result from vector similarity search.
/// </summary>
/// <param name="Vector">The matching vector embedding.</param>
/// <param name="Similarity">The similarity score (0.0 to 1.0).</param>
/// <param name="Rank">The rank of this result in the search results.</param>
public readonly record struct VectorSearchResult(
    VectorEmbedding Vector,
    float Similarity,
    int Rank)
{
    /// <summary>
    /// Validates the search result for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the result is valid.</returns>
    public Result Validate()
    {
        if (Similarity < 0.0f || Similarity > 1.0f)
            return Result.WithFailure("Similarity score must be between 0.0 and 1.0");

        if (Rank < 0)
            return Result.WithFailure("Rank must be non-negative");

        return Vector.Validate();
    }
}

/// <summary>
/// Represents a semantic pattern for matching and recognition.
/// </summary>
/// <param name="Id">Unique identifier for the pattern.</param>
/// <param name="Name">Human-readable name for the pattern.</param>
/// <param name="Description">Description of what the pattern matches.</param>
/// <param name="Pattern">The pattern definition (regex, template, etc.).</param>
/// <param name="Category">Category classification for the pattern.</param>
/// <param name="Confidence">Confidence level for pattern matching (0.0 to 1.0).</param>
/// <param name="Metadata">Additional metadata for the pattern.</param>
public readonly record struct SemanticPattern(
    string Id,
    string Name,
    string Description,
    string Pattern,
    string Category,
    float Confidence = 0.8f,
    Dictionary<string, object>? Metadata = null)
{
    /// <summary>
    /// Validates the semantic pattern for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the pattern is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Pattern ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Name))
            return Result.WithFailure("Pattern name cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Description))
            return Result.WithFailure("Pattern description cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Pattern))
            return Result.WithFailure("Pattern definition cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Category))
            return Result.WithFailure("Pattern category cannot be null or empty");

        if (Confidence < 0.0f || Confidence > 1.0f)
            return Result.WithFailure("Pattern confidence must be between 0.0 and 1.0");

        return Result.Success();
    }
}

/// <summary>
/// Represents a pattern match result.
/// </summary>
/// <param name="Pattern">The pattern that was matched.</param>
/// <param name="Match">The matched text.</param>
/// <param name="StartIndex">Starting position of the match.</param>
/// <param name="EndIndex">Ending position of the match.</param>
/// <param name="Confidence">Confidence score for the match.</param>
public readonly record struct PatternMatch(
    SemanticPattern Pattern,
    string Match,
    int StartIndex,
    int EndIndex,
    float Confidence)
{
    /// <summary>
    /// Validates the pattern match for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the match is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Match))
            return Result.WithFailure("Match text cannot be null or empty");

        if (StartIndex < 0)
            return Result.WithFailure("Start index must be non-negative");

        if (EndIndex < StartIndex)
            return Result.WithFailure("End index must be greater than or equal to start index");

        if (Confidence < 0.0f || Confidence > 1.0f)
            return Result.WithFailure("Match confidence must be between 0.0 and 1.0");

        return Pattern.Validate();
    }
}

/// <summary>
/// Represents a knowledge graph node.
/// </summary>
/// <param name="Id">Unique identifier for the node.</param>
/// <param name="Label">The node label/type.</param>
/// <param name="Properties">Properties associated with the node.</param>
public readonly record struct GraphNode(
    string Id,
    string Label,
    Dictionary<string, object> Properties)
{
    /// <summary>
    /// Validates the graph node for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the node is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Label))
            return Result.WithFailure("Node label cannot be null or empty");

        if (Properties is null)
            return Result.WithFailure("Node properties cannot be null");

        return Result.Success();
    }
}

/// <summary>
/// Represents a knowledge graph edge/relationship.
/// </summary>
/// <param name="Id">Unique identifier for the edge.</param>
/// <param name="SourceNodeId">ID of the source node.</param>
/// <param name="TargetNodeId">ID of the target node.</param>
/// <param name="Relationship">The type of relationship.</param>
/// <param name="Properties">Properties associated with the edge.</param>
public readonly record struct GraphEdge(
    string Id,
    string SourceNodeId,
    string TargetNodeId,
    string Relationship,
    Dictionary<string, object> Properties)
{
    /// <summary>
    /// Validates the graph edge for consistency.
    /// </summary>
    /// <returns>A Result indicating whether the edge is valid.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Edge ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(SourceNodeId))
            return Result.WithFailure("Source node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(TargetNodeId))
            return Result.WithFailure("Target node ID cannot be null or empty");

        if (string.IsNullOrWhiteSpace(Relationship))
            return Result.WithFailure("Edge relationship cannot be null or empty");

        if (Properties is null)
            return Result.WithFailure("Edge properties cannot be null");

        return Result.Success();
    }
}