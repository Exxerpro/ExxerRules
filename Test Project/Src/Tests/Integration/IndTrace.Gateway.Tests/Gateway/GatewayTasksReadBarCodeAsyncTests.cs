using IndTrace.Application.BarCodes.Queries.GetBarCodeGatewayDetail;
using IndTrace.HubConnection.Abstractions;

namespace GateWay.Tests.Gateway;
/// <summary>
/// Represents the GatewayTasksReadBarCodeAsyncTests.
/// </summary>

public class GatewayTasksReadBarCodeAsyncTests : HubFactoryHelper
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
    public GatewayTasksReadBarCodeAsyncTests(ITestOutputHelper output) : base(output)
    {
        // Existing constructor logic (if any)
        this.output = output;
    }

    /// <summary>
    /// Executes ReadBarCodeAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds operation.
    /// </summary>
    /// <returns>The result of ReadBarCodeAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds.</returns>

    [Fact]
    public async Task ReadBarCodeAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds()
    {
        // Arrange
        var expectedBarCodeDetail = new TaskGatewayResponse();
        var commandResult = Result<TaskGatewayResponse>.Success(expectedBarCodeDetail);
        var logger = XUnitLogger.CreateLogger<GatewayTasksReadBarCodeAsyncTests>(output);
        await Task.Delay(2000, _cancellationToken);

        _guiCommandDispatcherMock
            .ProcessAsync(Arg.Any<ReadBarCodeQuery>(), Arg.Any<CancellationToken>())
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
            BarCode = "BARCODE123",
            PartNumber = "PNREAD",
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
        var result = await GatewayTasks.ReadBarCodeAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        result.Value.ShouldBeEquivalentTo(expectedBarCodeDetail);
        await _guiCommandDispatcherMock.Received(1)
            .ProcessAsync(Arg.Any<ReadBarCodeQuery>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes ReadBarCodeAsync_ShouldLogError_WhenCommandFails operation.
    /// </summary>
    /// <returns>The result of ReadBarCodeAsync_ShouldLogError_WhenCommandFails.</returns>

    [Fact]
    public async Task ReadBarCodeAsync_ShouldLogError_WhenCommandFails()
    {
        // Arrange
        var commandResult = Result<TaskGatewayResponse>.WithFailure("Command failed");
        var logger = XUnitLogger.CreateLogger<GatewayTasksReadBarCodeAsyncTests>(output);
        await Task.Delay(2000, _cancellationToken);

        _guiCommandDispatcherMock.ProcessAsync(Arg.Any<ReadBarCodeQuery>(), _cancellationToken).Returns(commandResult);

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
            PartNumber = "PNREAD",
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
        var result = await GatewayTasks.ReadBarCodeAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        await _guiCommandDispatcherMock.Received(1)
            .ProcessAsync(Arg.Any<ReadBarCodeQuery>(), Arg.Any<CancellationToken>());
    }
}
