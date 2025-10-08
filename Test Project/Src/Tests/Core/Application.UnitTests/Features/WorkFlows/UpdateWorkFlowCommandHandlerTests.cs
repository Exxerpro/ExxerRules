namespace Application.UnitTests.Features.WorkFlows;

/// <summary>
/// Unit tests for UpdateWorkFlowCommandHandler
/// </summary>
public class UpdateWorkFlowCommandHandlerTests
{
    private readonly IRepository<WorkFlow> _repository = Substitute.For<IRepository<WorkFlow>>();
    private readonly ILogger<UpdateWorkFlowCommandHandler> _logger = XUnitLogger.CreateLogger<UpdateWorkFlowCommandHandler>();
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new UpdateWorkFlowCommandHandler(_repository, _logger);

        // Assert
        handler.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes Constructor_WithNullRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullRepository_ShouldThrowException()
    //     {
    //         // Arrange
    //         IRepository<WorkFlow>? nullRepository = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new UpdateWorkFlowCommandHandler(nullRepository!, _logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         ILogger<UpdateWorkFlowCommandHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new UpdateWorkFlowCommandHandler(_repository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Process_WithValidCommand_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidCommand_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var handler = new UpdateWorkFlowCommandHandler(_repository, _logger);
        var existingWorkFlow = new WorkFlow
        {
            WorkFlowId = 1,
            ProductId = 5080,
            NextMachineId = 1000,
            LastMachineId = 20
        };

        var command = new UpdateWorkFlowCommand
        {
            WorkFlowId = 1,
            ProductId = 5081,
            NextMachineId = 1005,
            LastMachineId = 25
        };

        _repository.GetByIdAsync(command.WorkFlowId ?? 0, Arg.Any<CancellationToken>())
            .Returns(Result<WorkFlow?>.Success(existingWorkFlow));
        _repository.UpdateAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes Process_WhenEntityNotFound_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenEntityNotFound_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenEntityNotFound_ShouldReturnFailure()
    {
        // Arrange
        var handler = new UpdateWorkFlowCommandHandler(_repository, _logger);
        var command = new UpdateWorkFlowCommand { WorkFlowId = 1 };
        _repository.GetByIdAsync(command.WorkFlowId ?? 0, Arg.Any<CancellationToken>())
            .Returns(Result<WorkFlow?>.Success(null));

        // Act
        var result = await handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors?.ShouldContain($"WorkFlowId {command.WorkFlowId} does not exist");
    }
    /// <summary>
    /// Executes Process_WhenUpdateFails_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenUpdateFails_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenUpdateFails_ShouldReturnFailure()
    {
        // Arrange
        var handler = new UpdateWorkFlowCommandHandler(_repository, _logger);
        var existingWorkFlow = new WorkFlow { WorkFlowId = 1, ProductId = 5080, NextMachineId = 1000, LastMachineId = 20 };
        var command = new UpdateWorkFlowCommand { WorkFlowId = 1 };

        _repository.GetByIdAsync(command.WorkFlowId ?? 0, Arg.Any<CancellationToken>())
            .Returns(Result<WorkFlow?>.Success(existingWorkFlow));
        _repository.UpdateAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Database update failed"));

        // Act
        var result = await handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors?.ShouldContain("Database update failed");
    }
}
