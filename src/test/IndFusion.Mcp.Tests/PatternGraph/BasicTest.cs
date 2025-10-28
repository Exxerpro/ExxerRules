using Xunit;

namespace IndFusion.Mcp.Tests.PatternGraph;

/// <summary>
/// Basic test to verify test infrastructure.
/// </summary>
public class BasicTest
{
    /// <summary>
    /// Test that basic assertions work.
    /// </summary>
    [Fact]
    public void BasicAssertion_ShouldWork()
    {
        // Arrange
        var value = 42;
        
        // Act & Assert
        Assert.Equal(42, value);
    }
}
