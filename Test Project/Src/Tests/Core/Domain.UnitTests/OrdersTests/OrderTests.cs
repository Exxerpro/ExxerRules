namespace IndTrace.Domain.UnitTests.OrdersTests;

/// <summary>
/// Unit tests for Order
/// </summary>
public class OrderTests
{
    /// <summary>
    /// Executes Order_Order_WithDefaultConstructor_ShouldInitializeAllPropertiesToDefaultValues operation.
    /// </summary>
    [Fact]
    public void Order_Order_WithDefaultConstructor_ShouldInitializeAllPropertiesToDefaultValues()
    {
        // Arrange & Act
        var instance = new Order();

        // Assert
        instance.ShouldNotBeNull();
        instance.OrderId.ShouldBe(default(int));
        instance.MachineId.ShouldBe(default(int));
        instance.OperatorId.ShouldBe(default(int));
        instance.LeaderId.ShouldBe(default(int));
        instance.ProgrammerId.ShouldBe(default(int));
        instance.ProductId.ShouldBe(default(int));
        instance.ToolingId.ShouldBe(default(int));
        instance.TimeStamp.ShouldBe(default(int));
        instance.OrderSize.ShouldBe(default(int));
        instance.OrderTime.ShouldBe(default(int));
        instance.OrderStart.ShouldBe(default(DateTime));
        instance.OrderEnd.ShouldBe(default(DateTime));
        instance.ResultsId.ShouldBe(default(int));
    }

    /// <summary>
    /// Executes Order_WithNegativeValues_ShouldAcceptNegativeValuesForFlexibility operation.
    /// </summary>

    [Fact]
    public void Order_WithNegativeValues_ShouldAcceptNegativeValuesForFlexibility()
    {
        // Arrange
        var validOrder = new Order();

        // Act - Testing edge case scenarios for manufacturing order constraints
        validOrder.OrderSize = -100; // Negative order size should be handled
        validOrder.OrderTime = -3600; // Negative order time
        validOrder.MachineId = -1; // Invalid machine ID
        validOrder.OperatorId = -1; // Invalid operator ID
        validOrder.ProductId = -1; // Invalid product ID

        // Assert - Order should gracefully handle negative/invalid values
        validOrder.ShouldNotBeNull();
        validOrder.OrderSize.ShouldBe(-100);
        validOrder.OrderTime.ShouldBe(-3600);
        validOrder.MachineId.ShouldBe(-1);
        validOrder.OperatorId.ShouldBe(-1);
        validOrder.ProductId.ShouldBe(-1);
    }

    /// <summary>
    /// Executes Order_WhenAllManufacturingPropertiesSet_ShouldRetainAllAssignedValues operation.
    /// </summary>

    [Fact]
    public void Order_WhenAllManufacturingPropertiesSet_ShouldRetainAllAssignedValues()
    {
        // Arrange
        var instance = new Order();
        var orderStart = DateTime.UtcNow;
        var orderEnd = orderStart.AddHours(8);

        // Act - Setting all order properties for manufacturing scenario
        instance.OrderId = 12345;
        instance.MachineId = 100001;
        instance.OperatorId = 2001;
        instance.LeaderId = 3001;
        instance.ProgrammerId = 4001;
        instance.ProductId = 5001;
        instance.ToolingId = 6001;
        instance.TimeStamp = 1640995200; // Unix timestamp
        instance.OrderSize = 1000;
        instance.OrderTime = 28800; // 8 hours in seconds
        instance.OrderStart = orderStart;
        instance.OrderEnd = orderEnd;
        instance.ResultsId = 7001;

        // Assert - Verify all properties are set correctly
        instance.OrderId.ShouldBe(12345);
        instance.MachineId.ShouldBe(100001);
        instance.OperatorId.ShouldBe(2001);
        instance.LeaderId.ShouldBe(3001);
        instance.ProgrammerId.ShouldBe(4001);
        instance.ProductId.ShouldBe(5001);
        instance.ToolingId.ShouldBe(6001);
        instance.TimeStamp.ShouldBe(1640995200);
        instance.OrderSize.ShouldBe(1000);
        instance.OrderTime.ShouldBe(28800);
        instance.OrderStart.ShouldBe(orderStart);
        instance.OrderEnd.ShouldBe(orderEnd);
        instance.ResultsId.ShouldBe(7001);
    }

    /// <summary>
    /// Executes Order_WhenCalculatingProductionMetrics_ShouldComputeCorrectProductionRate operation.
    /// </summary>

