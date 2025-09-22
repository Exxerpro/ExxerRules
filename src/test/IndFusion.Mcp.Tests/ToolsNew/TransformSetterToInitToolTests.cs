using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for transforming auto-properties from setter to init-only.
/// </summary>
public class TransformSetterToInitToolTests : TestBase
{
    /// <summary>
    /// Converts a property setter to init and updates source accordingly.
    /// </summary>
    [Fact]
    public async Task TransformSetter_ConvertsToInit()
    {
        const string initialCode = """
public class Sample
{
    public string Name { get; set; } = "";
}
""";

        const string expectedCode = """
public class Sample
{
    public string Name { get; init; } = "";
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "SetterToInit.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await TransformSetterToInitTool.TransformSetterToInit(
            SolutionPath,
            testFile,
            "Name");

        Assert.Contains("Successfully converted setter", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }

    /// <summary>
    /// Throws when the specified property cannot be found.
    /// </summary>
    [Fact]
    public async Task TransformSetter_InvalidProperty_ReturnsError()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<McpException>(async () =>
            await TransformSetterToInitTool.TransformSetterToInit(
                SolutionPath,
                ExampleFilePath,
                "Nonexistent"));
    }
}


