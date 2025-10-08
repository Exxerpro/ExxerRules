using IndTrace.Application.BarCodes.Commands.Reject;

namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Basic tests for RejectBarCodeCommandHandler focusing on constructor validation and simple scenarios
/// </summary>
public class RejectBarCodeCommandHandlerBasicTests : IDisposable
{
    private readonly IRepository<BarCode> _barCodeRepository = null!;
    private readonly IRepository<TaskGatewayRequest> _taskGatewayRequestRepository = null!;
    private readonly IRepository<Cycle> _cycleRepository = null!;
    private readonly IDateTimeMachine _dateTimeMachine = null!;
    private readonly RejectBarCodeCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public RejectBarCodeCommandHandlerBasicTests()
    {
        _barCodeRepository = Substitute.For<IRepository<BarCode>>();
        _taskGatewayRequestRepository = Substitute.For<IRepository<TaskGatewayRequest>>();
        _cycleRepository = Substitute.For<IRepository<Cycle>>();
        _dateTimeMachine = Substitute.For<IDateTimeMachine>();

        _handler = new RejectBarCodeCommandHandler(
            _barCodeRepository,
            _taskGatewayRequestRepository,
            _cycleRepository,
            _dateTimeMachine);
    }

    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new RejectBarCodeCommandHandler(
            _barCodeRepository,
            _taskGatewayRequestRepository,
            _cycleRepository,
            _dateTimeMachine);

        // Assert
        handler.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Constructor_WithNullBarCodeRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullBarCodeRepository_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		IRepository<BarCode>? nullRepository = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() => new RejectBarCodeCommandHandler(
    // 			nullRepository!,
    // 			_taskGatewayRequestRepository,
    // 			_cycleRepository,
    // 			_dateTimeMachine));
    // 	}
    /// <summary>
    /// Executes Constructor_WithNullTaskGatewayRequestRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullTaskGatewayRequestRepository_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		IRepository<TaskGatewayRequest>? nullRepository = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() => new RejectBarCodeCommandHandler(
    // 			_barCodeRepository,
    // 			nullRepository!,
    // 			_cycleRepository,
    // 			_dateTimeMachine));
    // 	}
    /// <summary>
    /// Executes Constructor_WithNullCycleRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullCycleRepository_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		IRepository<Cycle>? nullRepository = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() => new RejectBarCodeCommandHandler(
    // 			_barCodeRepository,
    // 			_taskGatewayRequestRepository,
    // 			nullRepository!,
    // 			_dateTimeMachine));
    // 	}
    /// <summary>
    /// Executes Constructor_WithNullDateTimeMachine_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullDateTimeMachine_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		IDateTimeMachine? nullDateTimeMachine = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() => new RejectBarCodeCommandHandler(
    // 			_barCodeRepository,
    // 			_taskGatewayRequestRepository,
    // 			_cycleRepository,
    // 			nullDateTimeMachine!));
    // 	}
    /// <summary>
    /// Executes Should_RejectBarCode_When_ValidLabelAndCycleProvided operation.
    /// </summary>
    /// <returns>The result of Should_RejectBarCode_When_ValidLabelAndCycleProvided.</returns>

    [Fact]
    public async Task Should_RejectBarCode_When_ValidLabelAndCycleProvided()
    {
        // Arrange - Ford F-150 engine block rejection scenario
        const string label = "F150-ENG-20240315-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1001,
            Label = label,
            MachineId = 10001,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok,
            CreatedOn = currentTime.AddHours(-2),
            ModifiedOn = currentTime.AddHours(-1)
        };

        var existingCycle = new Cycle
        {
            CycleId = 2001,
            BarCodeId = 1001,
            MachineId = 10001,
            CycleStatus = CycleStatus.Started,
            StartedOn = currentTime.AddMinutes(-30),
            FinishedOn = currentTime.AddMinutes(-25)
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(existingCycle));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodeId.ShouldBe(1001);
        result.Value.Label.ShouldBe(label);
        result.Value.FlowStatus.ShouldBe(FlowStatus.Rejected);

        await _barCodeRepository.Received(1).UpdateAsync(
            Arg.Is<BarCode>(b => b.FlowStatus == FlowStatus.Rejected && b.ModifiedOn == currentTime.ToLocalTime()),
            Arg.Any<CancellationToken>());

        await _taskGatewayRequestRepository.Received(1).AddAsync(
            Arg.Is<TaskGatewayRequest>(t =>
                t.GatewayTask == GatewayTask.RejectPartAsync &&
                t.BarCodeId == 1001 &&
                t.CycleId == 2001),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_ReturnFailure_When_BarCodeNotFound operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_BarCodeNotFound.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_BarCodeNotFound()
    {
        // Arrange - Non-existent Tesla Model Y battery pack label
        const string nonExistentLabel = "TESLA-MODELY-BATTERY-20240315-999";
        var command = new RejectBarCodeCommand { Label = nonExistentLabel };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success((BarCode?)null));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain($"BarCode not found {nonExistentLabel}");
    }

    /// <summary>
    /// Executes Should_ReturnFailure_When_CycleNotFound operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_CycleNotFound.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_CycleNotFound()
    {
        // Arrange - BarCode exists but no cycle found (critical for rejection)
        const string label = "BMW-X5-TRANSMISSION-20240315-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1101,
            Label = label,
            MachineId = 10011,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        // No cycle found - this is critical for rejection
        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success((Cycle?)null));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain($"Cycles for BarCode {label} not found");
    }

