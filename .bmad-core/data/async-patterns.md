# .NET Async Patterns

## Async/Await Fundamentals

### Basic Async Patterns
```csharp
// Simple async method
public async Task<string> GetDataAsync()
{
    var data = await FetchDataFromApiAsync();
    return ProcessData(data);
}

// Async method with cancellation
public async Task<Result<T>> ProcessAsync<T>(CancellationToken cancellationToken = default)
{
    try
    {
        var result = await SomeAsyncOperation(cancellationToken).ConfigureAwait(false);
        return Result<T>.Success(result);
    }
    catch (OperationCanceledException)
    {
        return Result<T>.WithFailure("Operation was cancelled");
    }
}
```

### ConfigureAwait Best Practices
```csharp
// Library code should use ConfigureAwait(false)
public class DataService
{
    public async Task<string> GetDataAsync()
    {
        // Use ConfigureAwait(false) in library code
        var response = await _httpClient.GetAsync("api/data").ConfigureAwait(false);
        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}

// Application code can omit ConfigureAwait
public class Controller
{
    public async Task<IActionResult> GetData()
    {
        var data = await _dataService.GetDataAsync(); // No ConfigureAwait needed
        return Ok(data);
    }
}
```

## Advanced Async Patterns

### Async Enumerable
```csharp
// Async stream processing
public async IAsyncEnumerable<DataItem> ProcessDataStreamAsync(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    await foreach (var item in GetDataStreamAsync(cancellationToken))
    {
        if (cancellationToken.IsCancellationRequested)
            yield break;
            
        var processedItem = await ProcessItemAsync(item, cancellationToken);
        yield return processedItem;
    }
}

// Consuming async enumerable
public async Task ProcessAllDataAsync()
{
    await foreach (var item in ProcessDataStreamAsync())
    {
        // Process each item as it arrives
        Console.WriteLine($"Processed: {item.Name}");
    }
}
```

### ValueTask for Performance
```csharp
public class CachedDataService
{
    private readonly Dictionary<string, string> _cache = new();
    
    public ValueTask<string> GetDataAsync(string key)
    {
        if (_cache.TryGetValue(key, out var cachedValue))
        {
            return new ValueTask<string>(cachedValue);
        }
        
        return new ValueTask<string>(LoadDataAsync(key));
    }
    
    private async Task<string> LoadDataAsync(string key)
    {
        var data = await FetchFromApiAsync(key);
        _cache[key] = data;
        return data;
    }
}
```

## Concurrency Patterns

### Parallel Processing
```csharp
// Parallel async operations
public async Task<Result<List<ProcessedData>>> ProcessDataInParallelAsync(
    IEnumerable<RawData> dataItems, CancellationToken cancellationToken = default)
{
    var tasks = dataItems.Select(item => ProcessItemAsync(item, cancellationToken));
    
    try
    {
        var results = await Task.WhenAll(tasks);
        return Result<List<ProcessedData>>.Success(results.ToList());
    }
    catch (Exception ex) when (!(ex is OperationCanceledException))
    {
        return Result<List<ProcessedData>>.WithFailure($"Processing failed: {ex.Message}");
    }
}

// Parallel processing with concurrency limit
public async Task ProcessWithConcurrencyLimitAsync(
    IEnumerable<DataItem> items, int maxConcurrency = 5)
{
    var semaphore = new SemaphoreSlim(maxConcurrency);
    var tasks = items.Select(async item =>
    {
        await semaphore.WaitAsync();
        try
        {
            await ProcessItemAsync(item);
        }
        finally
        {
            semaphore.Release();
        }
    });
    
    await Task.WhenAll(tasks);
}
```

### Task Coordination
```csharp
// Task coordination with TaskCompletionSource
public class AsyncCoordinator
{
    private readonly TaskCompletionSource<bool> _tcs = new();
    private int _pendingOperations;
    
    public async Task<bool> WaitForCompletionAsync(TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        cts.Token.Register(() => _tcs.TrySetCanceled());
        
        return await _tcs.Task;
    }
    
    public void CompleteOperation()
    {
        if (Interlocked.Decrement(ref _pendingOperations) == 0)
        {
            _tcs.TrySetResult(true);
        }
    }
    
    public void StartOperation()
    {
        Interlocked.Increment(ref _pendingOperations);
    }
}
```

