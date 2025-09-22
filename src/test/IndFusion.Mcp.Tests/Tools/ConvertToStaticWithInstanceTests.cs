namespace IndFusion.Mcp.Tests.Tools;

 ///<summary>
 ///Type ConvertToStaticWithInstanceTests : TestBase.
 ///</summary>
public class ConvertToStaticWithInstanceTests : TestBase
{
    /// <summary>
    /// ConvertToStaticWithInstance ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ConvertToStaticWithInstance_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "ConvertToStaticInstance.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForConvertToStaticInstance());

        var result = await ConvertToStaticWithInstanceTool.ConvertToStaticWithInstance(
            SolutionPath,
            testFile,
            "GetFormattedNumber",
            "instance");

        Assert.Contains("Successfully converted method 'GetFormattedNumber' to static with instance parameter", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Contains("static string GetFormattedNumber", fileContent);
        Assert.Contains("Calculator instance", fileContent);
    }
}
