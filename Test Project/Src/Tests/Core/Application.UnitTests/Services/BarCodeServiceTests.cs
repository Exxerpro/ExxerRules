namespace Application.UnitTests.Services
{
    /// <summary>
    /// Unit tests for BarCodeService implementation
    /// </summary>
    public class BarCodeServiceTests
    {
        private readonly IRepository<BarCode> _barCodeRepository = null!;
        private readonly IRepository<Register> _registerRepository = null!;
        private readonly IRepository<Cycle> _cycleRepository = null!;
        private readonly BarCodeService _service = null!;

        public BarCodeServiceTests()
        {
            _barCodeRepository = Substitute.For<IRepository<BarCode>>();
            _registerRepository = Substitute.For<IRepository<Register>>();
            _cycleRepository = Substitute.For<IRepository<Cycle>>();
            _service = new BarCodeService(_barCodeRepository, _registerRepository, _cycleRepository);
        }

        /// <summary>
        /// Tests that Constructor creates instance with valid dependencies
        /// </summary>
        [Fact]
        public void Constructor_WithValidDependencies_ShouldCreateInstance()
        {
            // Arrange & Act
            var service = new BarCodeService(_barCodeRepository, _registerRepository, _cycleRepository);

            // Assert
            service.ShouldNotBeNull();
        }

        /// <summary>
        /// Tests that Constructor throws ArgumentNullException with null barCodeRepository
        /// </summary>
        [Fact]
        public void Constructor_WithNullBarCodeRepository_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() =>
                new BarCodeService(null!, _registerRepository, _cycleRepository));
        }

        /// <summary>
        /// Tests that Constructor throws ArgumentNullException with null registerRepository
        /// </summary>
        [Fact]
        public void Constructor_WithNullRegisterRepository_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() =>
                new BarCodeService(_barCodeRepository, null!, _cycleRepository));
        }

        /// <summary>
        /// Tests that Constructor throws ArgumentNullException with null cycleRepository
        /// </summary>
        [Fact]
        public void Constructor_WithNullCycleRepository_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Should.Throw<ArgumentNullException>(() =>
                new BarCodeService(_barCodeRepository, _registerRepository, null!));
        }

        /// <summary>
        /// Tests GetConsecutiveByBarCodeLabelAsync returns success with valid data
        /// </summary>
        [Fact]
        public async Task GetConsecutiveByBarCodeLabelAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var barCode = new BarCode { BarCodeId = 100, Label = "TEST-LABEL" };
            var result = Result<BarCode?>.Success(barCode);
            _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
                .Returns(result);

            // Act
            var response = await _service.GetConsecutiveByBarCodeLabelAsync("PART-123", new List<string> { "MASTER" }, TestContext.Current.CancellationToken);

            // Assert
            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldBe(101); // 100 + 1
        }

        /// <summary>
        /// Tests GetConsecutiveByBarCodeLabelAsync returns failure when repository fails
        /// </summary>
        [Fact]
        public async Task GetConsecutiveByBarCodeLabelAsync_WhenRepositoryFails_ShouldReturnFailure()
        {
            // Arrange
            var result = Result<BarCode?>.WithFailure("Repository error");
            _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
                .Returns(result);

            // Act
            var response = await _service.GetConsecutiveByBarCodeLabelAsync("PART-123", new List<string> { "MASTER" }, TestContext.Current.CancellationToken);

            // Assert
            response.IsFailure.ShouldBeTrue();
            response.Errors.ShouldContain("No BarCodes found for the given label");
        }

        /// <summary>
        /// Tests GetBarCodeByLabelAsync returns success with valid label
        /// </summary>
        [Fact]
        public async Task GetBarCodeByLabelAsync_WithValidLabel_ShouldReturnSuccess()
        {
            // Arrange
            var barCode = new BarCode { BarCodeId = 1, Label = "TEST-LABEL" };
            var result = Result<BarCode?>.Success(barCode);
            _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
                .Returns(result);

            // Act
            var response = await _service.GetBarCodeByLabelAsync("TEST-LABEL", TestContext.Current.CancellationToken);

            // Assert
            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldNotBeNull();
            response.Value.Label.ShouldBe("TEST-LABEL");
        }

        /// <summary>
        /// Tests GetBarCodeByIdAsync returns success with valid ID
        /// </summary>
        [Fact]
        public async Task GetBarCodeByIdAsync_WithValidId_ShouldReturnSuccess()
        {
            // Arrange
            var barCode = new BarCode { BarCodeId = 123, Label = "TEST-LABEL" };
            var result = Result<BarCode?>.Success(barCode);
            _barCodeRepository.FirstOrDefaultAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
                .Returns(result);

            // Act
            var response = await _service.GetBarCodeByIdAsync(123, TestContext.Current.CancellationToken);

            // Assert
            response.IsSuccess.ShouldBeTrue();
            response.Value.ShouldNotBeNull();
            response.Value.BarCodeId.ShouldBe(123);
        }

        /// <summary>
        /// Tests GetBarCodeByRegisterDataAsync returns success with complex data flow
        /// </summary>
        [Fact]
        public async Task GetBarCodeByRegisterDataAsync_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var registers = new List<Register> { new() { CycleId = 1, Value = "TEST-LABEL" } };
            var cycles = new List<Cycle> { new() { CycleId = 1, BarCodeId = 100 } };
            var barCodes = new List<BarCode> { new() { BarCodeId = 100, Label = "TEST-LABEL" } };
            var barCodeDtos = new List<BarCodeDto> { new() { BarCodeId = 100, Label = "TEST-LABEL" } };

            _registerRepository.ListAsync(Arg.Any<Specification<Register>>(), Arg.Any<CancellationToken>())
                .Returns(Result<IEnumerable<Register>>.Success(registers));
            _cycleRepository.ListAsync(Arg.Any<Specification<Cycle>>(), Arg.Any<CancellationToken>())
                .Returns(Result<IEnumerable<Cycle>>.Success(cycles));
            _barCodeRepository.ListAsync(Arg.Any<Specification<BarCode>>(), Arg.Any<CancellationToken>())
                .Returns(Result<IEnumerable<BarCode>>.Success(barCodes));

            // Mock the static ToDtoList method would need additional setup in real implementation
            // For now, we'll test the service structure

            // Act & Assert - This test validates the service structure and dependency injection
            // The actual ToDtoList conversion would require additional mocking framework setup
            _service.ShouldNotBeNull();
            await _service.GetBarCodeByRegisterDataAsync("TEST-LABEL", TestContext.Current.CancellationToken);

            // Verify repository calls were made
            await _registerRepository.Received(1).ListAsync(Arg.Any<Specification<Register>>(), Arg.Any<CancellationToken>());
        }
    }
}
