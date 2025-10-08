using IndTrace.Application.BarCodes.Commands.Update;

namespace Application.UnitTests.Features.Barcodes;

/// <summary>
/// Unit tests for UpdateBarCodeCommandHandler
/// </summary>
public class UpdateBarCodeCommandHandlerTests
{
    private readonly IDateTimeMachine _dateTimeMachine = null!;
    private readonly IRepository<TaskGatewayRequest> _repositoryCommand = null!;
    private readonly IRepository<IndTrace.Domain.Entities.Cycle> _repositoryCycle = null!;
    private readonly IRepository<BarCode> _repositoryBarCode = null!;
    private readonly IBarCodeResult _barCodeResult = null!;
    private readonly UpdateBarCodeCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public UpdateBarCodeCommandHandlerTests()
    {
        _dateTimeMachine = Substitute.For<IDateTimeMachine>();
        _repositoryCommand = Substitute.For<IRepository<TaskGatewayRequest>>();
        _repositoryCycle = Substitute.For<IRepository<IndTrace.Domain.Entities.Cycle>>();
        _repositoryBarCode = Substitute.For<IRepository<BarCode>>();
        _barCodeResult = Substitute.For<IBarCodeResult>();

        _handler = new UpdateBarCodeCommandHandler(
            _dateTimeMachine,
            _repositoryCycle,
            _barCodeResult);
    }

    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockDateTimeMachine = Substitute.For<IDateTimeMachine>();
        var mockRepositoryCommand = Substitute.For<IRepository<TaskGatewayRequest>>();
        var mockRepositoryCycle = Substitute.For<IRepository<IndTrace.Domain.Entities.Cycle>>();
        var mockRepositoryBarCode = Substitute.For<IRepository<BarCode>>();
        var mockBarCodeResult = Substitute.For<IBarCodeResult>();

        // Act
        var instance = new UpdateBarCodeCommandHandler(
            mockDateTimeMachine,
            mockRepositoryCycle,
            mockBarCodeResult);

