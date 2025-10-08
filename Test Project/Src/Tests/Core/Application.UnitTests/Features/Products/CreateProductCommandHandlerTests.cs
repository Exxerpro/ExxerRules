using IndTrace.Application.Products.Events;
using CreateProductCommand = IndTrace.Application.Products.Commands.Create.CreateProductCommand;
using IndTrace.Application.Products.Services.Interfaces;
using IndTrace.Domain.Services.Products;
using Meziantou.Extensions.Logging.Xunit.v3;

namespace Application.UnitTests.Features.Products;

/// <summary>
/// Comprehensive unit tests for CreateProductCommandHandler with SRP services.
/// Maintains all original test coverage while adapting to SRP architecture.
/// </summary>
public class CreateProductCommandHandlerTests
{
    // Domain Services
    private readonly IProductValidator _productValidator;

    private readonly IProductFactory _productFactory;
    private readonly IProductEventFactory _productEventFactory;

    // Application Services
    private readonly IProductUniquenessValidator _uniquenessValidator;

    private readonly ICustomerLookupService _customerLookupService;
    private readonly ILineLookupService _lineLookupService;
    private readonly IWorkflowOrchestrator _workflowOrchestrator;
    private readonly IRuleOrchestrator _ruleOrchestrator;
    private readonly IRecipeOrchestrator _recipeOrchestrator;
    private readonly IProductPersistenceOrchestrator _persistenceOrchestrator;

    // Infrastructure
    private readonly ILogger<CreateProductCommandHandler> _logger;

    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        // Domain services
        _productValidator = Substitute.For<IProductValidator>();
        _productFactory = Substitute.For<IProductFactory>();
        _productEventFactory = Substitute.For<IProductEventFactory>();

        // Application services
        _uniquenessValidator = Substitute.For<IProductUniquenessValidator>();
        _customerLookupService = Substitute.For<ICustomerLookupService>();
        _lineLookupService = Substitute.For<ILineLookupService>();
        _workflowOrchestrator = Substitute.For<IWorkflowOrchestrator>();
        _ruleOrchestrator = Substitute.For<IRuleOrchestrator>();
        _recipeOrchestrator = Substitute.For<IRecipeOrchestrator>();
        _persistenceOrchestrator = Substitute.For<IProductPersistenceOrchestrator>();

        // Infrastructure
        _logger = XUnitLogger.CreateLogger<CreateProductCommandHandler>();

        _handler = new CreateProductCommandHandler(
            _productValidator, _productFactory, _productEventFactory,
            _uniquenessValidator, _customerLookupService, _lineLookupService,
            _workflowOrchestrator, _ruleOrchestrator, _recipeOrchestrator,
            _persistenceOrchestrator, _logger);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var instance = new CreateProductCommandHandler(
            _productValidator, _productFactory, _productEventFactory,
            _uniquenessValidator, _customerLookupService, _lineLookupService,
            _workflowOrchestrator, _ruleOrchestrator, _recipeOrchestrator,
            _persistenceOrchestrator, _logger);

