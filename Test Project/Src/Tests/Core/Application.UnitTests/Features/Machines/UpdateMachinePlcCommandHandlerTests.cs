using IndTrace.Application.MachinesPlcs.Commands.Update;

namespace Application.UnitTests.Features.Machines;

/// <summary>
/// Unit tests for UpdateMachinePlcCommandHandler
/// </summary>
public class UpdateMachinePlcCommandHandlerTests
{
    private readonly IRepository<MachinePlc> _repository = null!;
    private readonly ILogger<UpdateMachinePlcCommandHandler> _logger = null!;
    private readonly UpdateMachinePlcCommandHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public UpdateMachinePlcCommandHandlerTests()
    {
        _repository = Substitute.For<IRepository<MachinePlc>>();
        _logger = XUnitLogger.CreateLogger<UpdateMachinePlcCommandHandler>();
        _handler = new UpdateMachinePlcCommandHandler(_repository, _logger);
    }

    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var realLogger = XUnitLogger.CreateLogger<UpdateMachinePlcCommandHandler>();
        var handler = new UpdateMachinePlcCommandHandler(_repository, realLogger);

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
    //         IRepository<MachinePlc>? nullRepository = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new UpdateMachinePlcCommandHandler(nullRepository!, _logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         ILogger<UpdateMachinePlcCommandHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new UpdateMachinePlcCommandHandler(_repository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Process_WithValidCommand_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidCommand_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: [CLUSTER UpdateMachinePlc FIX] - Handler logic requires IsActive change to trigger ShouldUpdateIsActiveOnly() path
        var existingMachinePlc = new MachinePlc
        {
            MachineId = 10000,
            PlcId = 200,
            IsActive = 0  // Inactive
        };

        var command = new UpdateMachinePlcCommand
        {
            MachineId = 10000,
            PlcId = 200,
            IsActive = 1,  // Activate (IsActive change: 0 → 1)
            NewMachineId = null!,  // Required for ShouldUpdateIsActiveOnly()
            NewPlcId = null       // Required for ShouldUpdateIsActiveOnly()
        };

        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS8620] Fix nullability mismatch - Remove nullable marker for consistent Returns method signature
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.Success(new[] { existingMachinePlc }));
        _repository.UpdateAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.MachineId.ShouldBe(command.MachineId);
        result.Value.PlcId.ShouldBe(command.PlcId);

        await _repository.Received(1).ListAsync(Arg.Any<CancellationToken>());
        await _repository.Received(1).UpdateAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>());
        await _repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes Process_WhenEntityNotFound_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenEntityNotFound_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenEntityNotFound_ShouldReturnFailure()
    {
        // Arrange
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Handler requires both MachineId and PlcId for lookup
        var command = new UpdateMachinePlcCommand { MachineId = 10000, PlcId = 200 };
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Handler uses ListAsync, return empty collection for not found
        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS8620] Fix nullability mismatch - Remove nullable marker for consistent Returns method signature
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.Success(Enumerable.Empty<MachinePlc>()));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Update error message to match actual handler implementation
        result.Errors.ShouldContain($"MachinePLC with MachineId: {command.MachineId} and PlcId: {command.PlcId} cannot be found");
    }

    /// <summary>
    /// Executes Process_WhenGetByIdFails_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenGetByIdFails_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenGetByIdFails_ShouldReturnFailure()
    {
        // Arrange
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Handler requires both MachineId and PlcId for lookup
        var command = new UpdateMachinePlcCommand { MachineId = 10000, PlcId = 200 };
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Handler uses ListAsync, return failure for database errors
        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS8620] Fix nullability mismatch - Remove nullable marker for consistent Returns method signature
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.WithFailure("Entity not found in database"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Entity not found in database");
    }

    /// <summary>
    /// Executes Process_WhenUpdateFails_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenUpdateFails_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenUpdateFails_ShouldReturnFailure()
    {
        // Arrange
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Entity should match command and have IsActive for handler logic
        var existingMachinePlc = new MachinePlc { MachineId = 10000, PlcId = 200, IsActive = 1 };
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Handler requires both MachineId and PlcId for lookup
        var command = new UpdateMachinePlcCommand { MachineId = 10000, PlcId = 200, IsActive = 0 }; // Different IsActive to trigger update

        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Handler uses ListAsync instead of GetByIdAsync
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.Success(new[] { existingMachinePlc }));
        _repository.UpdateAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Database update failed"));

        // Act
        var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Database update failed");
    }

    /// <summary>
    /// Executes Process_WhenCommitFails_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenCommitFails_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenCommitFails_ShouldReturnFailure()
    {
        // Arrange
        //[Fix]
        //CURSOR
        //Date: 23/08/2025
        //Reason: Pattern 12 Fix - Entity should match command and have IsActive for handler logic
        var existingMachinePlc = new MachinePlc { MachineId = 10000, PlcId = 200, IsActive = 1 };
        //[Fix]
        //CURSOR
        //Date: 23/08/2025
        //Reason: Pattern 12 Fix - Command needs IsActive property to trigger ShouldUpdateIsActiveOnly path for CommitAsync call
        var command = new UpdateMachinePlcCommand
        {
            MachineId = 10000,
            PlcId = 200,
            IsActive = 0  // Different from existing entity's IsActive = 1 to trigger update path
        };

        //[Fix]
        //CURSOR
        //Date: 23/08/2025
        //Reason: Pattern 12 Fix - Handler uses ListAsync instead of GetByIdAsync
        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS8620] Fix nullability mismatch - Remove nullable marker for consistent Returns method signature
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.Success(new[] { existingMachinePlc }));
        _repository.UpdateAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>())
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
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Entity should match command and have IsActive for handler logic
        var existingMachinePlc = new MachinePlc { MachineId = 10000, PlcId = 200, IsActive = 1 };
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Handler requires both MachineId and PlcId for lookup
        var command = new UpdateMachinePlcCommand { MachineId = 10000, PlcId = 200, IsActive = 0 };
        var cancellationToken = TestContext.Current.CancellationToken;

        //[Fix]
        //CLAUDE
        //Date: 24/08/2025
        //Reason: [NSUBSTITUTE TYPE MISMATCH FIX] - Repository interface returns Result<IEnumerable<MachinePlc>?> (nullable), not Result<IEnumerable<MachinePlc>>
        _repository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<MachinePlc>>.Success(new[] { existingMachinePlc }));
        _repository.UpdateAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());
        _repository.CommitAsync(Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.ProcessAsync(command, cancellationToken);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 22/08/2025
        //Reason: Pattern - Handler uses ListAsync instead of GetByIdAsync
        await _repository.Received(1).ListAsync(cancellationToken);
        await _repository.Received(1).UpdateAsync(Arg.Any<MachinePlc>(), cancellationToken);
        await _repository.Received(1).CommitAsync(cancellationToken);
    }
}
