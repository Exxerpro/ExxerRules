using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for converting methods to static and moving them to target types.
/// </summary>
public class MakeStaticThenMoveToolTests : TestBase
{
    /// <summary>
    /// Converts an instance method to static and moves it to a target type.
    /// </summary>
    [Fact]
    public async Task MakeStaticThenMove_ReturnsSuccess()
    {
        const string initialCode = "public class SourceClass { public string Value = \"x\"; public string GetValueWithSuffix(string suffix){ return Value + suffix; } } public class NewMathUtils { }";

        UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MakeStaticThenMove.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await MakeStaticThenMoveTool.MakeStaticThenMove(
            SolutionPath,
            testFile,
            "GetValueWithSuffix",
            "NewMathUtils",
            "source",
            null,
            null,
            Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved static method", result);
        var newFile = Path.Combine(Path.GetDirectoryName(testFile)!, "NewMathUtils.cs");
        Assert.True(File.Exists(newFile));
    }
}
