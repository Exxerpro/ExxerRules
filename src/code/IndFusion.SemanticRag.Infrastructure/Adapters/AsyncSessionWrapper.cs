using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Neo4j.Driver;

namespace IndFusion.SemanticRag.Infrastructure.Adapters;

/// <summary>
/// Wrapper implementation for Neo4j IAsyncSession to enable proper mocking with NSubstitute.
/// This wrapper delegates all calls to the underlying IAsyncSession and explicitly implements
/// DisposeAsync to work around NSubstitute's limitation with mocking non-generic ValueTask.
/// </summary>
public class AsyncSessionWrapper : IAsyncSessionWrapper
{
	private readonly IAsyncSession _session;

	/// <summary>
	/// Initializes a new instance of the <see cref="AsyncSessionWrapper"/> class.
	/// </summary>
	/// <param name="session">The Neo4j async session to wrap.</param>
	/// <exception cref="ArgumentNullException">Thrown when session is null.</exception>
	public AsyncSessionWrapper(IAsyncSession session)
	{
		_session = session ?? throw new ArgumentNullException(nameof(session));
	}

	/// <inheritdoc />
	public Task<IResultCursor> RunAsync(string query, Dictionary<string, object> parameters)
	{
		return _session.RunAsync(query, parameters);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		_session.Dispose();
	}
}

