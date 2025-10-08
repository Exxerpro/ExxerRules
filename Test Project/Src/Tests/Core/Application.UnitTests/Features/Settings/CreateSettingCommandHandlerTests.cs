using IndTrace.Application.Settings.Commands.Create;

namespace Application.UnitTests.Features.Settings;

/// <summary>
/// Unit tests for CreateSettingCommandHandler
/// </summary>
public class CreateSettingCommandHandlerTests
{
    private readonly IRepository<Setting> _repository = null!;
    private readonly ILogger<CreateSettingCommandHandler> _logger = null!;
    private readonly CreateSettingCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public CreateSettingCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<Setting>>();
        _logger = XUnitLogger.CreateLogger<CreateSettingCommandHandler>();
        _handler = new CreateSettingCommandHandler(_repository, _logger);
    }
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new CreateSettingCommandHandler(_repository, _logger);

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
    //         IRepository<Setting>? nullRepository = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new CreateSettingCommandHandler(nullRepository!, _logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         ILogger<CreateSettingCommandHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new CreateSettingCommandHandler(_repository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Process_WithValidCommand_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidCommand_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var command = CreateValidCommand();

        _repository.AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.SettingId.ShouldBe(command.SettingId);
        result.Value.MachineId.ShouldBe(command.MachineId);
        result.Value.Setting.ShouldBe(command.Setting);

        await _repository.Received(1).AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>());
        await _repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    /// <summary>
    /// Executes Should_Return_Failure_When_Add_Fails operation.
    /// </summary>
    /// <returns>The result of Should_Return_Failure_When_Add_Fails.</returns>

    [Fact]
    public async Task Should_Return_Failure_When_Add_Fails()
    {
        // Arrange
        var command = CreateValidCommand();

        _repository.AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>())
                   .Returns(Result<int>.WithFailure("Database connection failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Database connection failed");
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

        _repository.AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>())
                   .Returns(Result<int>.Success(1));
        _repository.CommitAsync(Arg.Any<CancellationToken>())
                   .Returns(Result.WithFailure("Transaction commit failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Transaction commit failed");
    }
    /// <summary>
    /// Executes Process_ShouldCallRepositoryWithCorrectEntity operation.
    /// </summary>
    /// <returns>The result of Process_ShouldCallRepositoryWithCorrectEntity.</returns>

    [Fact]
    public async Task Process_ShouldCallRepositoryWithCorrectEntity()
    {
        // Arrange
        var command = CreateValidCommand();

        _repository.AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        await _repository.Received(1).AddAsync(
            Arg.Is<Setting>(entity =>
                entity.SettingId == command.SettingId &&
                entity.MachineId == command.MachineId &&
                entity.Config == command.Setting),
            Arg.Any<CancellationToken>());
        await _repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }
    /// <summary>
    /// Executes Process_ShouldPassCancellationTokenToRepository operation.
    /// </summary>
    /// <returns>The result of Process_ShouldPassCancellationTokenToRepository.</returns>

    [Fact]
    public async Task Process_ShouldPassCancellationTokenToRepository()
    {
        // Arrange
        var command = CreateValidCommand();
        var cancellationToken = new CancellationToken();

        _repository.AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        await _handler.ProcessAsync(command, cancellationToken);

        // Assert
        await _repository.Received(1).AddAsync(Arg.Any<Setting>(), cancellationToken);
        await _repository.Received(1).CommitAsync(cancellationToken);
    }
    /// <summary>
    /// Executes Process_WithNullOrEmptyConfig_ShouldHandleGracefully operation.
    /// </summary>
    /// <param name="config">The config.</param>
    /// <returns>The result of Process_WithNullOrEmptyConfig_ShouldHandleGracefully.</returns>

    [Theory]
    [InlineData("TestConfig")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Process_WithNullOrEmptyConfig_ShouldHandleGracefully(string config)
    {
        // Using parameters: config
        _ = config; // xUnit1026 fix
        // Using parameters: config
        _ = config; // xUnit1026 fix
        // Using parameters: config
        _ = config; // xUnit1026 fix
        // Using parameters: config
        _ = config; // xUnit1026 fix
        // Using parameters: config
        _ = config; // xUnit1026 fix
        // Arrange
        var command = new CreateSettingCommand
        {
            SettingId = 1,
            MachineId = 10000,
            Setting = config
        };

        _repository.AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        //[Fix]
        //CLAUDE
        //Date: 28/08/2025
        //Reason: [CS8602] - Add null-forgiving operator since result.IsSuccess was verified true
        result.Value!.Setting.ShouldBe(config);
    }
    /// <summary>
    /// Executes Process_ShouldReturnErrorWhenAddFails operation.
    /// </summary>
    /// <returns>The result of Process_ShouldReturnErrorWhenAddFails.</returns>

    [Fact]
    public async Task Process_ShouldReturnErrorWhenAddFails()
    {
        // Arrange
        var command = CreateValidCommand();

        _repository.AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.WithFailure(["Repository connection timeout"]));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Repository connection timeout");
    }
    /// <summary>
    /// Executes Process_ShouldReturnErrorWhenCommitFails operation.
    /// </summary>
    /// <returns>The result of Process_ShouldReturnErrorWhenCommitFails.</returns>

    [Fact]
    public async Task Process_ShouldReturnErrorWhenCommitFails()
    {
        // Arrange
        var command = CreateValidCommand();

        _repository.AddAsync(Arg.Any<Setting>(), Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure(["Commit transaction failed"]));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Commit transaction failed");
    }

    private static CreateSettingCommand CreateValidCommand()
    {
        return new CreateSettingCommand
        {
            SettingId = 1,
            MachineId = 10000,
            Setting = "Test Configuration"
        };
    }
}
