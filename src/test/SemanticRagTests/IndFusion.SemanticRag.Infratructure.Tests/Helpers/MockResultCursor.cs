using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Logging;
using Neo4j.Driver;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Infratructure.Tests.Helpers;

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
    private readonly ILogger<MockResultCursor>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockResultCursor"/> class.
    /// </summary>
    /// <param name="output">Optional test output helper for logging.</param>
    public MockResultCursor(ITestOutputHelper? output = null)
    {
        if (output != null)
        {
            _logger = XUnitLogger.CreateLogger<MockResultCursor>(output);
            _logger.LogInformation("MockResultCursor initialized");
        }
    }

    /// <summary>
    /// Gets or sets the record to return from SingleOrDefaultAsync.
    /// </summary>
    public IRecord? SingleRecord
    {
        get => _singleRecord;
        set
        {
            _logger?.LogInformation("SingleRecord setter called. Previous value: {PreviousValue}, New value: {NewValue}", 
                _singleRecord != null ? "not null" : "null", 
                value != null ? "not null" : "null");
            _singleRecord = value;
            _throwOnSingleOrDefault = false;
            _logger?.LogInformation("SingleRecord set. _throwOnSingleOrDefault cleared: {ThrowFlag}", _throwOnSingleOrDefault);
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
        _logger?.LogInformation("SetExceptionForSingleOrDefault called. Exception: {ExceptionType}, Message: {Message}", 
            exception.GetType().Name, exception.Message);
        _exceptionToThrow = exception;
        _throwOnSingleOrDefault = true;
        _singleRecord = null; // Clear the record when setting exception
        _logger?.LogInformation("Exception set. _throwOnSingleOrDefault: {ThrowFlag}, _singleRecord: {RecordValue}", 
            _throwOnSingleOrDefault, _singleRecord != null ? "not null" : "null");
    }

    /// <summary>
    /// Sets an exception to throw on the next ToListAsync call.
    /// </summary>
    public void SetExceptionForToList(Exception exception)
    {
        _logger?.LogInformation("SetExceptionForToList called. Exception: {ExceptionType}, Message: {Message}", 
            exception.GetType().Name, exception.Message);
        _exceptionToThrow = exception;
        _throwOnToList = true;
        _recordsList = null; // Clear the records when setting exception
        _logger?.LogInformation("Exception set. _throwOnToList: {ThrowFlag}, _recordsList: {RecordsValue}", 
            _throwOnToList, _recordsList != null ? "not null" : "null");
    }

    /// <inheritdoc />
    public ValueTask<IRecord?> SingleOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("=== MockResultCursor.SingleOrDefaultAsync CALLED ===");
        _logger?.LogInformation("_throwOnSingleOrDefault: {ThrowFlag}", _throwOnSingleOrDefault);
        _logger?.LogInformation("_exceptionToThrow: {HasException} ({ExceptionType})", 
            _exceptionToThrow != null, _exceptionToThrow?.GetType().Name ?? "null");
        _logger?.LogInformation("_singleRecord: {HasRecord}", _singleRecord != null);
        
        if (_throwOnSingleOrDefault && _exceptionToThrow != null)
        {
            var ex = _exceptionToThrow;
            _logger?.LogInformation("THROWING exception from SingleOrDefaultAsync: {ExceptionType}, Message: {Message}", 
                ex.GetType().Name, ex.Message);
            _exceptionToThrow = null;
            _throwOnSingleOrDefault = false;
            var valueTask = ValueTask.FromException<IRecord?>(ex);
            _logger?.LogInformation("ValueTask.FromException created. IsFaulted: {IsFaulted}", 
                valueTask.IsFaulted);
            return valueTask;
        }

        _logger?.LogInformation("RETURNING record from SingleOrDefaultAsync. Record: {HasRecord}, Value: {RecordId}", 
            _singleRecord != null, _singleRecord?["id"]?.ToString() ?? "null");
        return ValueTask.FromResult<IRecord?>(_singleRecord);
    }

    /// <inheritdoc />
    public Task<List<IRecord>> ToListAsync(CancellationToken cancellationToken = default)
    {
        _logger?.LogInformation("=== MockResultCursor.ToListAsync CALLED ===");
        _logger?.LogInformation("_throwOnToList: {ThrowFlag}", _throwOnToList);
        _logger?.LogInformation("_exceptionToThrow: {HasException} ({ExceptionType})", 
            _exceptionToThrow != null, _exceptionToThrow?.GetType().Name ?? "null");
        _logger?.LogInformation("_recordsList: {HasRecords} (Count: {Count})", 
            _recordsList != null, _recordsList?.Count ?? 0);
        
        if (_throwOnToList && _exceptionToThrow != null)
        {
            var ex = _exceptionToThrow;
            _logger?.LogInformation("THROWING exception from ToListAsync: {ExceptionType}, Message: {Message}", 
                ex.GetType().Name, ex.Message);
            _exceptionToThrow = null;
            _throwOnToList = false;
            var task = Task.FromException<List<IRecord>>(ex);
            _logger?.LogInformation("Task.FromException created. IsFaulted: {IsFaulted}", 
                task.IsFaulted);
            return task;
        }

        var records = _recordsList ?? [];
        _logger?.LogInformation("RETURNING {Count} records from ToListAsync", records.Count);
        return Task.FromResult(records);
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
    public IAsyncEnumerable<IRecord> StreamAsync() => new RecordAsyncEnumerable(_recordsList ?? []);

    public Task<IRecord?> PeekAsync() => throw new NotImplementedException("PeekAsync not implemented in mock");

    public IAsyncEnumerator<IRecord> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new RecordAsyncEnumerator(_recordsList ?? [], cancellationToken);
}