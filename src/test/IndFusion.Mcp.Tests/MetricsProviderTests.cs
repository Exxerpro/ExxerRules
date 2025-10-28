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
    [Fact(Timeout = 30000)] // 30 second timeout for unit test
    public async Task GetFileMetrics_CachesToDiskAndMemory()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Use a file that's actually part of the test solution
        var solutionDir = Path.GetDirectoryName(SolutionPath)!;
        var testFilePath = Path.Combine(solutionDir, "TestProject", "TestClass.cs");

        // Debug: Check what documents are in the solution
        var solution = await ExxerFactoringHelpers.GetOrLoadSolution(SolutionPath, Xunit.TestContext.Current.CancellationToken);
        Console.WriteLine($"Solution has {solution.Projects.Count()} projects");
        foreach (var project in solution.Projects)
        {
            Console.WriteLine($"Project: {project.Name}");
            foreach (var doc in project.Documents)
            {
                Console.WriteLine($"  Document: {doc.FilePath}");
            }
        }
        Console.WriteLine($"Looking for file: {testFilePath}");

        // First call computes metrics and writes them to disk
        Console.WriteLine($"About to call MetricsProvider.GetFileMetrics with solutionPath: {SolutionPath}");
        Console.WriteLine($"About to call MetricsProvider.GetFileMetrics with filePath: {testFilePath}");
        string first;
        try
        {
            first = await MetricsProvider.GetFileMetrics(SolutionPath, testFilePath, Xunit.TestContext.Current.CancellationToken);
            Console.WriteLine($"MetricsProvider.GetFileMetrics returned: {first.Substring(0, Math.Min(100, first.Length))}...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MetricsProvider.GetFileMetrics threw exception: {ex.Message}");
            throw new InvalidOperationException($"Failed to get metrics for {testFilePath}: {ex.Message}", ex);
        }
        using var json = JsonDocument.Parse(first);
        Assert.True(json.RootElement.TryGetProperty("linesOfCode", out _));

        var relative = Path.GetRelativePath(solutionDir, testFilePath);
        var metricsPath = Path.Combine(solutionDir, ".ExxerFactor-Mcp", "metrics", relative);
        var metricsFile = Path.ChangeExtension(metricsPath, ".json");

        Assert.True(File.Exists(metricsFile));
        var diskFirst = await File.ReadAllTextAsync(metricsFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(first, diskFirst);

        // Modify the metrics file on disk
        const string modified = "modified";
        await File.WriteAllTextAsync(metricsFile, modified, cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Second call should return cached result, not the modified file
        var second = await MetricsProvider.GetFileMetrics(SolutionPath, testFilePath, Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(first, second);

        var diskSecond = await File.ReadAllTextAsync(metricsFile, cancellationToken: Xunit.TestContext.Current.CancellationToken);
        Assert.Equal(modified, diskSecond);
    }
}
