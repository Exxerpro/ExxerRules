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
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForIntroduceVariable());

        var result = await IntroduceVariableTool.IntroduceVariable(
            SolutionPath,
            testFile,
            "42:50-42:63",
            "processedValue");

        Assert.Contains("Successfully introduced variable", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Contains("processedValue", fileContent);
    }
}