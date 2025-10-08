namespace Integration.Tests.Machines.Tests.Commands;
/// <summary>
/// Represents the CreateMachineTests.
/// </summary>

public class CreateMachineTests(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output) : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services = fixture.Services;
    private readonly ITestOutputHelper _output = output;
    public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;
    /// <summary>
    /// Executes ShouldThrowExceptionWhenMachineExists operation.
    /// </summary>
    /// <returns>The result of ShouldThrowExceptionWhenMachineExists.</returns>

    [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
    [Trait("Db","DB45")]
    public async Task ShouldThrowExceptionWhenMachineExists()
    {
        var mockMediator = Substitute.For<IMonitorRequestDispatcher>();
        var dateTimeMock = Substitute.For<IDateTimeMachine>();
        dateTimeMock.Now.Returns(new DateTime(2020, 06, 06, 06, 06, 06, 6));

        var request = new CreateMachineMonitorRequest
        {
            MachineId = 100,
            Name = "Station 60",
            Location = "Trunk",
            MachineType = 2,
            EnableAppTraceability = 1,
            EnableBypassTraceability = 0
        };

        var logger = XUnitLogger.CreateLogger<CreateMachineMonitorRequestHandler>();
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CreateMachineTests));

        var machineRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(dbKey);
        var sut = new CreateMachineMonitorRequestHandler(machineRepo, logger);
        var result = await sut.ProcessAsync(request, TestContext.Current.CancellationToken);

        result.IsFailure.ShouldBeTrue("Because this machine already exist");

        result.Errors.ShouldContain("A machine with the same ID or name already exists.");
    }

    /// <summary>
    /// Executes ShouldThrowExceptionWhenMachineIdExists operation.
    /// </summary>
    /// <returns>The result of ShouldThrowExceptionWhenMachineIdExists.</returns>

    [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
    [Trait("Db","DB45")]
    public async Task ShouldThrowExceptionWhenMachineIdExists()
    {
        var mockMediator = Substitute.For<IMonitorRequestDispatcher>();

        var request = new CreateMachineMonitorRequest
        {
            MachineId = 100,
            Name = "Station 61",
            Location = "Trunk",
            MachineType = 2,
            EnableAppTraceability = 1,
            EnableBypassTraceability = 0
        };

        var logger = XUnitLogger.CreateLogger<CreateMachineMonitorRequestHandler>();
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CreateMachineTests));

        var machineRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(dbKey);
        var sut = new CreateMachineMonitorRequestHandler(machineRepo, logger);

        var result = await sut.ProcessAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("A machine with the same ID or name already exists.");
    }

    /// <summary>
    /// Executes ShouldThrowExceptionWhenMachineNameExists operation.
    /// </summary>
    /// <returns>The result of ShouldThrowExceptionWhenMachineNameExists.</returns>

    [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
    [Trait("Db","DB45")]
    public async Task ShouldThrowExceptionWhenMachineNameExists()
    {
        var mockMediator = Substitute.For<IMonitorRequestDispatcher>();

        var request = new CreateMachineMonitorRequest
        {
            MachineId = 100,
            Name = "WS100",
            Location = "Trunk",
            MachineType = 2,
            EnableAppTraceability = 1,
            EnableBypassTraceability = 0
        };

        var logger = XUnitLogger.CreateLogger<CreateMachineMonitorRequestHandler>();
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(CreateMachineTests));

        var machineRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(dbKey);
        var sut = new CreateMachineMonitorRequestHandler(machineRepo, logger);

        var result = await sut.ProcessAsync(request, TestContext.Current.CancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("A machine with the same ID or name already exists.");
    }

    /// <summary>
    /// Executes InitializeAsync operation.
    /// </summary>
    /// <returns>The result of InitializeAsync.</returns>

    private async Task InitializeAsync()
    {
        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        var scope = _services.CreateScope();
        var provider = scope.ServiceProvider;
        var request = new CreateMachineMonitorRequest
        {
            MachineId = 100,
            Name = "Station 60",
            Location = "Trunk",
            MachineType = 2,
            EnableAppTraceability = 1,
            EnableBypassTraceability = 0
        };

        var logger = XUnitLogger.CreateLogger<CreateMachineMonitorRequestHandler>();
        var machineRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(dbKey);
        var sut = new CreateMachineMonitorRequestHandler(machineRepo, logger);
        await sut.ProcessAsync(request, TestContext.Current.CancellationToken);
    }

    /// <summary>
    /// Executes DisposeAsync operation.
    /// </summary>
    /// <returns>The result of DisposeAsync.</returns>

    private Task DisposeAsync() => Task.CompletedTask;
}
