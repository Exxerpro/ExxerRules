namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for MakeFieldReadonlyTests.
/// </summary>
public class MakeFieldReadonlyTests : TestBase
{
    /// <summary>
    /// MakeFieldReadonly FieldWithInitializer ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MakeFieldReadonly_FieldWithInitializer_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MakeFieldReadonlyTest.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForMakeFieldReadonly());

        var result = await MakeFieldReadonlyTool.MakeFieldReadonly(
            SolutionPath,
            testFile,
            "format",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully made field 'format' readonly", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("readonly string format", fileContent);
    }

    /// <summary>
    /// MakeFieldReadonly FieldWithoutInitializer ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MakeFieldReadonly_FieldWithoutInitializer_ReturnsSuccess()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MakeFieldReadonlyNoInitTest.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForMakeFieldReadonlyNoInit());

        var result = await MakeFieldReadonlyTool.MakeFieldReadonly(
            SolutionPath,
            testFile,
            "description",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully made field 'description' readonly", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("readonly string description", fileContent);
    }

    /// <summary>
    /// MakeFieldReadonly InvalidLine ReturnsError.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MakeFieldReadonly_InvalidLine_ReturnsError()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<McpException>(async () =>
            await MakeFieldReadonlyTool.MakeFieldReadonly(
                SolutionPath,
                ExampleFilePath,
                "nonexistent",
                cancellationToken: Xunit.TestContext.Current.CancellationToken));
    }
}
