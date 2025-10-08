namespace IndTrace.Aggregation.BoundedTests.DiagnosticTests;

/// <summary>
/// Test using normal read/write repository (not cached) to isolate cache vs converter issues
/// </summary>
public class NormalRepositoryTest
{
    [Fact]
    public async Task NormalRepository_ShouldWork_WithConverter()
    {
        // Arrange - Create InMemory database with standard repository (no cache)
        var options = new DbContextOptionsBuilder<IndTraceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .Options;

        using var context = new IndTraceDbContext(options);
        await context.Database.EnsureCreatedAsync(TestContext.Current.CancellationToken);



        // Create a machine with explicit enum values (like the failing test)
        var machine = new Machine
        {
            MachineId = 300,
            Name = "WS300",
            Description = "SPOILER",
            Location = "SPOILER",
            MachineType = MachineType.Process,  // This should have value 8
            WorkFlowType = WorkFlowType.Serial,
            EnableAppTraceability = 1,
            EnableBypassTraceability = 0,
            RuleId = 300
        };

        Console.WriteLine($"Before Save - MachineType: {machine.MachineType?.Name} (Value: {machine.MachineType?.Value})");

        // Act - Use repository to save (not direct context)




    }
}
