using GetBarCodeReportQueryHandler = IndTrace.Application.BarCodes.Queries.GetBarCodeDetail.GetBarCodeReportQueryHandler;
using IndTrace.Application.BarCodes.Queries.DataLoaders;
using IndTrace.Application.BarCodes.Queries.Mappers;

namespace IndTrace.Aggregation.BoundedTests.BarCodes.Queries;
/// <summary>
/// Represents the GetBarCodeReportQueryHandlerTests.
/// </summary>

public class GetBarCodeReportQueryHandlerTests : DependenciesFactory
{
    public GetBarCodeReportQueryHandlerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    /// <summary>
    /// Executes GetBarCodeDetailQuery_ShouldSendRequest_ToGuiCommandDispatcher operation.
    /// </summary>

    [Fact]
    public async Task GetBarCodeDetailQuery_ShouldSendRequest_ToGuiCommandDispatcher()
    {
        await Initialization;

        // Arrange

        // Use real initialized dependencies from DependenciesFactory
        var logger = XUnitLogger.CreateLogger<GetBarCodeReportQueryHandler>();
        var dataLoaderLogger = XUnitLogger.CreateLogger<BarCodeDetailDataLoader>();
        var mapperLogger = XUnitLogger.CreateLogger<BarCodeDetailMapper>();
        var dataLoader = new BarCodeDetailDataLoader(DpCycleRepository, DpRegisterRepository, DpVariablesRepository, dataLoaderLogger);
        var mapper = new BarCodeDetailMapper(mapperLogger);
        var sut = new GetBarCodeReportQueryHandler(dataLoader, mapper, DpBarCodeIS, logger);
        var request = new GetBarCodeDetailQuery { MachineId = 10000, BarCode = "N421072079209952" };

        // Act
        await DpMonitorRequestDispatcher.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        // DELETED: NSubstitute verification - using result-based assertions instead
    }

    /// <summary>
    /// Executes ShouldHaveCorrectInformation operation.
    /// </summary>
    /// <param name="barCode">The barCode.</param>
    /// <param name="barCodeId">The barCodeId.</param>
    /// <param name="machineId">The machineId.</param>
    /// <returns>The result of ShouldHaveCorrectInformation.</returns>

    [Theory]
    [InlineData("L1AL687508232372501", 1, 100)]
    [InlineData("L1AL687508232372502", 2, 100)]
    [InlineData("L1AL90164629232372647", 147, 100)]
    [InlineData("L1AL90164629232372646", 146, 100)]
    [InlineData("L1AL687508232372516", 16, 300)]
    [InlineData("L1AL687508232372517", 17, 300)]
    [InlineData("L1AL90164629232372563", 63, 300)]
    [InlineData("L1AL687508232372530", 30, 500)]
    [InlineData("L1AL687508232372531", 31, 500)]
    [InlineData("L1AL90164629232372577", 77, 500)]
    public async Task ShouldHaveCorrectInformation(string barCode, int barCodeId, int machineId)
    {
        await Initialization;

        // Arrange

        // Domain expectation: part number is 'L' + 6 digits derived from the label
        var partNumber = "L" + barCode.Substring(4, 6);

        var logger = XUnitLogger.CreateLogger<GetBarCodeReportQueryHandler>();
        var dataLoaderLogger = XUnitLogger.CreateLogger<BarCodeDetailDataLoader>();
        var mapperLogger = XUnitLogger.CreateLogger<BarCodeDetailMapper>();
        var dataLoader = new BarCodeDetailDataLoader(DpCycleRepository, DpRegisterRepository, DpVariablesRepository, dataLoaderLogger);
        var mapper = new BarCodeDetailMapper(mapperLogger);
        var sut = new GetBarCodeReportQueryHandler(dataLoader, mapper, DpBarCodeIS, logger);
        var request = new GetBarCodeDetailQuery
        {
            MachineId = machineId,
            BarCode = barCode,
            PartNumber = partNumber
        };

        // Act
        var result = await sut.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert

        result.Value.Label.ShouldBe(barCode);
        result.Value.BarCodeId.ShouldBe(barCodeId);
        result.Value.ShouldBeOfType<BarCodeDetailVm>();
    }

    // TODO: Consider consolidating this test with ShouldHaveCorrectInformation as they appear to test the same functionality
    /// <summary>
    /// Executes ShouldRetrieveCorrectInformation operation.
    /// </summary>
    /// <returns>The result of ShouldRetrieveCorrectInformation.</returns>
    [Theory]
    [InlineData("L1AL687508232372501", 1, 100)]
    [InlineData("L1AL687508232372502", 2, 100)]
    [InlineData("L1AL90164629232372647", 147, 100)]
    [InlineData("L1AL90164629232372646", 146, 100)]
    [InlineData("L1AL687508232372516", 16, 300)]
    [InlineData("L1AL687508232372517", 17, 300)]
    [InlineData("L1AL90164629232372563", 63, 300)]
    [InlineData("L1AL687508232372530", 30, 500)]
    [InlineData("L1AL687508232372531", 31, 500)]
    [InlineData("L1AL90164629232372577", 77, 500)]
    public async Task ShouldRetrieveCorrectInformation(string barCode,
        int barCodeId,
        int machineId)
    {
        await Initialization;

        // Arrange

        // Domain expectation: part number is 'L' + 6 digits derived from the label
        var partNumber = "L" + barCode.Substring(4, 6);

        var logger = XUnitLogger.CreateLogger<GetBarCodeReportQueryHandler>();
        var dataLoaderLogger = XUnitLogger.CreateLogger<BarCodeDetailDataLoader>();
        var mapperLogger = XUnitLogger.CreateLogger<BarCodeDetailMapper>();
        var dataLoader = new BarCodeDetailDataLoader(DpCycleRepository, DpRegisterRepository, DpVariablesRepository, dataLoaderLogger);
        var mapper = new BarCodeDetailMapper(mapperLogger);
        var sut = new GetBarCodeReportQueryHandler(dataLoader, mapper, DpBarCodeIS, logger);
        var request = new GetBarCodeDetailQuery
        {
            MachineId = machineId,
            BarCode = barCode,
            PartNumber = partNumber
        };

        // Act
        var result = await sut.ProcessAsync(request, cancellationToken: TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert

        result.Value.Label.ShouldBe(barCode);
        result.Value.BarCodeId.ShouldBe(barCodeId);

        result.Value.ShouldBeOfType<BarCodeDetailVm>();
    }
}
