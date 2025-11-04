using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Ports;
using IndQuestResults;
using IndQuestResults.Operations;
using Microsoft.Extensions.Logging;
using Qdrant.Client;
using QdrantCollectionInfo = Qdrant.Client.Grpc.CollectionInfo;
using Qdrant.Client.Grpc;
using static IndFusion.SemanticRag.Domain.Errors.ResultExtensionsWithErrorCodes;
using static Qdrant.Client.Grpc.Conditions;

namespace IndFusion.SemanticRag.Infrastructure.Adapters;

/// <summary>
/// Qdrant-based implementation of the vector database port.
/// </summary>
public class QdrantVectorDatabaseAdapter : IVectorDatabasePort
{
	private readonly QdrantClient _client;
	private readonly ILogger<QdrantVectorDatabaseAdapter> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="QdrantVectorDatabaseAdapter"/> class.
	/// </summary>
	/// <param name="client">The Qdrant client instance.</param>
	/// <param name="logger">The logger instance.</param>
	public QdrantVectorDatabaseAdapter(
		QdrantClient client,
		ILogger<QdrantVectorDatabaseAdapter> logger)
	{
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
	}

	/// <inheritdoc />
	public async Task<Result<Domain.Ports.CollectionInfo?>> GetCollectionInfoAsync(
		string collectionName,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(collectionName))
		{
			return Result<Domain.Ports.CollectionInfo?>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Cancelled<Domain.Ports.CollectionInfo?>(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(collectionName)
				.Ensure(
					name => !string.IsNullOrWhiteSpace(name),
					ErrorCodes.ParameterNullOrWhitespace))
				.ThenAsync(
					async name =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result<Domain.Ports.CollectionInfo?>.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Getting collection info for: {CollectionName}", name);
						var qdrantCollectionInfo = await _client.GetCollectionInfoAsync(name, cancellationToken);
						
						if (qdrantCollectionInfo == null)
						{
							_logger.LogInformation("Collection not found: {CollectionName}", name);
							return Result<Domain.Ports.CollectionInfo?>.Success(null);
						}

						// Map Qdrant collection info to domain CollectionInfo
						var vectorSize = qdrantCollectionInfo.Config?.Params?.VectorsConfig?.Params?.Size ?? 0U;
						var distance = MapDistance(qdrantCollectionInfo.Config?.Params?.VectorsConfig?.Params?.Distance);
						var pointsCount = (long)qdrantCollectionInfo.PointsCount;

						var collectionInfo = new Domain.Ports.CollectionInfo(
							CollectionName: name,
							VectorSize: (uint)vectorSize,
							Distance: distance,
							PointsCount: pointsCount);

						_logger.LogInformation("Successfully retrieved collection info for: {CollectionName}", name);
						return Result<Domain.Ports.CollectionInfo?>.Success(collectionInfo);
					});
		}
		catch (OperationCanceledException)
		{
			return Cancelled<Domain.Ports.CollectionInfo?>(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to get collection info for: {CollectionName}", collectionName);
			return WithFailure<Domain.Ports.CollectionInfo?>(ErrorCodes.VectorDatabaseError, ex);
		}
	}

	/// <inheritdoc />
	public async Task<Result> CreateCollectionAsync(
		string collectionName,
		uint vectorSize,
		VectorDistance distance,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(collectionName))
		{
			return Result.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(collectionName)
				.Ensure(
					name => !string.IsNullOrWhiteSpace(name),
					ErrorCodes.ParameterNullOrWhitespace)
				.Ensure(
					_ => vectorSize > 0,
					ErrorCodes.ValueOutOfRange))
				.ThenAsync<string, Result>(
					async name =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Creating collection: {CollectionName} with vector size: {VectorSize}", name, vectorSize);

						var qdrantDistance = MapToQdrantDistance(distance);
						var vectorParams = new VectorParams
						{
							Size = vectorSize,
							Distance = qdrantDistance
						};

						await _client.CreateCollectionAsync(name, vectorParams, cancellationToken: cancellationToken);

						_logger.LogInformation("Successfully created collection: {CollectionName}", name);
						return Result.Success();
					});
		}
		catch (OperationCanceledException)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to create collection: {CollectionName}", collectionName);
			return Result.WithFailure(ErrorCodes.VectorDatabaseError);
		}
	}

	/// <inheritdoc />
	public async Task<Result<IReadOnlyList<VectorSearchHit>>> SearchAsync(
		string collectionName,
		float[] queryVector,
		uint limit,
		float? scoreThreshold = null,
		Dictionary<string, object>? filter = null,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException in fluent chain
		if (queryVector == null)
		{
			return Result<IReadOnlyList<VectorSearchHit>>.WithFailure(ErrorCodes.VectorContentRequired);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Cancelled<IReadOnlyList<VectorSearchHit>>(ErrorCodes.OperationCancelled);
		}

		try
		{
			return await Task.FromResult(Result<(string collectionName, float[] queryVector)>.Success((collectionName, queryVector))
				.Ensure(
					args => !string.IsNullOrWhiteSpace(args.collectionName),
					ErrorCodes.ParameterNullOrWhitespace)
				.Ensure(
					args => args.queryVector is not null && args.queryVector.Length > 0,
					ErrorCodes.VectorContentRequired))
				.ThenAsync(
					async args =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result<IReadOnlyList<VectorSearchHit>>.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Searching collection: {CollectionName} with limit: {Limit}", args.collectionName, limit);

						// Build Qdrant filter from domain filter
						Filter? qdrantFilter = null;
						if (filter != null && filter.Count > 0)
						{
							qdrantFilter = BuildQdrantFilter(filter);
						}

						var searchResults = await _client.SearchAsync(
							collectionName: args.collectionName,
							vector: args.queryVector,
							filter: qdrantFilter,
							limit: limit,
							cancellationToken: cancellationToken);

						var hits = new List<VectorSearchHit>();
						foreach (var point in searchResults)
						{
							var score = (float)point.Score;

							// Apply score threshold if provided
							if (scoreThreshold.HasValue && score < scoreThreshold.Value)
							{
								continue;
							}

							// Extract payload
							var payload = new Dictionary<string, object>();
							if (point.Payload != null)
							{
								foreach (var kvp in point.Payload)
								{
									payload[kvp.Key] = kvp.Value.ToString() ?? string.Empty;
								}
							}

							// Extract vector if present
							float[]? vector = null;
							if (point.Vectors != null && point.Vectors.Vector != null && point.Vectors.Vector.Data != null)
							{
								vector = point.Vectors.Vector.Data.ToArray();
							}

							var hit = new VectorSearchHit(
								PointId: point.Id.Num,
								Score: score,
								Payload: payload.Count > 0 ? payload : null,
								Vector: vector);

							hits.Add(hit);
						}

						_logger.LogInformation("Search completed: {Count} results found", hits.Count);
						return Result<IReadOnlyList<VectorSearchHit>>.Success(hits.AsReadOnly());
					});
		}
		catch (OperationCanceledException)
		{
			return Cancelled<IReadOnlyList<VectorSearchHit>>(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to search collection: {CollectionName}", collectionName);
			return WithFailure<IReadOnlyList<VectorSearchHit>>(ErrorCodes.VectorSearchFailed, ex);
		}
	}

	/// <inheritdoc />
	public async Task<Result> UpsertAsync(
		string collectionName,
		IReadOnlyList<QdrantPoint> points,
		CancellationToken cancellationToken = default)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}

		// Early validation to avoid NullReferenceException in fluent chain
		if (points == null || points.Count == 0)
		{
			return Result.WithFailure(ErrorCodes.CollectionEmpty);
		}

		try
		{
			return await Task.FromResult(Result<(string collectionName, IReadOnlyList<QdrantPoint> points)>.Success((collectionName, points))
				.Ensure(
					args => !string.IsNullOrWhiteSpace(args.collectionName),
					ErrorCodes.ParameterNullOrWhitespace)
				.Ensure(
					args => args.points != null && args.points.Count > 0,
					ErrorCodes.CollectionEmpty))
				.ThenAsync<(string collectionName, IReadOnlyList<QdrantPoint> points), Result>(
					async args =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Upserting {Count} points to collection: {CollectionName}", args.points.Count, args.collectionName);

						var qdrantPoints = new List<PointStruct>();
						foreach (var point in args.points)
						{
							var qdrantPoint = new PointStruct
							{
								Id = point.Id,
								Vectors = point.Vector
							};

							// Convert payload to Qdrant Value format
							if (point.Payload != null)
							{
								foreach (var kvp in point.Payload)
								{
									qdrantPoint.Payload[kvp.Key] = ConvertToValue(kvp.Value);
								}
							}

							qdrantPoints.Add(qdrantPoint);
						}

						await _client.UpsertAsync(args.collectionName, qdrantPoints, cancellationToken: cancellationToken);

						_logger.LogInformation("Successfully upserted {Count} points to collection: {CollectionName}", args.points.Count, args.collectionName);
						return Result.Success();
					});
		}
		catch (OperationCanceledException)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to upsert points to collection: {CollectionName}", collectionName);
			return Result.WithFailure(ErrorCodes.VectorDatabaseError);
		}
	}

	/// <inheritdoc />
	public async Task<Result> DeleteAsync(
		string collectionName,
		IReadOnlyList<ulong>? pointIds = null,
		Dictionary<string, object>? filter = null,
		CancellationToken cancellationToken = default)
	{
		// Early validation to avoid NullReferenceException
		if (string.IsNullOrWhiteSpace(collectionName))
		{
			return Result.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}

		// Early validation: either pointIds or filter must be provided
		var hasPointIds = pointIds != null && pointIds.Count > 0;
		var hasFilter = filter != null && filter.Count > 0;
		if (!hasPointIds && !hasFilter)
		{
			return Result.WithFailure(ErrorCodes.ParameterNull);
		}

		try
		{
			return await Task.FromResult(Result<string>.Success(collectionName)
				.Ensure(
					name => !string.IsNullOrWhiteSpace(name),
					ErrorCodes.ParameterNullOrWhitespace))
				.ThenAsync<string, Result>(
					async name =>
					{
						if (cancellationToken.IsCancellationRequested)
						{
							return Result.WithFailure(ErrorCodes.OperationCancelled);
						}

						_logger.LogInformation("Deleting points from collection: {CollectionName}", name);

						if (pointIds != null && pointIds.Count > 0)
						{
							// Delete by point IDs
							await _client.DeleteAsync(name, pointIds, cancellationToken: cancellationToken);
							_logger.LogInformation("Successfully deleted {Count} points by ID from collection: {CollectionName}", pointIds.Count, name);
						}
						else if (filter != null && filter.Count > 0)
						{
							// Delete by filter
							var qdrantFilter = BuildQdrantFilter(filter);
							if (qdrantFilter == null)
							{
								return Result.WithFailure(ErrorCodes.ParameterNull);
							}
							await _client.DeleteAsync(name, filter: qdrantFilter, cancellationToken: cancellationToken);
							_logger.LogInformation("Successfully deleted points by filter from collection: {CollectionName}", name);
						}

						return Result.Success();
					});
		}
		catch (OperationCanceledException)
		{
			return Result.WithFailure(ErrorCodes.OperationCancelled);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to delete points from collection: {CollectionName}", collectionName);
			return Result.WithFailure(ErrorCodes.VectorDatabaseError);
		}
	}

	/// <summary>
	/// Maps Qdrant distance enum to domain VectorDistance enum.
	/// </summary>
	private static VectorDistance MapDistance(Distance? qdrantDistance)
	{
		if (!qdrantDistance.HasValue)
		{
			return VectorDistance.Cosine; // Default
		}

		return qdrantDistance.Value switch
		{
			Distance.Cosine => VectorDistance.Cosine,
			Distance.Dot => VectorDistance.Dot,
			_ => VectorDistance.Cosine // Euclidean not directly supported in Qdrant Distance enum, defaulting to Cosine
		};
	}

	/// <summary>
	/// Maps domain VectorDistance enum to Qdrant Distance enum.
	/// </summary>
	private static Distance MapToQdrantDistance(VectorDistance distance)
	{
		return distance switch
		{
			VectorDistance.Cosine => Distance.Cosine,
			VectorDistance.Dot => Distance.Dot,
			VectorDistance.Euclidean => Distance.Cosine, // Euclidean not directly supported, mapping to Cosine
			_ => Distance.Cosine
		};
	}

	/// <summary>
	/// Builds a Qdrant filter from domain filter dictionary.
	/// </summary>
	private static Filter? BuildQdrantFilter(Dictionary<string, object> filter)
	{
		if (filter == null || filter.Count == 0)
		{
			return null;
		}

		var conditions = new List<Condition>();
		foreach (var kvp in filter)
		{
			Condition condition = kvp.Value switch
			{
				bool boolValue => Match(kvp.Key, boolValue),
				string stringValue => MatchKeyword(kvp.Key, stringValue),
				int or long => new Condition
				{
					Field = new FieldCondition
					{
						Key = kvp.Key,
						Match = new Qdrant.Client.Grpc.Match { Integer = Convert.ToInt64(kvp.Value) }
					}
				},
				float or double => new Condition
				{
					Field = new FieldCondition
					{
						Key = kvp.Key,
						Match = new Qdrant.Client.Grpc.Match { Integer = Convert.ToInt64(kvp.Value) }
					}
				},
				_ => MatchKeyword(kvp.Key, kvp.Value?.ToString() ?? string.Empty)
			};

			conditions.Add(condition);
		}

		if (conditions.Count == 0)
		{
			return null;
		}

		if (conditions.Count == 1)
		{
			return new Filter { Must = { conditions[0] } };
		}

		return new Filter { Must = { conditions } };
	}

	/// <summary>
	/// Converts a C# object to a Qdrant Value type.
	/// </summary>
	private static Value ConvertToValue(object? value)
	{
		if (value == null)
		{
			return new Value { NullValue = NullValue.NullValue };
		}

		return value switch
		{
			string s => new Value { StringValue = s },
			int i => new Value { IntegerValue = i },
			long l => new Value { IntegerValue = l },
			float f => new Value { DoubleValue = f },
			double d => new Value { DoubleValue = d },
			bool b => new Value { BoolValue = b },
			_ => new Value { StringValue = value.ToString() ?? string.Empty }
		};
	}
}

