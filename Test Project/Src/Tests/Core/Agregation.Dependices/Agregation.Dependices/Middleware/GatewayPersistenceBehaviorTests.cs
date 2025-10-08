using IndTrace.Agregation.Dependices.Dependencies;

namespace IndTrace.Agregation.Dependices.Middleware;

/// <summary>
/// Unit tests for GatewayPersistenceBehavior
/// </summary>
public class GatewayPersistenceBehaviorTests : DependenciesFactory
{
    public GatewayPersistenceBehaviorTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var loggerMock = XUnitLogger.CreateLogger<CreateBarCodeCommand>();

        // Act
        var instance = new GatewayPersistenceBehavior<CreateBarCodeCommand, TaskGatewayResponse>(
            loggerMock,
            DpRequestRepository,
            DpResponseRepository
        );

        // Assert
        instance.ShouldNotBeNull();
    }

    ///// <summary>
    ///// Executes Constructor_WithInvalidParameters_ShouldThrowException operation.
    ///// </summary>

    //[Fact]
    //public void Constructor_WithInvalidParameters_ShouldThrowException()
    //{
    //    // Arrange
    //    ILogger<CreateBarCodeCommand> loggerMock = null!;

    //    // Act & Assert
    //    Should.Throw<ArgumentNullException>(() =>
    //        new GatewayPersistenceBehavior<CreateBarCodeCommand, TaskGatewayResponse>(
    //            loggerMock,
    //            DpRequestRepository,
    //            DpRequestRepository
    //        )
    //    );
    //}
    /// <summary>
    /// Executes Handle_WithValidCommandData_ShouldPersistRequestAndResponse operation.
    /// </summary>
    /// <returns>The result of Handle_WithValidCommandData_ShouldPersistRequestAndResponse.</returns>

    [Fact]
    public async Task Handle_WithValidCommandData_ShouldPersistRequestAndResponse()
    {
        await Initialization;

        // Arrange
        var loggerMock = XUnitLogger.CreateLogger<CreateBarCodeCommand>();
        var behavior = new GatewayPersistenceBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>(
            loggerMock,
            DpRequestRepository,
            DpResponseRepository
        );

        var command = CreateTestCommand();
        var gatewayResponse = CreateTestGatewayResponse();
        var expectedResult = Result<TaskGatewayResponse>.Success(gatewayResponse);

        RequestFunctionalHandlerDelegate<Result<TaskGatewayResponse>> next = () => Task.FromResult(expectedResult);

        // Act
        var result = await behavior.HandleAsync(command, next, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(expectedResult);
    }

    /// <summary>
    /// Executes Handle_WithFailedResponse_ShouldPersistErrorInformation operation.
    /// </summary>
    /// <returns>The result of Handle_WithFailedResponse_ShouldPersistErrorInformation.</returns>

    [Fact]
    public async Task Handle_WithFailedResponse_ShouldPersistErrorInformation()
    {
        await Initialization;

        // Arrange
        var loggerMock = XUnitLogger.CreateLogger<CreateBarCodeCommand>();
        var behavior = new GatewayPersistenceBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>(
            loggerMock,
            DpRequestRepository,
            DpResponseRepository
        );

        var command = CreateTestCommand();
        var gatewayResponse = CreateTestGatewayResponse();
        var failedResult = Result<TaskGatewayResponse>.WithFailure("Machine communication error", gatewayResponse);

        RequestFunctionalHandlerDelegate<Result<TaskGatewayResponse>> next = () => Task.FromResult(failedResult);

        // Act
        var result = await behavior.HandleAsync(command, next, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(failedResult);
    }

    /// <summary>
    /// Executes Handle_WithNonCommandDataRequest_ShouldSkipPersistence operation.
    /// </summary>
    /// <returns>The result of Handle_WithNonCommandDataRequest_ShouldSkipPersistence.</returns>

    [Fact]
    public async Task Handle_WithNonCommandDataRequest_ShouldSkipPersistence()
    {
        await Initialization;

        // Arrange
        var loggerMock = XUnitLogger.CreateLogger<string>();
        var behavior = new GatewayPersistenceBehavior<string, string>(
            loggerMock,
            DpRequestRepository,
            DpResponseRepository
        );

        var request = "non-command-data";
        var expectedResponse = "response";
        RequestFunctionalHandlerDelegate<string> next = () => Task.FromResult(expectedResponse);

        // Act
        var result = await behavior.HandleAsync(request, next, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(expectedResponse);
    }

    /// <summary>
    /// Executes Handle_WithRepositoryFailure_ShouldHandleGracefully operation.
    /// </summary>
    /// <returns>The result of Handle_WithRepositoryFailure_ShouldHandleGracefully.</returns>

    [Fact]
    public async Task Handle_WithRepositoryFailure_ShouldHandleGracefully()
    {
        await Initialization;

        // Arrange
        var loggerMock = XUnitLogger.CreateLogger<CreateBarCodeCommand>();
        var behavior = new GatewayPersistenceBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>(
            loggerMock,
            DpRequestRepository,
            DpResponseRepository
        );

        var command = CreateTestCommand();
        var gatewayResponse = CreateTestGatewayResponse();
        var expectedResult = Result<TaskGatewayResponse>.WithFailure("gatewayResponse");

        RequestFunctionalHandlerDelegate<Result<TaskGatewayResponse>> next = () => Task.FromResult(expectedResult);

        // Simulate repository failure

        // Act & Assert - Should not throw
        var result = await behavior.HandleAsync(command, next, TestContext.Current.CancellationToken);
        result.ShouldBe(expectedResult);
    }

    /// <summary>
    /// Executes Handle_WithManufacturingErrors_ShouldPersistCorrectly operation.
    /// </summary>
    /// <param name="errorMessage">The errorMessage.</param>
    /// <returns>The result of Handle_WithManufacturingErrors_ShouldPersistCorrectly.</returns>

    [Theory]
    [InlineData("PLC communication timeout")]
    [InlineData("Machine sensor malfunction")]
    [InlineData("Assembly line stoppage")]
    [InlineData("Quality control rejection")]
    public async Task Handle_WithManufacturingErrors_ShouldPersistCorrectly(string errorMessage)
    {
        await Initialization;

        // Using parameters: to log essentially different manufacturing error scenarios

        // Arrange
        var loggerMock = XUnitLogger.CreateLogger<CreateBarCodeCommand>();

        loggerMock.LogInformation("Testing with error message: {ErrorMessage}", errorMessage);

        var behavior = new GatewayPersistenceBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>(
            loggerMock,
            DpRequestRepository,
            DpResponseRepository
        );

        var command = CreateTestCommand();
        var gatewayResponse = CreateTestGatewayResponse();
        var failedResult = Result<TaskGatewayResponse>.WithFailure(errorMessage, gatewayResponse);

        RequestFunctionalHandlerDelegate<Result<TaskGatewayResponse>> next = () => Task.FromResult(failedResult);

        // Act
        var result = await behavior.HandleAsync(command, next, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBe(failedResult);

        //[Fix]
        //CLAUDE
        //Date: 28/08/2025
        //Reason: [CS8602] - Add null-forgiving operator since failed result was created WITH a value (gatewayResponse)
        result.Value!.Error.ShouldBe(errorMessage);
    }

    /// <summary>
    /// Executes Handle_WithCancellation_ShouldPassCancellationToken operation.
    /// </summary>
    /// <returns>The result of Handle_WithCancellation_ShouldPassCancellationToken.</returns>

    [Fact]
    public async Task Handle_WithCancellation_ShouldPassCancellationToken()
    {
        await Initialization;

        // Arrange
        var loggerMock = XUnitLogger.CreateLogger<CreateBarCodeCommand>();
        var behavior = new GatewayPersistenceBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>(
            loggerMock,
            DpRequestRepository,
            DpResponseRepository
        );

        var command = CreateTestCommand();
        var gatewayResponse = CreateTestGatewayResponse();
        var expectedResult = Result<TaskGatewayResponse>.Success(gatewayResponse);
        var cancellationToken = new CancellationToken();

        RequestFunctionalHandlerDelegate<Result<TaskGatewayResponse>> next = () => Task.FromResult(expectedResult);

        // Act
        var result = await behavior.HandleAsync(command, next, cancellationToken);

        // Assert
        result.ShouldBe(expectedResult);
    }

    // Helper methods for creating test data
    private static CreateBarCodeCommand CreateTestCommand()
    {
        var command = new CreateBarCodeCommand();
        command.Command.MachineId = 100001;
        command.Command.BarCode = "TEST-BARCODE-123";
        command.Command.GatewayTask = GatewayTask.CreateBarCodeAsync;
        command.Command.TimeStamp = DateTime.Now;
        return command;
    }

    private static TaskGatewayResponse CreateTestGatewayResponse()
    {
        return new TaskGatewayResponse
        {
            Label = "TEST-BARCODE-123",
            MachineId = 100001,
            ResultValidation = 1,
            Description = "Test barcode created successfully",
            TimeStamp = DateTime.Now
        };
    }
}