    [Fact]
    public void Order_WhenCalculatingProductionMetrics_ShouldComputeCorrectProductionRate()
    {
        // Arrange
        var instance = new Order();
        var startTime = DateTime.UtcNow;
        var endTime = startTime.AddHours(4);

        instance.OrderStart = startTime;
        instance.OrderEnd = endTime;
        instance.OrderSize = 500;
        instance.OrderTime = 14400; // 4 hours in seconds

        // Act - Calculate manufacturing metrics
        var orderDuration = instance.OrderEnd - instance.OrderStart;
        var expectedDurationSeconds = (int)orderDuration.TotalSeconds;
        var productionRate = instance.OrderSize > 0 && instance.OrderTime > 0
            ? (double)instance.OrderSize / (instance.OrderTime / 3600.0) // units per hour
            : 0.0;

        // Assert - Verify calculated manufacturing metrics
        orderDuration.TotalHours.ShouldBe(4.0, 0.001);
        expectedDurationSeconds.ShouldBe(instance.OrderTime);
        productionRate.ShouldBe(125.0, 0.001); // 500 units / 4 hours = 125 units/hour
    }

    /// <summary>
    /// Executes Order_WithAutomotiveManufacturingScenario_ShouldCalculateCorrectCycleTimeAndProductionRate operation.
    /// </summary>

    [Fact]
    public void Order_WithAutomotiveManufacturingScenario_ShouldCalculateCorrectCycleTimeAndProductionRate()
    {
        // Arrange - Automotive manufacturing order scenario
        var automotiveOrder = new Order
        {
            OrderId = 2023001,
            MachineId = 100101, // Stamping press
            OperatorId = 5501,
            LeaderId = 5601,
            ProgrammerId = 5701,
            ProductId = 8001, // Automotive part
            ToolingId = 9001, // Stamping die
            TimeStamp = 1640995200,
            OrderSize = 2000, // 2000 parts
            OrderTime = 32400, // 9 hours
            OrderStart = DateTime.Today.AddHours(6), // 6 AM start
            OrderEnd = DateTime.Today.AddHours(15), // 3 PM end
            ResultsId = 3001
        };

        // Act - Verify business rules and calculations
        var actualDuration = (automotiveOrder.OrderEnd - automotiveOrder.OrderStart).TotalSeconds;
        var plannedDuration = automotiveOrder.OrderTime;
        var cycleTime = automotiveOrder.OrderTime / (double)automotiveOrder.OrderSize; // seconds per part
        var theoreticalProductionRate = 3600.0 / cycleTime; // parts per hour

        // Assert - Verify automotive manufacturing business rules
        automotiveOrder.OrderId.ShouldBeGreaterThan(0);
        automotiveOrder.MachineId.ShouldBeGreaterThan(0);
        automotiveOrder.OperatorId.ShouldBeGreaterThan(0);
        automotiveOrder.ProductId.ShouldBeGreaterThan(0);
        automotiveOrder.OrderSize.ShouldBeGreaterThan(0);
        automotiveOrder.OrderTime.ShouldBeGreaterThan(0);
        automotiveOrder.OrderEnd.ShouldBeGreaterThan(automotiveOrder.OrderStart);

        // Manufacturing performance metrics
        actualDuration.ShouldBe(plannedDuration, 1.0); // Allow 1 second tolerance
        cycleTime.ShouldBe(16.2, 0.1); // 32400 seconds / 2000 parts = 16.2 seconds per part
        theoreticalProductionRate.ShouldBe(222.22, 0.01); // 3600 / 16.2 ≈ 222.22 parts/hour
    }

    /// <summary>
    /// Executes Order_WithDayAndNightShifts_ShouldCalculateCorrectShiftDurationsAndProductionRates operation.
    /// </summary>

