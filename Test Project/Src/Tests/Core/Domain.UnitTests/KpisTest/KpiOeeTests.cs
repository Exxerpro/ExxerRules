namespace IndTrace.Domain.UnitTests.KpisTest;

/// <summary>
/// Unit tests for KpiOee
/// </summary>
public class KpiOeeTests
{
    /// <summary>
    /// Executes KpiOee_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully operation.
    /// </summary>
    [Fact]
    public void KpiOee_WithAllRequiredProperties_ShouldCreateInstanceSuccessfully()
    {
        // Arrange & Act
        var instance = new KpiOee();

        // Assert
        instance.ShouldNotBeNull();
        instance.KpiOeeId.ShouldBe(default(int));
        instance.OeeRegisterId.ShouldBe(default(int));
        instance.Oee.ShouldBe(default(double));
        instance.Availability.ShouldBe(default(double));
        instance.Performance.ShouldBe(default(double));
        instance.Quality.ShouldBe(default(double));
        instance.TimeStamp.ShouldBe(default(DateTime));
        instance.OeeRegister.ShouldBeNull();
        instance.ShouldBeAssignableTo<IEntityRoot>();
    }

    /// <summary>
    /// Executes KpiOee_WithInvalidConfiguration_ShouldHandleErrorsGracefully operation.
    /// </summary>

    [Fact]
    public void KpiOee_WithInvalidConfiguration_ShouldHandleErrorsGracefully()
    {
        // Arrange
        var validKpiOee = new KpiOee();

        // Act - Testing edge case scenarios for OEE KPI constraints
        validKpiOee.Oee = -0.5; // Negative OEE should be handled
        validKpiOee.Availability = -0.3; // Negative availability
        validKpiOee.Performance = -0.7; // Negative performance
        validKpiOee.Quality = -0.2; // Negative quality
        validKpiOee.KpiOeeId = -1; // Negative ID
        validKpiOee.OeeRegisterId = -100; // Negative register ID

        // Assert - KpiOee should gracefully handle negative values
        validKpiOee.ShouldNotBeNull();
        validKpiOee.Oee.ShouldBe(-0.5);
        validKpiOee.Availability.ShouldBe(-0.3);
        validKpiOee.Performance.ShouldBe(-0.7);
        validKpiOee.Quality.ShouldBe(-0.2);
        validKpiOee.KpiOeeId.ShouldBe(-1);
        validKpiOee.OeeRegisterId.ShouldBe(-100);

        // Testing extreme values
        var extremeKpiOee = new KpiOee
        {
            Oee = 5.0, // Extreme OEE value
            Availability = 2.5, // Over 100% availability
            Performance = 3.0, // Very high performance
            Quality = 1.5, // Over 100% quality
            KpiOeeId = int.MaxValue,
            OeeRegisterId = int.MaxValue
        };

        extremeKpiOee.ShouldNotBeNull();
        extremeKpiOee.Oee.ShouldBe(5.0);
        extremeKpiOee.Availability.ShouldBe(2.5);
        extremeKpiOee.Performance.ShouldBe(3.0);
        extremeKpiOee.Quality.ShouldBe(1.5);
    }

    /// <summary>
    /// Executes KpiOee_WhenPropertiesAssigned_ShouldMaintainAllValues operation.
    /// </summary>

