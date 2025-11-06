namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for RAG query operations.
/// </summary>
/// <param name="Question">Question to ask the RAG system.</param>
/// <param name="Context">Additional context for the question.</param>
/// <param name="MaxTokens">Maximum number of tokens in response.</param>
/// <param name="Temperature">Temperature for response generation.</param>
/// <param name="IncludeSources">Whether to include source citations.</param>
public record RagQueryRequest(
    string Question,
    Dictionary<string, object>? Context = null,
    int MaxTokens = 1000,
    double Temperature = 0.7,
    bool IncludeSources = true
);