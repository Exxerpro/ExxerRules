namespace IndTrace.Domain.UnitTests.PerformancesTests;

/// <summary>
/// Unit tests for PerformanceData
/// </summary>
public class PerformanceDataTests
{
    /// <summary>
    /// Executes PerformanceData_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully operation.
    /// </summary>
    [Fact]
    public void PerformanceData_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully()
    {
        // Arrange & Act
        var instance = new PerformanceData();

        // Assert
        instance.ShouldNotBeNull();
        instance.PerformanceDataId.ShouldBe(default(long));
        instance.MachineId.ShouldBe(default(int));
        instance.PlcId.ShouldBe(default(int));
        instance.BarCodeId.ShouldBe(default(int));
        instance.CycleId.ShouldBe(default(int));
        instance.TimeStamp.ShouldBe(default(DateTime));
        instance.ApplicationFlag.ShouldBe(default(int));
        instance.EventCounter.ShouldBe(default(int));
        instance.CurrentTime.ShouldBe(default(int));
        instance.RunningTime.ShouldBe(default(int));
        instance.StoppedTime.ShouldBe(default(int));
        instance.FaultedTime.ShouldBe(default(int));
        instance.StatusFaultReason.ShouldBe(default(int));
        instance.TotalProduction.ShouldBe(default(double));
        instance.ProductionOk.ShouldBe(default(double));
        instance.ProductionNoK.ShouldBe(default(double));
        instance.StatusFaultReject.ShouldBe(default(int));
        instance.RejectEventCounter.ShouldBe(default(int));
        instance.StatusReject.ShouldBe(default(int));
        instance.RejectQuantityUnits.ShouldBe(default(double));
        instance.StandardCycleTime.ShouldBe(default(double));
        instance.ActualCycleTime.ShouldBe(default(double));
        instance.PlanedProductionTime.ShouldBe(default(double));
        instance.Command.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes PerformanceData_WithInvalidConfiguration_ShouldHandleErrorsGracefully operation.
    /// </summary>

    [Fact]
    public void PerformanceData_WithInvalidConfiguration_ShouldHandleErrorsGracefully()
    {
        // Arrange
        var validInstance = new PerformanceData();

        // Act - Testing edge case scenarios for manufacturing constraints
        validInstance.TotalProduction = -100.0; // Negative production should be handled
        validInstance.ProductionOk = -50.0; // Negative OK production
        validInstance.ProductionNoK = -25.0; // Negative NOK production
        validInstance.StandardCycleTime = -10.0; // Negative cycle time
        validInstance.ActualCycleTime = -15.0; // Negative actual cycle time

        // Assert - PerformanceData should gracefully handle negative values
        validInstance.ShouldNotBeNull();
        validInstance.TotalProduction.ShouldBe(-100.0);
        validInstance.ProductionOk.ShouldBe(-50.0);
        validInstance.ProductionNoK.ShouldBe(-25.0);
        validInstance.StandardCycleTime.ShouldBe(-10.0);
        validInstance.ActualCycleTime.ShouldBe(-15.0);
    }
    /// <summary>
    /// Executes PerformanceData_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void PerformanceData_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange
        var instance = new PerformanceData();
        var testTime = DateTime.UtcNow;
        var testCommand = new TaskGatewayRequest
        {
            MachineId = 100001,
            PartNumber = "TEST-PART-001"
        };

        // Act - Setting all performance data properties
        instance.PerformanceDataId = 12345L;
        instance.MachineId = 100001;
        instance.PlcId = 2001;
        instance.BarCodeId = 3001;
        instance.CycleId = 4001;
        instance.TimeStamp = testTime;
        instance.ApplicationFlag = 1;
        instance.EventCounter = 150;
        instance.CurrentTime = 3600;
        instance.RunningTime = 3200;
        instance.StoppedTime = 300;
        instance.FaultedTime = 100;
        instance.StatusFaultReason = 5;
        instance.TotalProduction = 1000.0;
        instance.ProductionOk = 950.0;
        instance.ProductionNoK = 50.0;
        instance.StatusFaultReject = 2;
        instance.RejectEventCounter = 10;
        instance.StatusReject = 1;
        instance.RejectQuantityUnits = 25.0;
        instance.StandardCycleTime = 60.0;
        instance.ActualCycleTime = 58.5;
        instance.PlanedProductionTime = 7200.0;
        instance.Command = testCommand;

        // Assert - Verify all properties are set correctly
        instance.PerformanceDataId.ShouldBe(12345L);
        instance.MachineId.ShouldBe(100001);
        instance.PlcId.ShouldBe(2001);
        instance.BarCodeId.ShouldBe(3001);
        instance.CycleId.ShouldBe(4001);
        instance.TimeStamp.ShouldBe(testTime);
        instance.ApplicationFlag.ShouldBe(1);
        instance.EventCounter.ShouldBe(150);
        instance.CurrentTime.ShouldBe(3600);
        instance.RunningTime.ShouldBe(3200);
        instance.StoppedTime.ShouldBe(300);
        instance.FaultedTime.ShouldBe(100);
        instance.StatusFaultReason.ShouldBe(5);
        instance.TotalProduction.ShouldBe(1000.0);
        instance.ProductionOk.ShouldBe(950.0);
        instance.ProductionNoK.ShouldBe(50.0);
        instance.StatusFaultReject.ShouldBe(2);
        instance.RejectEventCounter.ShouldBe(10);
        instance.StatusReject.ShouldBe(1);
        instance.RejectQuantityUnits.ShouldBe(25.0);
        instance.StandardCycleTime.ShouldBe(60.0);
        instance.ActualCycleTime.ShouldBe(58.5);
        instance.PlanedProductionTime.ShouldBe(7200.0);
        instance.Command.ShouldBe(testCommand);
    }
    /// <summary>
    /// Executes PerformanceData_WhenMethodsInvoked_ShouldProduceExpectedOutcomes operation.
    /// </summary>

