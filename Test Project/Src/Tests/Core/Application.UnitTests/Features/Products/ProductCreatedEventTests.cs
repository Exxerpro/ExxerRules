using IndTrace.Application.Products.Events;
using IndTrace.Application.Models.Interfaces;
using IndTrace.Application.Notifications.Models;

namespace Application.UnitTests.Features.Products;

/// <summary>
/// Unit tests for ProductCreatedEvent
/// </summary>
public class ProductCreatedEventTests
{
    /// <summary>
    /// Executes Constructor_WithDefaultConstructor_ShouldCreateInstanceWithDefaultValues operation.
    /// </summary>
    [Fact]
    public void Constructor_WithDefaultConstructor_ShouldCreateInstanceWithDefaultValues()
    {
        // Act
        var productEvent = new ProductCreatedEvent();

        // Assert
        productEvent.ShouldNotBeNull();
        productEvent.ProductId.ShouldBe(0);
        productEvent.CustomerId.ShouldBe(0);
        productEvent.PartNumber.ShouldBe(string.Empty);
        productEvent.Name.ShouldBe(string.Empty);
        productEvent.IsActive.ShouldBe(0);
        productEvent.Version.ShouldBe(0);
        //productEvent.PartNumberCustomer.ShouldBe(string.Empty);
        //productEvent.AliasNoParte.ShouldBe(string.Empty);
        productEvent.Description.ShouldBeNullOrEmpty();
    }

    /// <summary>
    /// Executes Properties_WhenSet_ShouldReturnCorrectValues operation.
    /// </summary>

    [Fact]
    public void Properties_WhenSet_ShouldReturnCorrectValues()
    {
        // Arrange
        var productEvent = new ProductCreatedEvent
        {
            ProductId = 12345,
            CustomerId = 100,
            PartNumber = "PART-ABC-123",
            Name = "Industrial Widget",
            IsActive = 1,
            Version = 2,
            CustomerPartNumber = "CUST-PART-456",
            AliasPartNumber = "ALIAS-789",
            Description = "High-precision industrial component"
        };

        // Assert
        productEvent.ProductId.ShouldBe(12345);
        productEvent.CustomerId.ShouldBe(100);
        productEvent.PartNumber.ShouldBe("PART-ABC-123");
        productEvent.Name.ShouldBe("Industrial Widget");
        productEvent.IsActive.ShouldBe(1);
        productEvent.Version.ShouldBe(2);
        productEvent.CustomerPartNumber.ShouldBe("CUST-PART-456");
        productEvent.AliasPartNumber.ShouldBe("ALIAS-789");
        productEvent.Description.ShouldBe("High-precision industrial component");
    }

    /// <summary>
    /// Executes ProductCreatedEvent_ShouldImplementINotification operation.
    /// </summary>

    [Fact]
    public void ProductCreatedEvent_ShouldImplementINotification()
    {
        // Arrange & Act
        var productEvent = new ProductCreatedEvent();

        // Assert
        productEvent.ShouldBeAssignableTo<INotification>();
    }

    // FromProduct Static Method Tests
    /// <summary>
    /// Executes FromProduct_WithNullProduct_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void FromProduct_WithNullProduct_ShouldReturnFailureResult()
    {
        // Arrange
        Product? nullProduct = null!;

        // Act
        var result = ProductCreatedEvent.FromProduct(nullProduct!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Product cannot be null for event creation.");
    }

    /// <summary>
    /// Executes FromProduct_WithValidProduct_ShouldMapAllProperties operation.
    /// </summary>

    [Fact]
    public void FromProduct_WithValidProduct_ShouldMapAllProperties()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 12345,
            CustomerId = 100,
            PartNumber = "PART-ABC-123",
            ProductName = "Industrial Widget",
            IsActive = 1,
            Version = 2,
            CustomerPartNumber = "CUST-PART-456",
            AliasPartNumber = "ALIAS-789",
            Description = "High-precision industrial component"
        };

        // Act
        var productEventWrapper = ProductCreatedEvent.FromProduct(product);