        // Assert
        instance.ShouldNotBeNull();
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new CreateProductCommandHandler(
            _productValidator, _productFactory, _productEventFactory,
            _uniquenessValidator, _customerLookupService, _lineLookupService,
            _workflowOrchestrator, _ruleOrchestrator, _recipeOrchestrator,
            _persistenceOrchestrator, null!));
    }

    [Fact]
    public void Constructor_WithNullProductValidator_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Should.Throw<ArgumentNullException>(() => new CreateProductCommandHandler(
            null!, _productFactory, _productEventFactory,
            _uniquenessValidator, _customerLookupService, _lineLookupService,
            _workflowOrchestrator, _ruleOrchestrator, _recipeOrchestrator,
            _persistenceOrchestrator, _logger));
    }

    [Fact]
    public async Task ProcessAsync_WithValidCommand_ShouldReturnSuccess()
    {
        // Arrange
        var command = CreateValidCommand();
        var customer = new Customer { CustomerId = 1, Name = "Test Customer" };
        var line = new Line { LineId = 1, Name = "Test Line" };
        var product = new Product { ProductId = 123, PartNumber = "TEST123" };
        var productEvent = new ProductCreatedEvent
        {
            ProductId = 123,
            PartNumber = "TEST123",
            Name = "Test Product"
        };

        // Mock SRP services for successful product creation pipeline
        SetupSuccessfulValidationPipeline(command.Product, customer, line);
        SetupSuccessfulCreationPipeline(product, productEvent);

        // Act
        var result = await _handler.ProcessAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.PartNumber.ShouldBe(command.Product.PartNumber);

        // Verify all SRP services were called
        await VerifySuccessfulCreationFlow(command.Product);
    }

    [Fact]
    public async Task ProcessAsync_WhenValidationFails_ShouldReturnFailure()
    {
        // Arrange
        var command = CreateValidCommand();

        // Mock validation failure
        _productValidator.ValidateProductData(Arg.Any<ProductInput>())
            .Returns(Result.WithFailure("Invalid product data"));

        // Act
        var result = await _handler.ProcessAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Invalid product data");
    }

    [Fact]
    public async Task ProcessAsync_WhenCustomerNotFound_ShouldReturnFailure()
    {
        // Arrange
        var command = CreateValidCommand();

        // Mock successful validation but customer not found
        _productValidator.ValidateProductData(Arg.Any<ProductInput>())
            .Returns(Result.Success());

        _uniquenessValidator.ValidateProductUniquenessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        _customerLookupService.ResolveCustomerAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Customer>.WithFailure("Customer not found 1")));

        // Act
        var result = await _handler.ProcessAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Customer not found 1");
    }

    [Fact]
    public async Task ProcessAsync_WhenPersistenceFails_ShouldReturnFailure()
    {
        // Arrange
        var command = CreateValidCommand();
        var customer = new Customer { CustomerId = 1, Name = "Test Customer" };
        var line = new Line { LineId = 1, Name = "Test Line" };
        var product = new Product { ProductId = 123, PartNumber = "TEST123" };

        // Mock successful validation but persistence failure
        SetupSuccessfulValidationPipeline(command.Product, customer, line);

        _productFactory.CreateProduct(Arg.Any<ProductInput>(), Arg.Any<Customer>(), Arg.Any<Line>())
            .Returns(product);
        _productFactory.TryParseLastInteger(Arg.Any<string>())
            .Returns((true, 123));
        _productFactory.GetDynamicOffset(Arg.Any<int>())
            .Returns(0);

        _persistenceOrchestrator.CreateProductWithIntelligentIdAsync(Arg.Any<Product>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Product>.WithFailure("Database error")));

        // Act
        var result = await _handler.ProcessAsync(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldContain("Database error");
    }

    [Fact]
    public async Task ProcessAsync_ShouldPassCancellationTokenToServices()
    {
        // Arrange
        var command = CreateValidCommand();
        var cancellationToken = CancellationToken.None;
        var customer = new Customer { CustomerId = 1, Name = "Test Customer" };
        var line = new Line { LineId = 1, Name = "Test Line" };
        var product = new Product { ProductId = 123, PartNumber = "TEST123" };
        var productEvent = new ProductCreatedEvent
        {
            ProductId = 123,
            PartNumber = "TEST123",
            Name = "Test Product"
        };

        // Mock successful flow
        SetupSuccessfulValidationPipeline(command.Product, customer, line);
        SetupSuccessfulCreationPipeline(product, productEvent);

        // Act
        var result = await _handler.ProcessAsync(command, cancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        // Verify cancellation token was passed to async services
        await _uniquenessValidator.Received(1).ValidateProductUniquenessAsync(Arg.Any<string>(), Arg.Any<string>(), cancellationToken);
        await _customerLookupService.Received(1).ResolveCustomerAsync(Arg.Any<int>(), Arg.Any<string>(), cancellationToken);
        await _lineLookupService.Received(1).GetLineByIdAsync(Arg.Any<int>(), cancellationToken);
        await _persistenceOrchestrator.Received(1).CreateProductWithIntelligentIdAsync(Arg.Any<Product>(), Arg.Any<int>(), Arg.Any<int>(), cancellationToken);
    }

    private void SetupSuccessfulValidationPipeline(ProductDto productDto, Customer customer, Line line)
    {
        _productValidator.ValidateProductData(Arg.Any<ProductInput>())
            .Returns(Result.Success());

        _uniquenessValidator.ValidateProductUniquenessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        _customerLookupService.ResolveCustomerAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Customer>.Success(customer)));

        // Line lookup service mocks (using GetLineByIdAsync as per handler implementation)
        _lineLookupService.GetLineByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Line>.Success(line)));

        _lineLookupService.GetLineByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Line>.Success(line)));
    }

    private void SetupSuccessfulCreationPipeline(Product product, ProductCreatedEvent productEvent)
    {
        _productFactory.CreateProduct(Arg.Any<ProductInput>(), Arg.Any<Customer>(), Arg.Any<Line>())
            .Returns(product);
        _productFactory.TryParseLastInteger(Arg.Any<string>())
            .Returns((true, 123));
        _productFactory.GetDynamicOffset(Arg.Any<int>())
            .Returns(0);

        _persistenceOrchestrator.CreateProductWithIntelligentIdAsync(Arg.Any<Product>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Product>.Success(product)));

        _workflowOrchestrator.CreateAndPersistWorkflowsAsync(Arg.Any<Product>(), Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<IEnumerable<WorkFlow>>.Success(new List<WorkFlow>())));

        _ruleOrchestrator.CreateAndLinkRuleAsync(Arg.Any<RuleDto>(), Arg.Any<Product>(), Arg.Any<IEnumerable<WorkFlow>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<Rule>.Success(new Rule { RuleId = 999 })));

        _recipeOrchestrator.CreateAndPersistRecipesAsync(Arg.Any<RecipeDto>(), Arg.Any<Product>(), Arg.Any<IEnumerable<WorkFlow>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result<IEnumerable<Recipe>>.Success(new List<Recipe>())));

        _productEventFactory.CreateProductCreatedEvent(Arg.Any<Product>())
            .Returns(Result<ProductCreatedEvent>.Success(productEvent));
    }

    private async Task VerifySuccessfulCreationFlow(ProductDto productDto)
    {
        _productValidator.Received(1).ValidateProductData(Arg.Any<ProductInput>());
        await _uniquenessValidator.Received(1).ValidateProductUniquenessAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _customerLookupService.Received(1).ResolveCustomerAsync(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _lineLookupService.Received(1).GetLineByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>());
        _productFactory.Received(1).CreateProduct(Arg.Any<ProductInput>(), Arg.Any<Customer>(), Arg.Any<Line>());
        await _persistenceOrchestrator.Received(1).CreateProductWithIntelligentIdAsync(Arg.Any<Product>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        _productEventFactory.Received(1).CreateProductCreatedEvent(Arg.Any<Product>());
    }

    private static CreateProductCommand CreateValidCommand()
    {
        var productDto = new ProductCreationDto
        {
            Product = new ProductDto
            {
                PartNumber = "TEST123",
                ProductName = "Test Product",
                Description = "Test Product Description",
                CustomerName = "Test Customer",
                CustomerId = 1,
                IsActive = 1,
                Version = 1,
                LineId = 1
            },
            Machines = new List<int> { 1, 2, 3 },
            Rule = new RuleDto
            {
                RuleJson = "{\"test\": true}",
                Name = "Test Rule",
                Description = "Test Rule Description",
                Version = 1,
                IsActive = true
            },
            Recipe = new RecipeDto
            {
                CycleTimeMinimum = 10,
                CycleTimeMaximum = 30
            }
        };

        return new CreateProductCommand(productDto);
    }
}
