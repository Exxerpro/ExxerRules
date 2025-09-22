using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for moving methods across namespaces while preserving resolution.
/// </summary>
public class MoveMethodNamespaceToolTests : TestBase
{
    /// <summary>
    /// MoveInstanceMethod PreservesNamespaceInNewFile.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveInstanceMethod_PreservesNamespaceInNewFile()
    {
        UnloadSolutionTool.ClearSolutionCache();
        const string initialCode = "namespace Sample.Namespace { public class A { public void Foo() {} } }";
        var testFile = Path.Combine(TestOutputPath, "NamespaceSample.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        var targetFile = Path.Combine(Path.GetDirectoryName(testFile)!, "B.cs");
        var result = await MoveMethodTool.MoveInstanceMethod(
            SolutionPath,
            testFile,
            "A",
            new[] { "Foo" },
            "B",
            targetFile,
            Array.Empty<string>(),
            Array.Empty<string>(),
            null,
            Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved", result);
        var newContent = await File.ReadAllTextAsync(targetFile);
        Assert.Contains("namespace Sample.Namespace", newContent);
    }

    /// <summary>
    /// MoveInstanceMethod DoesNotAddNamespaceUsing.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task MoveInstanceMethod_DoesNotAddNamespaceUsing()
    {
        UnloadSolutionTool.ClearSolutionCache();
        const string initialCode = "namespace Sample.Namespace { public class A { public void Foo() {} } }";
        var testFile = Path.Combine(TestOutputPath, "NamespaceUsingSample.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        var targetFile = Path.Combine(Path.GetDirectoryName(testFile)!, "C.cs");
        var result = await MoveMethodTool.MoveInstanceMethod(
            SolutionPath,
            testFile,
            "A",
            new[] { "Foo" },
            "C",
            targetFile,
            Array.Empty<string>(),
            Array.Empty<string>(),
            null,
            Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Successfully moved", result);
        var newContent = await File.ReadAllTextAsync(targetFile);
        Assert.DoesNotContain("using Sample.Namespace;", newContent);
    }
}

