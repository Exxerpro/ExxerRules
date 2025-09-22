namespace IndFusion.Mcp.Tests.Tools;

public class LoadSolutionTests : TestBase
{
    [Fact]
    public async Task LoadSolution_ValidPath_ReturnsSuccess()
    {
        var result = await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        Assert.Contains("Successfully loaded solution", result);
        Assert.Contains("IndFusion.Mcp.Core", result);
        Assert.Contains("IndFusion.Mcp.Tests", result);
    }

    [Fact]
    public async Task UnloadSolution_RemovesCachedSolution()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var result = UnloadSolutionTool.UnloadSolution(SolutionPath);
        Assert.Contains("Unloaded solution", result);
    }

    [Fact]
    public async Task LoadSolution_InvalidPath_ReturnsError()
    {
        await Assert.ThrowsAsync<McpException>(async () =>
            await LoadSolutionTool.LoadSolution("./NonExistent.sln", null, Xunit.TestContext.Current.CancellationToken));
    }

    [Fact]
    public void Version_ReturnsInfo()
    {
        var result = VersionTool.Version();
        Assert.Contains("Version:", result);
        Assert.Contains("Build", result);
    }

    [Fact]
    public async Task ClearSolutionCache_RemovesAllCachedSolutions()
    {
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
        var clearResult = UnloadSolutionTool.ClearSolutionCache();
        Assert.Contains("Cleared all cached solutions", clearResult);

        var unloadResult = UnloadSolutionTool.UnloadSolution(SolutionPath);
        Assert.Contains("was not loaded", unloadResult);
    }
}

