namespace IndTrace.Filters.Tests;
/// <summary>
/// Represents the ReadRegistersBulkAsyncTests.
/// </summary>

public class ReadRegistersBulkAsyncTests(ITestOutputHelper output)
{
    private readonly IPlc _controller = Substitute.For<IPlc>();
    /// <summary>
    /// Executes ReadRegistersBulkAsync_ShouldReturnSuccess_WhenAllReadsAreGood operation.
    /// </summary>
    /// <returns>The result of ReadRegistersBulkAsync_ShouldReturnSuccess_WhenAllReadsAreGood.</returns>

    [Fact]
    public async Task ReadRegistersBulkAsync_ShouldReturnSuccess_WhenAllReadsAreGood()
    {
        var logger = XUnitLogger.CreateLogger(output);
        var dateTimeMachine = new DateTimeMachine();
        var plcDetail = new PlcDto();

        var sut = new IndTraceControllerRx(logger, plcDetail, dateTimeMachine)
        {
            EnableSimulation = false,
            IsConnected = true,
            IsInitialized = true,
            Registers = new Dictionary<string, Register>
            {
                { "R1", new() { Name = "R1", DataType = "int", VariableId = 1 } }
            },
            Controller = _controller
        };

        _controller.GetBatchValuesPlcAsync(Arg.Any<List<(string, Type, int)>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromResult<IEnumerable<PlcBatchReadResult>>(new List<PlcBatchReadResult>
            {
                new("R1", "R1", "100", 100, 1)
            }));
        var result = await sut.ReadRegistersBulkAsync(TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Executes ReadRegistersBulkAsync_ShouldHandleVariousDataTypes operation.
    /// </summary>
    /// <param name="alias">The alias.</param>
    /// <param name="address">The address.</param>
    /// <param name="dataType">The dataType.</param>
    /// <param name="expected">The expected.</param>
    /// <returns>The result of ReadRegistersBulkAsync_ShouldHandleVariousDataTypes.</returns>

    [Theory]
    [InlineData("Component UserId BarCode", "DB257.S100.32", "String", "hello world")]
    [InlineData("ValueTest Current SPOILER", "DB257.D80", "Single", "456.78")]
    [InlineData("ResultValidation", "DB257.DBW414", "Int16", "456")]
    [InlineData("Test Cam Ventpatch", "DB257.X10.5", "Boolean", "True")]
    [InlineData("Value LED SPOILER E 40", "DB256.DINT40", "Int32", "123")]
    public async Task ReadRegistersBulkAsync_ShouldHandleVariousDataTypes(string alias, string address, string dataType, string value)
    {
        var logger = XUnitLogger.CreateLogger(output);
        var dateTimeMachine = new DateTimeMachine();

        var netType = Type.GetType($"System.{dataType}", throwOnError: false) ?? typeof(string);
        var convertedValue = Convert.ChangeType(value, netType);

        var variable = new Variable
        {
            Name = alias,
            NativeType = dataType,
            VariableId = 1,
            Alias = address
        };

        var plcDetail = new PlcDto
        {
            Variables = new Dictionary<string, Variable>
            {
                { alias, variable }
            }
        };

        var tag = new IndTraceTagRx(netType)
        {
            Value = convertedValue,
            EnableSimulation = false,
            Variable = variable
        };

        var s7Connector = Substitute.For<ISharp7Connector>();
        s7Connector.WriteBatchValuesPlcAsync(Arg.Any<List<PlcBatchWriteTag>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        var controller = Substitute.For<IPlc>();
        controller.s7Connector.Returns(s7Connector);

        var sut = new IndTraceControllerRx(logger, plcDetail, dateTimeMachine)
        {
            EnableSimulation = false,
            IsConnected = true,
            IsInitialized = true,
            References = new Dictionary<string, Register>
            {
                { alias, new Register { Name = alias, DataType = dataType, VariableId = 1, Value = value } }
            },
            IndTraceTags = new Dictionary<string, IIndTraceTagRx>
            {
                { alias, tag }
            },
            Controller = controller
        };

        var result = await sut.DownloadReferencesBulkAsync(TestContext.Current.CancellationToken);

        result.IsSuccess.ShouldBeTrue();
    }
}