        // Assert
        productEventWrapper.IsSuccess.ShouldBeTrue();
        productEventWrapper.Value.ShouldNotBeNull();
        var productEvent = productEventWrapper.Value;
        productEvent.ShouldNotBeNull();
        productEvent.ShouldNotBeNull();
        productEvent.ShouldNotBeNull();
        productEvent.ProductId.ShouldBe(12345);
        productEvent.CustomerId.ShouldBe(100);
        productEvent.PartNumber.ShouldBe("PART-ABC-123");
        productEvent.Name.ShouldBe("Industrial Widget");
        productEvent.IsActive.ShouldBe(1);
        productEvent.Version.ShouldBe(2);
        productEvent.CustomerPartNumber.ShouldBe("CUST-PART-456");
        productEvent.AliasPartNumber.ShouldBe("ALIAS-789");
        productEvent.Description.ShouldBe("High-precision industrial component");
    }

    /// <summary>
    /// Executes FromProduct_WithMinimalProduct_ShouldMapBasicProperties operation.
    /// </summary>

    [Fact]
    public void FromProduct_WithMinimalProduct_ShouldMapBasicProperties()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 1,
            PartNumber = "MIN-PART",
            ProductName = "Minimal Product"
        };

        // Act
        var productEventWrapper = ProductCreatedEvent.FromProduct(product);

        // Assert
        productEventWrapper.IsSuccess.ShouldBeTrue();
        productEventWrapper.Value.ShouldNotBeNull();
        var productEvent = productEventWrapper.Value;
        productEvent.ShouldNotBeNull();
        productEvent.ShouldNotBeNull();
        productEvent.ShouldNotBeNull();
        productEvent.ProductId.ShouldBe(1);
        productEvent.PartNumber.ShouldBe("MIN-PART");
        productEvent.Name.ShouldBe("Minimal Product");
        productEvent.CustomerId.ShouldBe(0);
        productEvent.IsActive.ShouldBe(0);
        productEvent.Version.ShouldBe(0);
    }

    /// <summary>
    /// Executes FromProduct_WithNullStringProperties_ShouldHandleGracefully operation.
    /// </summary>

    [Fact]
    public void FromProduct_WithNullStringProperties_ShouldHandleGracefully()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 123,
            PartNumber = null!,
            ProductName = null!,
            CustomerPartNumber = null!,
            AliasPartNumber = null!,
            Description = null!
        };

        // Act
        var productEventWrapper = ProductCreatedEvent.FromProduct(product);

        // Assert
        productEventWrapper.IsSuccess.ShouldBeTrue();
        productEventWrapper.Value.ShouldNotBeNull();
        var productEvent = productEventWrapper.Value;
        productEvent.ShouldNotBeNull();
        productEvent.ShouldNotBeNull();
        productEvent.ShouldNotBeNull();
        productEvent.ProductId.ShouldBe(123);
        productEvent.PartNumber.ShouldBe(string.Empty);
        productEvent.Name.ShouldBe(string.Empty);
        productEvent.CustomerId.ShouldBe(0);
        productEvent.IsActive.ShouldBe(0);
        productEvent.Version.ShouldBe(0);
    }

    // ToProduct Static Method Tests
    /// <summary>
    /// Executes ToProduct_WithNullProductCreatedEvent_ShouldReturnFailureResult operation.
    /// </summary>

    [Fact]
    public void ToProduct_WithNullProductCreatedEvent_ShouldReturnFailureResult()
    {
        // Arrange
        ProductCreatedEvent? nullEvent = null!;

        // Act
        var result = ProductCreatedEvent.ToProduct(nullEvent!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors.ShouldContain("Product cannot be null for event creation.");
    }

    /// <summary>
    /// Executes ToProduct_WithValidProductCreatedEvent_ShouldMapAllProperties operation.
    /// </summary>

    [Fact]
    public void ToProduct_WithValidProductCreatedEvent_ShouldMapAllProperties()
    {
        // Arrange
        var productEvent = new ProductCreatedEvent
        {
            ProductId = 12345,
            CustomerId = 100,
            PartNumber = "PART-ABC-123",
            Name = "Industrial Widget",
            IsActive = 1,
            Version = 2,
            CustomerPartNumber = "CUST-PART-456",
            AliasPartNumber = "ALIAS-789",
            Description = "High-precision industrial component"
        };

        // Act
        var product = ProductCreatedEvent.ToProduct(productEvent);

        // Assert
        product.ShouldNotBeNull();
        //[Fix]
        //CLAUDE
        //Date: 28/08/2025
        //Reason: [CS8602] - Add null-forgiving operator since product.ShouldNotBeNull() was verified
        product.Value!.ProductId.ShouldBe(12345);
        product.Value!.CustomerId.ShouldBe(100);
        product.Value!.PartNumber.ShouldBe("PART-ABC-123");
        product.Value!.ProductName.ShouldBe("Industrial Widget");
        product.Value!.IsActive.ShouldBe(1);
        product.Value!.Version.ShouldBe(2);
        product.Value!.CustomerPartNumber.ShouldBe("CUST-PART-456");
        product.Value!.AliasPartNumber.ShouldBe("ALIAS-789");
        product.Value!.Description.ShouldBe("High-precision industrial component");
    }

    /// <summary>
    /// Executes ToProduct_WithMinimalProductCreatedEvent_ShouldMapBasicProperties operation.
    /// </summary>

    [Fact]
    public void ToProduct_WithMinimalProductCreatedEvent_ShouldMapBasicProperties()
    {
        // Arrange
        var productEvent = new ProductCreatedEvent
        {
            ProductId = 1,
            PartNumber = "MIN-PART",
            Name = "Minimal Product"
        };

        // Act
        var product = ProductCreatedEvent.ToProduct(productEvent);

        // Assert
        product.ShouldNotBeNull();
        //[Fix]
        //CLAUDE
        //Date: 28/08/2025
        //Reason: [CS8602] - Add null-forgiving operator since product.ShouldNotBeNull() was verified
        product.Value!.ProductId.ShouldBe(1);
        product.Value!.PartNumber.ShouldBe("MIN-PART");
        product.Value!.ProductName.ShouldBe("Minimal Product");
        product.Value!.CustomerId.ShouldBe(0);
        product.Value!.IsActive.ShouldBe(0);
        product.Value!.Version.ShouldBe(0);
    }

    // Round-trip Conversion Tests
    /// <summary>
    /// Executes FromProduct_ThenToProduct_ShouldPreserveAllProperties operation.
    /// </summary>

    [Fact]
    public void FromProduct_ThenToProduct_ShouldPreserveAllProperties()
    {
        // Arrange
        var originalProduct = new Product
        {
            ProductId = 12345,
            CustomerId = 100,
            PartNumber = "ROUND-TRIP-PART",
            ProductName = "Round Trip Product",
            IsActive = 1,
            Version = 3,
            CustomerPartNumber = "RT-CUST-PART",
            AliasPartNumber = "RT-ALIAS",
            Description = "Round trip test product"
        };

        // Act
        var productEvent = ProductCreatedEvent.FromProduct(originalProduct);
        //[Fix]
        //CLAUDE
        //Date: 29/08/2025
        //Reason: [CS8604] Add null check before using productEvent.Value
        productEvent.Value.ShouldNotBeNull();
        var convertedProduct = ProductCreatedEvent.ToProduct(productEvent.Value);

        // Assert
        convertedProduct.Value!.ProductId.ShouldBe(originalProduct.ProductId);
        convertedProduct.Value!.CustomerId.ShouldBe(originalProduct.CustomerId);
        convertedProduct.Value!.PartNumber.ShouldBe(originalProduct.PartNumber);
        convertedProduct.Value!.ProductName.ShouldBe(originalProduct.ProductName);
        convertedProduct.Value!.IsActive.ShouldBe(originalProduct.IsActive);
        convertedProduct.Value!.Version.ShouldBe(originalProduct.Version);
        convertedProduct.Value!.CustomerPartNumber.ShouldBe(originalProduct.CustomerPartNumber);
        convertedProduct.Value!.AliasPartNumber.ShouldBe(originalProduct.AliasPartNumber);
        convertedProduct.Value!.Description.ShouldBe(originalProduct.Description);
    }

    // Industrial Scenario Tests
    /// <summary>
    /// Executes ProductCreatedEvent_WithIndustrialAutomotiveScenario_ShouldHandleComplexData operation.
    /// </summary>

    [Fact]
    public void ProductCreatedEvent_WithIndustrialAutomotiveScenario_ShouldHandleComplexData()
    {
        // Arrange - Automotive manufacturing scenario
        var productEvent = new ProductCreatedEvent
        {
            ProductId = 501234,
            PartNumber = "AUTO-ENGINE-BLOCK-V8-2024",
            Name = "V8 Engine Block Assembly",
            CustomerId = 1001,
            CustomerPartNumber = "FORD-ENG-V8-2024",
            AliasPartNumber = "F150-ENGINE-BLOCK",
            Description = "High-performance V8 engine block for F-150 series",
            IsActive = 1,
            Version = 5
        };

        // Assert - Verify automotive scenario
        productEvent.PartNumber.ShouldBe("AUTO-ENGINE-BLOCK-V8-2024");
        productEvent.Name.ShouldBe("V8 Engine Block Assembly");
        productEvent.CustomerPartNumber.ShouldBe("FORD-ENG-V8-2024");
        productEvent.AliasPartNumber.ShouldBe("F150-ENGINE-BLOCK");
        productEvent.Description.ShouldContain("F-150 series");
    }

    /// <summary>
    /// Executes ProductCreatedEvent_WithIndustrialElectronicsScenario_ShouldHandleComplexData operation.
    /// </summary>

    [Fact]
    public void ProductCreatedEvent_WithIndustrialElectronicsScenario_ShouldHandleComplexData()
    {
        // Arrange - Electronics manufacturing scenario
        var productEvent = new ProductCreatedEvent
        {
            ProductId = 700456,
            PartNumber = "PCB-MAIN-CTRL-V3.2",
            Name = "Main Control PCB Assembly",
            CustomerId = 2001,
            CustomerPartNumber = "SIEMENS-PCB-CTRL-32",
            AliasPartNumber = "MAIN-BOARD-V3",
            Description = "Primary control board for industrial automation systems",
            IsActive = 1,
            Version = 3
        };

        // Assert - Verify electronics scenario
        productEvent.PartNumber.ShouldBe("PCB-MAIN-CTRL-V3.2");
        productEvent.Name.ShouldBe("Main Control PCB Assembly");
        productEvent.CustomerPartNumber.ShouldBe("SIEMENS-PCB-CTRL-32");
        productEvent.AliasPartNumber.ShouldBe("MAIN-BOARD-V3");
        productEvent.Description.ShouldContain("automation systems");
    }

    // ProductCreatedHandler Tests
    /// <summary>
    /// Executes ProductCreatedHandler_Constructor_ShouldCreateInstance operation.
    /// </summary>

    [Fact]
    public void ProductCreatedHandler_Constructor_ShouldCreateInstance()
    {
        // Arrange
        var notificationService = Substitute.For<INotificationService>();

        // Act
        var handler = new ProductCreatedEvent.ProductCreatedHandler(notificationService);

        // Assert
        handler.ShouldNotBeNull();
        handler.ShouldBeAssignableTo<INotificationHandler<ProductCreatedEvent>>();
    }

    /// <summary>
    /// Executes ProductCreatedHandler_Process_ShouldCallNotificationService operation.
    /// </summary>
    /// <returns>The result of ProductCreatedHandler_Process_ShouldCallNotificationService.</returns>

    [Fact]
    public async Task ProductCreatedHandler_Process_ShouldCallNotificationService()
    {
        // Arrange
        var notificationService = Substitute.For<INotificationService>();
        var handler = new ProductCreatedEvent.ProductCreatedHandler(notificationService);
        var productEvent = new ProductCreatedEvent
        {
            ProductId = 123,
            PartNumber = "TEST-PART",
            Name = "Test Product"
        };

        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: Pattern B Fix - Added mock setup for Railway-Oriented Programming Result<T> pattern
        notificationService.SendAsync(Arg.Any<MessageDto>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await handler.Process(productEvent, TestContext.Current.CancellationToken);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: Pattern B Fix - Updated to validate Result<T> pattern
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        await notificationService.Received(1).SendAsync(Arg.Any<MessageDto>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes ProductCreatedHandler_Process_ShouldRespectCancellationToken operation.
    /// </summary>
    /// <returns>The result of ProductCreatedHandler_Process_ShouldRespectCancellationToken.</returns>

    [Fact]
    public async Task ProductCreatedHandler_Process_ShouldRespectCancellationToken()
    {
        // Arrange
        var notificationService = Substitute.For<INotificationService>();
        var handler = new ProductCreatedEvent.ProductCreatedHandler(notificationService);
        var productEvent = new ProductCreatedEvent();
        var cancellationToken = TestContext.Current.CancellationToken;

        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: Pattern B Fix - Updated mock setup for Railway-Oriented Programming Result<T> pattern
        notificationService.SendAsync(Arg.Any<MessageDto>(), Arg.Any<CancellationToken>())
                          .Returns(Result.Success());

        // Act
        var result = await handler.Process(productEvent, cancellationToken);

        // Assert - Should complete without throwing
        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: Pattern B Fix - Updated to validate Result<T> instead of Task completion
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        await notificationService.Received(1).SendAsync(Arg.Any<MessageDto>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Executes ProductCreatedHandler_Process_WithCancelledToken_ShouldHandleGracefully operation.
    /// </summary>
    /// <returns>The result of ProductCreatedHandler_Process_WithCancelledToken_ShouldHandleGracefully.</returns>

    [Fact]
    public async Task ProductCreatedHandler_Process_WithCancelledToken_ShouldHandleGracefully()
    {
        // Arrange
        var notificationService = Substitute.For<INotificationService>();
        var handler = new ProductCreatedEvent.ProductCreatedHandler(notificationService);
        var productEvent = new ProductCreatedEvent();

        using var cts = new CancellationTokenSource();
        cts.Cancel();

        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: Pattern B Fix - Handler checks for cancellation and returns failure result, no longer throws
        notificationService.SendAsync(Arg.Any<MessageDto>(), Arg.Any<CancellationToken>())
                          .Returns(Result.Success());

        // Act
        var result = await handler.Process(productEvent, cts.Token);

        // Assert
        //[Fix]
        //CLAUDE
        //Date: 23/08/2025
        //Reason: Pattern B Fix - Handler returns failure result for cancelled token instead of throwing
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldContain("Operation was canceled.");
    }
}