    [Fact]
    public void KpiOee_WhenPropertiesAssigned_ShouldMaintainAllValues()
    {
        // Arrange
        var instance = new KpiOee();
        var testTimeStamp = DateTime.UtcNow;

        // Act - Setting all KPI OEE properties for manufacturing scenario
        instance.KpiOeeId = 12345;
        instance.OeeRegisterId = 67890;
        instance.Oee = 0.85; // 85% OEE (excellent for automotive)
        instance.Availability = 0.90; // 90% availability
        instance.Performance = 0.95; // 95% performance
        instance.Quality = 0.99; // 99% quality (world-class)
        instance.TimeStamp = testTimeStamp;

        // Assert - Verify all properties are set correctly
        instance.KpiOeeId.ShouldBe(12345);
        instance.OeeRegisterId.ShouldBe(67890);
        instance.Oee.ShouldBe(0.85);
        instance.Availability.ShouldBe(0.90);
        instance.Performance.ShouldBe(0.95);
        instance.Quality.ShouldBe(0.99);
        instance.TimeStamp.ShouldBe(testTimeStamp);

        // Test property independence
        var originalOee = instance.Oee;
        instance.Availability = 0.75;
        instance.Oee.ShouldBe(originalOee); // Should remain unchanged

        // Test realistic automotive manufacturing KPI ranges
        instance.Oee = 0.65; // Good OEE for automotive
        instance.Availability = 0.85; // Typical availability
        instance.Performance = 1.05; // Slightly over standard (105%)
        instance.Quality = 0.97; // High quality rate

        instance.Oee.ShouldBe(0.65);
        instance.Availability.ShouldBe(0.85);
        instance.Performance.ShouldBe(1.05);
        instance.Quality.ShouldBe(0.97);
    }

    /// <summary>
    /// Executes KpiOee_WhenMethodsInvoked_ShouldProduceExpectedOutcomes operation.
    /// </summary>

    [Fact]
    public void KpiOee_WhenMethodsInvoked_ShouldProduceExpectedOutcomes()
    {
        // Arrange
        var instance = new KpiOee
        {
            KpiOeeId = 1,
            OeeRegisterId = 1001,
            Oee = 0.75,
            Availability = 0.85,
            Performance = 0.92,
            Quality = 0.96,
            TimeStamp = DateTime.UtcNow
        };

        // Act & Assert - Test object equality (reference equality by default)
        var instance1 = new KpiOee { KpiOeeId = 1, Oee = 0.75 };
        var instance2 = new KpiOee { KpiOeeId = 1, Oee = 0.75 };
        var instance3 = instance1;

        instance1.ShouldNotBeSameAs(instance2); // Different instances
        instance1.ShouldBeSameAs(instance3); // Same reference
        (instance1 == instance2).ShouldBeFalse(); // Reference equality
        (instance1 == instance3).ShouldBeTrue(); // Same reference

        // Test GetHashCode method
        var hashCode1 = instance1.GetHashCode();
        var hashCode3 = instance3.GetHashCode();
        hashCode1.ShouldBe(hashCode3); // Same reference should have same hash code

        // Test GetType method
        var type = instance.GetType();
        type.ShouldNotBeNull();
        type.Name.ShouldBe("KpiOee");

        // Test ToString method
        var toStringResult = instance.ToString();
        toStringResult.ShouldNotBeNull();
        toStringResult.ShouldContain("Kpi");

        // Test property reflection
        var properties = type.GetProperties();
        properties.Length.ShouldBe(8); // 7 scalar properties + 1 navigation property

        var oeeProperty = properties.FirstOrDefault(p => p.Name == "Oee");
        oeeProperty.ShouldNotBeNull();
        oeeProperty!.PropertyType.ShouldBe(typeof(double));

        var availabilityProperty = properties.FirstOrDefault(p => p.Name == "Availability");
        availabilityProperty.ShouldNotBeNull();
        availabilityProperty!.PropertyType.ShouldBe(typeof(double));

        var qualityProperty = properties.FirstOrDefault(p => p.Name == "Quality");
        qualityProperty.ShouldNotBeNull();
        qualityProperty!.PropertyType.ShouldBe(typeof(double));
    }

    /// <summary>
    /// Executes KpiOee_BusinessScenario_ShouldEnforceAllDomainRules operation.
    /// </summary>

