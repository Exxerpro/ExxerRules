using IndTrace.Application.Shifts.Commands.Update;

namespace Application.UnitTests.Features.Shifts;

/// <summary>
/// Unit tests for UpdateShiftCommandHandler
/// </summary>
public class UpdateShiftCommandHandlerTests
{
    private readonly IRepository<Shift> _repository = null!;
    private readonly ILogger<UpdateShiftCommandHandler> _logger = null!;
    private readonly UpdateShiftCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public UpdateShiftCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Shift>>();
        _logger = XUnitLogger.CreateLogger<UpdateShiftCommandHandler>();
        _handler = new UpdateShiftCommandHandler(_repository, _logger);
    }

    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new UpdateShiftCommandHandler(_repository, _logger);

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
    //         IRepository<Shift>? nullRepository = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new UpdateShiftCommandHandler(nullRepository!, _logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         ILogger<UpdateShiftCommandHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new UpdateShiftCommandHandler(_repository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Process_WithValidCommand_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidCommand_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var existingShift = new Shift(new DateTimeMachine())
        {
            ShiftId = 1,
            ShiftType = "Morning",
            StartBy = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Local),
            Duration = TimeSpan.FromHours(8)
        };

        var command = new UpdateShiftCommand
        {
            ShiftId = 1,
            ShiftName = "Evening Shift",
            Description = "Updated Description"
        };

        _repository.GetByIdAsync(command.ShiftId ?? 0, Arg.Any<CancellationToken>())
            .Returns(Result<Shift?>.Success(existingShift));
        _repository.UpdateAsync(Arg.Any<Shift>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        await _repository.Received(1).GetByIdAsync(command.ShiftId ?? 0, Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(Arg.Any<Shift>(), Arg.Any<CancellationToken>());
        await _repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Should_Return_Failure_When_Entity_Not_Found operation.
    /// </summary>
    /// <returns>The result of Should_Return_Failure_When_Entity_Not_Found.</returns>

    [Fact]
    public async Task Should_Return_Failure_When_Entity_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();

        _repository.GetByIdAsync(command.ShiftId ?? 0, Arg.Any<CancellationToken>())
                   .Returns(Result<Shift?>.WithFailure("Entity not found in database"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
    }

    /// <summary>
    /// Executes Should_Return_Failure_When_Update_Fails operation.
    /// </summary>
    /// <returns>The result of Should_Return_Failure_When_Update_Fails.</returns>

    [Fact]
    public async Task Should_Return_Failure_When_Update_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingShift = CreateExistingShift();

        _repository.GetByIdAsync(command.ShiftId ?? 0, Arg.Any<CancellationToken>())
                   .Returns(Result<Shift?>.Success(existingShift));
        _repository.UpdateAsync(Arg.Any<Shift>(), Arg.Any<CancellationToken>())
                   .Returns(Result.WithFailure("Database update failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Database update failed");
    }

    /// <summary>
    /// Executes Should_Return_Failure_When_Commit_Fails operation.
    /// </summary>
    /// <returns>The result of Should_Return_Failure_When_Commit_Fails.</returns>

    [Fact]
    public async Task Should_Return_Failure_When_Commit_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        var existingShift = CreateExistingShift();

        _repository.GetByIdAsync(command.ShiftId ?? 0, Arg.Any<CancellationToken>())
                   .Returns(Result<Shift?>.Success(existingShift));
        _repository.UpdateAsync(Arg.Any<Shift>(), Arg.Any<CancellationToken>())
                   .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
                   .Returns(Result.WithFailure("Transaction commit failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Transaction commit failed");
    }

    /// <summary>
    /// Executes Process_ShouldPassCancellationTokenToRepository operation.
    /// </summary>
    /// <returns>The result of Process_ShouldPassCancellationTokenToRepository.</returns>

    [Fact]
    public async Task Process_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var existingShift = new Shift(new DateTimeMachine())
        { ShiftId = 1, ShiftType = "Morning" };
        var command = new UpdateShiftCommand { ShiftId = 1 };
        var cancellationToken = new CancellationToken();

        _repository.GetByIdAsync(command.ShiftId ?? 0, Arg.Any<CancellationToken>())
            .Returns(Result<Shift?>.Success(existingShift));
        _repository.UpdateAsync(Arg.Any<Shift>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        await _handler.ProcessAsync(command, cancellationToken);

        // Assert
        await _repository.Received(1).GetByIdAsync(command.ShiftId ?? 0, cancellationToken);
        await _repository.Received(1).UpdateAsync(Arg.Any<Shift>(), cancellationToken);
        await _repository.Received(1).CommitAsync(cancellationToken);
    }

    private static UpdateShiftCommand CreateValidCommand()
    {
        return new UpdateShiftCommand
        {
            ShiftId = 1,
            ShiftName = "Updated Shift",
            Description = "Updated Description",
            IsActive = 1,
            Version = 1
        };
    }

    private static Shift CreateExistingShift()
    {
        return new Shift(new DateTimeMachine())
        {
            ShiftId = 1,
            ShiftType = "Morning",
            StartBy = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Local).AddHours(-1),
            Duration = TimeSpan.FromHours(8),
            EndTime = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Local).AddHours(7)
        };
    }
}
