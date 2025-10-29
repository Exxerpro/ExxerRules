# .NET Memory Management Patterns

## Memory Management Fundamentals

### Garbage Collection Basics
```csharp
// Understanding GC generations
public class GarbageCollectionExample
{
    public void DemonstrateGenerations()
    {
        // Generation 0: Short-lived objects
        var shortLived = new List<int>();
        
        // Generation 1: Medium-lived objects
        var mediumLived = new List<string>();
        GC.Collect(0); // Only collect generation 0
        
        // Generation 2: Long-lived objects
        var longLived = new Dictionary<string, object>();
        GC.Collect(); // Collect all generations
    }
}
```

### IDisposable Pattern
```csharp
// Proper IDisposable implementation
public class ResourceManager : IDisposable
{
    private bool _disposed = false;
    private readonly Stream _stream;
    
    public ResourceManager(string filePath)
    {
        _stream = File.OpenRead(filePath);
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _stream?.Dispose();
            }
            _disposed = true;
        }
    }
    
    ~ResourceManager()
    {
        Dispose(false);
    }
}
```

## Memory Leak Prevention

### Event Handler Management
```csharp
// Weak event pattern to prevent memory leaks
public class WeakEventManager
{
    private readonly List<WeakReference<EventHandler>> _handlers = new();
    
    public void AddHandler(EventHandler handler)
    {
        _handlers.Add(new WeakReference<EventHandler>(handler));
    }
    
    public void RemoveHandler(EventHandler handler)
    {
        _handlers.RemoveAll(wr => 
            wr.TryGetTarget(out var target) && target == handler);
    }
    
    public void RaiseEvent(object sender, EventArgs e)
    {
        for (int i = _handlers.Count - 1; i >= 0; i--)
        {
            if (_handlers[i].TryGetTarget(out var handler))
            {
                handler(sender, e);
            }
            else
            {
                _handlers.RemoveAt(i);
            }
        }
    }
}
```

### Static Collection Management
```csharp
// Proper static collection management
public class CacheManager
{
    private static readonly ConcurrentDictionary<string, object> _cache = new();
    private static readonly Timer _cleanupTimer;
    
    static CacheManager()
    {
        // Cleanup every 5 minutes
        _cleanupTimer = new Timer(CleanupExpiredItems, null, 
            TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    public static void AddItem(string key, object value, TimeSpan expiration)
    {
        var item = new CacheItem(value, DateTime.UtcNow.Add(expiration));
        _cache.AddOrUpdate(key, item, (k, v) => item);
    }
    
    private static void CleanupExpiredItems(object state)
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _cache
            .Where(kvp => kvp.Value.ExpirationTime < now)
            .Select(kvp => kvp.Key)
            .ToList();
        
        foreach (var key in expiredKeys)
        {
            _cache.TryRemove(key, out _);
        }
    }
    
    private class CacheItem
    {
        public object Value { get; }
        public DateTime ExpirationTime { get; }
        
        public CacheItem(object value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }
    }
}
```

## High-Performance Memory Patterns

### Span<T> and Memory<T>
```csharp
// Zero-allocation string processing
public static int CountOccurrences(ReadOnlySpan<char> text, char character)
{
    int count = 0;
    for (int i = 0; i < text.Length; i++)
    {
        if (text[i] == character)
            count++;
    }
    return count;
}

// Memory<T> for async operations
public async Task<int> ProcessDataAsync(Memory<byte> data)
{
    // Process data without additional allocations
    var span = data.Span;
    int sum = 0;
    for (int i = 0; i < span.Length; i++)
    {
        sum += span[i];
    }
    return sum;
}
```

### ArrayPool<T> Usage
```csharp
public class ArrayPoolExample
{
    private static readonly ArrayPool<byte> _pool = ArrayPool<byte>.Shared;
    
    public async Task<byte[]> ProcessLargeDataAsync(Stream input)
    {
        var buffer = _pool.Rent(8192);
        try
        {
            var result = new List<byte>();
            int bytesRead;
            
            while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                // Process buffer
                for (int i = 0; i < bytesRead; i++)
                {
                    result.Add(buffer[i]);
                }
            }
            
            return result.ToArray();
        }
        finally
        {
            _pool.Return(buffer);
        }
    }
}
```

