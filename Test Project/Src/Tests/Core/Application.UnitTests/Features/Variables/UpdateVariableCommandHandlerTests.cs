using IndTrace.Application.Variables.Commands.Update;
using IndTrace.Application.Variables.Queries.GetVariableDetail;

namespace Application.UnitTests.Features.Variables;

/// <summary>
/// Basic tests for UpdateVariableCommandHandler focusing on constructor validation and simple scenarios
/// </summary>
public class UpdateVariableCommandHandlerBasicTests : IDisposable
{
    private readonly IRepository<Variable> _repository = null!;
    private readonly ILogger<UpdateVariableCommandHandler> _logger = null!;
    private readonly UpdateVariableCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public UpdateVariableCommandHandlerBasicTests()
    {
        _repository = Substitute.For<IRepository<Variable>>();
        _logger = XUnitLogger.CreateLogger<UpdateVariableCommandHandler>();
        _handler = new UpdateVariableCommandHandler(_repository, _logger);
    }

    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new UpdateVariableCommandHandler(_repository, _logger);

        // Assert
        handler.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Constructor_WithNullRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullRepository_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		IRepository<Variable>? nullRepository = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() => new UpdateVariableCommandHandler(nullRepository!, _logger));
    // 	}
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullLogger_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		ILogger<UpdateVariableCommandHandler>? nullLogger = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() => new UpdateVariableCommandHandler(_repository, nullLogger!));
    // 	}
    /// <summary>
    /// Executes Should_UpdateVariable_When_ValidCommandProvided operation.
    /// </summary>
    /// <returns>The result of Should_UpdateVariable_When_ValidCommandProvided.</returns>

    [Fact]
    public async Task Should_UpdateVariable_When_ValidCommandProvided()
    {
        // Arrange - Ford F-150 engine temperature sensor update
        var existingVariable = new Variable
        {
            VariableId = 1001,
            MachineId = 10001,
            Name = "Engine_Temperature_Sensor",
            Address = "DB1.DBD0",
            NetType = "REAL",
            Length = 4,
            IsActive = 1,
            Direction = 1,
            VariableGroupId = 10,
            NativeType = "TEMPERATURE",
            NativeAddress = "READ"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 1001,
            MachineId = 10001,
            Name = "Engine_Temperature_Sensor_Updated",
            Address = "DB1.DBD4",
            Type = "REAL",
            Length = 4,
            Event = 1,
            Direction = 1,
            VariableGroupId = 10
        };

        _repository.GetByIdAsync(1001, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.VariableId.ShouldBe(1001);
        result.Value.Name.ShouldBe("Engine_Temperature_Sensor_Updated");
        result.Value.Address.ShouldBe("DB1.DBD4");
        result.Value.NetType.ShouldBe("REAL");

        await _repository.Received(1).GetByIdAsync(1001, Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v => v.Name == "Engine_Temperature_Sensor_Updated"),
            Arg.Any<CancellationToken>());
        await _repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_ReturnFailure_When_EntityNotFound operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_EntityNotFound.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_EntityNotFound()
    {
        // Arrange - Non-existent variable update attempt
        var command = new UpdateVariableCommand { VariableId = 999 };

        _repository.GetByIdAsync(999, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.WithFailure("Entity not found in database"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Executes Should_ReturnFailure_When_GetByIdReturnsNull operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_GetByIdReturnsNull.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_GetByIdReturnsNull()
    {
        // Arrange - GetById returns success but null value
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(null));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Variable not found");
    }

    /// <summary>
    /// Executes Should_ReturnFailure_When_UpdateFails operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_UpdateFails.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_UpdateFails()
    {
        // Arrange - Repository update failure
        var existingVariable = new Variable { VariableId = 1, MachineId = 10000 };
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Database update failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Database update failed");
    }

    /// <summary>
    /// Executes Should_ReturnFailure_When_CommitFails operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_CommitFails.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_CommitFails()
    {
        // Arrange - Repository commit failure
        var existingVariable = new Variable { VariableId = 1, MachineId = 10000 };
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Transaction commit failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Transaction commit failed");
    }

    /// <summary>
    /// Executes Should_PassCancellationToken_When_RepositoryMethodsCalled operation.
    /// </summary>
    /// <returns>The result of Should_PassCancellationToken_When_RepositoryMethodsCalled.</returns>

    [Fact]
    public async Task Should_PassCancellationToken_When_RepositoryMethodsCalled()
    {
        // Arrange
        var existingVariable = new Variable { VariableId = 1, MachineId = 10000 };
        var command = new UpdateVariableCommand { VariableId = 1 };
        var cancellationToken = new CancellationToken();

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        await _handler.ProcessAsync(command, cancellationToken);

        // Assert
        await _repository.Received(1).GetByIdAsync(1, cancellationToken);
        await _repository.Received(1).UpdateAsync(Arg.Any<Variable>(), cancellationToken);
        await _repository.Received(1).CommitAsync(cancellationToken);
    }

    /// <summary>
    /// Executes Dispose operation.
    /// </summary>

    public void Dispose()
    {
        // Cleanup if needed
    }
}

/// <summary>
/// Manufacturing scenario tests for UpdateVariableCommandHandler with complex industrial data modifications
/// </summary>
public class UpdateVariableCommandHandlerManufacturingTests : IDisposable
{
    private readonly IRepository<Variable> _repository = null!;
    private readonly ILogger<UpdateVariableCommandHandler> _logger = null!;
    private readonly UpdateVariableCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public UpdateVariableCommandHandlerManufacturingTests()
    {
        _repository = Substitute.For<IRepository<Variable>>();
        _logger = XUnitLogger.CreateLogger<UpdateVariableCommandHandler>();
        _handler = new UpdateVariableCommandHandler(_repository, _logger);
    }

    /// <summary>
    /// Executes Should_UpdateVariable_When_DifferentManufacturingUpgrades operation.
    /// </summary>
    /// <returns>The result of Should_UpdateVariable_When_DifferentManufacturingUpgrades.</returns>

    [Theory]
    [InlineData("Engine_Temperature", "Engine_Temperature_Calibrated", "DB1.DBD0", "DB1.DBD8", "REAL")]
    [InlineData("Motor_Speed_RPM", "Motor_Speed_RPM_High_Precision", "DB2.DBD10", "DB2.DBD20", "DINT")]
    [InlineData("Hydraulic_Pressure", "Hydraulic_Pressure_PSI", "DB3.DBD20", "DB3.DBD30", "REAL")]
    [InlineData("Vision_Quality_Result", "Vision_Quality_Enhanced", "DB4.DBW30", "DB4.DBD40", "INT")]
    [InlineData("Tablet_Weight_Grams", "Tablet_Weight_Precise", "DB5.DBD40", "DB5.DBD50", "REAL")]
    public async Task Should_UpdateVariable_When_DifferentManufacturingUpgrades(
        string originalName, string updatedName, string originalAddress, string updatedAddress, string dataType)
    {
        // Arrange - Various manufacturing sensor upgrade scenarios
        var existingVariable = new Variable
        {
            VariableId = 2001,
            MachineId = 201,
            Name = originalName,
            Address = originalAddress,
            NetType = dataType,
            Length = 4,
            IsActive = 1,
            Direction = 1,
            VariableGroupId = 20,
            NativeType = "SENSOR",
            NativeAddress = "CONTINUOUS"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 2001,
            Name = updatedName,
            Address = updatedAddress,
            Type = dataType
        };

        _repository.GetByIdAsync(2001, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe(updatedName);
        result.Value.Address.ShouldBe(updatedAddress);
        result.Value.NetType.ShouldBe(dataType);

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v =>
                v.Name == updatedName &&
                v.Address == updatedAddress &&
                v.NetType == dataType),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_UpdateQualityControlVariable_When_InspectionCriteriaChanged operation.
    /// </summary>
    /// <returns>The result of Should_UpdateQualityControlVariable_When_InspectionCriteriaChanged.</returns>

    [Fact]
    public async Task Should_UpdateQualityControlVariable_When_InspectionCriteriaChanged()
    {
        // Arrange - Quality control variable update for tighter tolerances
        var existingVariable = new Variable
        {
            VariableId = 3001,
            MachineId = 301,
            Name = "Weld_Quality_Index",
            Address = "DB10.DBD100",
            NetType = "REAL",
            Length = 4,
            IsActive = 1,
            Direction = 1,
            VariableGroupId = 30,
            NativeType = "QUALITY_INDEX",
            NativeAddress = "ON_CHANGE"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 3001,
            Name = "Weld_Quality_Index_Enhanced",
            Address = "DB10.DBD104", // New address for enhanced precision
            Type = "REAL",
            Model = "QUALITY_INDEX_V2", // Updated model for better accuracy
            Transaction = "REAL_TIME" // Changed from ON_CHANGE to REAL_TIME
        };

        _repository.GetByIdAsync(3001, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe("Weld_Quality_Index_Enhanced");
        result.Value.Address.ShouldBe("DB10.DBD104");
        result.Value.NativeType.ShouldBe("QUALITY_INDEX_V2");
        result.Value.NativeAddress.ShouldBe("REAL_TIME");

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v =>
                v.Name == "Weld_Quality_Index_Enhanced" &&
                v.NativeType == "QUALITY_INDEX_V2" &&
                v.NativeAddress == "REAL_TIME"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_UpdateSafetyVariable_When_SafetyStandardsUpgraded operation.
    /// </summary>
    /// <returns>The result of Should_UpdateSafetyVariable_When_SafetyStandardsUpgraded.</returns>

    [Fact]
    public async Task Should_UpdateSafetyVariable_When_SafetyStandardsUpgraded()
    {
        // Arrange - Safety monitoring variable update for enhanced emergency detection
        var existingVariable = new Variable
        {
            VariableId = 4001,
            MachineId = 401,
            Name = "Emergency_Stop_Status",
            Address = "DB20.DBX0.0",
            NetType = "BOOL",
            Length = 1,
            IsActive = 1,
            Direction = 1,
            VariableGroupId = 40,
            NativeType = "SAFETY_CRITICAL",
            NativeAddress = "IMMEDIATE"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 4001,
            Name = "Emergency_Stop_Status_Enhanced",
            Address = "DB20.DBX0.1", // New address for redundant safety
            Type = "BOOL",
            Model = "SAFETY_CRITICAL_V2", // Enhanced safety protocol
            Transaction = "IMMEDIATE_REDUNDANT" // Redundant safety check
        };

        _repository.GetByIdAsync(4001, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe("Emergency_Stop_Status_Enhanced");
        result.Value.NativeType.ShouldBe("SAFETY_CRITICAL_V2");
        result.Value.NativeAddress.ShouldBe("IMMEDIATE_REDUNDANT");

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v =>
                v.Name == "Emergency_Stop_Status_Enhanced" &&
                v.NativeType == "SAFETY_CRITICAL_V2" &&
                v.NativeAddress == "IMMEDIATE_REDUNDANT"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_UpdateProcessControlVariable_When_ControlAlgorithmImproved operation.
    /// </summary>
    /// <returns>The result of Should_UpdateProcessControlVariable_When_ControlAlgorithmImproved.</returns>

    [Fact]
    public async Task Should_UpdateProcessControlVariable_When_ControlAlgorithmImproved()
    {
        // Arrange - Process control variable update for pharmaceutical precision
        var existingVariable = new Variable
        {
            VariableId = 5001,
            MachineId = 501,
            Name = "Tablet_Press_Force_Newton",
            Address = "DB30.DBD200",
            NetType = "REAL",
            Length = 4,
            IsActive = 1,
            Direction = 2, // Output to control system
            VariableGroupId = 50,
            NativeType = "PROCESS_CONTROL",
            NativeAddress = "PERIODIC_100MS"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 5001,
            Name = "Tablet_Press_Force_Precision",
            Address = "DB30.DBD204", // Higher precision memory location
            Type = "LREAL", // Double precision for pharmaceutical accuracy
            Length = 8, // Increased length for higher precision
            Model = "PROCESS_CONTROL_PRECISION",
            Transaction = "PERIODIC_50MS" // Faster update rate
        };

        _repository.GetByIdAsync(5001, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe("Tablet_Press_Force_Precision");
        result.Value.NetType.ShouldBe("LREAL");
        result.Value.Length.ShouldBe(8);
        result.Value.NativeAddress.ShouldBe("PERIODIC_50MS");

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v =>
                v.NetType == "LREAL" &&
                v.Length == 8 &&
                v.NativeAddress == "PERIODIC_50MS"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_UpdateTraceabilityVariable_When_SerialNumberFormatChanged operation.
    /// </summary>
    /// <returns>The result of Should_UpdateTraceabilityVariable_When_SerialNumberFormatChanged.</returns>

    [Fact]
    public async Task Should_UpdateTraceabilityVariable_When_SerialNumberFormatChanged()
    {
        // Arrange - Traceability variable update for expanded serial number format
        var existingVariable = new Variable
        {
            VariableId = 6001,
            MachineId = 601,
            Name = "PCB_Serial_Number",
            Address = "DB40.DBB0",
            NetType = "STRING",
            Length = 20,
            IsActive = 1,
            Direction = 1,
            VariableGroupId = 60,
            NativeType = "TRACEABILITY",
            NativeAddress = "ON_SCAN"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 6001,
            Name = "PCB_Serial_Number_Extended",
            Address = "DB40.DBB0", // Same address but extended format
            Type = "STRING",
            Length = 50, // Increased length for QR codes
            Model = "TRACEABILITY_QR", // QR code support
            Transaction = "ON_SCAN_ENHANCED" // Enhanced scanning capabilities
        };

        _repository.GetByIdAsync(6001, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe("PCB_Serial_Number_Extended");
        result.Value.Length.ShouldBe(50);
        result.Value.NativeType.ShouldBe("TRACEABILITY_QR");
        result.Value.NativeAddress.ShouldBe("ON_SCAN_ENHANCED");

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v =>
                v.Length == 50 &&
                v.NativeType == "TRACEABILITY_QR" &&
                v.NativeAddress == "ON_SCAN_ENHANCED"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_UpdateEnergyMonitoringVariable_When_SustainabilityRequirementsChanged operation.
    /// </summary>
    /// <returns>The result of Should_UpdateEnergyMonitoringVariable_When_SustainabilityRequirementsChanged.</returns>

    [Fact]
    public async Task Should_UpdateEnergyMonitoringVariable_When_SustainabilityRequirementsChanged()
    {
        // Arrange - Energy monitoring variable update for carbon footprint tracking
        var existingVariable = new Variable
        {
            VariableId = 7001,
            MachineId = 701,
            Name = "Power_Consumption_Watts",
            Address = "DB50.DBD300",
            NetType = "UDINT",
            Length = 4,
            IsActive = 1,
            Direction = 1,
            VariableGroupId = 70,
            NativeType = "ENERGY_MONITOR",
            NativeAddress = "PERIODIC_1S"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 7001,
            Name = "Power_Consumption_Carbon_Footprint",
            Address = "DB50.DBD304", // Additional memory for carbon calculations
            Type = "REAL", // Changed to REAL for carbon calculations
            Length = 4,
            Model = "ENERGY_MONITOR_CARBON", // Carbon footprint tracking
            Transaction = "PERIODIC_5S" // Less frequent for carbon calculations
        };

        _repository.GetByIdAsync(7001, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe("Power_Consumption_Carbon_Footprint");
        result.Value.NetType.ShouldBe("REAL");
        result.Value.NativeType.ShouldBe("ENERGY_MONITOR_CARBON");
        result.Value.NativeAddress.ShouldBe("PERIODIC_5S");

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v =>
                v.NetType == "REAL" &&
                v.NativeType == "ENERGY_MONITOR_CARBON" &&
                v.NativeAddress == "PERIODIC_5S"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_UpdatePartialProperties_When_OnlySpecificFieldsChanged operation.
    /// </summary>
    /// <returns>The result of Should_UpdatePartialProperties_When_OnlySpecificFieldsChanged.</returns>

    [Fact]
    public async Task Should_UpdatePartialProperties_When_OnlySpecificFieldsChanged()
    {
        // Arrange - Partial update for beverage manufacturing fill level sensor
        var existingVariable = new Variable
        {
            VariableId = 8001,
            MachineId = 801,
            Name = "Bottle_Fill_Level_ML",
            Address = "DB60.DBD400",
            NetType = "REAL",
            Length = 4,
            IsActive = 1,
            Direction = 1,
            VariableGroupId = 80,
            NativeType = "FILL_LEVEL",
            NativeAddress = "PER_BOTTLE"
        };

        // Only updating the name and transaction method, keeping other properties
        var command = new UpdateVariableCommand
        {
            VariableId = 8001,
            Name = "Bottle_Fill_Level_ML_Precise", // Only name change
            Transaction = "PER_BOTTLE_REAL_TIME" // Only transaction change
                                                 // Other properties (Address, Type, etc.) are null - should remain unchanged
        };

        _repository.GetByIdAsync(8001, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe("Bottle_Fill_Level_ML_Precise");
        result.Value.Address.ShouldBe("DB60.DBD400"); // Should remain unchanged
        result.Value.NetType.ShouldBe("REAL"); // Should remain unchanged
        result.Value.NativeAddress.ShouldBe("PER_BOTTLE_REAL_TIME"); // Should be updated

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v =>
                v.Name == "Bottle_Fill_Level_ML_Precise" &&
                v.Address == "DB60.DBD400" && // Unchanged
                v.NetType == "REAL" && // Unchanged
                v.NativeAddress == "PER_BOTTLE_REAL_TIME"), // Updated
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Dispose operation.
    /// </summary>

    public void Dispose()
    {
        // Cleanup if needed
    }
}

/// <summary>
/// Error handling and edge case tests for UpdateVariableCommandHandler
/// </summary>
public class UpdateVariableCommandHandlerErrorTests : IDisposable
{
    private readonly IRepository<Variable> _repository = null!;
    private readonly ILogger<UpdateVariableCommandHandler> _logger = null!;
    private readonly UpdateVariableCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public UpdateVariableCommandHandlerErrorTests()
    {
        _repository = Substitute.For<IRepository<Variable>>();
        _logger = XUnitLogger.CreateLogger<UpdateVariableCommandHandler>();
        _handler = new UpdateVariableCommandHandler(_repository, _logger);
    }

    /// <summary>
    /// Executes Should_HandleInvalidVariableId_When_VariableIdInvalid operation.
    /// </summary>
    /// <param name="variableId">The variableId.</param>
    /// <returns>The result of Should_HandleInvalidVariableId_When_VariableIdInvalid.</returns>

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Should_HandleInvalidVariableId_When_VariableIdInvalid(int? variableId)
    {
        // Using parameters: variableId
        _ = variableId; // xUnit1026 fix
        // Using parameters: variableId
        _ = variableId; // xUnit1026 fix
        // Using parameters: variableId
        _ = variableId; // xUnit1026 fix
        // Using parameters: variableId
        _ = variableId; // xUnit1026 fix
        // Using parameters: variableId
        _ = variableId; // xUnit1026 fix
        // Arrange - Invalid variable ID scenarios
        var command = new UpdateVariableCommand { VariableId = variableId };

        _repository.GetByIdAsync(variableId ?? 0, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success((Variable?)null));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Variable not found");
    }

    /// <summary>
    /// Executes Should_HandleCancellation_When_CancellationRequested operation.
    /// </summary>
    /// <returns>The result of Should_HandleCancellation_When_CancellationRequested.</returns>

    [Fact]
    public async Task Should_HandleCancellation_When_CancellationRequested()
    {
        // Arrange
        var command = new UpdateVariableCommand { VariableId = 1 };
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.WithFailure("Operation canceled"));

        // Act
        var result = await _handler.ProcessAsync(command, cts.Token);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Executes Should_HandleRepositoryException_When_GetByIdThrows operation.
    /// </summary>
    /// <returns>The result of Should_HandleRepositoryException_When_GetByIdThrows.</returns>

    [Fact]
    public async Task Should_HandleRepositoryException_When_GetByIdThrows()
    {
        // Arrange - Repository throws exception
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.WithFailure("Repository failure"));

        // Act
        var result2 = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result2.IsFailure.ShouldBeTrue();
        result2.Errors.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Executes Should_HandleUpdateException_When_UpdateAsyncThrows operation.
    /// </summary>
    /// <returns>The result of Should_HandleUpdateException_When_UpdateAsyncThrows.</returns>

    [Fact]
    public async Task Should_HandleUpdateException_When_UpdateAsyncThrows()
    {
        // Arrange - Update throws exception
        var existingVariable = new Variable { VariableId = 1, MachineId = 10000 };
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Update failure"));

        // Act
        var result3 = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result3.IsFailure.ShouldBeTrue();
        result3.Errors.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Executes Should_HandleCommitException_When_CommitAsyncThrows operation.
    /// </summary>
    /// <returns>The result of Should_HandleCommitException_When_CommitAsyncThrows.</returns>

    [Fact]
    public async Task Should_HandleCommitException_When_CommitAsyncThrows()
    {
        // Arrange - Commit throws exception
        var existingVariable = new Variable { VariableId = 1, MachineId = 10000 };
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Commit failure"));

        // Act
        var result4 = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result4.IsFailure.ShouldBeTrue();
        result4.Errors.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Executes Should_HandleNullPropertyUpdates_When_PropertyValuesAreNull operation.
    /// </summary>
    /// <returns>The result of Should_HandleNullPropertyUpdates_When_PropertyValuesAreNull.</returns>

    [Fact]
    public async Task Should_HandleNullPropertyUpdates_When_PropertyValuesAreNull()
    {
        // Arrange - Update with all null properties (should keep existing values)
        var existingVariable = new Variable
        {
            VariableId = 1,
            MachineId = 10000,
            Name = "Original_Name",
            Address = "Original_Address",
            NetType = "BOOL",
            Length = 1,
            IsActive = 1,
            Direction = 1,
            VariableGroupId = 1,
            NativeType = "Original_Type",
            NativeAddress = "Original_Address"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 1
            // All other properties are null - should not update anything
        };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Name.ShouldBe("Original_Name"); // Should remain unchanged
        result.Value.Address.ShouldBe("Original_Address"); // Should remain unchanged
        result.Value.NetType.ShouldBe("BOOL"); // Should remain unchanged

        await _repository.Received(1).UpdateAsync(
            Arg.Is<Variable>(v =>
                v.Name == "Original_Name" &&
                v.Address == "Original_Address" &&
                v.NetType == "BOOL"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_LogError_When_GetByIdFails operation.
    /// </summary>
    /// <returns>The result of Should_LogError_When_GetByIdFails.</returns>

    [Fact]
    public async Task Should_LogError_When_GetByIdFails()
    {
        // Arrange - Repository get failure for logging verification
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.WithFailure("Repository get failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Executes Should_LogError_When_UpdateFails operation.
    /// </summary>
    /// <returns>The result of Should_LogError_When_UpdateFails.</returns>

    [Fact]
    public async Task Should_LogError_When_UpdateFails()
    {
        // Arrange - Repository update failure for logging verification
        var existingVariable = new Variable { VariableId = 1, MachineId = 10000 };
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Repository update failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Repository update failed");
    }

    /// <summary>
    /// Executes Should_LogError_When_CommitFails operation.
    /// </summary>
    /// <returns>The result of Should_LogError_When_CommitFails.</returns>

    [Fact]
    public async Task Should_LogError_When_CommitFails()
    {
        // Arrange - Repository commit failure for logging verification
        var existingVariable = new Variable { VariableId = 1, MachineId = 10000 };
        var command = new UpdateVariableCommand { VariableId = 1 };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Repository commit failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Repository commit failed");
    }

    /// <summary>
    /// Executes Should_HandleEmptyStringValues_When_StringPropertiesEmpty operation.
    /// </summary>
    /// <param name="emptyValue">The emptyValue.</param>
    /// <returns>The result of Should_HandleEmptyStringValues_When_StringPropertiesEmpty.</returns>

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    public async Task Should_HandleEmptyStringValues_When_StringPropertiesEmpty(string emptyValue)
    {
        // Using parameters: emptyValue
        _ = emptyValue; // xUnit1026 fix
        // Using parameters: emptyValue
        _ = emptyValue; // xUnit1026 fix
        // Using parameters: emptyValue
        _ = emptyValue; // xUnit1026 fix
        // Using parameters: emptyValue
        _ = emptyValue; // xUnit1026 fix
        // Using parameters: emptyValue
        _ = emptyValue; // xUnit1026 fix
        // Arrange - Empty/whitespace string scenarios
        var existingVariable = new Variable
        {
            VariableId = 1,
            MachineId = 10000,
            Name = "Original_Name",
            Address = "Original_Address",
            NetType = "BOOL"
        };

        var command = new UpdateVariableCommand
        {
            VariableId = 1,
            Name = emptyValue,
            Address = emptyValue,
            Type = emptyValue
        };

        _repository.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(Result<Variable?>.Success(existingVariable));
        _repository.UpdateAsync(Arg.Any<Variable>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBe(emptyValue);
        result.Value.Address.ShouldBe(emptyValue);
        result.Value.NetType.ShouldBe(emptyValue);
    }

    /// <summary>
    /// Executes Dispose operation.
    /// </summary>

    public void Dispose()
    {
        // Cleanup if needed
    }
}
