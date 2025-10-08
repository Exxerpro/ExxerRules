namespace IndTrace.Domain.UnitTests.ShiftsTests;

/// <summary>
/// Unit tests for ShiftDefinition - Manufacturing shift definition support
/// </summary>
public class ShiftDefinitionTests
{
    /// <summary>
    /// Executes ShiftDefinition_WhenDefaultConstructor_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void ShiftDefinition_WhenDefaultConstructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var shiftDefinition = new ShiftDefinition();

        // Assert
        shiftDefinition.ShouldNotBeNull();
        shiftDefinition.ShouldBeAssignableTo<AuditableEntity>();
        shiftDefinition.ShiftCatalogId.ShouldBe(0);
        shiftDefinition.PlantId.ShouldBe(0);
        shiftDefinition.ShiftName.ShouldBe(string.Empty); // Refactored to use string.Empty (safer than null)
        shiftDefinition.StartBy.ShouldBe(default(TimeSpan));
        shiftDefinition.Duration.ShouldBe(default(TimeSpan));
        shiftDefinition.EndTime.ShouldBe(default(TimeSpan));
    }
    /// <summary>
    /// Executes ShiftDefinition_WhenInitializer_ShouldCreateInstanceWithValues operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_WhenInitializer_ShouldCreateInstanceWithValues()
    {
        // Arrange
        var shiftCatalogId = 1;
        var plantId = 100;
        var shiftName = "Morning Shift";
        var startBy = new TimeSpan(7, 0, 0); // 7:00 AM
        var duration = new TimeSpan(8, 0, 0); // 8 hours
        var endTime = new TimeSpan(15, 0, 0); // 3:00 PM

        // Act
        var shiftDefinition = new ShiftDefinition
        {
            ShiftCatalogId = shiftCatalogId,
            PlantId = plantId,
            ShiftName = shiftName,
            StartBy = startBy,
            Duration = duration,
            EndTime = endTime
        };

        // Assert
        shiftDefinition.ShouldNotBeNull();
        shiftDefinition.ShiftCatalogId.ShouldBe(shiftCatalogId);
        shiftDefinition.PlantId.ShouldBe(plantId);
        shiftDefinition.ShiftName.ShouldBe(shiftName);
        shiftDefinition.StartBy.ShouldBe(startBy);
        shiftDefinition.Duration.ShouldBe(duration);
        shiftDefinition.EndTime.ShouldBe(endTime);
    }
    /// <summary>
    /// Executes ShiftDefinition_Properties_WithManufacturingShifts_ShouldSetAndGetCorrectly operation.
    /// </summary>

    [Theory]
    [InlineData(1, 101, "First Shift", 7, 0, 8, 0, 15, 0)]
    [InlineData(2, 102, "Second Shift", 15, 0, 8, 0, 23, 0)]
    [InlineData(3, 103, "Third Shift", 23, 0, 8, 0, 7, 0)]
    [InlineData(4, 104, "Weekend Shift", 8, 0, 12, 0, 20, 0)]
    [InlineData(5, 105, "Maintenance Shift", 22, 0, 6, 0, 4, 0)]
    public void ShiftDefinition_Properties_WithManufacturingShifts_ShouldSetAndGetCorrectly(int catalogId, int plantId, string shiftName,
        int startHour, int startMinute, int durationHours, int durationMinutes, int endHour, int endMinute)
    {
        // Arrange
        var shiftDefinition = new ShiftDefinition();
        var startBy = new TimeSpan(startHour, startMinute, 0);
        var duration = new TimeSpan(durationHours, durationMinutes, 0);
        var endTime = new TimeSpan(endHour, endMinute, 0);

        // Act
        shiftDefinition.ShiftCatalogId = catalogId;
        shiftDefinition.PlantId = plantId;
        shiftDefinition.ShiftName = shiftName;
        shiftDefinition.StartBy = startBy;
        shiftDefinition.Duration = duration;
        shiftDefinition.EndTime = endTime;

        // Assert
        shiftDefinition.ShiftCatalogId.ShouldBe(catalogId);
        shiftDefinition.PlantId.ShouldBe(plantId);
        shiftDefinition.ShiftName.ShouldBe(shiftName);
        shiftDefinition.StartBy.ShouldBe(startBy);
        shiftDefinition.Duration.ShouldBe(duration);
        shiftDefinition.EndTime.ShouldBe(endTime);
    }
    /// <summary>
    /// Executes ShiftDefinition_Properties_WithNullShiftName_ShouldAcceptNullValue operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_Properties_WithNullShiftName_ShouldAcceptNullValue()
    {
        // Arrange
        var shiftDefinition = new ShiftDefinition();

        // Act
        shiftDefinition.ShiftName = null!;

        // Assert
        shiftDefinition.ShiftName.ShouldBeNull();
    }
    /// <summary>
    /// Executes ShiftDefinition_Properties_WithEmptyShiftName_ShouldAcceptEmptyString operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_Properties_WithEmptyShiftName_ShouldAcceptEmptyString()
    {
        // Arrange
        var shiftDefinition = new ShiftDefinition();

        // Act
        shiftDefinition.ShiftName = string.Empty;

        // Assert
        shiftDefinition.ShiftName.ShouldBe(string.Empty);
    }
    /// <summary>
    /// Executes ShiftCatalogId_WithVariousValues_ShouldAcceptAllValues operation.
    /// </summary>
    /// <param name="catalogId">The catalogId.</param>

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void ShiftCatalogId_WithVariousValues_ShouldAcceptAllValues(int catalogId)
    {
        // Arrange
        var shiftDefinition = new ShiftDefinition();

        // Act
        shiftDefinition.ShiftCatalogId = catalogId;

        // Assert
        shiftDefinition.ShiftCatalogId.ShouldBe(catalogId);
    }
    /// <summary>
    /// Executes PlantId_WithVariousValues_ShouldAcceptAllValues operation.
    /// </summary>
    /// <param name="plantId">The plantId.</param>

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void PlantId_WithVariousValues_ShouldAcceptAllValues(int plantId)
    {
        // Arrange
        var shiftDefinition = new ShiftDefinition();

        // Act
        shiftDefinition.PlantId = plantId;

        // Assert
        shiftDefinition.PlantId.ShouldBe(plantId);
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithFordF150PlantShiftDefinition_ShouldCreateValidShiftDefinition operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_WithFordF150PlantShiftDefinition_ShouldCreateValidShiftDefinition()
    {
        // Arrange - Ford F-150 Plant Shift Schedule (Dearborn Plant)
        var fordMorningShift = new ShiftDefinition
        {
            ShiftCatalogId = 1,
            PlantId = 1001, // Ford Dearborn Plant
            ShiftName = "Ford_Morning_Shift",
            StartBy = new TimeSpan(6, 0, 0), // 6:00 AM
            Duration = new TimeSpan(8, 30, 0), // 8.5 hours
            EndTime = new TimeSpan(14, 30, 0) // 2:30 PM
        };

        // Assert
        fordMorningShift.ShouldNotBeNull();
        fordMorningShift.ShiftCatalogId.ShouldBe(1);
        fordMorningShift.PlantId.ShouldBe(1001);
        fordMorningShift.ShiftName.ShouldBe("Ford_Morning_Shift");
        fordMorningShift.StartBy.ShouldBe(new TimeSpan(6, 0, 0));
        fordMorningShift.Duration.ShouldBe(new TimeSpan(8, 30, 0));
        fordMorningShift.EndTime.ShouldBe(new TimeSpan(14, 30, 0));
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithTeslaModelSFactoryShiftDefinition_ShouldCreateValidShiftDefinition operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_WithTeslaModelSFactoryShiftDefinition_ShouldCreateValidShiftDefinition()
    {
        // Arrange - Tesla Model S Factory Shift Schedule (Fremont Factory)
        var teslaAfternoonShift = new ShiftDefinition
        {
            ShiftCatalogId = 2,
            PlantId = 2001, // Tesla Fremont Factory
            ShiftName = "Tesla_Afternoon_Shift",
            StartBy = new TimeSpan(15, 0, 0), // 3:00 PM
            Duration = new TimeSpan(8, 0, 0), // 8 hours
            EndTime = new TimeSpan(23, 0, 0) // 11:00 PM
        };

        // Assert
        teslaAfternoonShift.ShouldNotBeNull();
        teslaAfternoonShift.ShiftCatalogId.ShouldBe(2);
        teslaAfternoonShift.PlantId.ShouldBe(2001);
        teslaAfternoonShift.ShiftName.ShouldBe("Tesla_Afternoon_Shift");
        teslaAfternoonShift.StartBy.ShouldBe(new TimeSpan(15, 0, 0));
        teslaAfternoonShift.Duration.ShouldBe(new TimeSpan(8, 0, 0));
        teslaAfternoonShift.EndTime.ShouldBe(new TimeSpan(23, 0, 0));
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithBMWX5PlantShiftDefinition_ShouldCreateValidShiftDefinition operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_WithBMWX5PlantShiftDefinition_ShouldCreateValidShiftDefinition()
    {
        // Arrange - BMW X5 Plant Shift Schedule (Spartanburg Plant)
        var bmwNightShift = new ShiftDefinition
        {
            ShiftCatalogId = 3,
            PlantId = 3001, // BMW Spartanburg Plant
            ShiftName = "BMW_Night_Shift",
            StartBy = new TimeSpan(23, 0, 0), // 11:00 PM
            Duration = new TimeSpan(8, 0, 0), // 8 hours
            EndTime = new TimeSpan(7, 0, 0) // 7:00 AM (next day)
        };

        // Assert
        bmwNightShift.ShouldNotBeNull();
        bmwNightShift.ShiftCatalogId.ShouldBe(3);
        bmwNightShift.PlantId.ShouldBe(3001);
        bmwNightShift.ShiftName.ShouldBe("BMW_Night_Shift");
        bmwNightShift.StartBy.ShouldBe(new TimeSpan(23, 0, 0));
        bmwNightShift.Duration.ShouldBe(new TimeSpan(8, 0, 0));
        bmwNightShift.EndTime.ShouldBe(new TimeSpan(7, 0, 0));
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithMercedesQualityControlShiftDefinition_ShouldCreateValidShiftDefinition operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_WithMercedesQualityControlShiftDefinition_ShouldCreateValidShiftDefinition()
    {
        // Arrange - Mercedes Quality Control Shift Schedule (Tuscaloosa Plant)
        var mercedesQcShift = new ShiftDefinition
        {
            ShiftCatalogId = 4,
            PlantId = 4001, // Mercedes Tuscaloosa Plant
            ShiftName = "Mercedes_QC_Extended_Shift",
            StartBy = new TimeSpan(8, 0, 0), // 8:00 AM
            Duration = new TimeSpan(10, 0, 0), // 10 hours (extended for QC)
            EndTime = new TimeSpan(18, 0, 0) // 6:00 PM
        };

        // Assert
        mercedesQcShift.ShouldNotBeNull();
        mercedesQcShift.ShiftCatalogId.ShouldBe(4);
        mercedesQcShift.PlantId.ShouldBe(4001);
        mercedesQcShift.ShiftName.ShouldBe("Mercedes_QC_Extended_Shift");
        mercedesQcShift.StartBy.ShouldBe(new TimeSpan(8, 0, 0));
        mercedesQcShift.Duration.ShouldBe(new TimeSpan(10, 0, 0));
        mercedesQcShift.EndTime.ShouldBe(new TimeSpan(18, 0, 0));
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithAudiA4MaintenanceShiftDefinition_ShouldCreateValidShiftDefinition operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_WithAudiA4MaintenanceShiftDefinition_ShouldCreateValidShiftDefinition()
    {
        // Arrange - Audi A4 Maintenance Shift Schedule (Ingolstadt Plant)
        var audiMaintenanceShift = new ShiftDefinition
        {
            ShiftCatalogId = 5,
            PlantId = 5001, // Audi Ingolstadt Plant
            ShiftName = "Audi_Maintenance_Shift",
            StartBy = new TimeSpan(2, 0, 0), // 2:00 AM
            Duration = new TimeSpan(4, 0, 0), // 4 hours (maintenance window)
            EndTime = new TimeSpan(6, 0, 0) // 6:00 AM
        };

        // Assert
        audiMaintenanceShift.ShouldNotBeNull();
        audiMaintenanceShift.ShiftCatalogId.ShouldBe(5);
        audiMaintenanceShift.PlantId.ShouldBe(5001);
        audiMaintenanceShift.ShiftName.ShouldBe("Audi_Maintenance_Shift");
        audiMaintenanceShift.StartBy.ShouldBe(new TimeSpan(2, 0, 0));
        audiMaintenanceShift.Duration.ShouldBe(new TimeSpan(4, 0, 0));
        audiMaintenanceShift.EndTime.ShouldBe(new TimeSpan(6, 0, 0));
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithIndustry40ShiftDefinitions_ShouldCreateValidShiftDefinitions operation.
    /// </summary>

    [Theory]
    [InlineData(1, 1001, "Standard_Day_Shift", 8, 0, 8, 0, 16, 0)]
    [InlineData(2, 2001, "Swing_Shift", 16, 0, 8, 0, 0, 0)]
    [InlineData(3, 3001, "Graveyard_Shift", 0, 0, 8, 0, 8, 0)]
    [InlineData(4, 4001, "Weekend_Extended", 6, 0, 12, 0, 18, 0)]
    [InlineData(5, 5001, "Maintenance_Window", 1, 0, 5, 0, 6, 0)]
    public void ShiftDefinition_DomainLogic_WithIndustry40ShiftDefinitions_ShouldCreateValidShiftDefinitions(int catalogId, int plantId, string shiftName,
        int startHour, int startMinute, int durationHours, int durationMinutes, int endHour, int endMinute)
    {
        // Arrange & Act
        var shiftDefinition = new ShiftDefinition
        {
            ShiftCatalogId = catalogId,
            PlantId = plantId,
            ShiftName = shiftName,
            StartBy = new TimeSpan(startHour, startMinute, 0),
            Duration = new TimeSpan(durationHours, durationMinutes, 0),
            EndTime = new TimeSpan(endHour, endMinute, 0)
        };

        // Assert - Industry 4.0 Shift Management Support
        shiftDefinition.ShouldNotBeNull();
        shiftDefinition.ShiftCatalogId.ShouldBe(catalogId);
        shiftDefinition.PlantId.ShouldBe(plantId);
        shiftDefinition.ShiftName.ShouldBe(shiftName);
        shiftDefinition.StartBy.ShouldBe(new TimeSpan(startHour, startMinute, 0));
        shiftDefinition.Duration.ShouldBe(new TimeSpan(durationHours, durationMinutes, 0));
        shiftDefinition.EndTime.ShouldBe(new TimeSpan(endHour, endMinute, 0));
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithCrossPlantShiftCoordination_ShouldSupportMultiplePlantOperations operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_WithCrossPlantShiftCoordination_ShouldSupportMultiplePlantOperations()
    {
        // Arrange - Multi-plant manufacturing coordination
        var plant1MorningShift = new ShiftDefinition
        {
            ShiftCatalogId = 1,
            PlantId = 1001, // Plant 1
            ShiftName = "Plant1_Morning",
            StartBy = new TimeSpan(6, 0, 0),
            Duration = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(14, 0, 0)
        };

        var plant2MorningShift = new ShiftDefinition
        {
            ShiftCatalogId = 1, // Same catalog ID, different plant
            PlantId = 2001, // Plant 2
            ShiftName = "Plant2_Morning",
            StartBy = new TimeSpan(7, 0, 0), // 1-hour offset
            Duration = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(15, 0, 0)
        };

        // Act & Assert - Cross-plant shift coordination
        plant1MorningShift.ShouldNotBeNull();
        plant2MorningShift.ShouldNotBeNull();

        // Same shift type, different plants
        plant1MorningShift.ShiftCatalogId.ShouldBe(plant2MorningShift.ShiftCatalogId);
        plant1MorningShift.PlantId.ShouldNotBe(plant2MorningShift.PlantId);

        // Different timing for coordination
        plant1MorningShift.StartBy.ShouldNotBe(plant2MorningShift.StartBy);
        plant1MorningShift.EndTime.ShouldNotBe(plant2MorningShift.EndTime);
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithMaximumLengthShiftName_ShouldBeValidForDatabase operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_WithMaximumLengthShiftName_ShouldBeValidForDatabase()
    {
        // Arrange - Based on EF Configuration (ShiftName MaxLength 50)
        var longShiftName = new string('S', 50); // Maximum allowed length
        var shiftDefinition = new ShiftDefinition();

        // Act
        shiftDefinition.ShiftCatalogId = 999;
        shiftDefinition.PlantId = 9999;
        shiftDefinition.ShiftName = longShiftName;
        shiftDefinition.StartBy = new TimeSpan(12, 0, 0);
        shiftDefinition.Duration = new TimeSpan(8, 0, 0);
        shiftDefinition.EndTime = new TimeSpan(20, 0, 0);

        // Assert
        shiftDefinition.ShouldNotBeNull();
        shiftDefinition.ShiftName.ShouldBe(longShiftName);
        shiftDefinition.ShiftName.Length.ShouldBe(50);
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_WithComplexShiftRotationSchedule_ShouldSupportScheduleManagement operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_WithComplexShiftRotationSchedule_ShouldSupportScheduleManagement()
    {
        // Arrange - Complex 24/7 Manufacturing Schedule
        var shifts = new[]
        {
            new ShiftDefinition { ShiftCatalogId = 1, PlantId = 1001, ShiftName = "Day_Shift", StartBy = new TimeSpan(6, 0, 0), Duration = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(14, 0, 0) },
            new ShiftDefinition { ShiftCatalogId = 2, PlantId = 1001, ShiftName = "Evening_Shift", StartBy = new TimeSpan(14, 0, 0), Duration = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(22, 0, 0) },
            new ShiftDefinition { ShiftCatalogId = 3, PlantId = 1001, ShiftName = "Night_Shift", StartBy = new TimeSpan(22, 0, 0), Duration = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(6, 0, 0) }
        };

        // Act & Assert - 24/7 shift coverage validation
        shifts.Length.ShouldBe(3);
        shifts.All(s => s != null).ShouldBeTrue();
        shifts.All(s => s.PlantId == 1001).ShouldBeTrue(); // Same plant
        shifts.All(s => !string.IsNullOrEmpty(s.ShiftName)).ShouldBeTrue();
        shifts.All(s => s.Duration.TotalHours == 8).ShouldBeTrue(); // 8-hour shifts

        // All catalog IDs should be unique for different shift types
        var uniqueCatalogIds = shifts.Select(s => s.ShiftCatalogId).Distinct().Count();
        uniqueCatalogIds.ShouldBe(shifts.Length);

        // Shifts should provide continuous coverage (end time of one = start time of next)
        shifts[0].EndTime.ShouldBe(shifts[1].StartBy); // Day to Evening
        shifts[1].EndTime.ShouldBe(shifts[2].StartBy); // Evening to Night
        // Night to Day wraps around midnight
    }
    /// <summary>
    /// Executes ShiftDefinition_DomainLogic_AsAuditableEntity_ShouldSupportAuditingCapabilities operation.
    /// </summary>

    [Fact]
    public void ShiftDefinition_DomainLogic_AsAuditableEntity_ShouldSupportAuditingCapabilities()
    {
        // Arrange & Act
        var shiftDefinition = new ShiftDefinition();

        // Assert - AuditableEntity inheritance validation
        shiftDefinition.ShouldNotBeNull();
        shiftDefinition.ShouldBeAssignableTo<AuditableEntity>();

        // Should have auditing properties available (inherited from AuditableEntity)
        var auditableType = typeof(AuditableEntity);
        var shiftType = typeof(ShiftDefinition);

        auditableType.IsAssignableFrom(shiftType).ShouldBeTrue();
    }
}
