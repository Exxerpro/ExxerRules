using Xunit;

namespace IndFusion.Mcp.Tests;

/// <summary>
/// Basic tests to verify test runner functionality.
/// </summary>
public class SimpleTests
{
    /// <summary>
    /// Basic test that should always pass.
    /// </summary>
    [Fact]
    public void BasicTest_ShouldPass()
    {
        Assert.True(true);
    }

    /// <summary>
    /// Test simple arithmetic.
    /// </summary>
    [Fact]
    public void Math_Addition_ShouldWork()
    {
        var result = 2 + 2;
        Assert.Equal(4, result);
    }
}