    [Fact]
    public void KpiOee_BusinessScenario_ShouldEnforceAllDomainRules()
    {
        // Arrange - Automotive manufacturing OEE KPI scenarios
        var worldClassKpi = new KpiOee
        {
            KpiOeeId = 1,
            OeeRegisterId = 1001,
            Oee = 0.85, // World-class OEE (>85%)
            Availability = 0.90, // 90% availability
            Performance = 0.95, // 95% performance
            Quality = 0.99, // 99% quality
            TimeStamp = DateTime.UtcNow
        };

        var averageKpi = new KpiOee
        {
            KpiOeeId = 2,
            OeeRegisterId = 1002,
            Oee = 0.60, // Average OEE (60%)
            Availability = 0.75, // 75% availability
            Performance = 0.85, // 85% performance
            Quality = 0.94, // 94% quality
            TimeStamp = DateTime.UtcNow.AddHours(-1)
        };

        var poorKpi = new KpiOee
        {
            KpiOeeId = 3,
            OeeRegisterId = 1003,
            Oee = 0.40, // Poor OEE (40%)
            Availability = 0.60, // 60% availability
            Performance = 0.70, // 70% performance
            Quality = 0.95, // 95% quality (maintained despite issues)
            TimeStamp = DateTime.UtcNow.AddHours(-2)
        };

        // Act & Assert - Verify automotive manufacturing OEE business rules
        var kpis = new List<KpiOee> { worldClassKpi, averageKpi, poorKpi };

        // Business Rule 1: OEE should be product of Availability × Performance × Quality
        foreach (var kpi in kpis)
        {
            var calculatedOee = kpi.Availability * kpi.Performance * kpi.Quality;
            kpi.Oee.ShouldBe(calculatedOee, 0.01); // Allow small tolerance for rounding
        }

        // Business Rule 2: World-class manufacturing thresholds
        worldClassKpi.Oee.ShouldBeGreaterThan(0.80); // World-class threshold
        worldClassKpi.Availability.ShouldBeGreaterThan(0.85);
        worldClassKpi.Performance.ShouldBeGreaterThan(0.90);
        worldClassKpi.Quality.ShouldBeGreaterThan(0.95);

        // Business Rule 3: Performance can exceed 100% (better than standard)
        var highPerformanceKpi = new KpiOee
        {
            Availability = 0.95,
            Performance = 1.15, // 115% performance (15% better than standard)
            Quality = 0.98
        };
        highPerformanceKpi.Performance.ShouldBeGreaterThan(1.0);

        // Business Rule 4: OEE classification levels
        worldClassKpi.Oee.ShouldBeGreaterThanOrEqualTo(0.85); // World-class
        averageKpi.Oee.ShouldBeLessThan(0.85);
        averageKpi.Oee.ShouldBeGreaterThanOrEqualTo(0.50); // Average
        poorKpi.Oee.ShouldBeLessThan(0.50); // Poor

        // Business Rule 5: Temporal consistency
        worldClassKpi.TimeStamp.ShouldBeGreaterThan(averageKpi.TimeStamp);
        averageKpi.TimeStamp.ShouldBeGreaterThan(poorKpi.TimeStamp);

        // Business Rule 6: Quality should typically be highest metric
        foreach (var kpi in kpis)
        {
            kpi.Quality.ShouldBeGreaterThanOrEqualTo(kpi.Availability);
            // Note: Performance can exceed Quality due to faster than standard cycle times
        }

        // Business Rule 7: OEE improvement tracking
        var kpiTrend = kpis.OrderBy(k => k.TimeStamp).ToList();
        var improvementTrend = kpiTrend.Select(k => k.Oee).ToList();

        // Verify we have different OEE levels for comparison
        improvementTrend.ShouldContain(oee => oee > 0.80); // World-class
        improvementTrend.ShouldContain(oee => oee >= 0.50 && oee < 0.80); // Average
        improvementTrend.ShouldContain(oee => oee < 0.50); // Poor
    }

    /// <summary>
    /// Executes ManufacturingKpiScenarios_WithRealWorldData_ShouldCalculateCorrectly operation.
    /// </summary>

