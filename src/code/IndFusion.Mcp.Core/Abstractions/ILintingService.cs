using Microsoft.CodeAnalysis;

namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Defines the contract for linting operations that run EXXER analyzers and apply policies.
/// This service orchestrates analyzer execution, violation detection, and policy enforcement.
/// </summary>
public interface ILintingService
{
    /// <summary>
    /// Runs EXXER analyzers on the specified solution and returns violations with policy recommendations.
    /// </summary>
    /// <param name="request">The linting request containing solution path, scope, and severity configuration.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result containing violations, policy decisions, and remediation suggestions.</returns>
    Task<LintingResult> RunLintingAsync(LintingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a real-time watcher that monitors file changes and triggers linting automatically.
    /// </summary>
    /// <param name="request">The watch request containing solution path and configuration.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating watcher status and any initial violations found.</returns>
    Task<LintingResult> StartWatcherAsync(LintingWatchRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the active linting watcher for the specified solution.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating the watcher was stopped successfully.</returns>
    Task<LintingResult> StopWatcherAsync(string solutionPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current linting policy configuration for the specified solution.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>The current policy configuration including rule severities and enforcement settings.</returns>
    Task<LintingPolicy> GetPolicyAsync(string solutionPath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the linting policy configuration for the specified solution.
    /// </summary>
    /// <param name="solutionPath">Path to the solution file (.sln).</param>
    /// <param name="policy">The new policy configuration to apply.</param>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A result indicating whether the policy was updated successfully.</returns>
    Task<LintingResult> UpdatePolicyAsync(string solutionPath, LintingPolicy policy, CancellationToken cancellationToken = default);
}