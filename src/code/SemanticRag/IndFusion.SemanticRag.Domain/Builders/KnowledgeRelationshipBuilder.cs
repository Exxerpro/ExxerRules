using System;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;
using IndQuestResults.Operations;

namespace IndFusion.SemanticRag.Domain.Builders;

/// <summary>
/// Factory builder for creating <see cref="KnowledgeRelationship"/> instances with validation.
/// </summary>
public static class KnowledgeRelationshipBuilder
{
	/// <summary>
	/// Builds a <see cref="KnowledgeRelationship"/> with validation.
	/// </summary>
	/// <param name="id">Unique identifier for the relationship.</param>
	/// <param name="fromNodeId">ID of the source node.</param>
	/// <param name="toNodeId">ID of the target node.</param>
	/// <param name="relationshipType">Type of the relationship.</param>
	/// <param name="properties">Properties associated with the relationship.</param>
	/// <param name="createdAt">Timestamp when the relationship was created.</param>
	/// <returns>A Result containing the created KnowledgeRelationship or validation error.</returns>
	public static Result<KnowledgeRelationship> Build(
		string id,
		string fromNodeId,
		string toNodeId,
		string relationshipType,
		System.Collections.Generic.IReadOnlyDictionary<string, object> properties,
		DateTimeOffset createdAt)
	{
		var validation = ResultExtensions.ValidateNotNull(
			(id, nameof(id)),
			(fromNodeId, nameof(fromNodeId)),
			(toNodeId, nameof(toNodeId)),
			(relationshipType, nameof(relationshipType)),
			(properties, nameof(properties))
		);

		if (validation.IsFailure)
			return Result<KnowledgeRelationship>.WithFailure(validation.Error!);

		// Validate string parameters are not empty
		if (string.IsNullOrWhiteSpace(id))
			return Result<KnowledgeRelationship>.WithFailure(ErrorCodes.KnowledgeRelationshipIdRequired);

		if (string.IsNullOrWhiteSpace(fromNodeId))
			return Result<KnowledgeRelationship>.WithFailure(ErrorCodes.KnowledgeRelationshipSourceIdRequired);

		if (string.IsNullOrWhiteSpace(toNodeId))
			return Result<KnowledgeRelationship>.WithFailure(ErrorCodes.KnowledgeRelationshipTargetIdRequired);

		if (string.IsNullOrWhiteSpace(relationshipType))
			return Result<KnowledgeRelationship>.WithFailure(ErrorCodes.KnowledgeRelationshipTypeRequired);

		// Validate from and to nodes are different
		if (fromNodeId == toNodeId)
			return Result<KnowledgeRelationship>.WithFailure(ErrorCodes.ParameterNullOrEmpty);

		var relationship = new KnowledgeRelationship(
			Id: id,
			FromNodeId: fromNodeId,
			ToNodeId: toNodeId,
			RelationshipType: relationshipType,
			Properties: properties,
			CreatedAt: createdAt
		);

		return Result<KnowledgeRelationship>.Success(relationship);
	}
}

