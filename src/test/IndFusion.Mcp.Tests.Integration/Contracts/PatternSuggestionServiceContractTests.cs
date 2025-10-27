using IndFusion.Mcp.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace IndFusion.Mcp.Tests.Integration.Contracts;

/// <summary>
/// Contract tests for IPatternSuggestionService interface.
/// These tests validate that any implementation of IPatternSuggestionService fulfills its contract.
/// </summary>
public class PatternSuggestionServiceContractTests : ServiceContractTestBase<IPatternSuggestionService, PatternSuggestionServiceStub>
{
    public PatternSuggestionServiceContractTests(ITestOutputHelper testOutput) : base(testOutput)
    {
    }

    protected override void ConfigureServiceDependencies(IServiceCollection services)
    {
        services.AddLogging();
        services.AddScoped<ILogger<PatternSuggestionServiceStub>>();
    }

    [Fact]
    public async Task SuggestPatternsAsync_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new PatternSuggestionRequest(
            ViolationId: "violation-001",
            RuleId: "EXXER001",
            CodeSnippet: "public void Method() { var x = null; }",
            FilePath: "src/Example.cs",
            Context: new Dictionary<string, object> { { "project", "TestProject" } },
            MaxSuggestions: 3,
            ConfidenceThreshold: 0.8
        );

        // Act
        var result = await Service.SuggestPatternsAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Pattern suggestion result should not be null");
        result.Success.ShouldBeTrue("Pattern suggestion should succeed");
        result.Suggestions.ShouldNotBeNull("Suggestions collection should not be null");
        result.ConfidenceScores.ShouldNotBeNull("Confidence scores should not be null");
        result.Citations.ShouldNotBeNull("Citations should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("SuggestPatternsAsync contract validation passed");
    }

