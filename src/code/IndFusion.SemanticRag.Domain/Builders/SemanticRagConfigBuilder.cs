using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndQuestResults;
using IndQuestResults.Operations;

namespace IndFusion.SemanticRag.Domain.Builders;

/// <summary>
/// Factory builder for creating <see cref="SemanticRagConfig"/> instances with validation.
/// </summary>
public static class SemanticRagConfigBuilder
{
	/// <summary>
	/// Builds a <see cref="SemanticRagConfig"/> with validation.
	/// </summary>
	/// <param name="id">Unique identifier for the configuration.</param>
	/// <param name="name">Name of the configuration.</param>
	/// <param name="embeddingModel">Model to use for embeddings.</param>
	/// <param name="vectorDimensions">Number of dimensions for vectors.</param>
	/// <param name="similarityThreshold">Default similarity threshold (0.0 to 1.0).</param>
	/// <param name="maxResults">Default maximum results.</param>
	/// <param name="properties">Additional configuration properties.</param>
	/// <returns>A Result containing the created SemanticRagConfig or validation error.</returns>
	public static Result<SemanticRagConfig> Build(
		string id,
		string name,
		string embeddingModel,
		int vectorDimensions,
		double similarityThreshold,
		int maxResults,
		System.Collections.Generic.Dictionary<string, object> properties)
	{
		var validation = ResultExtensions.ValidateNotNull(
			(id, nameof(id)),
			(name, nameof(name)),
			(embeddingModel, nameof(embeddingModel)),
			(properties, nameof(properties))
		);

		if (validation.IsFailure)
			return Result<SemanticRagConfig>.WithFailure(validation.Error!);

		// Validate string parameters are not empty
		if (string.IsNullOrWhiteSpace(id))
			return Result<SemanticRagConfig>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);

		if (string.IsNullOrWhiteSpace(name))
			return Result<SemanticRagConfig>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);

		if (string.IsNullOrWhiteSpace(embeddingModel))
			return Result<SemanticRagConfig>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);

		// Validate numeric parameters
		if (vectorDimensions <= 0)
			return Result<SemanticRagConfig>.WithFailure(ErrorCodes.ConfigVectorSizeInvalid);

		if (similarityThreshold < 0.0 || similarityThreshold > 1.0)
			return Result<SemanticRagConfig>.WithFailure(ErrorCodes.ValueOutOfRange);

		if (maxResults <= 0)
			return Result<SemanticRagConfig>.WithFailure(ErrorCodes.SearchLimitInvalid);

		var config = new SemanticRagConfig(
			Id: id,
			Name: name,
			EmbeddingModel: embeddingModel,
			VectorDimensions: vectorDimensions,
			SimilarityThreshold: similarityThreshold,
			MaxResults: maxResults,
			Properties: properties
		);

		return Result<SemanticRagConfig>.Success(config);
	}
}

