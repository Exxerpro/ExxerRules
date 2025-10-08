using IndTrace.Application.BarCodes.Commands.Update;

namespace GateWay.Tests.Gateway;

using IndTrace.HubConnection.Abstractions;

/// <summary>
/// Represents the GatewayTasksEndOfProcessAsyncTests.
/// </summary>

public class GatewayTasksEndOfProcessAsyncTests : HubFactoryHelper
{
    private readonly IIndTraceControllerRx _controllerMock = Substitute.For<IIndTraceControllerRx>();
    private readonly IGatewayCommandDispatcher _guiCommandDispatcherMock = Substitute.For<IGatewayCommandDispatcher>();
    private readonly IHubConnection? _hubConnectionMock = default;
    private readonly CancellationToken _cancellationToken = new();
    private readonly ITestOutputHelper output;

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTasksEndOfProcessAsyncTests"/> class.
    /// </summary>
    /// <param name="output"></param>
    public GatewayTasksEndOfProcessAsyncTests(ITestOutputHelper output) : base(output)
    {
        // Existing constructor logic (if any)
        this.output = output;
    }

    /// <summary>
    /// Executes EndOfProcessAsync_ShouldLogError_WhenCommandFails operation.
    /// </summary>
    /// <returns>The result of EndOfProcessAsync_ShouldLogError_WhenCommandFails.</returns>

    [Fact]
    public async Task EndOfProcessAsync_ShouldLogError_WhenCommandFails()
    {
        // Arrange
        var commandResult = Result<TaskGatewayResponse>.WithFailure("Command failed");
        var logger = XUnitLogger.CreateLogger<GatewayTasksEndOfProcessAsyncTests>(output);
        await Task.Delay(2000, _cancellationToken);

        _guiCommandDispatcherMock
            .ProcessAsync(Arg.Any<UpdateBarCodeCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(commandResult));

        // Mock controller dependencies
        _controllerMock.References.Returns(new Dictionary<string, Register>());
        _controllerMock.Registers.Returns(new Dictionary<string, Register>());
        _controllerMock.ResetCommandAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult("OK"));
        _controllerMock.DownloadReferencesBulkAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Success()));
        _controllerMock.SetFeedBackAsync(Arg.Any<short>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult("OK"));
        _controllerMock.ReadRegistersBulkAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Success()));

        var dataFromPlc = new DataFromPlc
        {
            MachineId = 100,
            Command = 0,
            BarCode = "",
            PartNumber = "PNEND",
            CycleStatus = CycleStatus.EndOfProcess,
            PartStatus = PartStatus.Ok,
            WatchDogTime = WatchDog.Disable
        };
        var resultDataFromPlc = Result<DataFromPlc>.Success(dataFromPlc);
        _controllerMock
            .UploadCommandDataFromController(
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(resultDataFromPlc));

        // Act
        var result = await GatewayTasks.EndOfProcessAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Value.ShouldBeNull();
        await _guiCommandDispatcherMock.Received(1)
            .ProcessAsync(Arg.Any<UpdateBarCodeCommand>(), Arg.Any<CancellationToken>());
        await _guiCommandDispatcherMock.Received(1).ProcessAsync(Arg.Any<UpdateBarCodeCommand>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes EndOfProcessAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds operation.
    /// </summary>
    /// <returns>The result of EndOfProcessAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds.</returns>

    [Fact]
    public async Task EndOfProcessAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds()
    {
        // Arrange
        var expectedBarCodeDetail = new TaskGatewayResponse();
        var commandResult = Result<TaskGatewayResponse>.Success(expectedBarCodeDetail);
        var logger = XUnitLogger.CreateLogger<GatewayTasksEndOfProcessAsyncTests>(output);
        await Task.Delay(2000, _cancellationToken);

        _guiCommandDispatcherMock
        .ProcessAsync(Arg.Any<UpdateBarCodeCommand>(), Arg.Any<CancellationToken>())
        .Returns(Task.FromResult(commandResult));

        // Mock controller dependencies
        _controllerMock.References.Returns(new Dictionary<string, Register>());
        _controllerMock.Registers.Returns(new Dictionary<string, Register>());
        _controllerMock.ResetCommandAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult("OK"));
        _controllerMock.DownloadReferencesBulkAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Success()));
        _controllerMock.SetFeedBackAsync(Arg.Any<short>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult("OK"));
        _controllerMock.ReadRegistersBulkAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult(Result.Success()));

        var dataFromPlc = new DataFromPlc
        {
            MachineId = 100,
            Command = 0,
            BarCode = "END123",
            PartNumber = "PNEND",
            CycleStatus = CycleStatus.EndOfProcess,
            PartStatus = PartStatus.Ok,
            WatchDogTime = WatchDog.Disable
        };
        var resultDataFromPlc = Result<DataFromPlc>.Success(dataFromPlc);
        _controllerMock
            .UploadCommandDataFromController(
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(resultDataFromPlc));

        // Act
        var result = await GatewayTasks.EndOfProcessAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);
        result.Value.ShouldNotBeNull();

        // Assert
        result.Value.ShouldBeEquivalentTo(expectedBarCodeDetail);
        await _guiCommandDispatcherMock.Received(1)
            .ProcessAsync(Arg.Any<UpdateBarCodeCommand>(), Arg.Any<CancellationToken>());
    }
}
