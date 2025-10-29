# .NET Performance Patterns

## High-Performance C# Patterns

### Memory Management
```csharp
// Use Span<T> for zero-allocation operations
public static int ProcessData(ReadOnlySpan<byte> data)
{
    int sum = 0;
    for (int i = 0; i < data.Length; i++)
    {
        sum += data[i];
    }
    return sum;
}

// Use ArrayPool<T> for temporary arrays
public static void ProcessLargeArray()
{
    var pool = ArrayPool<int>.Shared;
    var array = pool.Rent(1000);
    try
    {
        // Process array
    }
    finally
    {
        pool.Return(array);
    }
}
```

### Async Performance
```csharp
// Use ConfigureAwait(false) in library code
public async Task<Result<T>> ProcessAsync(CancellationToken cancellationToken = default)
{
    var result = await SomeAsyncOperation().ConfigureAwait(false);
    return Result<T>.Success(result);
}

// Use ValueTask<T> for high-performance scenarios
public ValueTask<int> GetCachedValueAsync(int key)
{
    if (_cache.TryGetValue(key, out var value))
        return new ValueTask<int>(value);
    
    return new ValueTask<int>(LoadValueAsync(key));
}
```

### LINQ Optimization
```csharp
// Use specific LINQ methods for better performance
public static IEnumerable<T> OptimizedFilter<T>(IEnumerable<T> source, Func<T, bool> predicate)
{
    return source.Where(predicate); // Use Where instead of Select + Where
}

// Use ToArray() or ToList() only when needed
public static T[] ProcessItems<T>(IEnumerable<T> items)
{
    return items.Where(x => x != null).ToArray();
}
```

## Performance Anti-Patterns

### Common Performance Issues
```csharp
// BAD: Unnecessary async/await
public async Task<int> BadAsync()
{
    return await Task.FromResult(42); // Unnecessary async
}

// GOOD: Direct return
public Task<int> GoodAsync()
{
    return Task.FromResult(42);
}

// BAD: String concatenation in loops
public string BadStringConcatenation(IEnumerable<string> items)
{
    string result = "";
    foreach (var item in items)
    {
        result += item; // Creates new string each time
    }
    return result;
}

// GOOD: Use StringBuilder
public string GoodStringConcatenation(IEnumerable<string> items)
{
    var sb = new StringBuilder();
    foreach (var item in items)
    {
        sb.Append(item);
    }
    return sb.ToString();
}
```

### Memory Allocation Issues
```csharp
// BAD: Unnecessary allocations
public void BadMethod()
{
    var list = new List<int>();
    for (int i = 0; i < 1000; i++)
    {
        list.Add(i); // Allocates new List<int> each time
    }
}

// GOOD: Reuse collections
private readonly List<int> _reusableList = new();
public void GoodMethod()
{
    _reusableList.Clear();
    for (int i = 0; i < 1000; i++)
    {
        _reusableList.Add(i);
    }
}
```

## Profiling and Benchmarking

### BenchmarkDotNet Usage
```csharp
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
public class PerformanceBenchmark
{
    private readonly int[] _data = Enumerable.Range(0, 1000).ToArray();
    
    [Benchmark]
    public int SumWithLINQ()
    {
        return _data.Sum();
    }
    
    [Benchmark]
    public int SumWithForLoop()
    {
        int sum = 0;
        for (int i = 0; i < _data.Length; i++)
        {
            sum += _data[i];
        }
        return sum;
    }
}
```

### Performance Counters
```csharp
public class PerformanceCounter
{
    private readonly Counter _requestCounter;
    private readonly Histogram _responseTimeHistogram;
    
    public PerformanceCounter()
    {
        _requestCounter = Meter.CreateCounter<int>("requests_total");
        _responseTimeHistogram = Meter.CreateHistogram<double>("response_time_seconds");
    }
    
    public void RecordRequest(double responseTime)
    {
        _requestCounter.Add(1);
        _responseTimeHistogram.Record(responseTime);
    }
}
```

## Caching Strategies