    [Fact]
    public void Order_WithDayAndNightShifts_ShouldCalculateCorrectShiftDurationsAndProductionRates()
    {
        // Arrange - Multiple shift scenarios
        var dayShiftOrder = new Order
        {
            OrderId = 2023101,
            MachineId = 100201,
            OperatorId = 6001,
            OrderSize = 800,
            OrderTime = 28800, // 8 hours
            OrderStart = DateTime.Today.AddHours(6), // 6 AM
            OrderEnd = DateTime.Today.AddHours(14) // 2 PM
        };

        var nightShiftOrder = new Order
        {
            OrderId = 2023102,
            MachineId = 100201,
            OperatorId = 6002,
            OrderSize = 600,
            OrderTime = 28800, // 8 hours
            OrderStart = DateTime.Today.AddHours(22), // 10 PM
            OrderEnd = DateTime.Today.AddDays(1).AddHours(6) // 6 AM next day
        };

        // Act & Assert - Day shift validation
        dayShiftOrder.ShouldNotBeNull();
        dayShiftOrder.OrderStart.Hour.ShouldBe(6);
        dayShiftOrder.OrderEnd.Hour.ShouldBe(14);
        (dayShiftOrder.OrderEnd - dayShiftOrder.OrderStart).TotalHours.ShouldBe(8.0);

        // Act & Assert - Night shift validation
        nightShiftOrder.ShouldNotBeNull();
        nightShiftOrder.OrderStart.Hour.ShouldBe(22);
        nightShiftOrder.OrderEnd.Hour.ShouldBe(6);
        (nightShiftOrder.OrderEnd - nightShiftOrder.OrderStart).TotalHours.ShouldBe(8.0);

        // Verify production targets are met based on shift capacity
        var dayShiftProductionRate = dayShiftOrder.OrderSize / 8.0; // 100 units/hour
        var nightShiftProductionRate = nightShiftOrder.OrderSize / 8.0; // 75 units/hour

        dayShiftProductionRate.ShouldBe(100.0);
        nightShiftProductionRate.ShouldBe(75.0);
    }

    /// <summary>
    /// Executes Order_WithHighVolumeProduction_ShouldMaintainRealisticCycleTimeAndUtilization operation.
    /// </summary>

    [Fact]
    public void Order_WithHighVolumeProduction_ShouldMaintainRealisticCycleTimeAndUtilization()
    {
        // Arrange - High-volume automotive order
        var highVolumeOrder = new Order
        {
            OrderId = 2023201,
            MachineId = 100301,
            ProductId = 8101,
            OrderSize = 5000, // High volume
            OrderTime = 86400, // 24 hours continuous production
            OrderStart = DateTime.Today,
            OrderEnd = DateTime.Today.AddDays(1)
        };

        // Act - Calculate manufacturing constraints
        var totalCycleTime = highVolumeOrder.OrderTime / (double)highVolumeOrder.OrderSize;
        var partsPerMinute = 60.0 / totalCycleTime;
        var utilizationEfficiency = (highVolumeOrder.OrderEnd - highVolumeOrder.OrderStart).TotalSeconds / highVolumeOrder.OrderTime;

        // Assert - Verify manufacturing feasibility
        highVolumeOrder.OrderSize.ShouldBeGreaterThan(1000); // High volume threshold
        totalCycleTime.ShouldBe(17.28, 0.01); // 86400 / 5000 = 17.28 seconds per part
        partsPerMinute.ShouldBe(3.47, 0.01); // 60 / 17.28 ≈ 3.47 parts per minute
        utilizationEfficiency.ShouldBe(1.0, 0.001); // 100% utilization

        // Business rule: Order should not exceed machine capacity
        partsPerMinute.ShouldBeLessThan(10.0); // Reasonable automotive production rate
        totalCycleTime.ShouldBeGreaterThan(10.0); // Minimum cycle time for complex parts
    }

    /// <summary>
    /// Executes Order_WithMinimumAndMaximumValues_ShouldHandleExtremeCycleTimesCorrectly operation.
    /// </summary>

    [Fact]
    public void Order_WithMinimumAndMaximumValues_ShouldHandleExtremeCycleTimesCorrectly()
    {
        // Arrange - Edge case scenarios
        var miniOrder = new Order
        {
            OrderId = 1,
            OrderSize = 1, // Minimum order size
            OrderTime = 60, // 1 minute
            TimeStamp = 0 // Unix epoch
        };

        var maxOrder = new Order
        {
            OrderId = int.MaxValue,
            OrderSize = 100000, // Large order
            OrderTime = 604800, // 1 week in seconds
            TimeStamp = int.MaxValue
        };

        // Act & Assert - Minimum values
        miniOrder.OrderId.ShouldBe(1);
        miniOrder.OrderSize.ShouldBe(1);
        miniOrder.OrderTime.ShouldBe(60);
        miniOrder.TimeStamp.ShouldBe(0);

        // Act & Assert - Maximum values
        maxOrder.OrderId.ShouldBe(int.MaxValue);
        maxOrder.OrderSize.ShouldBe(100000);
        maxOrder.OrderTime.ShouldBe(604800);
        maxOrder.TimeStamp.ShouldBe(int.MaxValue);

        // Verify extreme cycle times
        var miniCycleTime = miniOrder.OrderTime / (double)miniOrder.OrderSize;
        var maxCycleTime = maxOrder.OrderTime / (double)maxOrder.OrderSize;

        miniCycleTime.ShouldBe(60.0); // 1 minute per part
        maxCycleTime.ShouldBe(6.048); // ~6 seconds per part for high volume
    }
}
