namespace IndFusion.Mcp.Tests.Tools;

///<summary>
///Type IntroduceVariableTests : TestBase.
///</summary>
public class IntroduceVariableTests : TestBase
{
    /// <summary>
    /// IntroduceVariable ValidExpression ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task IntroduceVariable_ValidExpression_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "IntroduceVariableTest.cs");
        var sample = TestUtilities.GetSampleCodeForIntroduceVariable();
        await TestUtilities.CreateTestFile(testFile, sample);

        var selection = TestUtilities.GetSelectionRange(sample, "value * 2 + 10");

        var result = await IntroduceVariableTool.IntroduceVariable(
            SolutionPath,
            testFile,
            selection,
            "processedValue",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully introduced variable", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("processedValue", fileContent);
    }
}
