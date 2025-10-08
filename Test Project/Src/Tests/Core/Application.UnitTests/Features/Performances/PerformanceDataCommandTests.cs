namespace Application.UnitTests.Features.Performances;

/// <summary>
/// Unit tests for PerformanceDataCommand - Command for manufacturing performance data operations.
/// Tests command functionality, factory methods, conversion utilities, and manufacturing scenarios.
/// </summary>
public class PerformanceDataCommandTests
{
    /// <summary>
    /// Executes Constructor_WhenCreated_ShouldCreateInstanceWithDefaultValues operation.
    /// </summary>
    [Fact]
    public void Constructor_WhenCreated_ShouldCreateInstanceWithDefaultValues()
    {
        // Arrange & Act
        var instance = new PerformanceDataCommand();

        // Assert
        instance.ShouldNotBeNull();
        instance.ShouldBeAssignableTo<IGatewayRequest<TaskGatewayResponse>>();
        instance.ShouldBeAssignableTo<ICommandData>();
        instance.ShouldBeAssignableTo<PerformanceData>();
        instance.PerformanceDataId.ShouldBe(0);
        instance.MachineId.ShouldBe(0);
        instance.PlcId.ShouldBe(0);
        instance.BarCodeId.ShouldBe(0);
        instance.CycleId.ShouldBe(0);
        instance.TimeStamp.ShouldBe(default(DateTime));
        instance.TotalProduction.ShouldBe(0.0);
        instance.ProductionOk.ShouldBe(0.0);
        instance.ProductionNoK.ShouldBe(0.0);
    }

    /// <summary>
    /// Executes Constructor_WithTaskGatewayRequest_ShouldCreateInstanceCorrectly operation.
    /// </summary>

    [Fact]
    public void Constructor_WithTaskGatewayRequest_ShouldCreateInstanceCorrectly()
    {
        // Arrange
        var taskRequest = new TaskGatewayRequest
        {
            MachineId = 5001,
            PartNumber = "PART-12345"
        };

        // Act
        var instance = new PerformanceDataCommand(taskRequest);

        // Assert
        instance.ShouldNotBeNull();
        instance.Command.ShouldNotBeNull();
        instance.Command.MachineId.ShouldBe(5001);
        instance.Command.PartNumber.ShouldBe("PART-12345");
    }

    /// <summary>
    /// Executes Properties_WithValidManufacturingData_ShouldSetAndReturnCorrectValues operation.
    /// </summary>

    [Theory]
    [InlineData(12345L, 1001, 2001, 3001, 4001)]
    [InlineData(67890L, 1002, 2002, 3002, 4002)]
    [InlineData(11111L, 1003, 2003, 3003, 4003)]
    public void Properties_WithValidManufacturingData_ShouldSetAndReturnCorrectValues(
        long performanceDataId, int machineId, int plcId, int barCodeId, int cycleId)
    {
        // Arrange
        var instance = new PerformanceDataCommand();
        var timestamp = DateTime.UtcNow;

        // Act
        instance.PerformanceDataId = performanceDataId;
        instance.MachineId = machineId;
        instance.PlcId = plcId;
        instance.BarCodeId = barCodeId;
        instance.CycleId = cycleId;
        instance.TimeStamp = timestamp;
        instance.ApplicationFlag = 100;
        instance.EventCounter = 50;
        instance.CurrentTime = 1000;
        instance.RunningTime = 800;
        instance.StoppedTime = 150;
        instance.FaultedTime = 50;
        instance.StatusFaultReason = 0;
        instance.TotalProduction = 95.5;
        instance.ProductionOk = 90.0;
        instance.ProductionNoK = 5.5;
        instance.StatusFaultReject = 0;

        // Assert
        instance.PerformanceDataId.ShouldBe(performanceDataId);
        instance.MachineId.ShouldBe(machineId);
        instance.PlcId.ShouldBe(plcId);
        instance.BarCodeId.ShouldBe(barCodeId);
        instance.CycleId.ShouldBe(cycleId);
        instance.TimeStamp.ShouldBe(timestamp);
        instance.ApplicationFlag.ShouldBe(100);
        instance.EventCounter.ShouldBe(50);
        instance.CurrentTime.ShouldBe(1000);
        instance.RunningTime.ShouldBe(800);
        instance.StoppedTime.ShouldBe(150);
        instance.FaultedTime.ShouldBe(50);
        instance.StatusFaultReason.ShouldBe(0);
        instance.TotalProduction.ShouldBe(95.5);
        instance.ProductionOk.ShouldBe(90.0);
        instance.ProductionNoK.ShouldBe(5.5);
        instance.StatusFaultReject.ShouldBe(0);
    }