### Memory Caching
```csharp
public class CachedService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);
    
    public async Task<T> GetCachedDataAsync<T>(string key, Func<Task<T>> factory)
    {
        if (_cache.TryGetValue(key, out T cachedValue))
            return cachedValue;
        
        var value = await factory();
        _cache.Set(key, value, _cacheExpiry);
        return value;
    }
}
```

### Distributed Caching
```csharp
public class DistributedCachedService
{
    private readonly IDistributedCache _cache;
    private readonly IJsonSerializer _serializer;
    
    public async Task<T> GetCachedDataAsync<T>(string key, Func<Task<T>> factory)
    {
        var cachedData = await _cache.GetStringAsync(key);
        if (cachedData != null)
        {
            return _serializer.Deserialize<T>(cachedData);
        }
        
        var value = await factory();
        var serializedValue = _serializer.Serialize(value);
        await _cache.SetStringAsync(key, serializedValue);
        return value;
    }
}
```

## Database Performance

### Entity Framework Optimization
```csharp
// Use AsNoTracking for read-only queries
public async Task<List<Customer>> GetCustomersAsync()
{
    return await _context.Customers
        .AsNoTracking()
        .Where(c => c.IsActive)
        .ToListAsync();
}

// Use projection to select only needed fields
public async Task<List<CustomerDto>> GetCustomerDtosAsync()
{
    return await _context.Customers
        .Select(c => new CustomerDto
        {
            Id = c.Id,
            Name = c.Name,
            Email = c.Email
        })
        .ToListAsync();
}
```

### Connection Pooling
```csharp
// Configure connection pooling
services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.CommandTimeout(30);
        sqlOptions.EnableRetryOnFailure(3);
    });
});
```

## I/O Performance

### Async I/O Operations
```csharp
public async Task<byte[]> ReadFileAsync(string filePath)
{
    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
    var buffer = new byte[fileStream.Length];
    await fileStream.ReadAsync(buffer, 0, buffer.Length);
    return buffer;
}
```

### Network Performance
```csharp
public class HttpClientService
{
    private readonly HttpClient _httpClient;
    
    public HttpClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }
    
    public async Task<string> GetAsync(string url)
    {
        using var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
```

## Monitoring and Alerting

### Performance Monitoring
```csharp
public class PerformanceMonitor
{
    private readonly ILogger<PerformanceMonitor> _logger;
    private readonly IMetrics _metrics;
    
    public async Task<T> MonitorAsync<T>(string operationName, Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await operation();
            _metrics.RecordSuccess(operationName, stopwatch.Elapsed);
            return result;
        }
        catch (Exception ex)
        {
            _metrics.RecordFailure(operationName, stopwatch.Elapsed);
            _logger.LogError(ex, "Operation {OperationName} failed", operationName);
            throw;
        }
    }
}
```

### Health Checks
```csharp
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.CanConnectAsync(cancellationToken);
            return HealthCheckResult.Healthy("Database is accessible");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is not accessible", ex);
        }
    }
}
```

## Best Practices

### General Performance Guidelines
1. **Measure Before Optimizing**: Use profiling tools to identify bottlenecks
2. **Optimize Hot Paths**: Focus on frequently executed code
3. **Minimize Allocations**: Reduce garbage collection pressure
4. **Use Appropriate Data Structures**: Choose the right collection type
5. **Implement Caching**: Cache expensive operations
6. **Use Async I/O**: Don't block threads for I/O operations
7. **Monitor Performance**: Set up alerts and dashboards
8. **Test Performance**: Include performance tests in CI/CD

### Memory Management
1. **Use Span<T> and Memory<T>**: For zero-allocation operations
2. **Implement IDisposable**: Properly dispose of resources
3. **Use Object Pooling**: For frequently created objects
4. **Avoid Large Object Heap**: Keep objects under 85KB
5. **Monitor GC Pressure**: Watch for excessive garbage collection

### Async Performance
1. **Use ConfigureAwait(false)**: In library code
2. **Avoid async void**: Except for event handlers
3. **Use ValueTask<T>**: For high-performance scenarios
4. **Implement Cancellation**: For long-running operations
5. **Use SemaphoreSlim**: For async synchronization
