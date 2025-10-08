namespace GateWay.Tests.Gateway;

//TODO
//ABR
//[IMPORTANT]
// 16 JUN 2025

// TODO: [Code Coverage & Mutation Testing]
// Consider running Coverlet for code coverage analysis and Stryker.NET for mutation testing.
// This will help identify untested code paths and weak assertions, guiding future test improvements and abstractions.
// Revisit when time allows to ensure robust, high-quality, and well-tested codebase.
using IndTrace.HubConnection.Abstractions;

/// <summary>
/// Represents the GatewayTasksCreateCyclesCommandTests.
/// </summary>

public class GatewayTasksCreateCyclesCommandTests : HubFactoryHelper
{
    private readonly IIndTraceControllerRx _controllerMock = Substitute.For<IIndTraceControllerRx>();
    private readonly IGatewayCommandDispatcher _guiCommandDispatcherMock = Substitute.For<IGatewayCommandDispatcher>();
    private readonly IHubConnection? _hubConnectionMock = default;
    private readonly CancellationToken _cancellationToken = new();
    private readonly ITestOutputHelper output;

    /// <summary>
    /// Initializes a new instance of the <see cref="GatewayTasksCreateCyclesCommandTests"/> class.
    /// </summary>
    /// <param name="output"></param>
    public GatewayTasksCreateCyclesCommandTests(ITestOutputHelper output) : base(output)
    {
        this.output = output;
    }

    /// <summary>
    /// Executes CreateCycleAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds operation.
    /// </summary>
    /// <returns>The result of CreateCycleAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds.</returns>

    [Fact]
    public async Task CreateCycleAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds()
    {
        var logger = XUnitLogger.CreateLogger<GatewayTasksCreateCyclesCommandTests>(output);
        logger.LogInformation("Starting test: CreateCycleAsync_ShouldReturnBarCodeDetail_WhenCommandSucceeds");
        // Arrange
        var expectedBarCodeDetail = new TaskGatewayResponse();
        var commandResult = Result<TaskGatewayResponse>.Success(expectedBarCodeDetail);

        await Task.Delay(2000, _cancellationToken);

        _guiCommandDispatcherMock
            .ProcessAsync(Arg.Any<CreateCyclesCommand>(), Arg.Any<CancellationToken>())
            .Returns(commandResult);

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
            BarCode = "CYCLE123",
            PartNumber = "PNCYCLE",
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
        var result = await GatewayTasks.CreateCycleAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        result.Value.ShouldBeEquivalentTo(expectedBarCodeDetail);
        await _guiCommandDispatcherMock.Received(1).ProcessAsync(Arg.Any<CreateCyclesCommand>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes CreateCycleAsync_ShouldLogError_WhenCommandFails operation.
    /// </summary>
    /// <returns>The result of CreateCycleAsync_ShouldLogError_WhenCommandFails.</returns>

    [Fact]
    public async Task CreateCycleAsync_ShouldLogError_WhenCommandFails()
    {
        // Arrange
        var commandResult = Result<TaskGatewayResponse>.WithFailure("Command failed");
        var logger = XUnitLogger.CreateLogger<GatewayTasksCreateCyclesCommandTests>(output);
        await Task.Delay(2000, _cancellationToken);

        _guiCommandDispatcherMock
            .ProcessAsync(Arg.Any<CreateCyclesCommand>(), Arg.Any<CancellationToken>())
            .Returns(commandResult);

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
            PartNumber = "PNCYCLE",
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
        var result = await GatewayTasks.CreateCycleAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        await _guiCommandDispatcherMock.Received(1).ProcessAsync(Arg.Any<CreateCyclesCommand>(), Arg.Any<CancellationToken>());
    }
}