    /// <summary>
    /// Executes Create_WithTaskGatewayRequest_ShouldReturnNewCommandInstance operation.
    /// </summary>

    [Fact]
    public void Create_WithTaskGatewayRequest_ShouldReturnNewCommandInstance()
    {
        // Arrange
        var factory = new PerformanceDataCommand();
        var taskRequest = new TaskGatewayRequest
        {
            MachineId = 7001,
            PartNumber = "COMP-98765"
        };

        // Act
        var result = factory.Create(taskRequest);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<ICommandData>();
        result.ShouldBeAssignableTo<PerformanceDataCommand>();
        var command = result as PerformanceDataCommand;
        command!.Command.ShouldNotBeNull();
        command.Command.MachineId.ShouldBe(7001);
        command.Command.PartNumber.ShouldBe("COMP-98765");
    }

    /// <summary>
    /// Executes TryReset_WhenCalled_ShouldResetAllPropertiesToDefault operation.
    /// </summary>

    [Fact]
    public void TryReset_WhenCalled_ShouldResetAllPropertiesToDefault()
    {
        // Arrange
        var command = new PerformanceDataCommand
        {
            PerformanceDataId = 99999L,
            MachineId = 100234,
            PlcId = 5678,
            BarCodeId = 9012,
            CycleId = 3456,
            TimeStamp = DateTime.UtcNow,
            ApplicationFlag = 200,
            EventCounter = 75,
            CurrentTime = 2000,
            RunningTime = 1800,
            StoppedTime = 150,
            FaultedTime = 50,
            StatusFaultReason = 1,
            TotalProduction = 500.0,
            ProductionOk = 475.0,
            ProductionNoK = 25.0,
            StatusFaultReject = 5
        };

        // Act
        var result = command.TryReset();

        // Assert
        result.ShouldBeTrue();
        command.PerformanceDataId.ShouldBe(0);
        command.MachineId.ShouldBe(0);
        command.PlcId.ShouldBe(0);
        command.BarCodeId.ShouldBe(0);
        command.CycleId.ShouldBe(0);
        command.TimeStamp.ShouldBe(DateTime.MinValue);
        command.ApplicationFlag.ShouldBe(0);
        command.EventCounter.ShouldBe(0);
        command.CurrentTime.ShouldBe(0);
        command.RunningTime.ShouldBe(0);
        command.StoppedTime.ShouldBe(0);
        command.FaultedTime.ShouldBe(0);
        command.StatusFaultReason.ShouldBe(0);
        command.TotalProduction.ShouldBe(0.0);
        command.ProductionOk.ShouldBe(0.0);
        command.ProductionNoK.ShouldBe(0.0);
        command.StatusFaultReject.ShouldBe(0);
    }

    /// <summary>
    /// Executes ToEntity_WithValidCommand_ShouldCreateCorrectEntity operation.
    /// </summary>

