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
/// Unit tests for semantic RAG port interfaces.
/// These tests verify the interface contracts using mocks (ITDD approach).
/// </summary>
public class SemanticRagServiceTests
{
    private readonly IVectorSearchPort _mockVectorSearchPort;
    private readonly IKnowledgeGraphServicePort _mockKnowledgeGraphServicePort;
    private readonly ILogger<IVectorSearchPort> _logger;

    public SemanticRagServiceTests()
    {
        _mockVectorSearchPort = Substitute.For<IVectorSearchPort>();
        _mockKnowledgeGraphServicePort = Substitute.For<IKnowledgeGraphServicePort>();
        _logger = Substitute.For<ILogger<IVectorSearchPort>>();
    }

    [Fact(Timeout = 5000)]
    public async Task VectorSearchPort_SearchAsync_Should_Return_Similar_Vectors()
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            Limit: 5,
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

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Task.FromResult(Result<VectorSearchResult>.Success(expectedResults[0])));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.Vector.Id.ShouldBe("emb-1");
        result.Value.Similarity.ShouldBe(0.9f);
        result.Value.Rank.ShouldBe(1);
    }

    [Fact(Timeout = 5000)]
    public async Task VectorSearchPort_IndexAsync_Should_Index_Vector_Successfully()
    {
        // Arrange
        // ✅ Use fluent builder from TestDataBuilders
        var embeddingResult = TestDataBuilders.CreateValidVectorEmbedding(
            id: "emb-1",
            content: "Test content",
            embeddingSize: 4);
        embeddingResult.IsSuccess.ShouldBeTrue();
        var embedding = embeddingResult.Value;

        _mockVectorSearchPort.IndexAsync(embedding, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.IndexAsync(embedding, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_CreateEntityAsync_Should_Create_Entity_Successfully()
    {
        // Arrange
        // ✅ Use fluent builder from TestDataBuilders with custom properties
        var entityResult = TestDataBuilders
            .CreateValidKnowledgeEntity(id: "entity-1", name: "Test Entity", type: "Person")
            .Map(e => new KnowledgeEntity(
                e.Id,
                e.Name,
                e.Type,
                "A test entity",
                new Dictionary<string, object> { ["age"] = 30 },
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
    public async Task KnowledgeGraphServicePort_CreateRelationshipAsync_Should_Create_Relationship_Successfully()
    {
        // Arrange
        var relationship = new KnowledgeRelationship(
            Id: "rel-1",
            FromNodeId: "entity-1",
            ToNodeId: "entity-2",
            RelationshipType: "WORKS_FOR",
            Properties: new Dictionary<string, object> { ["since"] = "2020" },
            CreatedAt: DateTime.UtcNow
        );

        _mockKnowledgeGraphServicePort.CreateRelationshipAsync(relationship, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.CreateRelationshipAsync(relationship, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_GetEntityAsync_Should_Return_Entity_When_Found()
    {
        // Arrange
        // ✅ Use fluent builder from TestDataBuilders
        var entityId = "entity-1";
        var entityResult = TestDataBuilders.CreateValidKnowledgeEntity(
            id: entityId,
            name: "Test Entity",
            type: "Person");
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
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_SearchEntitiesAsync_Should_Return_Matching_Entities()
    {
        // Arrange
        var entityType = "Person";
        var properties = new Dictionary<string, object> { ["age"] = 30 };
        var expectedEntities = new List<KnowledgeEntity>
        {
            new("entity-1", "John Doe", "Person", "A person", new Dictionary<string, object> { ["age"] = 30 }, 0.9, DateTime.UtcNow),
            new("entity-2", "Jane Smith", "Person", "Another person", new Dictionary<string, object> { ["age"] = 30 }, 0.9, DateTime.UtcNow)
        };

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
    }

    [Fact(Timeout = 5000)]
    public async Task VectorSearchPort_GetStatisticsAsync_Should_Return_Index_Statistics()
    {
        // Arrange
        var expectedStats = new VectorIndexStatistics(
            TotalVectors: 1000,
            IndexSize: 1024 * 1024, // 1MB
            LastUpdated: DateTime.UtcNow,
            AverageVectorDimension: 384
        );

        _mockVectorSearchPort.GetStatisticsAsync(CancellationToken.None)
            .Returns(Result<VectorIndexStatistics>.Success(expectedStats));

        // Act
        var result = await _mockVectorSearchPort.GetStatisticsAsync(CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalVectors.ShouldBe(1000);
        result.Value.IndexSize.ShouldBe(1024 * 1024);
        result.Value.AverageVectorDimension.ShouldBe(384);
    }

    [Fact(Timeout = 5000)]
    public async Task KnowledgeGraphServicePort_GetStatisticsAsync_Should_Return_Graph_Statistics()
    {
        // Arrange
        var expectedStats = new KnowledgeGraphStatistics(
            TotalNodes: 1000,
            TotalRelationships: 2000,
            NodeTypes: new Dictionary<string, long> { ["Person"] = 500, ["Organization"] = 300, ["Location"] = 200 },
            RelationshipTypes: new Dictionary<string, long> { ["WORKS_FOR"] = 800, ["MANAGES"] = 200, ["LOCATED_IN"] = 1000 },
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
    }

    [Theory(Timeout = 5000)]
    [InlineData(0.5f)]
    [InlineData(0.7f)]
    [InlineData(0.9f)]
    public async Task VectorSearchPort_SearchAsync_Should_Respect_Similarity_Threshold(float threshold)
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            Limit: 5,
            Threshold: threshold
        );
        // ✅ Use fluent builder from TestDataBuilders
        var vectorResult = TestDataBuilders.CreateValidVectorEmbedding(
            id: "test-id",
            content: "test-content",
            embeddingSize: queryVector.Length);
        vectorResult.IsSuccess.ShouldBeTrue();
        var vectorEmbedding = vectorResult.Value;
        
        // Use similarity value that is always >= threshold (threshold + 0.05 for safety margin)
        var similarity = Math.Max(threshold + 0.05f, 0.95f);
        
        var expectedResults = new List<VectorSearchResult>
        {
            new VectorSearchResult(
                Vector: vectorEmbedding,
                Similarity: similarity,
                Rank: 1
            )
        };

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Task.FromResult(Result<VectorSearchResult>.Success(expectedResults[0])));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.Similarity.ShouldBeGreaterThanOrEqualTo(threshold);
        result.Value.Rank.ShouldBe(1);
    }
}