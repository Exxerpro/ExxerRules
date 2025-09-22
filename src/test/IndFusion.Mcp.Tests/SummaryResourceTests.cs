using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests;

/// <summary>
/// Tests for code summarization utilities that omit method bodies.
/// </summary>
public class SummaryResourceTests : TestBase
{
    /// <summary>
    /// Verifies method bodies are omitted in the summary output.
    /// </summary>
    [Fact]
    public async Task GetSummary_OmitsMethodBodies()
    {
        var result = await SummaryResources.GetSummary(ExampleFilePath, Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("public int Calculate(int a, int b)\n        {}", result);
        Assert.DoesNotContain("throw new ArgumentException", result);
    }

    /// <summary>
    /// Returns a friendly message when the file does not exist.
    /// </summary>
    [Fact]
    public async Task GetSummary_FileNotFound_ReturnsMessage()
    {
        var result = await SummaryResources.GetSummary("does_not_exist.cs", Xunit.TestContext.Current.CancellationToken);
        Assert.StartsWith("// File not found:", result);
    }
}
