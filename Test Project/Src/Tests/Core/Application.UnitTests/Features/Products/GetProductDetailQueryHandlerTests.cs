namespace Application.UnitTests.Features.Products;

/// <summary>
/// Unit tests for GetProductDetailQueryHandler
/// </summary>
public class GetProductDetailQueryHandlerTests
{
    private readonly IRepository<Product> _productRepository = Substitute.For<IRepository<Product>>();
    private readonly IRepository<Customer> _customerRepository = Substitute.For<IRepository<Customer>>();
    private readonly ILogger<GetProductDetailQueryHandler> _logger = XUnitLogger.CreateLogger<GetProductDetailQueryHandler>();
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new GetProductDetailQueryHandler(_productRepository, _customerRepository, _logger);

        // Assert
        handler.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes Constructor_WithNullProductRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullProductRepository_ShouldThrowException()
    //     {
    //         // Arrange
    //         IRepository<Product>? nullRepository = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetProductDetailQueryHandler(nullRepository!, _customerRepository, _logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullCustomerRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullCustomerRepository_ShouldThrowException()
    //     {
    //         // Arrange
    //         IRepository<Customer>? nullRepository = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetProductDetailQueryHandler(_productRepository, nullRepository!, _logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         ILogger<GetProductDetailQueryHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetProductDetailQueryHandler(_productRepository, _customerRepository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Process_WithValidProductId_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidProductId_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidProductId_ShouldReturnSuccess()
    {
        // Arrange
        var handler = new GetProductDetailQueryHandler(_productRepository, _customerRepository, _logger);
        var query = new GetProductDetailQuery { ProductId = 5080 };
        var product = new Product { ProductId = 5080, ProductName = "Test Product", CustomerId = 200 };
        var customer = new Customer { CustomerId = 200, Name = "Test Customer" };

        _productRepository.GetByIdAsync(query.ProductId, Arg.Any<CancellationToken>())
            .Returns(Result<Product?>.Success(product));
        _customerRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Customer>>.Success(new List<Customer> { customer }));

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ProductId.ShouldBe(5080);
    }
    /// <summary>
    /// Executes Process_WhenProductNotFound_ShouldReturnFailure operation.
    /// </summary>
    /// <returns>The result of Process_WhenProductNotFound_ShouldReturnFailure.</returns>

    [Fact]
    public async Task Process_WhenProductNotFound_ShouldReturnFailure()
    {
        // Arrange
        var handler = new GetProductDetailQueryHandler(_productRepository, _customerRepository, _logger);
        var query = new GetProductDetailQuery { ProductId = 5080 };

        _productRepository.GetByIdAsync(query.ProductId, Arg.Any<CancellationToken>())
            .Returns(Result<Product?>.Success(null));

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain($"Product with ID {query.ProductId} not found");
    }
    /// <summary>
    /// Executes Process_WithValidProductName_ShouldReturnSuccess operation.
    /// </summary>
    /// <returns>The result of Process_WithValidProductName_ShouldReturnSuccess.</returns>

    [Fact]
    public async Task Process_WithValidProductName_ShouldReturnSuccess()
    {
        // Arrange
        var handler = new GetProductDetailQueryHandler(_productRepository, _customerRepository, _logger);
        var query = new GetProductDetailQuery { ProductName = "Test Product" };
        var product = new Product { ProductId = 5080, ProductName = "Test Product", CustomerId = 200, CustomerName = "Test Customer" };
        var customer = new Customer { CustomerId = 200, Name = "Test Customer" };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(new List<Product> { product }));
        _customerRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Customer>>.Success(new List<Customer> { customer }));

        // Act
        var result = await handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ProductName.ShouldBe("Test Product");
    }
}
