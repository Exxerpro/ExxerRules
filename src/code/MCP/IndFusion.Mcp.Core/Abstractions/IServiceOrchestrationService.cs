namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Service orchestration contract for coordinating multiple services.
/// </summary>
public interface IServiceOrchestrationService
{
    /// <summary>
    /// Orchestrates a multi-step workflow involving multiple services.
    /// </summary>
    /// <param name="workflowRequest">Request for the workflow.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Orchestration result.</returns>
    Task<WorkflowOrchestrationResult> OrchestrateWorkflowAsync(WorkflowRequest workflowRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a service chain where the output of one service feeds into the next.
    /// </summary>
    /// <param name="chainRequest">Request for the service chain.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Service chain result.</returns>
    Task<ServiceChainResult> ExecuteServiceChainAsync(ServiceChainRequest chainRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Coordinates parallel execution of multiple services.
    /// </summary>
    /// <param name="parallelRequest">Request for parallel execution.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Parallel execution result.</returns>
    Task<ParallelExecutionResult> ExecuteParallelServicesAsync(ParallelExecutionRequest parallelRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the orchestration configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Orchestration configuration.</returns>
    Task<OrchestrationConfiguration> GetOrchestrationConfigurationAsync(CancellationToken cancellationToken = default);
}