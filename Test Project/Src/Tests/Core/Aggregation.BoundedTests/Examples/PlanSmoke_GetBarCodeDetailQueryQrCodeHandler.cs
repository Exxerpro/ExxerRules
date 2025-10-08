namespace IndTrace.Aggregation.BoundedTests.PlanSmoke;

public class PlanSmoke_GetBarCodeDetailQueryQrCodeHandler
{
    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-QR")]
    public async Task Known_Label_WS100_QR()
    {
        // Input: label = L1AL687508232372501
        // Expect: Success; MachineId = 100; matches monitor handler behavior/logs
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-QR")]
    public async Task Known_Label_WS300_QR()
    {
        // Input: label = L1AL687508232372516
        // Expect: Success; MachineId = 300
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-QR")]
    public async Task Invalid_Label_QR()
    {
        // Input: label = Q
        // Expect: Failure("Label must be at least 3 characters")
        await Task.CompletedTask;
    }
}