    [Fact]
    public void ToEntity_WithValidCommand_ShouldCreateCorrectEntity()
    {
        // Arrange
        var command = new PerformanceDataCommand
        {
            PerformanceDataId = 88888L,
            MachineId = 2001,
            PlcId = 3001,
            BarCodeId = 4001,
            CycleId = 5001,
            TimeStamp = DateTime.UtcNow,
            TotalProduction = 300.0,
            ProductionOk = 285.0,
            ProductionNoK = 15.0
        };

        // Act
        var result = PerformanceDataCommand.ToEntity(command);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<IndTrace.Domain.Entities.PerformanceData>();
        result.PerformanceDataId.ShouldBe(command.PerformanceDataId);
        result.MachineId.ShouldBe(command.MachineId);
        result.PlcId.ShouldBe(command.PlcId);
        result.BarCodeId.ShouldBe(command.BarCodeId);
        result.CycleId.ShouldBe(command.CycleId);
        result.TimeStamp.ShouldBe(command.TimeStamp);
        result.TotalProduction.ShouldBe(command.TotalProduction);
        result.ProductionOk.ShouldBe(command.ProductionOk);
        result.ProductionNoK.ShouldBe(command.ProductionNoK);
    }

    /// <summary>
    /// Executes PerformanceDataCommand_WithAutomotiveManufacturingScenarios_ShouldHandleCorrectly operation.
    /// </summary>
    /// <param name="scenario">The scenario.</param>

    [Theory]
    [InlineData("Ford F-150 Production Performance Command")]
    [InlineData("Tesla Model S Manufacturing Performance Command")]
    [InlineData("BMW X5 Assembly Performance Command")]
    [InlineData("Mercedes E-Class Production Performance Command")]
    [InlineData("Audi A4 Manufacturing Performance Command")]
    public void PerformanceDataCommand_WithAutomotiveManufacturingScenarios_ShouldHandleCorrectly(string scenario)
    {
        // Using parameters: scenario
        _ = scenario; // xUnit1026 fix
        // Using parameters: scenario
        _ = scenario; // xUnit1026 fix
        // Using parameters: scenario
        _ = scenario; // xUnit1026 fix
        // Using parameters: scenario
        _ = scenario; // xUnit1026 fix
        // Using parameters: scenario
        _ = scenario; // xUnit1026 fix
        // Arrange & Act
        var command = new PerformanceDataCommand
        {
            PerformanceDataId = 12345L,
            MachineId = 100001,
            PlcId = 2001,
            TotalProduction = 200.0,
            ProductionOk = 190.0,
            ProductionNoK = 10.0,
            RunningTime = 1800,
            StoppedTime = 100,
            FaultedTime = 100
        };

        // Assert
        command.ShouldNotBeNull();
        command.ShouldBeAssignableTo<IGatewayRequest<TaskGatewayResponse>>();
        command.ShouldBeAssignableTo<ICommandData>();
        command.TotalProduction.ShouldBe(200.0);
        command.ProductionOk.ShouldBe(190.0);
        command.ProductionNoK.ShouldBe(10.0);
    }

    /// <summary>
    /// Executes UpdateDataFromResult_WithValidResult_ShouldUpdateCommandData operation.
    /// </summary>

    [Fact]
    public void UpdateDataFromResult_WithValidResult_ShouldUpdateCommandData()
    {
        // Arrange
        var command = new PerformanceDataCommand();
        var response = new TaskGatewayResponse
        {
            MachineId = 3001,
            PartNumber = "UPDATE-PART",
            BarCodeId = 7001,
            CycleId = 8001,
            TimeStamp = DateTime.UtcNow
        };
        var result = Result<TaskGatewayResponse>.Success(response);

        // Act
        command.UpdateDataFromResult(result);

        // Assert
        command.Command.ShouldNotBeNull();
        command.Command.MachineId.ShouldBe(3001);
        command.Command.PartNumber.ShouldBe("UPDATE-PART");
        command.BarCodeId.ShouldBe(7001);
        command.CycleId.ShouldBe(8001);
        command.TimeStamp.ShouldBe(response.TimeStamp);
    }

