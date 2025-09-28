namespace IndFusion.Mcp.Tests.Tools;

/// <summary> /// Represents the  MoveMultipleMethodsToolTests  class. /// </summary>
public class MoveMultipleMethodsToolTests : TestBase
{
    /// <summary> ///  MoveMultipleMethods FailureDoesNotRecordHistory. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task MoveMultipleMethods_FailureDoesNotRecordHistory()
    {
        UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MoveMultiFailHistory.cs");
        await TestUtilities.CreateTestFile(testFile, File.ReadAllText(ExampleFilePath));
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        var error = await MoveMultipleMethodsTool.MoveMultipleMethodsStatic(
            SolutionPath,
            testFile,
            "Calculator",
            new[] { "FormatCurrency", "Wrong" },
            "MathUtilities",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("Error:", error);

        var result = await MoveMultipleMethodsTool.MoveMultipleMethodsStatic(
            SolutionPath,
            testFile,
            "Calculator",
            new[] { "FormatCurrency", "LogOperation" },
            "MathUtilities",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved", result);
    }

    /// <summary> ///  MoveMultipleMethods NestedClassGenerics ShouldSucceed. /// </summary> /// <returns>A task that represents the asynchronous operation.</returns>
    [Fact]
    public async Task MoveMultipleMethods_NestedClassGenerics_ShouldSucceed()
    {
        UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "NestedGeneric.cs");
        var code = @"using System.Collections.Generic;
public class Outer
{
    public class Inner { }
    public List<Inner> MakeList() => new List<Inner>();
    public int CountList(List<Inner> items) => items.Count;
}
public class Target { }";
        await TestUtilities.CreateTestFile(testFile, code);
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        var result = await MoveMultipleMethodsTool.MoveMultipleMethodsStatic(
            SolutionPath,
            testFile,
            "Outer",
            new[] { "MakeList", "CountList" },
            "Target",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved", result);
    }
}
