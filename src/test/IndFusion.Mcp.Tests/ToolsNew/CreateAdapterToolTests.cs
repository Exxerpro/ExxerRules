using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for generating adapters to bridge incompatible interfaces or classes.
/// </summary>
public class CreateAdapterToolTests : TestBase
{
    /// <summary>
    /// CreateAdapter AddsClass.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAdapter_AddsClass()
    {
        const string initialCode = "public class LegacyLogger { public void Write(string message){ System.Console.WriteLine(message); } }";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Adapter.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await CreateAdapterTool.CreateAdapter(
            SolutionPath,
            testFile,
            "LegacyLogger",
            "Write",
            "LoggerAdapter");

        Assert.Contains("Created adapter", result);
        var text = await File.ReadAllTextAsync(testFile);
        Assert.Contains("LoggerAdapter", text);
    }
}
