using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using IndQuestResults;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Interfaces;

/// <summary>
/// ITDD tests for IPatternGraphQueryService interface contract validation.
/// </summary>
public class IPatternGraphQueryServiceTests
{
    private readonly IPatternGraphQueryService _mockPatternGraphQueryService;

    /// <summary>
    /// Initializes a new instance of the IPatternGraphQueryServiceTests class.
    /// </summary>
    public IPatternGraphQueryServiceTests()
    {
        _mockPatternGraphQueryService = Substitute.For<IPatternGraphQueryService>();
    }

    /// <summary>
    /// Verifies that QueryPatternGraphAsync returns success for valid query.
    /// </summary>
    [Fact]
    public async Task QueryPatternGraphAsync_Should_Return_Success_For_Valid_Query()
    {
        // Arrange
        var query = new PatternGraphQuery(
            Query: "MATCH (p:Pattern) RETURN p",
            Parameters: new Dictionary<string, object> { ["limit"] = 10 },
            MaxResults: 10,
            TimeoutMs: 60000);

        var expectedResult = new PatternGraphResult(
            Patterns:
            [
                new(
                    Id: "singleton",
                    Name: "Singleton Pattern",
                    Description: "Ensures a class has only one instance",
                    Category: "Design Patterns",
                    Severity: PatternSeverity.Info,
                    Pattern: "class.*Singleton",
                    Tags: ["creational"])
            ],
            Relationships:
            [
                new(
                    Id: "rel1",
                    Type: "RELATED_TO",
                    SourcePatternId: "singleton",
                    TargetPatternId: "factory",
                    Properties: new Dictionary<string, object>(),
                    Strength: 0.8f)
            ],
            ExecutionTimeMs: 150,
            TotalResults: 1,
            HasMoreResults: false);

        _mockPatternGraphQueryService.QueryPatternGraphAsync(query, Arg.Any<CancellationToken>())
            .Returns(Result<PatternGraphResult>.Success(expectedResult));

        // Act
        var result = await _mockPatternGraphQueryService.QueryPatternGraphAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternGraphResult>(default);
        result.Value.PatternCount.ShouldBe(1);
        result.Value.RelationshipCount.ShouldBe(1);
        result.Value.TotalResults.ShouldBe(1);
        result.Value.HasMoreResults.ShouldBeFalse();
        result.Value.ExecutionTimeMs.ShouldBe(150);
    }