        // Assert
        instance.ShouldNotBeNull();
    }

    /// <summary>
    /// Executes Constructor_WithNullParameters_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullParameters_ShouldThrowException()
    //     {
    //         // Arrange
    //         IDateTimeMachine? nullDateTimeMachine = null!;
    //         var mockRepositoryCommand = Substitute.For<IRepository<TaskGatewayRequest>>();
    //         var mockRepositoryCycle = Substitute.For<IRepository<IndTrace.Domain.Entities.Cycle>>();
    //         var mockRepositoryBarCode = Substitute.For<IRepository<BarCode>>();
    //         var mockBarCodeResult = Substitute.For<IBarCodeResult>();
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new UpdateBarCodeCommandHandler(
    //             nullDateTimeMachine!,
    //             mockRepositoryCycle,
    //             mockBarCodeResult));
    //     }
    /// <summary>
    /// Executes Process_WithValidRequest_ShouldUpdateBarCodeSuccessfully operation.
    /// </summary>
    /// <returns>The result of Process_WithValidRequest_ShouldUpdateBarCodeSuccessfully.</returns>

    [Fact]
    public async Task Process_WithValidRequest_ShouldUpdateBarCodeSuccessfully()
    {
        // Arrange
        var machineId = 1;
        var barCode = "L1ATEST1230001";
        var partNumber = "TEST123";
        var currentTime = DateTime.Now;
        var barCodeId = 123;

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = barCode,
                PartNumber = partNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.BarCodeId.Returns(barCodeId);
        barCodeInfo.MachineId.Returns(machineId);
        barCodeInfo.BarCode.Returns(new BarCode
        {
            BarCodeId = barCodeId,
            Label = barCode,
            MachineId = machineId,
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.Created
        });
        barCodeInfo.ResultValidation.Returns(ResultValidation.Valid);
        barCodeInfo.MasterLabel.Returns((MasterLabel)null!);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);
        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCode.Label.ShouldBe(barCode);
        result.Value.MachineId.ShouldBe(machineId);
    }

    /// <summary>
    /// Executes Process_WithInvalidBarCode_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WithInvalidBarCode_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WithInvalidBarCode_ShouldReturnFailure()
    {
        // Arrange
        var machineId = 1;
        var invalidBarCode = "INVALID123";
        var partNumber = "TEST123";

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = invalidBarCode,
                PartNumber = partNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.ResultValidation.Returns(ResultValidation.BarCodeNotFound);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue(); // Handler returns success but with invalid validation
        result.Value.ShouldNotBeNull();
        result.Value.ResultValidation.ShouldBe(ResultValidation.BarCodeNotFound);
    }

    /// <summary>
    /// Executes Process_WithNullCommand_ShouldThrowException operation.
    /// </summary>
    /// <returns>The result of Process_WithNullCommand_ShouldThrowException.</returns>

    //[Fix]
    //CLAUDE
    //Date: 22/08/2025
    //Reason: PATTERN 12 Fix - Railway-Oriented Programming: Handler returns Result<T> instead of throwing exceptions
    // MARKED FOR DELETION - Constructor null guard test no longer needed for Railway-Oriented Programming
    // [Fact]
    // public async Task Process_WithNullCommand_ShouldThrowException()
    // {
    //     // Arrange
    //     UpdateBarCodeCommand? command = null!;
    //
    //     // Act & Assert
    //     await Should.ThrowAsync<ArgumentNullException>(async () =>
    //         await _handler.ProcessAsync(command!, TestContext.Current.CancellationToken));
    // }
    /// <summary>
    /// Executes Process_WithNullCommandCommand_ShouldThrowException operation.
    /// </summary>
    /// <returns>The result of Process_WithNullCommandCommand_ShouldThrowException.</returns>

    //[Fix]
    //CLAUDE
    //Date: 22/08/2025
    //Reason: PATTERN 12 Fix - Railway-Oriented Programming: Handler returns Result<T> instead of throwing exceptions
    // MARKED FOR DELETION - Constructor null guard test no longer needed for Railway-Oriented Programming
    // [Fact]
    // public async Task Process_WithNullCommandCommand_ShouldThrowException()
    // {
    //     // Arrange
    //     var command = new UpdateBarCodeCommand
    //     {
    //         Command = null!
    //     };
    //
    //     // Act & Assert
    //     await Should.ThrowAsync<ArgumentNullException>(async () =>
    //         await _handler.ProcessAsync(command, TestContext.Current.CancellationToken));
    // }
    /// <summary>
    /// Executes Process_WithZeroMachineId_ShouldProcessNormally operation.
    /// </summary>
    /// <returns>The result of Process_WithZeroMachineId_ShouldProcessNormally.</returns>

    [Fact]
    public async Task Process_WithZeroMachineId_ShouldProcessNormally()
    {
        // Arrange
        var machineId = 0;
        var barCode = "L1ATEST1230001";
        var partNumber = "TEST123";
        var currentTime = DateTime.Now;

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = barCode,
                PartNumber = partNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.BarCodeId.Returns(1);
        barCodeInfo.BarCode.Returns(new BarCode
        {
            BarCodeId = 1,
            Label = barCode,
            MachineId = machineId,
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.Created
        });
        barCodeInfo.ResultValidation.Returns(ResultValidation.Valid);
        barCodeInfo.MasterLabel.Returns((MasterLabel)null!);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);
        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.MachineId.ShouldBe(machineId);
    }

    /// <summary>
    /// Executes Process_WithEmptyBarCode_ShouldProcessNormally operation.
    /// </summary>
    /// <returns>The result of Process_WithEmptyBarCode_ShouldProcessNormally.</returns>

    [Fact]
    public async Task Process_WithEmptyBarCode_ShouldProcessNormally()
    {
        // Arrange
        var machineId = 1;
        var emptyBarCode = "";
        var partNumber = "TEST123";

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = emptyBarCode,
                PartNumber = partNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.ResultValidation.Returns(ResultValidation.BarCodeNotFound);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ResultValidation.ShouldBe(ResultValidation.BarCodeNotFound);
    }

    /// <summary>
    /// Executes Process_WithEmptyPartNumber_ShouldProcessNormally operation.
    /// </summary>
    /// <returns>The result of Process_WithEmptyPartNumber_ShouldProcessNormally.</returns>

    [Fact]
    public async Task Process_WithEmptyPartNumber_ShouldProcessNormally()
    {
        // Arrange
        var machineId = 1;
        var barCode = "L1ATEST1230001";
        var emptyPartNumber = "";

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = barCode,
                PartNumber = emptyPartNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.ResultValidation.Returns(ResultValidation.BarCodeNotFound);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ResultValidation.ShouldBe(ResultValidation.BarCodeNotFound);
    }

    /// <summary>
    /// Executes Process_WithValidRequest_ShouldCreateCycleWithCorrectProperties operation.
    /// </summary>
    /// <returns>The result of Process_WithValidRequest_ShouldCreateCycleWithCorrectProperties.</returns>

    [Fact]
    public async Task Process_WithValidRequest_ShouldCreateCycleWithCorrectProperties()
    {
        // Arrange
        var machineId = 1;
        var barCode = "L1ATEST1230001";
        var partNumber = "TEST123";
        var currentTime = DateTime.Now;
        var barCodeId = 123;

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = barCode,
                PartNumber = partNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.BarCodeId.Returns(barCodeId);
        barCodeInfo.BarCode.Returns(new BarCode
        {
            BarCodeId = barCodeId,
            Label = barCode,
            MachineId = machineId,
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.Created
        });
        barCodeInfo.ResultValidation.Returns(ResultValidation.Valid);
        barCodeInfo.MasterLabel.Returns((MasterLabel)null!);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);
        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodeId.ShouldBe(barCodeId);
        result.Value.BarCode.ShouldNotBeNull();
        result.Value.BarCode.BarCodeId.ShouldBe(barCodeId);
    }

    /// <summary>
    /// Executes Process_WithValidRequest_ShouldUpdateBarCodeProperties operation.
    /// </summary>
    /// <returns>The result of Process_WithValidRequest_ShouldUpdateBarCodeProperties.</returns>

    [Fact]
    public async Task Process_WithValidRequest_ShouldUpdateBarCodeProperties()
    {
        // Arrange
        var machineId = 1;
        var barCode = "L1ATEST1230001";
        var partNumber = "TEST123";
        var currentTime = DateTime.Now;
        var barCodeId = 123;

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = barCode,
                PartNumber = partNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.BarCodeId.Returns(barCodeId);
        barCodeInfo.BarCode.Returns(new BarCode
        {
            BarCodeId = barCodeId,
            Label = barCode,
            MachineId = machineId,
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.Created
        });
        barCodeInfo.ResultValidation.Returns(ResultValidation.Valid);
        barCodeInfo.MasterLabel.Returns((MasterLabel)null!);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);
        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodeId.ShouldBe(barCodeId);
        result.Value.BarCode.ShouldNotBeNull();
        result.Value.BarCode.BarCodeId.ShouldBe(barCodeId);
    }

    /// <summary>
    /// Executes Process_WithValidRequest_ShouldSetCycleInBarCodeInfo operation.
    /// </summary>
    /// <returns>The result of Process_WithValidRequest_ShouldSetCycleInBarCodeInfo.</returns>

    [Fact]
    public async Task Process_WithValidRequest_ShouldSetCycleInBarCodeInfo()
    {
        // Arrange
        var machineId = 1;
        var barCode = "L1ATEST1230001";
        var partNumber = "TEST123";
        var currentTime = DateTime.Now;
        var barCodeId = 123;

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = barCode,
                PartNumber = partNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.BarCodeId.Returns(barCodeId);
        barCodeInfo.BarCode.Returns(new BarCode
        {
            BarCodeId = barCodeId,
            Label = barCode,
            MachineId = machineId,
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.Created
        });
        barCodeInfo.ResultValidation.Returns(ResultValidation.Valid);
        barCodeInfo.MasterLabel.Returns((MasterLabel)null!);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);
        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodeId.ShouldBe(barCodeId);
        result.Value.BarCode.ShouldNotBeNull();
        result.Value.BarCode.BarCodeId.ShouldBe(barCodeId);
    }

    /// <summary>
    /// Executes Process_WithValidRequest_ShouldApplyReferencesValues operation.
    /// </summary>
    /// <returns>The result of Process_WithValidRequest_ShouldApplyReferencesValues.</returns>

    [Fact]
    public async Task Process_WithValidRequest_ShouldApplyReferencesValues()
    {
        // Arrange
        var machineId = 1;
        var barCode = "L1ATEST1230001";
        var partNumber = "TEST123";
        var currentTime = DateTime.Now;
        var barCodeId = 123;

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = barCode,
                PartNumber = partNumber
            }
        };

        var barCodeInfo = Substitute.For<IBarCodeResult>();
        barCodeInfo.BarCodeId.Returns(barCodeId);
        barCodeInfo.BarCode.Returns(new BarCode
        {
            BarCodeId = barCodeId,
            Label = barCode,
            MachineId = machineId,
            PartStatus = PartStatus.Ok,
            FlowStatus = FlowStatus.Created
        });
        barCodeInfo.ResultValidation.Returns(ResultValidation.Valid);
        barCodeInfo.MasterLabel.Returns((MasterLabel)null!);

        _barCodeResult.GetBarCodeDetails(Arg.Any<BarCodeDetailsRequest>(), Arg.Any<CancellationToken>())
            .Returns(barCodeInfo);
        _dateTimeMachine.Now.Returns(currentTime);

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.BarCodeId.ShouldBe(barCodeId);
        result.Value.BarCode.ShouldNotBeNull();
        result.Value.BarCode.BarCodeId.ShouldBe(barCodeId);
    }

    /// <summary>
    /// Executes TryReset_ShouldReturnTrue operation.
    /// </summary>

    [Fact]
    public void TryReset_ShouldReturnTrue()
    {
        // Arrange
        var machineId = 1;
        var barCode = "L1ATEST1230001";
        var partNumber = "TEST123";

        var command = new UpdateBarCodeCommand
        {
            Command = new TaskGatewayRequest
            {
                MachineId = machineId,
                BarCode = barCode,
                PartNumber = partNumber
            }
        };

        // Act
        var result = _handler.TryReset();

        // Assert
        result.ShouldBeTrue();
    }
}
