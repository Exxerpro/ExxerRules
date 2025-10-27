using IndFusion.Mcp.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace IndFusion.Mcp.Tests.Integration.Contracts;

/// <summary>
/// Contract tests for IMcpToolCompositionService interface.
/// These tests validate that any implementation of IMcpToolCompositionService fulfills its contract.
/// </summary>
public class McpToolCompositionServiceContractTests : ServiceContractTestBase<IMcpToolCompositionService, McpToolCompositionServiceStub>
{
    public McpToolCompositionServiceContractTests(ITestOutputHelper testOutput) : base(testOutput)
    {
    }

    protected override void ConfigureServiceDependencies(IServiceCollection services)
    {
        services.AddLogging();
        services.AddScoped<ILogger<McpToolCompositionServiceStub>>();
    }

    [Fact]
    public async Task ComposeLintRunToolAsync_WithValidRequest_ShouldReturnComposedResult()
    {
        // Arrange
        var request = new LintingRequest(
            ProjectPath: "C:\\Test\\TestProject",
            RunAnalyzers: true,
            IncludeWarnings: true,
            OutputFormat: "json",
            MaxIssues: 100
        );

        // Act
        var result = await Service.ComposeLintRunToolAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Composed tool result should not be null");
        result.Success.ShouldBeTrue("Lint run tool composition should succeed");
        result.ToolName.ShouldBe("lint_run", "Tool name should be correct");
        result.Result.ShouldNotBeNull("Tool result should not be null");
        result.CompositionDetails.ShouldNotBeNull("Composition details should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("ComposeLintRunToolAsync contract validation passed");
    }

    [Fact]
    public async Task ComposePatternSuggestToolAsync_WithValidRequest_ShouldReturnComposedResult()
    {
        // Arrange
        var request = new PatternSuggestionRequest(
            ViolationId: "violation-001",
            RuleId: "EXXER001",
            CodeSnippet: "public void Method() { }",
            FilePath: "src/Example.cs",
            MaxSuggestions: 5,
            ConfidenceThreshold: 0.8
        );

        // Act
        var result = await Service.ComposePatternSuggestToolAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Composed tool result should not be null");
        result.Success.ShouldBeTrue("Pattern suggest tool composition should succeed");
        result.ToolName.ShouldBe("pattern_suggest", "Tool name should be correct");
        result.Result.ShouldNotBeNull("Tool result should not be null");
        result.CompositionDetails.ShouldNotBeNull("Composition details should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("ComposePatternSuggestToolAsync contract validation passed");
    }

    [Fact]
    public async Task ComposeFixer001ApplyToolAsync_WithValidRequest_ShouldReturnComposedResult()
    {
        // Arrange
        var request = new Fixer001Request(
            DiagnosticId: "EXXER001",
            TargetFiles: new[] { "src/Example.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: false,
            BackupOriginal: true,
            MaxFixesPerFile: 10
        );

        // Act
        var result = await Service.ComposeFixer001ApplyToolAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Composed tool result should not be null");
        result.Success.ShouldBeTrue("Fixer001 apply tool composition should succeed");
        result.ToolName.ShouldBe("fixer001_apply", "Tool name should be correct");
        result.Result.ShouldNotBeNull("Tool result should not be null");
        result.CompositionDetails.ShouldNotBeNull("Composition details should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("ComposeFixer001ApplyToolAsync contract validation passed");
    }

    [Fact]
    public async Task ComposeSafeRegexReplaceToolAsync_WithValidRequest_ShouldReturnComposedResult()
    {
        // Arrange
        var request = new SafeRegexRequest(
            Pattern: @"public\s+void\s+(\w+)\s*\(",
            Replacement: "public async Task $1Async(",
            TargetFiles: new[] { "src/Example.cs" },
            ValidationOptions: new TransformationValidationOptions(),
            DryRun: true,
            CaseSensitive: true,
            Multiline: false
        );

        // Act
        var result = await Service.ComposeSafeRegexReplaceToolAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Composed tool result should not be null");
        result.Success.ShouldBeTrue("Safe regex replace tool composition should succeed");
        result.ToolName.ShouldBe("safe_regex_replace", "Tool name should be correct");
        result.Result.ShouldNotBeNull("Tool result should not be null");
        result.CompositionDetails.ShouldNotBeNull("Composition details should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("ComposeSafeRegexReplaceToolAsync contract validation passed");
    }

    [Fact]
    public async Task ComposeKnowledgeRagToolAsync_WithValidRequest_ShouldReturnComposedResult()
    {
        // Arrange
        var request = new RagQueryRequest(
            Question: "What are the best practices for async programming in C#?",
            Context: new Dictionary<string, object> { { "language", "C#" } },
            MaxTokens: 500,
            Temperature: 0.7,
            IncludeSources: true
        );

        // Act
        var result = await Service.ComposeKnowledgeRagToolAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Composed tool result should not be null");
        result.Success.ShouldBeTrue("Knowledge RAG tool composition should succeed");
        result.ToolName.ShouldBe("knowledge_rag", "Tool name should be correct");
        result.Result.ShouldNotBeNull("Tool result should not be null");
        result.CompositionDetails.ShouldNotBeNull("Composition details should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("ComposeKnowledgeRagToolAsync contract validation passed");
    }

    [Fact]
    public async Task GetCompositionConfigurationAsync_ShouldReturnValidConfiguration()
    {
        // Act
        var configuration = await Service.GetCompositionConfigurationAsync(CreateTestCancellationToken());

        // Assert
        configuration.ShouldNotBeNull("Composition configuration should not be null");
        configuration.AvailableTools.ShouldNotBeNull("Available tools should not be null");
        configuration.CompositionStrategies.ShouldNotBeNull("Composition strategies should not be null");
        configuration.ServiceMappings.ShouldNotBeNull("Service mappings should not be null");
        configuration.PerformanceSettings.ShouldNotBeNull("Performance settings should not be null");
        configuration.Version.ShouldNotBeNullOrEmpty("Version should not be null or empty");
        configuration.LastUpdated.ShouldBeLessThanOrEqualTo(DateTime.UtcNow, "Last updated should be in the past");
        
        Logger.LogInformation("GetCompositionConfigurationAsync contract validation passed");
    }

    public override async Task Service_ShouldHandleCancellationTokens()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var lintRequest = new LintingRequest("C:\\Test", true, true, "json", 100);
        var patternRequest = new PatternSuggestionRequest("violation-001", "EXXER001", "code", "file.cs");
        var fixerRequest = new Fixer001Request("EXXER001", new[] { "test.cs" }, new TransformationValidationOptions());
        var regexRequest = new SafeRegexRequest("pattern", "replacement", new[] { "test.cs" }, new TransformationValidationOptions());
        var ragRequest = new RagQueryRequest("question");

        // All methods should handle cancellation gracefully
        await Should.NotThrowAsync(async () => await Service.ComposeLintRunToolAsync(lintRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ComposePatternSuggestToolAsync(patternRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ComposeFixer001ApplyToolAsync(fixerRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ComposeSafeRegexReplaceToolAsync(regexRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ComposeKnowledgeRagToolAsync(ragRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.GetCompositionConfigurationAsync(cts.Token));

        Logger.LogInformation("Cancellation token handling validation passed for all methods");
    }

    public override async Task Service_ShouldHandleNullParameters()
    {
        var lintRequest = new LintingRequest("C:\\Test", true, true, "json", 100);
        var patternRequest = new PatternSuggestionRequest("violation-001", "EXXER001", "code", "file.cs");
        var fixerRequest = new Fixer001Request("EXXER001", new[] { "test.cs" }, new TransformationValidationOptions());
        var regexRequest = new SafeRegexRequest("pattern", "replacement", new[] { "test.cs" }, new TransformationValidationOptions());
        var ragRequest = new RagQueryRequest("question");

        // Methods should handle null parameters gracefully
        await Should.NotThrowAsync(async () => await Service.ComposeLintRunToolAsync(lintRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ComposePatternSuggestToolAsync(patternRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ComposeFixer001ApplyToolAsync(fixerRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ComposeSafeRegexReplaceToolAsync(regexRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ComposeKnowledgeRagToolAsync(ragRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.GetCompositionConfigurationAsync(CancellationToken.None));

        Logger.LogInformation("Null parameter handling validation passed");
    }
}

/// <summary>
/// Stub implementation of IMcpToolCompositionService for contract testing.
/// </summary>
public class McpToolCompositionServiceStub : IMcpToolCompositionService
{
    private readonly ILogger<McpToolCompositionServiceStub> _logger;

    public McpToolCompositionServiceStub(ILogger<McpToolCompositionServiceStub> logger)
    {
        _logger = logger;
    }

    public async Task<McpToolResult> ComposeLintRunToolAsync(LintingRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Composing lint_run tool for project: {ProjectPath}", request.ProjectPath);
        
        await Task.Delay(200, cancellationToken);
        
        return new McpToolResult(
            Success: true,
            ToolName: "lint_run",
            Result: new LintingResult(
                Success: true,
                Issues: new List<LintingIssue>(),
                Metrics: new LintingMetrics(0, 0, 0, 0),
                ExecutionTimeMs: 200
            ),
            CompositionDetails: new CompositionDetails(
                ServicesUsed: new[] { "LintingService", "ValidationService" },
                CompositionStrategy: "Sequential",
                Dependencies: new[]
                {
                    new ServiceDependency("LintingService", "ValidationService", "DataFlow", "Issues")
                },
                PerformanceMetrics: new CompositionPerformanceMetrics(
                    TotalExecutionTimeMs: 200,
                    ServiceExecutionTimes: new Dictionary<string, long>
                    {
                        { "LintingService", 150 },
                        { "ValidationService", 50 }
                    },
                    OverheadTimeMs: 20,
                    MemoryUsage: 1024 * 1024
                )
            ),
            ExecutionTimeMs: 200
        );
    }

    public async Task<McpToolResult> ComposePatternSuggestToolAsync(PatternSuggestionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Composing pattern_suggest tool for violation: {ViolationId}", request.ViolationId);
        
        await Task.Delay(250, cancellationToken);
        
        return new McpToolResult(
            Success: true,
            ToolName: "pattern_suggest",
            Result: new PatternSuggestionResult(
                Success: true,
                Suggestions: new List<PatternSuggestion>(),
                ConfidenceScores: new Dictionary<string, double>(),
                Citations: new List<PatternCitation>(),
                ExecutionTimeMs: 250
            ),
            CompositionDetails: new CompositionDetails(
                ServicesUsed: new[] { "PatternSuggestionService", "KnowledgeRagService" },
                CompositionStrategy: "Parallel",
                Dependencies: new[]
                {
                    new ServiceDependency("PatternSuggestionService", "KnowledgeRagService", "DataFlow", "Patterns")
                },
                PerformanceMetrics: new CompositionPerformanceMetrics(
                    TotalExecutionTimeMs: 250,
                    ServiceExecutionTimes: new Dictionary<string, long>
                    {
                        { "PatternSuggestionService", 200 },
                        { "KnowledgeRagService", 180 }
                    },
                    OverheadTimeMs: 30,
                    MemoryUsage: 2 * 1024 * 1024
                )
            ),
            ExecutionTimeMs: 250
        );
    }

    public async Task<McpToolResult> ComposeFixer001ApplyToolAsync(Fixer001Request request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Composing fixer001_apply tool for diagnostic: {DiagnosticId}", request.DiagnosticId);
        
        await Task.Delay(300, cancellationToken);
        
        return new McpToolResult(
            Success: true,
            ToolName: "fixer001_apply",
            Result: new CodeTransformationResult(
                Success: true,
                TransformationDetails: new TransformationDetails(
                    TransformationType: "Fixer001",
                    TransformationId: "fixer-001",
                    Description: "Applied Fixer001 transformation",
                    ChangesApplied: 3,
                    FilesAffected: request.TargetFiles.Count(),
                    Confidence: 0.95
                ),
                ValidationResults: new List<ValidationResult>(),
                ExecutionTimeMs: 300
            ),
            CompositionDetails: new CompositionDetails(
                ServicesUsed: new[] { "CodeTransformationService", "ValidationService", "BackupService" },
                CompositionStrategy: "Sequential",
                Dependencies: new[]
                {
                    new ServiceDependency("CodeTransformationService", "ValidationService", "DataFlow", "TransformedCode"),
                    new ServiceDependency("CodeTransformationService", "BackupService", "DataFlow", "OriginalCode")
                },
                PerformanceMetrics: new CompositionPerformanceMetrics(
                    TotalExecutionTimeMs: 300,
                    ServiceExecutionTimes: new Dictionary<string, long>
                    {
                        { "CodeTransformationService", 200 },
                        { "ValidationService", 80 },
                        { "BackupService", 20 }
                    },
                    OverheadTimeMs: 40,
                    MemoryUsage: 3 * 1024 * 1024
                )
            ),
            ExecutionTimeMs: 300
        );
    }

    public async Task<McpToolResult> ComposeSafeRegexReplaceToolAsync(SafeRegexRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Composing safe_regex_replace tool with pattern: {Pattern}", request.Pattern);
        
        await Task.Delay(180, cancellationToken);
        
        return new McpToolResult(
            Success: true,
            ToolName: "safe_regex_replace",
            Result: new CodeTransformationResult(
                Success: true,
                TransformationDetails: new TransformationDetails(
                    TransformationType: "SafeRegex",
                    TransformationId: "regex-001",
                    Description: "Applied safe regex transformation",
                    ChangesApplied: 2,
                    FilesAffected: request.TargetFiles.Count(),
                    Confidence: 0.9
                ),
                ValidationResults: new List<ValidationResult>(),
                ExecutionTimeMs: 180
            ),
            CompositionDetails: new CompositionDetails(
                ServicesUsed: new[] { "CodeTransformationService", "ValidationService" },
                CompositionStrategy: "Sequential",
                Dependencies: new[]
                {
                    new ServiceDependency("CodeTransformationService", "ValidationService", "DataFlow", "TransformedCode")
                },
                PerformanceMetrics: new CompositionPerformanceMetrics(
                    TotalExecutionTimeMs: 180,
                    ServiceExecutionTimes: new Dictionary<string, long>
                    {
                        { "CodeTransformationService", 150 },
                        { "ValidationService", 30 }
                    },
                    OverheadTimeMs: 25,
                    MemoryUsage: 1.5 * 1024 * 1024
                )
            ),
            ExecutionTimeMs: 180
        );
    }

    public async Task<McpToolResult> ComposeKnowledgeRagToolAsync(RagQueryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Composing knowledge_rag tool for question: {Question}", request.Question);
        
        await Task.Delay(400, cancellationToken);
        
        return new McpToolResult(
            Success: true,
            ToolName: "knowledge_rag",
            Result: new RagQueryResult(
                Success: true,
                Answer: "Generated answer from RAG system",
                Sources: new List<RagSource>(),
                Confidence: 0.9,
                TokensUsed: 150,
                GenerationTimeMs: 400
            ),
            CompositionDetails: new CompositionDetails(
                ServicesUsed: new[] { "KnowledgeRagService", "VectorStoreService", "EmbeddingService" },
                CompositionStrategy: "Parallel",
                Dependencies: new[]
                {
                    new ServiceDependency("KnowledgeRagService", "VectorStoreService", "DataFlow", "Embeddings"),
                    new ServiceDependency("KnowledgeRagService", "EmbeddingService", "DataFlow", "QueryEmbedding")
                },
                PerformanceMetrics: new CompositionPerformanceMetrics(
                    TotalExecutionTimeMs: 400,
                    ServiceExecutionTimes: new Dictionary<string, long>
                    {
                        { "KnowledgeRagService", 300 },
                        { "VectorStoreService", 250 },
                        { "EmbeddingService", 100 }
                    },
                    OverheadTimeMs: 50,
                    MemoryUsage: 4 * 1024 * 1024
                )
            ),
            ExecutionTimeMs: 400
        );
    }

    public async Task<McpToolCompositionConfiguration> GetCompositionConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting MCP tool composition configuration");
        
        await Task.Delay(50, cancellationToken);
        
        return new McpToolCompositionConfiguration(
            AvailableTools: new[]
            {
                new McpToolInfo(
                    Name: "lint_run",
                    Description: "Runs linting analysis on code",
                    RequiredServices: new[] { "LintingService", "ValidationService" },
                    Parameters: new Dictionary<string, object> { { "projectPath", "string" } },
                    IsEnabled: true
                ),
                new McpToolInfo(
                    Name: "pattern_suggest",
                    Description: "Suggests patterns for code improvements",
                    RequiredServices: new[] { "PatternSuggestionService", "KnowledgeRagService" },
                    Parameters: new Dictionary<string, object> { { "violationId", "string" } },
                    IsEnabled: true
                ),
                new McpToolInfo(
                    Name: "fixer001_apply",
                    Description: "Applies Fixer001 transformations to code",
                    RequiredServices: new[] { "CodeTransformationService", "ValidationService" },
                    Parameters: new Dictionary<string, object> { { "diagnosticId", "string" } },
                    IsEnabled: true
                ),
                new McpToolInfo(
                    Name: "safe_regex_replace",
                    Description: "Safely applies regex transformations to code",
                    RequiredServices: new[] { "CodeTransformationService", "ValidationService" },
                    Parameters: new Dictionary<string, object> { { "pattern", "string" } },
                    IsEnabled: true
                ),
                new McpToolInfo(
                    Name: "knowledge_rag",
                    Description: "Queries the knowledge base using RAG",
                    RequiredServices: new[] { "KnowledgeRagService", "VectorStoreService" },
                    Parameters: new Dictionary<string, object> { { "question", "string" } },
                    IsEnabled: true
                )
            },
            CompositionStrategies: new[]
            {
                new CompositionStrategy(
                    Name: "Sequential",
                    Description: "Execute services in sequence",
                    ServiceOrder: new[] { "Service1", "Service2", "Service3" },
                    DependencyRules: new[]
                    {
                        new DependencyRule("Rule1", "Service1 completes", "Start Service2", 1)
                    },
                    IsDefault: true
                ),
                new CompositionStrategy(
                    Name: "Parallel",
                    Description: "Execute services in parallel",
                    ServiceOrder: new[] { "Service1", "Service2" },
                    DependencyRules: new[]
                    {
                        new DependencyRule("Rule1", "All services ready", "Start all services", 1)
                    },
                    IsDefault: false
                )
            },
            ServiceMappings: new Dictionary<string, IEnumerable<string>>
            {
                { "lint_run", new[] { "LintingService", "ValidationService" } },
                { "pattern_suggest", new[] { "PatternSuggestionService", "KnowledgeRagService" } },
                { "fixer001_apply", new[] { "CodeTransformationService", "ValidationService" } },
                { "safe_regex_replace", new[] { "CodeTransformationService", "ValidationService" } },
                { "knowledge_rag", new[] { "KnowledgeRagService", "VectorStoreService" } }
            },
            PerformanceSettings: new CompositionPerformanceSettings(
                MaxExecutionTimeMs: 30000,
                MaxMemoryUsage: 100 * 1024 * 1024,
                TimeoutMs: 30000,
                RetryAttempts: 3
            ),
            Version: "1.0.0",
            LastUpdated: DateTime.UtcNow
        );
    }
}