namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for RenameSymbolTests.
/// </summary>
public class RenameSymbolTests : TestBase
{
    /// <summary>
    /// RenameSymbol Field RenamesAllReferences.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RenameSymbol_Field_RenamesAllReferences()
    {
        UnloadSolutionTool.ClearSolutionCache();
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "RenameSymbol.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForRenameSymbol());
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        var result = await RenameSymbolTool.RenameSymbol(
            SolutionPath,
            testFile,
            "numbers",
            "values");

        Assert.Contains("Successfully renamed", result);
        var content = await File.ReadAllTextAsync(testFile);
        Assert.DoesNotContain("List<int> numbers", content);
        Assert.DoesNotContain("numbers.Add", content);
        Assert.Contains("List<int> values", content);
        Assert.Contains("values.Add", content);
    }

    /// <summary>
    /// RenameSymbol InvalidName ReturnsError.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task RenameSymbol_InvalidName_ReturnsError()
    {
        UnloadSolutionTool.ClearSolutionCache();
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "RenameInvalid.cs");
        await TestUtilities.CreateTestFile(testFile, TestUtilities.GetSampleCodeForRenameSymbol());
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        await Assert.ThrowsAsync<McpException>(() =>
            RenameSymbolTool.RenameSymbol(
                SolutionPath,
                testFile,
                "missing",
                "newName"));
    }
}