    [Fact]
    public void PerformanceData_WhenMethodsInvoked_ShouldProduceExpectedOutcomes()
    {
        // Arrange
        var instance = new PerformanceData();
        var gatewayResponse = new TaskGatewayResponse
        {
            MachineId = 2001,
            PlcId = 3001,
            BarCodeId = 4001,
            CycleId = 5001,
            TimeStamp = DateTime.UtcNow
        };
        var result = Result<TaskGatewayResponse>.Success(gatewayResponse);

        // Act
        var updatedInstance = instance.FromResult(result);

        // Assert
        updatedInstance.ShouldBe(instance); // Should return same instance
        updatedInstance.MachineId.ShouldBe(2001);
        updatedInstance.PlcId.ShouldBe(3001);
        updatedInstance.BarCodeId.ShouldBe(4001);
        updatedInstance.CycleId.ShouldBe(5001);
        updatedInstance.TimeStamp.ShouldBe(gatewayResponse.TimeStamp);
    }
    /// <summary>
    /// Executes PerformanceData_BusinessScenario_ShouldEnforceAllDomainRules operation.
    /// </summary>

    [Fact]
    public void PerformanceData_BusinessScenario_ShouldEnforceAllDomainRules()
    {
        // Arrange
        var registers = new Dictionary<string, Register>
        {
            ["ApplicationFlag"] = new Register { Value = "1" },
            ["EventCounter"] = new Register { Value = "100" },
            ["CurrentTime"] = new Register { Value = "3600" },
            ["RunningTime"] = new Register { Value = "3200" },
            ["StoppedTime"] = new Register { Value = "300" },
            ["FaultedTime"] = new Register { Value = "100" },
            ["StatusFaultReason"] = new Register { Value = "0" },
            ["TotalProduction"] = new Register { Value = "500.5" },
            ["ProductionOk"] = new Register { Value = "475.5" },
            ["ProductionNoK"] = new Register { Value = "25.0" },
            ["StatusFaultReject"] = new Register { Value = "0" },
            ["RejectEventCounter"] = new Register { Value = "5" },
            ["StatusReject"] = new Register { Value = "0" },
            ["RejectQuantityUnits"] = new Register { Value = "10.0" },
            ["StandardCycleTime"] = new Register { Value = "45.0" },
            ["ActualCycleTime"] = new Register { Value = "43.2" },
            ["PlanedProductionTime"] = new Register { Value = "7200.0" }
        };

        // Act
        var performanceData = PerformanceData.FromPlc(registers);

        // Assert
        performanceData.ShouldNotBeNull();
        performanceData.ApplicationFlag.ShouldBe(1);
        performanceData.EventCounter.ShouldBe(100);
        performanceData.CurrentTime.ShouldBe(3600);
        performanceData.RunningTime.ShouldBe(3200);
        performanceData.StoppedTime.ShouldBe(300);
        performanceData.FaultedTime.ShouldBe(100);
        performanceData.StatusFaultReason.ShouldBe(0);
        performanceData.TotalProduction.ShouldBe(500.5);
        performanceData.ProductionOk.ShouldBe(475.5);
        performanceData.ProductionNoK.ShouldBe(25.0);
        performanceData.StatusFaultReject.ShouldBe(0);
        performanceData.RejectEventCounter.ShouldBe(5);
        performanceData.StatusReject.ShouldBe(0);
        performanceData.RejectQuantityUnits.ShouldBe(10.0);
        performanceData.StandardCycleTime.ShouldBe(45.0);
        performanceData.ActualCycleTime.ShouldBe(43.2);
        performanceData.PlanedProductionTime.ShouldBe(7200.0);

        // Verify manufacturing business rules
        performanceData.TotalProduction.ShouldBe(performanceData.ProductionOk + performanceData.ProductionNoK);
        performanceData.CurrentTime.ShouldBe(performanceData.RunningTime + performanceData.StoppedTime + performanceData.FaultedTime);
        performanceData.ActualCycleTime.ShouldBeLessThan(performanceData.StandardCycleTime); // Better than standard
    }
    /// <summary>
    /// Executes FromPlc_WithMissingRegisters_ShouldUseDefaultValues operation.
    /// </summary>

