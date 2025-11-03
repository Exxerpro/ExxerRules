namespace IndFusion.Mcp.Tests.Tools;

/// <summary> /// Represents the  MoveMultipleMethodsDuplicateTests  class. /// </summary>
public class MoveMultipleMethodsDuplicateTests : TestBase
{
    /// <summary> ///  MoveMultipleMethods DuplicateNames Fails. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task MoveMultipleMethods_DuplicateNames_Fails()
    {
        UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "DupMethods.cs");
        var code = @"public class Source { public void A() { } } public class Target { }";
        await TestUtilities.CreateTestFile(testFile, code);
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        var result = await MoveMultipleMethodsTool.MoveMultipleMethodsStatic(
            SolutionPath,
            testFile,
            "Source",
            new[] { "A", "A" },
            "Target",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Duplicate method names", result);
    }
}
