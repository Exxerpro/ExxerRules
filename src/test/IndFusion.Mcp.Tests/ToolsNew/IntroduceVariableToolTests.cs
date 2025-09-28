using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for introducing local variables from inline expressions.
/// </summary>
public class IntroduceVariableToolTests : TestBase
{
    /// <summary> ///  IntroduceVariable CreatesLocalVariable. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task IntroduceVariable_CreatesLocalVariable()
    {
        const string initialCode = """
public class Sample
{
    public string FormatResult(int value)
    {
        return $"Result: {value * 2 + 10}";
    }
}
""";

        const string expectedCode = """
public class Sample
{
    public string FormatResult(int value)
    {
        string processedValue = $"Result: {value * 2 + 10}";
        return processedValue;
    }
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "IntroduceVariable.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await IntroduceVariableTool.IntroduceVariable(
            SolutionPath,
            testFile,
            "4:24-4:37",
            "processedValue");

        Assert.Contains("Successfully introduced variable", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }
}
