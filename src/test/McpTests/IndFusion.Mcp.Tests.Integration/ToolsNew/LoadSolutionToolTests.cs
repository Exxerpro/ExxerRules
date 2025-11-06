using IndFusion.Mcp.Tests.Integration.Infrastructure;

namespace IndFusion.Mcp.Tests.Integration.ToolsNew;

/// <summary>
/// Tests for loading and unloading Roslyn solutions used by tools.
/// </summary>
[Trait("Category", "Integration")]
public class LoadSolutionToolTests : IntegrationTestBase
{
    /// <summary>
    /// LoadSolution ValidPath ReturnsSuccess.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 120000)] // 120 second timeout for integration test (doubled from 60s)
    public async Task LoadSolution_ValidPath_ReturnsSuccess()
    {
        var result = await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("Successfully loaded solution", result);
        Assert.Contains("CleanArchitectureTemplate.API", result);
    }

    /// <summary>
    /// UnloadSolution RemovesCachedSolution.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 120000)] // 120 second timeout for integration test (doubled from 60s)
    public async Task UnloadSolution_RemovesCachedSolution()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var result = UnloadSolutionTool.UnloadSolution(SolutionPath, Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("Unloaded solution", result);
    }

    /// <summary>
    /// LoadSolution InvalidPath ReturnsError.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 120000)] // 120 second timeout for integration test (doubled from 60s)
    public async Task LoadSolution_InvalidPath_ReturnsError()
    {
        await Assert.ThrowsAsync<McpException>(async () =>
            await LoadSolutionTool.LoadSolution("./NonExistent.sln", null, Xunit.TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Version ReturnsInfo.
    /// </summary>
    [Fact]
    public void Version_ReturnsInfo()
    {
        var result = VersionTool.Version();
        Assert.Contains("Version:", result);
        Assert.Contains("Build", result);
    }

    /// <summary>
    /// ClearSolutionCache RemovesAllCachedSolutions.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 120000)] // 120 second timeout for integration test (doubled from 60s)
    public async Task ClearSolutionCache_RemovesAllCachedSolutions()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var clearResult = UnloadSolutionTool.ClearSolutionCache(Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("Cleared all cached solutions", clearResult);

        var unloadResult = UnloadSolutionTool.UnloadSolution(SolutionPath, Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("was not loaded", unloadResult);
    }
}