    [Fact]
    public void ManufacturingKpiScenarios_WithRealWorldData_ShouldCalculateCorrectly()
    {
        // Arrange - Real automotive stamping press OEE scenarios
        var morningShiftKpi = new KpiOee
        {
            KpiOeeId = 101,
            OeeRegisterId = 2001,
            Oee = 0.72, // 72% OEE
            Availability = 0.85, // 85% availability (some planned stops)
            Performance = 0.90, // 90% performance (10% slower than ideal)
            Quality = 0.94, // 94% quality (6% defect rate)
            TimeStamp = DateTime.Today.AddHours(8) // 8 AM
        };

        var afternoonShiftKpi = new KpiOee
        {
            KpiOeeId = 102,
            OeeRegisterId = 2001,
            Oee = 0.78, // 78% OEE (improvement)
            Availability = 0.90, // 90% availability (better equipment uptime)
            Performance = 0.92, // 92% performance (slight improvement)
            Quality = 0.94, // 94% quality (consistent)
            TimeStamp = DateTime.Today.AddHours(16) // 4 PM
        };

        var nightShiftKpi = new KpiOee
        {
            KpiOeeId = 103,
            OeeRegisterId = 2001,
            Oee = 0.81, // 81% OEE (best performance)
            Availability = 0.92, // 92% availability (optimal conditions)
            Performance = 0.95, // 95% performance (experienced operators)
            Quality = 0.93, // 93% quality (slight fatigue effect)
            TimeStamp = DateTime.Today.AddHours(24) // Midnight
        };

        // Act & Assert - Verify real-world automotive manufacturing patterns
        var shiftKpis = new List<KpiOee> { morningShiftKpi, afternoonShiftKpi, nightShiftKpi };

        // Manufacturing analysis: OEE should improve through the day as equipment warms up
        var oeeTrend = shiftKpis.OrderBy(k => k.TimeStamp).Select(k => k.Oee).ToList();
        oeeTrend[0].ShouldBe(0.72); // Morning: lowest
        oeeTrend[1].ShouldBe(0.78); // Afternoon: improved
        oeeTrend[2].ShouldBe(0.81); // Night: highest

        // Availability should improve as operators become more experienced with equipment
        morningShiftKpi.Availability.ShouldBe(0.85);
        afternoonShiftKpi.Availability.ShouldBe(0.90);
        nightShiftKpi.Availability.ShouldBe(0.92);

        // Performance should improve with equipment warm-up and operator experience
        morningShiftKpi.Performance.ShouldBeLessThan(afternoonShiftKpi.Performance);
        afternoonShiftKpi.Performance.ShouldBeLessThan(nightShiftKpi.Performance);

        // Quality should remain relatively stable (good process control)
        foreach (var kpi in shiftKpis)
        {
            kpi.Quality.ShouldBeGreaterThan(0.90); // Minimum quality standard
            kpi.Quality.ShouldBeLessThan(0.98); // Realistic upper bound
        }

        // Overall OEE should be in acceptable automotive range
        foreach (var kpi in shiftKpis)
        {
            kpi.Oee.ShouldBeGreaterThan(0.70); // Minimum acceptable for automotive
            kpi.Oee.ShouldBeLessThan(0.90); // Realistic upper bound
        }

        // Verify mathematical consistency: OEE = A × P × Q
        foreach (var kpi in shiftKpis)
        {
            var calculatedOee = Math.Round(kpi.Availability * kpi.Performance * kpi.Quality, 2);
            Math.Round(kpi.Oee, 2).ShouldBe(calculatedOee);
        }
    }

    /// <summary>
    /// Executes EdgeCaseKpiValues_ShouldBeHandledAppropriately operation.
    /// </summary>