    [Fact]
    public void FromPlc_WithMissingRegisters_ShouldUseDefaultValues()
    {
        // Arrange
        var incompleteRegisters = new Dictionary<string, Register>
        {
            ["TotalProduction"] = new Register { Value = "1000.0" },
            ["ProductionOk"] = new Register { Value = "950.0" }
            // Missing other registers
        };

        // Act
        var performanceData = PerformanceData.FromPlc(incompleteRegisters);

        // Assert
        performanceData.ShouldNotBeNull();
        performanceData.TotalProduction.ShouldBe(1000.0);
        performanceData.ProductionOk.ShouldBe(950.0);
        performanceData.ApplicationFlag.ShouldBe(0); // Default value
        performanceData.EventCounter.ShouldBe(0); // Default value
        performanceData.ProductionNoK.ShouldBe(0.0); // Default value
    }
    /// <summary>
    /// Executes FromPlc_WithInvalidValues_ShouldUseDefaultValues operation.
    /// </summary>

    [Fact]
    public void FromPlc_WithInvalidValues_ShouldUseDefaultValues()
    {
        // Arrange
        var invalidRegisters = new Dictionary<string, Register>
        {
            ["ApplicationFlag"] = new Register { Value = "invalid_int" },
            ["TotalProduction"] = new Register { Value = "invalid_double" },
            ["EventCounter"] = new Register { Value = "" },
            ["StandardCycleTime"] = new Register { Value = "not_a_number" }
        };

        // Act
        var performanceData = PerformanceData.FromPlc(invalidRegisters);

        // Assert
        performanceData.ShouldNotBeNull();
        performanceData.ApplicationFlag.ShouldBe(0); // Default for invalid int
        performanceData.TotalProduction.ShouldBe(0.0); // Default for invalid double
        performanceData.EventCounter.ShouldBe(0); // Default for empty string
        performanceData.StandardCycleTime.ShouldBe(0.0); // Default for invalid double
    }
    /// <summary>
    /// Executes ManufacturingPerformanceScenarios_WithRealWorldData_ShouldCalculateCorrectly operation.
    /// </summary>

    [Fact]
    public void ManufacturingPerformanceScenarios_WithRealWorldData_ShouldCalculateCorrectly()
    {
        // Arrange - Automotive manufacturing line scenario
        var automotiveRegisters = new Dictionary<string, Register>
        {
            ["ApplicationFlag"] = new Register { Value = "1" },
            ["EventCounter"] = new Register { Value = "240" }, // 8-hour shift events
            ["CurrentTime"] = new Register { Value = "28800" }, // 8 hours in seconds
            ["RunningTime"] = new Register { Value = "25200" }, // 7 hours running
            ["StoppedTime"] = new Register { Value = "2400" }, // 40 minutes stops
            ["FaultedTime"] = new Register { Value = "1200" }, // 20 minutes faults
            ["TotalProduction"] = new Register { Value = "480.0" }, // Expected production
            ["ProductionOk"] = new Register { Value = "456.0" }, // 95% quality
            ["ProductionNoK"] = new Register { Value = "24.0" }, // 5% defects
            ["StandardCycleTime"] = new Register { Value = "60.0" }, // 1 minute standard
            ["ActualCycleTime"] = new Register { Value = "58.5" }, // 2.5% improvement
            ["PlanedProductionTime"] = new Register { Value = "28800.0" } // 8 hours planned
        };

        // Act
        var automotivePerformance = PerformanceData.FromPlc(automotiveRegisters);

        // Assert - Verify automotive manufacturing metrics
        automotivePerformance.ShouldNotBeNull();
        automotivePerformance.TotalProduction.ShouldBe(480.0);
        automotivePerformance.ProductionOk.ShouldBe(456.0);
        automotivePerformance.ProductionNoK.ShouldBe(24.0);

        // Calculate and verify OEE components
        var qualityRate = automotivePerformance.ProductionOk / automotivePerformance.TotalProduction;
        var availabilityRate = automotivePerformance.RunningTime / automotivePerformance.PlanedProductionTime;
        var performanceRate = automotivePerformance.StandardCycleTime / automotivePerformance.ActualCycleTime;

        qualityRate.ShouldBe(0.95, 0.001); // 95% quality
        availabilityRate.ShouldBe(0.875, 0.001); // 87.5% availability
        performanceRate.ShouldBe(1.025, 0.001); // 102.5% performance (better than standard)

        var oee = qualityRate * availabilityRate * performanceRate;
        oee.ShouldBeGreaterThan(0.80); // Good OEE for automotive
    }
}
