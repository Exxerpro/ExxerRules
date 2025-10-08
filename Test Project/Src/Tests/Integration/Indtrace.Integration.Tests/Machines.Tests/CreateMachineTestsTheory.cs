using Integration.Tests.Extensions;

namespace Integration.Tests.Machines.Tests;
/// <summary>
/// Represents the CreateMachineTestsTheory.
/// </summary>

public class CreateMachineTestsTheory : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services;
    private readonly ITestOutputHelper _output;
    public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    /// <param name="fixture">The test host fixture.</param>
    /// <param name="output">The output.</param>

    public CreateMachineTestsTheory(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output)
    {
        _services = fixture.Services;
        _output = output;
    }

    public static IEnumerable<object[]> DuplicateMachineTestCases => new List<object[]>
    {
        new object[] { 400, "NewName" }, // ID exists
        new object[] { 999, "WS400" },   // Name exists
        new object[] { 400, "WS400" },   // Both exist
        new object[] { 500, "WS500" },   // Common case
    };

    /// <summary>
    /// Executes Should_Fail_When_MachineId_Or_Name_Already_Exists operation.
    /// </summary>
    /// <param name="machineId">The machineId.</param>
    /// <param name="name">The name.</param>
    /// <returns>The result of Should_Fail_When_MachineId_Or_Name_Already_Exists.</returns>

    [Theory(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
    [Trait("Db", "DB45")]
    [MemberData(nameof(DuplicateMachineTestCases))]
    public async Task Should_Fail_When_MachineId_Or_Name_Already_Exists(int machineId, string name)
    {
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        var (sut, repo, logger) = _services.BuildHandler<CreateMachineMonitorRequestHandler, Machine>(_output, dbKey,
            (r, l) => new CreateMachineMonitorRequestHandler(r, l));

        var seedMachine = new Machine
        {
            MachineId = 400,
            Name = "WS400",
            Location = "RAM",
            MachineType = 1,
            EnableAppTraceability = 1,
            EnableBypassTraceability = 0
        };

        await repo.AddAsync(seedMachine, TestContext.Current.CancellationToken);
        await repo.DetachAsync(seedMachine, TestContext.Current.CancellationToken);

        // Act
        var request = new CreateMachineMonitorRequest
        {
            MachineId = machineId,
            Name = name,
            Location = "AUDI",
            MachineType = 2,
            EnableAppTraceability = 1,
            EnableBypassTraceability = 0
        };

        var result = await sut.ProcessAsync(request, TestContext.Current.CancellationToken);
        await Task.Yield(); // safer, more semantically meaningful

        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("A machine with the same ID or name already exists.");
    }
}