## Error Handling Patterns

### Async Exception Handling
```csharp
public class AsyncErrorHandler
{
    public async Task<Result<T>> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation, int maxRetries = 3)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var result = await operation();
                return Result<T>.Success(result);
            }
            catch (HttpRequestException ex) when (attempt < maxRetries)
            {
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                continue;
            }
            catch (Exception ex)
            {
                return Result<T>.WithFailure($"Operation failed: {ex.Message}");
            }
        }
        
        return Result<T>.WithFailure("Max retries exceeded");
    }
}
```

### Aggregate Exception Handling
```csharp
public async Task<Result<List<T>>> ProcessMultipleAsync<T>(
    IEnumerable<Func<Task<T>>> operations)
{
    var tasks = operations.Select(op => op()).ToArray();
    
    try
    {
        var results = await Task.WhenAll(tasks);
        return Result<List<T>>.Success(results.ToList());
    }
    catch (AggregateException ex)
    {
        var errors = ex.InnerExceptions.Select(e => e.Message);
        return Result<List<T>>.WithFailure($"Multiple errors: {string.Join(", ", errors)}");
    }
}
```

## Cancellation Patterns

### Cancellation Token Propagation
```csharp
public class CancellationAwareService
{
    public async Task<Result<T>> ProcessWithCancellationAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        TimeSpan timeout,
        CancellationToken externalToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
        cts.CancelAfter(timeout);
        
        try
        {
            var result = await operation(cts.Token);
            return Result<T>.Success(result);
        }
        catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
        {
            return Result<T>.WithFailure("Operation was cancelled");
        }
    }
}
```

### Graceful Shutdown
```csharp
public class GracefulShutdownService : IHostedService
{
    private readonly CancellationTokenSource _shutdownCts = new();
    private Task _backgroundTask;
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _backgroundTask = RunBackgroundWorkAsync(_shutdownCts.Token);
        await Task.CompletedTask;
    }
    
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _shutdownCts.Cancel();
        
        try
        {
            await _backgroundTask.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Expected during shutdown
        }
    }
    
    private async Task RunBackgroundWorkAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await DoWorkAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }
}
```

## Performance Patterns

### Async Caching
```csharp
public class AsyncCache<TKey, TValue>
{
    private readonly ConcurrentDictionary<TKey, Task<TValue>> _cache = new();
    private readonly Func<TKey, Task<TValue>> _factory;
    
    public AsyncCache(Func<TKey, Task<TValue>> factory)
    {
        _factory = factory;
    }
    
    public Task<TValue> GetAsync(TKey key)
    {
        return _cache.GetOrAdd(key, _factory);
    }
    
    public void Invalidate(TKey key)
    {
        _cache.TryRemove(key, out _);
    }
}
```

### Async Batching
```csharp
public class AsyncBatcher<T>
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly List<T> _batch = new();
    private readonly Timer _timer;
    private readonly Func<IEnumerable<T>, Task> _processor;
    private readonly int _batchSize;
    
    public AsyncBatcher(Func<IEnumerable<T>, Task> processor, int batchSize = 100, TimeSpan? flushInterval = null)
    {
        _processor = processor;
        _batchSize = batchSize;
        _timer = new Timer(FlushBatch, null, flushInterval ?? TimeSpan.FromSeconds(5), flushInterval ?? TimeSpan.FromSeconds(5));
    }
    
    public async Task AddAsync(T item)
    {
        await _semaphore.WaitAsync();
        try
        {
            _batch.Add(item);
            if (_batch.Count >= _batchSize)
            {
                await FlushBatchInternal();
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async void FlushBatch(object state)
    {
        await _semaphore.WaitAsync();
        try
        {
            await FlushBatchInternal();
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    private async Task FlushBatchInternal()
    {
        if (_batch.Count > 0)
        {
            var batchToProcess = _batch.ToList();
            _batch.Clear();
            await _processor(batchToProcess);
        }
    }
}
```

