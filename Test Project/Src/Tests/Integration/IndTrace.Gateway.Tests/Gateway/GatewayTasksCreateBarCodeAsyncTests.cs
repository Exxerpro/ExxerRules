using IndTrace.Application.Models.Services;
using IndTrace.Domain.Entities.BarCodes;
using IndTrace.HubConnection.Abstractions;
using IndTrace.HubConnection.Implementations;
using Meziantou.Extensions.Logging.Xunit.v3;
using Microsoft.Extensions.Options;
using Xunit.v3;

namespace GateWay.Tests.Gateway;
/// <summary>
/// Represents the GatewayTasksCreateBarCodeAsyncTests.
/// </summary>

[Collection(nameof(NonParallelCollectionDefinition))]
public class GatewayTasksCreateBarCodeAsyncTests : HubFactoryHelper
{
    private readonly IIndTraceControllerRx _controllerMock = Substitute.For<IIndTraceControllerRx>();
    private readonly IGatewayCommandDispatcher _guiCommandDispatcherMock = Substitute.For<IGatewayCommandDispatcher>();
    private readonly IHubConnection? _hubConnectionMock = default;
    private readonly CancellationToken _cancellationToken = new();
    private readonly ITestOutputHelper _output;

    public GatewayTasksCreateBarCodeAsyncTests(ITestOutputHelper output) : base(output)
    {
        _output = output;
    }

    [Fact]
    public async Task CreateBarCodeAsync_ShouldReturnLabel_WhenCommandSucceeds()
    {
        // Arrange
        var expectedLabel = "12345";
        var barcode = new BarCode { Label = expectedLabel };
        var expectedBarCodeDetail = new TaskGatewayResponse();
        expectedBarCodeDetail.WithBarCode(barcode);

        var commandResult = Result<TaskGatewayResponse>.Success(expectedBarCodeDetail);

        // Setup the dispatcher mock to return the expected result for any CreateBarCodeCommand
        _guiCommandDispatcherMock
            .ProcessAsync(Arg.Any<CreateBarCodeCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(commandResult));

        var logger = XUnitLogger.CreateLogger<GatewayTasksCreateBarCodeAsyncTests>(_output);
        await Task.Delay(2000, _cancellationToken);

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
            BarCode = expectedLabel,
            PartNumber = "PN123",
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
        var result = await GatewayTasks.CreateBarCodeAsync(
            _controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Label.ShouldBeEquivalentTo(expectedLabel);
    }

    [Fact]
    public async Task CreateBarCodeAsync_ShouldLogError_WhenCommandFails()
    {
        // Arrange
        var commandResult = Result<TaskGatewayResponse>.WithFailure("Command failed");

        _guiCommandDispatcherMock
            .ProcessAsync(Arg.Any<CreateBarCodeCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(commandResult));

        var logger = XUnitLogger.CreateLogger<GatewayTasksCreateBarCodeAsyncTests>(_output);
        await Task.Delay(2000, _cancellationToken);

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
            PartNumber = "PN123",
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
        var result = await GatewayTasks.CreateBarCodeAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        await _guiCommandDispatcherMock.Received(1).ProcessAsync(Arg.Any<CreateBarCodeCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateBarCodeAsync_ShouldReturnValidResult_WhenCalled()
    {
        // Arrange
        var expectedLabel = "12345";
        var barcode = new BarCode { Label = expectedLabel };
        var resultForController = new TaskGatewayResponse();
        resultForController.WithBarCode(barcode);
        var commandResult = Result<TaskGatewayResponse>.Success(resultForController);

        _guiCommandDispatcherMock
            .ProcessAsync(Arg.Any<CreateBarCodeCommand>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(commandResult));

        var logger = XUnitLogger.CreateLogger<GatewayTasksCreateBarCodeAsyncTests>(_output);
        await Task.Delay(2000, _cancellationToken);

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
            BarCode = expectedLabel,
            PartNumber = "PN123",
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
        await Task.Delay(2000, _cancellationToken);
        var firstResult = await GatewayTasks.CreateBarCodeAsync(_controllerMock, _guiCommandDispatcherMock, _hubConnectionMock, connectionFactory, logger, _cancellationToken);

        // Assert
        firstResult.Value.ShouldNotBeNull();
        firstResult.Value.Label.ShouldBeEquivalentTo(expectedLabel);
        await _guiCommandDispatcherMock.Received(1).ProcessAsync(Arg.Any<CreateBarCodeCommand>(), Arg.Any<CancellationToken>()); // Ensure it was only called once
    }
}

/// <summary>
/// Represents the HubFactoryHelper.
/// </summary>

public class HubFactoryHelper
{
    public readonly IHubConnectionFactory connectionFactory;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public HubFactoryHelper(ITestOutputHelper testOutputHelper)
    {
        var setups = new List<IConfigureOptions<HubMonitorOptions>>()
        {
            new ConfigureOptions<HubMonitorOptions>(
                options =>
                {
                    options.Url = "http://localhost:5000/hub";
                })
        };

        var postConfigures = new List<PostConfigureOptions<HubMonitorOptions>>();

        OptionsFactory<HubMonitorOptions> factory = new OptionsFactory<HubMonitorOptions>(setups, postConfigures);

        IOptions<HubMonitorOptions> options = new OptionsManager<HubMonitorOptions>(factory);
        var logger = XUnitLogger.CreateLogger<HubConnectionFactory>(testOutputHelper);
        connectionFactory = new HubConnectionFactory(options, logger);
    }
}
