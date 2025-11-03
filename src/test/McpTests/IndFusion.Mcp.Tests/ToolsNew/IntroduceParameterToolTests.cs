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

        var sampleCode = TestUtilities.GetSampleCodeForIntroduceVariable();
        var selectionRange = TestUtilities.GetSelectionRange(sampleCode, "value * 2 + 10");

        var result = await IntroduceParameterTool.IntroduceParameter(
            SolutionPath,
            testFile,
            "FormatResult",
            selectionRange,
            "processedValue",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

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
            "param",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal("Error: No method named 'Nonexistent' found", result);
    }
}
