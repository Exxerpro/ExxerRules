using IndTrace.Application.MachinesPlcs.Commands.Create;

namespace Application.UnitTests.Features.Machines
{
    /// <summary>
    /// Unit tests for CreateMachinePlcCommandHandler
    /// </summary>
    public class CreateMachinePlcCommandHandlerTests
    {
        private readonly IRepository<MachinePlc> _repository = null!;
        private readonly ILogger<CreateMachinePlcCommandHandler> _logger = null!;
        private readonly CreateMachinePlcCommandHandler _handler = null!;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>

        public CreateMachinePlcCommandHandlerTests()
        {
            _repository = Substitute.For<IRepository<MachinePlc>>();
            _logger = XUnitLogger.CreateLogger<CreateMachinePlcCommandHandler>();
            _handler = new CreateMachinePlcCommandHandler(_repository, _logger);
        }
        /// <summary>
        /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
        /// </summary>

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange & Act
            var handler = new CreateMachinePlcCommandHandler(_repository, _logger);

            // Assert
            handler.ShouldNotBeNull();
        }
        /// <summary>
        /// Executes Constructor_WithNullRepository_ShouldThrowException operation.
        /// </summary>

        // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
        // [Fact]
        //         public void Constructor_WithNullRepository_ShouldThrowException()
        //         {
        //             // Arrange
        //             IRepository<MachinePlc>? nullRepository = null!;
        //
        //             // Act & Assert
        //             Should.Throw<ArgumentNullException>(() => new CreateMachinePlcCommandHandler(nullRepository!, _logger));
        //         }
        /// <summary>
        /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
        /// </summary>

        // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
        // [Fact]
        //         public void Constructor_WithNullLogger_ShouldThrowException()
        //         {
        //             // Arrange
        //             ILogger<CreateMachinePlcCommandHandler>? nullLogger = null!;
        //
        //             // Act & Assert
        //             Should.Throw<ArgumentNullException>(() => new CreateMachinePlcCommandHandler(_repository, nullLogger!));
        //         }
        /// <summary>
        /// Executes Process_WithValidCommand_ShouldReturnSuccess operation.
        /// </summary>
        /// <returns>The result of Process_WithValidCommand_ShouldReturnSuccess.</returns>

        [Fact]
        public async Task Process_WithValidCommand_ShouldReturnSuccess()
        {
            // Arrange
            var command = CreateValidCommand();

            _repository.AddAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>())
                .Returns(Result<int>.Success(1));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.MachineId.ShouldBe(command.MachineId);
            result.Value.PlCsId.ShouldBe(command.PlCsId);

            await _repository.Received(1).AddAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>());
            await _repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }
        /// <summary>
        /// Executes Process_WhenAddFails_ShouldReturnFailure operation.
        /// </summary>
        /// <returns>The result of Process_WhenAddFails_ShouldReturnFailure.</returns>

        [Fact]
        public async Task Process_WhenAddFails_ShouldReturnFailure()
        {
            // Arrange
            var command = CreateValidCommand();

            _repository.AddAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>())
                .Returns(Result<int>.WithFailure("Database connection failed"));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Errors.ShouldContain("Database connection failed");
        }
        /// <summary>
        /// Executes Process_WhenCommitFails_ShouldReturnFailure operation.
        /// </summary>
        /// <returns>The result of Process_WhenCommitFails_ShouldReturnFailure.</returns>

        [Fact]
        public async Task Process_WhenCommitFails_ShouldReturnFailure()
        {
            // Arrange
            var command = CreateValidCommand();

            _repository.AddAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>())
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
        /// Executes Process_ShouldPassCancellationTokenToRepository operation.
        /// </summary>
        /// <returns>The result of Process_ShouldPassCancellationTokenToRepository.</returns>

        [Fact]
        public async Task Process_ShouldPassCancellationTokenToRepository()
        {
            // Arrange
            var command = CreateValidCommand();
            var cancellationToken = TestContext.Current.CancellationToken;

            _repository.AddAsync(Arg.Any<MachinePlc>(), Arg.Any<CancellationToken>())
                .Returns(Result<int>.Success(1));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            // Act
            await _handler.ProcessAsync(command, cancellationToken);

            // Assert
            await _repository.Received(1).AddAsync(Arg.Any<MachinePlc>(), cancellationToken);
            await _repository.Received(1).CommitAsync(cancellationToken);
        }

        private static CreateMachinePlcCommand CreateValidCommand()
        {
            return new CreateMachinePlcCommand
            {
                MachineId = 10000,
                PlCsId = 200
            };
        }
    }
}
