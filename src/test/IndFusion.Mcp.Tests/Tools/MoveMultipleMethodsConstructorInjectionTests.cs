namespace IndFusion.Mcp.Tests.Tools;

/// <summary>
/// Tests for MoveMultipleMethodsConstructorInjectionTests.
/// </summary>
public class MoveMultipleMethodsConstructorInjectionTests : TestBase
{
    /// <summary>
    /// MoveMultipleMethods ConstructorInjection UsesThis.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveMultipleMethods_ConstructorInjection_UsesThis()
    {
        UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var project = solution.Projects.First();
        
        // Create test file in the TestProject directory (like other successful tests)
        var solutionDir = Path.GetDirectoryName(SolutionPath)!;
        var testFile = Path.Combine(solutionDir, "TestProject", "MultiCtor.cs");
        var code = "public class cA{ public int Value=>1; public int Get(){ return Value; } public int Add(int x){ return x + Value; } } public class B{ }";
        await TestUtilities.CreateTestFile(testFile, code);
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
        // Constructor injection should NOT generate fields - it should inject via parameter
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
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var project = solution.Projects.First();
        
        // Create test file in the TestProject directory (like other successful tests)
        var solutionDir = Path.GetDirectoryName(SolutionPath)!;
        var testFile = Path.Combine(solutionDir, "TestProject", "MultiParam.cs");
        var code = "public class cA{ public int Value=>1; public int Get(){ return Value; } public int Add(int x){ return x + Value; } } public class B{ }";
        await TestUtilities.CreateTestFile(testFile, code);
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