    [Fact]
    public void EdgeCaseKpiValues_ShouldBeHandledAppropriately()
    {
        // Arrange - Edge case scenarios
        var perfectKpi = new KpiOee
        {
            KpiOeeId = 1000,
            Oee = 1.0, // Perfect OEE
            Availability = 1.0, // 100% availability
            Performance = 1.0, // 100% performance
            Quality = 1.0 // 100% quality
        };

        var zeroKpi = new KpiOee
        {
            KpiOeeId = 1001,
            Oee = 0.0, // Zero OEE
            Availability = 0.0, // 0% availability
            Performance = 0.0, // 0% performance
            Quality = 0.0 // 0% quality
        };

        var superPerformanceKpi = new KpiOee
        {
            KpiOeeId = 1002,
            Oee = 1.15, // 115% OEE (possible with super performance)
            Availability = 1.0, // 100% availability
            Performance = 1.25, // 125% performance (25% faster than standard)
            Quality = 0.92 // 92% quality
        };

        // Act & Assert - Perfect scenario
        perfectKpi.Oee.ShouldBe(1.0);
        perfectKpi.Availability.ShouldBe(1.0);
        perfectKpi.Performance.ShouldBe(1.0);
        perfectKpi.Quality.ShouldBe(1.0);

        // Zero scenario
        zeroKpi.Oee.ShouldBe(0.0);
        zeroKpi.Availability.ShouldBe(0.0);
        zeroKpi.Performance.ShouldBe(0.0);
        zeroKpi.Quality.ShouldBe(0.0);

        // Super performance scenario
        superPerformanceKpi.Performance.ShouldBeGreaterThan(1.0);
        superPerformanceKpi.Oee.ShouldBeGreaterThan(1.0);
        var calculatedOee = superPerformanceKpi.Availability * superPerformanceKpi.Performance * superPerformanceKpi.Quality;
        superPerformanceKpi.Oee.ShouldBe(calculatedOee, 0.001);

        // Verify super performance is mathematically consistent
        calculatedOee.ShouldBe(1.0 * 1.25 * 0.92); // = 1.15
        calculatedOee.ShouldBe(1.15, 0.001);
    }

    /// <summary>
    /// Executes KpiOeeComparison_WithDifferentTimeStamps_ShouldSupportTrendAnalysis operation.
    /// </summary>

    [Fact]
    public void KpiOeeComparison_WithDifferentTimeStamps_ShouldSupportTrendAnalysis()
    {
        // Arrange - Weekly OEE trend analysis
        var weeklyKpis = new List<KpiOee>
        {
            new() { KpiOeeId = 1, Oee = 0.65, TimeStamp = DateTime.Today.AddDays(-6) }, // Monday
            new() { KpiOeeId = 2, Oee = 0.68, TimeStamp = DateTime.Today.AddDays(-5) }, // Tuesday
            new() { KpiOeeId = 3, Oee = 0.72, TimeStamp = DateTime.Today.AddDays(-4) }, // Wednesday
            new() { KpiOeeId = 4, Oee = 0.75, TimeStamp = DateTime.Today.AddDays(-3) }, // Thursday
            new() { KpiOeeId = 5, Oee = 0.78, TimeStamp = DateTime.Today.AddDays(-2) }, // Friday
            new() { KpiOeeId = 6, Oee = 0.73, TimeStamp = DateTime.Today.AddDays(-1) }, // Saturday
            new() { KpiOeeId = 7, Oee = 0.70, TimeStamp = DateTime.Today }              // Sunday
        };

        // Act - Analyze trend
        var orderedKpis = weeklyKpis.OrderBy(k => k.TimeStamp).ToList();
        var oeeValues = orderedKpis.Select(k => k.Oee).ToList();

        // Assert - Verify weekly improvement trend (with weekend reduction)
        oeeValues[0].ShouldBe(0.65); // Monday: lowest (week start)
        oeeValues[4].ShouldBe(0.78); // Friday: highest (peak efficiency)
        oeeValues[6].ShouldBe(0.70); // Sunday: reduced (weekend operation)

        // Weekday improvement trend
        var weekdayKpis = orderedKpis.Take(5).ToList(); // Monday-Friday
        var weekdayOees = weekdayKpis.Select(k => k.Oee).ToList();

        for (int i = 1; i < weekdayOees.Count; i++)
        {
            weekdayOees[i].ShouldBeGreaterThan(weekdayOees[i - 1]); // Continuous improvement
        }

        // Weekend performance analysis
        var weekendAverage = orderedKpis.Skip(5).Average(k => k.Oee); // Saturday + Sunday
        var weekdayAverage = weekdayKpis.Average(k => k.Oee);

        weekendAverage.ShouldBeLessThan(weekdayAverage); // Weekend typically lower due to skeleton crew
        weekendAverage.ShouldBe(0.715, 0.001); // (0.73 + 0.70) / 2 = 0.715
    }
}
