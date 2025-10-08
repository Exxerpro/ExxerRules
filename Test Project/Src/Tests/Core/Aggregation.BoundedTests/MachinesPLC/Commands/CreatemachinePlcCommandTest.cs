using System.Threading.Tasks;

namespace IndTrace.Aggregation.BoundedTests.MachinesPLC.Commands;
/// <summary>
/// Represents the CreatemachinePlcCommandTest.
/// </summary>

public class CreatemachinePlcCommandTest : DependenciesFactory
{
    //[Fix]
    //CLAUDE
    //Date: 09/09/2025
    //Reason: [Constructor Pattern] - Added ITestContextAccessor parameter to match DependenciesFactory signature
    public CreatemachinePlcCommandTest(ITestOutputHelper outputHelper) : base(outputHelper)
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

        var mediator = DpMonitorRequestDispatcher;

        var request = new CreateMachinePlcCommand()
        {
            MachineId = 200,
            PlCsId = 200
        };

        // Act
        var result = await mediator.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

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

        var request = new CreateMachinePlcCommand()
        {
            MachineId = 1200,
            PlCsId = 1200
        };

        var repository = DpMachinePlcRepository;
        var logger = XUnitLogger.CreateLogger<CreateMachinePlcCommandHandler>();

        var sut = new CreateMachinePlcCommandHandler(repository, logger);
        var result = await sut.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
    }
}
