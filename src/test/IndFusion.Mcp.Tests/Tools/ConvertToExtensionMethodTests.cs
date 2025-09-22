namespace IndFusion.Mcp.Tests.Tools;

 ///<summary>
 ///Type ConvertToExtensionMethodTests : TestBase.
 ///</summary>
public class ConvertToExtensionMethodTests : TestBase
{
    /// <summary>
    /// ConvertToExtensionMethod ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ConvertToExtensionMethod_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "ConvertToExtension.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForConvertToExtension());

        var result = await ConvertToExtensionMethodTool.ConvertToExtensionMethod(
            SolutionPath,
            testFile,
            "GetFormattedNumber",
            null);

        Assert.Contains("Successfully converted method 'GetFormattedNumber' to extension method", result);
    }
}
