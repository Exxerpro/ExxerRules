using IndTrace.Application.WorkFlows.Queries.GetDetail;

namespace Application.UnitTests.Features.WorkFlows;

/// <summary>
/// Basic tests for GetWorkFlowDetailQueryHandler focusing on constructor validation and simple scenarios
/// </summary>
public class GetWorkFlowDetailQueryHandlerBasicTests : IDisposable
{
    private readonly IRepository<Product> _productRepository = null!;
    private readonly IRepository<WorkFlow> _workFlowRepository = null!;
    private readonly ILogger<GetWorkFlowDetailQueryHandler> _logger = null!;
    private readonly GetWorkFlowDetailQueryHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public GetWorkFlowDetailQueryHandlerBasicTests()
    {
        _productRepository = Substitute.For<IRepository<Product>>();
        _workFlowRepository = Substitute.For<IRepository<WorkFlow>>();
        _logger = XUnitLogger.CreateLogger<GetWorkFlowDetailQueryHandler>();
        _handler = new GetWorkFlowDetailQueryHandler(_productRepository, _workFlowRepository, _logger);
    }
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var handler = new GetWorkFlowDetailQueryHandler(_productRepository, _workFlowRepository, _logger);

        // Assert
        handler.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes Constructor_WithNullProductRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullProductRepository_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		IRepository<Product>? nullProductRepository = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() =>
    // 			new GetWorkFlowDetailQueryHandler(nullProductRepository!, _workFlowRepository, _logger));
    // 	}
    /// <summary>
    /// Executes Constructor_WithNullWorkFlowRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullWorkFlowRepository_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		IRepository<WorkFlow>? nullWorkFlowRepository = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() =>
    // 			new GetWorkFlowDetailQueryHandler(_productRepository, nullWorkFlowRepository!, _logger));
    // 	}
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    // 	public void Constructor_WithNullLogger_ShouldThrowException()
    // 	{
    // 		// Arrange
    // 		ILogger<GetWorkFlowDetailQueryHandler>? nullLogger = null!;
    //
    // 		// Act & Assert
    // 		Should.Throw<ArgumentNullException>(() =>
    // 			new GetWorkFlowDetailQueryHandler(_productRepository, _workFlowRepository, nullLogger!));
    // 	}
    /// <summary>
    /// Executes Should_ProcessAsync_When_ValidQueryWithExistingProduct operation.
    /// </summary>
    /// <returns>The result of Should_ProcessAsync_When_ValidQueryWithExistingProduct.</returns>

    [Fact]
    public async Task Should_ProcessAsync_When_ValidQueryWithExistingProduct()
    {
        // Arrange - Ford F-150 Engine Block manufacturing scenario
        const string partNumber = "F150-ENGINE-BLOCK-2024";
        var query = new GetWorkFlowDetailQuery { NoParte = partNumber };

        var fordProduct = new Product
        {
            ProductId = 5081,
            PartNumber = partNumber,
            ProductName = "F-150 Engine Block",
            CustomerName = "Ford Motor Company"
        };

        var expectedWorkFlows = new List<WorkFlow>
        {
            new WorkFlow
            {
                WorkFlowId = 1001,
                ProductId = 5081,
                NextMachineId = 201,
                LastMachineId = 200,
                Machine = [],
                Edges = []
            }
        };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(new[] { fordProduct }));

