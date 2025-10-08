namespace Application.UnitTests.Uncategorized;

/// <summary>
/// Unit tests for CacheManager
/// </summary>
public class CacheManagerTests
{
    /// <summary>
    /// Executes Constructor_WithValidParameters_ShouldCreateInstance operation.
    /// </summary>
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act
        var instance = new CacheManager<ApplicationConfiguration>(TimeSpan.FromMinutes(60));
        // Assert
        instance.ShouldNotBeNull();
    }
}
