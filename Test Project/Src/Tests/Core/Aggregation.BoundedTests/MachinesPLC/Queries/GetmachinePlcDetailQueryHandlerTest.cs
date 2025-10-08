namespace IndTrace.Aggregation.BoundedTests.MachinesPLC.Queries;
/// <summary>
/// Represents the GetMachinePlcDetailQueryHandlerTest.
/// </summary>

public class GetMachinePlcDetailQueryHandlerTest : DependenciesFactory
{
    public GetMachinePlcDetailQueryHandlerTest(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    // Test
    /// <summary>
    /// Executes GetMachinePlcDetailTest operation.
    /// </summary>
    /// <returns>The result of GetMachinePlcDetailTest.</returns>

    [Fact]
    public async Task GetMachinePlcDetailTest()
    {
        await Initialization;

        // DELETED: Mock repository declaration
        var logger = XUnitLogger.CreateLogger<GetMachinePlcDetailQueryHandler>();

        var sut = new GetMachinePlcDetailQueryHandler(DpMachinePlcRepository, logger);

        var result = await sut.ProcessAsync(new GetMachinePlcDetailQuery { MachineId = 100, PlcId = 100 }, cancellationToken: TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        result.Value.ShouldBeOfType<MachinePlcDetailVm>();
        result.Value.PlcId.ShouldBe(100);
    }
}
