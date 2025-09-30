namespace IndFusion.Mcp.Tests.Tools;

/// <summary> /// Represents the  MakeStaticThenMoveTests  class. /// </summary>
public class MakeStaticThenMoveTests : TestBase
{
    /// <summary> ///  MakeStaticThenMove ReturnsSuccess. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task MakeStaticThenMove_ReturnsSuccess()
    {
        UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MakeStaticThenMove1.cs");
        await TestUtilities.CreateTestFile(testFile, @"public class SourceClass
{
    public string Value = ""x"";
    public string GetValueWithSuffix(string suffix)
    {
        return Value + suffix;
    }
}

public class NewMathUtils1 { }");

        var result = await MakeStaticThenMoveTool.MakeStaticThenMove(
            SolutionPath,
            testFile,
            "GetValueWithSuffix",
            "NewMathUtils1",
            "source",
            null,
            null,
            Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved static method", result);
        var newFile = Path.Combine(Path.GetDirectoryName(testFile)!, "NewMathUtils1.cs");
        Assert.True(File.Exists(newFile));
    }
}
