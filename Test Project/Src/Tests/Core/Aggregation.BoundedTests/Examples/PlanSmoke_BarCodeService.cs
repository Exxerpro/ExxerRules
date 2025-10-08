namespace IndTrace.Aggregation.BoundedTests.PlanSmoke;

public class PlanSmoke_BarCodeService
{
    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCode-Service")]
    public async Task List_By_Product_508()
    {
        // Input: productId = 508
        // Expect: Success; items >= 1; durations logged
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCode-Service")]
    public async Task Consecutive_Next_From_Last()
    {
        // Input: last label from highest L1AL6875082323725xx
        // Expect: Success; next calculation according to policy; no exceptions on gaps
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCode-Service")]
    public async Task Invalid_Input_Label()
    {
        // Input: empty label
        // Expect: Failure("Label must be at least 3 characters")
        await Task.CompletedTask;
    }
}
