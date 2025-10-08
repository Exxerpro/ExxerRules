namespace IndTrace.Aggregation.BoundedTests.PlanSmoke;

public class PlanSmoke_BarCodeResultService
{
    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCode-Result-Service")]
    public async Task Valid_Label_Success()
    {
        // Input: label = L1AL687508232372501
        // Expect: Success; state.BarCodeId = 1; state.Label equals input
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCode-Result-Service")]
    public async Task Invalid_Label_TooShort()
    {
        // Input: label = Z
        // Expect: Failure("Label must be at least 3 characters")
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCode-Result-Service")]
    public async Task Not_Found_Label()
    {
        // Input: label = NOT_EXIST_123
        // Expect: Failure("BarCode not found")
        await Task.CompletedTask;
    }
}
