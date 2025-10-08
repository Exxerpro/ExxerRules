using IndTrace.Application.Products.Services.Interfaces;
using IndTrace.Domain.Services.Products;
using Meziantou.Extensions.Logging.Xunit;

namespace IndTrace.Application.UnitTests.Products.Services;

/// <summary>
/// Unit tests for ProductPersistenceOrchestrator - Critical intelligent ID assignment logic.
/// Tests the sophisticated persistence strategy with 4-parameter vs 2-parameter AddAsync patterns.
/// </summary>
public class ProductPersistenceOrchestratorTests
{
    private readonly IRepository<Product> _mockProductRepository;
    private readonly IProductFactory _mockProductFactory;
    private readonly IProductUniquenessValidator _mockUniquenessValidator;
    private readonly ILogger<ProductPersistenceOrchestrator> _mockLogger;
    private readonly ProductPersistenceOrchestrator _orchestrator;

    public ProductPersistenceOrchestratorTests(ITestOutputHelper output)
    {
        _mockProductRepository = Substitute.For<IRepository<Product>>();
        _mockProductFactory = Substitute.For<IProductFactory>();
        _mockUniquenessValidator = Substitute.For<IProductUniquenessValidator>();
        _mockLogger = XUnitLogger.CreateLogger<ProductPersistenceOrchestrator>(output);

        _orchestrator = new ProductPersistenceOrchestrator(
            _mockProductRepository,
            _mockProductFactory,
            _mockUniquenessValidator,
            _mockLogger);
    }

    #region Constructor Tests

