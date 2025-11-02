using System;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;
using IndQuestResults.Operations;

namespace IndFusion.SemanticRag.Domain.Builders;

/// <summary>
/// Factory builder for creating <see cref="SemanticDocument"/> instances with validation.
/// </summary>
public static class SemanticDocumentBuilder
{
	/// <summary>
	/// Builds a <see cref="SemanticDocument"/> with validation.
	/// </summary>
	/// <param name="id">Unique identifier for the document.</param>
	/// <param name="title">Document title.</param>
	/// <param name="content">Extracted text content.</param>
	/// <param name="metadata">Document metadata including source, author, etc.</param>
	/// <param name="createdAt">When the document was processed.</param>
	/// <param name="updatedAt">When the document was last updated.</param>
	/// <returns>A Result containing the created SemanticDocument or validation error.</returns>
	public static Result<SemanticDocument> Build(
		string id,
		string title,
		string content,
		System.Collections.Generic.Dictionary<string, object> metadata,
		DateTime createdAt,
		DateTime updatedAt)
	{
		var validation = ResultExtensions.ValidateNotNull(
			(id, nameof(id)),
			(title, nameof(title)),
			(content, nameof(content)),
			(metadata, nameof(metadata))
		);

		if (validation.IsFailure)
			return Result<SemanticDocument>.WithFailure(validation.Error!);

		// Validate string parameters are not empty
		if (string.IsNullOrWhiteSpace(id))
			return Result<SemanticDocument>.WithFailure(ErrorCodes.SemanticDocumentIdRequired);

		if (string.IsNullOrWhiteSpace(title))
			return Result<SemanticDocument>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);

		if (string.IsNullOrWhiteSpace(content))
			return Result<SemanticDocument>.WithFailure(ErrorCodes.SemanticDocumentContentRequired);

		var document = new SemanticDocument(
			Id: id,
			Title: title,
			Content: content,
			Metadata: metadata,
			CreatedAt: createdAt,
			UpdatedAt: updatedAt
		);

		return Result<SemanticDocument>.Success(document);
	}
}

