namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for removing unused using directives from files.
/// </summary>
public class CleanupUsingsTests : TestBase
{
    /// <summary>
    /// Removes unused using directives and updates the source file.
    /// </summary>
    [Fact]
    public async Task CleanupUsings_RemovesUnusedUsings()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "CleanupSample.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForCleanupUsings());

        var result = await CleanupUsingsTool.CleanupUsings(SolutionPath, testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Removed unused usings", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.DoesNotContain("System.Text", fileContent);
    }
}
