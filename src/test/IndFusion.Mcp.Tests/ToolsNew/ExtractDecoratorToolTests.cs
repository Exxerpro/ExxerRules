using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for extracting the decorator pattern from existing classes.
/// </summary>
public class ExtractDecoratorToolTests : TestBase
{
    /// <summary>
    /// ExtractDecorator AddsClass.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ExtractDecorator_AddsClass()
    {
        const string initialCode = "public class Greeter { public void Greet(string name){ System.Console.WriteLine(\"Hello {name}\"); } }";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Decorator.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await ExtractDecoratorTool.ExtractDecorator(
            SolutionPath,
            testFile,
            "Greeter",
            "Greet");

        Assert.Contains("Created decorator", result);
        var text = await File.ReadAllTextAsync(testFile);
        Assert.Contains("GreeterDecorator", text);
    }
}
