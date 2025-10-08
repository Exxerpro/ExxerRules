# Test Data Loading Strategy Pattern Implementation

## Overview

This implementation provides a flexible, performant, and maintainable approach to test data loading using the Strategy Pattern. It addresses the performance and maintainability challenges of the original file-based approach while maintaining backward compatibility.

## Architecture

### Core Components

1. **ITestDataLoadingStrategy** - Interface defining the contract for all data loading strategies
2. **StaticTestDataStrategy** - Loads data from compile-time raw string literals (fastest)
3. **JsonTestDataStrategy** - Loads data from JSON files with caching
4. **HybridTestDataStrategy** - Combines multiple strategies with intelligent fallback
5. **TestDataStrategyFactory** - Manages and provides access to strategies
6. **TestDataLoaderV2** - Main API for loading test data

### Strategy Hierarchy

```
ITestDataLoadingStrategy
├── StaticTestDataStrategy (Fastest - compile-time)
├── JsonTestDataStrategy (Flexible - file-based)
└── HybridTestDataStrategy (Smart - combines all)
```

## Usage Examples

### Basic Usage

```csharp
// Load data using the best available strategy
var barCodes = await TestDataLoaderV2.LoadDataAsync<BarCode>("BarCodes");

// Load data using a specific strategy
var staticData = await TestDataLoaderV2.LoadStaticDataAsync<BarCode>("BarCodes");
var jsonData = await TestDataLoaderV2.LoadJsonDataAsync<BarCode>("BarCodes");
var hybridData = await TestDataLoaderV2.LoadHybridDataAsync<BarCode>("BarCodes");
```

### Advanced Usage

```csharp
// Configure the loader
var config = new TestDataStrategyConfiguration
{
    DefaultStrategy = "Static",
    EnableStaticStrategy = true,
    EnableJsonStrategy = false,
    EnableHybridStrategy = false
};
TestDataLoaderV2.Configure(config);

// Load with specific strategy type
var result = await TestDataLoaderV2.LoadDataWithStrategyAsync<BarCode>(
    "BarCodes",
    typeof(StaticTestDataStrategy));

// Check if loading was successful
if (result.IsSuccess)
{
    var data = result.Data;
    Console.WriteLine($"Loaded {data.Count} items using {result.StrategyUsed}");
    Console.WriteLine($"Load time: {result.LoadTime.TotalMilliseconds}ms");
}
```

### Performance Monitoring

```csharp
// Get metrics from all strategies
var metrics = TestDataLoaderV2.GetMetrics();
foreach (var (strategyName, metric) in metrics)
{
    Console.WriteLine($"{strategyName}:");
    Console.WriteLine($"  Total loads: {metric.TotalLoads}");
    Console.WriteLine($"  Cache hits: {metric.CacheHits}");
    Console.WriteLine($"  Cache hit ratio: {metric.CacheHitRatio:P}");
    Console.WriteLine($"  Average load time: {metric.AverageLoadTimeMs:F2}ms");
}
```

## Strategy Details

### StaticTestDataStrategy

**Performance**: ⭐⭐⭐⭐⭐ (Fastest)
**Flexibility**: ⭐⭐ (Requires recompilation)
**Use Case**: Frequently used, stable test data

```csharp
// Data is embedded in C# code using raw string literals
public static string SampleBarCodes => """
[
    {
        "id": 1,
        "label": "BC001",
        "partNumber": "PART-001"
    }
]
""";
```

**Benefits**:
- Zero I/O overhead
- No file path resolution
- Version controlled with code
- Fastest possible loading

**Drawbacks**:
- Requires recompilation for changes
- Limited to smaller datasets

### JsonTestDataStrategy

**Performance**: ⭐⭐⭐ (Good with caching)
**Flexibility**: ⭐⭐⭐⭐⭐ (Highly flexible)
**Use Case**: Large datasets, frequently changing data

```csharp
// Loads from JSON files with intelligent caching
var data = await TestDataLoaderV2.LoadJsonDataAsync<BarCode>("BarCodes.json");
```

**Benefits**:
- No recompilation needed
- Handles large datasets
- Intelligent caching
- Memory-mapped file support for very large files

**Drawbacks**:
- File I/O overhead (mitigated by caching)
- Path resolution complexity

### HybridTestDataStrategy

**Performance**: ⭐⭐⭐⭐ (Optimized)
**Flexibility**: ⭐⭐⭐⭐⭐ (Maximum flexibility)
**Use Case**: Production environments, mixed data sources

```csharp
// Automatically selects the best strategy with fallback
var data = await TestDataLoaderV2.LoadHybridDataAsync<BarCode>("BarCodes");
```

**Benefits**:
- Automatic strategy selection
- Intelligent fallback
- Deduplication support
- Best of all worlds

**Drawbacks**:
- Slightly more complex
- Potential for strategy conflicts

## Usage Tracking and Optimization

### Automatic Usage Tracking

