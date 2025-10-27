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

/// <summary>
/// Represents a collection of search results with metadata.
/// </summary>
/// <param name="Results">The search results.</param>
/// <param name="TotalCount">Total number of results available.</param>
/// <param name="Query">The original search query.</param>
/// <param name="SearchTime">Time taken to perform the search in milliseconds.</param>
/// <param name="Metadata">Additional search metadata.</param>
public record SearchResultCollection(
    IReadOnlyList<SearchResult> Results,
    int TotalCount,
    string Query,
    long SearchTime,
    IReadOnlyDictionary<string, object> Metadata)
{
    /// <summary>
    /// Gets the search results.
    /// </summary>
    public IReadOnlyList<SearchResult> Results { get; init; } = Results;

    /// <summary>
    /// Gets the total count of available results.
    /// </summary>
    public int TotalCount { get; init; } = TotalCount;

    /// <summary>
    /// Gets the original search query.
    /// </summary>
    public string Query { get; init; } = Query;

    /// <summary>
    /// Gets the search time in milliseconds.
    /// </summary>
    public long SearchTime { get; init; } = SearchTime;

    /// <summary>
    /// Gets the search metadata.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = Metadata;

    /// <summary>
    /// Gets the number of results in this collection.
    /// </summary>
    public int Count => Results.Count;

    /// <summary>
    /// Gets a value indicating whether there are any results.
    /// </summary>
    public bool HasResults => Results.Count > 0;

    /// <summary>
    /// Validates the search result collection for basic requirements.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Query))
            return Result.WithFailure("Query cannot be empty or whitespace");

        if (TotalCount < 0)
            return Result.WithFailure("Total count cannot be negative");

        if (SearchTime < 0)
            return Result.WithFailure("Search time cannot be negative");

        if (Results.Count > TotalCount)
            return Result.WithFailure("Results count cannot exceed total count");

        // Validate each result
        foreach (var result in Results)
        {
            var resultValidation = result.Validate();
            if (resultValidation.IsFailure)
                return resultValidation;
        }

        return Result.Success();
    }
}
