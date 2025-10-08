using static IndTrace.Application.ConfigApplication.Commands.Create.ConfigAppCreated;

namespace Application.UnitTests.Features.ConfigApps;

/// <summary>
/// Unit tests for ConfigAppCreatedHandler
/// </summary>
public class ConfigAppCreatedHandlerTests
{
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var notificationService = Substitute.For<INotificationService>();
        // Act
        var instance = new ConfigAppCreatedHandler(notificationService);
        // Assert
        instance.ShouldNotBeNull();
    }
}
