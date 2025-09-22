namespace IndFusion.Mcp.Tests.Tools;

 ///<summary>
 ///Type CreateAdapterTests : TestBase.
 ///</summary>
public class CreateAdapterTests : TestBase
{
    /// <summary>
    /// CreateAdapter AddsClass.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task CreateAdapter_AddsClass()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "Adapter.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForAdapter());

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
