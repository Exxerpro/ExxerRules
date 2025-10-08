using IndTrace.Application.Plcs.Queries.GetDetail.DataLoaders;
using IndTrace.Application.Plcs.Queries.GetDetail.Assemblers;

namespace IndTrace.Aggregation.BoundedTests.PLCs.Queries;
/// <summary>
/// Represents the GetPlcDetailQueryHandlerTests.
/// </summary>

public class GetPlcDetailQueryHandlerTests : DependenciesFactory
{
    public GetPlcDetailQueryHandlerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    // Test
    /// <summary>
    /// Executes GetPlcDetail operation.
    /// </summary>
    /// <returns>The result of GetPlcDetail.</returns>

    [Fact]
    public async Task GetPlcDetail()
    {
        await Initialization;

        // Arrange
        var logger = XUnitLogger.CreateLogger<GetPlcDetailQueryHandler>();

        // Create real SRP services using repositories
        var dataLoader = new PlcDetailDataLoader(
            DpPlcRepository,
            DpMachinePlcRepository,
            DpMachineRepository,
            DpVariablesRepository,
            DpVariablesGroupRepository,
            XUnitLogger.CreateLogger<PlcDetailDataLoader>());

        var assembler = new PlcDetailAssembler(
            XUnitLogger.CreateLogger<PlcDetailAssembler>());

        var sut = new GetPlcDetailQueryHandler(dataLoader, assembler, logger);

        // Act
        var result = await sut.ProcessAsync(new GetPlcDetailQuery { Id = 100 }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<PlcDto>();
        result.Value.BrandOwner.ShouldBe("Siemens");
    }
}
