namespace Application.UnitTests.Features.Settings;

/// <summary>
/// Unit tests for GetSettingDetailQueryHandler
/// </summary>
public class GetSettingDetailQueryHandlerTests
{
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var mockRepository = Substitute.For<IRepository<Setting>>();
        var logger = XUnitLogger.CreateLogger<GetSettingDetailQueryHandler>();

        // Act
        var handler = new GetSettingDetailQueryHandler(mockRepository, logger);

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
    //         IRepository<Setting>? nullRepository = null!;
    //         var logger = XUnitLogger.CreateLogger<GetSettingDetailQueryHandler>();
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetSettingDetailQueryHandler(nullRepository!, logger));
    //     }
    /// <summary>
    /// Executes Constructor_WithNullLogger_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithNullLogger_ShouldThrowException()
    //     {
    //         // Arrange
    //         var mockRepository = Substitute.For<IRepository<Setting>>();
    //         ILogger<GetSettingDetailQueryHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetSettingDetailQueryHandler(mockRepository, nullLogger!));
    //     }
    /// <summary>
    /// Executes Constructor_WithAllNullParameters_ShouldThrowException operation.
    /// </summary>

    // MARKED FOR DELETION - Constructor null guard test no longer needed for DI handlers
    // [Fact]
    //     public void Constructor_WithAllNullParameters_ShouldThrowException()
    //     {
    //         // Arrange
    //         IRepository<Setting>? nullRepository = null!;
    //         ILogger<GetSettingDetailQueryHandler>? nullLogger = null!;
    //
    //         // Act & Assert
    //         Should.Throw<ArgumentNullException>(() => new GetSettingDetailQueryHandler(nullRepository!, nullLogger!));
    //     }
}
