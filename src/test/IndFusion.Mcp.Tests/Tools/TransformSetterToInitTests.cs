namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for converting property setters to init-only.
/// </summary>
public class TransformSetterToInitTests : TestBase
{
    /// <summary>
    /// Converts a property setter to init in the source file.
    /// </summary>
    [Fact]
    public async Task TransformSetterToInit_PropertyWithSetter_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "TransformSetterToInit.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForTransformSetter());

        var result = await TransformSetterToInitTool.TransformSetterToInit(
            SolutionPath,
            testFile,
            "Name");

        Assert.Contains("Successfully converted setter to init", result);
        var fileContent = await File.ReadAllTextAsync(testFile);
        Assert.Contains("init;", fileContent);
    }

    /// <summary>
    /// Throws when the specified property cannot be found.
    /// </summary>
    [Fact]
    public async Task TransformSetterToInit_InvalidProperty_ReturnsError()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<McpException>(async () =>
            await TransformSetterToInitTool.TransformSetterToInit(
                SolutionPath,
                ExampleFilePath,
                "Nonexistent"));
    }
}
