namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Represents the ingestion status for a repository.
/// </summary>
/// <param name="RepositoryName">Name of the repository.</param>
/// <param name="LastIngested">Timestamp of last ingestion.</param>
/// <param name="CommitHash">Last ingested commit hash.</param>
/// <param name="DocumentCount">Number of documents ingested.</param>
/// <param name="IsUpToDate">Whether the ingestion is up to date.</param>
public record IngestionStatus(
    string RepositoryName,
    DateTimeOffset LastIngested,
    string CommitHash,
    int DocumentCount,
    bool IsUpToDate)
{
    /// <summary>
    /// Gets the repository name.
    /// </summary>
    public string RepositoryName { get; init; } = RepositoryName;

    /// <summary>
    /// Gets the last ingestion timestamp.
    /// </summary>
    public DateTimeOffset LastIngested { get; init; } = LastIngested;

    /// <summary>
    /// Gets the last ingested commit hash.
    /// </summary>
    public string CommitHash { get; init; } = CommitHash;

    /// <summary>
    /// Gets the document count.
    /// </summary>
    public int DocumentCount { get; init; } = DocumentCount;

    /// <summary>
    /// Gets whether the ingestion is up to date.
    /// </summary>
    public bool IsUpToDate { get; init; } = IsUpToDate;
}