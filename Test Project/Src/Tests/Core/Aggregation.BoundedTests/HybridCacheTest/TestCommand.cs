namespace IndTrace.Aggregation.BoundedTests.HybridCacheTest;

public class TestCommand
{
    public int Id { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string[] Includes { get; set; } = Array.Empty<string>();
    public string Name { get; set; } = string.Empty;
}
