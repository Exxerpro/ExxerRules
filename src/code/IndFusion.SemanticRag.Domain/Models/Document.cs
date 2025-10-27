using System;
using System.Collections.Generic;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Models;

/// <summary>
/// Represents a document in the semantic RAG system.
/// </summary>
/// <param name="Id">Unique identifier for the document.</param>
/// <param name="Content">The text content of the document.</param>
/// <param name="SourcePath">Path to the source file or location.</param>
/// <param name="Repository">Repository name where the document originates.</param>
/// <param name="CommitHash">Git commit hash for version tracking.</param>
/// <param name="DocumentType">Type of document (code, documentation, etc.).</param>
/// <param name="Metadata">Additional metadata associated with the document.</param>
/// <param name="CreatedAt">Timestamp when the document was created.</param>
/// <param name="UpdatedAt">Timestamp when the document was last updated.</param>
public record Document(
    string Id,
    string Content,
    string SourcePath,
    string Repository,
    string CommitHash,
    DocumentType DocumentType,
    IReadOnlyDictionary<string, object> Metadata,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    /// <summary>
    /// Gets the document identifier.
    /// </summary>
    public string Id { get; init; } = Id;

    /// <summary>
    /// Gets the document content.
    /// </summary>
    public string Content { get; init; } = Content;

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
    /// Gets the document type.
    /// </summary>
    public DocumentType DocumentType { get; init; } = DocumentType;

    /// <summary>
    /// Gets the metadata dictionary.
    /// </summary>
    public IReadOnlyDictionary<string, object> Metadata { get; init; } = Metadata;

    /// <summary>
    /// Gets the creation timestamp.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; } = CreatedAt;

    /// <summary>
    /// Gets the last update timestamp.
    /// </summary>
    public DateTimeOffset UpdatedAt { get; init; } = UpdatedAt;

    /// <summary>
    /// Validates the document for basic requirements.
    /// </summary>
    /// <returns>A Result indicating validation success or failure.</returns>
    public Result Validate()
    {
        if (string.IsNullOrWhiteSpace(Id))
            return Result.WithFailure("Document ID cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(Content))
            return Result.WithFailure("Document content cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(SourcePath))
            return Result.WithFailure("Document source path cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(Repository))
            return Result.WithFailure("Document repository cannot be empty or whitespace");

        if (string.IsNullOrWhiteSpace(CommitHash))
            return Result.WithFailure("Document commit hash cannot be empty or whitespace");

        return Result.Success();
    }
}

/// <summary>
/// Represents the type of document in the system.
/// </summary>
public enum DocumentType
{
    /// <summary>
    /// Source code file.
    /// </summary>
    SourceCode,

    /// <summary>
    /// Documentation file.
    /// </summary>
    Documentation,

    /// <summary>
    /// Configuration file.
    /// </summary>
    Configuration,

    /// <summary>
    /// Test file.
    /// </summary>
    Test,

    /// <summary>
    /// Build script or automation file.
    /// </summary>
    BuildScript,

    /// <summary>
    /// Other type of file.
    /// </summary>
    Other
}
