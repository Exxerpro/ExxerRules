namespace IndTrace.Aggregation.BoundedTests.Machines.Commands;
/// <summary>
/// Represents the UpdateMachineCommandTest.
/// </summary>

public class UpdateMachineCommandTest : DependenciesFactory
{
    public UpdateMachineCommandTest(ITestOutputHelper outputHelper)
        : base(outputHelper)
    {
    }

    /// <summary>
    /// Executes ShouldSendRequestAsync operation.
    /// </summary>
    /// <returns>The result of ShouldSendRequestAsync.</returns>

    [Fact]
    public async Task ShouldSendRequestAsync()
    {
        await Initialization;

        // Arrange

        var dispatcher = DpMonitorRequestDispatcher;

        // Set deterministic time for test
        DpDateTimeMachine.SetDateTimeNow(new DateTimeOffset(2020, 06, 06, 06, 06, 06, 6, TimeSpan.Zero));

        var request = new ToggleEnableMachineCommand()
        {
            MachineId = 100, // Fixed: Use existing machine ID from test data
            Enable = true,
        };

        // Act
        var result = await dispatcher.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes ShouldExecuteRequestHandler operation.
    /// </summary>
    /// <returns>The result of ShouldExecuteRequestHandler.</returns>

    [Fact]
    public async Task ShouldExecuteRequestHandler()
    {
        await Initialization;

        // Arrange

        // NO MOCKING: Use real DependenciesFactory members
        var logger = XUnitLogger.CreateLogger<MachineUpdateCommandHandler>();

        var request = new MachineUpdateCommand()
        {
            MachineId = 300, // Using Machine 300 which exists in test data with MachineType.Process
            Name = "Updated WS300",
            Location = "Updated Location"
        };

        var sut = new MachineUpdateCommandHandler(DpMachineRepository, logger, DpMonitorRequestDispatcher);
        var result = await sut.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<MachineDto>();
        result.Value.MachineId.ShouldBe(request.MachineId);

        // Verify the machine was actually updated
        result.Value.Name.ShouldBe(request.Name);
        result.Value.Location.ShouldBe(request.Location);

        // Verify enum types are preserved from test data
        MachineType.Process.ShouldBeEquivalentTo(result.Value.MachineType);
        WorkFlowType.Serial.ShouldBeEquivalentTo(result.Value.WorkFlowType);
    }
}