The system automatically tracks which test data objects are accessed during test runs:

```csharp
// Usage is automatically logged when data is loaded
var barCodes = await TestDataLoaderV2.LoadDataAsync<BarCode>("BarCodes");
// TestEntityDataUsageTracker.LogUsage("BarCode", "1") is called automatically
```

### Generating Static Classes

Based on usage analysis, generate optimized static classes:

```csharp
// Generate static classes from current usage
var result = await TestDataLoaderV2.GenerateStaticClassesAsync("Generated");

if (result.IsSuccess)
{
    Console.WriteLine($"Generated {result.GeneratedFiles.Count} files");
    Console.WriteLine($"Total entities: {result.TotalEntities}");
}
```

### Usage Reports

```csharp
// Save usage report to file
await TestDataLoaderV2.SaveUsageReportAsync("usage_report.json");

// Get current usage statistics
var stats = TestDataLoaderV2.GetUsageStats();
Console.WriteLine($"Total registers accessed: {stats.TotalRegistersAccessed}");
Console.WriteLine($"Total bar codes accessed: {stats.TotalBarCodesAccessed}");
```

## Configuration Options

### TestDataStrategyConfiguration

```csharp
var config = new TestDataStrategyConfiguration
{
    // Default strategy to use
    DefaultStrategy = "Hybrid",

    // Strategy preference order
    StrategyPreference = new List<string> { "Static", "JSON", "Hybrid" },

    // Enable/disable specific strategies
    EnableStaticStrategy = true,
    EnableJsonStrategy = true,
    EnableHybridStrategy = true,

    // JSON file base path
    JsonBasePath = "CustomDataPath",

    // Feature toggles
    EnableCaching = true,
    EnableUsageTracking = true,
    EnableMetrics = true
};

TestDataLoaderV2.Configure(config);
```

## Migration Guide

### From Original TestDataLoader

**Before**:
```csharp
var data = await TestDataLoader.LoadDataAsync<BarCode>("BarCodes.json");
```

**After**:
```csharp
// Option 1: Direct replacement (uses best strategy)
var data = await TestDataLoaderV2.LoadDataAsync<BarCode>("BarCodes");

// Option 2: Explicit JSON strategy (maintains original behavior)
var data = await TestDataLoaderV2.LoadJsonDataAsync<BarCode>("BarCodes");

// Option 3: Hybrid approach (recommended)
var data = await TestDataLoaderV2.LoadHybridDataAsync<BarCode>("BarCodes");
```

### Performance Optimization Workflow

1. **Initial Setup**: Use hybrid strategy for maximum compatibility
2. **Usage Analysis**: Run tests to collect usage data
3. **Generate Static Data**: Create optimized static classes
4. **Performance Tuning**: Switch to static strategy for common cases
5. **Monitor and Iterate**: Use metrics to optimize further

## Best Practices

### Strategy Selection

- **Development**: Use hybrid strategy for flexibility
- **CI/CD**: Use static strategy for speed
- **Large Datasets**: Use JSON strategy with caching
- **Frequently Changing Data**: Use JSON strategy

### Performance Optimization

1. **Cache Management**: Clear caches between test runs if needed
2. **Memory Monitoring**: Use metrics to track memory usage
3. **Strategy Tuning**: Configure strategy preference based on usage patterns
4. **Static Generation**: Regularly generate static classes from usage data

### Error Handling

```csharp
try
{
    var data = await TestDataLoaderV2.LoadDataAsync<BarCode>("BarCodes");
}
catch (InvalidOperationException ex)
{
    // Handle loading failures
    Console.WriteLine($"Failed to load data: {ex.Message}");
}
```

## Testing

The implementation includes comprehensive unit tests covering:

- All strategy types
- Error conditions
- Performance metrics
- Configuration options
- Usage tracking
- Source generation

Run tests with:
```bash
dotnet test --filter "TestDataLoaderV2Tests"
```

## Future Enhancements

1. **Database Strategy**: Load from test databases
2. **API Strategy**: Load from REST APIs
3. **Dynamic Generation**: Generate test data programmatically
4. **Compression**: Support for compressed data sources
5. **Validation**: Schema validation for loaded data
6. **Parallel Loading**: Concurrent loading of multiple data sources

## Troubleshooting

### Common Issues

1. **File Not Found**: Check file paths and ensure JSON files exist
2. **Strategy Not Available**: Verify strategy is enabled in configuration
3. **Performance Issues**: Check cache hit ratios and consider static generation
4. **Memory Issues**: Monitor memory usage and clear caches if needed

### Debug Information

```csharp
// Enable detailed logging
var result = await TestDataLoaderV2.LoadDataWithStrategyAsync<BarCode>("BarCodes", typeof(HybridTestDataStrategy));

foreach (var warning in result.Warnings)
{
    Console.WriteLine($"Warning: {warning}");
}
```

This implementation provides a robust, flexible, and performant solution for test data loading that can evolve with your testing needs.
