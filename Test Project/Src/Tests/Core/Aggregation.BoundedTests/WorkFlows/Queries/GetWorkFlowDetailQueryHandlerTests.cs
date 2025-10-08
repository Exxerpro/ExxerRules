namespace IndTrace.Aggregation.BoundedTests.WorkFlows.Queries;
/// <summary>
/// Represents the GetWorkFlowDetailQueryHandlerTests.
/// </summary>

public class GetWorkFlowDetailQueryHandlerTests : DependenciesFactory
{
    public GetWorkFlowDetailQueryHandlerTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    // Test
    /// <summary>
    /// Executes GetWorkFlowDetail operation.
    /// </summary>
    /// <param name="partNumber">The partNumber.</param>
    /// <param name="rowCount">The rowCount.</param>
    /// <returns>The result of GetWorkFlowDetail.</returns>

    [Theory]
    [InlineData("L90164629", 4)]
    [InlineData("L687508", 5)]
    public async Task GetWorkFlowDetail(string partNumber, int rowCount)
    {
        await Initialization;

        // Arrange

        var productRepository = DpProductRepository;
        var workFlowRepository = DpWorkFlowRepository;
        var logger = XUnitLogger.CreateLogger<GetWorkFlowDetailQueryHandler>();

        var sut = new GetWorkFlowDetailQueryHandler(productRepository, workFlowRepository, logger);

        // Act
        var result = await sut.ProcessAsync(new GetWorkFlowDetailQuery { NoParte = partNumber }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<List<WorkFlowDetailVm>>();
        result.Value.Count.ShouldBe(rowCount);
    }
}
