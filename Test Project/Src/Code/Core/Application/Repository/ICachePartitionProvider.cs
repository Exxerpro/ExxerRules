namespace IndTrace.Application.Repository;

/// <summary>
/// Provides cache partition prefixes to isolate cache entries
/// </summary>
public interface ICachePartitionProvider
{
    /// <summary>
    /// Gets the cache partition prefix
    /// </summary>
    /// <returns>Empty string in production, GUID string in tests</returns>
    string GetPrefix();
}
