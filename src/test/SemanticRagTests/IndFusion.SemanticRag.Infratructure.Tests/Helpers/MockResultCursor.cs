using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Neo4j.Driver;

namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers;

/// <summary>
/// Simple async enumerator for records.
/// </summary>
internal sealed class RecordAsyncEnumerator : IAsyncEnumerator<IRecord>
{
	private readonly IReadOnlyList<IRecord> _records;
	private readonly CancellationToken _cancellationToken;
	private int _index = -1;

	public RecordAsyncEnumerator(IReadOnlyList<IRecord> records, CancellationToken cancellationToken)
	{
		_records = records;
		_cancellationToken = cancellationToken;
	}

	public IRecord Current => _index >= 0 && _index < _records.Count ? _records[_index] : throw new InvalidOperationException();

	public ValueTask<bool> MoveNextAsync()
	{
		_cancellationToken.ThrowIfCancellationRequested();
		_index++;
		return ValueTask.FromResult(_index < _records.Count);
	}

	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}
}

/// <summary>
/// Manual mock implementation of IResultCursor to work around NSubstitute's limitation
/// with non-generic ValueTask return types in IAsyncDisposable.DisposeAsync().
/// This mock properly implements both IDisposable and IAsyncDisposable.
/// </summary>
public class MockResultCursor : IResultCursor
{
	private IRecord? _singleRecord;
	private List<IRecord>? _recordsList;
	private Exception? _exceptionToThrow;
	private bool _throwOnSingleOrDefault;
	private bool _throwOnToList;
	private bool _isOpen = true;
	private IRecord? _current = null;

	/// <summary>
	/// Gets or sets the record to return from SingleOrDefaultAsync.
	/// </summary>
	public IRecord? SingleRecord
	{
		get => _singleRecord;
		set
		{
			_singleRecord = value;
			_throwOnSingleOrDefault = false;
		}
	}

	/// <summary>
	/// Gets or sets the list of records to return from ToListAsync.
	/// </summary>
	public List<IRecord>? RecordsList
	{
		get => _recordsList;
		set
		{
			_recordsList = value;
			_throwOnToList = false;
		}
	}

	/// <summary>
	/// Sets an exception to throw on the next SingleOrDefaultAsync call.
	/// </summary>
	public void SetExceptionForSingleOrDefault(Exception exception)
	{
		_exceptionToThrow = exception;
		_throwOnSingleOrDefault = true;
	}

	/// <summary>
	/// Sets an exception to throw on the next ToListAsync call.
	/// </summary>
	public void SetExceptionForToList(Exception exception)
	{
		_exceptionToThrow = exception;
		_throwOnToList = true;
	}

	/// <inheritdoc />
	public ValueTask<IRecord?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
	{
		if (_throwOnSingleOrDefault && _exceptionToThrow != null)
		{
			var ex = _exceptionToThrow;
			_exceptionToThrow = null;
			_throwOnSingleOrDefault = false;
			return ValueTask.FromException<IRecord?>(ex);
		}

		return ValueTask.FromResult<IRecord?>(_singleRecord);
	}

	/// <inheritdoc />
	public Task<List<IRecord>> ToListAsync(CancellationToken cancellationToken = default)
	{
		if (_throwOnToList && _exceptionToThrow != null)
		{
			var ex = _exceptionToThrow;
			_exceptionToThrow = null;
			_throwOnToList = false;
			return Task.FromException<List<IRecord>>(ex);
		}

		return Task.FromResult(_recordsList ?? new List<IRecord>());
	}

	/// <inheritdoc />
	public void Dispose()
	{
		// No-op for test mock
	}

	/// <inheritdoc />
	public ValueTask DisposeAsync()
	{
		return ValueTask.CompletedTask;
	}

	/// <inheritdoc />
	public Task<string[]> KeysAsync()
	{
		return Task.FromResult<string[]>(Array.Empty<string>());
	}

	/// <inheritdoc />
	public Task<IResultSummary> ConsumeAsync()
	{
		return Task.FromException<IResultSummary>(new NotImplementedException("ConsumeAsync not implemented in mock"));
	}

	/// <inheritdoc />
	public Task<bool> FetchAsync()
	{
		return Task.FromResult(false);
	}

	/// <inheritdoc />
	public IRecord? Current => _current;

	/// <inheritdoc />
	public bool IsOpen => _isOpen;

	// Other IResultCursor members that may not be used in tests but are required by the interface
	public IAsyncEnumerable<IRecord> StreamAsync() => new RecordAsyncEnumerable(_recordsList ?? new List<IRecord>());
	public Task<IRecord?> PeekAsync() => throw new NotImplementedException("PeekAsync not implemented in mock");
	public IAsyncEnumerator<IRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default) 
		=> new RecordAsyncEnumerator(_recordsList ?? new List<IRecord>(), cancellationToken);
}

/// <summary>
/// Simple async enumerable for records.
/// </summary>
internal sealed class RecordAsyncEnumerable : IAsyncEnumerable<IRecord>
{
	private readonly IReadOnlyList<IRecord> _records;

	public RecordAsyncEnumerable(IReadOnlyList<IRecord> records)
	{
		_records = records;
	}

	public IAsyncEnumerator<IRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
		=> new RecordAsyncEnumerator(_records, cancellationToken);
}

