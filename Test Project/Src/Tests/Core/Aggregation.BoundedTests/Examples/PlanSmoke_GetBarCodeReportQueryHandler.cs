namespace IndTrace.Aggregation.BoundedTests.PlanSmoke;

public class PlanSmoke_GetBarCodeReportQueryHandler
{
    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-Report")]
    public async Task Found_Label_WS100()
    {
        // Input: label = L1AL687508232372501 (MachineId 100)
        // Expect: Success; VM shows MachineId = 100; counts >= 1; durations logged
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-Report")]
    public async Task Found_Label_WS300()
    {
        // Input: label = L1AL687508232372516 (MachineId 300)
        // Expect: Success; MachineId = 300; counts >= 1
        await Task.CompletedTask;
    }

    [Fact(Skip = "Pending refactor wiring")]
    [Trait("PlanSmoke", "BarCodes-Detail-Report")]
    public async Task Label_NotFound()
    {
        // Input: label = L1AL687508232372599
        // Expect: Failure("BarCode not found"); no data loads attempted
        await Task.CompletedTask;
    }
}