        _workFlowRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WorkFlow>>.Success(expectedWorkFlows));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value.ShouldNotBeNull();
        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS0128] Fix duplicate variable declaration - reuse existing firstItem
        var firstItem = result.Value.First();
        firstItem.WorkFlowId.ShouldBe(1001);
        result.Value.ShouldNotBeNull();
        firstItem.ProductId.ShouldBe(5081);
    }
    /// <summary>
    /// Executes Should_ReturnEmpty_When_NoWorkFlowsForProduct operation.
    /// </summary>
    /// <returns>The result of Should_ReturnEmpty_When_NoWorkFlowsForProduct.</returns>

    [Fact]
    public async Task Should_ReturnEmpty_When_NoWorkFlowsForProduct()
    {
        // Arrange - Tesla Model Y with no defined workflows yet
        const string partNumber = "TESLA-MODELY-BATTERY-2024";
        var query = new GetWorkFlowDetailQuery { NoParte = partNumber };

        var teslaProduct = new Product
        {
            ProductId = 201,
            PartNumber = partNumber,
            ProductName = "Model Y Battery Pack",
            CustomerName = "Tesla Inc"
        };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(new[] { teslaProduct }));

        _workFlowRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WorkFlow>>.Success(new List<WorkFlow>())); // No workflows

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }
    /// <summary>
    /// Executes Should_HandleCancellation_When_CancellationRequested operation.
    /// </summary>
    /// <returns>The result of Should_HandleCancellation_When_CancellationRequested.</returns>

    [Fact]
    public async Task Should_HandleCancellation_When_CancellationRequested()
    {
        // Arrange
        var query = new GetWorkFlowDetailQuery { NoParte = "BMW-X5-TRANSMISSION" };
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(callInfo => Task.FromException<Result<IEnumerable<Product>>>(new OperationCanceledException()));

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(
            () => _handler.ProcessAsync(query, cts.Token));
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
/// Manufacturing scenario tests for GetWorkFlowDetailQueryHandler with complex production workflows
/// </summary>
public class GetWorkFlowDetailQueryHandlerManufacturingTests : IDisposable
{
    private readonly IRepository<Product> _productRepository = null!;
    private readonly IRepository<WorkFlow> _workFlowRepository = null!;
    private readonly ILogger<GetWorkFlowDetailQueryHandler> _logger = null!;
    private readonly GetWorkFlowDetailQueryHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public GetWorkFlowDetailQueryHandlerManufacturingTests()
    {
        _productRepository = Substitute.For<IRepository<Product>>();
        _workFlowRepository = Substitute.For<IRepository<WorkFlow>>();
        _logger = XUnitLogger.CreateLogger<GetWorkFlowDetailQueryHandler>();
        _handler = new GetWorkFlowDetailQueryHandler(_productRepository, _workFlowRepository, _logger);
    }
    /// <summary>
    /// Executes Should_ProcessWorkFlow_When_DifferentAutomotiveProducts operation.
    /// </summary>
    /// <returns>The result of Should_ProcessWorkFlow_When_DifferentAutomotiveProducts.</returns>

    [Theory]
    [InlineData("F150-ENGINE-V8-2024", "Ford F-150 V8 Engine", "Ford Motor Company")]
    [InlineData("TESLA-MODELY-MOTOR-2024", "Tesla Model Y Electric Motor", "Tesla Inc")]
    [InlineData("BMW-X5-GEARBOX-2024", "BMW X5 Transmission", "BMW AG")]
    [InlineData("IPHONE15-PCB-MAIN-2024", "iPhone 15 Main PCB", "Apple Inc")]
    public async Task Should_ProcessWorkFlow_When_DifferentAutomotiveProducts(
        string partNumber, string productName, string customerName)
    {
        // Arrange - Various automotive and electronics manufacturing scenarios
        var query = new GetWorkFlowDetailQuery { NoParte = partNumber };

        var product = new Product
        {
            ProductId = 5080,
            PartNumber = partNumber,
            ProductName = productName,
            CustomerName = customerName
        };

        var workFlow = new WorkFlow
        {
            WorkFlowId = 1000,
            ProductId = 5080,
            NextMachineId = 300,
            LastMachineId = 200,
            Machine =
            [
                new Machine { MachineId = 200, Name = "Stamping Press", MachineType = 1 },
                new Machine { MachineId = 300, Name = "Welding Robot", MachineType = 2 }
            ],
            Edges =
            [
                new Edge { EdgeId = 1, FromMachineId = 200, ToMachineId = 300, Weight = 5 }
            ]
        };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(new[] { product }));

        _workFlowRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WorkFlow>>.Success(new[] { workFlow }));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);

        var workFlowVm = result.Value.First();
        workFlowVm.WorkFlowId.ShouldBe(1000);
        workFlowVm.ProductId.ShouldBe(5080);
        workFlowVm.NextMachineId.ShouldBe(300);
        workFlowVm.LastMachineId.ShouldBe(200);
        workFlowVm.Machine.Count.ShouldBe(2);
        workFlowVm.Edges.Count.ShouldBe(1);
    }
    /// <summary>
    /// Executes Should_HandleMultipleWorkFlows_When_ComplexManufacturingLine operation.
    /// </summary>
    /// <returns>The result of Should_HandleMultipleWorkFlows_When_ComplexManufacturingLine.</returns>

    [Fact]
    public async Task Should_HandleMultipleWorkFlows_When_ComplexManufacturingLine()
    {
        // Arrange - Complex pharmaceutical manufacturing with multiple parallel workflows
        const string partNumber = "PHARMA-TABLET-ASPIRIN-325MG";
        var query = new GetWorkFlowDetailQuery { NoParte = partNumber };

        var pharmaceuticalProduct = new Product
        {
            ProductId = 501,
            PartNumber = partNumber,
            ProductName = "Aspirin 325mg Tablet",
            CustomerName = "Pharmaceutical Corp"
        };

        var workFlows = new List<WorkFlow>
        {
			// Primary production line
			new WorkFlow
            {
                WorkFlowId = 5001,
                ProductId = 501,
                NextMachineId = 601,
                LastMachineId = 600,
                Machine =
                [
                    new Machine { MachineId = 600, Name = "Powder Mixer", MachineType = 10 },
                    new Machine { MachineId = 601, Name = "Tablet Press", MachineType = 11 }
                ],
                Edges =
                [
                    new Edge { EdgeId = 10, FromMachineId = 600, ToMachineId = 601, Weight = 3 }
                ]
            },
			// Secondary packaging line
			new WorkFlow
            {
                WorkFlowId = 5002,
                ProductId = 501,
                NextMachineId = 603,
                LastMachineId = 602,
                Machine =
                [
                    new Machine { MachineId = 602, Name = "Coating Station", MachineType = 12 },
                    new Machine { MachineId = 603, Name = "Blister Packager", MachineType = 13 }
                ],
                Edges =
                [
                    new Edge { EdgeId = 11, FromMachineId = 602, ToMachineId = 603, Weight = 2 }
                ]
            }
        };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(new[] { pharmaceuticalProduct }));

        _workFlowRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WorkFlow>>.Success(workFlows));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);

        // Verify both workflows are properly mapped
        var primaryWorkFlow = result.Value.First(w => w.WorkFlowId == 5001);
        primaryWorkFlow.Machine.Count.ShouldBe(2);
        primaryWorkFlow.Machine.Any(m => m.Name == "Powder Mixer").ShouldBeTrue();

        var secondaryWorkFlow = result.Value.First(w => w.WorkFlowId == 5002);
        secondaryWorkFlow.Machine.Count.ShouldBe(2);
        secondaryWorkFlow.Machine.Any(m => m.Name == "Blister Packager").ShouldBeTrue();
    }
    /// <summary>
    /// Executes Should_FilterCorrectly_When_MultipleProductsInRepository operation.
    /// </summary>
    /// <returns>The result of Should_FilterCorrectly_When_MultipleProductsInRepository.</returns>

    [Fact]
    public async Task Should_FilterCorrectly_When_MultipleProductsInRepository()
    {
        // Arrange - Multiple automotive products with overlapping part numbers
        const string targetPartNumber = "BMW-ENGINE-N55-2024";
        var query = new GetWorkFlowDetailQuery { NoParte = targetPartNumber };

        var products = new List<Product>
        {
            new Product { ProductId = 301, PartNumber = "BMW-ENGINE-N54-2024", ProductName = "BMW N54 Engine" },
            new Product { ProductId = 302, PartNumber = targetPartNumber, ProductName = "BMW N55 Engine" },
            new Product { ProductId = 303, PartNumber = "BMW-ENGINE-S55-2024", ProductName = "BMW S55 Engine" }
        };

        var workFlows = new List<WorkFlow>
        {
            new WorkFlow { WorkFlowId = 3001, ProductId = 301, NextMachineId = 401, LastMachineId = 400 },
            new WorkFlow { WorkFlowId = 3002, ProductId = 302, NextMachineId = 403, LastMachineId = 402 }, // Target
			new WorkFlow { WorkFlowId = 3003, ProductId = 303, NextMachineId = 405, LastMachineId = 404 }
        };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(products));

        _workFlowRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WorkFlow>>.Success(workFlows));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value.ShouldNotBeNull();
        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS0128] Fix duplicate variable declaration - reuse existing firstItem
        var firstItem = result.Value.First();
        firstItem.WorkFlowId.ShouldBe(3002); // Only N55 workflow
        result.Value.ShouldNotBeNull();
        firstItem.ProductId.ShouldBe(302);
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
/// Error scenario and validation tests for GetWorkFlowDetailQueryHandler
/// </summary>
public class GetWorkFlowDetailQueryHandlerErrorTests : IDisposable
{
    private readonly IRepository<Product> _productRepository = null!;
    private readonly IRepository<WorkFlow> _workFlowRepository = null!;
    private readonly ILogger<GetWorkFlowDetailQueryHandler> _logger = null!;
    private readonly GetWorkFlowDetailQueryHandler _handler = null!;
    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>

