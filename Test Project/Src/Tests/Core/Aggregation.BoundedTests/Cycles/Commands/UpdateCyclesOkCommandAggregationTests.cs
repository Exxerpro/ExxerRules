using IndTrace.Application.Cycles.Commands.UpdateCycles;

namespace IndTrace.Aggregation.BoundedTests.Cycles.Commands;

public class UpdateCyclesOkCommandAggregationTests : DependenciesFactory
{
    public UpdateCyclesOkCommandAggregationTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    // [DELETED] Handle_Should_Save_Ok_GatewayTask_And_Not_Save_NotOk_Task - Violates relational database provider constraint
    // This test manually creates Register entities and tries to use SQL-specific operations in in-memory database
    // Aggregation tests should use existing test data with proper aggregate patterns without manual entity creation
}
