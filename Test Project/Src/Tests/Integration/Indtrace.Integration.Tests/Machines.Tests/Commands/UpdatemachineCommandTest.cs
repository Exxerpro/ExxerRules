using IndTrace.Application.Machines.Commands.Enable;
using IndTrace.Application.Machines.Commands.Update;
using IndTrace.Application.Machines.Queries.GetMachinesList;

namespace Integration.Tests.Machines.Tests.Commands;
/// <summary>
/// Represents the UpdateMachineCommandTest.
/// </summary>

public class UpdateMachineCommandTest(Integration.Tests.Infrastructure.TestHostFixture fixture, ITestOutputHelper output) : IClassFixture<Integration.Tests.Infrastructure.TestHostFixture>
{
    private readonly IServiceProvider _services = fixture.Services;
    private readonly ITestOutputHelper _output = output;
    public static bool ShouldSkipDb45 => TestDbGuards.ShouldSkipDb45;

    /// <summary>
    /// Executes ShouldSendRequestAsync operation.
    /// </summary>
    /// <returns>The result of ShouldSendRequestAsync.</returns>
    [Fact]
    public async Task ShouldSendRequestAsync()
    {
        // Arrange
        var mockMediator = Substitute.For<IMonitorRequestDispatcher>();

        var dateTimeMock = Substitute.For<IDateTimeMachine>();
        dateTimeMock.Now.Returns(new DateTime(2020, 06, 06, 06, 06, 06, 6));
        var dateTime = dateTimeMock;

        var response = new MachineDto();

        var request = new ToggleEnableMachineCommand()
        {
            MachineId = 20,
            Enable = true,
        };

        mockMediator.ProcessAsync(request, Arg.Any<CancellationToken>()).Returns(new MachineDto());

        // Act
        var result = await mockMediator.ProcessAsync(request, TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert
        result.Value.ShouldBeOfType<MachineDto>();
        await mockMediator.Received(1).ProcessAsync(Arg.Any<ToggleEnableMachineCommand>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes ShouldExecuteRequestHandler operation.
    /// </summary>
    /// <returns>The result of ShouldExecuteRequestHandler.</returns>

    [Fact(Skip = "Missing DB or SKIP_DB_TESTS set.", SkipWhen = nameof(ShouldSkipDb45))]
    [Trait("Db", "DB45")]
    public async Task ShouldExecuteRequestHandler()
    {
        // Arrange

        var dateTimeMock = Substitute.For<IDateTimeMachine>();
        dateTimeMock.Now.Returns(new DateTime(2020, 06, 06, 06, 06, 06, 6));
        var dateTime = dateTimeMock;

        var request = new MachineUpdateCommand()
        {
            MachineId = 100,
            Name = "WS100", // Provide proper name to avoid OR logic issues
        };

        const string dbKey = Integration.Tests.Utilities.DbProfiles.IndTraceDbContext45;
        using var scope = _services.CreateScope();
        DbLogging.LogConnectionString(scope.ServiceProvider, dbKey, _output, nameof(UpdateMachineCommandTest));

        var dispatcher = scope.ServiceProvider.GetRequiredService<IMonitorRequestDispatcher>();
        var repo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<TaskGatewayRequest>>(dbKey);
        var cycleRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Cycle>>(dbKey);
        var machineRepo = scope.ServiceProvider.GetRequiredKeyedService<IRepository<Machine>>(dbKey);

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MachineUpdateCommandHandler>>();

        var sut = new MachineUpdateCommandHandler(machineRepo, logger, dispatcher);
        var result = await sut.ProcessAsync(request, TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert
        result.Value.ShouldBeOfType<MachineDto>();
        result.Value.MachineId.ShouldBe(request.MachineId);

        MachineType.InitialPrinter.ShouldBeEquivalentTo(result.Value.MachineType);
        WorkFlowType.Initial.ShouldBeEquivalentTo(result.Value.WorkFlowType);
    }
}
