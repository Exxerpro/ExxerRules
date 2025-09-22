using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for converting instance methods to extension methods.
/// </summary>
public class ConvertToExtensionMethodToolTests : TestBase
{
    /// <summary>
    /// Converts a target method to an extension method and reports success.
    /// </summary>
    [Fact]
    public async Task ConvertToExtensionMethod_ReturnsSuccess()
    {
        const string initialCode = "public class Calculator { public string GetFormattedNumber(int n){ return $\"{n}\"; } }";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "ConvertToExtension.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await ConvertToExtensionMethodTool.ConvertToExtensionMethod(
            SolutionPath,
            testFile,
            "GetFormattedNumber",
            null);

        Assert.Contains("Successfully converted method 'GetFormattedNumber' to extension method", result);
    }
}
