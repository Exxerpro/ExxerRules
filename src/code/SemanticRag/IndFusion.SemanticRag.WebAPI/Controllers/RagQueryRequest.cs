namespace IndFusion.SemanticRag.WebAPI.Controllers;

/// <summary>
/// Request model for RAG queries.
/// </summary>
public record RagQueryRequest
{
    /// <summary>
    /// The query to process.
    /// </summary>
    public required string Query { get; init; }

    /// <summary>
    /// Additional context for the query.
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Maximum number of relevant documents to retrieve.
    /// </summary>
    public int MaxResults { get; init; } = 5;
}