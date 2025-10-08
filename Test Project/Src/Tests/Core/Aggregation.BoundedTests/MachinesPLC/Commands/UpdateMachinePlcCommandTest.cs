namespace IndTrace.Aggregation.BoundedTests.MachinesPLC.Commands;
/// <summary>
/// Represents the UpdateMachinePlcCommandTest.
/// </summary>

public class UpdateMachinePlcCommandTest : DependenciesFactory
{
    public UpdateMachinePlcCommandTest(ITestOutputHelper outputHelper) : base(outputHelper)
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

        var request = new UpdateMachinePlcCommand()
        {
            MachineId = 100,
            PlcId = 100
        };

        // Act
        var result = await dispatcher.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
    }
}
