namespace IndFusion.Mcp.Core.Abstractions;

/// <summary>
/// Request for workflow orchestration.
/// </summary>
/// <param name="WorkflowName">Name of the workflow to execute.</param>
/// <param name="Steps">Steps in the workflow.</param>
/// <param name="Context">Context for the workflow.</param>
/// <param name="ParallelExecution">Whether steps can be executed in parallel.</param>
/// <param name="ErrorHandling">Error handling strategy.</param>
public record WorkflowRequest(
    string WorkflowName,
    IEnumerable<WorkflowStep> Steps,
    Dictionary<string, object> Context,
    bool ParallelExecution = false,
    string ErrorHandling = "StopOnError"
);

/// <summary>
/// Request for service chain execution.
/// </summary>
/// <param name="ChainName">Name of the service chain.</param>
/// <param name="Services">Services in the chain.</param>
/// <param name="DataFlow">Data flow between services.</param>
/// <param name="ValidationSteps">Validation steps between services.</param>
/// <param name="TimeoutMs">Timeout for the entire chain.</param>
public record ServiceChainRequest(
    string ChainName,
    IEnumerable<ServiceChainStep> Services,
    IEnumerable<DataFlowStep> DataFlow,
    IEnumerable<ValidationStep> ValidationSteps,
    int TimeoutMs = 30000
);

/// <summary>
/// Request for parallel service execution.
/// </summary>
/// <param name="ExecutionName">Name of the parallel execution.</param>
/// <param name="Services">Services to execute in parallel.</param>
/// <param name="CoordinationStrategy">Strategy for coordinating parallel execution.</param>
/// <param name="ResultAggregation">Strategy for aggregating results.</param>
/// <param name="TimeoutMs">Timeout for parallel execution.</param>
public record ParallelExecutionRequest(
    string ExecutionName,
    IEnumerable<ParallelServiceStep> Services,
    string CoordinationStrategy,
    string ResultAggregation,
    int TimeoutMs = 30000
);

/// <summary>
/// Request for external linting integration.
/// </summary>
/// <param name="ExternalService">External service to integrate with.</param>
/// <param name="CodeToAnalyze">Code to analyze.</param>
/// <param name="AnalysisOptions">Options for analysis.</param>
/// <param name="IntegrationSettings">Settings for integration.</param>
public record ExternalLintingRequest(
    string ExternalService,
    string CodeToAnalyze,
    Dictionary<string, object> AnalysisOptions,
    IntegrationSettings IntegrationSettings
);

/// <summary>
/// Request for external analysis integration.
/// </summary>
/// <param name="ExternalService">External service to integrate with.</param>
/// <param name="AnalysisType">Type of analysis to perform.</param>
/// <param name="InputData">Input data for analysis.</param>
/// <param name="AnalysisParameters">Parameters for analysis.</param>
/// <param name="IntegrationSettings">Settings for integration.</param>
public record ExternalAnalysisRequest(
    string ExternalService,
    string AnalysisType,
    Dictionary<string, object> InputData,
    Dictionary<string, object> AnalysisParameters,
    IntegrationSettings IntegrationSettings
);

/// <summary>
/// Request for external knowledge integration.
/// </summary>
/// <param name="ExternalService">External service to integrate with.</param>
/// <param name="KnowledgeOperation">Operation to perform on knowledge.</param>
/// <param name="KnowledgeData">Knowledge data to process.</param>
/// <param name="IntegrationSettings">Settings for integration.</param>
public record ExternalKnowledgeRequest(
    string ExternalService,
    string KnowledgeOperation,
    Dictionary<string, object> KnowledgeData,
    IntegrationSettings IntegrationSettings
);

