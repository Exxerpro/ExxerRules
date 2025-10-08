namespace IndTrace.Aggregation.BoundedTests.PlanSmoke;

public class PlanSmoke_GetMachineConfigQueryHandler
{
    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "Machine-Config")]
    public async Task Known_PartNumber_L687508()
    {
        // Input: partNumber = L687508
        // Expect: Success; VM with product/workflows; PLCs from Machines/PLCs data; counts logged
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "Machine-Config")]
    public async Task Invalid_PartNumber_TooShort()
    {
        // Input: partNumber = L (len < 3)
        // Expect: Failure(validation); no data loads
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "Machine-Config")]
    public async Task Non_Existent_PartNumber()
    {
        // Input: partNumber = ZZZ9999
        // Expect: Failure("Product not found")
        await Task.CompletedTask;
    }
}
