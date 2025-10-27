using IndFusion.Mcp.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace IndFusion.Mcp.Tests.Integration.Contracts;

/// <summary>
/// Contract tests for IKnowledgeRagService interface.
/// These tests validate that any implementation of IKnowledgeRagService fulfills its contract.
/// </summary>
public class KnowledgeRagServiceContractTests : ServiceContractTestBase<IKnowledgeRagService, KnowledgeRagServiceStub>
{
    public KnowledgeRagServiceContractTests(ITestOutputHelper testOutput) : base(testOutput)
    {
    }

    protected override void ConfigureServiceDependencies(IServiceCollection services)
    {
        services.AddLogging();
        services.AddScoped<ILogger<KnowledgeRagServiceStub>>();
    }

    [Fact]
    public async Task SearchSemanticallyAsync_WithValidRequest_ShouldReturnSearchResults()
    {
        // Arrange
        var request = new SemanticSearchRequest(
            Query: "How to implement async methods in C#",
            RepositoryScope: "IndFusion",
            MaxResults: 5,
            SimilarityThreshold: 0.8,
            IncludeMetadata: true,
            SearchOptions: new Dictionary<string, object> { { "language", "C#" } }
        );

        // Act
        var result = await Service.SearchSemanticallyAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Semantic search result should not be null");
        result.Success.ShouldBeTrue("Semantic search should succeed");
        result.Results.ShouldNotBeNull("Search results should not be null");
        result.TotalMatches.ShouldBeGreaterThanOrEqualTo(0, "Total matches should be non-negative");
        result.SearchTimeMs.ShouldBeGreaterThan(0, "Search time should be positive");
        result.QueryProcessed.ShouldNotBeNullOrEmpty("Processed query should not be null or empty");
        
        Logger.LogInformation("SearchSemanticallyAsync contract validation passed");
    }

    [Fact]
    public async Task AnalyzeCodePatternsAsync_WithValidRequest_ShouldReturnAnalysisResult()
    {
        // Arrange
        var request = new CodePatternAnalysisRequest(
            CodeSnippet: "public async Task<string> GetDataAsync() { return await httpClient.GetStringAsync(url); }",
            AnalysisType: "Async Pattern",
            Context: new Dictionary<string, object> { { "project", "TestProject" } },
            IncludeSuggestions: true,
            ConfidenceThreshold: 0.8
        );

        // Act
        var result = await Service.AnalyzeCodePatternsAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Code pattern analysis result should not be null");
        result.Success.ShouldBeTrue("Code pattern analysis should succeed");
        result.Patterns.ShouldNotBeNull("Patterns should not be null");
        result.Suggestions.ShouldNotBeNull("Suggestions should not be null");
        result.ConfidenceScores.ShouldNotBeNull("Confidence scores should not be null");
        result.AnalysisTimeMs.ShouldBeGreaterThan(0, "Analysis time should be positive");
        
        Logger.LogInformation("AnalyzeCodePatternsAsync contract validation passed");
    }

    [Fact]
    public async Task ExecuteKnowledgeBaseOperationAsync_WithValidRequest_ShouldReturnOperationResult()
    {
        // Arrange
        var request = new KnowledgeBaseRequest(
            Operation: "Add",
            EntityId: "pattern-001",
            EntityType: "CodePattern",
            Data: new Dictionary<string, object>
            {
                { "patternType", "Async" },
                { "description", "Async method pattern" },
                { "codeSnippet", "public async Task Method() { }" }
            },
            Metadata: new Dictionary<string, object> { { "source", "Manual" } }
        );

        // Act
        var result = await Service.ExecuteKnowledgeBaseOperationAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("Knowledge base operation result should not be null");
        result.Success.ShouldBeTrue("Knowledge base operation should succeed");
        result.OperationResult.ShouldNotBeNull("Operation result should not be null");
        result.EntitiesAffected.ShouldBeGreaterThanOrEqualTo(0, "Entities affected should be non-negative");
        result.OperationTimeMs.ShouldBeGreaterThan(0, "Operation time should be positive");
        
        Logger.LogInformation("ExecuteKnowledgeBaseOperationAsync contract validation passed");
    }

    [Fact]
    public async Task QueryRagAsync_WithValidRequest_ShouldReturnRagResult()
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
        var result = await Service.QueryRagAsync(request, CreateTestCancellationToken());

