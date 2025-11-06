using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndQuestResults;

namespace IndFusion.SemanticRag.Domain.Ports;

/// <summary>
/// Port interface for graph database operations in the Semantic RAG system.
/// This interface abstracts graph database operations to enable hexagonal architecture
/// and testability by mocking infrastructure dependencies.
/// </summary>
public interface IGraphDatabasePort
{
	/// <summary>
	/// Executes a read-only Cypher query in a session.
	/// </summary>
	/// <param name="cypher">Cypher query string.</param>
	/// <param name="parameters">Query parameters.</param>
	/// <param name="database">Database name (optional).</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result containing query results.</returns>
	Task<Result<IReadOnlyList<CypherRecord>>> ExecuteReadAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Executes a write Cypher query in a session.
	/// </summary>
	/// <param name="cypher">Cypher query string.</param>
	/// <param name="parameters">Query parameters.</param>
	/// <param name="database">Database name (optional).</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result containing query results.</returns>
	Task<Result<IReadOnlyList<CypherRecord>>> ExecuteWriteAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Executes a read-only Cypher query and returns a single result.
	/// </summary>
	/// <param name="cypher">Cypher query string.</param>
	/// <param name="parameters">Query parameters.</param>
	/// <param name="database">Database name (optional).</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result containing a single query result, or null if no results.</returns>
	Task<Result<CypherRecord?>> ExecuteReadSingleAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Executes a write Cypher query and returns a single result.
	/// </summary>
	/// <param name="cypher">Cypher query string.</param>
	/// <param name="parameters">Query parameters.</param>
	/// <param name="database">Database name (optional).</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result containing a single query result, or null if no results.</returns>
	Task<Result<CypherRecord?>> ExecuteWriteSingleAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Executes a read-only Cypher query without returning results (for operations like CREATE/MERGE).
	/// </summary>
	/// <param name="cypher">Cypher query string.</param>
	/// <param name="parameters">Query parameters.</param>
	/// <param name="database">Database name (optional).</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result indicating success or failure.</returns>
	Task<Result> ExecuteReadVoidAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default);

	/// <summary>
	/// Executes a write Cypher query without returning results (for operations like CREATE/MERGE/DELETE).
	/// </summary>
	/// <param name="cypher">Cypher query string.</param>
	/// <param name="parameters">Query parameters.</param>
	/// <param name="database">Database name (optional).</param>
	/// <param name="cancellationToken">Cancellation token.</param>
	/// <returns>A Result indicating success or failure.</returns>
	Task<Result> ExecuteWriteVoidAsync(
		string cypher,
		Dictionary<string, object>? parameters = null,
		string? database = null,
		CancellationToken cancellationToken = default);
}

