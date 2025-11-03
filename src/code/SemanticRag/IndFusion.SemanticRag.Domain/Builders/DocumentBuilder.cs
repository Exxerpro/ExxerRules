using System;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;
using IndQuestResults.Operations;

namespace IndFusion.SemanticRag.Domain.Builders;

/// <summary>
/// Factory builder for creating <see cref="Document"/> instances with validation.
/// </summary>
public static class DocumentBuilder
{
	/// <summary>
	/// Builds a <see cref="Document"/> with validation.
	/// </summary>
	/// <param name="id">Unique identifier for the document.</param>
	/// <param name="content">The text content of the document.</param>
	/// <param name="sourcePath">Path to the source file or location.</param>
	/// <param name="repository">Repository name where the document originates.</param>
	/// <param name="commitHash">Git commit hash for version tracking.</param>
	/// <param name="documentType">Type of document (code, documentation, etc.).</param>
	/// <param name="metadata">Additional metadata associated with the document.</param>
	/// <param name="createdAt">Timestamp when the document was created.</param>
	/// <param name="updatedAt">Timestamp when the document was last updated.</param>
	/// <returns>A Result containing the created Document or validation error.</returns>
	public static Result<Document> Build(
		string id,
		string content,
		string sourcePath,
		string repository,
		string commitHash,
		DocumentType documentType,
		System.Collections.Generic.IReadOnlyDictionary<string, object> metadata,
		DateTimeOffset createdAt,
		DateTimeOffset updatedAt)
	{
		var validation = ResultExtensions.ValidateNotNull(
			(id, nameof(id)),
			(content, nameof(content)),
			(sourcePath, nameof(sourcePath)),
			(repository, nameof(repository)),
			(commitHash, nameof(commitHash)),
			(metadata, nameof(metadata))
		);

		if (validation.IsFailure)
			return Result<Document>.WithFailure(validation.Error!);

		// Validate string parameters are not empty
		if (string.IsNullOrWhiteSpace(id))
			return Result<Document>.WithFailure(ErrorCodes.DocumentIdRequired);

		if (string.IsNullOrWhiteSpace(content))
			return Result<Document>.WithFailure(ErrorCodes.DocumentContentRequired);

		if (string.IsNullOrWhiteSpace(sourcePath))
			return Result<Document>.WithFailure(ErrorCodes.DocumentSourcePathEmpty);

		if (string.IsNullOrWhiteSpace(repository))
			return Result<Document>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);

		if (string.IsNullOrWhiteSpace(commitHash))
			return Result<Document>.WithFailure(ErrorCodes.DocumentCommitHashEmpty);

		var document = new Document(
			Id: id,
			Content: content,
			SourcePath: sourcePath,
			Repository: repository,
			CommitHash: commitHash,
			DocumentType: documentType,
			Metadata: metadata,
			CreatedAt: createdAt,
			UpdatedAt: updatedAt
		);

		return Result<Document>.Success(document);
	}
}

