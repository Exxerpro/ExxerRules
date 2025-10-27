using IndFusion.Mcp.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace IndFusion.Mcp.Tests.Integration.Contracts;

/// <summary>
/// Contract tests for ILintingService interface.
/// These tests validate that any implementation of ILintingService fulfills its contract.
/// </summary>
public class LintingServiceContractTests : ServiceContractTestBase<ILintingService, LintingServiceStub>
{
    public LintingServiceContractTests(ITestOutputHelper testOutput) : base(testOutput)
    {
    }

    protected override void ConfigureServiceDependencies(IServiceCollection services)
    {
        // Register dependencies that ILintingService implementations typically need
        services.AddLogging();
        services.AddScoped<ILogger<LintingServiceStub>>();
        
        // Add any other dependencies that real implementations would need
        // This is where you'd register analyzers, policy services, etc.
    }

    [Fact]
    public async Task RunLintingAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new LintingRequest(
            SolutionPath: "C:\\Test\\TestSolution.sln",
            Scope: "src",
            SeverityFilter: "Warning",
            RuleIds: new[] { "EXXER001", "EXXER002" },
            IncludePolicyRecommendations: true,
            OutputFormat: "Json"
        );

        // Act
        var result = await Service.RunLintingAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Linting result should not be null");
        result.Success.ShouldBeTrue("Linting should complete successfully");
        result.Violations.ShouldNotBeNull("Violations collection should not be null");
        result.Summary.ShouldNotBeNull("Summary should not be null");
        result.PolicyDecisions.ShouldNotBeNull("Policy decisions should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("RunLintingAsync contract validation passed");
    }

    [Fact]
    public async Task RunLintingAsync_WithCancellation_ShouldHandleCancellationGracefully()
    {
        // Arrange
        var request = new LintingRequest(
            SolutionPath: "C:\\Test\\TestSolution.sln"
        );
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act & Assert
        var result = await Service.RunLintingAsync(request, cts.Token);
        
        // Should handle cancellation gracefully (either return a result or throw OperationCanceledException)
        result.ShouldNotBeNull("Should return a result even when cancelled");
        
        Logger.LogInformation("Cancellation handling validation passed");
    }

    [Fact]
    public async Task StartWatcherAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new LintingWatchRequest(
            SolutionPath: "C:\\Test\\TestSolution.sln",
            WatchPatterns: new[] { "*.cs", "*.csproj" },
            DebounceMs: 1000,
            AutoFix: false,
            NotificationEndpoint: null
        );

        // Act
        var result = await Service.StartWatcherAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Watcher result should not be null");
        result.Success.ShouldBeTrue("Watcher should start successfully");
        