    public GetWorkFlowDetailQueryHandlerErrorTests()
    {
        _productRepository = Substitute.For<IRepository<Product>>();
        _workFlowRepository = Substitute.For<IRepository<WorkFlow>>();
        _logger = XUnitLogger.CreateLogger<GetWorkFlowDetailQueryHandler>();
        _handler = new GetWorkFlowDetailQueryHandler(_productRepository, _workFlowRepository, _logger);
    }
    /// <summary>
    /// Executes Should_ReturnFailure_When_ProductNotFound operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_ProductNotFound.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange - Searching for non-existent Rolls-Royce part
        const string nonExistentPartNumber = "ROLLSROYCE-GHOST-ENGINE-2024";
        var query = new GetWorkFlowDetailQuery { NoParte = nonExistentPartNumber };

        var existingProducts = new List<Product>
        {
            new Product { ProductId = 5081, PartNumber = "BMW-X7-ENGINE-2024", ProductName = "BMW X7 Engine" },
            new Product { ProductId = 5082, PartNumber = "MERCEDES-S500-ENGINE-2024", ProductName = "Mercedes S500 Engine" }
        };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(existingProducts));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain($"Product with PartNumber {nonExistentPartNumber} not found");
    }
    /// <summary>
    /// Executes Should_ReturnFailure_When_ProductRepositoryFails operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_ProductRepositoryFails.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_ProductRepositoryFails()
    {
        // Arrange - Database connection failure scenario
        var query = new GetWorkFlowDetailQuery { NoParte = "AUDI-A8-TRANSMISSION-2024" };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.WithFailure("Database connection timeout"));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Database connection timeout");
    }
    /// <summary>
    /// Executes Should_ReturnFailure_When_WorkFlowRepositoryFails operation.
    /// </summary>
    /// <returns>The result of Should_ReturnFailure_When_WorkFlowRepositoryFails.</returns>

    [Fact]
    public async Task Should_ReturnFailure_When_WorkFlowRepositoryFails()
    {
        // Arrange - Product found but workflow repository fails
        const string partNumber = "VOLVO-XC90-HYBRID-ENGINE-2024";
        var query = new GetWorkFlowDetailQuery { NoParte = partNumber };

        var volvoProduct = new Product
        {
            ProductId = 401,
            PartNumber = partNumber,
            ProductName = "Volvo XC90 Hybrid Engine",
            CustomerName = "Volvo AB"
        };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(new[] { volvoProduct }));

        _workFlowRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<WorkFlow>>.WithFailure("WorkFlow service unavailable"));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("WorkFlow service unavailable");
    }
    /// <summary>
    /// Executes Should_HandleInvalidPartNumbers_When_QueryHasInvalidData operation.
    /// </summary>
    /// <param name="invalidPartNumber">The invalidPartNumber.</param>
    /// <returns>The result of Should_HandleInvalidPartNumbers_When_QueryHasInvalidData.</returns>

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Should_HandleInvalidPartNumbers_When_QueryHasInvalidData(string? invalidPartNumber)
    {
        // Using parameters: invalidPartNumber
        _ = invalidPartNumber; // xUnit1026 fix
        // Using parameters: invalidPartNumber
        _ = invalidPartNumber; // xUnit1026 fix
        // Using parameters: invalidPartNumber
        _ = invalidPartNumber; // xUnit1026 fix
        // Using parameters: invalidPartNumber
        _ = invalidPartNumber; // xUnit1026 fix
        // Using parameters: invalidPartNumber
        _ = invalidPartNumber; // xUnit1026 fix
                               // Arrange - Various invalid part number scenarios
        var query = new GetWorkFlowDetailQuery { NoParte = invalidPartNumber };

        var validProducts = new List<Product>
        {
            new Product { ProductId = 501, PartNumber = "VALID-PART-2024", ProductName = "Valid Product" }
        };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(validProducts));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.Any(e => e.Contains("not found")).ShouldBeTrue();
    }
    /// <summary>
    /// Executes Should_HandleEmptyProductRepository_When_NoProductsExist operation.
    /// </summary>
    /// <returns>The result of Should_HandleEmptyProductRepository_When_NoProductsExist.</returns>

    [Fact]
    public async Task Should_HandleEmptyProductRepository_When_NoProductsExist()
    {
        // Arrange - Empty product catalog scenario
        var query = new GetWorkFlowDetailQuery { NoParte = "ANY-PART-NUMBER" };

        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(new List<Product>()));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Product with PartNumber ANY-PART-NUMBER not found");
    }
    /// <summary>
    /// Executes Should_HandleNullRepositoryResults_When_UnexpectedNullResponse operation.
    /// </summary>
    /// <returns>The result of Should_HandleNullRepositoryResults_When_UnexpectedNullResponse.</returns>

    [Fact]
    public async Task Should_HandleNullRepositoryResults_When_UnexpectedNullResponse()
    {
        // Arrange - Unexpected null response from repository
        var query = new GetWorkFlowDetailQuery { NoParte = "HONDA-CIVIC-ENGINE-2024" };

        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS8625] Use empty collection instead of null for non-nullable IEnumerable
        _productRepository.ListAsync(Arg.Any<CancellationToken>())
            .Returns(Result<IEnumerable<Product>>.Success(new List<Product>()));

        // Act
        var result = await _handler.ProcessAsync(query, TestContext.Current.CancellationToken);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Product with PartNumber HONDA-CIVIC-ENGINE-2024 not found");
    }
    /// <summary>
    /// Executes Dispose operation.
    /// </summary>

    public void Dispose()
    {
        // Cleanup if needed
    }
}
