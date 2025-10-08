namespace IndTrace.Aggregation.BoundedTests.Settings.Queries;
/// <summary>
/// Represents the GetSettingsListQueryHandlerTests.
/// </summary>

public class GetSettingsListQueryHandlerTests : DependenciesFactory
{
    public GetSettingsListQueryHandlerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    /// <summary>
    /// Executes GetSettingsTest operation.
    /// </summary>
    /// <returns>The result of GetSettingsTest.</returns>

    [Fact]
    public async Task GetSettingsTest()
    {
        await Initialization;

        var logger = XUnitLogger.CreateLogger<GetSettingsListQueryHandler>();

        var sut = new GetSettingsListQueryHandler(DpSettingRepository, logger);

        var result = await sut.ProcessAsync(new GetSettingsListQuery(), TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        result.Value.ShouldBeOfType<SettingsListVm>();
        result.Value.Settings.Count.ShouldBe(1);
    }
}
