namespace IndTrace.Persistence.Caching;

/// <summary>
/// Options to enable/disable caching globally via configuration.
/// </summary>
public class CacheToggleOptions
{
    /// <summary>
    /// When false, caching operations are bypassed.
    /// Default true.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
