using IndTrace.Application.Cycles.Commands.UpdateCyclesOk;

namespace GateWay.Tests.Gateway;

using IndTrace.HubConnection.Abstractions;

/// <summary>
/// Represents the GatewayTasksUpdateCycleAsyncTests.
/// </summary>

public class GatewayTasksUpdateCycleAsyncTests : HubFactoryHelper
{
    private readonly IIndTraceControllerRx _controllerMock = Substitute.For<IIndTraceControllerRx>();
    private readonly IGatewayCommandDispatcher _guiCommandDispatcherMock = Substitute.For<IGatewayCommandDispatcher>();
    private readonly IHubConnection? _hubConnectionMock = default;
    private readonly CancellationToken _cancellationToken = new();
    private readonly ITestOutputHelper output;

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTasksReadBarCodeAsyncTests"/> class.
    /// </summary>
    /// <param name="output"></param>
    public GatewayTasksUpdateCycleAsyncTests(ITestOutputHelper output) : base(output)
    {
        // Existing constructor logic (if any)
        this.output = output;
    }

    /// <summary>
    /// Executes UpdateCycleAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds operation.
    /// </summary>
    /// <returns>The result of UpdateCycleAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds.</returns>

    [Fact]
    public async Task UpdateCycleAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds()
    {
        // Arrange
        var expectedBarCodeDetail = new TaskGatewayResponse();
        var commandResult = Result<TaskGatewayResponse>.Success(expectedBarCodeDetail);
        var logger = XUnitLogger.CreateLogger<GatewayTasksUpdateCycleAsyncTests>(output);
        await Task.Delay(2000, _cancellationToken);

        _guiCommandDispatcherMock
            .ProcessAsync(Arg.Any<UpdateCyclesOkCommand>(), Arg.Any<CancellationToken>())
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
            BarCode = "UPDATECYCLE123",
            PartNumber = "PNUPDATE",
            CycleStatus = CycleStatus.Started,
            PartStatus = PartStatus.Ok,
            WatchDogTime = WatchDog.Disable
        };
        var resultDataFromPlc = Result<DataFromPlc>.Success(dataFromPlc);
        _controllerMock
            .UploadCommandDataFromController(
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(resultDataFromPlc));

        // Act
        var result = await GatewayTasks.UpdateCycleOkAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        result.Value.ShouldBeEquivalentTo(expectedBarCodeDetail);
        await _guiCommandDispatcherMock.Received(1)
            .ProcessAsync(Arg.Any<UpdateCyclesOkCommand>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes UpdateCycleAsync_ShouldLogError_WhenCommandFails operation.
    /// </summary>
    /// <returns>The result of UpdateCycleAsync_ShouldLogError_WhenCommandFails.</returns>

    [Fact]
    public async Task UpdateCycleAsync_ShouldLogError_WhenCommandFails()
    {
        // Arrange
        var commandResult = Result<TaskGatewayResponse>.WithFailure("Command failed");
        var logger = XUnitLogger.CreateLogger<GatewayTasksUpdateCycleAsyncTests>(output);
        await Task.Delay(2000, _cancellationToken);

        _guiCommandDispatcherMock.ProcessAsync(Arg.Any<UpdateCyclesOkCommand>(), _cancellationToken).Returns(commandResult);

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
            PartNumber = "PNUPDATE",
            CycleStatus = CycleStatus.Started,
            PartStatus = PartStatus.Ok,
            WatchDogTime = WatchDog.Disable
        };
        var resultDataFromPlc = Result<DataFromPlc>.Success(dataFromPlc);
        _controllerMock
            .UploadCommandDataFromController(
                Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(resultDataFromPlc));

        // Act
        var result = await GatewayTasks.UpdateCycleOkAsync(
            _controllerMock,
            _guiCommandDispatcherMock,
            _hubConnectionMock,
            connectionFactory,
            logger,
            _cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        await _guiCommandDispatcherMock.Received(1).ProcessAsync(Arg.Any<UpdateCyclesOkCommand>(), Arg.Any<CancellationToken>());
    }
}