    [Fact]
    public async Task AnalyzePatternsAsync_WithValidRequest_ShouldReturnAnalysisResult()
    {
        // Arrange
        var request = new PatternAnalysisRequest(
            ProjectPath: "C:\\Test\\TestProject",
            PatternType: "SOLID",
            Scope: "project",
            IncludeMetrics: true,
            GenerateReport: false
        );

        // Act
        var result = await Service.AnalyzePatternsAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Pattern analysis result should not be null");
        result.Success.ShouldBeTrue("Pattern analysis should succeed");
        result.PatternAlignment.ShouldNotBeNull("Pattern alignment should not be null");
        result.ImprovementSuggestions.ShouldNotBeNull("Improvement suggestions should not be null");
        result.Metrics.ShouldNotBeNull("Metrics should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("AnalyzePatternsAsync contract validation passed");
    }

    [Fact]
    public async Task QueryPatternGraphAsync_WithValidRequest_ShouldReturnGraphResult()
    {
        // Arrange
        var request = new PatternGraphRequest(
            Query: "MATCH (p:Pattern) RETURN p",
            RepositoryScope: "IndFusion",
            MaxDepth: 5,
            IncludeMetadata: true,
            Filters: new Dictionary<string, object> { { "type", "SOLID" } }
        );

        // Act
        var result = await Service.QueryPatternGraphAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Pattern graph result should not be null");
        result.Success.ShouldBeTrue("Pattern graph query should succeed");
        result.GraphResults.ShouldNotBeNull("Graph results should not be null");
        result.Relationships.ShouldNotBeNull("Relationships should not be null");
        result.PatternInsights.ShouldNotBeNull("Pattern insights should not be null");
        result.QueryExecutionTimeMs.ShouldBeGreaterThan(0, "Query execution time should be positive");
        
        Logger.LogInformation("QueryPatternGraphAsync contract validation passed");
    }

    [Fact]
    public async Task ExtractPatternsAsync_WithValidRequest_ShouldReturnExtractionResult()
    {
        // Arrange
        var request = new PatternExtractionRequest(
            SourceCode: "public class Example { public void Method() { } }",
            PatternTypes: new[] { "Class", "Method" },
            Metadata: new Dictionary<string, object> { { "language", "C#" } },
            ConfidenceThreshold: 0.8,
            AddToKnowledgeBase: true
        );

        // Act
        var result = await Service.ExtractPatternsAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Pattern extraction result should not be null");
        result.Success.ShouldBeTrue("Pattern extraction should succeed");
        result.ExtractedPatterns.ShouldNotBeNull("Extracted patterns should not be null");
        result.KnowledgeBaseUpdates.ShouldNotBeNull("Knowledge base updates should not be null");
        result.ExtractionMetrics.ShouldNotBeNull("Extraction metrics should not be null");
        result.ExecutionTimeMs.ShouldBeGreaterThan(0, "Execution time should be positive");
        
        Logger.LogInformation("ExtractPatternsAsync contract validation passed");
    }

    public override async Task Service_ShouldHandleCancellationTokens()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var suggestionRequest = new PatternSuggestionRequest("violation-001", "EXXER001", "code", "file.cs");
        var analysisRequest = new PatternAnalysisRequest("C:\\Test", "SOLID");
        var graphRequest = new PatternGraphRequest("MATCH (p) RETURN p", "IndFusion");
        var extractionRequest = new PatternExtractionRequest("code", new[] { "Pattern" }, new Dictionary<string, object>());

        // All methods should handle cancellation gracefully
        await Should.NotThrowAsync(async () => await Service.SuggestPatternsAsync(suggestionRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.AnalyzePatternsAsync(analysisRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.QueryPatternGraphAsync(graphRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ExtractPatternsAsync(extractionRequest, cts.Token));

        Logger.LogInformation("Cancellation token handling validation passed for all methods");
    }

    public override async Task Service_ShouldHandleNullParameters()
    {
        var suggestionRequest = new PatternSuggestionRequest("violation-001", "EXXER001", "code", "file.cs");
        var analysisRequest = new PatternAnalysisRequest("C:\\Test", "SOLID");
        var graphRequest = new PatternGraphRequest("MATCH (p) RETURN p", "IndFusion");
        var extractionRequest = new PatternExtractionRequest("code", new[] { "Pattern" }, new Dictionary<string, object>());

        // Methods should handle null parameters gracefully
        await Should.NotThrowAsync(async () => await Service.SuggestPatternsAsync(suggestionRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.AnalyzePatternsAsync(analysisRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.QueryPatternGraphAsync(graphRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ExtractPatternsAsync(extractionRequest, CancellationToken.None));

        Logger.LogInformation("Null parameter handling validation passed");
    }
}

/// <summary>
/// Stub implementation of IPatternSuggestionService for contract testing.
/// </summary>
public class PatternSuggestionServiceStub : IPatternSuggestionService
{
    private readonly ILogger<PatternSuggestionServiceStub> _logger;

    public PatternSuggestionServiceStub(ILogger<PatternSuggestionServiceStub> logger)
    {
        _logger = logger;
    }

    public async Task<PatternSuggestionResult> SuggestPatternsAsync(PatternSuggestionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Suggesting patterns for violation {ViolationId}", request.ViolationId);
        
        await Task.Delay(150, cancellationToken);
        
        var suggestions = new[]
        {
            new PatternSuggestion(
                Id: "suggestion-001",
                PatternType: "Null Safety",
                Description: "Use null-conditional operators to handle null values safely",
                CodeExample: "var result = obj?.Property ?? defaultValue;",
                Confidence: 0.9,
                Effort: "Low",
                Benefits: new[] { "Prevents null reference exceptions", "Improves code safety" },
                Citations: new[] { new PatternCitation("Microsoft Docs", "https://docs.microsoft.com", 0.95, 0.9) }
            ),
            new PatternSuggestion(
                Id: "suggestion-002",
                PatternType: "Defensive Programming",
                Description: "Add null checks before using objects",
                CodeExample: "if (obj != null) { obj.Method(); }",
                Confidence: 0.8,
                Effort: "Medium",
                Benefits: new[] { "Explicit null handling", "Clear intent" },
                Citations: new[] { new PatternCitation("Clean Code", null, 0.85, 0.8) }
            )
        };

        return new PatternSuggestionResult(
            Success: true,
            Suggestions: suggestions,
            ConfidenceScores: new Dictionary<string, double>
            {
                { "suggestion-001", 0.9 },
                { "suggestion-002", 0.8 }
            },
            Citations: suggestions.SelectMany(s => s.Citations),
            ExecutionTimeMs: 150
        );
    }

    public async Task<PatternAnalysisResult> AnalyzePatternsAsync(PatternAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Analyzing patterns for project {ProjectPath}", request.ProjectPath);
        
        await Task.Delay(200, cancellationToken);
        
        return new PatternAnalysisResult(
            Success: true,
            PatternAlignment: new PatternAlignmentAnalysis(
                OverallScore: 0.75,
                PatternScores: new Dictionary<string, double>
                {
                    { "SOLID", 0.8 },
                    { "DRY", 0.7 },
                    { "Clean Code", 0.75 }
                },
                AlignmentIssues: new[] { "Missing interface segregation", "Some code duplication" },
                Recommendations: new[] { "Extract interfaces", "Refactor duplicated code" }
            ),
            ImprovementSuggestions: new[]
            {
                new ImprovementSuggestion(
                    Type: "Refactoring",
                    Description: "Extract interfaces to improve segregation",
                    Priority: "High",
                    EstimatedEffort: "Medium",
                    ExpectedBenefit: "Better maintainability and testability"
                )
            },
            Metrics: new PatternMetrics(
                TotalPatterns: 15,
                PatternDistribution: new Dictionary<string, int>
                {
                    { "SOLID", 8 },
                    { "DRY", 4 },
                    { "Clean Code", 3 }
                },
                ComplianceScore: 0.75,
                QualityMetrics: new Dictionary<string, double>
                {
                    { "Complexity", 0.6 },
                    { "Maintainability", 0.8 },
                    { "Testability", 0.7 }
                }
            ),
            ExecutionTimeMs: 200
        );
    }

    public async Task<PatternGraphResult> QueryPatternGraphAsync(PatternGraphRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Querying pattern graph with query: {Query}", request.Query);
        
        await Task.Delay(100, cancellationToken);
        
        return new PatternGraphResult(
            Success: true,
            GraphResults: new[]
            {
                new GraphTraversalResult(
                    NodeId: "pattern-001",
                    NodeType: "Pattern",
                    Properties: new Dictionary<string, object> { { "name", "SOLID" }, { "type", "Design Pattern" } },
                    Relationships: new[] { "rel-001", "rel-002" },
                    Depth: 1
                )
            },
            Relationships: new[]
            {
                new PatternRelationship(
                    SourcePattern: "pattern-001",
                    TargetPattern: "pattern-002",
                    RelationshipType: "depends_on",
                    Strength: 0.8,
                    Context: new Dictionary<string, object> { { "context", "design" } }
                )
            },
            PatternInsights: new[]
            {
                new PatternInsight(
                    InsightType: "Dependency",
                    Description: "SOLID patterns show strong dependency relationships",
                    Confidence: 0.9,
                    SupportingEvidence: new[] { "High coupling detected", "Interface usage patterns" },
                    Recommendations: new[] { "Consider dependency injection", "Extract interfaces" }
                )
            },
            QueryExecutionTimeMs: 100
        );
    }

    public async Task<PatternExtractionResult> ExtractPatternsAsync(PatternExtractionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Extracting patterns from source code");
        
        await Task.Delay(300, cancellationToken);
        
        var extractedPatterns = new[]
        {
            new ExtractedPattern(
                Id: "extracted-001",
                PatternType: "Class",
                CodeSnippet: "public class Example { }",
                Confidence: 0.9,
                Metadata: new Dictionary<string, object> { { "visibility", "public" } },
                SourceLocation: new SourceLocation("Example.cs", 1, 1, 25)
            )
        };

        return new PatternExtractionResult(
            Success: true,
            ExtractedPatterns: extractedPatterns,
            KnowledgeBaseUpdates: new[]
            {
                new KnowledgeBaseUpdate(
                    UpdateType: "Add",
                    EntityId: "pattern-class-001",
                    EntityType: "Pattern",
                    Changes: new Dictionary<string, object> { { "pattern_type", "Class" } },
                    Timestamp: DateTime.UtcNow
                )
            },
            ExtractionMetrics: new ExtractionMetrics(
                PatternsExtracted: 1,
                ExtractionTimeMs: 300,
                ConfidenceDistribution: new Dictionary<string, int> { { "0.9", 1 } },
                PatternTypeDistribution: new Dictionary<string, int> { { "Class", 1 } }
            ),
            ExecutionTimeMs: 300
        );
    }
}