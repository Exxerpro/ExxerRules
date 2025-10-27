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
    [Fact(Timeout = 5000)] // 5 second timeout - will fail and start TDD
    public async Task ReadMetrics_File_ReturnsJson()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var result = await MetricsResource.ReadMetrics(ExampleFilePath, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        using var doc = JsonDocument.Parse(result.Text);
        Assert.True(doc.RootElement.TryGetProperty("linesOfCode", out _));
    }

    /// <summary>
    /// ReadMetrics Directory ReturnsAggregatedJson.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 5000)] // 5 second timeout - will fail and start TDD
    public async Task ReadMetrics_Directory_ReturnsAggregatedJson()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var dir = Path.GetDirectoryName(ExampleFilePath)!;
        var result = await MetricsResource.ReadMetrics(dir, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        using var doc = JsonDocument.Parse(result.Text);
        Assert.Equal(JsonValueKind.Array, doc.RootElement.ValueKind);
        Assert.Contains(doc.RootElement.EnumerateArray(), e =>
            e.TryGetProperty("name", out var n) && n.GetString() == "Calculator");
    }

    /// <summary>
    /// ReadMetrics Class ReturnsClassMetrics.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 5000)] // 5 second timeout - will fail and start TDD
    public async Task ReadMetrics_Class_ReturnsClassMetrics()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var classPath = ExampleFilePath + Path.DirectorySeparatorChar + "Calculator";
        var result = await MetricsResource.ReadMetrics(classPath, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        using var doc = JsonDocument.Parse(result.Text);
        Assert.Equal("Calculator", doc.RootElement.GetProperty("name").GetString());
        Assert.True(doc.RootElement.TryGetProperty("methods", out _));
    }

    /// <summary>
    /// ReadMetrics Method ReturnsMethodMetrics.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 5000)] // 5 second timeout - will fail and start TDD
    public async Task ReadMetrics_Method_ReturnsMethodMetrics()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        var methodPath = ExampleFilePath + Path.DirectorySeparatorChar + "Calculator.Calculate";
        var result = await MetricsResource.ReadMetrics(methodPath, SolutionPath, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        using var doc = JsonDocument.Parse(result.Text);
        Assert.Equal("Calculate", doc.RootElement.GetProperty("name").GetString());
        Assert.True(doc.RootElement.TryGetProperty("cyclomaticComplexity", out _));
    }

    /// <summary>
    /// ReadMetrics InvalidPath ReturnsError.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 5000)] // 5 second timeout - will fail and start TDD
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