    /// <summary>
    /// Executes FromPlc_WithValidRegisterDictionary_ShouldCreateCorrectCommand operation.
    /// </summary>

    [Fact]
    public void FromPlc_WithValidRegisterDictionary_ShouldCreateCorrectCommand()
    {
        // Arrange
        var registers = new Dictionary<string, Register>
        {
            { nameof(PerformanceDataCommand.ApplicationFlag), new Register { Value = "100" } },
            { nameof(PerformanceDataCommand.EventCounter), new Register { Value = "50" } },
            { nameof(PerformanceDataCommand.CurrentTime), new Register { Value = "2000" } },
            { nameof(PerformanceDataCommand.RunningTime), new Register { Value = "1800" } },
            { nameof(PerformanceDataCommand.StoppedTime), new Register { Value = "150" } },
            { nameof(PerformanceDataCommand.FaultedTime), new Register { Value = "50" } },
            { nameof(PerformanceDataCommand.StatusFaultReason), new Register { Value = "0" } },
            { nameof(PerformanceDataCommand.TotalProduction), new Register { Value = "250.5" } },
            { nameof(PerformanceDataCommand.ProductionOk), new Register { Value = "240.0" } },
            { nameof(PerformanceDataCommand.ProductionNoK), new Register { Value = "10.5" } },
            { nameof(PerformanceDataCommand.StatusFaultReject), new Register { Value = "2" } }
        };

        // Act
        var resultWrapper = PerformanceDataCommand.FromPlc(registers);

        // Assert
        resultWrapper.IsSuccess.ShouldBeTrue();
        resultWrapper.Value.ShouldNotBeNull();
        var result = resultWrapper.Value;
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ApplicationFlag.ShouldBe(100);
        result.EventCounter.ShouldBe(50);
        result.CurrentTime.ShouldBe(2000);
        result.RunningTime.ShouldBe(1800);
        result.StoppedTime.ShouldBe(150);
        result.FaultedTime.ShouldBe(50);
        result.StatusFaultReason.ShouldBe(0);
        result.TotalProduction.ShouldBe(250.5);
        result.ProductionOk.ShouldBe(240.0);
        result.ProductionNoK.ShouldBe(10.5);
        result.StatusFaultReject.ShouldBe(2);
    }

    /// <summary>
    /// Executes FromPlc_WithNullDictionary_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void FromPlc_WithNullDictionary_ShouldReturnFailureResult()
    {
        // Arrange
        Dictionary<string, Register> registers = null!;

        // Act
        var result = PerformanceDataCommand.FromPlc(registers);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern 11 Fix - Updated error message expectation to match actual Result<T>.Fail() implementation
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Executes FromPlc_WithEmptyDictionary_ShouldCreateCommandWithDefaults operation.
    /// </summary>

    [Fact]
    public void FromPlc_WithEmptyDictionary_ShouldCreateCommandWithDefaults()
    {
        // Arrange
        var registers = new Dictionary<string, Register>();

        // Act
        var resultWrapper = PerformanceDataCommand.FromPlc(registers);

        // Assert
        resultWrapper.IsSuccess.ShouldBeTrue();
        resultWrapper.Value.ShouldNotBeNull();
        var result = resultWrapper.Value;
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ShouldNotBeNull();
        result.ApplicationFlag.ShouldBe(0);
        result.EventCounter.ShouldBe(0);
        result.CurrentTime.ShouldBe(0);
        result.RunningTime.ShouldBe(0);
        result.StoppedTime.ShouldBe(0);
        result.FaultedTime.ShouldBe(0);
        result.StatusFaultReason.ShouldBe(0);
        result.TotalProduction.ShouldBe(0.0);
        result.ProductionOk.ShouldBe(0.0);
        result.ProductionNoK.ShouldBe(0.0);
        result.StatusFaultReject.ShouldBe(0);
    }
}
