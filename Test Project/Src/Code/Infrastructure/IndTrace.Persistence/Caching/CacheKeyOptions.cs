namespace IndTrace.Persistence.Caching;

/// <summary>
/// Options for cache key generation
/// </summary>
public class CacheKeyOptions
{
    /// <summary>
    /// When true, specification keys are hashed.
    /// Key shape becomes: {Operation}|Type:{T}|Spec:{Hash}
    /// </summary>
    public bool HashSpecKeys { get; set; } = false;

    /// <summary>
    /// Length of the hex hash prefix for spec keys.
    /// </summary>
    public int HashLength { get; set; } = 16;
}
