namespace IndTrace.Aggregation.BoundedTests.Services;

/// <summary>
/// SEQUENTIAL TEST CONTEXT: One context, sequential operations, no concurrency
/// This is all you need - simple and effective
/// </summary>
public class SequentialTestContext : IDisposable
{
    private readonly IndTraceDbContext _context;

    public SequentialTestContext(string testName)
    {
        var options = new DbContextOptionsBuilder<IndTraceDbContext>()
            .UseInMemoryDatabase(testName)
            .Options;

        _context = new IndTraceDbContext(options);
    }

    // Single context for all operations
    public IIndTraceDbContext CreateDbContext() => _context;

    public void Dispose() => _context.Dispose();
}

/// <summary>
/// SIMPLE SEQUENTIAL TEST: No parallel operations, just step by step
/// </summary>
public class SimpleSequentialTest(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public async Task SequentialOperations_NoComplexity()
    {
        using var context = new SequentialTestContext(nameof(SequentialOperations_NoComplexity));

        // Step 1: Add data
        var db = context.CreateDbContext() as IndTraceDbContext;
        await db!.BarCodes.AddAsync(new BarCode
        {
            BarCodeId = 1,
            Label = "TEST-001",
            PartStatus = PartStatus.Ok
        }, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Step 2: Read it back

        // Step 3: Add more
        await db.BarCodes.AddAsync(new BarCode
        {
            BarCodeId = 2,
            Label = "TEST-002",
            PartStatus = PartStatus.Ok
        }, TestContext.Current.CancellationToken);
        await db.SaveChangesAsync(TestContext.Current.CancellationToken);

        _output.WriteLine("✅ Sequential operations completed - no concurrency needed!");
    }
}
