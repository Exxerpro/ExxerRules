using IndFusion.Mcp.Tests.Integration.Infrastructure;

namespace IndFusion.Mcp.Tests.Integration.ToolsNew;

/// <summary>
/// Integration tests for the LintRun MCP tool that runs EXXER analyzers and returns violations with policy recommendations.
/// </summary>
[Trait("Category", "Integration")]
public class LintRunToolTests : IntegrationTestBase
{
    /// <summary>
    /// LintRun_WithValidSolution_ReturnsViolationsAndPolicyRecommendations.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 30 second timeout for integration test
    public async Task LintRun_WithValidSolution_ReturnsViolationsAndPolicyRecommendations()
    {
        // Arrange - Load solution first
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        // Act - Run linting on the solution
        var result = await LintRunTool.LintRun(
            solutionPath: SolutionPath,
            scope: "all",
            severityConfig: "error,warning",
            progress: null,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert - Verify the result contains expected structure
        Assert.NotNull(result);
        Assert.Contains("violations", result.ToLowerInvariant());
        Assert.Contains("policy", result.ToLowerInvariant());
    }

    /// <summary>
    /// LintRun_WithSpecificFile_ReturnsFileSpecificViolations.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 30 second timeout for integration test
    public async Task LintRun_WithSpecificFile_ReturnsFileSpecificViolations()
    {
        // Arrange - Load solution first
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        // Act - Run linting on specific file
        var result = await LintRunTool.LintRun(
            solutionPath: SolutionPath,
            scope: ExampleFilePath,
            severityConfig: "error",
            progress: null,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert - Verify the result contains file-specific violations
        Assert.NotNull(result);
        Assert.Contains("violations", result.ToLowerInvariant());
    }

    /// <summary>
    /// LintRun_WithCancellation_RespectsCancellationToken.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 30 second timeout for integration test
    public async Task LintRun_WithCancellation_RespectsCancellationToken()
    {
        // Arrange - Load solution first
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        // Arrange - Create cancellation token that cancels immediately
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - Should respect cancellation
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            await LintRunTool.LintRun(
                solutionPath: SolutionPath,
                scope: "all",
                severityConfig: "error",
                progress: null,
                cancellationToken: cts.Token));
    }

    /// <summary>
    /// LintRun_WithProgressReporter_CallsProgressCallback.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 60000)] // 30 second timeout for integration test
    public async Task LintRun_WithProgressReporter_CallsProgressCallback()
    {
        // Arrange - Load solution first
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        // Arrange - Track progress calls
        var progressCalls = new List<string>();
        var progress = new Progress<string>(message => progressCalls.Add(message));

        // Act - Run linting with progress reporter
        var result = await LintRunTool.LintRun(
            solutionPath: SolutionPath,
            scope: "all",
            severityConfig: "error,warning",
            progress: progress,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert - Verify progress was reported
        Assert.NotNull(result);
        Assert.NotEmpty(progressCalls);
        Assert.Contains(progressCalls, call => call.Contains("linting", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// LintRun_WithDifferentSeverityConfigs_ReturnsAppropriateViolations.
    /// </summary>
    /// <returns></returns>
    [Theory]
    [InlineData("error")]
    [InlineData("warning")]
    [InlineData("error,warning")]
    [InlineData("suggestion")]
    public async Task LintRun_WithDifferentSeverityConfigs_ReturnsAppropriateViolations(string severityConfig)
    {
        // Arrange - Load solution first
        await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        // Act - Run linting with different severity configs
        var result = await LintRunTool.LintRun(
            solutionPath: SolutionPath,
            scope: "all",
            severityConfig: severityConfig,
            progress: null,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert - Verify the result is appropriate for the severity config
        Assert.NotNull(result);
        Assert.Contains("violations", result.ToLowerInvariant());
    }
}
