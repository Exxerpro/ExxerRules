namespace Application.UnitTests.Features.Products;

/// <summary>
/// Unit tests for ProductService
/// </summary>
public class ProductServiceTests
{
    // MARKED FOR REMOVAL - Constructor null guard test no longer needed with Result<T> patterns
    // /// <summary>
    // /// Executes Constructor_ShouldCreateInstance operation.
    // /// </summary>
    // [Fact]
    // public void Constructor_ShouldCreateInstance()
    // {
    //     // Arrange
    //     var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
    //     var logger =  XUnitLogger.CreateLogger<ProductService>();

    //     // Act
    //     var service = new ProductService(guiCommandDispatcher, logger);

    //     // Assert
    //     service.ShouldNotBeNull();
    // }
    // /// <summary>
    // /// Executes Constructor_WithNullRepository_ShouldThrowException operation.
    // /// </summary>

    // [Fact]
    // public void Constructor_WithNullRepository_ShouldThrowException()
    // {
    //     // Arrange & Act & Assert
    //     Should.Throw<ArgumentNullException>(() => new ProductService(null!, XUnitLogger.CreateLogger<ProductService>>()));
    // }
    /// <summary>
    /// Executes Constructor_WithValidRepository_ShouldNotThrowException operation.
    /// </summary>

    [Fact]
    public void Constructor_WithValidRepository_ShouldNotThrowException()
    {
        // Arrange
        var guiCommandDispatcher = Substitute.For<IMonitorRequestDispatcher>();
        var logger = XUnitLogger.CreateLogger<ProductService>();

        // Act & Assert
        Should.NotThrow(() => new ProductService(guiCommandDispatcher, logger));
    }
}
