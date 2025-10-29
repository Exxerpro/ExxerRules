using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
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

    [Fact]
    public async Task VectorSearchPort_SearchAsync_Should_Return_Similar_Vectors()
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            QueryVector: queryVector,
            MaxResults: 5,
            SimilarityThreshold: 0.7
        );
        var expectedResult = new VectorSearchResult(
            Id: "search-1",
            QueryId: "query-1",
            Results: new List<VectorEmbedding>
            {
                new("emb-1", "Content 1", new float[] { 0.11f, 0.21f, 0.31f, 0.41f }, 
                    new Dictionary<string, object>(), DateTimeOffset.UtcNow),
                new("emb-2", "Content 2", new float[] { 0.12f, 0.22f, 0.32f, 0.42f }, 
                    new Dictionary<string, object>(), DateTimeOffset.UtcNow)
            },
            TotalCount: 2,
            QueryTime: TimeSpan.FromMilliseconds(50),
            Metadata: new Dictionary<string, object>()
        );

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.Success(expectedResult));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(expectedResult.Id);
        result.Value.Results.Count.ShouldBe(2);
        result.Value.TotalCount.ShouldBe(2);
        result.Value.QueryTime.ShouldBe(expectedResult.QueryTime);
    }

    [Fact]
    public async Task VectorSearchPort_IndexAsync_Should_Index_Vector_Successfully()
    {
        // Arrange
        var embedding = new VectorEmbedding(
            Id: "emb-1",
            Content: "Test content",
            Embedding: new float[] { 0.1f, 0.2f, 0.3f, 0.4f },
            Metadata: new Dictionary<string, object> { ["source"] = "test" },
            CreatedAt: DateTimeOffset.UtcNow
        );

        _mockVectorSearchPort.IndexAsync(embedding, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockVectorSearchPort.IndexAsync(embedding, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task KnowledgeGraphServicePort_CreateEntityAsync_Should_Create_Entity_Successfully()
    {
        // Arrange
        var entity = new KnowledgeEntity(
            Id: "entity-1",
            Name: "Test Entity",
            Type: "Person",
            Description: "A test entity",
            Properties: new Dictionary<string, object> { ["age"] = 30 }
        );

        _mockKnowledgeGraphServicePort.CreateEntityAsync(entity, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.CreateEntityAsync(entity, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task KnowledgeGraphServicePort_CreateRelationshipAsync_Should_Create_Relationship_Successfully()
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

        _mockKnowledgeGraphServicePort.CreateRelationshipAsync(relationship, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockKnowledgeGraphServicePort.CreateRelationshipAsync(relationship, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task KnowledgeGraphServicePort_GetEntityAsync_Should_Return_Entity_When_Found()
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

    [Fact]
    public async Task KnowledgeGraphServicePort_SearchEntitiesAsync_Should_Return_Matching_Entities()
    {
        // Arrange
        var entityType = "Person";
        var properties = new Dictionary<string, object> { ["age"] = 30 };
        var expectedEntities = new List<KnowledgeEntity>
        {
            new("entity-1", "John Doe", "Person", "A person", new Dictionary<string, object> { ["age"] = 30 }),
            new("entity-2", "Jane Smith", "Person", "Another person", new Dictionary<string, object> { ["age"] = 30 })
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

    [Fact]
    public async Task VectorSearchPort_GetStatisticsAsync_Should_Return_Index_Statistics()
    {
        // Arrange
        var expectedStats = new VectorIndexStatistics(
            TotalVectors: 1000,
            IndexSize: 1024 * 1024, // 1MB
            LastUpdated: DateTimeOffset.UtcNow,
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

    [Fact]
    public async Task KnowledgeGraphServicePort_GetStatisticsAsync_Should_Return_Graph_Statistics()
    {
        // Arrange
        var expectedStats = new KnowledgeGraphStatistics(
            TotalNodes: 1000,
            TotalRelationships: 2000,
            NodeTypes: new Dictionary<string, long> { ["Person"] = 500, ["Organization"] = 300, ["Location"] = 200 },
            RelationshipTypes: new Dictionary<string, long> { ["WORKS_FOR"] = 800, ["MANAGES"] = 200, ["LOCATED_IN"] = 1000 },
            LastUpdated: DateTimeOffset.UtcNow
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

    [Theory]
    [InlineData(0.5f, 0.5f)]
    [InlineData(0.7f, 0.7f)]
    [InlineData(0.9f, 0.9f)]
    public async Task VectorSearchPort_SearchAsync_Should_Respect_Similarity_Threshold(float threshold, float expectedThreshold)
    {
        // Arrange
        var queryVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
        var options = new VectorSearchOptions(
            QueryVector: queryVector,
            MaxResults: 5,
            SimilarityThreshold: threshold
        );
        var expectedResult = new VectorSearchResult(
            Id: "search-threshold",
            QueryId: "query-threshold",
            Results: new List<VectorEmbedding>(),
            TotalCount: 0,
            QueryTime: TimeSpan.FromMilliseconds(10),
            Metadata: new Dictionary<string, object> { ["threshold"] = expectedThreshold }
        );

        _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None)
            .Returns(Result<VectorSearchResult>.Success(expectedResult));

        // Act
        var result = await _mockVectorSearchPort.SearchAsync(queryVector, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Metadata["threshold"].ShouldBe(expectedThreshold);
    }
}