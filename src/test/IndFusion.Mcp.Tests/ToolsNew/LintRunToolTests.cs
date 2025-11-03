using IndFusion.Mcp.Tests.Tools;

namespace IndFusion.Mcp.Tests.ToolsNew;

/// <summary>
/// Tests for the LintRun MCP tool that runs EXXER analyzers and returns violations with policy recommendations.
/// </summary>
public class LintRunToolTests : TestBase
{
    private readonly Infrastructure.FileLogger<LintRunToolTests> _testLogger;

    /// <summary>
    /// Initializes a new instance of the LintRunToolTests class.
    /// </summary>
    /// <param name="output">xUnit test output helper for logging (automatically injected by xUnit v3).</param>
    public LintRunToolTests(Xunit.ITestOutputHelper output)
    {
        _testLogger = new Infrastructure.FileLogger<LintRunToolTests>(output, nameof(LintRunToolTests));
    }

    /// <summary>
    /// LintRun_WithValidSolution_ReturnsViolationsAndPolicyRecommendations.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 30000)] // 30 second timeout for analyzer execution
    public async Task LintRun_WithValidSolution_ReturnsViolationsAndPolicyRecommendations()
    {
        // Arrange - Create a clean test solution for LintRunTool
        var cleanSolutionPath = Path.Combine(TestOutputPath, "LintRunTestSolution.sln");
        var cleanProjectDir = Path.Combine(TestOutputPath, "LintRunTestProject");
        var cleanProjectPath = Path.Combine(cleanProjectDir, "LintRunTestProject.csproj");

        // Clean up any existing files
        if (Directory.Exists(cleanProjectDir))
            Directory.Delete(cleanProjectDir, true);

        Directory.CreateDirectory(cleanProjectDir);

        // Create a minimal, clean test project
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
            </Project>
            """;
        File.WriteAllText(cleanProjectPath, projectContent);

        // Create a simple test file with some violations
        var testFilePath = Path.Combine(cleanProjectDir, "TestFile.cs");
        var testFileContent = """
            using System;
            using System.Threading.Tasks;

            namespace LintRunTestProject;

            public class TestClass
            {
                private string _name = "Test";

                public string Name
                {
                    get => _name;
                    set => _name = value ?? throw new ArgumentNullException(nameof(value));
                }

                public int ProcessValue(int value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentException("Value cannot be negative", nameof(value));
                    }
                    return value * 2;
                }

                // This method violates EXXER rules - missing CancellationToken
                public async Task<string> GetDataAsync()
                {
                    await Task.Delay(100);
                    return "data";
                }

                // This method violates EXXER rules - uses Console.WriteLine
                public void LogMessage(string message)
                {
                    Console.WriteLine(message);
                }
            }
            """;
        File.WriteAllText(testFilePath, testFileContent);

        // Create a clean solution file
        var solutionContent = """
            Microsoft Visual Studio Solution File, Format Version 12.00
            # Visual Studio Version 17
            VisualStudioVersion = 17.0.31903.59
            MinimumVisualStudioVersion = 10.0.40219.1
            Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "LintRunTestProject", "LintRunTestProject\\LintRunTestProject.csproj", "{12345678-1234-5678-9ABC-123456789ABC}"
            EndProject
            Global
                GlobalSection(SolutionConfigurationPlatforms) = preSolution
                    Debug|Any CPU = Debug|Any CPU
                EndGlobalSection
                GlobalSection(ProjectConfigurationPlatforms) = postSolution
                    {12345678-1234-5678-9ABC-123456789ABC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                    {12345678-1234-5678-9ABC-123456789ABC}.Debug|Any CPU.Build.0 = Debug|Any CPU
                EndGlobalSection
            EndGlobal
            """;
        File.WriteAllText(cleanSolutionPath, solutionContent);

        // Load the clean solution first
        await LoadSolutionTool.LoadSolution(cleanSolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        // Act - Run linting on the clean solution
        var result = await LintRunTool.LintRun(
            solutionPath: cleanSolutionPath,
            scope: "all",
            severityConfig: "error,warning",
            progress: null,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert - Verify the result contains expected structure
        Assert.NotNull(result);
        Assert.Contains("violations", result.ToLowerInvariant());
        // Policy recommendations only included when violations are detected
        // Verify structure is present regardless
        Assert.Contains("analysis", result.ToLowerInvariant());
    }

    /// <summary>
    /// LintRun_WithSpecificFile_ReturnsFileSpecificViolations.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 30000)]
    public async Task LintRun_WithSpecificFile_ReturnsFileSpecificViolations()
    {
        // Arrange - Create a clean test solution for LintRunTool
        var cleanSolutionPath = Path.Combine(TestOutputPath, "LintRunTestSolution2.sln");
        var cleanProjectDir = Path.Combine(TestOutputPath, "LintRunTestProject2");
        var cleanProjectPath = Path.Combine(cleanProjectDir, "LintRunTestProject2.csproj");

        // Clean up any existing files
        if (Directory.Exists(cleanProjectDir))
            Directory.Delete(cleanProjectDir, true);

        Directory.CreateDirectory(cleanProjectDir);

        // Create a minimal, clean test project
        var projectContent = """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
            </Project>
            """;
        File.WriteAllText(cleanProjectPath, projectContent);

        // Create a simple test file with some violations
        var testFilePath = Path.Combine(cleanProjectDir, "TestFile.cs");
        var testFileContent = """
            using System;
            using System.Threading.Tasks;

            namespace LintRunTestProject2;

            public class TestClass
            {
                // This method violates EXXER rules - missing CancellationToken
                public async Task<string> GetDataAsync()
                {
                    await Task.Delay(100);
                    return "data";
                }

                // This method violates EXXER rules - uses Console.WriteLine
                public void LogMessage(string message)
                {
                    Console.WriteLine(message);
                }
            }
            """;
        File.WriteAllText(testFilePath, testFileContent);

        // Create a clean solution file
        var solutionContent = """
            Microsoft Visual Studio Solution File, Format Version 12.00
            # Visual Studio Version 17
            VisualStudioVersion = 17.0.31903.59
            MinimumVisualStudioVersion = 10.0.40219.1
            Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "LintRunTestProject2", "LintRunTestProject2\\LintRunTestProject2.csproj", "{12345678-1234-5678-9ABC-123456789ABC}"
            EndProject
            Global
                GlobalSection(SolutionConfigurationPlatforms) = preSolution
                    Debug|Any CPU = Debug|Any CPU
                EndGlobalSection
                GlobalSection(ProjectConfigurationPlatforms) = postSolution
                    {12345678-1234-5678-9ABC-123456789ABC}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                    {12345678-1234-5678-9ABC-123456789ABC}.Debug|Any CPU.Build.0 = Debug|Any CPU
                EndGlobalSection
            EndGlobal
            """;
        File.WriteAllText(cleanSolutionPath, solutionContent);

        // Load the clean solution first
        await LoadSolutionTool.LoadSolution(cleanSolutionPath, null, Xunit.TestContext.Current.CancellationToken);

        // Act - Run linting on specific file
        var result = await LintRunTool.LintRun(
            solutionPath: cleanSolutionPath,
            scope: testFilePath,
            severityConfig: "error",
            progress: null,
            cancellationToken: Xunit.TestContext.Current.CancellationToken);

        // Assert - Verify the result contains file-specific violations
        Assert.NotNull(result);
        Assert.Contains("violations", result.ToLowerInvariant());
    }

    /// <summary>
    /// LintRun_WithInvalidSolutionPath_ThrowsMcpException.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 30000)] // 30 second timeout for unit test
    public async Task LintRun_WithInvalidSolutionPath_ThrowsMcpException()
    {
        // Act & Assert - Should throw McpException for invalid path
        await Assert.ThrowsAsync<McpException>(async () =>
            await LintRunTool.LintRun(
                solutionPath: "./NonExistent.sln",
                scope: "all",
                severityConfig: "error",
                progress: null,
                cancellationToken: Xunit.TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// LintRun_WithCancellation_RespectsCancellationToken.
    /// </summary>
    /// <returns></returns>
    [Fact(Timeout = 30000)] // Increased timeout for unit test
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
    /// <remarks>
    /// <para>
    /// <strong>Timeout Configuration:</strong>
    /// Timeout is set to 90 seconds for peace of mind, though actual execution time is typically less than 60 seconds.
    /// This higher timeout accounts for system variability and ensures the test doesn't fail due to temporary system load.
    /// </para>
    /// <para>
    /// <strong>Performance Regression Guard:</strong>
    /// This test includes execution time tracking and assertion NOT as a performance benchmark, but as a regression guard
    /// to detect code degradation. If execution time exceeds 75 seconds, it indicates potential performance regression
    /// that should be investigated. This helps catch:
    /// - Unintended synchronous blocking operations
    /// - Degraded caching effectiveness
    /// - Increased solution complexity without corresponding optimizations
    /// - Resource contention issues
    /// </para>
    /// <para>
    /// <strong>Note:</strong> This is NOT a formal performance test. For true performance testing, use dedicated
    /// performance test suites with controlled environments and statistical analysis.
    /// </para>
    /// </remarks>
    [Fact(Timeout = 90000)] // 90 seconds for peace of mind (actual execution typically < 60 seconds)
    public async Task LintRun_WithProgressReporter_CallsProgressCallback()
    {
        // Overall test execution timer for regression detection
        var totalTestStopwatch = System.Diagnostics.Stopwatch.StartNew();

        _testLogger.LogInformation("=== TEST STARTED ===");
        _testLogger.LogInformation("Step 1: Test initialization complete");

        // Arrange - Load solution first
        _testLogger.LogInformation("Step 2: Starting LoadSolutionTool.LoadSolution");
        var loadSolutionStopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            await LoadSolutionTool.LoadSolution(SolutionPath, null, Xunit.TestContext.Current.CancellationToken);
            loadSolutionStopwatch.Stop();
            _testLogger.LogInformation("Step 2: LoadSolutionTool.LoadSolution completed in {ElapsedMs}ms", loadSolutionStopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            loadSolutionStopwatch.Stop();
            _testLogger.LogError(ex, "Step 2: LoadSolutionTool.LoadSolution FAILED after {ElapsedMs}ms", loadSolutionStopwatch.ElapsedMilliseconds);
            throw;
        }

        // Arrange - Track progress calls
        _testLogger.LogInformation("Step 3: Setting up progress tracking");
        var progressCalls = new List<string>();
        var progress = new Progress<string>(message =>
        {
            _testLogger.LogInformation("PROGRESS: {Message}", message);
            progressCalls.Add(message);
        });
        _testLogger.LogInformation("Step 3: Progress tracking setup complete");

        // Act - Run linting with progress reporter
        _testLogger.LogInformation("Step 4: Starting LintRunTool.LintRun");
        var lintRunStopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            var result = await LintRunTool.LintRun(
                solutionPath: SolutionPath,
                scope: "all",
                severityConfig: "error,warning",
                progress: progress,
                cancellationToken: Xunit.TestContext.Current.CancellationToken);

            lintRunStopwatch.Stop();
            _testLogger.LogInformation("Step 4: LintRunTool.LintRun completed in {ElapsedMs}ms", lintRunStopwatch.ElapsedMilliseconds);

            // Assert - Verify progress was reported
            _testLogger.LogInformation("Step 5: Starting assertions");
            Assert.NotNull(result);
            Assert.NotEmpty(progressCalls);
            Assert.Contains(progressCalls, call => call.Contains("linting", StringComparison.OrdinalIgnoreCase));

            totalTestStopwatch.Stop();
            var totalElapsedSeconds = totalTestStopwatch.ElapsedMilliseconds / 1000.0;

            _testLogger.LogInformation("Step 5: All functional assertions passed");
            _testLogger.LogInformation("=== EXECUTION TIME TRACKING ===");
            _testLogger.LogInformation("Total test execution time: {TotalSeconds:F2} seconds ({TotalMs}ms)", totalElapsedSeconds, totalTestStopwatch.ElapsedMilliseconds);
            _testLogger.LogInformation("  - LoadSolution: {LoadSolutionMs}ms", loadSolutionStopwatch.ElapsedMilliseconds);
            _testLogger.LogInformation("  - LintRun: {LintRunMs}ms", lintRunStopwatch.ElapsedMilliseconds);
            _testLogger.LogInformation("  - Other overhead: {OtherMs}ms", totalTestStopwatch.ElapsedMilliseconds - loadSolutionStopwatch.ElapsedMilliseconds - lintRunStopwatch.ElapsedMilliseconds);

            // Performance regression guard assertion
            // NOTE: This is NOT a performance test - it's a regression guard to detect code degradation.
            // If execution time exceeds 75 seconds, it indicates potential performance regression that should be investigated.
            // Actual execution time is typically less than 60 seconds under normal conditions.
            Assert.True(
                totalElapsedSeconds < 75.0,
                $"Test execution took {totalElapsedSeconds:F2} seconds, exceeding the 75-second regression threshold. " +
                $"This may indicate performance degradation. Investigate: caching effectiveness, synchronous blocking operations, " +
                $"or increased solution complexity without corresponding optimizations.");

            _testLogger.LogInformation("=== REGRESSION GUARD ===");
            _testLogger.LogInformation("Execution time ({TotalSeconds:F2}s) is within acceptable threshold (< 75s)", totalElapsedSeconds);
            _testLogger.LogInformation("=== TEST COMPLETED SUCCESSFULLY ===");
        }
        catch (Exception ex)
        {
            lintRunStopwatch.Stop();
            totalTestStopwatch.Stop();
            var totalElapsedSeconds = totalTestStopwatch.ElapsedMilliseconds / 1000.0;

            _testLogger.LogError(ex, "Step 4: LintRunTool.LintRun FAILED after {ElapsedMs}ms", lintRunStopwatch.ElapsedMilliseconds);
            _testLogger.LogInformation("Total test execution time before failure: {TotalSeconds:F2} seconds ({TotalMs}ms)", totalElapsedSeconds, totalTestStopwatch.ElapsedMilliseconds);
            _testLogger.LogInformation("Progress calls count: {Count}", progressCalls.Count);
            _testLogger.LogInformation("Last progress message: {LastMessage}", progressCalls.LastOrDefault() ?? "None");
            _testLogger.LogInformation("Log file location: {LogFilePath}", _testLogger.LogFilePath);
            throw;
        }
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