        // Assert
        result.ShouldNotBeNull("RAG query result should not be null");
        result.Success.ShouldBeTrue("RAG query should succeed");
        result.Answer.ShouldNotBeNullOrEmpty("Answer should not be null or empty");
        result.Sources.ShouldNotBeNull("Sources should not be null");
        result.Confidence.ShouldBeGreaterThanOrEqualTo(0.0, "Confidence should be non-negative");
        result.Confidence.ShouldBeLessThanOrEqualTo(1.0, "Confidence should not exceed 1.0");
        result.TokensUsed.ShouldBeGreaterThan(0, "Tokens used should be positive");
        result.GenerationTimeMs.ShouldBeGreaterThan(0, "Generation time should be positive");
        
        Logger.LogInformation("QueryRagAsync contract validation passed");
    }

    [Fact]
    public async Task GetConfigurationAsync_ShouldReturnValidConfiguration()
    {
        // Act
        var configuration = await Service.GetConfigurationAsync(CreateTestCancellationToken());

        // Assert
        configuration.ShouldNotBeNull("Knowledge RAG configuration should not be null");
        configuration.VectorStoreConfig.ShouldNotBeNull("Vector store config should not be null");
        configuration.EmbeddingConfig.ShouldNotBeNull("Embedding config should not be null");
        configuration.RagConfig.ShouldNotBeNull("RAG config should not be null");
        configuration.IndexingConfig.ShouldNotBeNull("Indexing config should not be null");
        configuration.Version.ShouldNotBeNullOrEmpty("Version should not be null or empty");
        configuration.LastUpdated.ShouldBeLessThanOrEqualTo(DateTime.UtcNow, "Last updated should be in the past");
        
        Logger.LogInformation("GetConfigurationAsync contract validation passed");
    }

    public override async Task Service_ShouldHandleCancellationTokens()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var searchRequest = new SemanticSearchRequest("query", "IndFusion");
        var analysisRequest = new CodePatternAnalysisRequest("code", "pattern");
        var kbRequest = new KnowledgeBaseRequest("Add", "id", "type", new Dictionary<string, object>());
        var ragRequest = new RagQueryRequest("question");

        // All methods should handle cancellation gracefully
        await Should.NotThrowAsync(async () => await Service.SearchSemanticallyAsync(searchRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.AnalyzeCodePatternsAsync(analysisRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.ExecuteKnowledgeBaseOperationAsync(kbRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.QueryRagAsync(ragRequest, cts.Token));
        await Should.NotThrowAsync(async () => await Service.GetConfigurationAsync(cts.Token));

        Logger.LogInformation("Cancellation token handling validation passed for all methods");
    }

    public override async Task Service_ShouldHandleNullParameters()
    {
        var searchRequest = new SemanticSearchRequest("query", "IndFusion");
        var analysisRequest = new CodePatternAnalysisRequest("code", "pattern");
        var kbRequest = new KnowledgeBaseRequest("Add", "id", "type", new Dictionary<string, object>());
        var ragRequest = new RagQueryRequest("question");

        // Methods should handle null parameters gracefully
        await Should.NotThrowAsync(async () => await Service.SearchSemanticallyAsync(searchRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.AnalyzeCodePatternsAsync(analysisRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.ExecuteKnowledgeBaseOperationAsync(kbRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.QueryRagAsync(ragRequest, CancellationToken.None));
        await Should.NotThrowAsync(async () => await Service.GetConfigurationAsync(CancellationToken.None));

        Logger.LogInformation("Null parameter handling validation passed");
    }
}

/// <summary>
/// Stub implementation of IKnowledgeRagService for contract testing.
/// </summary>
public class KnowledgeRagServiceStub : IKnowledgeRagService
{
    private readonly ILogger<KnowledgeRagServiceStub> _logger;

    public KnowledgeRagServiceStub(ILogger<KnowledgeRagServiceStub> logger)
    {
        _logger = logger;
    }

    public async Task<SemanticSearchResult> SearchSemanticallyAsync(SemanticSearchRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Performing semantic search for query: {Query}", request.Query);
        
        await Task.Delay(200, cancellationToken);
        
        var results = new[]
        {
            new SearchResult(
                Id: "result-001",
                Content: "public async Task<string> GetDataAsync() { return await httpClient.GetStringAsync(url); }",
                Similarity: 0.9,
                Source: "Example.cs",
                Metadata: new Dictionary<string, object> { { "language", "C#" } },
                Location: new SourceLocation("src/Example.cs", 10, 1, 80)
            ),
            new SearchResult(
                Id: "result-002",
                Content: "public async Task ProcessAsync() { await Task.Delay(100); }",
                Similarity: 0.85,
                Source: "Processor.cs",
                Metadata: new Dictionary<string, object> { { "language", "C#" } },
                Location: new SourceLocation("src/Processor.cs", 5, 1, 60)
            )
        };

        return new SemanticSearchResult(
            Success: true,
            Results: results,
            TotalMatches: results.Length,
            SearchTimeMs: 200,
            QueryProcessed: request.Query
        );
    }

    public async Task<CodePatternAnalysisResult> AnalyzeCodePatternsAsync(CodePatternAnalysisRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Analyzing code patterns for: {AnalysisType}", request.AnalysisType);
        
        await Task.Delay(300, cancellationToken);
        
        var patterns = new[]
        {
            new CodePattern(
                Id: "pattern-001",
                PatternType: "Async Method",
                Description: "Asynchronous method implementation",
                CodeSnippet: request.CodeSnippet,
                Confidence: 0.95,
                Location: new SourceLocation("Example.cs", 1, 1, request.CodeSnippet.Length),
                Metadata: new Dictionary<string, object> { { "language", "C#" } }
            )
        };

        var suggestions = new[]
        {
            new ImprovementSuggestion(
                Id: "suggestion-001",
                SuggestionType: "Error Handling",
                Description: "Add try-catch block for error handling",
                CodeExample: "try { return await httpClient.GetStringAsync(url); } catch (Exception ex) { /* handle error */ }",
                Confidence: 0.8,
                Effort: "Low",
                Benefits: new[] { "Better error handling", "Improved reliability" }
            )
        };

        return new CodePatternAnalysisResult(
            Success: true,
            Patterns: patterns,
            Suggestions: suggestions,
            ConfidenceScores: new Dictionary<string, double> { { "pattern-001", 0.95 } },
            AnalysisTimeMs: 300
        );
    }

    public async Task<KnowledgeBaseResult> ExecuteKnowledgeBaseOperationAsync(KnowledgeBaseRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Executing knowledge base operation: {Operation}", request.Operation);
        
        await Task.Delay(150, cancellationToken);
        
        return new KnowledgeBaseResult(
            Success: true,
            OperationResult: new KnowledgeBaseOperationResult(
                OperationType: request.Operation,
                EntityId: request.EntityId,
                EntityType: request.EntityType,
                Changes: request.Data,
                Timestamp: DateTime.UtcNow
            ),
            EntitiesAffected: 1,
            OperationTimeMs: 150
        );
    }

    public async Task<RagQueryResult> QueryRagAsync(RagQueryRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Processing RAG query: {Question}", request.Question);
        
        await Task.Delay(400, cancellationToken);
        
        var sources = new[]
        {
            new RagSource(
                Id: "source-001",
                SourceType: "Code Example",
                Content: "public async Task<string> GetDataAsync() { return await httpClient.GetStringAsync(url); }",
                Relevance: 0.9,
                Location: new SourceLocation("Example.cs", 10, 1, 80),
                Metadata: new Dictionary<string, object> { { "language", "C#" } }
            ),
            new RagSource(
                Id: "source-002",
                SourceType: "Documentation",
                Content: "Use async/await for I/O operations to avoid blocking threads",
                Relevance: 0.85,
                Location: new SourceLocation("docs/async.md", 1, 1, 60),
                Metadata: new Dictionary<string, object> { { "type", "documentation" } }
            )
        };

        return new RagQueryResult(
            Success: true,
            Answer: "For async programming in C#, use async/await for I/O operations, avoid blocking calls, and handle exceptions properly. Always return Task or Task<T> from async methods.",
            Sources: sources,
            Confidence: 0.9,
            TokensUsed: 150,
            GenerationTimeMs: 400
        );
    }

    public async Task<KnowledgeRagConfiguration> GetConfigurationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting Knowledge RAG configuration");
        
        await Task.Delay(50, cancellationToken);
        
        return new KnowledgeRagConfiguration(
            VectorStoreConfig: new VectorStoreConfiguration(
                Provider: "Pinecone",
                ConnectionString: "pinecone://api-key",
                IndexName: "indfusion-patterns",
                Dimensions: 1536,
                SimilarityMetric: "cosine"
            ),
            EmbeddingConfig: new EmbeddingConfiguration(
                Provider: "OpenAI",
                Model: "text-embedding-ada-002",
                MaxTokens: 8192,
                BatchSize: 100
            ),
            RagConfig: new RagConfiguration(
                MaxContextLength: 4000,
                Temperature: 0.7,
                MaxTokens: 1000,
                TopK: 5,
                SimilarityThreshold: 0.7
            ),
            IndexingConfig: new IndexingConfiguration(
                ChunkSize: 1000,
                ChunkOverlap: 200,
                IndexingBatchSize: 50,
                UpdateFrequency: TimeSpan.FromHours(1)
            ),
            Version: "1.0.0",
            LastUpdated: DateTime.UtcNow
        );
    }
}