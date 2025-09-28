using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests;

/// <summary>
/// Type MetricsProviderTests : TestBase
/// </summary>
///<summary>
///Type MetricsProviderTests : TestBase.
///</summary>
///<summary>
///Type MetricsProviderTests : TestBase.
///</summary>
///<summary>
///Type MetricsProviderTests : TestBase.
///</summary>
public class MetricsProviderTests : TestBase
{
    /// <summary>
    /// GetFileMetrics CachesToDiskAndMemory.
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "Flaky in CI")]
    public async Task GetFileMetrics_CachesToDiskAndMemory()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // First call computes metrics and writes them to disk
        var first = await MetricsProvider.GetFileMetrics(SolutionPath, ExampleFilePath);
        using var json = JsonDocument.Parse(first);
        Assert.True(json.RootElement.TryGetProperty("linesOfCode", out _));

        var solutionDir = Path.GetDirectoryName(SolutionPath)!;
        var relative = Path.GetRelativePath(solutionDir, ExampleFilePath);
        var metricsPath = Path.Combine(solutionDir, ".ExxerFactor-Mcp", "metrics", relative);
        var metricsFile = Path.ChangeExtension(metricsPath, ".json");

        Assert.True(File.Exists(metricsFile));
        var diskFirst = await File.ReadAllTextAsync(metricsFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(first, diskFirst);

        // Modify the metrics file on disk
        const string modified = "modified";
        await File.WriteAllTextAsync(metricsFile, modified, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Second call should return cached result, not the modified file
        var second = await MetricsProvider.GetFileMetrics(SolutionPath, ExampleFilePath);
        Assert.Equal(first, second);

        var diskSecond = await File.ReadAllTextAsync(metricsFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(modified, diskSecond);
    }
}
