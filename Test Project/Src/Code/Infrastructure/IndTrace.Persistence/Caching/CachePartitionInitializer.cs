using IndTrace.Application.Repository;
using Microsoft.Extensions.Options;

namespace IndTrace.Persistence.Caching;

/// <summary>
/// Small initializer to attach the DI-provided partition provider to the static cache key builder.
/// </summary>
public class CachePartitionInitializer
{
    public CachePartitionInitializer(ICachePartitionProvider provider, IOptions<CacheKeyOptions>? options = null)
    {
        CacheKeyBuilderReadOnlyRepos.SetPartitionProvider(provider);
        CacheKeyBuilderReadOnlyRepos.SetOptions(options?.Value);
    }
}
