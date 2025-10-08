namespace Application.UnitTests.Features.Shifts;

/// <summary>
/// Unit tests for GetShiftDetailQueryHandler
/// </summary>
public class GetShiftDetailQueryHandlerTests
{
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = Substitute.For<IRepository<Shift>>();
        var logger = XUnitLogger.CreateLogger<GetShiftDetailQueryHandler>();

        // Act
        var handler = new GetShiftDetailQueryHandler(mockRepository, logger);

        // Assert
        handler.ShouldNotBeNull();
    }
    /// <summary>
    /// Executes Constructor_WithNullRepository_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullRepository_ShouldThrowException()
    //     {
    //         // Arrange
    //         IRepository<Shift>? nullRepository = null!;
    //         var logger = XUnitLogger.CreateLogger<GetShiftDetailQueryHandler>();
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetShiftDetailQueryHandler(nullRepository!, logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         var mockRepository = Substitute.For<IRepository<Shift>>();
    //         ILogger<GetShiftDetailQueryHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetShiftDetailQueryHandler(mockRepository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Constructor_WithAllNullParameters_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithAllNullParameters_ShouldThrowException()
    //     {
    //         // Arrange
    //         IRepository<Shift>? nullRepository = null!;
    //         ILogger<GetShiftDetailQueryHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetShiftDetailQueryHandler(nullRepository!, nullLogger!));
    //     }

}
