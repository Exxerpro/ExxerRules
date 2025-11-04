using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests;

/// <summary>
/// Type MetricsResourceTests : TestBase
/// </summary>
/// <summary>
/// Type MetricsResourceTests : TestBase.
/// </summary>
/// <summary>
/// Type MetricsResourceTests : TestBase.
/// </summary>
/// <summary>
/// Type MetricsResourceTests : TestBase.
/// </summary>
public class MetricsResourceTests : TestBase
{
    /// <summary>
    /// ReadMetrics File ReturnsJson.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 60 second timeout for unit test (doubled from 30s)
    public async Task ReadMetrics_File_ReturnsJson()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var solutionDir = Path.GetDirectoryName(SolutionPath)!;
        var testFilePath = Path.Combine(solutionDir, "TestProject", "TestClass.cs");
        Console.WriteLine($"Test calling MetricsResource.ReadMetrics with testFilePath: {testFilePath}, SolutionPath: {SolutionPath}");
        var result = await MetricsResource.ReadMetrics(testFilePath, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Console.WriteLine($"File test result: {result.Text}");
        using var doc = JsonDocument.Parse(result.Text);
        Assert.True(doc.RootElement.TryGetProperty("linesOfCode", out _));
    }

    /// <summary>
    /// ReadMetrics Directory ReturnsAggregatedJson.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 60 second timeout for unit test (doubled from 30s)
    public async Task ReadMetrics_Directory_ReturnsAggregatedJson()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var solutionDir = Path.GetDirectoryName(SolutionPath)!;
        var testProjectDir = Path.Combine(solutionDir, "TestProject");
        var result = await MetricsResource.ReadMetrics(testProjectDir, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        using var doc = JsonDocument.Parse(result.Text);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.Contains(doc.RootElement.EnumerateArray(), e =>
            e.TryGetProperty("name", out var n) && n.GetString() == "TestClass");
    }

    /// <summary>
    /// ReadMetrics Class ReturnsClassMetrics.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 60 second timeout for unit test (doubled from 30s)
    public async Task ReadMetrics_Class_ReturnsClassMetrics()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var solutionDir = Path.GetDirectoryName(SolutionPath)!;
        var testClassPath = Path.Combine(solutionDir, "TestProject", "TestClass.cs");
        var classPath = testClassPath + "/" + "TestClass";
        var result = await MetricsResource.ReadMetrics(classPath, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Console.WriteLine($"Class lookup result: {result.Text}");
        using var doc = JsonDocument.Parse(result.Text);
        Assert.Equal("TestClass", doc.RootElement.GetProperty("name").GetString());
        Assert.True(doc.RootElement.TryGetProperty("methods", out _));
    }

    /// <summary>
    /// ReadMetrics Method ReturnsMethodMetrics.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 60 second timeout for unit test (doubled from 30s)
    public async Task ReadMetrics_Method_ReturnsMethodMetrics()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var solutionDir = Path.GetDirectoryName(SolutionPath)!;
        var testClassPath = Path.Combine(solutionDir, "TestProject", "TestClass.cs");
        var methodPath = testClassPath + "/" + "TestClass.ProcessValue";
        var result = await MetricsResource.ReadMetrics(methodPath, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Console.WriteLine($"Method lookup result: {result.Text}");
        using var doc = JsonDocument.Parse(result.Text);
        Assert.Equal("ProcessValue", doc.RootElement.GetProperty("name").GetString());
        Assert.True(doc.RootElement.TryGetProperty("cyclomaticComplexity", out _));
    }

    /// <summary>
    /// ReadMetrics InvalidPath ReturnsError.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 60 second timeout for unit test (doubled from 30s)
    public async Task ReadMetrics_InvalidPath_ReturnsError()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var invalidPath = ExampleFilePath + Path.DirectorySeparatorChar + "Unknown";
        var result = await MetricsResource.ReadMetrics(invalidPath, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        using var doc = JsonDocument.Parse(result.Text);
        Assert.True(doc.RootElement.TryGetProperty("Error", out var err));
        Assert.Contains("not found", err.GetString());
    }
}
