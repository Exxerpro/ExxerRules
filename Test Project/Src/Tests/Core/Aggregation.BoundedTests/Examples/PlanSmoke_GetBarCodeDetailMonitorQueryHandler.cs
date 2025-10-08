namespace IndTrace.Aggregation.BoundedTests.PlanSmoke;

public class PlanSmoke_GetBarCodeDetailMonitorQueryHandler
{
    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-Monitor")]
    public async Task Known_Label_WS100()
    {
        // Input: label = L1AL687508232372501
        // Expect: Success; MachineId = 100; updater applied; counts >= 1; durations logged
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-Monitor")]
    public async Task Known_Label_WS300()
    {
        // Input: label = L1AL687508232372516
        // Expect: Success; MachineId = 300
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-Monitor")]
    public async Task Label_NotFound()
    {
        // Input: label = NOT_EXIST_123
        // Expect: Failure("BarCode not found")
        await Task.CompletedTask;
    }
}