## Memory Profiling and Analysis

### Memory Allocation Tracking
```csharp
public class MemoryTracker
{
    private readonly ILogger<MemoryTracker> _logger;
    private long _initialMemory;
    
    public void StartTracking()
    {
        _initialMemory = GC.GetTotalMemory(false);
        _logger.LogInformation("Memory tracking started. Initial: {Memory} bytes", _initialMemory);
    }
    
    public void LogMemoryUsage(string operation)
    {
        var currentMemory = GC.GetTotalMemory(false);
        var allocated = currentMemory - _initialMemory;
        
        _logger.LogInformation("Operation: {Operation}, Memory: {Current} bytes, Allocated: {Allocated} bytes", 
            operation, currentMemory, allocated);
    }
    
    public void ForceGC()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        var finalMemory = GC.GetTotalMemory(false);
        _logger.LogInformation("After GC: {Memory} bytes", finalMemory);
    }
}
```

### Memory Leak Detection
```csharp
public class MemoryLeakDetector
{
    private readonly Timer _timer;
    private long _previousMemory;
    private int _consecutiveIncreases;
    
    public MemoryLeakDetector()
    {
        _previousMemory = GC.GetTotalMemory(false);
        _timer = new Timer(CheckMemoryUsage, null, 
            TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }
    
    private void CheckMemoryUsage(object state)
    {
        var currentMemory = GC.GetTotalMemory(false);
        
        if (currentMemory > _previousMemory)
        {
            _consecutiveIncreases++;
            if (_consecutiveIncreases >= 5)
            {
                // Potential memory leak detected
                OnMemoryLeakDetected?.Invoke(currentMemory, _previousMemory);
            }
        }
        else
        {
            _consecutiveIncreases = 0;
        }
        
        _previousMemory = currentMemory;
    }
    
    public event Action<long, long> OnMemoryLeakDetected;
}
```

## Resource Management Patterns

### Using Statements
```csharp
// Modern using declarations
public async Task ProcessFileAsync(string filePath)
{
    using var fileStream = File.OpenRead(filePath);
    using var reader = new StreamReader(fileStream);
    
    string line;
    while ((line = await reader.ReadLineAsync()) != null)
    {
        // Process line
    }
}

// Traditional using statement
public void ProcessFile(string filePath)
{
    using (var fileStream = File.OpenRead(filePath))
    using (var reader = new StreamReader(fileStream))
    {
        // Process file
    }
}
```

### Async Resource Management
```csharp
// Async disposal pattern
public class AsyncResourceManager : IAsyncDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed = false;
    
    public AsyncResourceManager()
    {
        _semaphore = new SemaphoreSlim(1, 1);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _semaphore.WaitAsync();
            try
            {
                // Cleanup async resources
                await CleanupAsync();
            }
            finally
            {
                _semaphore.Release();
                _disposed = true;
            }
        }
    }
    
    private async Task CleanupAsync()
    {
        // Async cleanup logic
        await Task.Delay(100); // Simulate async cleanup
    }
}
```

## Memory Optimization Techniques

### Object Pooling
```csharp
public class ObjectPool<T> where T : class, new()
{
    private readonly ConcurrentQueue<T> _objects = new();
    private readonly Func<T> _objectGenerator;
    
    public ObjectPool(Func<T> objectGenerator = null)
    {
        _objectGenerator = objectGenerator ?? (() => new T());
    }
    
    public T Get()
    {
        if (_objects.TryDequeue(out T item))
        {
            return item;
        }
        return _objectGenerator();
    }
    
    public void Return(T item)
    {
        if (item != null)
        {
            _objects.Enqueue(item);
        }
    }
}

// Usage example
public class StringBuilderPool
{
    private static readonly ObjectPool<StringBuilder> _pool = new();
    
    public static string BuildString(Action<StringBuilder> builder)
    {
        var sb = _pool.Get();
        try
        {
            sb.Clear();
            builder(sb);
            return sb.ToString();
        }
        finally
        {
            _pool.Return(sb);
        }
    }
}
```

