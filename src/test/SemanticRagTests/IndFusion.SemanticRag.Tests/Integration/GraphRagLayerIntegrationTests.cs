using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Integration;

/// <summary>
/// End-to-end integration tests validating full workflows for the Graph RAG Layer.
/// </summary>
public class GraphRagLayerIntegrationTests : IClassFixture<IntegrationTestFixture>
{
    private readonly IntegrationTestFixture _fixture;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the GraphRagLayerIntegrationTests class.
    /// </summary>
    /// <param name="fixture">The integration test fixture.</param>
    public GraphRagLayerIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _serviceProvider = _fixture.ServiceProvider;
    }

    /// <summary>
    /// Verifies that the full workflow completes successfully.
    /// </summary>
    [Fact]
    public async Task Full_Workflow_Should_Complete_Successfully()
    {
        // Arrange
        var graphQueryService = _serviceProvider.GetRequiredService<IGraphQueryService>();
        var patternSuggestService = _serviceProvider.GetRequiredService<IPatternSuggestService>();
        var patternGraphQueryService = _serviceProvider.GetRequiredService<IPatternGraphQueryService>();

        var testCode = @"
public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User> GetUserAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }
}";

        // Act & Assert - Step 1: Analyze code for patterns
        var analysisResult = await patternSuggestService.AnalyzePatternAsync(testCode, "Repository Pattern", CancellationToken.None);
        analysisResult.IsSuccess.ShouldBeTrue();
        analysisResult.Value.ShouldNotBe<PatternAnalysis>(default);
        analysisResult.Value.PatternType.ShouldBe("Repository Pattern");

        // Step 2: Get pattern suggestions
        var suggestionOptions = new PatternSuggestionOptions(
            MaxSuggestions: 5,
            MinConfidence: 0.6f,
            Categories: ["Design Patterns"],
            IncludeCodeExamples: true,
            IncludeEffortEstimate: true
        );

        var suggestionsResult = await patternSuggestService.SuggestPatternsAsync(testCode, suggestionOptions, CancellationToken.None);
        suggestionsResult.IsSuccess.ShouldBeTrue();
        suggestionsResult.Value.ShouldNotBeNull();

        // Step 3: Find pattern violations
        var violationsResult = await patternSuggestService.FindViolationsAsync(testCode, "UserService.cs", CancellationToken.None);
        violationsResult.IsSuccess.ShouldBeTrue();
        violationsResult.Value.ShouldNotBeNull();

        // Step 4: Query pattern graph
        var patternGraphQuery = new PatternGraphQuery(
            Query: "MATCH (p:PatternDefinition) WHERE p.category = 'Design Patterns' RETURN p",
            Parameters: null,
            MaxResults: 10,
            TimeoutMs: 60000
        );

        var graphQueryResult = await patternGraphQueryService.QueryPatternGraphAsync(patternGraphQuery, CancellationToken.None);
        graphQueryResult.IsSuccess.ShouldBeTrue();
        graphQueryResult.Value.ShouldNotBe<PatternGraphResult>(default);

        // Step 5: Find pattern relationships
        var relationshipsResult = await patternGraphQueryService.FindPatternRelationshipsAsync("repository-pattern", 2, CancellationToken.None);
        relationshipsResult.IsSuccess.ShouldBeTrue();
        relationshipsResult.Value.ShouldNotBeNull();

        // Step 6: Find similar patterns
        var similarPatternsResult = await patternGraphQueryService.FindSimilarPatternsAsync("repository-pattern", 0.7f, 5, CancellationToken.None);
        similarPatternsResult.IsSuccess.ShouldBeTrue();
        similarPatternsResult.Value.ShouldNotBeNull();

        // Step 7: Get pattern usage statistics
        var statsResult = await patternGraphQueryService.GetPatternUsageStatisticsAsync("repository-pattern", CancellationToken.None);
        statsResult.IsSuccess.ShouldBeTrue();
        statsResult.Value.ShouldNotBe<PatternUsageStatistics>(default);

        // Step 8: Execute graph query
        var graphQuery = "MATCH (n:CodeNode) WHERE n.type = 'Class' RETURN n LIMIT 5";
        var executeQueryResult = await graphQueryService.ExecuteQueryAsync(graphQuery, null, CancellationToken.None);
        executeQueryResult.IsSuccess.ShouldBeTrue();
        executeQueryResult.Value.ShouldNotBe<GraphQueryResult>(default);

        // Step 9: Get nodes by type
        var nodesResult = await graphQueryService.GetNodesAsync("CodeNode", null, CancellationToken.None);
        nodesResult.IsSuccess.ShouldBeTrue();
        nodesResult.Value.ShouldNotBeNull();

        // Step 10: Get relationships by type
        var relationshipsByTypeResult = await graphQueryService.GetRelationshipsAsync("DEPENDS_ON", null, CancellationToken.None);
        relationshipsByTypeResult.IsSuccess.ShouldBeTrue();
        relationshipsByTypeResult.Value.ShouldNotBeNull();

        // Step 11: Traverse graph
        var traverseResult = await graphQueryService.TraverseAsync("user-service-node", 3, ["DEPENDS_ON"], CancellationToken.None);
        traverseResult.IsSuccess.ShouldBeTrue();
        traverseResult.Value.ShouldNotBe<GraphTraversalResult>(default);

        // Step 12: Find shortest path
        var shortestPathResult = await graphQueryService.FindShortestPathAsync("user-service-node", "user-repository-node", 5, CancellationToken.None);
        shortestPathResult.IsSuccess.ShouldBeTrue();
        // Path might be null if no path exists, which is valid

        // Step 13: Get graph statistics
        var statisticsResult = await graphQueryService.GetStatisticsAsync(CancellationToken.None);
        statisticsResult.IsSuccess.ShouldBeTrue();
        statisticsResult.Value.ShouldNotBe<GraphStatistics>(default);
    }

    /// <summary>
    /// Verifies that the pattern suggestion workflow handles complex code.
    /// </summary>
    [Fact]
    public async Task Pattern_Suggestion_Workflow_Should_Handle_Complex_Code()
    {
        // Arrange
        var patternSuggestService = _serviceProvider.GetRequiredService<IPatternSuggestService>();

        var complexCode = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IEmailService emailService,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
        {
            try
            {
                _logger.LogInformation(""Creating order for customer {CustomerId}"", request.CustomerId);

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    Items = request.Items,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await _orderRepository.SaveAsync(order);

                await _emailService.SendOrderConfirmationAsync(order);

                _logger.LogInformation(""Order {OrderId} created successfully"", order.Id);

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ""Failed to create order for customer {CustomerId}"", request.CustomerId);
                throw;
            }
        }
    }
}";

        // Act & Assert - Analyze for multiple patterns
        var repositoryAnalysis = await patternSuggestService.AnalyzePatternAsync(complexCode, "Repository Pattern", CancellationToken.None);
        repositoryAnalysis.IsSuccess.ShouldBeTrue();

        var dependencyInjectionAnalysis = await patternSuggestService.AnalyzePatternAsync(complexCode, "Dependency Injection", CancellationToken.None);
        dependencyInjectionAnalysis.IsSuccess.ShouldBeTrue();

        var loggingAnalysis = await patternSuggestService.AnalyzePatternAsync(complexCode, "Structured Logging", CancellationToken.None);
        loggingAnalysis.IsSuccess.ShouldBeTrue();

        // Get comprehensive suggestions
        var suggestionOptions = new PatternSuggestionOptions(
            MaxSuggestions: 10,
            MinConfidence: 0.5f,
            Categories: ["Design Patterns", "Best Practices", "Error Handling"],
            IncludeCodeExamples: true,
            IncludeEffortEstimate: true
        );

        var suggestionsResult = await patternSuggestService.SuggestPatternsAsync(complexCode, suggestionOptions, CancellationToken.None);
        suggestionsResult.IsSuccess.ShouldBeTrue();
        suggestionsResult.Value.ShouldNotBeNull();
        suggestionsResult.Value.Count.ShouldBeGreaterThan(0);

        // Find violations
        var violationsResult = await patternSuggestService.FindViolationsAsync(complexCode, "OrderService.cs", CancellationToken.None);
        violationsResult.IsSuccess.ShouldBeTrue();
        violationsResult.Value.ShouldNotBeNull();

        // Get pattern definitions
        var patternDefinitionsResult = await patternSuggestService.GetPatternDefinitionsAsync("Design Patterns", CancellationToken.None);
        patternDefinitionsResult.IsSuccess.ShouldBeTrue();
        patternDefinitionsResult.Value.ShouldNotBeNull();

        // Get pattern categories
        var categoriesResult = await patternSuggestService.GetPatternCategoriesAsync(CancellationToken.None);
        categoriesResult.IsSuccess.ShouldBeTrue();
        categoriesResult.Value.ShouldNotBeNull();
        categoriesResult.Value.Count.ShouldBeGreaterThan(0);
    }

    /// <summary>
    /// Verifies that the graph query workflow handles complex queries.
    /// </summary>
    [Fact]
    public async Task Graph_Query_Workflow_Should_Handle_Complex_Queries()
    {
        // Arrange
        var graphQueryService = _serviceProvider.GetRequiredService<IGraphQueryService>();
        var patternGraphQueryService = _serviceProvider.GetRequiredService<IPatternGraphQueryService>();

        // Act & Assert - Execute complex graph queries
        var complexQuery = @"
MATCH (c:CodeNode)-[r:DEPENDS_ON]->(d:CodeNode)
WHERE c.type = 'Class' AND d.type = 'Interface'
RETURN c.name as ClassName, d.name as InterfaceName, r.strength as DependencyStrength
ORDER BY r.strength DESC
LIMIT 10";

        var queryResult = await graphQueryService.ExecuteQueryAsync(complexQuery, null, CancellationToken.None);
        queryResult.IsSuccess.ShouldBeTrue();
        queryResult.Value.ShouldNotBe(default(GraphQueryResult));

        // Query with parameters
        var parameterizedQuery = "MATCH (n:CodeNode) WHERE n.language = $language AND n.type = $type RETURN n";
        var parameters = new Dictionary<string, object>
        {
            ["language"] = "C#",
            ["type"] = "Class"
        };

        var parameterizedResult = await graphQueryService.ExecuteQueryAsync(parameterizedQuery, parameters, CancellationToken.None);
        parameterizedResult.IsSuccess.ShouldBeTrue();
        parameterizedResult.Value.ShouldNotBe(default(GraphQueryResult));

        // Complex pattern graph query
        var patternGraphQuery = new PatternGraphQuery(
            Query: @"
MATCH (p:PatternDefinition)-[r:RELATED_TO]->(related:PatternDefinition)
WHERE p.category = $category
RETURN p, r, related
ORDER BY r.strength DESC",
            Parameters: new Dictionary<string, object> { ["category"] = "Design Patterns" },
            MaxResults: 20,
            TimeoutMs: 60000
        );

        var patternGraphResult = await patternGraphQueryService.QueryPatternGraphAsync(patternGraphQuery, CancellationToken.None);
        patternGraphResult.IsSuccess.ShouldBeTrue();
        patternGraphResult.Value.ShouldNotBe<PatternGraphResult>(default);

        // Find anti-patterns
        var antiPatternsResult = await patternGraphQueryService.FindAntiPatternsAsync("Performance", PatternSeverity.Warning, CancellationToken.None);
        antiPatternsResult.IsSuccess.ShouldBeTrue();
        antiPatternsResult.Value.ShouldNotBeNull();

        // Get pattern evolution
        var evolutionResult = await patternGraphQueryService.GetPatternEvolutionAsync("repository-pattern", CancellationToken.None);
        evolutionResult.IsSuccess.ShouldBeTrue();
        evolutionResult.Value.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that the error handling workflow handles failures gracefully.
    /// </summary>
    [Fact]
    public async Task Error_Handling_Workflow_Should_Handle_Failures_Gracefully()
    {
        // Arrange
        var graphQueryService = _serviceProvider.GetRequiredService<IGraphQueryService>();
        var patternSuggestService = _serviceProvider.GetRequiredService<IPatternSuggestService>();
        var patternGraphQueryService = _serviceProvider.GetRequiredService<IPatternGraphQueryService>();

        // Act & Assert - Test error handling scenarios

        // Invalid query
        var invalidQueryResult = await graphQueryService.ExecuteQueryAsync("INVALID QUERY", null, CancellationToken.None);
        invalidQueryResult.IsFailure.ShouldBeTrue();
        invalidQueryResult.Error.ShouldNotBeNullOrEmpty();

        // Empty code context
        var emptyCodeResult = await patternSuggestService.SuggestPatternsAsync("", new PatternSuggestionOptions(), CancellationToken.None);
        emptyCodeResult.IsFailure.ShouldBeTrue();
        emptyCodeResult.Error.ShouldContain("Code context cannot be null or empty");

        // Invalid pattern type
        var invalidPatternResult = await patternSuggestService.AnalyzePatternAsync("public class Test { }", "", CancellationToken.None);
        invalidPatternResult.IsFailure.ShouldBeTrue();
        invalidPatternResult.Error.ShouldContain("Pattern type cannot be null or empty");

        // Invalid pattern ID
        var invalidPatternIdResult = await patternGraphQueryService.FindPatternRelationshipsAsync("", 1, CancellationToken.None);
        invalidPatternIdResult.IsFailure.ShouldBeTrue();
        invalidPatternIdResult.Error.ShouldContain("Pattern ID cannot be null or empty");

        // Invalid similarity threshold
        var invalidThresholdResult = await patternGraphQueryService.FindSimilarPatternsAsync("test-pattern", 1.5f, 5, CancellationToken.None);
        invalidThresholdResult.IsFailure.ShouldBeTrue();
        invalidThresholdResult.Error.ShouldContain("Similarity threshold must be between 0.0 and 1.0");

        // Negative max depth
        var negativeDepthResult = await graphQueryService.TraverseAsync("test-node", -1, null, CancellationToken.None);
        negativeDepthResult.IsFailure.ShouldBeTrue();
        negativeDepthResult.Error.ShouldContain("Max depth cannot be negative");
    }

    /// <summary>
    /// Verifies that the cancellation workflow handles cancellation gracefully.
    /// </summary>
    [Fact]
    public async Task Cancellation_Workflow_Should_Handle_Cancellation_Gracefully()
    {
        // Arrange
        var graphQueryService = _serviceProvider.GetRequiredService<IGraphQueryService>();
        var patternSuggestService = _serviceProvider.GetRequiredService<IPatternSuggestService>();
        var patternGraphQueryService = _serviceProvider.GetRequiredService<IPatternGraphQueryService>();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - Test cancellation scenarios
        var cancelledQueryResult = await graphQueryService.ExecuteQueryAsync("MATCH (n) RETURN n", null, cts.Token);
        cancelledQueryResult.IsFailure.ShouldBeTrue();
        cancelledQueryResult.Error.ShouldContain("cancelled");

        var cancelledSuggestionResult = await patternSuggestService.SuggestPatternsAsync("public class Test { }", new PatternSuggestionOptions(), cts.Token);
        cancelledSuggestionResult.IsFailure.ShouldBeTrue();
        cancelledSuggestionResult.Error.ShouldContain("cancelled");

        var cancelledGraphQueryResult = await patternGraphQueryService.QueryPatternGraphAsync(
            new PatternGraphQuery("MATCH (p:PatternDefinition) RETURN p"), cts.Token);
        cancelledGraphQueryResult.IsFailure.ShouldBeTrue();
        cancelledGraphQueryResult.Error.ShouldContain("cancelled");
    }

    /// <summary>
    /// Verifies that the performance workflow completes within reasonable time.
    /// </summary>
    [Fact]
    public async Task Performance_Workflow_Should_Complete_Within_Reasonable_Time()
    {
        // Arrange
        var graphQueryService = _serviceProvider.GetRequiredService<IGraphQueryService>();
        var patternSuggestService = _serviceProvider.GetRequiredService<IPatternSuggestService>();
        var patternGraphQueryService = _serviceProvider.GetRequiredService<IPatternGraphQueryService>();

        var largeCode = string.Join("\n", Enumerable.Range(1, 1000).Select(i => $"public class Class{i} {{ public void Method{i}() {{ }} }}"));

        // Act & Assert - Test performance scenarios
        var startTime = DateTimeOffset.UtcNow;

        var analysisResult = await patternSuggestService.AnalyzePatternAsync(largeCode, "Class Pattern", CancellationToken.None);
        var analysisTime = DateTimeOffset.UtcNow - startTime;

        analysisResult.IsSuccess.ShouldBeTrue();
        analysisTime.TotalMilliseconds.ShouldBeLessThan(5000); // Should complete within 5 seconds

        startTime = DateTimeOffset.UtcNow;

        var suggestionsResult = await patternSuggestService.SuggestPatternsAsync(largeCode, new PatternSuggestionOptions(), CancellationToken.None);
        var suggestionsTime = DateTimeOffset.UtcNow - startTime;

        suggestionsResult.IsSuccess.ShouldBeTrue();
        suggestionsTime.TotalMilliseconds.ShouldBeLessThan(3000); // Should complete within 3 seconds

        startTime = DateTimeOffset.UtcNow;

        var graphQueryResult = await graphQueryService.ExecuteQueryAsync("MATCH (n) RETURN n LIMIT 1000", null, CancellationToken.None);
        var graphQueryTime = DateTimeOffset.UtcNow - startTime;

        graphQueryResult.IsSuccess.ShouldBeTrue();
        graphQueryTime.TotalMilliseconds.ShouldBeLessThan(2000); // Should complete within 2 seconds
    }
}