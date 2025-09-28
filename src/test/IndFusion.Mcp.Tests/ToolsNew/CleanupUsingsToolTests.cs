using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for removing unused using directives from source files.
/// </summary>
public class CleanupUsingsToolTests : TestBase
{
    /// <summary>
    /// Removes unused using directives and preserves required ones.
    /// </summary>
    [Fact]
    public async Task CleanupUsings_RemovesUnusedUsings()
    {
        const string initialCode = """
using System;
using System.Text;

public class CleanupSample
{
    public void Say() => Console.WriteLine("Hi");
}
""";

        const string expectedCode = """
using System;

public class CleanupSample
{
    public void Say() => Console.WriteLine("Hi");
}
""";

        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var testFile = Path.Combine(TestOutputPath, "CleanupSample.cs");
        await TestUtilities.CreateTestFile(testFile, initialCode);

        var result = await CleanupUsingsTool.CleanupUsings(SolutionPath, testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        Assert.Contains("Removed unused usings", result);
        var fileContent = await File.ReadAllTextAsync(testFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(expectedCode, fileContent.Replace("\r\n", "\n"));
    }
}
