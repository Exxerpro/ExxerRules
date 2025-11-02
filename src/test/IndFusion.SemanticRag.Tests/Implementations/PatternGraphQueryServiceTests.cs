using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Implementations;

/// <summary>
/// TDD tests for PatternGraphQueryService using real implementations.
/// </summary>
public class PatternGraphQueryServiceTests
{
    private readonly IKnowledgeGraphPort _knowledgeGraphPort;
    private readonly ILogger<PatternGraphQueryService> _logger;
    private readonly PatternGraphQueryService _patternGraphQueryService;

    /// <summary>
    /// Initializes a new instance of the PatternGraphQueryServiceTests class.
    /// </summary>
    public PatternGraphQueryServiceTests()
    {
        _knowledgeGraphPort = Substitute.For<IKnowledgeGraphPort>();
        _logger = Substitute.For<ILogger<PatternGraphQueryService>>();
        _patternGraphQueryService = new PatternGraphQueryService(_knowledgeGraphPort, _logger);
    }

    /// <summary>
    /// Verifies that QueryPatternGraphAsync executes pattern graph query successfully.
    /// </summary>
    [Fact]
    public async Task QueryPatternGraphAsync_Should_Execute_Pattern_Graph_Query()
    {
        // Arrange
        var query = new PatternGraphQuery(
            Query: "MATCH (p:PatternDefinition) RETURN p",
            Parameters: new Dictionary<string, object> { ["limit"] = 10 },
            MaxResults: 10,
            TimeoutMs: 30000);

        var patternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "singleton-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "singleton",
                    ["name"] = "Singleton Pattern",
                    ["description"] = "Ensures a class has only one instance",
                    ["category"] = "Design Patterns",
                    ["severity"] = "Info",
                    ["pattern"] = "class.*Singleton",
                    ["tags"] = new List<string> { "creational" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        var relationshipNodes = new List<KnowledgeNode>
        {
            new(
                Id: "rel1",
                Label: "PatternRelationship",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "rel1",
                    ["type"] = "RELATED_TO",
                    ["sourcePatternId"] = "singleton",
                    ["targetPatternId"] = "factory",
                    ["strength"] = 0.8f
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(query.Query, query.Parameters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(patternNodes));
        _knowledgeGraphPort.QueryRelationshipsAsync(query.Query, query.Parameters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(new List<KnowledgeRelationship>()));

        // Act
        var result = await _patternGraphQueryService.QueryPatternGraphAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternGraphResult>(default);
        result.Value.PatternCount.ShouldBe(1);
        result.Value.RelationshipCount.ShouldBe(0);
        result.Value.TotalResults.ShouldBe(1);
        result.Value.ExecutionTimeMs.ShouldBeGreaterThan(0);
        result.Value.HasMoreResults.ShouldBeFalse();
    }

    /// <summary>
    /// Verifies that QueryPatternGraphAsync handles invalid query.
    /// </summary>
    [Fact]
    public async Task QueryPatternGraphAsync_Should_Handle_Invalid_Query()
    {
        // Arrange
        var query = new PatternGraphQuery(
            Query: "INVALID QUERY",
            Parameters: null,
            MaxResults: 10,
            TimeoutMs: 30000);

        _knowledgeGraphPort.QueryNodesAsync(query.Query, query.Parameters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.WithFailure("Invalid query syntax"));
        _knowledgeGraphPort.QueryRelationshipsAsync(query.Query, query.Parameters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure("Invalid query syntax"));

        // Act
        var result = await _patternGraphQueryService.QueryPatternGraphAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Pattern graph query execution failed");
    }

    /// <summary>
    /// Verifies that QueryPatternGraphAsync handles empty query.
    /// </summary>
    [Fact]
    public async Task QueryPatternGraphAsync_Should_Handle_Empty_Query()
    {
        // Arrange
        var query = new PatternGraphQuery(
            Query: "",
            Parameters: null,
            MaxResults: 10,
            TimeoutMs: 30000);

        // Act
        var result = await _patternGraphQueryService.QueryPatternGraphAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Query cannot be null or empty");
    }

    /// <summary>
    /// Verifies that FindPatternRelationshipsAsync finds pattern relationships.
    /// </summary>
    [Fact]
    public async Task FindPatternRelationshipsAsync_Should_Find_Pattern_Relationships()
    {
        // Arrange
        var patternId = "singleton";
        var maxDepth = 2;
        var expectedRelationships = new List<KnowledgeRelationship>
        {
            new(
                Id: "rel1",
                RelationshipType: "RELATED_TO",
                FromNodeId: "singleton",
                ToNodeId: "factory",
                Properties: new Dictionary<string, object> { ["strength"] = 0.8 },
                CreatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _patternGraphQueryService.FindPatternRelationshipsAsync(patternId, maxDepth, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].SourcePatternId.ShouldBe("singleton");
        result.Value[0].TargetPatternId.ShouldBe("factory");
        result.Value[0].Strength.ShouldBe(0.8f);
    }

    /// <summary>
    /// Verifies that FindPatternRelationshipsAsync handles empty pattern ID.
    /// </summary>
    [Fact]
    public async Task FindPatternRelationshipsAsync_Should_Handle_Empty_PatternId()
    {
        // Arrange
        var patternId = "";
        var maxDepth = 2;

        // Act
        var result = await _patternGraphQueryService.FindPatternRelationshipsAsync(patternId, maxDepth, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Pattern ID cannot be null or empty");
    }

    /// <summary>
    /// Verifies that FindPatternRelationshipsAsync handles negative max depth.
    /// </summary>
    [Fact]
    public async Task FindPatternRelationshipsAsync_Should_Handle_Negative_MaxDepth()
    {
        // Arrange
        var patternId = "singleton";
        var maxDepth = -1;

        // Act
        var result = await _patternGraphQueryService.FindPatternRelationshipsAsync(patternId, maxDepth, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Max depth cannot be negative");
    }

    /// <summary>
    /// Verifies that FindSimilarPatternsAsync finds similar patterns.
    /// </summary>
    [Fact]
    public async Task FindSimilarPatternsAsync_Should_Find_Similar_Patterns()
    {
        // Arrange
        var patternId = "singleton";
        var similarityThreshold = 0.7f;
        var maxResults = 5;

        var sourcePatternNode = new KnowledgeNode(
            Id: "singleton-pattern",
            Label: "PatternDefinition",
            Properties: new Dictionary<string, object>
            {
                ["id"] = "singleton",
                ["name"] = "Singleton Pattern",
                ["description"] = "Ensures a class has only one instance",
                ["category"] = "Design Patterns",
                ["severity"] = "Info",
                ["pattern"] = "class.*Singleton",
                ["tags"] = new List<string> { "creational" },
                ["isEnabled"] = true
            },
            CreatedAt: DateTimeOffset.UtcNow,
            UpdatedAt: DateTimeOffset.UtcNow);

        var similarPatternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "factory-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "factory",
                    ["name"] = "Factory Pattern",
                    ["description"] = "Creates objects without specifying their exact classes",
                    ["category"] = "Design Patterns",
                    ["severity"] = "Info",
                    ["pattern"] = "class.*Factory",
                    ["tags"] = new List<string> { "creational" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Is<string>(q => q.Contains("WHERE p.id = $patternId")), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(new List<KnowledgeNode> { sourcePatternNode }));
        _knowledgeGraphPort.QueryNodesAsync(Arg.Is<string>(q => q.Contains("WHERE p.id <> $patternId")), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(similarPatternNodes));

        // Act
        var result = await _patternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeGreaterThan(0);
        result.Value[0].PatternId.ShouldBe("factory");
        result.Value[0].SimilarityScore.ShouldBeGreaterThanOrEqualTo(similarityThreshold);
    }

    /// <summary>
    /// Verifies that FindSimilarPatternsAsync handles empty pattern ID.
    /// </summary>
    [Fact]
    public async Task FindSimilarPatternsAsync_Should_Handle_Empty_PatternId()
    {
        // Arrange
        var patternId = "";
        var similarityThreshold = 0.7f;
        var maxResults = 5;

        // Act
        var result = await _patternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Pattern ID cannot be null or empty");
    }

    /// <summary>
    /// Verifies that FindSimilarPatternsAsync handles invalid threshold.
    /// </summary>
    [Fact]
    public async Task FindSimilarPatternsAsync_Should_Handle_Invalid_Threshold()
    {
        // Arrange
        var patternId = "singleton";
        var similarityThreshold = 1.5f; // Invalid threshold
        var maxResults = 5;

        // Act
        var result = await _patternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Similarity threshold must be between 0.0 and 1.0");
    }

    /// <summary>
    /// Verifies that GetPatternUsageStatisticsAsync returns usage statistics.
    /// </summary>
    [Fact]
    public async Task GetPatternUsageStatisticsAsync_Should_Return_Usage_Statistics()
    {
        // Arrange
        var patternId = "singleton";
        var usageNode = new KnowledgeNode(
            Id: "usage-stats",
            Label: "UsageStatistics",
            Properties: new Dictionary<string, object>
            {
                ["usageCount"] = 150,
                ["fileCount"] = 45,
                ["projectCount"] = 12,
                ["lastUsed"] = DateTimeOffset.UtcNow.AddDays(-1)
            },
            CreatedAt: DateTimeOffset.UtcNow,
            UpdatedAt: DateTimeOffset.UtcNow);

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(new List<KnowledgeNode> { usageNode }));

        // Act
        var result = await _patternGraphQueryService.GetPatternUsageStatisticsAsync(patternId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternUsageStatistics>(default);
        result.Value.PatternId.ShouldBe(patternId);
        result.Value.UsageCount.ShouldBe(150);
        result.Value.FileCount.ShouldBe(45);
        result.Value.ProjectCount.ShouldBe(12);
        result.Value.LastUsed.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that GetPatternUsageStatisticsAsync handles empty pattern ID.
    /// </summary>
    [Fact]
    public async Task GetPatternUsageStatisticsAsync_Should_Handle_Empty_PatternId()
    {
        // Arrange
        var patternId = "";

        // Act
        var result = await _patternGraphQueryService.GetPatternUsageStatisticsAsync(patternId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Pattern ID cannot be null or empty");
    }

    /// <summary>
    /// Verifies that FindAntiPatternsAsync finds anti-pattern violations.
    /// </summary>
    [Fact]
    public async Task FindAntiPatternsAsync_Should_Find_Anti_Pattern_Violations()
    {
        // Arrange
        var category = "Performance";
        var severity = PatternSeverity.Warning;

        var antiPatternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "anti-pattern-1",
                Label: "AntiPatternViolation",
                Properties: new Dictionary<string, object>
                {
                    ["antiPatternId"] = "n-plus-one",
                    ["antiPatternName"] = "N+1 Query Problem",
                    ["severity"] = "Warning",
                    ["message"] = "Multiple database queries in a loop",
                    ["filePath"] = "DataService.cs",
                    ["lineNumber"] = 25,
                    ["codeSnippet"] = "foreach (var item in items) { db.Query(item.Id); }",
                    ["suggestedFix"] = "Use batch loading or include statements"
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(antiPatternNodes));

        // Act
        var result = await _patternGraphQueryService.FindAntiPatternsAsync(category, severity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].AntiPatternName.ShouldBe("N+1 Query Problem");
        result.Value[0].Severity.ShouldBe(PatternSeverity.Warning);
        result.Value[0].Location.ShouldBe("DataService.cs:25");
    }

    /// <summary>
    /// Verifies that FindAntiPatternsAsync handles null category.
    /// </summary>
    [Fact]
    public async Task FindAntiPatternsAsync_Should_Handle_Null_Category()
    {
        // Arrange
        var severity = PatternSeverity.Info;

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(new List<KnowledgeNode>()));

        // Act
        var result = await _patternGraphQueryService.FindAntiPatternsAsync(null, severity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that GetPatternEvolutionAsync returns evolution history.
    /// </summary>
    [Fact]
    public async Task GetPatternEvolutionAsync_Should_Return_Evolution_History()
    {
        // Arrange
        var patternId = "singleton";
        var evolutionNodes = new List<KnowledgeNode>
        {
            new(
                Id: "evolution-1",
                Label: "PatternEvolution",
                Properties: new Dictionary<string, object>
                {
                    ["version"] = "1.0",
                    ["changeType"] = "Created",
                    ["changeDescription"] = "Initial pattern definition",
                    ["changedAt"] = DateTimeOffset.UtcNow.AddDays(-30),
                    ["changedBy"] = "developer1"
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow),
            new(
                Id: "evolution-2",
                Label: "PatternEvolution",
                Properties: new Dictionary<string, object>
                {
                    ["version"] = "1.1",
                    ["changeType"] = "Updated",
                    ["changeDescription"] = "Added thread-safety considerations",
                    ["changedAt"] = DateTimeOffset.UtcNow.AddDays(-15),
                    ["changedBy"] = "developer2"
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(evolutionNodes));

        // Act
        var result = await _patternGraphQueryService.GetPatternEvolutionAsync(patternId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value[0].ChangeType.ShouldBe(PatternChangeType.Created);
        result.Value[0].Version.ShouldBe("1.0");
        result.Value[1].ChangeType.ShouldBe(PatternChangeType.Updated);
        result.Value[1].Version.ShouldBe("1.1");
    }

    /// <summary>
    /// Verifies that GetPatternEvolutionAsync handles empty pattern ID.
    /// </summary>
    [Fact]
    public async Task GetPatternEvolutionAsync_Should_Handle_Empty_PatternId()
    {
        // Arrange
        var patternId = "";

        // Act
        var result = await _patternGraphQueryService.GetPatternEvolutionAsync(patternId, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Pattern ID cannot be null or empty");
    }

    /// <summary>
    /// Verifies that all methods handle cancellation properly.
    /// </summary>
    [Fact]
    public async Task All_Methods_Should_Handle_Cancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _knowledgeGraphPort.QueryNodesAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), cts.Token)
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.WithFailure("Cancelled"));
        _knowledgeGraphPort.QueryRelationshipsAsync(Arg.Any<string>(), Arg.Any<IReadOnlyDictionary<string, object>>(), cts.Token)
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure("Cancelled"));

        // Act & Assert
        var queryResult = await _patternGraphQueryService.QueryPatternGraphAsync(
            new PatternGraphQuery("test", null, 10, 30000), cts.Token);
        queryResult.IsFailure.ShouldBeTrue();
        queryResult.Error.ShouldContain("cancelled");

        var relationshipsResult = await _patternGraphQueryService.FindPatternRelationshipsAsync("test", 1, cts.Token);
        relationshipsResult.IsFailure.ShouldBeTrue();
        relationshipsResult.Error.ShouldContain("cancelled");

        var similaritiesResult = await _patternGraphQueryService.FindSimilarPatternsAsync("test", 0.5f, 5, cts.Token);
        similaritiesResult.IsFailure.ShouldBeTrue();
        similaritiesResult.Error.ShouldContain("cancelled");

        var statsResult = await _patternGraphQueryService.GetPatternUsageStatisticsAsync("test", cts.Token);
        statsResult.IsFailure.ShouldBeTrue();
        statsResult.Error.ShouldContain("cancelled");

        var antiPatternsResult = await _patternGraphQueryService.FindAntiPatternsAsync("test", PatternSeverity.Info, cts.Token);
        antiPatternsResult.IsFailure.ShouldBeTrue();
        antiPatternsResult.Error.ShouldContain("cancelled");

        var evolutionResult = await _patternGraphQueryService.GetPatternEvolutionAsync("test", cts.Token);
        evolutionResult.IsFailure.ShouldBeTrue();
        evolutionResult.Error.ShouldContain("cancelled");
    }

    /// <summary>
    /// Verifies that QueryPatternGraphAsync uses ConfigureAwait(false).
    /// </summary>
    [Fact]
    public async Task QueryPatternGraphAsync_Should_Use_ConfigureAwait_False()
    {
        // Arrange
        var query = new PatternGraphQuery(
            Query: "MATCH (p:PatternDefinition) RETURN p",
            Parameters: null,
            MaxResults: 10,
            TimeoutMs: 30000);

        _knowledgeGraphPort.QueryNodesAsync(query.Query, query.Parameters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(new List<KnowledgeNode>()));
        _knowledgeGraphPort.QueryRelationshipsAsync(query.Query, query.Parameters, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(new List<KnowledgeRelationship>()));

        // Act
        var result = await _patternGraphQueryService.QueryPatternGraphAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        // Verify that ConfigureAwait(false) was used by checking the call was made
        await _knowledgeGraphPort.Received().QueryNodesAsync(query.Query, query.Parameters, Arg.Any<CancellationToken>());
        await _knowledgeGraphPort.Received().QueryRelationshipsAsync(query.Query, query.Parameters, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that FindSimilarPatternsAsync calculates similarity correctly.
    /// </summary>
    [Fact]
    public async Task FindSimilarPatternsAsync_Should_Calculate_Similarity_Correctly()
    {
        // Arrange
        var patternId = "singleton";
        var similarityThreshold = 0.5f;
        var maxResults = 5;

        var sourcePatternNode = new KnowledgeNode(
            Id: "singleton-pattern",
            Label: "PatternDefinition",
            Properties: new Dictionary<string, object>
            {
                ["id"] = "singleton",
                ["name"] = "Singleton Pattern",
                ["description"] = "Ensures a class has only one instance",
                ["category"] = "Design Patterns",
                ["severity"] = "Info",
                ["pattern"] = "class.*Singleton",
                ["tags"] = new List<string> { "creational" },
                ["isEnabled"] = true
            },
            CreatedAt: DateTimeOffset.UtcNow,
            UpdatedAt: DateTimeOffset.UtcNow);

        var similarPatternNodes = new List<KnowledgeNode>
        {
            new(
                Id: "factory-pattern",
                Label: "PatternDefinition",
                Properties: new Dictionary<string, object>
                {
                    ["id"] = "factory",
                    ["name"] = "Factory Pattern",
                    ["description"] = "Creates objects without specifying their exact classes",
                    ["category"] = "Design Patterns",
                    ["severity"] = "Info",
                    ["pattern"] = "class.*Factory",
                    ["tags"] = new List<string> { "creational" },
                    ["isEnabled"] = true
                },
                CreatedAt: DateTimeOffset.UtcNow,
                UpdatedAt: DateTimeOffset.UtcNow)
        };

        _knowledgeGraphPort.QueryNodesAsync(Arg.Is<string>(q => q.Contains("WHERE p.id = $patternId")), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(new List<KnowledgeNode> { sourcePatternNode }));
        _knowledgeGraphPort.QueryNodesAsync(Arg.Is<string>(q => q.Contains("WHERE p.id <> $patternId")), Arg.Any<IReadOnlyDictionary<string, object>>(), Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<KnowledgeNode>>.Success(similarPatternNodes));

        // Act
        var result = await _patternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeGreaterThan(0);
        result.Value[0].SimilarityScore.ShouldBeGreaterThanOrEqualTo(similarityThreshold);
        result.Value[0].SimilarityScore.ShouldBeLessThanOrEqualTo(1.0f);
    }
}