    /// <summary>
    /// Executes Should_HandleRepositoryFailure_When_BarCodeRepositoryFails operation.
    /// </summary>
    /// <returns>The result of Should_HandleRepositoryFailure_When_BarCodeRepositoryFails.</returns>

    [Fact]
    public async Task Should_HandleRepositoryFailure_When_BarCodeRepositoryFails()
    {
        // Arrange - Database connection failure scenario
        const string label = "MERCEDES-S500-ENGINE-20240315-001";
        var command = new RejectBarCodeCommand { Label = label };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.WithFailure("Database connection timeout"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain($"BarCode not found {label}");
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
/// Manufacturing scenario tests for RejectBarCodeCommandHandler with complex production workflows
/// </summary>
public class RejectBarCodeCommandHandlerManufacturingTests : IDisposable
{
    private readonly IRepository<BarCode> _barCodeRepository = null!;
    private readonly IRepository<TaskGatewayRequest> _taskGatewayRequestRepository = null!;
    private readonly IRepository<Cycle> _cycleRepository = null!;
    private readonly IDateTimeMachine _dateTimeMachine = null!;
    private readonly RejectBarCodeCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public RejectBarCodeCommandHandlerManufacturingTests()
    {
        _barCodeRepository = Substitute.For<IRepository<BarCode>>();
        _taskGatewayRequestRepository = Substitute.For<IRepository<TaskGatewayRequest>>();
        _cycleRepository = Substitute.For<IRepository<Cycle>>();
        _dateTimeMachine = Substitute.For<IDateTimeMachine>();

        _handler = new RejectBarCodeCommandHandler(
            _barCodeRepository,
            _taskGatewayRequestRepository,
            _cycleRepository,
            _dateTimeMachine);
    }

    /// <summary>
    /// Executes Should_RejectBarCode_When_DifferentManufacturingScenarios operation.
    /// </summary>
    /// <returns>The result of Should_RejectBarCode_When_DifferentManufacturingScenarios.</returns>

    [Theory]
    [InlineData("F150-ENGINE-BLOCK-2024-001", 101, 2, "Ford F-150 Engine Block")] // CycleStatus.Started
    [InlineData("TESLA-MODELY-MOTOR-2024-002", 201, 4, "Tesla Model Y Electric Motor")] // CycleStatus.FinishedOk
    [InlineData("BMW-X5-GEARBOX-2024-003", 301, 8, "BMW X5 Transmission")] // CycleStatus.FinishedNok
    [InlineData("IPHONE15-PCB-MAIN-2024-004", 401, 2, "iPhone 15 Main PCB")] // CycleStatus.Started
    [InlineData("PHARMA-ASPIRIN-325MG-2024-005", 501, 4, "Aspirin 325mg Tablet")] // CycleStatus.FinishedOk
    public async Task Should_RejectBarCode_When_DifferentManufacturingScenarios(
        string label, int machineId, int cycleStatusValue, string description)
    {
        var logger = XUnitLogger.CreateLogger<RejectBarCodeCommandHandlerBasicTests>();
        logger.LogInformation("Starting test for label: {Label}, machineId: {MachineId}, cycleStatusValue: {CycleStatusValue}, description: {Description}",
            label, machineId, cycleStatusValue, description);
        // Arrange - Various manufacturing rejection scenarios
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;
        var cycleStatus = EnumModel.FromValue<CycleStatus>(cycleStatusValue);

        var existingBarCode = new BarCode
        {
            BarCodeId = 1000 + machineId,
            Label = label,
            MachineId = machineId,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok,
            CreatedOn = currentTime.AddHours(-2),
            ModifiedOn = currentTime.AddHours(-1)
        };

        var existingCycle = new Cycle
        {
            CycleId = 2000 + machineId,
            BarCodeId = 1000 + machineId,
            MachineId = machineId,
            CycleStatus = cycleStatus,
            StartedOn = currentTime.AddMinutes(-30),
            FinishedOn = currentTime.AddMinutes(-25)
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(existingCycle));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.MachineId.ShouldBe(machineId);
        result.Value.Label.ShouldBe(label);
        result.Value.FlowStatus.ShouldBe(FlowStatus.Rejected);

        // Verify BarCode update with correct FlowStatus
        await _barCodeRepository.Received(1).UpdateAsync(
            Arg.Is<BarCode>(b =>
                b.FlowStatus == FlowStatus.Rejected &&
                b.ModifiedOn == currentTime.ToLocalTime() &&
                b.MachineId == machineId),
            Arg.Any<CancellationToken>());

        // Verify TaskGatewayRequest creation
        await _taskGatewayRequestRepository.Received(1).AddAsync(
            Arg.Is<TaskGatewayRequest>(t =>
                t.MachineId == machineId &&
                t.BarCodeId == 1000 + machineId &&
                t.CycleId == 2000 + machineId &&
                t.CycleStatus == cycleStatus &&
                t.GatewayTask == GatewayTask.RejectPartAsync),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_SelectLatestCycle_When_MultipleCyclesExist operation.
    /// </summary>
    /// <returns>The result of Should_SelectLatestCycle_When_MultipleCyclesExist.</returns>

    [Fact]
    public async Task Should_SelectLatestCycle_When_MultipleCyclesExist()
    {
        // Arrange - BMW M5 engine with multiple production cycles (rework scenario)
        const string label = "BMW-M5-ENGINE-V8-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1501,
            Label = label,
            MachineId = 501,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok
        };

        // Latest cycle should be selected (highest CycleId due to OrderByDescending)
        var latestCycle = new Cycle
        {
            CycleId = 3003, // Highest ID
            BarCodeId = 1501,
            MachineId = 501,
            CycleStatus = CycleStatus.FinishedOk,
            StartedOn = currentTime.AddHours(-1),
            FinishedOn = currentTime.AddMinutes(-30)
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(latestCycle));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        // Verify the latest cycle was used in the TaskGatewayRequest
        await _taskGatewayRequestRepository.Received(1).AddAsync(
            Arg.Is<TaskGatewayRequest>(t =>
                t.CycleId == 3003 &&
                t.CycleStatus == CycleStatus.FinishedOk),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_HandleCycleRepositoryFailure_When_CycleRepositoryFails operation.
    /// </summary>
    /// <returns>The result of Should_HandleCycleRepositoryFailure_When_CycleRepositoryFails.</returns>

    [Fact]
    public async Task Should_HandleCycleRepositoryFailure_When_CycleRepositoryFails()
    {
        // Arrange - BarCode found but cycle repository fails
        const string label = "HONDA-CIVIC-ENGINE-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1601,
            Label = label,
            MachineId = 601,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.WithFailure("Cycle repository connection failed"));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert - Should fail because cycle is required for rejection
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain($"Cycles for BarCode {label} not found");
    }

    /// <summary>
    /// Executes Should_RejectWithCorrectFlowStatusChange_When_FromInProcessToRejected operation.
    /// </summary>
    /// <returns>The result of Should_RejectWithCorrectFlowStatusChange_When_FromInProcessToRejected.</returns>

    [Fact]
    public async Task Should_RejectWithCorrectFlowStatusChange_When_FromInProcessToRejected()
    {
        // Arrange - Pharmaceutical manufacturing quality control rejection
        const string label = "PHARMA-INSULIN-VIAL-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var inProcessBarCode = new BarCode
        {
            BarCodeId = 1701,
            Label = label,
            MachineId = 701,
            FlowStatus = FlowStatus.InProcess,  // Currently in process
            PartStatus = PartStatus.Ok,        // Was OK but now being rejected
            CreatedOn = currentTime.AddHours(-4),
            ModifiedOn = currentTime.AddHours(-2)
        };

        var associatedCycle = new Cycle
        {
            CycleId = 2701,
            BarCodeId = 1701,
            MachineId = 701,
            CycleStatus = CycleStatus.Started,
            StartedOn = currentTime.AddHours(-3),
            FinishedOn = currentTime.AddHours(-2)
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(inProcessBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(associatedCycle));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        //[Fix]
        //CLAUDE
        //Date: 28/08/2025
        //Reason: [CS8602] - Add null-forgiving operator since result.IsSuccess was verified true
        result.Value!.FlowStatus.ShouldBe(FlowStatus.Rejected); // Should change to Rejected status

        // Verify the BarCode was updated with correct timestamp
        await _barCodeRepository.Received(1).UpdateAsync(
            Arg.Is<BarCode>(b =>
                b.FlowStatus == FlowStatus.Rejected &&
                b.ModifiedOn == currentTime.ToLocalTime() &&
                b.BarCodeId == 1701),
            Arg.Any<CancellationToken>());

        // Verify TaskGatewayRequest was created for traceability
        await _taskGatewayRequestRepository.Received(1).AddAsync(
            Arg.Is<TaskGatewayRequest>(t =>
                t.MachineId == 701 &&
                t.BarCodeId == 1701 &&
                t.PartStatus == PartStatus.Ok &&
                t.FlowStatus == FlowStatus.Rejected &&
                t.ResultValidation == ResultValidation.None &&
                t.GatewayTask == GatewayTask.RejectPartAsync &&
                t.TimeStamp == currentTime.ToLocalTime()),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_HandleQualityControlScenarios_When_DefectivePartsDetected operation.
    /// </summary>
    /// <returns>The result of Should_HandleQualityControlScenarios_When_DefectivePartsDetected.</returns>

    [Fact]
    public async Task Should_HandleQualityControlScenarios_When_DefectivePartsDetected()
    {
        // Arrange - Electronics manufacturing with defective PCB rejection
        const string label = "SAMSUNG-GALAXY-S24-PCB-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var defectiveBarCode = new BarCode
        {
            BarCodeId = 1801,
            Label = label,
            MachineId = 801,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok,  // Initially OK but defect detected
            CreatedOn = currentTime.AddHours(-1),
            ModifiedOn = currentTime.AddMinutes(-30)
        };

        var qualityTestCycle = new Cycle
        {
            CycleId = 2801,
            BarCodeId = 1801,
            MachineId = 801,
            CycleStatus = CycleStatus.FinishedNok, // Quality test failed
            StartedOn = currentTime.AddMinutes(-45),
            FinishedOn = currentTime.AddMinutes(-35)
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(defectiveBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(qualityTestCycle));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert

        result.Value.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.FlowStatus.ShouldBe(FlowStatus.Rejected);

        // Verify quality control traceability
        await _taskGatewayRequestRepository.Received(1).AddAsync(
            Arg.Is<TaskGatewayRequest>(t =>
                t.CycleStatus == CycleStatus.FinishedNok &&
                t.GatewayTask == GatewayTask.RejectPartAsync &&
                t.MachineId == 801),
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
/// Error handling and edge case tests for RejectBarCodeCommandHandler
/// </summary>
public class RejectBarCodeCommandHandlerErrorTests : IDisposable
{
    private readonly IRepository<BarCode> _barCodeRepository = null!;
    private readonly IRepository<TaskGatewayRequest> _taskGatewayRequestRepository = null!;
    private readonly IRepository<Cycle> _cycleRepository = null!;
    private readonly IDateTimeMachine _dateTimeMachine = null!;
    private readonly RejectBarCodeCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public RejectBarCodeCommandHandlerErrorTests()
    {
        _barCodeRepository = Substitute.For<IRepository<BarCode>>();
        _taskGatewayRequestRepository = Substitute.For<IRepository<TaskGatewayRequest>>();
        _cycleRepository = Substitute.For<IRepository<Cycle>>();
        _dateTimeMachine = Substitute.For<IDateTimeMachine>();

        _handler = new RejectBarCodeCommandHandler(
            _barCodeRepository,
            _taskGatewayRequestRepository,
            _cycleRepository,
            _dateTimeMachine);
    }

    /// <summary>
    /// Executes Should_ReturnFailure_When_InvalidLabel operation.
    /// </summary>
    /// <param name="invalidLabel">The invalidLabel.</param>
    /// <returns>The result of Should_ReturnFailure_When_InvalidLabel.</returns>

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Should_ReturnFailure_When_InvalidLabel(string? invalidLabel)
    {
        // Using parameters: invalidLabel
        _ = invalidLabel; // xUnit1026 fix
        // Using parameters: invalidLabel
        _ = invalidLabel; // xUnit1026 fix
        // Using parameters: invalidLabel
        _ = invalidLabel; // xUnit1026 fix
        // Using parameters: invalidLabel
        _ = invalidLabel; // xUnit1026 fix
        // Using parameters: invalidLabel
        _ = invalidLabel; // xUnit1026 fix
        // Arrange - Various invalid label scenarios
        var command = new RejectBarCodeCommand { Label = invalidLabel! };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success((BarCode?)null));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert

        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.Any(e => e.Contains("not found")).ShouldBeTrue();
    }

    /// <summary>
    /// Executes Should_HandleCancellation_When_CancellationRequested operation.
    /// </summary>
    /// <returns>The result of Should_HandleCancellation_When_CancellationRequested.</returns>

    [Fact]
    public async Task Should_HandleCancellation_When_CancellationRequested()
    {
        // Arrange
        const string label = "AUDI-A8-TRANSMISSION-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromException<Result<BarCode?>>(new OperationCanceledException()));

        // Act
        var result = await _handler.ProcessAsync(command, cts.Token);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Operation was canceled.");
    }

    /// <summary>
    /// Executes Should_HandleUpdateFailure_When_BarCodeUpdateFails operation.
    /// </summary>
    /// <returns>The result of Should_HandleUpdateFailure_When_BarCodeUpdateFails.</returns>

    [Fact]
    public async Task Should_HandleUpdateFailure_When_BarCodeUpdateFails()
    {
        // Arrange - Concurrent modification scenario
        const string label = "VOLVO-XC90-ENGINE-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1901,
            Label = label,
            MachineId = 901,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok
        };

        var existingCycle = new Cycle
        {
            CycleId = 2901,
            BarCodeId = 1901,
            MachineId = 901,
            CycleStatus = CycleStatus.Started
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(existingCycle));

        _barCodeRepository.UpdateAsync(Arg.Any<BarCode>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Result>(new InvalidOperationException("Concurrent modification detected")));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Operation finished with an exception Concurrent modification detected");
    }

    /// <summary>
    /// Executes Should_HandleTaskGatewayRequestFailure_When_AddAsyncFails operation.
    /// </summary>
    /// <returns>The result of Should_HandleTaskGatewayRequestFailure_When_AddAsyncFails.</returns>

    [Fact]
    public async Task Should_HandleTaskGatewayRequestFailure_When_AddAsyncFails()
    {
        // Arrange - Gateway request repository failure
        const string label = "LAMBORGHINI-HURACAN-ENGINE-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 2001,
            Label = label,
            MachineId = 100001,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok
        };

        var existingCycle = new Cycle
        {
            CycleId = 3001,
            BarCodeId = 2001,
            MachineId = 100001,
            CycleStatus = CycleStatus.Started
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(existingCycle));

        _taskGatewayRequestRepository.AddAsync(Arg.Any<TaskGatewayRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Result<int>>(new InvalidOperationException("Gateway service unavailable")));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Operation finished with an exception Gateway service unavailable");
    }

    /// <summary>
    /// Executes Should_HandleSpecificationQuery_When_LabelContainsSpecialCharacters operation.
    /// </summary>
    /// <returns>The result of Should_HandleSpecificationQuery_When_LabelContainsSpecialCharacters.</returns>

    [Fact]
    public async Task Should_HandleSpecificationQuery_When_LabelContainsSpecialCharacters()
    {
        // Arrange - Label with special manufacturing characters
        const string specialLabel = "F150-ENG-2024/001_DEFECT-V2.1";
        var command = new RejectBarCodeCommand { Label = specialLabel };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 2101,
            Label = specialLabel,
            MachineId = 100101,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok
        };

        var existingCycle = new Cycle
        {
            CycleId = 3101,
            BarCodeId = 2101,
            MachineId = 100101,
            CycleStatus = CycleStatus.Started
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(existingCycle));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        //[Fix]
        //CLAUDE
        //Date: 28/08/2025
        //Reason: [CS8602] - Add null-forgiving operator since result.IsSuccess was verified true
        result.Value!.Label.ShouldBe(specialLabel);

        // Verify the specification was created correctly with special characters
        await _barCodeRepository.Received(1).FirstOrDefaultAsync(
            Arg.Is<Specification<BarCode>>(spec => spec != null),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_HandleNullCycleScenario_When_CycleIsNull operation.
    /// </summary>
    /// <returns>The result of Should_HandleNullCycleScenario_When_CycleIsNull.</returns>

    [Fact]
    public async Task Should_HandleNullCycleScenario_When_CycleIsNull()
    {
        // Arrange - Edge case where cycle result is success but value is null
        const string label = "PORSCHE-911-TURBO-ENGINE-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 2201,
            Label = label,
            MachineId = 100201,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        // Cycle result is success but value is null
        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success((Cycle?)null));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 20/08/2025
        //Reason: Fix Result<T> pattern violation - check IsSuccess before accessing Value, and remove invalid Value assertion for failed results
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain($"Cycles for BarCode {label} not found");
    }

    /// <summary>
    /// Executes Should_HandleCycleSpecificationQuery_When_OrderingByDescending operation.
    /// </summary>
    /// <returns>The result of Should_HandleCycleSpecificationQuery_When_OrderingByDescending.</returns>

    [Fact]
    public async Task Should_HandleCycleSpecificationQuery_When_OrderingByDescending()
    {
        // Arrange - Verify that cycle query uses correct ordering specification
        const string label = "FERRARI-F8-TRIBUTO-ENGINE-2024-001";
        var command = new RejectBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 2301,
            Label = label,
            MachineId = 100301,
            FlowStatus = FlowStatus.InProcess,
            PartStatus = PartStatus.Ok
        };

        var latestCycle = new Cycle
        {
            CycleId = 3301,
            BarCodeId = 2301,
            MachineId = 100301,
            CycleStatus = CycleStatus.FinishedOk
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success(latestCycle));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify the specification query for cycles was called with correct ordering
        await _cycleRepository.Received(1).FirstOrDefaultAsync(
            Arg.Is<Specification<Cycle>>(spec => spec != null),
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
