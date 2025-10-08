namespace IndTrace.Aggregation.BoundedTests.Settings.Queries;
/// <summary>
/// Represents the GetSettingDetailQueryHandlerTests.
/// </summary>

public class GetSettingDetailQueryHandlerTests : DependenciesFactory
{
    public GetSettingDetailQueryHandlerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    /// <summary>
    /// Executes GetSettingDetail operation.
    /// </summary>
    /// <returns>The result of GetSettingDetail.</returns>

    [Fact]
    public async Task GetSettingDetail()
    {
        await Initialization;

        var DpCycleRepository = DpSettingRepository;
        var logger = XUnitLogger.CreateLogger<GetSettingDetailQueryHandler>();

        var sut = new GetSettingDetailQueryHandler(DpCycleRepository, logger);

        var result = await sut.ProcessAsync(new GetSettingDetailQuery { SettingId = 1 }, cancellationToken: TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        result.Value.ShouldBeOfType<SettingDetailVm>();
        result.Value.SettingId.ShouldBe(1);
    }
}
