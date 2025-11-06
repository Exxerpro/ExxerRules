using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port interface for vector database operations in the Semantic RAG system.
/// This interface abstracts vector database operations to enable hexagonal architecture
/// and testability by mocking infrastructure dependencies.
/// </summary>
public interface IVectorDatabasePort
{
	/// <summary>
	/// Gets information about a collection.
	/// </summary>
	/// <param name="collectionName">Name of the collection.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result containing collection information, or null if collection doesn't exist.</returns>
	Task<Result<CollectionInfo?>> GetCollectionInfoAsync(
		string collectionName,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Creates a new collection with the specified vector parameters.
	/// </summary>
	/// <param name="collectionName">Name of the collection to create.</param>
	/// <param name="vectorSize">Size of the vectors in this collection.</param>
	/// <param name="distance">Distance metric for similarity calculation.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result indicating success or failure.</returns>
	Task<Result> CreateCollectionAsync(
		string collectionName,
		uint vectorSize,
		VectorDistance distance,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Searches for similar vectors in a collection.
	/// </summary>
	/// <param name="collectionName">Name of the collection to search.</param>
	/// <param name="queryVector">Query vector for similarity search.</param>
	/// <param name="limit">Maximum number of results to return.</param>
	/// <param name="scoreThreshold">Minimum similarity score threshold.</param>
	/// <param name="filter">Optional filter conditions.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result containing search results.</returns>
	Task<Result<IReadOnlyList<VectorSearchHit>>> SearchAsync(
		string collectionName,
		float[] queryVector,
		uint limit,
		float? scoreThreshold = null,
		Dictionary<string, object>? filter = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Upserts (inserts or updates) points in a collection.
	/// </summary>
	/// <param name="collectionName">Name of the collection.</param>
	/// <param name="points">Points to upsert.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result indicating success or failure.</returns>
	Task<Result> UpsertAsync(
		string collectionName,
		IReadOnlyList<QdrantPoint> points,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Deletes points from a collection.
	/// </summary>
	/// <param name="collectionName">Name of the collection.</param>
	/// <param name="pointIds">IDs of points to delete.</param>
	/// <param name="filter">Optional filter to delete matching points.</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result indicating success or failure.</returns>
	Task<Result> DeleteAsync(
		string collectionName,
		IReadOnlyList<ulong>? pointIds = null,
		Dictionary<string, object>? filter = null,
		CancellationToken cancellationToken = default);
}

