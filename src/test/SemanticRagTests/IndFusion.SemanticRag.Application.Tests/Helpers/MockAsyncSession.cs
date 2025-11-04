using Neo4j.Driver;

namespace IndFusion.SemanticRag.Application.Tests.Helpers;

/// <summary>
/// Manual mock implementation of IAsyncSession to work around NSubstitute's limitation
/// with non-generic ValueTask return types in IAsyncDisposable.DisposeAsync().
/// This mock properly implements both IDisposable and IAsyncDisposable.
/// </summary>
public class MockAsyncSession : IAsyncSession, IAsyncQueryRunner
{
	private readonly IResultCursor _resultCursor;

	/// <summary>
	/// Initializes a new instance of the <see cref="MockAsyncSession"/> class.
	/// </summary>
	/// <param name="resultCursor">The result cursor to return from RunAsync calls.</param>
	public MockAsyncSession(IResultCursor resultCursor)
	{
		_resultCursor = resultCursor ?? throw new ArgumentNullException(nameof(resultCursor));
	}

	/// <summary>
	/// Gets the last query executed on this mock session.
	/// </summary>
	public string? LastQuery { get; private set; }

	/// <summary>
	/// Gets the last parameters used with the last query.
	/// </summary>
	public Dictionary<string, object>? LastParameters { get; private set; }

	/// <summary>
	/// Gets the number of times RunAsync has been called.
	/// </summary>
	public int RunAsyncCallCount { get; private set; }

	/// <summary>
	/// Gets all queries executed on this mock session.
	/// </summary>
	public List<string> AllQueries { get; } = new List<string>();

	/// <summary>
	/// Gets or sets the exception to throw on the next RunAsync call. Set to null to clear.
	/// </summary>
	public Exception? ExceptionToThrow { get; set; }

	/// <inheritdoc />
	public Task<IResultCursor> RunAsync(string query, IDictionary<string, object>? parameters = null)
	{
		LastQuery = query;
		LastParameters = parameters != null ? new Dictionary<string, object>(parameters) : null;
		RunAsyncCallCount++;
		AllQueries.Add(query);

		if (ExceptionToThrow != null)
		{
			var ex = ExceptionToThrow;
			ExceptionToThrow = null; // Clear after throwing
			return Task.FromException<IResultCursor>(ex);
		}

		return Task.FromResult(_resultCursor);
	}

	/// <inheritdoc />
	public Task<IResultCursor> RunAsync(string query, object parameters)
	{
		return RunAsync(query, parameters as IDictionary<string, object>);
	}

	/// <inheritdoc />
	public Task<IResultCursor> RunAsync(Query query)
	{
		return RunAsync(query.Text, query.Parameters);
	}

	/// <inheritdoc />
	public Task<IResultCursor> RunAsync(string query, Action<TransactionConfigBuilder> action)
	{
		return RunAsync(query, (IDictionary<string, object>?)null);
	}

	/// <inheritdoc />
	public Task<IResultCursor> RunAsync(string query, IDictionary<string, object>? parameters, Action<TransactionConfigBuilder> action)
	{
		return RunAsync(query, parameters);
	}

	/// <inheritdoc />
	public Task<IResultCursor> RunAsync(Query query, Action<TransactionConfigBuilder> action)
	{
		return RunAsync(query);
	}

	/// <inheritdoc />
	public Task<TResult> ExecuteReadAsync<TResult>(Func<IAsyncQueryRunner, Task<TResult>> work, Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("ExecuteReadAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task ExecuteReadAsync(Func<IAsyncQueryRunner, Task> work, Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("ExecuteReadAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task<TResult> ExecuteWriteAsync<TResult>(Func<IAsyncQueryRunner, Task<TResult>> work, Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("ExecuteWriteAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task ExecuteWriteAsync(Func<IAsyncQueryRunner, Task> work, Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("ExecuteWriteAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task<IAsyncTransaction> BeginTransactionAsync(Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("BeginTransactionAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task<IAsyncTransaction> BeginTransactionAsync()
	{
		throw new NotImplementedException("BeginTransactionAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task<TResult> ReadTransactionAsync<TResult>(Func<IAsyncTransaction, Task<TResult>> work, Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("ReadTransactionAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task ReadTransactionAsync(Func<IAsyncTransaction, Task> work, Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("ReadTransactionAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task<TResult> WriteTransactionAsync<TResult>(Func<IAsyncTransaction, Task<TResult>> work, Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("WriteTransactionAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task WriteTransactionAsync(Func<IAsyncTransaction, Task> work, Action<TransactionConfigBuilder>? action = null)
	{
		throw new NotImplementedException("WriteTransactionAsync not implemented in mock");
	}

	/// <inheritdoc />
	public Task CloseAsync()
	{
		return Task.CompletedTask;
	}

	/// <inheritdoc />
	public Bookmarks LastBookmarks => NSubstitute.Substitute.For<Bookmarks>();

	/// <inheritdoc />
	[Obsolete("Replaced with Bookmarks. Will be removed in 6.0")]
	public Bookmark LastBookmark => NSubstitute.Substitute.For<Bookmark>();

	/// <inheritdoc />
	public SessionConfig SessionConfig => NSubstitute.Substitute.For<SessionConfig>();

	// IAsyncQueryRunner implementation
	/// <inheritdoc />
	Task<IResultCursor> IAsyncQueryRunner.RunAsync(string query)
	{
		return RunAsync(query, (IDictionary<string, object>?)null);
	}

	/// <inheritdoc />
	Task<IResultCursor> IAsyncQueryRunner.RunAsync(Query query)
	{
		return RunAsync(query);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		// No-op for test mock
	}

	/// <inheritdoc />
	public ValueTask DisposeAsync()
	{
		// Properly return non-generic ValueTask to avoid NSubstitute's limitation
		return ValueTask.CompletedTask;
	}
}

