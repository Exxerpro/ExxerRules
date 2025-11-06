namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Represents the summary of an ingestion operation.
/// </summary>
/// <param name="RepositoryName">Name of the repository.</param>
/// <param name="CommitHash">Git commit hash.</param>
/// <param name="DocumentsProcessed">Number of documents processed.</param>
/// <param name="EmbeddingsGenerated">Number of embeddings generated.</param>
/// <param name="KnowledgeNodesCreated">Number of knowledge graph nodes created.</param>
/// <param name="ProcessingTime">Time taken for processing in milliseconds.</param>
/// <param name="Errors">List of errors encountered during processing.</param>
public record IngestionSummary(
    string RepositoryName,
    string CommitHash,
    int DocumentsProcessed,
    int EmbeddingsGenerated,
    int KnowledgeNodesCreated,
    long ProcessingTime,
    IReadOnlyList<string> Errors)
{
    /// <summary>
    /// Gets the repository name.
    /// </summary>
    public string RepositoryName { get; init; } = RepositoryName;

    /// <summary>
    /// Gets the commit hash.
    /// </summary>
    public string CommitHash { get; init; } = CommitHash;

    /// <summary>
    /// Gets the number of documents processed.
    /// </summary>
    public int DocumentsProcessed { get; init; } = DocumentsProcessed;

    /// <summary>
    /// Gets the number of embeddings generated.
    /// </summary>
    public int EmbeddingsGenerated { get; init; } = EmbeddingsGenerated;

    /// <summary>
    /// Gets the number of knowledge graph nodes created.
    /// </summary>
    public int KnowledgeNodesCreated { get; init; } = KnowledgeNodesCreated;

    /// <summary>
    /// Gets the processing time in milliseconds.
    /// </summary>
    public long ProcessingTime { get; init; } = ProcessingTime;

    /// <summary>
    /// Gets the list of errors.
    /// </summary>
    public IReadOnlyList<string> Errors { get; init; } = Errors;

    /// <summary>
    /// Gets whether the ingestion was successful.
    /// </summary>
    public bool IsSuccessful => Errors.Count == 0;
}