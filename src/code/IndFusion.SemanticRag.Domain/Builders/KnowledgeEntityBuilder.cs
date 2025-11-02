using System;
using System.Collections.Generic;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;
using IndQuestResults.Operations;

namespace IndFusion.SemanticRag.Domain.Builders;

/// <summary>
/// Factory builder for creating <see cref="KnowledgeEntity"/> instances with validation.
/// </summary>
public static class KnowledgeEntityBuilder
{
	/// <summary>
	/// Builds a <see cref="KnowledgeEntity"/> with validation.
	/// </summary>
	/// <param name="id">Unique identifier for the entity.</param>
	/// <param name="name">Entity name.</param>
	/// <param name="type">Entity type (person, organization, concept, etc.).</param>
	/// <param name="description">Entity description.</param>
	/// <param name="properties">Additional entity properties.</param>
	/// <param name="confidence">Confidence score for the extraction (0.0 to 1.0).</param>
	/// <param name="createdAt">When the entity was extracted.</param>
	/// <returns>A Result containing the created KnowledgeEntity or validation error.</returns>
	public static Result<KnowledgeEntity> Build(
		string id,
		string name,
		string type,
		string description,
		Dictionary<string, object> properties,
		double confidence,
		DateTime createdAt)
	{
		var validation = ResultExtensions.ValidateNotNull(
			(id, nameof(id)),
			(name, nameof(name)),
			(type, nameof(type)),
			(description, nameof(description)),
			(properties, nameof(properties))
		);

		if (validation.IsFailure)
			return Result<KnowledgeEntity>.WithFailure(validation.Error!);

		// Validate string parameters are not empty
		if (string.IsNullOrWhiteSpace(id))
			return Result<KnowledgeEntity>.WithFailure(ErrorCodes.KnowledgeNodeIdRequired);

		if (string.IsNullOrWhiteSpace(name))
			return Result<KnowledgeEntity>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);

		if (string.IsNullOrWhiteSpace(type))
			return Result<KnowledgeEntity>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);

		if (string.IsNullOrWhiteSpace(description))
			return Result<KnowledgeEntity>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);

		// Validate confidence range
		if (confidence < 0.0 || confidence > 1.0)
			return Result<KnowledgeEntity>.WithFailure(ErrorCodes.ValueOutOfRange);

		var entity = new KnowledgeEntity(
			Id: id,
			Name: name,
			Type: type,
			Description: description,
			Properties: properties,
			Confidence: confidence,
			CreatedAt: createdAt
		);

		return Result<KnowledgeEntity>.Success(entity);
	}
}

