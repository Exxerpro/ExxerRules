using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for moving multiple methods and injecting dependencies via constructor.
/// </summary>
public class MoveMultipleMethodsConstructorInjectionToolTests : TestBase
{
    /// <summary>
    /// MoveMultipleMethods ConstructorInjection UsesThis.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveMultipleMethods_ConstructorInjection_UsesThis()
    {
        UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MultiCtor.cs");
        var code = "public class cA{ public int Value=>1; public int Get(){ return Value; } public int Add(int x){ return x + Value; } } public class B{ }";
        await TestUtilities.CreateTestFile(testFile, code);
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        var result = await MoveMultipleMethodsTool.MoveMultipleMethodsInstance(
            SolutionPath,
            testFile,
            "cA",
            new[] { "Get", "Add" },
            "B",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved", result);
        var targetPath = Path.Combine(Path.GetDirectoryName(testFile)!, "B.cs");
        var content = await File.ReadAllTextAsync(targetPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.DoesNotContain("_a", content);
    }

    /// <summary>
    /// MoveMultipleMethods ParameterInjection AddsParameter.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveMultipleMethods_ParameterInjection_AddsParameter()
    {
        UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "MultiParam.cs");
        var code = "public class cA{ public int Value=>1; public int Get(){ return Value; } public int Add(int x){ return x + Value; } } public class B{ }";
        await TestUtilities.CreateTestFile(testFile, code);
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var project = solution.Projects.First();
        ExxerFactoringHelpers.AddDocumentToProject(project, testFile);

        var result = await MoveMultipleMethodsTool.MoveMultipleMethodsStatic(
            SolutionPath,
            testFile,
            "cA",
            new[] { "Get", "Add" },
            "B",
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved", result);
        var targetPath = Path.Combine(Path.GetDirectoryName(testFile)!, "B.cs");
        var content = await File.ReadAllTextAsync(targetPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("Get(cA", content);
        Assert.Contains("Add(cA", content);
    }
}