## Testing Async Code

### Async Unit Testing
```csharp
[Test]
public async Task ProcessDataAsync_ShouldReturnProcessedData()
{
    // Arrange
    var service = new DataService();
    var inputData = new RawData { Value = "test" };
    
    // Act
    var result = await service.ProcessDataAsync(inputData);
    
    // Assert
    Assert.That(result.IsSuccess, Is.True);
    Assert.That(result.Value.ProcessedValue, Is.EqualTo("TEST"));
}

[Test]
public async Task ProcessDataAsync_WithCancellation_ShouldReturnCancelledResult()
{
    // Arrange
    var service = new DataService();
    var cts = new CancellationTokenSource();
    cts.Cancel();
    
    // Act
    var result = await service.ProcessDataAsync(new RawData(), cts.Token);
    
    // Assert
    Assert.That(result.IsFailure, Is.True);
    Assert.That(result.Error, Does.Contain("cancelled"));
}
```

### Mocking Async Methods
```csharp
[Test]
public async Task GetDataAsync_ShouldReturnMockedData()
{
    // Arrange
    var mockService = Substitute.For<IDataService>();
    mockService.GetDataAsync().Returns(Task.FromResult("mocked data"));
    
    var controller = new DataController(mockService);
    
    // Act
    var result = await controller.GetData();
    
    // Assert
    Assert.That(result, Is.EqualTo("mocked data"));
    await mockService.Received(1).GetDataAsync();
}
```

## Common Async Anti-Patterns

### Blocking Async Code
```csharp
// BAD: Blocking async code
public string GetData()
{
    return GetDataAsync().Result; // Can cause deadlocks
}

// BAD: Using Wait() instead of await
public void ProcessData()
{
    GetDataAsync().Wait(); // Can cause deadlocks
}

// GOOD: Proper async/await
public async Task<string> GetDataAsync()
{
    return await GetDataAsync();
}
```

### Async Void
```csharp
// BAD: async void (except for event handlers)
public async void ProcessData()
{
    await SomeAsyncOperation(); // Exceptions won't be caught
}

// GOOD: async Task
public async Task ProcessDataAsync()
{
    await SomeAsyncOperation();
}

// GOOD: async void only for event handlers
private async void Button_Click(object sender, EventArgs e)
{
    await ProcessDataAsync();
}
```

### Fire-and-Forget
```csharp
// BAD: Fire-and-forget without proper error handling
public void StartBackgroundWork()
{
    _ = ProcessDataAsync(); // Exceptions will be lost
}

// GOOD: Proper fire-and-forget with error handling
public void StartBackgroundWork()
{
    _ = ProcessDataAsync().ContinueWith(task =>
    {
        if (task.IsFaulted)
        {
            _logger.LogError(task.Exception, "Background work failed");
        }
    }, TaskContinuationOptions.OnlyOnFaulted);
}
```

## Best Practices

### General Async Guidelines
1. **Use async/await**: Instead of blocking calls
2. **ConfigureAwait(false)**: In library code
3. **Handle Cancellation**: Support cancellation tokens
4. **Avoid async void**: Except for event handlers
5. **Use ValueTask<T>**: For high-performance scenarios
6. **Test async code**: Include async unit tests
7. **Handle exceptions**: Proper async exception handling
8. **Use async streams**: For streaming data
9. **Avoid deadlocks**: Don't block async code
10. **Profile performance**: Monitor async performance

### Performance Considerations
1. **Minimize allocations**: Use ValueTask when possible
2. **Use async streams**: For large data sets
3. **Implement batching**: For bulk operations
4. **Use caching**: For expensive async operations
5. **Monitor concurrency**: Avoid thread pool starvation
6. **Use semaphores**: For concurrency control
7. **Implement retries**: With exponential backoff
8. **Use timeouts**: For long-running operations