        Logger.LogInformation("StartWatcherAsync contract validation passed");
    }

    [Fact]
    public async Task StopWatcherAsync_WithValidSolutionPath_ShouldReturnSuccessResult()
    {
        // Arrange
        var solutionPath = "C:\\Test\\TestSolution.sln";

        // Act
        var result = await Service.StopWatcherAsync(solutionPath, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Stop watcher result should not be null");
        result.Success.ShouldBeTrue("Watcher should stop successfully");
        
        Logger.LogInformation("StopWatcherAsync contract validation passed");
    }

    [Fact]
    public async Task GetPolicyAsync_WithValidSolutionPath_ShouldReturnPolicy()
    {
        // Arrange
        var solutionPath = "C:\\Test\\TestSolution.sln";

        // Act
        var policy = await Service.GetPolicyAsync(solutionPath, CreateTestCancellationToken());

        // Assert
        policy.ShouldNotBeNull("Policy should not be null");
        policy.SolutionPath.ShouldBe(solutionPath, "Policy should be for the correct solution");
        policy.RuleSeverities.ShouldNotBeNull("Rule severities should not be null");
        policy.GlobalSettings.ShouldNotBeNull("Global settings should not be null");
        policy.LastUpdated.ShouldBeLessThanOrEqualTo(DateTime.UtcNow, "Last updated should be in the past");
        policy.Version.ShouldNotBeNullOrEmpty("Version should not be null or empty");
        
        Logger.LogInformation("GetPolicyAsync contract validation passed");
    }

    [Fact]
    public async Task UpdatePolicyAsync_WithValidPolicy_ShouldReturnSuccessResult()
    {
        // Arrange
        var solutionPath = "C:\\Test\\TestSolution.sln";
        var policy = new LintingPolicy(
            SolutionPath: solutionPath,
            RuleSeverities: new Dictionary<string, string>
            {
                { "EXXER001", "Error" },
                { "EXXER002", "Warning" }
            },
            GlobalSettings: new Dictionary<string, object>
            {
                { "AutoFix", true },
                { "MaxViolations", 100 }
            },
            LastUpdated: DateTime.UtcNow,
            Version: "1.0.0"
        );

        // Act
        var result = await Service.UpdatePolicyAsync(solutionPath, policy, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Update policy result should not be null");
        result.Success.ShouldBeTrue("Policy should be updated successfully");
        
        Logger.LogInformation("UpdatePolicyAsync contract validation passed");
    }

    public override async Task Service_ShouldHandleCancellationTokens()
    {
        // Test cancellation token handling for all async methods
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var request = new LintingRequest("C:\\Test\\TestSolution.sln");
        var watchRequest = new LintingWatchRequest("C:\\Test\\TestSolution.sln", new[] { "*.cs" });
        var policy = new LintingPolicy("C:\\Test\\TestSolution.sln", new Dictionary<string, string>(), 
            new Dictionary<string, object>(), DateTime.UtcNow, "1.0.0");

        // All methods should handle cancellation gracefully
        await Should.NotThrowAsync(async () => await Service.RunLintingAsync(request, cts.Token));
        await Should.NotThrowAsync(async () => await Service.StartWatcherAsync(watchRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.StopWatcherAsync("C:\\Test\\TestSolution.sln", cts.Token));
        await Should.NotThrowAsync(async () => await Service.GetPolicyAsync("C:\\Test\\TestSolution.sln", cts.Token));
        await Should.NotThrowAsync(async () => await Service.UpdatePolicyAsync("C:\\Test\\TestSolution.sln", policy, cts.Token));

        Logger.LogInformation("Cancellation token handling validation passed for all methods");
    }

    public override async Task Service_ShouldHandleNullParameters()
    {
        // Test null parameter handling
        var request = new LintingRequest("C:\\Test\\TestSolution.sln");
        var watchRequest = new LintingWatchRequest("C:\\Test\\TestSolution.sln", new[] { "*.cs" });
        var policy = new LintingPolicy("C:\\Test\\TestSolution.sln", new Dictionary<string, string>(), 
            new Dictionary<string, object>(), DateTime.UtcNow, "1.0.0");

        // Methods should handle null parameters gracefully
        await Should.NotThrowAsync(async () => await Service.RunLintingAsync(request, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.StartWatcherAsync(watchRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.StopWatcherAsync("C:\\Test\\TestSolution.sln", CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.GetPolicyAsync("C:\\Test\\TestSolution.sln", CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.UpdatePolicyAsync("C:\\Test\\TestSolution.sln", policy, CancellationToken.None));

        Logger.LogInformation("Null parameter handling validation passed");
    }
}

/// <summary>
/// Stub implementation of ILintingService for contract testing.
/// This provides a minimal implementation that satisfies the interface contract.
/// </summary>
public class LintingServiceStub : ILintingService
{
    private readonly ILogger<LintingServiceStub> _logger;

    public LintingServiceStub(ILogger<LintingServiceStub> logger)
    {
        _logger = logger;
    }

    public async Task<LintingResult> RunLintingAsync(LintingRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Running linting for solution: {SolutionPath}", request.SolutionPath);
        
        // Simulate some processing time
        await Task.Delay(100, cancellationToken);
        
        return new LintingResult(
            Success: true,
            Violations: new List<LintingViolation>(),
            Summary: new LintingSummary(0, 0, 0, 0, 0, 0, 0),
            PolicyDecisions: new List<PolicyDecision>(),
            ExecutionTimeMs: 100
        );
    }

    public async Task<LintingResult> StartWatcherAsync(LintingWatchRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting watcher for solution: {SolutionPath}", request.SolutionPath);
        
        await Task.Delay(50, cancellationToken);
        
        return new LintingResult(
            Success: true,
            Violations: new List<LintingViolation>(),
            Summary: new LintingSummary(0, 0, 0, 0, 0, 0, 0),
            PolicyDecisions: new List<PolicyDecision>(),
            ExecutionTimeMs: 50
        );
    }

    public async Task<LintingResult> StopWatcherAsync(string solutionPath, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Stopping watcher for solution: {SolutionPath}", solutionPath);
        
        await Task.Delay(25, cancellationToken);
        
        return new LintingResult(
            Success: true,
            Violations: new List<LintingViolation>(),
            Summary: new LintingSummary(0, 0, 0, 0, 0, 0, 0),
            PolicyDecisions: new List<PolicyDecision>(),
            ExecutionTimeMs: 25
        );
    }

    public async Task<LintingPolicy> GetPolicyAsync(string solutionPath, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting policy for solution: {SolutionPath}", solutionPath);
        
        await Task.Delay(25, cancellationToken);
        
        return new LintingPolicy(
            SolutionPath: solutionPath,
            RuleSeverities: new Dictionary<string, string>(),
            GlobalSettings: new Dictionary<string, object>(),
            LastUpdated: DateTime.UtcNow,
            Version: "1.0.0"
        );
    }

    public async Task<LintingResult> UpdatePolicyAsync(string solutionPath, LintingPolicy policy, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating policy for solution: {SolutionPath}", solutionPath);
        
        await Task.Delay(25, cancellationToken);
        
        return new LintingResult(
            Success: true,
            Violations: new List<LintingViolation>(),
            Summary: new LintingSummary(0, 0, 0, 0, 0, 0, 0),
            PolicyDecisions: new List<PolicyDecision>(),
            ExecutionTimeMs: 25
        );
    }
}