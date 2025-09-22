using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for converting methods to static while preserving instance access via fields.
/// </summary>
public class ConvertToStaticWithInstanceToolTests : TestBase
{
    /// <summary>
    /// ConvertToStaticWithInstance ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ConvertToStaticWithInstance_ReturnsSuccess()
    {
        const string initialCode = "public class Calculator { public string GetFormattedNumber(int n){ return $\"{n}\"; } }";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "ConvertToStaticInstance.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

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
