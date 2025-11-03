using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Services;

/// <summary>
/// Unit tests for knowledge extraction port interfaces.
/// These tests verify the interface contracts using mocks (ITDD approach).
/// </summary>
public class KnowledgeExtractionServiceTests
{
    private readonly IKnowledgeGraphServicePort _mockKnowledgeGraphServicePort;
    private readonly ILogger<IKnowledgeGraphServicePort> _logger;

    public KnowledgeExtractionServiceTests()
    {
        _mockKnowledgeGraphServicePort = Substitute.For<IKnowledgeGraphServicePort>();
        _logger = Substitute.For<ILogger<IKnowledgeGraphServicePort>>();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_CreateEntityAsync_Should_Create_Entity_With_Properties()
    {
        // ✅ Use fluent builder from TestDataBuilders with custom properties
        var entityResult = TestDataBuilders
            .CreateValidKnowledgeEntity(
                id: "entity-1",
                name: "John Doe",
                type: "Person")
            .Map(e => new KnowledgeEntity(
                e.Id,
                e.Name,
                e.Type,
                "Software Engineer",
                new Dictionary<string, object>
                { 
                    ["age"] = 30,
                    ["department"] = "Engineering",
                    ["skills"] = new[] { "C#", "TypeScript", "React" }
                },
                0.9,
                DateTime.UtcNow));
        entityResult.IsSuccess.ShouldBeTrue();
        var entity = entityResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

        _mockKnowledgeGraphServicePort.CreateEntityAsync(entity, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.CreateEntityAsync(entity, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_CreateRelationshipAsync_Should_Create_Relationship_With_Properties()
    {
        // ✅ Use fluent builder from TestDataBuilders with custom properties
        var relationshipResult = TestDataBuilders
            .CreateValidKnowledgeRelationship(
                id: "rel-1",
                fromNodeId: "entity-1",
                toNodeId: "entity-2")
            .Map(r => new KnowledgeRelationship(
                r.Id,
                r.FromNodeId,
                r.ToNodeId,
                "WORKS_FOR",
                new Dictionary<string, object> 
                { 
                    ["since"] = "2020",
                    ["position"] = "Senior Developer",
                    ["team"] = "Backend"
                },
                DateTimeOffset.UtcNow));
        relationshipResult.IsSuccess.ShouldBeTrue();
        var relationship = relationshipResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

        _mockKnowledgeGraphServicePort.CreateRelationshipAsync(relationship, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.CreateRelationshipAsync(relationship, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_SearchEntitiesAsync_Should_Filter_By_Properties()
    {
        // Arrange
        var entityType = "Person";
        var properties = new Dictionary<string, object> 
        { 
            ["department"] = "Engineering",
            ["age"] = 30
        };
        // ✅ Use fluent builders from TestDataBuilders
        var entity1Result = TestDataBuilders
            .CreateValidKnowledgeEntity(id: "entity-1", name: "John Doe", type: "Person")
            .Map(e => new KnowledgeEntity(e.Id, e.Name, e.Type, "Senior Developer",
                new Dictionary<string, object> { ["department"] = "Engineering", ["age"] = 30 },
                0.9, DateTime.UtcNow));
        var entity2Result = TestDataBuilders
            .CreateValidKnowledgeEntity(id: "entity-2", name: "Jane Smith", type: "Person")
            .Map(e => new KnowledgeEntity(e.Id, e.Name, e.Type, "Tech Lead",
                new Dictionary<string, object> { ["department"] = "Engineering", ["age"] = 32 },
                0.9, DateTime.UtcNow));
        entity1Result.IsSuccess.ShouldBeTrue();
        entity2Result.IsSuccess.ShouldBeTrue();
        var expectedEntities = new List<KnowledgeEntity> { entity1Result.Value!, entity2Result.Value! }; // Null-forgiving: IsSuccess guarantees non-null

        _mockKnowledgeGraphServicePort.SearchEntitiesAsync(entityType, properties, 100, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphServicePort.SearchEntitiesAsync(entityType, properties, 100, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value.All(e => e.Type == "Person").ShouldBeTrue();
        result.Value.All(e => e.Properties.ContainsKey("department") && e.Properties["department"].ToString() == "Engineering").ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_SearchRelationshipsAsync_Should_Filter_By_Properties()
    {
        // Arrange
        var relationshipType = "WORKS_FOR";
        var properties = new Dictionary<string, object> 
        { 
            ["department"] = "Engineering",
            ["position"] = "Senior"
        };
        var expectedRelationships = new List<KnowledgeRelationship>
        {
            new("rel-1", "entity-1", "entity-2", "WORKS_FOR", 
                new Dictionary<string, object> { ["department"] = "Engineering", ["position"] = "Senior Developer" }, 
                DateTimeOffset.UtcNow),
            new("rel-2", "entity-3", "entity-4", "WORKS_FOR", 
                new Dictionary<string, object> { ["department"] = "Engineering", ["position"] = "Senior Architect" }, 
                DateTimeOffset.UtcNow)
        };

        _mockKnowledgeGraphServicePort.SearchRelationshipsAsync(relationshipType, properties, 100, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _mockKnowledgeGraphServicePort.SearchRelationshipsAsync(relationshipType, properties, 100, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value.All(r => r.RelationshipType == "WORKS_FOR").ShouldBeTrue();
        result.Value.All(r => r.Properties.ContainsKey("department") && r.Properties["department"].ToString() == "Engineering").ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_FindConnectedEntitiesAsync_Should_Return_Connected_Entities_With_Depth()
    {
        // Arrange
        var entityId = "entity-1";
        var relationshipTypes = new List<string> { "WORKS_FOR", "MANAGES", "COLLABORATES_WITH" };
        var maxDepth = 3;
        var expectedEntities = new List<KnowledgeEntity>
        {
            new("entity-2", "Direct Report 1", "Person", "Direct report", new Dictionary<string, object>(), 0.9, DateTime.UtcNow),
            new("entity-3", "Direct Report 2", "Person", "Another direct report", new Dictionary<string, object>(), 0.9, DateTime.UtcNow),
            new("entity-4", "Colleague 1", "Person", "Colleague", new Dictionary<string, object>(), 0.9, DateTime.UtcNow),
            new("entity-5", "Manager", "Person", "Manager", new Dictionary<string, object>(), 0.9, DateTime.UtcNow)
        };

        _mockKnowledgeGraphServicePort.FindConnectedEntitiesAsync(entityId, relationshipTypes, maxDepth, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphServicePort.FindConnectedEntitiesAsync(entityId, relationshipTypes, maxDepth, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(4);
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_FindPathsAsync_Should_Return_Paths_Between_Entities()
    {
        // Arrange
        var fromEntityId = "entity-1";
        var toEntityId = "entity-5";
        var maxPathLength = 3;
        var expectedPaths = new List<GraphPath>
        {
            new(
                Nodes: new List<GraphNode>
                {
                    new("entity-1", "Person", new Dictionary<string, object>(), new List<string> { "Person" }),
                    new("entity-2", "Person", new Dictionary<string, object>(), new List<string> { "Person" }),
                    new("entity-5", "Person", new Dictionary<string, object>(), new List<string> { "Person" })
                },
                Relationships: new List<GraphRelationship>
                {
                    new("rel-1", "entity-1", "entity-2", "MANAGES", new Dictionary<string, object>()),
                    new("rel-2", "entity-2", "entity-5", "COLLABORATES_WITH", new Dictionary<string, object>())
                },
                Length: 2
            )
        };

        _mockKnowledgeGraphServicePort.FindPathsAsync(fromEntityId, toEntityId, maxPathLength, CancellationToken.None)
            .Returns(Result<IReadOnlyList<GraphPath>>.Success(expectedPaths));

        // Act
        var result = await _mockKnowledgeGraphServicePort.FindPathsAsync(fromEntityId, toEntityId, maxPathLength, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Nodes.Count().ShouldBe(3);
        result.Value[0].Relationships.Count().ShouldBe(2);
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_GetStatisticsAsync_Should_Return_Detailed_Statistics()
    {
        // Arrange
        var expectedStats = new KnowledgeGraphStatistics(
            TotalNodes: 1000,
            TotalRelationships: 2000,
            NodeTypes: new Dictionary<string, long> 
            { 
                ["Person"] = 500, 
                ["Organization"] = 300, 
                ["Location"] = 200 
            },
            RelationshipTypes: new Dictionary<string, long> 
            { 
                ["WORKS_FOR"] = 800, 
                ["MANAGES"] = 200, 
                ["LOCATED_IN"] = 1000 
            },
            LastUpdated: DateTime.UtcNow
        );

        _mockKnowledgeGraphServicePort.GetStatisticsAsync(CancellationToken.None)
            .Returns(Result<KnowledgeGraphStatistics>.Success(expectedStats));

        // Act
        var result = await _mockKnowledgeGraphServicePort.GetStatisticsAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalNodes.ShouldBe(1000);
        result.Value.TotalRelationships.ShouldBe(2000);
        result.Value.NodeTypes.Count.ShouldBe(3);
        result.Value.RelationshipTypes.Count.ShouldBe(3);
        result.Value.NodeTypes["Person"].ShouldBe(500);
        result.Value.RelationshipTypes["WORKS_FOR"].ShouldBe(800);
    }

    [Theory(Timeout = 5000)]
    [InlineData("Person", 0)]
    [InlineData("Organization", 5)]
    [InlineData("Location", 10)]
    public async Task KnowledgeGraphServicePort_SearchEntitiesAsync_Should_Respect_Limit_Parameter(string entityType, int limit)
    {
        // ✅ Use fluent builders from TestDataBuilders
        var expectedEntities = new List<KnowledgeEntity>();
        for (int i = 0; i < Math.Min(limit, 10); i++)
        {
            var entityResult = TestDataBuilders
                .CreateValidKnowledgeEntity(id: $"entity-{i}", name: $"Entity {i}", type: entityType)
                .Map(e => new KnowledgeEntity(e.Id, e.Name, e.Type, $"Description {i}",
                    new Dictionary<string, object>(), 0.9, DateTime.UtcNow));
            entityResult.IsSuccess.ShouldBeTrue();
            expectedEntities.Add(entityResult.Value!); // Null-forgiving: IsSuccess guarantees non-null
        }

        _mockKnowledgeGraphServicePort.SearchEntitiesAsync(entityType, null, limit, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphServicePort.SearchEntitiesAsync(entityType, null, limit, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeLessThanOrEqualTo(limit);
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_GetEntityAsync_Should_Return_Entity_When_Found()
    {
        // ✅ Use fluent builder from TestDataBuilders
        var entityId = "entity-1";
        var entityResult = TestDataBuilders
            .CreateValidKnowledgeEntity(id: entityId, name: "Test Entity", type: "Person")
            .Map(e => new KnowledgeEntity(
                e.Id,
                e.Name,
                e.Type,
                "A test entity",
                new Dictionary<string, object> { ["test"] = true },
                0.9,
                DateTime.UtcNow));
        entityResult.IsSuccess.ShouldBeTrue();
        var expectedEntity = entityResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

        _mockKnowledgeGraphServicePort.GetEntityAsync(entityId, CancellationToken.None)
            .Returns(Result<KnowledgeEntity>.Success(expectedEntity));

        // Act
        var result = await _mockKnowledgeGraphServicePort.GetEntityAsync(entityId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(entityId);
        result.Value.Name.ShouldBe("Test Entity");
        result.Value.Type.ShouldBe("Person");
        result.Value.Properties["test"].ShouldBe(true);
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_GetEntityAsync_Should_Return_Failure_When_Not_Found()
    {
        // Arrange
        var entityId = "non-existent";

        _mockKnowledgeGraphServicePort.GetEntityAsync(entityId, CancellationToken.None)
            .Returns(Result<KnowledgeEntity>.WithFailure("Entity not found"));

        // Act
        var result = await _mockKnowledgeGraphServicePort.GetEntityAsync(entityId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }
}