namespace IndFusion.SemanticRag.Application.Interfaces;

/// <summary>
/// Represents a RAG response with generated answer and source documents.
/// </summary>
public record RagResponse
{
    /// <summary>
    /// The generated answer to the query.
    /// </summary>
    public required string Answer { get; init; }

    /// <summary>
    /// Source documents used to generate the answer.
    /// </summary>
    public required IReadOnlyList<RagSource> Sources { get; init; }

    /// <summary>
    /// Confidence score for the answer (0-1).
    /// </summary>
    public required float Confidence { get; init; }

    /// <summary>
    /// Time taken to generate the response in milliseconds.
    /// </summary>
    public required long ElapsedMilliseconds { get; init; }

    /// <summary>
    /// The original query.
    /// </summary>
    public required string Query { get; init; }
}