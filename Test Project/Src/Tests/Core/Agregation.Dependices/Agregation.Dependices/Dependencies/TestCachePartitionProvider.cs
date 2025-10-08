namespace IndTrace.Agregation.Dependices.Dependencies;

/// <summary>
/// Test implementation that provides per-test GUID partition
/// </summary>
public class TestCachePartitionProvider : ICachePartitionProvider
{
    private readonly string _partitionGuid;

    /// <summary>
    /// Creates a new instance with a unique partition GUID
    /// </summary>
    public TestCachePartitionProvider()
    {
        _partitionGuid = Guid.NewGuid().ToString("N");
    }

    /// <summary>
    /// Returns the unique partition GUID for this test
    /// </summary>
    public string GetPrefix() => _partitionGuid;
}
