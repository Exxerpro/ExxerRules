using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Domain.Tests.Shared;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IndFusion.SemanticRag.Domain.Tests.Domain.Services;

/// <summary>
/// Unit tests for semantic search port interfaces.
/// These tests verify the interface contracts using mocks (ITDD approach).
/// </summary>
public class SemanticSearchServiceTests
{
    private readonly IVectorSearchPort _mockVectorSearchPort;
    private readonly IKnowledgeGraphServicePort _mockKnowledgeGraphServicePort;
    private readonly ILogger<IVectorSearchPort> _logger;

    public SemanticSearchServiceTests()
    {
        _mockVectorSearchPort = Substitute.For<IVectorSearchPort>();
        _mockKnowledgeGraphServicePort = Substitute.For<IKnowledgeGraphServicePort>();
        _logger = Substitute.For<ILogger<IVectorSearchPort>>();
    }

    [Fact(Timeout = 5000)]
    public async Task VectorSearchPort_SearchBatchAsync_Should_Process_Multiple_Queries()
    {
        // Arrange
        var queryVectors = new List<float[]>
        {
            new float[] { 0.1f, 0.2f, 0.3f, 0.4f },
            new float[] { 0.5f, 0.6f, 0.7f, 0.8f }
        };
        var options = new VectorSearchOptions(
            Limit: 3,
            Threshold: 0.7f
        );
        // ✅ Use fluent builders from TestDataBuilders
        var vector1Result = TestDataBuilders.CreateValidVectorEmbedding(
            id: "emb-1",
            content: "Content 1",
            embeddingSize: 4);
        var vector2Result = TestDataBuilders.CreateValidVectorEmbedding(
            id: "emb-2",
            content: "Content 2",
            embeddingSize: 4);
        vector1Result.IsSuccess.ShouldBeTrue();
        vector2Result.IsSuccess.ShouldBeTrue();
        var vectorEmbedding1 = vector1Result.Value;
        var vectorEmbedding2 = vector2Result.Value;
        var expectedResults = new List<VectorSearchResult>
        {
            new VectorSearchResult(Vector: vectorEmbedding1, Similarity: 0.9f, Rank: 1),
            new VectorSearchResult(Vector: vectorEmbedding2, Similarity: 0.85f, Rank: 2)
        };

        _mockVectorSearchPort.SearchBatchAsync(queryVectors, options, CancellationToken.None)
            .Returns(Task.FromResult(Result<IReadOnlyList<VectorSearchResult>>.Success(expectedResults)));

        // Act
        var result = await _mockVectorSearchPort.SearchBatchAsync(queryVectors, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact(Timeout = 5000)]
    public async Task VectorSearchPort_IndexBatchAsync_Should_Index_Multiple_Vectors()
    {
        // Arrange
        var embeddings = new List<VectorEmbedding>
        {
            new("emb-1", "Content 1", new float[] { 0.1f, 0.2f }, new Dictionary<string, object>(), DateTimeOffset.UtcNow),
            new("emb-2", "Content 2", new float[] { 0.3f, 0.4f }, new Dictionary<string, object>(), DateTimeOffset.UtcNow)
        };

        _mockVectorSearchPort.IndexBatchAsync(embeddings, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.IndexBatchAsync(embeddings, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task VectorSearchPort_UpdateAsync_Should_Update_Existing_Vector()
    {
        // Arrange
        // ✅ Use fluent builder from TestDataBuilders
        var embeddingResult = TestDataBuilders.CreateValidVectorEmbedding(
            id: "emb-1",
            content: "Updated content",
            embeddingSize: 4);
        embeddingResult.IsSuccess.ShouldBeTrue();
        var embedding = embeddingResult.Value;

        _mockVectorSearchPort.UpdateAsync(embedding, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.UpdateAsync(embedding, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task VectorSearchPort_DeleteAsync_Should_Delete_Vector_Successfully()
    {
        // Arrange
        var embeddingId = "emb-1";

        _mockVectorSearchPort.DeleteAsync(embeddingId, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.DeleteAsync(embeddingId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task VectorSearchPort_ClearAsync_Should_Clear_All_Vectors()
    {
        // Arrange
        _mockVectorSearchPort.ClearAsync(CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.ClearAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_CreateEntitiesAsync_Should_Create_Multiple_Entities()
    {
        // Arrange
        var entities = new List<KnowledgeEntity>
        {
            new("entity-1", "Entity 1", "Person", "First entity", new Dictionary<string, object>(), 0.9, DateTime.UtcNow),
            new("entity-2", "Entity 2", "Organization", "Second entity", new Dictionary<string, object>(), 0.9, DateTime.UtcNow)
        };

        _mockKnowledgeGraphServicePort.CreateEntitiesAsync(entities, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.CreateEntitiesAsync(entities, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_CreateRelationshipsAsync_Should_Create_Multiple_Relationships()
    {
        // Arrange
        var relationships = new List<KnowledgeRelationship>
        {
            new("rel-1", "entity-1", "entity-2", "WORKS_FOR", new Dictionary<string, object>(), DateTimeOffset.UtcNow),
            new("rel-2", "entity-2", "entity-3", "MANAGES", new Dictionary<string, object>(), DateTimeOffset.UtcNow)
        };

        _mockKnowledgeGraphServicePort.CreateRelationshipsAsync(relationships, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.CreateRelationshipsAsync(relationships, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_GetEntitiesAsync_Should_Return_Multiple_Entities()
    {
        // Arrange
        var entityIds = new List<string> { "entity-1", "entity-2" };
        var expectedEntities = new List<KnowledgeEntity>
        {
            new("entity-1", "Entity 1", "Person", "First entity", new Dictionary<string, object>(), 0.9, DateTime.UtcNow),
            new("entity-2", "Entity 2", "Organization", "Second entity", new Dictionary<string, object>(), 0.9, DateTime.UtcNow)
        };

        _mockKnowledgeGraphServicePort.GetEntitiesAsync(entityIds, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphServicePort.GetEntitiesAsync(entityIds, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_GetRelationshipsAsync_Should_Return_Multiple_Relationships()
    {
        // Arrange
        var relationshipIds = new List<string> { "rel-1", "rel-2" };
        var expectedRelationships = new List<KnowledgeRelationship>
        {
            new("rel-1", "entity-1", "entity-2", "WORKS_FOR", new Dictionary<string, object>(), DateTimeOffset.UtcNow),
            new("rel-2", "entity-2", "entity-3", "MANAGES", new Dictionary<string, object>(), DateTimeOffset.UtcNow)
        };

        _mockKnowledgeGraphServicePort.GetRelationshipsAsync(relationshipIds, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

        // Act
        var result = await _mockKnowledgeGraphServicePort.GetRelationshipsAsync(relationshipIds, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_SearchRelationshipsAsync_Should_Return_Matching_Relationships()
    {
        // Arrange
        var relationshipType = "WORKS_FOR";
        var properties = new Dictionary<string, object> { ["department"] = "Engineering" };
        var expectedRelationships = new List<KnowledgeRelationship>
        {
            new("rel-1", "entity-1", "entity-2", "WORKS_FOR", new Dictionary<string, object> { ["department"] = "Engineering" }, DateTimeOffset.UtcNow),
            new("rel-2", "entity-3", "entity-4", "WORKS_FOR", new Dictionary<string, object> { ["department"] = "Engineering" }, DateTimeOffset.UtcNow)
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
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_FindConnectedEntitiesAsync_Should_Return_Connected_Entities()
    {
        // Arrange
        var entityId = "entity-1";
        var relationshipTypes = new List<string> { "WORKS_FOR", "MANAGES" };
        var expectedEntities = new List<KnowledgeEntity>
        {
            new("entity-2", "Connected Entity 1", "Person", "Connected entity", new Dictionary<string, object>(), 0.9, DateTime.UtcNow),
            new("entity-3", "Connected Entity 2", "Organization", "Another connected entity", new Dictionary<string, object>(), 0.9, DateTime.UtcNow)
        };

        _mockKnowledgeGraphServicePort.FindConnectedEntitiesAsync(entityId, relationshipTypes, 2, CancellationToken.None)
            .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

        // Act
        var result = await _mockKnowledgeGraphServicePort.FindConnectedEntitiesAsync(entityId, relationshipTypes, 2, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_UpdateEntityAsync_Should_Update_Entity_Successfully()
    {
        // Arrange
        var entity = new KnowledgeEntity(
            Id: "entity-1",
            Name: "Updated Entity",
            Type: "Person",
            Description: "An updated entity",
            Properties: new Dictionary<string, object> { ["updated"] = true },
            Confidence: 0.9,
            CreatedAt: DateTime.UtcNow
        );

        _mockKnowledgeGraphServicePort.UpdateEntityAsync(entity, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.UpdateEntityAsync(entity, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_UpdateRelationshipAsync_Should_Update_Relationship_Successfully()
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

        _mockKnowledgeGraphServicePort.UpdateRelationshipAsync(relationship, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.UpdateRelationshipAsync(relationship, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_DeleteEntityAsync_Should_Delete_Entity_Successfully()
    {
        // Arrange
        var entityId = "entity-1";

        _mockKnowledgeGraphServicePort.DeleteEntityAsync(entityId, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.DeleteEntityAsync(entityId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_DeleteRelationshipAsync_Should_Delete_Relationship_Successfully()
    {
        // Arrange
        var relationshipId = "rel-1";

        _mockKnowledgeGraphServicePort.DeleteRelationshipAsync(relationshipId, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.DeleteRelationshipAsync(relationshipId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_ClearAsync_Should_Clear_All_Data()
    {
        // Arrange
        _mockKnowledgeGraphServicePort.ClearAsync(CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.ClearAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }
}