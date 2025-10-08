using IndTrace.Application.BarCodes.Commands.Restore;

namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Basic tests for RestoreBarCodeCommandHandler focusing on constructor validation and simple scenarios
/// </summary>
public class RestoreBarCodeCommandHandlerBasicTests : IDisposable
{
    private readonly IRepository<BarCode> _barCodeRepository = null!;
    private readonly IRepository<TaskGatewayRequest> _taskGatewayRequestRepository = null!;
    private readonly IRepository<Cycle> _cycleRepository = null!;
    private readonly IDateTimeMachine _dateTimeMachine = null!;
    private readonly RestoreBarCodeCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public RestoreBarCodeCommandHandlerBasicTests()
    {
        _barCodeRepository = Substitute.For<IRepository<BarCode>>();
        _taskGatewayRequestRepository = Substitute.For<IRepository<TaskGatewayRequest>>();
        _cycleRepository = Substitute.For<IRepository<Cycle>>();
        _dateTimeMachine = Substitute.For<IDateTimeMachine>();

        _handler = new RestoreBarCodeCommandHandler(
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
        var handler = new RestoreBarCodeCommandHandler(
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
    // 		Should.Throw<ArgumentNullException>(() => new RestoreBarCodeCommandHandler(
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
    // 		Should.Throw<ArgumentNullException>(() => new RestoreBarCodeCommandHandler(
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
    // 		Should.Throw<ArgumentNullException>(() => new RestoreBarCodeCommandHandler(
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
    // 		Should.Throw<ArgumentNullException>(() => new RestoreBarCodeCommandHandler(
    // 			_barCodeRepository,
    // 			_taskGatewayRequestRepository,
    // 			_cycleRepository,
    // 			nullDateTimeMachine!));
    // 	}
    /// <summary>
    /// Executes Should_RestoreBarCode_When_ValidLabelProvided operation.
    /// </summary>
    /// <returns>The result of Should_RestoreBarCode_When_ValidLabelProvided.</returns>

    [Fact]
    public async Task Should_RestoreBarCode_When_ValidLabelProvided()
    {
        // Arrange - Ford F-150 engine block restoration scenario
        const string label = "F150-ENG-20240315-001";
        var command = new RestoreBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1001,
            Label = label,
            MachineId = 10001,
            FlowStatus = FlowStatus.Rejected,
            PartStatus = PartStatus.NOk,
            CreatedOn = currentTime.AddHours(-2),
            ModifiedOn = currentTime.AddHours(-1)
        };

        var existingCycle = new Cycle
        {
            CycleId = 2001,
            BarCodeId = 1001,
            MachineId = 10001,
            CycleStatus = CycleStatus.FinishedNok,
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
        var command = new RestoreBarCodeCommand { Label = nonExistentLabel };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success((BarCode?)null));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain($"BarCode not found {nonExistentLabel} not found");
    }

    /// <summary>
    /// Executes Should_HandleRepositoryFailure_When_BarCodeRepositoryFails operation.
    /// </summary>
    /// <returns>The result of Should_HandleRepositoryFailure_When_BarCodeRepositoryFails.</returns>

    [Fact]
    public async Task Should_HandleRepositoryFailure_When_BarCodeRepositoryFails()
    {
        // Arrange - Database connection failure scenario
        const string label = "BMW-X5-TRANSMISSION-20240315-001";
        var command = new RestoreBarCodeCommand { Label = label };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.WithFailure("Database connection timeout"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldNotBeEmpty();
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
/// Manufacturing scenario tests for RestoreBarCodeCommandHandler with complex production workflows
/// </summary>
public class RestoreBarCodeCommandHandlerManufacturingTests : IDisposable
{
    private readonly IRepository<BarCode> _barCodeRepository = null!;
    private readonly IRepository<TaskGatewayRequest> _taskGatewayRequestRepository = null!;
    private readonly IRepository<Cycle> _cycleRepository = null!;
    private readonly IDateTimeMachine _dateTimeMachine = null!;
    private readonly RestoreBarCodeCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public RestoreBarCodeCommandHandlerManufacturingTests()
    {
        _barCodeRepository = Substitute.For<IRepository<BarCode>>();
        _taskGatewayRequestRepository = Substitute.For<IRepository<TaskGatewayRequest>>();
        _cycleRepository = Substitute.For<IRepository<Cycle>>();
        _dateTimeMachine = Substitute.For<IDateTimeMachine>();

        _handler = new RestoreBarCodeCommandHandler(
            _barCodeRepository,
            _taskGatewayRequestRepository,
            _cycleRepository,
            _dateTimeMachine);
    }

    /// <summary>
    /// Executes Should_RestoreBarCode_When_DifferentManufacturingScenarios operation.
    /// </summary>
    /// <returns>The result of Should_RestoreBarCode_When_DifferentManufacturingScenarios.</returns>

    [Theory]
    [InlineData("F150-ENGINE-BLOCK-2024-001", 101, "Ford F-150 Engine Block")]
    [InlineData("TESLA-MODELY-MOTOR-2024-002", 201, "Tesla Model Y Electric Motor")]
    [InlineData("BMW-X5-GEARBOX-2024-003", 301, "BMW X5 Transmission")]
    [InlineData("IPHONE15-PCB-MAIN-2024-004", 401, "iPhone 15 Main PCB")]
    [InlineData("PHARMA-ASPIRIN-325MG-2024-005", 501, "Aspirin 325mg Tablet")]
    public async Task Should_RestoreBarCode_When_DifferentManufacturingScenarios(
        string label, int machineId, string description)
    {

        var logger = XUnitLogger.CreateLogger<RestoreBarCodeCommandHandlerManufacturingTests>();
        logger.LogInformation("Testing scenario: {description} with label={label}, machineId={machineId}",
            description, label, machineId);

        // Arrange - Various manufacturing restoration scenarios
        var command = new RestoreBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1000 + machineId,
            Label = label,
            MachineId = machineId,
            FlowStatus = FlowStatus.Rejected,
            PartStatus = PartStatus.NOk,
            CreatedOn = currentTime.AddHours(-2),
            ModifiedOn = currentTime.AddHours(-1)
        };

        var existingCycle = new Cycle
        {
            CycleId = 2000 + machineId,
            BarCodeId = 1000 + machineId,
            MachineId = machineId,
            CycleStatus = CycleStatus.FinishedNok,
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
                t.GatewayTask == GatewayTask.RejectPartAsync),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_HandleNoCycleFound_When_CycleRepositoryReturnsNull operation.
    /// </summary>
    /// <returns>The result of Should_HandleNoCycleFound_When_CycleRepositoryReturnsNull.</returns>

    [Fact]
    public async Task Should_HandleNoCycleFound_When_CycleRepositoryReturnsNull()
    {
        // Arrange - BarCode exists but no associated cycle (edge case in manufacturing)
        const string label = "VOLVO-XC90-HYBRID-2024-001";
        var command = new RestoreBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1501,
            Label = label,
            MachineId = 501,
            FlowStatus = FlowStatus.Rejected,
            PartStatus = PartStatus.NOk
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        // No cycle found
        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success((Cycle?)null));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodeId.ShouldBe(1501);

        // Should still update BarCode
        await _barCodeRepository.Received(1).UpdateAsync(
            Arg.Is<BarCode>(b => b.FlowStatus == FlowStatus.Rejected),
            Arg.Any<CancellationToken>());

        // Should NOT add TaskGatewayRequest when no cycle found
        await _taskGatewayRequestRepository.DidNotReceive().AddAsync(
            Arg.Any<TaskGatewayRequest>(),
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
        var command = new RestoreBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1601,
            Label = label,
            MachineId = 601,
            FlowStatus = FlowStatus.Rejected,
            PartStatus = PartStatus.NOk
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.WithFailure("Cycle repository connection failed"));

        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert - Should still succeed as cycle is optional for restore operation
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodeId.ShouldBe(1601);

        // Should still update BarCode even if cycle lookup fails
        await _barCodeRepository.Received(1).UpdateAsync(
            Arg.Is<BarCode>(b => b.FlowStatus == FlowStatus.Rejected),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_RestoreWithCorrectFlowStatusChange_When_PreviouslyRejected operation.
    /// </summary>
    /// <returns>The result of Should_RestoreWithCorrectFlowStatusChange_When_PreviouslyRejected.</returns>

    [Fact]
    public async Task Should_RestoreWithCorrectFlowStatusChange_When_PreviouslyRejected()
    {
        // Arrange - Pharmaceutical manufacturing quality control restoration
        const string label = "PHARMA-INSULIN-VIAL-2024-001";
        var command = new RestoreBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var rejectedBarCode = new BarCode
        {
            BarCodeId = 1701,
            Label = label,
            MachineId = 701,
            FlowStatus = FlowStatus.Rejected,  // Previously rejected
            PartStatus = PartStatus.NOk,      // Quality issue
            CreatedOn = currentTime.AddHours(-4),
            ModifiedOn = currentTime.AddHours(-2)
        };

        var associatedCycle = new Cycle
        {
            CycleId = 2701,
            BarCodeId = 1701,
            MachineId = 701,
            CycleStatus = CycleStatus.FinishedNok,
            StartedOn = currentTime.AddHours(-3),
            FinishedOn = currentTime.AddHours(-2)
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(rejectedBarCode));

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
                t.PartStatus == PartStatus.NOk &&
                t.FlowStatus == FlowStatus.Rejected &&
                t.ResultValidation == ResultValidation.None &&
                t.GatewayTask == GatewayTask.RejectPartAsync &&
                t.TimeStamp == currentTime.ToLocalTime()),
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
/// Error handling and edge case tests for RestoreBarCodeCommandHandler
/// </summary>
public class RestoreBarCodeCommandHandlerErrorTests : IDisposable
{
    private readonly IRepository<BarCode> _barCodeRepository = null!;
    private readonly IRepository<TaskGatewayRequest> _taskGatewayRequestRepository = null!;
    private readonly IRepository<Cycle> _cycleRepository = null!;
    private readonly IDateTimeMachine _dateTimeMachine = null!;
    private readonly RestoreBarCodeCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public RestoreBarCodeCommandHandlerErrorTests()
    {
        _barCodeRepository = Substitute.For<IRepository<BarCode>>();
        _taskGatewayRequestRepository = Substitute.For<IRepository<TaskGatewayRequest>>();
        _cycleRepository = Substitute.For<IRepository<Cycle>>();
        _dateTimeMachine = Substitute.For<IDateTimeMachine>();

        _handler = new RestoreBarCodeCommandHandler(
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
        var command = new RestoreBarCodeCommand { Label = invalidLabel! };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success((BarCode?)null));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 20/08/2025
        //Reason: Fix Result<T> pattern violation - check IsSuccess before accessing Value, and remove invalid Value assertion for failed results
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
        var command = new RestoreBarCodeCommand { Label = label };
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
        const string label = "MERCEDES-S500-ENGINE-2024-001";
        var command = new RestoreBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1801,
            Label = label,
            MachineId = 801,
            FlowStatus = FlowStatus.Rejected,
            PartStatus = PartStatus.NOk
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

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
        const string label = "ROLLSROYCE-GHOST-ENGINE-2024-001";
        var command = new RestoreBarCodeCommand { Label = label };
        var currentTime = DateTime.UtcNow;

        var existingBarCode = new BarCode
        {
            BarCodeId = 1901,
            Label = label,
            MachineId = 901,
            FlowStatus = FlowStatus.Rejected,
            PartStatus = PartStatus.NOk
        };

        var existingCycle = new Cycle
        {
            CycleId = 2901,
            BarCodeId = 1901,
            MachineId = 901,
            CycleStatus = CycleStatus.FinishedNok
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
        const string specialLabel = "F150-ENG-2024/001_SPEC-V2.1";
        var command = new RestoreBarCodeCommand { Label = specialLabel };

        var existingBarCode = new BarCode
        {
            BarCodeId = 2001,
            Label = specialLabel,
            MachineId = 100001,
            FlowStatus = FlowStatus.Rejected,
            PartStatus = PartStatus.NOk
        };

        _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
            .Returns(Result<BarCode?>.Success(existingBarCode));

        _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
            .Returns(Result<Cycle?>.Success((Cycle?)null));

        _dateTimeMachine.Now.Returns(DateTime.UtcNow);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 20/08/2025
        //Reason: Fix assertion order - check IsSuccess before accessing Value properties
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Label.ShouldBe(specialLabel);

        // Verify the specification was created correctly with special characters
        await _barCodeRepository.Received(1).FirstOrDefaultAsync(
            Arg.Is<Specification<BarCode>>(spec => spec != null),
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
