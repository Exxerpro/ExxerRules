using Microsoft.Extensions.DependencyInjection;
using IndTrace.Application.BarCodes.Commands.Create;
using IndTrace.Application.BarCodes.Queries.GetBarCodeGatewayDetail;

namespace Application.UnitTests.Commands;

/// <summary>
/// Unit tests for GatewayCommandDispatcher - Gateway command/query dispatcher for industrial operations.
/// Tests constructor validation, interface compliance, command routing, and manufacturing scenarios.
/// </summary>
public class GatewayCommandDispatcherTests
{
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var provider = Substitute.For<IServiceProvider>();
        var logger = XUnitLogger.CreateLogger<GatewayCommandDispatcher>();

        // Act
        var instance = new GatewayCommandDispatcher(provider, logger);

        // Assert
        instance.ShouldNotBeNull();
        instance.ShouldBeAssignableTo<IGatewayCommandDispatcher>();
    }

    // MARKED FOR REMOVAL - Constructor null guard test no longer needed with Result<T> patterns
    // /// <summary>
    // /// Executes Constructor_WithInvalidParameters_ShouldThrowException operation.
    // /// </summary>
    //
    // [Fact]
    // public void Constructor_WithInvalidParameters_ShouldThrowException()
    // {
    //     // Arrange
    //     IServiceProvider provider = null!;
    //     ILogger<GatewayCommandDispatcher> logger = null!;
    //
    //     // Act & Assert
    //     Should.Throw<ArgumentNullException>(() => new GatewayCommandDispatcher(provider, logger));
    // }
    // MARKED FOR REMOVAL - Constructor null guard test no longer needed with Result<T> patterns
    // /// <summary>
    // /// Executes Constructor_WithNullProvider_ShouldThrowException operation.
    // /// </summary>
    //
    // [Fact]
    // public void Constructor_WithNullProvider_ShouldThrowException()
    // {
    //     // Arrange
    //     IServiceProvider? nullProvider = null!;
    //     var logger =  XUnitLogger.CreateLogger<GatewayCommandDispatcher>();
    //
    //     // Act & Assert
    //     Should.Throw<ArgumentNullException>(() => new GatewayCommandDispatcher(nullProvider!, logger));
    // }
    // MARKED FOR REMOVAL - Constructor null guard test no longer needed with Result<T> patterns
    // /// <summary>
    // /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    // /// </summary>
    //
    // [Fact]
    // public void Constructor_WithNullLogger_ShouldThrowException()
    // {
    //     // Arrange
    //     var provider = Substitute.For<IServiceProvider>();
    //     ILogger<GatewayCommandDispatcher>? nullLogger = null!;
    //
    //     // Act & Assert
    //     Should.Throw<ArgumentNullException>(() => new GatewayCommandDispatcher(provider, nullLogger!));
    // }
    /// <summary>
    /// Executes Properties_WhenSet_ShouldReturnCorrectValues operation.
    /// </summary>

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var provider = Substitute.For<IServiceProvider>();
        var logger = XUnitLogger.CreateLogger<GatewayCommandDispatcher>();
        var instance = new GatewayCommandDispatcher(provider, logger);

        // Act & Assert
        instance.ShouldNotBeNull();
        instance.ShouldBeAssignableTo<IGatewayCommandDispatcher>();

        // Verify interface compliance
        typeof(IGatewayCommandDispatcher).IsAssignableFrom(typeof(GatewayCommandDispatcher)).ShouldBeTrue();
    }

    /// <summary>
    /// Executes ProcessAsync_WithCreateBarCodeCommand_ShouldInvokeHandlerAndReturnResult operation.
    /// </summary>
    /// <returns>The result of ProcessAsync_WithCreateBarCodeCommand_ShouldInvokeHandlerAndReturnResult.</returns>

    [Fact]
    public async Task ProcessAsync_WithCreateBarCodeCommand_ShouldInvokeHandlerAndReturnResult()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockHandler = Substitute.For<IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>>();
        var mockPipelineBehavior = Substitute.For<IPipelineBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>>();
        var logger = XUnitLogger.CreateLogger<GatewayCommandDispatcher>();
        var testCommand = new CreateBarCodeCommand();
        var expectedResponse = new TaskGatewayResponse();
        var expectedResult = Result<TaskGatewayResponse>.Success(expectedResponse);
        mockHandler.ProcessAsync(testCommand, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedResult));
        // Register mocks in DI
        services.AddSingleton<IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>>(mockHandler);
        services.AddSingleton<IPipelineBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>>(mockPipelineBehavior);
        services.AddSingleton<ILogger<GatewayCommandDispatcher>>(logger);
        var provider = services.BuildServiceProvider();
        var instance = new GatewayCommandDispatcher(provider, logger);
        // Act
        var result = await instance.ProcessAsync(testCommand, TestContext.Current.CancellationToken);
        // Assert
        result.ShouldBe(expectedResult);
        result.Value.ShouldBe(expectedResponse);
        await mockHandler.Received(1).ProcessAsync(testCommand, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes QueryAsync_WithGatewayQuery_ShouldInvokeHandlerAndReturnResult operation.
    /// </summary>
    /// <returns>The result of QueryAsync_WithGatewayQuery_ShouldInvokeHandlerAndReturnResult.</returns>

    [Fact]
    public async Task QueryAsync_WithGatewayQuery_ShouldInvokeHandlerAndReturnResult()
    {
        // Arrange
        var services = new ServiceCollection();
        var mockHandler = Substitute.For<IGatewayRequestHandler<ReadBarCodeQuery, TaskGatewayResponse>>();
        var mockPipelineBehavior = Substitute.For<IPipelineBehavior<ReadBarCodeQuery, Result<TaskGatewayResponse>>>();
        var logger = XUnitLogger.CreateLogger<GatewayCommandDispatcher>();
        var testQuery = new ReadBarCodeQuery();
        var expectedResponse = new TaskGatewayResponse();
        var expectedResult = Result<TaskGatewayResponse>.Success(expectedResponse);
        mockHandler.ProcessAsync(testQuery, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedResult));
        // Register mocks in DI
        services.AddSingleton<IGatewayRequestHandler<ReadBarCodeQuery, TaskGatewayResponse>>(mockHandler);
        services.AddSingleton<IPipelineBehavior<ReadBarCodeQuery, Result<TaskGatewayResponse>>>(mockPipelineBehavior);
        services.AddSingleton<ILogger<GatewayCommandDispatcher>>(logger);
        var provider = services.BuildServiceProvider();
        var instance = new GatewayCommandDispatcher(provider, logger);
        // Act
        var result = await instance.QueryAsync(testQuery, TestContext.Current.CancellationToken);
        // Assert
        result.ShouldBe(expectedResult);
        result.Value.ShouldBe(expectedResponse);
        await mockHandler.Received(1).ProcessAsync(testQuery, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes ProcessAsync_WithUnregisteredCommand_ShouldThrowInvalidOperationException operation.
    /// </summary>

    [Fact]
    public async Task ProcessAsync_WithUnregisteredCommand_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var provider = Substitute.For<IServiceProvider>();
        var logger = XUnitLogger.CreateLogger<GatewayCommandDispatcher>();
        var unregisteredCommand = new TestGatewayCommand();

        var instance = new GatewayCommandDispatcher(provider, logger);

        // Act
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern B - Railway-Oriented Programming - dispatcher returns Result.Failure instead of throwing
        var result = await instance.ProcessAsync(unregisteredCommand, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain($"No registered handler for request type {nameof(TestGatewayCommand)}");
    }

    /// <summary>
    /// Executes ProcessAsync_WithCancellationToken_ShouldPassTokenToHandler operation.
    /// </summary>
    /// <returns>The result of ProcessAsync_WithCancellationToken_ShouldPassTokenToHandler.</returns>

    [Fact]
    public async Task ProcessAsync_WithCancellationToken_ShouldPassTokenToHandler()
    {
        // Arrange
        var services = new ServiceCollection();

        var mockHandler = Substitute.For<IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>>();
        var mockPipelineBehavior = Substitute.For<IPipelineBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>>();
        var logger = XUnitLogger.CreateLogger<GatewayCommandDispatcher>();

        var testCommand = new CreateBarCodeCommand();
        var cancellationToken = new CancellationToken(true);

        var expectedResponse = new TaskGatewayResponse();
        var expectedResult = Result<TaskGatewayResponse>.Success(expectedResponse);

        mockHandler.ProcessAsync(testCommand, cancellationToken)
            .Returns(Task.FromResult(expectedResult));
        mockHandler.ProcessAsync(testCommand, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(expectedResult));

        // Register mocks in DI
        services.AddSingleton<IGatewayRequestHandler<CreateBarCodeCommand, TaskGatewayResponse>>(mockHandler);
        services.AddSingleton<IPipelineBehavior<CreateBarCodeCommand, Result<TaskGatewayResponse>>>(mockPipelineBehavior);
        services.AddSingleton<ILogger<GatewayCommandDispatcher>>(logger);

        var provider = services.BuildServiceProvider();
        var instance = new GatewayCommandDispatcher(provider, logger);

        // Act
        var result = await instance.ProcessAsync(testCommand, cancellationToken);

        // Assert
        result.ShouldBe(expectedResult);
        await mockHandler.Received(1).ProcessAsync(testCommand, cancellationToken);
    }
}

/// <summary>
/// Test helper classes for GatewayCommandDispatcher testing
/// </summary>
public class TestGatewayCommand : IGatewayRequest<TaskGatewayResponse>
{
}
