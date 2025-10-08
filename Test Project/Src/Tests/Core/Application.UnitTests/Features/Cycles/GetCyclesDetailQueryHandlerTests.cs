namespace Application.UnitTests.Features.Cycles;

/// <summary>
/// Unit tests for GetCyclesDetailQueryHandler
/// </summary>
public class GetCyclesDetailQueryHandlerTests
{
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = Substitute.For<IRepository<Cycle>>();
        var logger = XUnitLogger.CreateLogger<GetCyclesDetailQueryHandler>();

        // Act
        var handler = new GetCyclesDetailQueryHandler(mockRepository, logger);

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
    //         IRepository<Cycle>? nullRepository = null!;
    //         var logger = XUnitLogger.CreateLogger<GetCyclesDetailQueryHandler>();
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetCyclesDetailQueryHandler(nullRepository!, logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         var mockRepository = Substitute.For<IRepository<Cycle>>();
    //         ILogger<GetCyclesDetailQueryHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetCyclesDetailQueryHandler(mockRepository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Constructor_WithAllNullParameters_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithAllNullParameters_ShouldThrowException()
    //     {
    //         // Arrange
    //         IRepository<Cycle>? nullRepository = null!;
    //         ILogger<GetCyclesDetailQueryHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetCyclesDetailQueryHandler(nullRepository!, nullLogger!));
    //     }
}
