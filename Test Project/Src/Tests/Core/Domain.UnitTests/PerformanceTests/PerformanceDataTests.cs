namespace IndTrace.Domain.UnitTests.PerformanceTests;
/// <summary>
/// Represents the PerformanceDataTests.
/// </summary>

public class PerformanceDataTests
{
    /// <summary>
    /// Executes FromPlc_WithValidRegisters_ShouldParseAllPropertiesCorrectly operation.
    /// </summary>
    [Fact]
    public void FromPlc_WithValidRegisters_ShouldParseAllPropertiesCorrectly()
    {
        // Arrange
        var registers = new Dictionary<string, Register>
        {
            ["ApplicationFlag"] = new() { Value = "1" },
            ["EventCounter"] = new() { Value = "123" },
            ["CurrentTime"] = new() { Value = "1000" },
            ["RunningTime"] = new() { Value = "800" },
            ["StoppedTime"] = new() { Value = "150" },
            ["FaultedTime"] = new() { Value = "50" },
            ["StatusFaultReason"] = new() { Value = "4" },
            ["TotalProduction"] = new() { Value = "500.5" },
            ["ProductionOk"] = new() { Value = "490.0" },
            ["ProductionNoK"] = new() { Value = "10.5" },
            ["StatusFaultReject"] = new() { Value = "2" },
            ["RejectEventCounter"] = new() { Value = "5" },
            ["StatusReject"] = new() { Value = "1" },
            ["RejectQuantityUnits"] = new() { Value = "2.5" },
            ["StandardCycleTime"] = new() { Value = "1.5" },
            ["ActualCycleTime"] = new() { Value = "1.6" },
            ["PlanedProductionTime"] = new() { Value = "950.0" }
        };

        // Act
        var performanceData = PerformanceData.FromPlc(registers);

        // Assert
        performanceData.ApplicationFlag.ShouldBe(1);
        performanceData.EventCounter.ShouldBe(123);
        performanceData.CurrentTime.ShouldBe(1000);
        performanceData.RunningTime.ShouldBe(800);
        performanceData.StoppedTime.ShouldBe(150);
        performanceData.FaultedTime.ShouldBe(50);
        performanceData.StatusFaultReason.ShouldBe(4);
        performanceData.TotalProduction.ShouldBe(500.5);
        performanceData.ProductionOk.ShouldBe(490.0);
        performanceData.ProductionNoK.ShouldBe(10.5);
        performanceData.StatusFaultReject.ShouldBe(2);
        performanceData.RejectEventCounter.ShouldBe(5);
        performanceData.StatusReject.ShouldBe(1);
        performanceData.RejectQuantityUnits.ShouldBe(2.5);
        performanceData.StandardCycleTime.ShouldBe(1.5);
        performanceData.ActualCycleTime.ShouldBe(1.6);
        performanceData.PlanedProductionTime.ShouldBe(950.0);
    }
    /// <summary>
    /// Executes FromPlc_WithMissingKeys_ShouldDefaultToZero operation.
    /// </summary>

    [Fact]
    public void FromPlc_WithMissingKeys_ShouldDefaultToZero()
    {
        // Arrange
        var registers = new Dictionary<string, Register>();

        // Act
        var performanceData = PerformanceData.FromPlc(registers);

        // Assert
        performanceData.ApplicationFlag.ShouldBe(0);
        performanceData.EventCounter.ShouldBe(0);
        performanceData.TotalProduction.ShouldBe(0.0);
        performanceData.StandardCycleTime.ShouldBe(0.0);
    }
    /// <summary>
    /// Executes FromPlc_WithInvalidData_ShouldDefaultToZero operation.
    /// </summary>

    [Fact]
    public void FromPlc_WithInvalidData_ShouldDefaultToZero()
    {
        // Arrange
        var registers = new Dictionary<string, Register>
        {
            ["ApplicationFlag"] = new() { Value = "abc" },
            ["TotalProduction"] = new() { Value = "xyz" },
            ["RunningTime"] = new() { Value = "1.2.3" }, // Invalid double
            ["EventCounter"] = null! // Null register object
        };

        // Act
        var performanceData = PerformanceData.FromPlc(registers);

        // Assert
        performanceData.ApplicationFlag.ShouldBe(0);
        performanceData.TotalProduction.ShouldBe(0.0);
        performanceData.RunningTime.ShouldBe(0);
        performanceData.EventCounter.ShouldBe(0);
    }
    /// <summary>
    /// Executes FromPlc_WithNullRegisterValue_ShouldDefaultToZero operation.
    /// </summary>

    [Fact]
    public void FromPlc_WithNullRegisterValue_ShouldDefaultToZero()
    {
        // Arrange
        var registers = new Dictionary<string, Register>
        {
            ["ApplicationFlag"] = new() { Value = null! }
        };

        // Act
        var performanceData = PerformanceData.FromPlc(registers);

        // Assert
        performanceData.ApplicationFlag.ShouldBe(0);
    }
}
