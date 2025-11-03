using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace IndFusion.SemanticRag.Infrastructure.Adapters;

/// <summary>
/// Wrapper interface for Neo4j IAsyncSession to enable proper mocking with NSubstitute.
/// This interface implements IDisposable for synchronous disposal patterns.
/// The implementation class also implements IAsyncDisposable internally, but it's not exposed
/// through the interface to avoid NSubstitute's limitation with mocking non-generic ValueTask.
/// </summary>
public interface IAsyncSessionWrapper : IDisposable
{
	/// <summary>
	/// Executes a query asynchronously.
	/// </summary>
	/// <param name="query">The Cypher query to execute.</param>
	/// <param name="parameters">The parameters for the query.</param>
	/// <returns>A task representing the asynchronous operation, containing the result cursor.</returns>
	Task<IResultCursor> RunAsync(string query, Dictionary<string, object> parameters);
}

