using System.Reactive.Subjects;
using IndTrace.DataStore.ModelsComs;
using IndTrace.OEE.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Sharp7.Rx.BatchRead;
using Sharp7.Rx.Enums;
using Sharp7.Rx.Interfaces;
using Shouldly;
using Xunit;

namespace IndTrace.Oee.Tests;
/// <summary>
/// Represents the PlcProcessorTests.
/// </summary>

public class PlcProcessorTests
{
    private readonly IPlc _controller;

    private readonly ILogger _logger;
    private readonly PlcData _plcData;
    private readonly Dictionary<string, VariableS7> _variables;
    private readonly PlcProcessor _processor;
    private readonly BehaviorSubject<ConnectionState> _connectionState;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public PlcProcessorTests()
    {
        _controller = Substitute.For<IPlc>();
        _logger = Substitute.For<ILogger>();
        _connectionState = new BehaviorSubject<ConnectionState>(ConnectionState.Initial);

        _plcData = new PlcData
        {
            IpAddress = "192.168.0.1",
            RackNumber = 0,
            CpuMpiAddress = 2,
            Port = 102,
            Variables = new Dictionary<string, VariableS7>
            {
                ["Tag1"] = new VariableS7
                {
                    Name = "Tag1",
                    Alias = "DB20.DINT0",  // ✅ Proper S7 format
                    VariableId = 1,
                    NetType = typeof(int).AssemblyQualifiedName ?? string.Empty
                }
            }
        };

        _variables = new Dictionary<string, VariableS7>
        {
            ["Tag1"] = new VariableS7 { Name = "Tag1", Alias = "DB20.DINT0", VariableId = 1, NetType = typeof(int).AssemblyQualifiedName ?? string.Empty },
            ["MachineId"] = new VariableS7 { Name = "MachineId", Alias = "DB20.DINT4", VariableId = 2, NetType = typeof(int).AssemblyQualifiedName ?? string.Empty },
            ["PlcId"] = new VariableS7 { Name = "PlcId", Alias = "DB20.DINT8", VariableId = 3, NetType = typeof(int).AssemblyQualifiedName ?? string.Empty }
        };

        _processor = new PlcProcessor(1, _plcData, _variables, _logger)
        {
            Controller = _controller,
            IpAddress = _plcData.IpAddress
        };

        _controller.ConnectionState.Returns(_connectionState);
    }
    /// <summary>
    /// Executes ConnectWithNotificationAsync_Should_ConnectSuccessfully operation.
    /// </summary>
    /// <returns>The result of ConnectWithNotificationAsync_Should_ConnectSuccessfully.</returns>

    [Fact]
    public async Task ConnectWithNotificationAsync_Should_ConnectSuccessfully()
    {
        await _controller.InitializeConnection(Arg.Any<CancellationToken>());
        _connectionState.OnNext(ConnectionState.Connected);

        var result = await _processor.ConnectWithNotificationAsync(TestContext.Current.CancellationToken);

        result.ShouldBeTrue();
        _processor.IsConnected.ShouldBeTrue();
    }
    /// <summary>
    /// Executes ConnectWithNotificationAsync_Should_Timeout operation.
    /// </summary>
    /// <returns>The result of ConnectWithNotificationAsync_Should_Timeout.</returns>

    [Fact]
    public async Task ConnectWithNotificationAsync_Should_Timeout()
    {
        var resultTask = _processor.ConnectWithNotificationAsync(TestContext.Current.CancellationToken);

        await Should.ThrowAsync<TimeoutException>(async () => await resultTask);
    }
    /// <summary>
    /// Executes ReadPerformanceDataFromPlcAsync_Should_ReturnValidData operation.
    /// </summary>
    /// <returns>The result of ReadPerformanceDataFromPlcAsync_Should_ReturnValidData.</returns>

    [Fact]
    public async Task ReadPerformanceDataFromPlcAsync_Should_ReturnValidData()
    {
        _processor.IsConnected = true;
        _processor.IsInitialized = true;
        await _processor.InitializeAsync();
        _connectionState.OnNext(ConnectionState.Connected);

        _controller.GetBatchValuesPlcAsync(Arg.Any<List<(string, Type, int)>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<PlcBatchReadResult>>(new List<PlcBatchReadResult>
            {
                new PlcBatchReadResult("DB400.DINT0", "DB400.DINT0", "42", 0, 1),
            }));

        var result = await _processor.ReadPerformanceDataFromPlcAsync(1, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
        if (!(result.IsSuccess && result.Value is { } value1))
        {
            throw new Xunit.Sdk.XunitException("Expected success result with non-null Value");
        }
        value1.PlcId.ShouldBe(1);
    }
    /// <summary>
    /// Executes ReadPerformanceDataFromPlcAsync_Should_Return_ValidResults operation.
    /// </summary>
    /// <returns>The result of ReadPerformanceDataFromPlcAsync_Should_Return_ValidResults.</returns>

    [Fact]
    public async Task ReadPerformanceDataFromPlcAsync_Should_Return_ValidResults()
    {
        _processor.IsConnected = true;
        _processor.IsInitialized = true;
        await _processor.InitializeAsync();
        _connectionState.OnNext(ConnectionState.Connected);

        _controller
            .GetBatchValuesPlcAsync(Arg.Any<List<(string, Type, int)>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IEnumerable<PlcBatchReadResult>>(new List<PlcBatchReadResult>
            {
                new PlcBatchReadResult("DB400.DINT0", "DB400.DINT0", "42", 0, 1),
            }));

        var result = await _processor.ReadPerformanceDataFromPlcAsync(1, TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
        if (!(result.IsSuccess && result.Value is { } value2))
        {
            throw new Xunit.Sdk.XunitException("Expected success result with non-null Value");
        }
        value2.PlcId.ShouldBe(1);
    }
}