### Lazy Initialization
```csharp
public class LazyResourceManager
{
    private readonly Lazy<ExpensiveResource> _resource;
    
    public LazyResourceManager()
    {
        _resource = new Lazy<ExpensiveResource>(() => new ExpensiveResource());
    }
    
    public void UseResource()
    {
        var resource = _resource.Value;
        // Use resource
    }
}

// Thread-safe lazy initialization
public class ThreadSafeLazyManager
{
    private volatile ExpensiveResource _resource;
    private readonly object _lock = new object();
    
    public ExpensiveResource GetResource()
    {
        if (_resource == null)
        {
            lock (_lock)
            {
                if (_resource == null)
                {
                    _resource = new ExpensiveResource();
                }
            }
        }
        return _resource;
    }
}
```

## Memory Monitoring and Diagnostics

### Memory Pressure Monitoring
```csharp
public class MemoryPressureMonitor
{
    private readonly ILogger<MemoryPressureMonitor> _logger;
    private readonly Timer _timer;
    
    public MemoryPressureMonitor(ILogger<MemoryPressureMonitor> logger)
    {
        _logger = logger;
        _timer = new Timer(CheckMemoryPressure, null, 
            TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
    }
    
    private void CheckMemoryPressure(object state)
    {
        var memoryInfo = GC.GetGCMemoryInfo();
        var totalMemory = GC.GetTotalMemory(false);
        
        _logger.LogInformation("Memory Info - Total: {Total} bytes, Gen0: {Gen0}, Gen1: {Gen1}, Gen2: {Gen2}", 
            totalMemory, memoryInfo.Generation0Collections, 
            memoryInfo.Generation1Collections, memoryInfo.Generation2Collections);
        
        if (memoryInfo.HeapSizeBytes > 100 * 1024 * 1024) // 100MB
        {
            _logger.LogWarning("High memory usage detected: {HeapSize} bytes", memoryInfo.HeapSizeBytes);
        }
    }
}
```

### Memory Dump Analysis
```csharp
public class MemoryDumpAnalyzer
{
    public void AnalyzeMemoryDump()
    {
        // Force garbage collection
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        
        // Get memory information
        var memoryInfo = GC.GetGCMemoryInfo();
        var totalMemory = GC.GetTotalMemory(false);
        
        Console.WriteLine($"Total Memory: {totalMemory:N0} bytes");
        Console.WriteLine($"Heap Size: {memoryInfo.HeapSizeBytes:N0} bytes");
        Console.WriteLine($"Fragmented Bytes: {memoryInfo.FragmentedBytes:N0} bytes");
        Console.WriteLine($"Generation 0 Collections: {memoryInfo.Generation0Collections}");
        Console.WriteLine($"Generation 1 Collections: {memoryInfo.Generation1Collections}");
        Console.WriteLine($"Generation 2 Collections: {memoryInfo.Generation2Collections}");
    }
}
```

## Best Practices

### Memory Management Guidelines
1. **Implement IDisposable**: For resources that need cleanup
2. **Use Using Statements**: For automatic resource disposal
3. **Avoid Memory Leaks**: Properly unsubscribe from events
4. **Use Weak References**: For event handlers and caches
5. **Monitor Memory Usage**: Set up alerts and monitoring
6. **Use Object Pooling**: For frequently created objects
7. **Optimize Collections**: Choose appropriate collection types
8. **Use Span<T> and Memory<T>**: For zero-allocation operations
9. **Implement Async Disposal**: For async resources
10. **Profile Memory Usage**: Use memory profiling tools

### Common Memory Issues
1. **Event Handler Leaks**: Not unsubscribing from events
2. **Static Collections**: Growing without bounds
3. **Circular References**: Preventing garbage collection
4. **Large Object Heap**: Objects over 85KB
5. **Finalizers**: Incorrect finalizer implementation
6. **Resource Leaks**: Not disposing of resources
7. **Memory Fragmentation**: Poor allocation patterns
8. **GC Pressure**: Excessive allocations