    [Fact]
    public void Constructor_NullRepository_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new ProductPersistenceOrchestrator(null!, _mockProductFactory, _mockUniquenessValidator, _mockLogger));
    }

    [Fact]
    public void Constructor_NullProductFactory_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new ProductPersistenceOrchestrator(_mockProductRepository, null!, _mockUniquenessValidator, _mockLogger));
    }

    [Fact]
    public void Constructor_NullUniquenessValidator_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new ProductPersistenceOrchestrator(_mockProductRepository, _mockProductFactory, null!, _mockLogger));
    }

    [Fact]
    public void Constructor_NullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() =>
            new ProductPersistenceOrchestrator(_mockProductRepository, _mockProductFactory, _mockUniquenessValidator, null!));
    }

    #endregion Constructor Tests

    #region PersistProductWithIntelligentIdAsync - Intelligent Assignment Strategy

    [Fact]
    public async Task PersistProductWithIntelligentIdAsync_SuccessfulParsing_AvailableId_ShouldUse4ParameterAddAsync()
    {
        // Arrange
        var product = CreateTestProduct("FORD-F150-001");
        var productInput = CreateTestProductInput("FORD-F150-001");

        // Setup intelligent ID parsing
        _mockProductFactory
            .TryParseLastInteger("FORD-F150-001")
            .Returns((true, 1)); // Parsed number: 1

        _mockProductFactory
            .GetDynamicOffset(1)
            .Returns(10); // Offset for single digit

        // ID 11 (1 + 10) is available
        _mockUniquenessValidator
            .IsProductIdAvailableAsync(11, Arg.Any<CancellationToken>())
            .Returns(true);

        // Repository accepts 4-parameter AddAsync
        _mockProductRepository
            .AddAsync(product, 11, "Products", Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _orchestrator.PersistProductWithIntelligentIdAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.ProductId.ShouldBe(11); // Should be set to calculated ID

        // Verify 4-parameter AddAsync was called (intelligent assignment)
        await _mockProductRepository
            .Received(1)
            .AddAsync(product, 11, "Products", Arg.Any<CancellationToken>());

        // Verify 2-parameter AddAsync was NOT called
        await _mockProductRepository
            .DidNotReceive()
            .AddAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PersistProductWithIntelligentIdAsync_SuccessfulParsing_IdNotAvailable_ShouldUse2ParameterAddAsync()
    {
        // Arrange
        var product = CreateTestProduct("FORD-F150-001");
        var productInput = CreateTestProductInput("FORD-F150-001");

        // Setup intelligent ID parsing
        _mockProductFactory
            .TryParseLastInteger("FORD-F150-001")
            .Returns((true, 1)); // Parsed number: 1

        _mockProductFactory
            .GetDynamicOffset(1)
            .Returns(10); // Offset for single digit

        // ID 11 (1 + 10) is NOT available
        _mockUniquenessValidator
            .IsProductIdAvailableAsync(11, Arg.Any<CancellationToken>())
            .Returns(false);

        // Repository accepts 2-parameter AddAsync for auto-assignment
        _mockProductRepository
            .AddAsync(product, Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _orchestrator.PersistProductWithIntelligentIdAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.ProductId.ShouldBe(0); // Should be reset to 0 for auto-assignment

        // Verify 2-parameter AddAsync was called (auto assignment)
        await _mockProductRepository
            .Received(1)
            .AddAsync(product, Arg.Any<CancellationToken>());

        // Verify 4-parameter AddAsync was NOT called
        await _mockProductRepository
            .DidNotReceive()
            .AddAsync(product, Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PersistProductWithIntelligentIdAsync_ParsingFails_ShouldUse2ParameterAddAsync()
    {
        // Arrange
        var product = CreateTestProduct("FORD-F150-ABC"); // Non-numeric suffix
        var productInput = CreateTestProductInput("FORD-F150-ABC");

        // Setup failed parsing
        _mockProductFactory
            .TryParseLastInteger("FORD-F150-ABC")
            .Returns((false, 0)); // Parsing failed

        // Repository accepts 2-parameter AddAsync for auto-assignment
        _mockProductRepository
            .AddAsync(product, Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _orchestrator.PersistProductWithIntelligentIdAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.ProductId.ShouldBe(0); // Should be 0 for auto-assignment

        // Verify 2-parameter AddAsync was called (auto assignment)
        await _mockProductRepository
            .Received(1)
            .AddAsync(product, Arg.Any<CancellationToken>());

        // Verify uniqueness validator was NOT called (no parsing)
        await _mockUniquenessValidator
            .DidNotReceive()
            .IsProductIdAvailableAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("TESLA-Y-23", 23, 100, 123)] // Double digit: 23 + 100 = 123
    [InlineData("BMW-X5-999", 999, 1000, 1999)] // Triple digit: 999 + 1000 = 1999
    [InlineData("MERCEDES-C180-1234", 1234, 10000, 11234)] // Four digit: 1234 + 10000 = 11234
    public async Task PersistProductWithIntelligentIdAsync_VariousNumberWidths_ShouldCalculateCorrectOffsets(
        string partNumber, int parsedNumber, int expectedOffset, int expectedFinalId)
    {
        // Arrange
        var product = CreateTestProduct(partNumber);
        var productInput = CreateTestProductInput(partNumber);

        _mockProductFactory
            .TryParseLastInteger(partNumber)
            .Returns((true, parsedNumber));

        _mockProductFactory
            .GetDynamicOffset(parsedNumber)
            .Returns(expectedOffset);

        _mockUniquenessValidator
            .IsProductIdAvailableAsync(expectedFinalId, Arg.Any<CancellationToken>())
            .Returns(true);

        _mockProductRepository
            .AddAsync(product, expectedFinalId, "Products", Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _orchestrator.PersistProductWithIntelligentIdAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        product.ProductId.ShouldBe(expectedFinalId);

        // Verify correct ID was used
        await _mockProductRepository
            .Received(1)
            .AddAsync(product, expectedFinalId, "Products", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PersistProductWithIntelligentIdAsync_RepositoryFailure_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateTestProduct("FORD-F150-001");
        var productInput = CreateTestProductInput("FORD-F150-001");

        _mockProductFactory
            .TryParseLastInteger("FORD-F150-001")
            .Returns((true, 1));

        _mockProductFactory
            .GetDynamicOffset(1)
            .Returns(10);

        _mockUniquenessValidator
            .IsProductIdAvailableAsync(11, Arg.Any<CancellationToken>())
            .Returns(true);

        // Repository failure
        _mockProductRepository
            .AddAsync(product, 11, "Products", Arg.Any<CancellationToken>())
            .Returns(Result<int>.WithFailure(["Database error"]));

        // Act
        var result = await _orchestrator.PersistProductWithIntelligentIdAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        // [Fix] Update error message expectation to match actual implementation
        result.Errors.ShouldContain("Database error");
    }

    [Fact]
    public async Task PersistProductWithIntelligentIdAsync_CancellationRequested_ShouldReturnCancellationError()
    {
        // Arrange
        var product = CreateTestProduct("FORD-F150-001");
        var productInput = CreateTestProductInput("FORD-F150-001");
        var cancellationToken = new CancellationToken(true);

        // Act
        var result = await _orchestrator.PersistProductWithIntelligentIdAsync(product, productInput, cancellationToken);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Operation was canceled.");
    }

    [Fact]
    public async Task PersistProductWithIntelligentIdAsync_Exception_ShouldReturnExceptionError()
    {
        // Arrange
        var product = CreateTestProduct("FORD-F150-001");
        var productInput = CreateTestProductInput("FORD-F150-001");

        _mockProductFactory
            .TryParseLastInteger("FORD-F150-001")
            .Returns(x => throw new InvalidOperationException("Factory error"));

        // Act
        var result = await _orchestrator.PersistProductWithIntelligentIdAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        // [Fix] Update error message expectation to match actual implementation
        result.Errors.ShouldContain("Exception during ID assignment: Factory error");
    }

    #endregion PersistProductWithIntelligentIdAsync - Intelligent Assignment Strategy

    #region ValidateProductForPersistence Tests

    [Fact]
    public void ValidateProductForPersistence_ValidProduct_ShouldReturnSuccess()
    {
        // Arrange
        var product = CreateValidProductForPersistence();

        // Act
        var result = _orchestrator.ValidateProductForPersistence(product);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void ValidateProductForPersistence_NullProduct_ShouldReturnFailure()
    {
        // Act
        var result = _orchestrator.ValidateProductForPersistence(null!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product cannot be null for persistence validation.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateProductForPersistence_InvalidPartNumber_ShouldReturnFailure(string partNumber)
    {
        // Arrange
        var product = CreateValidProductForPersistence();
        product.PartNumber = partNumber;

        // Act
        var result = _orchestrator.ValidateProductForPersistence(product);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product PartNumber is required for persistence.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateProductForPersistence_InvalidProductName_ShouldReturnFailure(string productName)
    {
        // Arrange
        var product = CreateValidProductForPersistence();
        product.ProductName = productName;

        // Act
        var result = _orchestrator.ValidateProductForPersistence(product);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product ProductName is required for persistence.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void ValidateProductForPersistence_InvalidCustomerId_ShouldReturnFailure(int customerId)
    {
        // Arrange
        var product = CreateValidProductForPersistence();
        product.CustomerId = customerId;

        // Act
        var result = _orchestrator.ValidateProductForPersistence(product);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product must have a valid CustomerId for persistence.");
    }

    [Fact]
    public void ValidateProductForPersistence_NullCustomer_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProductForPersistence();

#pragma warning disable cs8604 //testing null response
        product.Customer = null!;
#pragma warning restore cs8625 //testing null response
        // Act
        var result = _orchestrator.ValidateProductForPersistence(product);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product must have a Customer entity for persistence.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void ValidateProductForPersistence_InvalidLineId_ShouldReturnFailure(int lineId)
    {
        // Arrange
        var product = CreateValidProductForPersistence();
        product.LineId = lineId;

        // Act
        var result = _orchestrator.ValidateProductForPersistence(product);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product must have a valid LineId for persistence.");
    }

    [Fact]
    public void ValidateProductForPersistence_NullLine_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProductForPersistence();
#pragma warning disable cs8625 //testing null response
        product.Line = null!;
#pragma warning restore cs8625 //testing null response
        // Act
        var result = _orchestrator.ValidateProductForPersistence(product);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product must have a Line entity for persistence.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ValidateProductForPersistence_InvalidCreatedBy_ShouldReturnFailure(string createdBy)
    {
        // Arrange
        var product = CreateValidProductForPersistence();
        product.CreatedBy = createdBy;

        // Act
        var result = _orchestrator.ValidateProductForPersistence(product);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product CreatedBy is required for persistence.");
    }

    #endregion ValidateProductForPersistence Tests

    #region UpdateProductWithIntelligentStrategyAsync Tests

    [Fact]
    public async Task UpdateProductWithIntelligentStrategyAsync_ValidProduct_ShouldUpdateSuccessfully()
    {
        // Arrange
        var product = CreateValidProductForPersistence();
        var productInput = CreateTestProductInput("FORD-F150-001");

        _mockProductRepository
            .UpdateAsync(product, Arg.Any<CancellationToken>())
            .Returns(Result<int>.Success(1));

        // Act
        var result = await _orchestrator.UpdateProductWithIntelligentStrategyAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(product);

        // Verify audit fields were updated
        product.ModifiedBy.ShouldBe("TEST_USER");
        product.ModifiedOn.ShouldNotBeNull();
        (DateTime.Now - product.ModifiedOn.Value).TotalSeconds.ShouldBeLessThan(5);

        await _mockProductRepository
            .Received(1)
            .UpdateAsync(product, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task UpdateProductWithIntelligentStrategyAsync_NullProduct_ShouldReturnFailure()
    {
        // Arrange
        var productInput = CreateTestProductInput("FORD-F150-001");

        // Act
        var result = await _orchestrator.UpdateProductWithIntelligentStrategyAsync(null!, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Product cannot be null for update.");
    }

    [Fact]
    public async Task UpdateProductWithIntelligentStrategyAsync_RepositoryFailure_ShouldReturnFailure()
    {
        // Arrange
        var product = CreateValidProductForPersistence();
        var productInput = CreateTestProductInput("FORD-F150-001");

        _mockProductRepository
            .UpdateAsync(product, Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Update failed"));

        // Act
        var result = await _orchestrator.UpdateProductWithIntelligentStrategyAsync(product, productInput, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Update failed");
    }

    #endregion UpdateProductWithIntelligentStrategyAsync Tests

    #region Helper Methods

    private Product CreateTestProduct(string partNumber)
    {
        return new Product
        {
            ProductId = 0, // Not assigned yet
            PartNumber = partNumber,
            ProductName = $"Product {partNumber}",
            CustomerId = 1,
            LineId = 1
        };
    }

    private ProductInput CreateTestProductInput(string partNumber)
    {
        return new ProductInput
        {
            PartNumber = partNumber,
            ProductName = $"Product {partNumber}",
            CustomerId = 1,
            LineId = 1,
            CreatedBy = "TEST_USER"
        };
    }

    private Product CreateValidProductForPersistence()
    {
        return new Product
        {
            ProductId = 1,
            PartNumber = "FORD-F150-001",
            ProductName = "Ford F-150 Test Product",
            CustomerId = 1,
            Customer = new Customer { CustomerId = 1, Name = "Ford Motor" },
            LineId = 1,
            Line = new Line { LineId = 1, Name = "Production Line 1" },
            CreatedBy = "TEST_USER",
            CreatedOn = DateTime.Now,
            ModifiedBy = "TEST_USER",
            ModifiedOn = DateTime.Now
        };
    }

    #endregion Helper Methods
}
