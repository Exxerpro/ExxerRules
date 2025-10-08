using IndTrace.Application.Repository;

namespace IndTrace.Persistence.Caching;

/// <summary>
/// Production implementation that returns empty prefix (no partitioning)
/// </summary>
public class ProductionCachePartitionProvider : ICachePartitionProvider
{
    /// <summary>
    /// Returns empty string for production (no cache partitioning)
    /// </summary>
    public string GetPrefix() => string.Empty;
}