    /// <summary>
    /// Verifies that QueryPatternGraphAsync returns failure for invalid query.
    /// </summary>
    [Fact]
    public async Task QueryPatternGraphAsync_Should_Return_Failure_For_Invalid_Query()
    {
        // Arrange
        var query = new PatternGraphQuery(
            Query: "INVALID QUERY",
            Parameters: null,
            MaxResults: 10,
            TimeoutMs: 60000);

        var expectedError = "Invalid query syntax";

        _mockPatternGraphQueryService.QueryPatternGraphAsync(query, Arg.Any<CancellationToken>())
            .Returns(Result<PatternGraphResult>.WithFailure(expectedError));

        // Act
        var result = await _mockPatternGraphQueryService.QueryPatternGraphAsync(query, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(expectedError);
    }

    /// <summary>
    /// Verifies that FindPatternRelationshipsAsync returns success for valid pattern.
    /// </summary>
    [Fact]
    public async Task FindPatternRelationshipsAsync_Should_Return_Success_For_Valid_Pattern()
    {
        // Arrange
        var patternId = "singleton";
        var maxDepth = 2;
        var expectedRelationships = new List<PatternRelationship>
        {
            new(
                Id: "rel1",
                Type: "RELATED_TO",
                SourcePatternId: "singleton",
                TargetPatternId: "factory",
                Properties: new Dictionary<string, object> { ["relationship_type"] = "creational" },
                Strength: 0.8f),
            new(
                Id: "rel2",
                Type: "ANTI_PATTERN_OF",
                SourcePatternId: "singleton",
                TargetPatternId: "god-object",
                Properties: new Dictionary<string, object>(),
                Strength: 0.6f)
        };

        _mockPatternGraphQueryService.FindPatternRelationshipsAsync(patternId, maxDepth, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _mockPatternGraphQueryService.FindPatternRelationshipsAsync(patternId, maxDepth, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value[0].SourcePatternId.ShouldBe("singleton");
        result.Value[0].TargetPatternId.ShouldBe("factory");
        result.Value[0].Strength.ShouldBe(0.8f);
    }

    /// <summary>
    /// Verifies that FindPatternRelationshipsAsync returns empty list for isolated pattern.
    /// </summary>
    [Fact]
    public async Task FindPatternRelationshipsAsync_Should_Return_Empty_List_For_Isolated_Pattern()
    {
        // Arrange
        var patternId = "isolated-pattern";
        var maxDepth = 3;

        _mockPatternGraphQueryService.FindPatternRelationshipsAsync(patternId, maxDepth, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternRelationship>>.Success([]));

        // Act
        var result = await _mockPatternGraphQueryService.FindPatternRelationshipsAsync(patternId, maxDepth, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that FindSimilarPatternsAsync returns success with similar patterns.
    /// </summary>
    [Fact]
    public async Task FindSimilarPatternsAsync_Should_Return_Success_With_Similar_Patterns()
    {
        // Arrange
        var patternId = "singleton";
        var similarityThreshold = 0.7f;
        var maxResults = 5;
        var expectedSimilarities = new List<PatternSimilarity>
        {
            new(
                PatternId: "factory",
                SimilarityScore: 0.85f,
                SimilarityType: "structural",
                CommonElements: ["creational", "object-creation"]),
            new(
                PatternId: "builder",
                SimilarityScore: 0.75f,
                SimilarityType: "semantic",
                CommonElements: ["creational"])
        };

        _mockPatternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternSimilarity>>.Success(expectedSimilarities));

        // Act
        var result = await _mockPatternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value[0].PatternId.ShouldBe("factory");
        result.Value[0].SimilarityScore.ShouldBe(0.85f);
        result.Value[0].SimilarityType.ShouldBe("structural");
        result.Value[0].CommonElements.Count.ShouldBe(2);
    }

    /// <summary>
    /// Verifies that FindSimilarPatternsAsync returns empty list for unique pattern.
    /// </summary>
    [Fact]
    public async Task FindSimilarPatternsAsync_Should_Return_Empty_List_For_Unique_Pattern()
    {
        // Arrange
        var patternId = "unique-pattern";
        var similarityThreshold = 0.8f;
        var maxResults = 10;

        _mockPatternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternSimilarity>>.Success([]));

        // Act
        var result = await _mockPatternGraphQueryService.FindSimilarPatternsAsync(patternId, similarityThreshold, maxResults, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that GetPatternUsageStatisticsAsync returns success with statistics.
    /// </summary>
    [Fact]
    public async Task GetPatternUsageStatisticsAsync_Should_Return_Success_With_Statistics()
    {
        // Arrange
        var patternId = "singleton";
        var expectedStats = new PatternUsageStatistics(
            PatternId: patternId,
            UsageCount: 150,
            FileCount: 45,
            ProjectCount: 12,
            LastUsed: DateTimeOffset.UtcNow.AddDays(-1),
            Trend: UsageTrend.Increasing);

        _mockPatternGraphQueryService.GetPatternUsageStatisticsAsync(patternId, Arg.Any<CancellationToken>())
            .Returns(Result<PatternUsageStatistics>.Success(expectedStats));

        // Act
        var result = await _mockPatternGraphQueryService.GetPatternUsageStatisticsAsync(patternId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternUsageStatistics>(default);
        result.Value.PatternId.ShouldBe(patternId);
        result.Value.UsageCount.ShouldBe(150);
        result.Value.FileCount.ShouldBe(45);
        result.Value.ProjectCount.ShouldBe(12);
        result.Value.Trend.ShouldBe(UsageTrend.Increasing);
        result.Value.AverageUsagePerFile.ShouldBe(150.0 / 45);
        result.Value.AverageUsagePerProject.ShouldBe(150.0 / 12);
    }

    /// <summary>
    /// Verifies that GetPatternUsageStatisticsAsync returns zero stats for unused pattern.
    /// </summary>
    [Fact]
    public async Task GetPatternUsageStatisticsAsync_Should_Return_Zero_Stats_For_Unused_Pattern()
    {
        // Arrange
        var patternId = "unused-pattern";
        var expectedStats = new PatternUsageStatistics(
            PatternId: patternId,
            UsageCount: 0,
            FileCount: 0,
            ProjectCount: 0,
            LastUsed: null,
            Trend: UsageTrend.Unknown);

        _mockPatternGraphQueryService.GetPatternUsageStatisticsAsync(patternId, Arg.Any<CancellationToken>())
            .Returns(Result<PatternUsageStatistics>.Success(expectedStats));

        // Act
        var result = await _mockPatternGraphQueryService.GetPatternUsageStatisticsAsync(patternId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternUsageStatistics>(default);
        result.Value.UsageCount.ShouldBe(0);
        result.Value.FileCount.ShouldBe(0);
        result.Value.ProjectCount.ShouldBe(0);
        result.Value.LastUsed.ShouldBeNull();
        result.Value.Trend.ShouldBe(UsageTrend.Unknown);
    }

    /// <summary>
    /// Verifies that FindAntiPatternsAsync returns success with violations.
    /// </summary>
    [Fact]
    public async Task FindAntiPatternsAsync_Should_Return_Success_With_Violations()
    {
        // Arrange
        var category = "Performance";
        var severity = PatternSeverity.Warning;
        var expectedViolations = new List<AntiPatternViolation>
        {
            new(
                Id: "anti1",
                AntiPatternId: "n-plus-one",
                AntiPatternName: "N+1 Query Problem",
                Severity: PatternSeverity.Warning,
                Message: "Multiple database queries in a loop",
                FilePath: "DataService.cs",
                LineNumber: 25,
                CodeSnippet: "foreach (var item in items) { db.Query(item.Id); }",
                SuggestedFix: "Use batch loading or include statements")
        };

        _mockPatternGraphQueryService.FindAntiPatternsAsync(category, severity, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<AntiPatternViolation>>.Success(expectedViolations));

        // Act
        var result = await _mockPatternGraphQueryService.FindAntiPatternsAsync(category, severity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Id.ShouldBe("anti1");
        result.Value[0].AntiPatternName.ShouldBe("N+1 Query Problem");
        result.Value[0].Severity.ShouldBe(PatternSeverity.Warning);
        result.Value[0].Location.ShouldBe("DataService.cs:25");
    }

    /// <summary>
    /// Verifies that FindAntiPatternsAsync returns empty list for clean code.
    /// </summary>
    [Fact]
    public async Task FindAntiPatternsAsync_Should_Return_Empty_List_For_Clean_Code()
    {
        // Arrange
        var category = "Security";
        var severity = PatternSeverity.Error;

        _mockPatternGraphQueryService.FindAntiPatternsAsync(category, severity, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<AntiPatternViolation>>.Success([]));

        // Act
        var result = await _mockPatternGraphQueryService.FindAntiPatternsAsync(category, severity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that FindAntiPatternsAsync handles null category.
    /// </summary>
    [Fact]
    public async Task FindAntiPatternsAsync_Should_Handle_Null_Category()
    {
        // Arrange
        var severity = PatternSeverity.Info;
        var expectedViolations = new List<AntiPatternViolation>
        {
            new(
                Id: "anti1",
                AntiPatternId: "god-class",
                AntiPatternName: "God Class",
                Severity: PatternSeverity.Info,
                Message: "Class with too many responsibilities")
        };

        _mockPatternGraphQueryService.FindAntiPatternsAsync(null, severity, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<AntiPatternViolation>>.Success(expectedViolations));

        // Act
        var result = await _mockPatternGraphQueryService.FindAntiPatternsAsync(null, severity, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
    }

    /// <summary>
    /// Verifies that GetPatternEvolutionAsync returns success with evolution history.
    /// </summary>
    [Fact]
    public async Task GetPatternEvolutionAsync_Should_Return_Success_With_Evolution_History()
    {
        // Arrange
        var patternId = "singleton";
        var expectedEvolution = new List<PatternEvolution>
        {
            new(
                PatternId: patternId,
                Version: "1.0",
                ChangeType: PatternChangeType.Created,
                ChangeDescription: "Initial pattern definition",
                ChangedAt: DateTimeOffset.UtcNow.AddDays(-30),
                ChangedBy: "developer1"),
            new(
                PatternId: patternId,
                Version: "1.1",
                ChangeType: PatternChangeType.Updated,
                ChangeDescription: "Added thread-safety considerations",
                ChangedAt: DateTimeOffset.UtcNow.AddDays(-15),
                ChangedBy: "developer2")
        };

        _mockPatternGraphQueryService.GetPatternEvolutionAsync(patternId, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternEvolution>>.Success(expectedEvolution));

        // Act
        var result = await _mockPatternGraphQueryService.GetPatternEvolutionAsync(patternId, CancellationToken.None);

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
    /// Verifies that GetPatternEvolutionAsync returns empty list for new pattern.
    /// </summary>
    [Fact]
    public async Task GetPatternEvolutionAsync_Should_Return_Empty_List_For_New_Pattern()
    {
        // Arrange
        var patternId = "new-pattern";

        _mockPatternGraphQueryService.GetPatternEvolutionAsync(patternId, Arg.Any<CancellationToken>())
            .Returns(Result<IReadOnlyList<PatternEvolution>>.Success([]));

        // Act
        var result = await _mockPatternGraphQueryService.GetPatternEvolutionAsync(patternId, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(0);
    }

    /// <summary>
    /// Verifies that all methods handle cancellation token.
    /// </summary>
    [Fact]
    public async Task All_Methods_Should_Handle_Cancellation_Token()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockPatternGraphQueryService.QueryPatternGraphAsync(Arg.Any<PatternGraphQuery>(), cts.Token)
            .Returns(Result<PatternGraphResult>.WithFailure("Cancelled"));
        _mockPatternGraphQueryService.FindPatternRelationshipsAsync(Arg.Any<string>(), Arg.Any<int>(), cts.Token)
            .Returns(Result<IReadOnlyList<PatternRelationship>>.WithFailure("Cancelled"));
        _mockPatternGraphQueryService.FindSimilarPatternsAsync(Arg.Any<string>(), Arg.Any<float>(), Arg.Any<int>(), cts.Token)
            .Returns(Result<IReadOnlyList<PatternSimilarity>>.WithFailure("Cancelled"));
        _mockPatternGraphQueryService.GetPatternUsageStatisticsAsync(Arg.Any<string>(), cts.Token)
            .Returns(Result<PatternUsageStatistics>.WithFailure("Cancelled"));
        _mockPatternGraphQueryService.FindAntiPatternsAsync(Arg.Any<string>(), Arg.Any<PatternSeverity>(), cts.Token)
            .Returns(Result<IReadOnlyList<AntiPatternViolation>>.WithFailure("Cancelled"));
        _mockPatternGraphQueryService.GetPatternEvolutionAsync(Arg.Any<string>(), cts.Token)
            .Returns(Result<IReadOnlyList<PatternEvolution>>.WithFailure("Cancelled"));

        // Act & Assert
        var queryResult = await _mockPatternGraphQueryService.QueryPatternGraphAsync(
            new PatternGraphQuery("test", null, 10, 30000), cts.Token);
        queryResult.IsFailure.ShouldBeTrue();

        var relationshipsResult = await _mockPatternGraphQueryService.FindPatternRelationshipsAsync("test", 1, cts.Token);
        relationshipsResult.IsFailure.ShouldBeTrue();

        var similaritiesResult = await _mockPatternGraphQueryService.FindSimilarPatternsAsync("test", 0.5f, 5, cts.Token);
        similaritiesResult.IsFailure.ShouldBeTrue();

        var statsResult = await _mockPatternGraphQueryService.GetPatternUsageStatisticsAsync("test", cts.Token);
        statsResult.IsFailure.ShouldBeTrue();

        var antiPatternsResult = await _mockPatternGraphQueryService.FindAntiPatternsAsync("test", PatternSeverity.Info, cts.Token);
        antiPatternsResult.IsFailure.ShouldBeTrue();

        var evolutionResult = await _mockPatternGraphQueryService.GetPatternEvolutionAsync("test", cts.Token);
        evolutionResult.IsFailure.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that QueryPatternGraphAsync handles null parameters.
    /// </summary>
    [Fact]
    public async Task QueryPatternGraphAsync_Should_Handle_Null_Parameters()
    {
        // Arrange
        var query = new PatternGraphQuery(
            Query: "MATCH (p:Pattern) RETURN p",
            Parameters: null,
            MaxResults: 10,
            TimeoutMs: 60000);

        var expectedResult = new PatternGraphResult(
            Patterns: [],
            Relationships: [],
            ExecutionTimeMs: 100,
            TotalResults: 0,
            HasMoreResults: false);

        _mockPatternGraphQueryService.QueryPatternGraphAsync(query, Arg.Any<CancellationToken>())
            .Returns(Result<PatternGraphResult>.Success(expectedResult));

        // Act
        var result = await _mockPatternGraphQueryService.QueryPatternGraphAsync(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBe<PatternGraphResult>(default);
        result.Value.PatternCount.ShouldBe(0);
        result.Value.RelationshipCount.ShouldBe(0);
    }
}
