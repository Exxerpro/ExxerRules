namespace IndTrace.Aggregation.BoundedTests.WorkFlows.Commands;
/// <summary>
/// Represents the UpdateWorkFlowCommandTests.
/// </summary>

public class UpdateWorkFlowCommandTests : DependenciesFactory
{
    //[Fix]
    //CLAUDE
    //Date: 09/09/2025
    //Reason: [Constructor Pattern] - Added ITestContextAccessor parameter to match DependenciesFactory signature
    public UpdateWorkFlowCommandTests(ITestOutputHelper outputHelper) : base(outputHelper)
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
        // NO MOCKING: Use real DpMonitorRequestDispatcher for UI operations

        DpDateTimeMachine.SetDateTimeNow(new DateTimeOffset(2020, 06, 06, 06, 06, 06, 6, TimeSpan.Zero));
        DpDateTimeMachine.SetDateTimeNow(DateTime.Now);
        var dateTime = DpDateTimeMachine;

        //[Fix]
        //CLAUDE
        //Date: 09/09/2025
        //Reason: [Test Data Mismatch] - Updated to use existing WorkFlowId from test data
        var request = new UpdateWorkFlowCommand()
        {
            WorkFlowId = 1, // Use existing WorkFlowId that exists in test data
            ProductId = 508, // Existing ProductId
            NextMachineId = 200, // Existing MachineId
            LastMachineId = 100  // Existing MachineId
        };

        // Act - Use real dispatcher (no mocking)
        var result = await DpMonitorRequestDispatcher.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType<Result<WorkFlowDetailVm>>();
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

        // DELETED: Mock repository declaration
        var logger = XUnitLogger.CreateLogger<UpdateWorkFlowCommandHandler>();

        DpDateTimeMachine.SetDateTimeNow(new DateTimeOffset(2020, 06, 06, 06, 06, 06, 6, TimeSpan.Zero));
        DpDateTimeMachine.SetDateTimeNow(DateTime.Now);
        var dateTime = DpDateTimeMachine;

        //[Fix]
        //CLAUDE
        //Date: 09/09/2025
        //Reason: [Test Data Mismatch] - Updated to use existing WorkFlow data from test data
        var existingWorkFlow = new WorkFlow
        {
            WorkFlowId = 1,
            ProductId = 508,
            NextMachineId = 100,
            LastMachineId = 0
        };

        var request = new UpdateWorkFlowCommand()
        {
            WorkFlowId = 1,       // Use existing WorkFlowId
            ProductId = 508,      // Use existing ProductId
            NextMachineId = 300,  // Update to existing MachineId
            LastMachineId = 200   // Update to existing MachineId
        };

        // Setup repository mock responses

        var sut = new UpdateWorkFlowCommandHandler(DpWorkFlowRepository, logger);

        // Act
        var result = await sut.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert

        result.Value.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeOfType<WorkFlowDetailVm>();
        result.Value.WorkFlowId.ShouldBeEquivalentTo(request.WorkFlowId);
        result.Value.ProductId.ShouldBeEquivalentTo(request.ProductId);
        result.Value.NextMachineId.ShouldBeEquivalentTo(request.NextMachineId);
        result.Value.LastMachineId.ShouldBeEquivalentTo(request.LastMachineId);

        // Verify repository interactions
    }
}
