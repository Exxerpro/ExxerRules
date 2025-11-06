using System;
using System.Collections.Generic;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a search result from the semantic RAG system.
/// </summary>
/// <param name="DocumentId">ID of the matched document.</param>
/// <param name="Content">The matched content snippet.</param>
/// <param name="Score">The relevance score for this result.</param>
/// <param name="SourcePath">Path to the source file.</param>
/// <param name="Repository">Repository name.</param>
/// <param name="CommitHash">Git commit hash.</param>
/// <param name="Metadata">Additional metadata for the result.</param>
public record SearchResult(
    string DocumentId,
    string Content,
    float Score,
    string SourcePath,
    string Repository,
    string CommitHash,
    IReadOnlyDictionary<string, object> Metadata)
{
    /// <summary>
    /// Gets the document identifier.
    /// </summary>
    public string DocumentId { get; init; } = DocumentId;

    /// <summary>
    /// Gets the content snippet.
    /// </summary>
    public string Content { get; init; } = Content;

    /// <summary>
    /// Gets the relevance score.
    /// </summary>
    public float Score { get; init; } = Score;

    /// <summary>
    /// Gets the source path.
    /// </summary>
    public string SourcePath { get; init; } = SourcePath;

    /// <summary>
    /// Gets the repository name.
    /// </summary>
    public string Repository { get; init; } = Repository;

    /// <summary>
    /// Gets the commit hash.
    /// </summary>
    public string CommitHash { get; init; } = CommitHash;

    /// <summary>
    /// Gets the result metadata.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = Metadata;

    /// <summary>
    /// Validates the search result for basic requirements.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(DocumentId))
            return Result.WithFailure("Document ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(Content))
            return Result.WithFailure("Content cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(SourcePath))
            return Result.WithFailure("Source path cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(Repository))
            return Result.WithFailure("Repository cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(CommitHash))
            return Result.WithFailure("Commit hash cannot be empty or whitespace");

        if (Score < 0.0f || Score > 1.0f)
            return Result.WithFailure("Score must be between 0.0 and 1.0");

        return Result.Success();
    }
}
