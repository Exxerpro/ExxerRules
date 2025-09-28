using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for IntroduceParameterToolTests.
/// </summary>
public class IntroduceParameterToolTests : TestBase
{
    /// <summary>
    /// IntroduceParameter ValidExpression ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task IntroduceParameter_ValidExpression_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "IntroduceParameter.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForIntroduceVariable());

        var result = await IntroduceParameterTool.IntroduceParameter(
            SolutionPath,
            testFile,
            "FormatResult",
            "1:48-1:61",
            "processedValue");

        Assert.Contains("Successfully introduced parameter", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("processedValue", fileContent);
    }

    /// <summary>
    /// IntroduceParameter InvalidMethod ReturnsError.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task IntroduceParameter_InvalidMethod_ReturnsError()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var result = await IntroduceParameterTool.IntroduceParameter(
            SolutionPath,
            ExampleFilePath,
            "Nonexistent",
            "1:1-1:2",
            "param");
        Assert.Equal("Error: No method named 'Nonexistent' found", result);
    }
}