/// <summary>
/// Result of workflow orchestration.
/// </summary>
/// <param name="Success">Whether the workflow succeeded.</param>
/// <param name="WorkflowName">Name of the executed workflow.</param>
/// <param name="StepResults">Results from individual steps.</param>
/// <param name="FinalResult">Final result of the workflow.</param>
/// <param name="ExecutionTimeMs">Time taken for the workflow.</param>
/// <param name="ErrorDetails">Error details if workflow failed.</param>
public record WorkflowOrchestrationResult(
    bool Success,
    string WorkflowName,
    IEnumerable<WorkflowStepResult> StepResults,
    object FinalResult,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of service chain execution.
/// </summary>
/// <param name="Success">Whether the chain succeeded.</param>
/// <param name="ChainName">Name of the executed chain.</param>
/// <param name="ServiceResults">Results from individual services.</param>
/// <param name="ChainResult">Final result of the chain.</param>
/// <param name="ExecutionTimeMs">Time taken for the chain.</param>
/// <param name="ErrorDetails">Error details if chain failed.</param>
public record ServiceChainResult(
    bool Success,
    string ChainName,
    IEnumerable<ServiceChainStepResult> ServiceResults,
    object ChainResult,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of parallel service execution.
/// </summary>
/// <param name="Success">Whether the parallel execution succeeded.</param>
/// <param name="ExecutionName">Name of the parallel execution.</param>
/// <param name="ServiceResults">Results from individual services.</param>
/// <param name="AggregatedResult">Aggregated result from all services.</param>
/// <param name="ExecutionTimeMs">Time taken for parallel execution.</param>
/// <param name="ErrorDetails">Error details if execution failed.</param>
public record ParallelExecutionResult(
    bool Success,
    string ExecutionName,
    IEnumerable<ParallelServiceStepResult> ServiceResults,
    object AggregatedResult,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of external linting integration.
/// </summary>
/// <param name="Success">Whether the integration succeeded.</param>
/// <param name="ExternalService">External service used.</param>
/// <param name="LintingResults">Results from external linting.</param>
/// <param name="IntegrationMetrics">Metrics for the integration.</param>
/// <param name="ExecutionTimeMs">Time taken for integration.</param>
/// <param name="ErrorDetails">Error details if integration failed.</param>
public record ExternalLintingResult(
    bool Success,
    string ExternalService,
    IEnumerable<ExternalLintingIssue> LintingResults,
    IntegrationMetrics IntegrationMetrics,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of external analysis integration.
/// </summary>
/// <param name="Success">Whether the integration succeeded.</param>
/// <param name="ExternalService">External service used.</param>
/// <param name="AnalysisResults">Results from external analysis.</param>
/// <param name="IntegrationMetrics">Metrics for the integration.</param>
/// <param name="ExecutionTimeMs">Time taken for integration.</param>
/// <param name="ErrorDetails">Error details if integration failed.</param>
public record ExternalAnalysisResult(
    bool Success,
    string ExternalService,
    IEnumerable<ExternalAnalysisResult> AnalysisResults,
    IntegrationMetrics IntegrationMetrics,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of external knowledge integration.
/// </summary>
/// <param name="Success">Whether the integration succeeded.</param>
/// <param name="ExternalService">External service used.</param>
/// <param name="KnowledgeResults">Results from external knowledge.</param>
/// <param name="IntegrationMetrics">Metrics for the integration.</param>
/// <param name="ExecutionTimeMs">Time taken for integration.</param>
/// <param name="ErrorDetails">Error details if integration failed.</param>
public record ExternalKnowledgeResult(
    bool Success,
    string ExternalService,
    IEnumerable<ExternalKnowledgeItem> KnowledgeResults,
    IntegrationMetrics IntegrationMetrics,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// A step in a workflow.
/// </summary>
/// <param name="StepName">Name of the step.</param>
/// <param name="ServiceName">Service to execute in this step.</param>
/// <param name="InputMapping">Mapping of inputs for this step.</param>
/// <param name="OutputMapping">Mapping of outputs from this step.</param>
/// <param name="Dependencies">Dependencies on other steps.</param>
/// <param name="TimeoutMs">Timeout for this step.</param>
public record WorkflowStep(
    string StepName,
    string ServiceName,
    Dictionary<string, object> InputMapping,
    Dictionary<string, object> OutputMapping,
    IEnumerable<string> Dependencies,
    int TimeoutMs = 30000
);

/// <summary>
/// A step in a service chain.
/// </summary>
/// <param name="ServiceName">Name of the service.</param>
/// <param name="InputTransformation">Transformation to apply to inputs.</param>
/// <param name="OutputTransformation">Transformation to apply to outputs.</param>
/// <param name="ValidationRules">Validation rules for this step.</param>
/// <param name="TimeoutMs">Timeout for this step.</param>
public record ServiceChainStep(
    string ServiceName,
    string InputTransformation,
    string OutputTransformation,
    IEnumerable<ValidationRule> ValidationRules,
    int TimeoutMs = 30000
);

/// <summary>
/// A step in parallel service execution.
/// </summary>
/// <param name="ServiceName">Name of the service.</param>
/// <param name="InputData">Input data for the service.</param>
/// <param name="Priority">Priority of this service.</param>
/// <param name="TimeoutMs">Timeout for this service.</param>
public record ParallelServiceStep(
    string ServiceName,
    Dictionary<string, object> InputData,
    int Priority = 0,
    int TimeoutMs = 30000
);

/// <summary>
/// A data flow step between services.
/// </summary>
/// <param name="SourceService">Source service name.</param>
/// <param name="TargetService">Target service name.</param>
/// <param name="DataMapping">Mapping of data between services.</param>
/// <param name="Transformation">Transformation to apply to data.</param>
public record DataFlowStep(
    string SourceService,
    string TargetService,
    Dictionary<string, string> DataMapping,
    string? Transformation = null
);

/// <summary>
/// A validation step between services.
/// </summary>
/// <param name="ValidationName">Name of the validation.</param>
/// <param name="ValidationType">Type of validation.</param>
/// <param name="ValidationRules">Rules for validation.</param>
/// <param name="ErrorAction">Action to take on validation error.</param>
public record ValidationStep(
    string ValidationName,
    string ValidationType,
    IEnumerable<ValidationRule> ValidationRules,
    string ErrorAction = "Stop"
);

/// <summary>
/// Settings for external service integration.
/// </summary>
/// <param name="Authentication">Authentication settings.</param>
/// <param name="TimeoutMs">Timeout for external service calls.</param>
/// <param name="RetryAttempts">Number of retry attempts.</param>
/// <param name="RateLimiting">Rate limiting settings.</param>
/// <param name="Caching">Caching settings.</param>
public record IntegrationSettings(
    AuthenticationSettings Authentication,
    RateLimitingSettings RateLimiting,
    CachingSettings Caching,
    int TimeoutMs = 30000,
    int RetryAttempts = 3
);

/// <summary>
/// Configuration for orchestration.
/// </summary>
/// <param name="AvailableWorkflows">Available workflows.</param>
/// <param name="ServiceChains">Available service chains.</param>
/// <param name="ParallelExecutionSettings">Settings for parallel execution.</param>
/// <param name="ErrorHandlingStrategies">Available error handling strategies.</param>
/// <param name="Version">Version of the configuration.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record OrchestrationConfiguration(
    IEnumerable<WorkflowInfo> AvailableWorkflows,
    IEnumerable<ServiceChainInfo> ServiceChains,
    ParallelExecutionSettings ParallelExecutionSettings,
    IEnumerable<ErrorHandlingStrategy> ErrorHandlingStrategies,
    string Version,
    DateTime LastUpdated
);

/// <summary>
/// Configuration for external service integration.
/// </summary>
/// <param name="ExternalServices">Available external services.</param>
/// <param name="IntegrationTemplates">Templates for integration.</param>
/// <param name="AuthenticationProviders">Available authentication providers.</param>
/// <param name="RateLimitingSettings">Rate limiting settings.</param>
/// <param name="Version">Version of the configuration.</param>
/// <param name="LastUpdated">When the configuration was last updated.</param>
public record IntegrationConfiguration(
    IEnumerable<ExternalServiceInfo> ExternalServices,
    IEnumerable<IntegrationTemplate> IntegrationTemplates,
    IEnumerable<AuthenticationProvider> AuthenticationProviders,
    RateLimitingSettings RateLimitingSettings,
    string Version,
    DateTime LastUpdated
);

/// <summary>
/// Result of a workflow step.
/// </summary>
/// <param name="StepName">Name of the step.</param>
/// <param name="Success">Whether the step succeeded.</param>
/// <param name="Result">Result from the step.</param>
/// <param name="ExecutionTimeMs">Time taken for the step.</param>
/// <param name="ErrorDetails">Error details if step failed.</param>
public record WorkflowStepResult(
    string StepName,
    bool Success,
    object Result,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of a service chain step.
/// </summary>
/// <param name="ServiceName">Name of the service.</param>
/// <param name="Success">Whether the service succeeded.</param>
/// <param name="Result">Result from the service.</param>
/// <param name="ExecutionTimeMs">Time taken for the service.</param>
/// <param name="ErrorDetails">Error details if service failed.</param>
public record ServiceChainStepResult(
    string ServiceName,
    bool Success,
    object Result,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// Result of a parallel service step.
/// </summary>
/// <param name="ServiceName">Name of the service.</param>
/// <param name="Success">Whether the service succeeded.</param>
/// <param name="Result">Result from the service.</param>
/// <param name="ExecutionTimeMs">Time taken for the service.</param>
/// <param name="ErrorDetails">Error details if service failed.</param>
public record ParallelServiceStepResult(
    string ServiceName,
    bool Success,
    object Result,
    long ExecutionTimeMs,
    string? ErrorDetails = null
);

/// <summary>
/// An issue from external linting.
/// </summary>
/// <param name="IssueId">Unique identifier for the issue.</param>
/// <param name="Severity">Severity of the issue.</param>
/// <param name="Message">Description of the issue.</param>
/// <param name="Location">Location of the issue.</param>
/// <param name="RuleId">ID of the rule that generated the issue.</param>
/// <param name="SuggestedFix">Suggested fix for the issue.</param>
public record ExternalLintingIssue(
    string IssueId,
    string Severity,
    string Message,
    SourceLocation Location,
    string RuleId,
    string? SuggestedFix
);

/// <summary>
/// An item from external knowledge.
/// </summary>
/// <param name="ItemId">Unique identifier for the item.</param>
/// <param name="ItemType">Type of the knowledge item.</param>
/// <param name="Content">Content of the item.</param>
/// <param name="Metadata">Metadata about the item.</param>
/// <param name="Relevance">Relevance score (0.0-1.0).</param>
public record ExternalKnowledgeItem(
    string ItemId,
    string ItemType,
    string Content,
    Dictionary<string, object> Metadata,
    double Relevance
);

/// <summary>
/// Metrics for external service integration.
/// </summary>
/// <param name="RequestCount">Number of requests made.</param>
/// <param name="SuccessCount">Number of successful requests.</param>
/// <param name="FailureCount">Number of failed requests.</param>
/// <param name="AverageResponseTimeMs">Average response time.</param>
/// <param name="TotalDataTransferred">Total data transferred.</param>
public record IntegrationMetrics(
    int RequestCount,
    int SuccessCount,
    int FailureCount,
    long AverageResponseTimeMs,
    long TotalDataTransferred
);

/// <summary>
/// Authentication settings for external services.
/// </summary>
/// <param name="Provider">Authentication provider.</param>
/// <param name="Credentials">Authentication credentials.</param>
/// <param name="TokenExpiry">Token expiry time.</param>
/// <param name="RefreshToken">Refresh token if available.</param>
public record AuthenticationSettings(
    string Provider,
    Dictionary<string, object> Credentials,
    DateTime? TokenExpiry = null,
    string? RefreshToken = null
);

/// <summary>
/// Rate limiting settings for external services.
/// </summary>
/// <param name="RequestsPerMinute">Maximum requests per minute.</param>
/// <param name="BurstLimit">Burst limit for requests.</param>
/// <param name="BackoffStrategy">Strategy for backoff on rate limit.</param>
public record RateLimitingSettings(
    int RequestsPerMinute,
    int BurstLimit,
    string BackoffStrategy
);

/// <summary>
/// Caching settings for external services.
/// </summary>
/// <param name="Enabled">Whether caching is enabled.</param>
/// <param name="CacheDuration">Duration to cache results.</param>
/// <param name="CacheKeyStrategy">Strategy for generating cache keys.</param>
public record CachingSettings(
    bool Enabled,
    TimeSpan CacheDuration,
    string CacheKeyStrategy
);

/// <summary>
/// Information about a workflow.
/// </summary>
/// <param name="Name">Name of the workflow.</param>
/// <param name="Description">Description of the workflow.</param>
/// <param name="Steps">Steps in the workflow.</param>
/// <param name="IsEnabled">Whether the workflow is enabled.</param>
public record WorkflowInfo(
    string Name,
    string Description,
    IEnumerable<WorkflowStep> Steps,
    bool IsEnabled
);

/// <summary>
/// Information about a service chain.
/// </summary>
/// <param name="Name">Name of the service chain.</param>
/// <param name="Description">Description of the service chain.</param>
/// <param name="Services">Services in the chain.</param>
/// <param name="IsEnabled">Whether the service chain is enabled.</param>
public record ServiceChainInfo(
    string Name,
    string Description,
    IEnumerable<ServiceChainStep> Services,
    bool IsEnabled
);

/// <summary>
/// Settings for parallel execution.
/// </summary>
/// <param name="MaxConcurrency">Maximum number of concurrent services.</param>
/// <param name="CoordinationStrategy">Strategy for coordinating parallel execution.</param>
/// <param name="ResultAggregationStrategy">Strategy for aggregating results.</param>
public record ParallelExecutionSettings(
    int MaxConcurrency,
    string CoordinationStrategy,
    string ResultAggregationStrategy
);

/// <summary>
/// Error handling strategy.
/// </summary>
/// <param name="Name">Name of the strategy.</param>
/// <param name="Description">Description of the strategy.</param>
/// <param name="ErrorActions">Actions to take on different error types.</param>
/// <param name="IsDefault">Whether this is the default strategy.</param>
public record ErrorHandlingStrategy(
    string Name,
    string Description,
    Dictionary<string, string> ErrorActions,
    bool IsDefault
);

/// <summary>
/// Information about an external service.
/// </summary>
/// <param name="Name">Name of the service.</param>
/// <param name="Description">Description of the service.</param>
/// <param name="Endpoint">Endpoint URL for the service.</param>
/// <param name="AuthenticationRequired">Whether authentication is required.</param>
/// <param name="IsEnabled">Whether the service is enabled.</param>
public record ExternalServiceInfo(
    string Name,
    string Description,
    string Endpoint,
    bool AuthenticationRequired,
    bool IsEnabled
);

/// <summary>
/// Template for external service integration.
/// </summary>
/// <param name="TemplateName">Name of the template.</param>
/// <param name="ServiceType">Type of service the template is for.</param>
/// <param name="IntegrationSteps">Steps for integration.</param>
/// <param name="ConfigurationTemplate">Template for configuration.</param>
public record IntegrationTemplate(
    string TemplateName,
    string ServiceType,
    IEnumerable<IntegrationStep> IntegrationSteps,
    Dictionary<string, object> ConfigurationTemplate
);

/// <summary>
/// Authentication provider for external services.
/// </summary>
/// <param name="Name">Name of the provider.</param>
/// <param name="ProviderType">Type of the provider.</param>
/// <param name="Configuration">Configuration for the provider.</param>
/// <param name="IsEnabled">Whether the provider is enabled.</param>
public record AuthenticationProvider(
    string Name,
    string ProviderType,
    Dictionary<string, object> Configuration,
    bool IsEnabled
);

/// <summary>
/// A step in external service integration.
/// </summary>
/// <param name="StepName">Name of the step.</param>
/// <param name="StepType">Type of the step.</param>
/// <param name="Description">Description of the step.</param>
/// <param name="RequiredParameters">Parameters required for the step.</param>
public record IntegrationStep(
    string StepName,
    string StepType,
    string Description,
    IEnumerable<string> RequiredParameters
);

/// <summary>
/// A validation rule for service chains.
/// </summary>
/// <param name="RuleName">Name of the rule.</param>
/// <param name="RuleType">Type of the rule.</param>
/// <param name="Condition">Condition for the rule.</param>
/// <param name="ErrorMessage">Error message if rule fails.</param>
public record ValidationRule(
    string RuleName,
    string RuleType,
    string Condition,
    string ErrorMessage
);