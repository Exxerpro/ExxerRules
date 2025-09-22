using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests;

public class SummaryResourceTests : TestBase
{
    [Fact]
    public async Task GetSummary_OmitsMethodBodies()
    {
        var result = await SummaryResources.GetSummary(ExampleFilePath, Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("public int Calculate(int a, int b)\n        {}", result);
        Assert.DoesNotContain("throw new ArgumentException", result);
    }

    [Fact]
    public async Task GetSummary_FileNotFound_ReturnsMessage()
    {
        var result = await SummaryResources.GetSummary("does_not_exist.cs", Xunit.TestContext.Current.CancellationToken);
        Assert.StartsWith("// File not found:", result);
    }
}

