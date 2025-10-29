using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;

namespace IndFusion.SemanticRag.Tests.Interfaces;

/// <summary>
/// ITDD tests for IKnowledgeGraphServicePort interface contracts.
/// These tests verify that any implementation satisfies the interface contract using mocks.
/// </summary>
public class IKnowledgeGraphServicePortTests
{
    private readonly IKnowledgeGraphServicePort _mockKnowledgeGraphService;

    public IKnowledgeGraphServicePortTests()
    {
        _mockKnowledgeGraphService = Substitute.For<IKnowledgeGraphServicePort>();
    }

    [Fact]
    public async Task CreateEntityAsync_Should_Create_Entity_Successfully()
    {
        // Arrange
        var entity = new KnowledgeEntity(
            Id: "entity-1",
            Name: "Test Entity",
            Type: "Person",
            Description: "A test entity",
            Properties: new Dictionary<string, object> { ["age"] = 30 }
        );

        _mockKnowledgeGraphService.CreateEntityAsync(entity, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.CreateEntityAsync(entity, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateEntitiesAsync_Should_Create_Multiple_Entities()
    {
        // Arrange
        var entities = new List<KnowledgeEntity>
        {
            new("entity-1", "Entity 1", "Person", "First entity", new Dictionary<string, object>()),
            new("entity-2", "Entity 2", "Organization", "Second entity", new Dictionary<string, object>())
        };

        _mockKnowledgeGraphService.CreateEntitiesAsync(entities, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.CreateEntitiesAsync(entities, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateRelationshipAsync_Should_Create_Relationship_Successfully()
    {
        // Arrange
        var relationship = new KnowledgeRelationship(
            Id: "rel-1",
            FromNodeId: "entity-1",
            ToNodeId: "entity-2",
            RelationshipType: "WORKS_FOR",
            Properties: new Dictionary<string, object> { ["since"] = "2020" },
            CreatedAt: DateTimeOffset.UtcNow
        );

        _mockKnowledgeGraphService.CreateRelationshipAsync(relationship, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.CreateRelationshipAsync(relationship, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task CreateRelationshipsAsync_Should_Create_Multiple_Relationships()
    {
        // Arrange
        var relationships = new List<KnowledgeRelationship>
        {
            new("rel-1", "entity-1", "entity-2", "WORKS_FOR", new Dictionary<string, object>(), DateTimeOffset.UtcNow),
            new("rel-2", "entity-2", "entity-3", "MANAGES", new Dictionary<string, object>(), DateTimeOffset.UtcNow)
        };

        _mockKnowledgeGraphService.CreateRelationshipsAsync(relationships, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.CreateRelationshipsAsync(relationships, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task GetEntityAsync_Should_Return_Entity_When_Found()
    {
        // Arrange
        var entityId = "entity-1";
        var expectedEntity = new KnowledgeEntity(
            Id: entityId,
            Name: "Test Entity",
            Type: "Person",
            Description: "A test entity",
            Properties: new Dictionary<string, object>()
        );

        _mockKnowledgeGraphService.GetEntityAsync(entityId, CancellationToken.None)
            .Returns(Result<KnowledgeEntity>.Success(expectedEntity));

        // Act
        var result = await _mockKnowledgeGraphService.GetEntityAsync(entityId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(entityId);
        result.Value.Name.ShouldBe("Test Entity");
        result.Value.Type.ShouldBe("Person");
    }

    [Fact]
    public async Task GetEntityAsync_Should_Return_Failure_When_Not_Found()
    {
        // Arrange
        var entityId = "non-existent";

        _mockKnowledgeGraphService.GetEntityAsync(entityId, CancellationToken.None)
            .Returns(Result<KnowledgeEntity>.WithFailure("Entity not found"));

        // Act
        var result = await _mockKnowledgeGraphService.GetEntityAsync(entityId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Entity not found");
    }

    [Fact]
    public async Task GetEntitiesAsync_Should_Return_Multiple_Entities()
    {
        // Arrange
        var entityIds = new List<string> { "entity-1", "entity-2" };
        var expectedEntities = new List<KnowledgeEntity>
        {
            new("entity-1", "Entity 1", "Person", "First entity", new Dictionary<string, object>()),
            new("entity-2", "Entity 2", "Organization", "Second entity", new Dictionary<string, object>())
        };

        _mockKnowledgeGraphService.GetEntitiesAsync(entityIds, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphService.GetEntitiesAsync(entityIds, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetRelationshipAsync_Should_Return_Relationship_When_Found()
    {
        // Arrange
        var relationshipId = "rel-1";
        var expectedRelationship = new KnowledgeRelationship(
            Id: relationshipId,
            FromNodeId: "entity-1",
            ToNodeId: "entity-2",
            RelationshipType: "WORKS_FOR",
            Properties: new Dictionary<string, object>(),
            CreatedAt: DateTimeOffset.UtcNow
        );

        _mockKnowledgeGraphService.GetRelationshipAsync(relationshipId, CancellationToken.None)
            .Returns(Result<KnowledgeRelationship>.Success(expectedRelationship));

        // Act
        var result = await _mockKnowledgeGraphService.GetRelationshipAsync(relationshipId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(relationshipId);
        result.Value.RelationshipType.ShouldBe("WORKS_FOR");
    }

    [Fact]
    public async Task SearchEntitiesAsync_Should_Return_Matching_Entities()
    {
        // Arrange
        var entityType = "Person";
        var properties = new Dictionary<string, object> { ["age"] = 30 };
        var expectedEntities = new List<KnowledgeEntity>
        {
            new("entity-1", "John Doe", "Person", "A person", new Dictionary<string, object> { ["age"] = 30 }),
            new("entity-2", "Jane Smith", "Person", "Another person", new Dictionary<string, object> { ["age"] = 30 })
        };

        _mockKnowledgeGraphService.SearchEntitiesAsync(entityType, properties, 100, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphService.SearchEntitiesAsync(entityType, properties, 100, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value.All(e => e.Type == "Person").ShouldBeTrue();
    }

    [Fact]
    public async Task SearchRelationshipsAsync_Should_Return_Matching_Relationships()
    {
        // Arrange
        var relationshipType = "WORKS_FOR";
        var properties = new Dictionary<string, object> { ["department"] = "Engineering" };
        var expectedRelationships = new List<KnowledgeRelationship>
        {
            new("rel-1", "entity-1", "entity-2", "WORKS_FOR", new Dictionary<string, object> { ["department"] = "Engineering" }, DateTimeOffset.UtcNow),
            new("rel-2", "entity-3", "entity-4", "WORKS_FOR", new Dictionary<string, object> { ["department"] = "Engineering" }, DateTimeOffset.UtcNow)
        };

        _mockKnowledgeGraphService.SearchRelationshipsAsync(relationshipType, properties, 100, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _mockKnowledgeGraphService.SearchRelationshipsAsync(relationshipType, properties, 100, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value.All(r => r.RelationshipType == "WORKS_FOR").ShouldBeTrue();
    }

    [Fact]
    public async Task FindConnectedEntitiesAsync_Should_Return_Connected_Entities()
    {
        // Arrange
        var entityId = "entity-1";
        var relationshipTypes = new List<string> { "WORKS_FOR", "MANAGES" };
        var expectedEntities = new List<KnowledgeEntity>
        {
            new("entity-2", "Connected Entity 1", "Person", "Connected entity", new Dictionary<string, object>()),
            new("entity-3", "Connected Entity 2", "Organization", "Another connected entity", new Dictionary<string, object>())
        };

        _mockKnowledgeGraphService.FindConnectedEntitiesAsync(entityId, relationshipTypes, 2, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphService.FindConnectedEntitiesAsync(entityId, relationshipTypes, 2, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact]
    public async Task FindPathsAsync_Should_Return_Paths_Between_Entities()
    {
        // Arrange
        var fromEntityId = "entity-1";
        var toEntityId = "entity-3";
        var expectedPaths = new List<GraphPath>
        {
            new(
                Nodes: new List<GraphNode>
                {
                    new("entity-1", "Person", new Dictionary<string, object>()),
                    new("entity-2", "Organization", new Dictionary<string, object>()),
                    new("entity-3", "Person", new Dictionary<string, object>())
                },
                Relationships: new List<GraphRelationship>
                {
                    new("rel-1", "entity-1", "entity-2", "WORKS_FOR", new Dictionary<string, object>()),
                    new("rel-2", "entity-2", "entity-3", "MANAGES", new Dictionary<string, object>())
                },
                TotalWeight: 2.0
            )
        };

        _mockKnowledgeGraphService.FindPathsAsync(fromEntityId, toEntityId, 5, CancellationToken.None)
            .Returns(Result<IReadOnlyList<GraphPath>>.Success(expectedPaths));

        // Act
        var result = await _mockKnowledgeGraphService.FindPathsAsync(fromEntityId, toEntityId, 5, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Nodes.Count().ShouldBe(3);
        result.Value[0].Relationships.Count().ShouldBe(2);
    }

    [Fact]
    public async Task UpdateEntityAsync_Should_Update_Entity_Successfully()
    {
        // Arrange
        var entity = new KnowledgeEntity(
            Id: "entity-1",
            Name: "Updated Entity",
            Type: "Person",
            Description: "An updated entity",
            Properties: new Dictionary<string, object> { ["updated"] = true }
        );

        _mockKnowledgeGraphService.UpdateEntityAsync(entity, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.UpdateEntityAsync(entity, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task UpdateRelationshipAsync_Should_Update_Relationship_Successfully()
    {
        // Arrange
        var relationship = new KnowledgeRelationship(
            Id: "rel-1",
            FromNodeId: "entity-1",
            ToNodeId: "entity-2",
            RelationshipType: "WORKS_FOR",
            Properties: new Dictionary<string, object> { ["updated"] = true },
            CreatedAt: DateTimeOffset.UtcNow
        );

        _mockKnowledgeGraphService.UpdateRelationshipAsync(relationship, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.UpdateRelationshipAsync(relationship, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteEntityAsync_Should_Delete_Entity_Successfully()
    {
        // Arrange
        var entityId = "entity-1";

        _mockKnowledgeGraphService.DeleteEntityAsync(entityId, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.DeleteEntityAsync(entityId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task DeleteRelationshipAsync_Should_Delete_Relationship_Successfully()
    {
        // Arrange
        var relationshipId = "rel-1";

        _mockKnowledgeGraphService.DeleteRelationshipAsync(relationshipId, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.DeleteRelationshipAsync(relationshipId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task GetStatisticsAsync_Should_Return_Graph_Statistics()
    {
        // Arrange
        var expectedStats = new KnowledgeGraphStatistics(
            TotalNodes: 1000,
            TotalRelationships: 2000,
            NodeTypes: new Dictionary<string, long> { ["Person"] = 500, ["Organization"] = 300, ["Location"] = 200 },
            RelationshipTypes: new Dictionary<string, long> { ["WORKS_FOR"] = 800, ["MANAGES"] = 200, ["LOCATED_IN"] = 1000 },
            LastUpdated: DateTimeOffset.UtcNow
        );

        _mockKnowledgeGraphService.GetStatisticsAsync(CancellationToken.None)
            .Returns(Result<KnowledgeGraphStatistics>.Success(expectedStats));

        // Act
        var result = await _mockKnowledgeGraphService.GetStatisticsAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalNodes.ShouldBe(1000);
        result.Value.TotalRelationships.ShouldBe(2000);
        result.Value.NodeTypes.Count.ShouldBe(3);
        result.Value.RelationshipTypes.Count.ShouldBe(3);
    }

    [Fact]
    public async Task ClearAsync_Should_Clear_All_Data()
    {
        // Arrange
        _mockKnowledgeGraphService.ClearAsync(CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphService.ClearAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Theory]
    [InlineData("Person", 0)]
    [InlineData("Organization", 5)]
    [InlineData("Location", 10)]
    public async Task SearchEntitiesAsync_Should_Respect_Limit_Parameter(string entityType, int limit)
    {
        // Arrange
        var expectedEntities = Enumerable.Range(0, Math.Min(limit, 10))
            .Select(i => new KnowledgeEntity($"entity-{i}", $"Entity {i}", entityType, $"Description {i}", new Dictionary<string, object>()))
            .ToList();

        _mockKnowledgeGraphService.SearchEntitiesAsync(entityType, null, limit, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphService.SearchEntitiesAsync(entityType, null, limit, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBeLessThanOrEqualTo(limit);
    }
}
