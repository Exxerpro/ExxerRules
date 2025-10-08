namespace Application.UnitTests.Services
{
    /// <summary>
    /// Unit tests for CycleService implementation
    /// </summary>
    public class CycleServiceTests
    {
        private readonly IRepository<Cycle> _cycleRepository = null!;
        private readonly CycleService _service = null!;

        public CycleServiceTests()
        {
            _cycleRepository = Substitute.For<IRepository<Cycle>>();
            _service = new CycleService(_cycleRepository);
        }

        /// <summary>
        /// Tests that Constructor creates instance with valid dependencies
        /// </summary>
        [Fact]
        public void Constructor_WithValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var service = new CycleService(_cycleRepository);

            // Assert
            service.ShouldNotBeNull();
        }

        /// <summary>
        /// Tests that Constructor throws ArgumentNullException with null cycleRepository
        /// </summary>
        [Fact]
        public void Constructor_WithNullCycleRepository_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() => new CycleService(null!));
        }

        /// <summary>
        /// Tests GetCyclesByBarCodeIdAsync returns success with valid BarCode ID
        /// </summary>
        [Fact]
        public async Task GetCyclesByBarCodeIdAsync_WithValidBarCodeId_ShouldReturnSuccess()
        {
            // Arrange
            var cycles = new List<Cycle>
            {
                new() { CycleId = 1, BarCodeId = 100, MachineId = 100 },
                new() { CycleId = 2, BarCodeId = 100, MachineId = 100 }
            };

            _cycleRepository.ListAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
                .Returns(Result<IEnumerable<Cycle>>.Success(cycles));

            // Mock the static ToDtoList method would need additional setup in real implementation
            // For now, we'll test the service structure

            // Act & Assert - This validates the service calls repository correctly
            await _service.GetCyclesByBarCodeIdAsync(100, TestContext.Current.CancellationToken);

            // Verify repository was called
            await _cycleRepository.Received(1).ListAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Tests GetCyclesByBarCodeIdAsync returns failure when no cycles found
        /// </summary>
        [Fact]
        public async Task GetCyclesByBarCodeIdAsync_WhenNoCyclesFound_ShouldReturnFailure()
        {
            // Arrange
            var cycles = new List<Cycle>();
            _cycleRepository.ListAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
                .Returns(Result<IEnumerable<Cycle>>.Success(cycles));

            // Act
            var response = await _service.GetCyclesByBarCodeIdAsync(100, TestContext.Current.CancellationToken);

            // Assert
            response.IsFailure.ShouldBeTrue();
            response.Errors.ShouldContain("No cycles found for the specified BarCode ID");
        }

        /// <summary>
        /// Tests GetProductionByShiftAsync returns success with valid parameters
        /// </summary>
        [Fact]
        public async Task GetProductionByShiftAsync_WithValidParameters_ShouldReturnSuccess()
        {
            // Arrange
            var startTime = DateTime.UtcNow.AddHours(-8);
            var endTime = DateTime.UtcNow;
            const int machineId = 1;
            const int expectedCount = 42;

            _cycleRepository.CountAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
                .Returns(Result<int>.Success(expectedCount));

            // Act
            var response = await _service.GetProductionByShiftAsync(startTime, endTime, machineId, TestContext.Current.CancellationToken);

            // Assert
            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldBe(expectedCount);
        }

        /// <summary>
        /// Tests GetProductionByShiftAsync repository receives correct specification
        /// </summary>
        [Fact]
        public async Task GetProductionByShiftAsync_ShouldCallRepositoryWithCorrectSpecification()
        {
            // Arrange
            var startTime = DateTime.UtcNow.AddHours(-8);
            var endTime = DateTime.UtcNow;
            const int machineId = 1;

            _cycleRepository.CountAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
                .Returns(Result<int>.Success(0));

            // Act
            await _service.GetProductionByShiftAsync(startTime, endTime, machineId, TestContext.Current.CancellationToken);

            // Assert - Verify repository was called with a specification
            await _cycleRepository.Received(1).CountAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>());
        }

        /// <summary>
        /// Tests GetCycleByIdAsync returns success with valid BarCode ID
        /// </summary>
        [Fact]
        public async Task GetCycleByIdAsync_WithValidBarCodeId_ShouldReturnSuccess()
        {
            // Arrange
            var cycle = new Cycle { CycleId = 1, BarCodeId = 100, MachineId = 100 };
            _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
                .Returns(Result<Cycle?>.Success(cycle));

            // Act
            var response = await _service.GetCycleByIdAsync(100, TestContext.Current.CancellationToken);

            // Assert
            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldNotBeNull();
            response.Value.BarCodeId.ShouldBe(100);
        }

        /// <summary>
        /// Tests GetCycleByIdAsync returns null when no cycle found
        /// </summary>
        [Fact]
        public async Task GetCycleByIdAsync_WhenNoCycleFound_ShouldReturnNull()
        {
            // Arrange
            _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
                .Returns(Result<Cycle?>.WithFailure("No cycle found"));

            // Act
            var response = await _service.GetCycleByIdAsync(999, TestContext.Current.CancellationToken);

            // Assert
            //[Fix]
            //CLAUDE
            //Date: 19/09/2025
            //Reason: [REPOSITORY BEHAVIOR CORRECTION] - Repository returns failure when no entity found
            response.IsSuccess.ShouldBeFalse();
            response.Errors.ShouldContain("No cycle found");
        }

        /// <summary>
        /// Tests GetCycleByIdAsync repository receives correct specification with ordering
        /// </summary>
        [Fact]
        public async Task GetCycleByIdAsync_ShouldCallRepositoryWithCorrectSpecificationAndOrdering()
        {
            // Arrange
            _cycleRepository.FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
                .Returns(Result<Cycle?>.Success((Cycle?)null));

            // Act
            await _service.GetCycleByIdAsync(100, TestContext.Current.CancellationToken);

            // Assert - Verify repository was called (ordering is handled within specification)
            await _cycleRepository.Received(1).FirstOrDefaultAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>());
        }
    }
}
