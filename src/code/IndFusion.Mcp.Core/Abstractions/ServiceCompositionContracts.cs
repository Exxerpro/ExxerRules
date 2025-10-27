namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Service composition contract for MCP tools.
/// Defines how individual services are composed to create MCP tools.
/// </summary>
public interface IMcpToolCompositionService
{
    /// <summary>
    /// Composes the lint_run MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for linting operations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed linting result.</returns>
    Task<McpToolResult> ComposeLintRunToolAsync(LintingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the pattern_suggest MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for pattern suggestions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed pattern suggestion result.</returns>
    Task<McpToolResult> ComposePatternSuggestToolAsync(PatternSuggestionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the fixer001_apply MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for code transformations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed transformation result.</returns>
    Task<McpToolResult> ComposeFixer001ApplyToolAsync(Fixer001Request request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the safe_regex_replace MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for regex transformations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed regex transformation result.</returns>
    Task<McpToolResult> ComposeSafeRegexReplaceToolAsync(SafeRegexRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Composes the knowledge_rag MCP tool from individual services.
    /// </summary>
    /// <param name="request">Request for RAG operations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composed RAG result.</returns>
    Task<McpToolResult> ComposeKnowledgeRagToolAsync(RagQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the composition configuration for MCP tools.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Composition configuration.</returns>
    Task<McpToolCompositionConfiguration> GetCompositionConfigurationAsync(CancellationToken cancellationToken = default);
}

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

/// <summary>
/// Service integration contract for integrating with external systems.
/// </summary>
public interface IServiceIntegrationService
{
    /// <summary>
    /// Integrates with external linting services.
    /// </summary>
    /// <param name="request">Request for external linting.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>External linting result.</returns>
    Task<ExternalLintingResult> IntegrateExternalLintingAsync(ExternalLintingRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Integrates with external code analysis services.
    /// </summary>
    /// <param name="request">Request for external analysis.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>External analysis result.</returns>
    Task<ExternalAnalysisResult> IntegrateExternalAnalysisAsync(ExternalAnalysisRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Integrates with external knowledge bases.
    /// </summary>
    /// <param name="request">Request for external knowledge integration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>External knowledge integration result.</returns>
    Task<ExternalKnowledgeResult> IntegrateExternalKnowledgeAsync(ExternalKnowledgeRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the integration configuration.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Integration configuration.</returns>
    Task<IntegrationConfiguration> GetIntegrationConfigurationAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of MCP tool composition operations.
/// </summary>
/// <param name="Success">Whether the composition succeeded.</param>
/// <param name="ToolName">Name of the composed tool.</param>
/// <param name="Result">Result from the composed tool.</param>
/// <param name="CompositionDetails">Details about the composition process.</param>
/// <param name="ExecutionTimeMs">Time taken for composition and execution.</param>
/// <param name="ErrorDetails">Error details if composition failed.</param>
public record McpToolResult(
    bool Success,
    string ToolName,
    object Result,
    CompositionDetails CompositionDetails,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Details about the composition process.
/// </summary>
/// <param name="ServicesUsed">Services used in the composition.</param>
/// <param name="CompositionStrategy">Strategy used for composition.</param>
/// <param name="Dependencies">Dependencies between services.</param>
/// <param name="PerformanceMetrics">Performance metrics for the composition.</param>
public record CompositionDetails(
    IEnumerable<string> ServicesUsed,
    string CompositionStrategy,
    IEnumerable<ServiceDependency> Dependencies,
    CompositionPerformanceMetrics PerformanceMetrics
);

/// <summary>
/// Dependency between services in a composition.
/// </summary>
/// <param name="SourceService">Source service name.</param>
/// <param name="TargetService">Target service name.</param>
/// <param name="DependencyType">Type of dependency.</param>
/// <param name="DataFlow">Data flow between services.</param>
public record ServiceDependency(
    string SourceService,
    string TargetService,
    string DependencyType,
    string DataFlow
);

/// <summary>
/// Performance metrics for service composition.
/// </summary>
/// <param name="TotalExecutionTimeMs">Total execution time.</param>
/// <param name="ServiceExecutionTimes">Execution times for individual services.</param>
/// <param name="OverheadTimeMs">Overhead time for composition.</param>
/// <param name="MemoryUsage">Memory usage during composition.</param>
public record CompositionPerformanceMetrics(
    long TotalExecutionTimeMs,
    Dictionary<string, long> ServiceExecutionTimes,
    long OverheadTimeMs,
    long MemoryUsage
);

/// <summary>
/// Configuration for MCP tool composition.
/// </summary>
/// <param name="AvailableTools">Available MCP tools.</param>
/// <param name="CompositionStrategies">Available composition strategies.</param>
/// <param name="ServiceMappings">Mappings between tools and services.</param>
/// <param name="PerformanceSettings">Performance settings for composition.</param>
/// <param name="Version">Version of the configuration.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record McpToolCompositionConfiguration(
    IEnumerable<McpToolInfo> AvailableTools,
    IEnumerable<CompositionStrategy> CompositionStrategies,
    Dictionary<string, IEnumerable<string>> ServiceMappings,
    CompositionPerformanceSettings PerformanceSettings,
    string Version,
    DateTime LastUpdated
);

/// <summary>
/// Information about an MCP tool.
/// </summary>
/// <param name="Name">Name of the tool.</param>
/// <param name="Description">Description of the tool.</param>
/// <param name="RequiredServices">Services required by the tool.</param>
/// <param name="Parameters">Parameters accepted by the tool.</param>
/// <param name="IsEnabled">Whether the tool is enabled.</param>
public record McpToolInfo(
    string Name,
    string Description,
    IEnumerable<string> RequiredServices,
    Dictionary<string, object> Parameters,
    bool IsEnabled
);

/// <summary>
/// Composition strategy for services.
/// </summary>
/// <param name="Name">Name of the strategy.</param>
/// <param name="Description">Description of the strategy.</param>
/// <param name="ServiceOrder">Order of services in the composition.</param>
/// <param name="DependencyRules">Rules for service dependencies.</param>
/// <param name="IsDefault">Whether this is the default strategy.</param>
public record CompositionStrategy(
    string Name,
    string Description,
    IEnumerable<string> ServiceOrder,
    IEnumerable<DependencyRule> DependencyRules,
    bool IsDefault
);

/// <summary>
/// Rule for service dependencies.
/// </summary>
/// <param name="RuleName">Name of the rule.</param>
/// <param name="Condition">Condition for the rule.</param>
/// <param name="Action">Action to take when condition is met.</param>
/// <param name="Priority">Priority of the rule.</param>
public record DependencyRule(
    string RuleName,
    string Condition,
    string Action,
    int Priority
);

/// <summary>
/// Performance settings for composition.
/// </summary>
/// <param name="MaxExecutionTimeMs">Maximum execution time.</param>
/// <param name="MaxMemoryUsage">Maximum memory usage.</param>
/// <param name="TimeoutMs">Timeout for individual services.</param>
/// <param name="RetryAttempts">Number of retry attempts.</param>
public record CompositionPerformanceSettings(
    long MaxExecutionTimeMs,
    long MaxMemoryUsage,
    int TimeoutMs,
    int RetryAttempts
);