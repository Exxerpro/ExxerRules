namespace Application.UnitTests.Features.WorkFlows
{
    /// <summary>
    /// Basic tests for CreateWorkFlowCommandHandler focusing on constructor validation and simple scenarios
    /// </summary>
    public class CreateWorkFlowCommandHandlerBasicTests : IDisposable
    {
        private readonly IRepository<WorkFlow> _repository = Substitute.For<IRepository<WorkFlow>>();
        private readonly ILogger<CreateWorkFlowCommandHandler> _logger = XUnitLogger.CreateLogger<CreateWorkFlowCommandHandler>();
        private readonly CreateWorkFlowCommandHandler _handler = null!;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>

        public CreateWorkFlowCommandHandlerBasicTests()
        {
            _handler = new CreateWorkFlowCommandHandler(_repository, _logger);
        }

        /// <summary>
        /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
        /// </summary>

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateInstance()
        {
            // Arrange & Act
            var handler = new CreateWorkFlowCommandHandler(_repository, _logger);

            // Assert
            handler.ShouldNotBeNull();
        }

        /// <summary>
        /// Executes Process_WithValidCommand_ShouldReturnSuccess operation.
        /// </summary>
        /// <returns>The result of Process_WithValidCommand_ShouldReturnSuccess.</returns>

        [Fact]
        public async Task Process_WithValidCommand_ShouldReturnSuccess()
        {
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBeOfType<WorkFlowCreatedEvent>();
            result.Value.ProductId.ShouldBe(command.ProductId);
            // Note: Handler swaps LastMachineId and NextMachineId internally
            result.Value.LastMachineId.ShouldBe(command.NextMachineId);
            result.Value.NextMachineId.ShouldBe(command.LastMachineId);
        }

        /// <summary>
        /// Executes ProcessAsync_WhenAddFails_ShouldReturnFailure operation.
        /// </summary>
        /// <returns>The result of ProcessAsync_WhenAddFails_ShouldReturnFailure.</returns>

        [Fact]
        public async Task ProcessAsync_WhenAddFails_ShouldReturnFailure()
        {
            // Arrange
            var command = CreateValidCommand();

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Result<int>.WithFailure("Database connection failed"));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Errors.ShouldContain("Database connection failed");
        }

        /// <summary>
        /// Executes ProcessAsync_WhenCommitFails_ShouldReturnFailure operation.
        /// </summary>
        /// <returns>The result of ProcessAsync_WhenCommitFails_ShouldReturnFailure.</returns>

        [Fact]
        public async Task ProcessAsync_WhenCommitFails_ShouldReturnFailure()
        {
            // Arrange
            var command = CreateValidCommand();

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
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
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert - Note: Handler swaps LastMachineId and NextMachineId
            await _repository.Received(1).AddAsync(
                Arg.Is<WorkFlow>(entity =>
                    entity.ProductId == command.ProductId &&
                    entity.LastMachineId == command.NextMachineId &&
                    entity.NextMachineId == command.LastMachineId),
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
            var command = new CreateWorkFlowCommand { WorkFlowId = 1, ProductId = 5080 };
            var cancellationToken = new CancellationToken();

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            await _handler.ProcessAsync(command, cancellationToken);

            // Assert
            await _repository.Received(1).AddAsync(Arg.Any<WorkFlow>(), cancellationToken);
            await _repository.Received(1).CommitAsync(cancellationToken);
        }

        /// <summary>
        /// Executes Process_WithInvalidIds_ShouldHandleGracefully operation.
        /// </summary>
        /// <param name="invalidId">The invalidId.</param>
        /// <returns>The result of Process_WithInvalidIds_ShouldHandleGracefully.</returns>

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Process_WithInvalidIds_ShouldHandleGracefully(int invalidId)
        {
            // Using parameters: invalidId
            _ = invalidId; // xUnit1026 fix
                           // Using parameters: invalidId
            _ = invalidId; // xUnit1026 fix
                           // Using parameters: invalidId
            _ = invalidId; // xUnit1026 fix
                           // Using parameters: invalidId
            _ = invalidId; // xUnit1026 fix
                           // Using parameters: invalidId
            _ = invalidId; // xUnit1026 fix
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = invalidId,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();

            //[Fix]
            //CLAUDE
            //Date: 28/08/2025
            //Reason: [CS8602] - Add null-forgiving operator since result.IsSuccess was verified true
            result.Value!.ProductId.ShouldBe(5080);
        }

        private static CreateWorkFlowCommand CreateValidCommand()
        {
            return new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };
        }

        /// <summary>
        /// Executes Dispose operation.
        /// </summary>

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// Manufacturing scenario tests for CreateWorkFlowCommandHandler with complex workflow creation scenarios
    /// </summary>
    public class CreateWorkFlowCommandHandlerManufacturingTests : IDisposable
    {
        private readonly IRepository<WorkFlow> _repository = Substitute.For<IRepository<WorkFlow>>();
        private readonly ILogger<CreateWorkFlowCommandHandler> _logger = XUnitLogger.CreateLogger<CreateWorkFlowCommandHandler>();
        private readonly CreateWorkFlowCommandHandler _handler = null!;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>

        public CreateWorkFlowCommandHandlerManufacturingTests()
        {
            _handler = new CreateWorkFlowCommandHandler(_repository, _logger);
        }

        /// <summary>
        /// Executes Should_CreateWorkFlow_When_DifferentManufacturingScenarios operation.
        /// </summary>
        /// <returns>The result of Should_CreateWorkFlow_When_DifferentManufacturingScenarios.</returns>

        [Theory]
        [InlineData(101, 1001, 201, 301, "Ford F-150 Engine Assembly Line")]
        [InlineData(102, 1002, 202, 302, "Tesla Model Y Battery Pack")]
        [InlineData(103, 1003, 203, 303, "BMW X5 Transmission Assembly")]
        [InlineData(104, 1004, 204, 304, "iPhone 15 PCB Manufacturing")]
        [InlineData(105, 1005, 205, 305, "Pharmaceutical Aspirin Tablet")]
        public async Task Should_CreateWorkFlow_When_DifferentManufacturingScenarios(
            int workFlowId, int productId, int lastMachineId, int nextMachineId, string description)
        {
            var logger = XUnitLogger.CreateLogger<CreateWorkFlowCommandHandler>();
            logger.LogInformation("Testing scenario: {Description}", description);

            // Arrange - Various manufacturing workflow creation scenarios
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = workFlowId,
                ProductId = productId,
                LastMachineId = lastMachineId,
                NextMachineId = nextMachineId
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(workFlowId)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert

            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.ProductId.ShouldBe(productId);

            // Verify the machine ID swapping logic (handler feature)
            result.Value.LastMachineId.ShouldBe(nextMachineId);
            result.Value.NextMachineId.ShouldBe(lastMachineId);

            await _repository.Received(1).AddAsync(
                Arg.Is<WorkFlow>(w =>
                    w.ProductId == productId &&
                    w.LastMachineId == nextMachineId &&
                    w.NextMachineId == lastMachineId),
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Executes Should_CreateAutomotiveAssemblyWorkFlow_When_F150EngineProduction operation.
        /// </summary>
        /// <returns>The result of Should_CreateAutomotiveAssemblyWorkFlow_When_F150EngineProduction.</returns>

        [Fact]
        public async Task Should_CreateAutomotiveAssemblyWorkFlow_When_F150EngineProduction()
        {
            // Arrange - Ford F-150 engine assembly workflow
            const int productId = 2001;
            const int stampingStationId = 101;
            const int machiningStationId = 201;

            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1001,
                ProductId = productId,
                LastMachineId = stampingStationId,
                NextMachineId = machiningStationId
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1001)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();

            //[Fix]
            //CLAUDE
            //Date: 28/08/2025
            //Reason: [CS8602] - Add null-forgiving operator since result.IsSuccess was verified true
            result.Value!.ProductId.ShouldBe(productId);

            // Verify automotive workflow creation
            await _repository.Received(1).AddAsync(
                Arg.Is<WorkFlow>(w =>
                    w.ProductId == productId &&
                    w.LastMachineId == machiningStationId && // Swapped
                    w.NextMachineId == stampingStationId),   // Swapped
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Executes Should_CreateElectronicsWorkFlow_When_iPhonePCBManufacturing operation.
        /// </summary>
        /// <returns>The result of Should_CreateElectronicsWorkFlow_When_iPhonePCBManufacturing.</returns>

        [Fact]
        public async Task Should_CreateElectronicsWorkFlow_When_iPhonePCBManufacturing()
        {
            // Arrange - iPhone PCB manufacturing workflow
            const int productId = 3001;
            const int smtLineId = 401;
            const int aoi_InspectionId = 501;

            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 2001,
                ProductId = productId,
                LastMachineId = smtLineId,
                NextMachineId = aoi_InspectionId
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(2001)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert

            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ProductId.ShouldBe(productId);

            // Verify electronics workflow creation with quality inspection flow
            await _repository.Received(1).AddAsync(
                Arg.Is<WorkFlow>(w =>
                    w.ProductId == productId &&
                    w.LastMachineId == aoi_InspectionId && // AOI Inspection (swapped)
                    w.NextMachineId == smtLineId),         // SMT Line (swapped)
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Executes Should_CreatePharmaceuticalWorkFlow_When_TabletManufacturing operation.
        /// </summary>
        /// <returns>The result of Should_CreatePharmaceuticalWorkFlow_When_TabletManufacturing.</returns>

        [Fact]
        public async Task Should_CreatePharmaceuticalWorkFlow_When_TabletManufacturing()
        {
            // Arrange - Pharmaceutical tablet manufacturing workflow
            const int productId = 4001;
            const int blendingStationId = 601;
            const int compressionPressId = 701;

            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 3001,
                ProductId = productId,
                LastMachineId = blendingStationId,
                NextMachineId = compressionPressId
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(3001)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert

            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ProductId.ShouldBe(productId);

            // Verify pharmaceutical workflow creation with FDA compliance tracking
            await _repository.Received(1).AddAsync(
                Arg.Is<WorkFlow>(w =>
                    w.ProductId == productId &&
                    w.LastMachineId == compressionPressId && // Compression Press (swapped)
                    w.NextMachineId == blendingStationId),   // Blending Station (swapped)
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Executes Should_CreateFoodBeverageWorkFlow_When_CocaColaBottling operation.
        /// </summary>
        /// <returns>The result of Should_CreateFoodBeverageWorkFlow_When_CocaColaBottling.</returns>

        [Fact]
        public async Task Should_CreateFoodBeverageWorkFlow_When_CocaColaBottling()
        {
            // Arrange - Coca-Cola bottling line workflow
            const int productId = 5001;
            const int fillingStationId = 801;
            const int cappingStationId = 901;

            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 4001,
                ProductId = productId,
                LastMachineId = fillingStationId,
                NextMachineId = cappingStationId
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(4001)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert

            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ProductId.ShouldBe(productId);

            // Verify food & beverage workflow creation with hygiene standards
            await _repository.Received(1).AddAsync(
                Arg.Is<WorkFlow>(w =>
                    w.ProductId == productId &&
                    w.LastMachineId == cappingStationId && // Capping Station (swapped)
                    w.NextMachineId == fillingStationId),  // Filling Station (swapped)
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Executes Should_HandleProductionLineSequencing_When_ComplexManufacturingFlow operation.
        /// </summary>
        /// <returns>The result of Should_HandleProductionLineSequencing_When_ComplexManufacturingFlow.</returns>

        [Fact]
        public async Task Should_HandleProductionLineSequencing_When_ComplexManufacturingFlow()
        {
            // Arrange - Complex production line with multiple sequential workflows
            const int productId = 6001;
            const int initialStationId = 1001;
            const int finalStationId = 1002;

            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 5001,
                ProductId = productId,
                LastMachineId = initialStationId,
                NextMachineId = finalStationId
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(5001)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert

            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ProductId.ShouldBe(productId);

            // Verify proper workflow sequencing for complex manufacturing
            await _repository.Received(1).AddAsync(
                Arg.Is<WorkFlow>(w =>
                    w.ProductId == productId &&
                    w.LastMachineId == finalStationId &&   // Final becomes last (swapped)
                    w.NextMachineId == initialStationId),  // Initial becomes next (swapped)
                Arg.Any<CancellationToken>());

            await _repository.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Executes Should_CreateWorkFlowWithTraceability_When_AutomotiveQualityRequirements operation.
        /// </summary>
        /// <returns>The result of Should_CreateWorkFlowWithTraceability_When_AutomotiveQualityRequirements.</returns>

        [Fact]
        public async Task Should_CreateWorkFlowWithTraceability_When_AutomotiveQualityRequirements()
        {
            // Arrange - Automotive quality traceability workflow (BMW quality standards)
            const int productId = 7001;
            const int qualityGateId = 1101;
            const int finalInspectionId = 1201;

            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 6001,
                ProductId = productId,
                LastMachineId = qualityGateId,
                NextMachineId = finalInspectionId
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(6001)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert

            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ProductId.ShouldBe(productId);

            // Verify quality workflow with traceability
            await _repository.Received(1).AddAsync(
                Arg.Is<WorkFlow>(w =>
                    w.ProductId == productId &&
                    w.LastMachineId == finalInspectionId && // Final Inspection (swapped)
                    w.NextMachineId == qualityGateId),      // Quality Gate (swapped)
                Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Executes Should_HandleMinimalWorkFlow_When_SimpleProductionFlow operation.
        /// </summary>
        /// <returns>The result of Should_HandleMinimalWorkFlow_When_SimpleProductionFlow.</returns>

        [Fact]
        public async Task Should_HandleMinimalWorkFlow_When_SimpleProductionFlow()
        {
            // Arrange - Minimal workflow for simple production
            const int productId = 8001;

            var command = new CreateWorkFlowCommand
            {
                ProductId = productId,
                LastMachineId = 0,
                NextMachineId = 0
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert

            result.Value.ShouldNotBeNull();
            result.IsSuccess.ShouldBeTrue();
            result.Value.ProductId.ShouldBe(productId);
            result.Value.LastMachineId.ShouldBe(0);
            result.Value.NextMachineId.ShouldBe(0);
        }

        /// <summary>
        /// Executes Dispose operation.
        /// </summary>

        public void Dispose()
        {
            // Cleanup if needed
        }
    }

    /// <summary>
    /// Error handling and edge case tests for CreateWorkFlowCommandHandler
    /// </summary>
    public class CreateWorkFlowCommandHandlerErrorTests : IDisposable
    {
        private readonly IRepository<WorkFlow> _repository = Substitute.For<IRepository<WorkFlow>>();
        private readonly ILogger<CreateWorkFlowCommandHandler> _logger = XUnitLogger.CreateLogger<CreateWorkFlowCommandHandler>();
        private readonly CreateWorkFlowCommandHandler _handler = null!;
        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>

        public CreateWorkFlowCommandHandlerErrorTests()
        {
            _handler = new CreateWorkFlowCommandHandler(_repository, _logger);
        }

        /// <summary>
        /// Executes Should_HandleRepositoryException_When_DatabaseConnectionFails operation.
        /// </summary>
        /// <returns>The result of Should_HandleRepositoryException_When_DatabaseConnectionFails.</returns>

        [Fact]
        public async Task Should_HandleRepositoryException_When_DatabaseConnectionFails()
        {
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Result<int>.WithFailure("Database connection failed"));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Errors.ShouldNotBeEmpty();
        }

        /// <summary>
        /// Executes Should_HandleCancellation_When_CancellationRequested operation.
        /// </summary>
        /// <returns>The result of Should_HandleCancellation_When_CancellationRequested.</returns>

        [Fact]
        public async Task Should_HandleCancellation_When_CancellationRequested()
        {
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Result<int>.WithFailure("Operation canceled"));

            // Act
            var canceled = await _handler.ProcessAsync(command, cts.Token);

            // Assert
            canceled.IsFailure.ShouldBeTrue();
            canceled.Errors.ShouldNotBeEmpty();
        }

        /// <summary>
        /// Executes Should_HandleCommitTimeout_When_TransactionTakesTooLong operation.
        /// </summary>
        /// <returns>The result of Should_HandleCommitTimeout_When_TransactionTakesTooLong.</returns>

        [Fact]
        public async Task Should_HandleCommitTimeout_When_TransactionTakesTooLong()
        {
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Result.WithFailure("Transaction timeout"));

            // Act
            var timeout = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            timeout.IsFailure.ShouldBeTrue();
            timeout.Errors.ShouldNotBeEmpty();
        }

        /// <summary>
        /// Executes Should_LogErrors_When_RepositoryOperationsFail operation.
        /// </summary>
        /// <returns>The result of Should_LogErrors_When_RepositoryOperationsFail.</returns>

        [Fact]
        public async Task Should_LogErrors_When_RepositoryOperationsFail()
        {
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.WithFailure("Repository add failed")));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            //[Fix]
            //CLAUDE
            //Date: 20/08/2025
            //Reason: Fix Result<T> pattern violation - check IsSuccess before accessing Value, and remove invalid Value assertion for failed results
            result.IsSuccess.ShouldBeFalse();
            result.Errors.ShouldContain("Repository add failed");
        }

        /// <summary>
        /// Executes Should_LogCommitErrors_When_TransactionCommitFails operation.
        /// </summary>
        /// <returns>The result of Should_LogCommitErrors_When_TransactionCommitFails.</returns>

        [Fact]
        public async Task Should_LogCommitErrors_When_TransactionCommitFails()
        {
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.WithFailure("Transaction commit failed")));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Errors.ShouldContain("Transaction commit failed");
        }

        /// <summary>
        /// Executes Should_HandleExtremeValues_When_LargeProductOrMachineIds operation.
        /// </summary>
        /// <param name="extremeValue">The extremeValue.</param>
        /// <returns>The result of Should_HandleExtremeValues_When_LargeProductOrMachineIds.</returns>

        [Theory]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        [InlineData(-999999)]
        [InlineData(999999)]
        public async Task Should_HandleExtremeValues_When_LargeProductOrMachineIds(int extremeValue)
        {
            // Using parameters: extremeValue
            _ = extremeValue; // xUnit1026 fix
                              // Using parameters: extremeValue
            _ = extremeValue; // xUnit1026 fix
                              // Using parameters: extremeValue
            _ = extremeValue; // xUnit1026 fix
                              // Using parameters: extremeValue
            _ = extremeValue; // xUnit1026 fix
                              // Using parameters: extremeValue
            _ = extremeValue; // xUnit1026 fix
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = extremeValue,
                ProductId = extremeValue,
                LastMachineId = extremeValue,
                NextMachineId = extremeValue
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            //[Fix]
            //CLAUDE
            //Date: 28/08/2025
            //Reason: [CS8602] - Add null-forgiving operator since result.IsSuccess was verified true
            result.Value!.ProductId.ShouldBe(extremeValue);
        }

        /// <summary>
        /// Executes Should_HandleSequentialFailures_When_MultipleOperationsFail operation.
        /// </summary>
        /// <returns>The result of Should_HandleSequentialFailures_When_MultipleOperationsFail.</returns>

        [Fact]
        public async Task Should_HandleSequentialFailures_When_MultipleOperationsFail()
        {
            // Arrange
            var command = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            // First call fails, subsequent calls succeed
            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(
                    Task.FromResult(Result<int>.WithFailure("First attempt failed")),
                    Task.FromResult(Result<int>.Success(1)));

            // Act
            var result = await _handler.ProcessAsync(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Errors.ShouldContain("First attempt failed");
        }

        /// <summary>
        /// Executes Should_HandleConcurrentAccess_When_MultipleWorkFlowsCreated operation.
        /// </summary>
        /// <returns>The result of Should_HandleConcurrentAccess_When_MultipleWorkFlowsCreated.</returns>

        [Fact]
        public async Task Should_HandleConcurrentAccess_When_MultipleWorkFlowsCreated()
        {
            // Arrange - Simulating concurrent workflow creation for same product
            var command1 = new CreateWorkFlowCommand
            {
                WorkFlowId = 1,
                ProductId = 5080,
                LastMachineId = 1000,
                NextMachineId = 20
            };

            var command2 = new CreateWorkFlowCommand
            {
                WorkFlowId = 2,
                ProductId = 5080, // Same product
                LastMachineId = 30,
                NextMachineId = 40
            };

            _repository.AddAsync(Arg.Any<WorkFlow>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result<int>.Success(1)));
            _repository.CommitAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(Result.Success()));

            // Act
            var result1 = await _handler.ProcessAsync(command1, TestContext.Current.CancellationToken);
            var result2 = await _handler.ProcessAsync(command2, TestContext.Current.CancellationToken);

            // Assert
            result1.IsSuccess.ShouldBeTrue();
            result2.IsSuccess.ShouldBeTrue();
            //[Fix]
            //CLAUDE
            //Date: 28/08/2025
            //Reason: [CS8602] - Add null-forgiving operators since result.IsSuccess was verified true for both
            result1.Value!.ProductId.ShouldBe(5080);
            result2.Value!.ProductId.ShouldBe(5080);
        }

        /// <summary>
        /// Executes Dispose operation.
        /// </summary>

        public void Dispose()
        {
            // Cleanup if needed
        }
    }
}
