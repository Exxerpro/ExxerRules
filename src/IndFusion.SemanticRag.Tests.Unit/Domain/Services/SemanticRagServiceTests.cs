using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Services;

/// <summary>
/// Unit tests for the semantic RAG service.
/// </summary>
public class SemanticRagServiceTests
{
    private readonly ISemanticRagService _semanticRagService;
    private readonly ILogger<ISemanticRagService> _logger;

    public SemanticRagServiceTests()
    {
        _semanticRagService = Substitute.For<ISemanticRagService>();
        _logger = Substitute.For<ILogger<ISemanticRagService>>();
    }

    public class SearchAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSearchResults_When_ValidQueryProvided()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var query = new SemanticSearchQuery("test query");
            var config = new SemanticRagConfig();
            var expectedResults = new[]
            {
                CreateTestSearchResult("doc-1", 0.9f),
                CreateTestSearchResult("doc-2", 0.8f)
            };

            semanticRagService.SearchAsync(query, config, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticSearchResult>>.Success(expectedResults));

            // Act
            var result = await semanticRagService.SearchAsync(query, config, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBe(expectedResults);
            result.Value.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ServiceFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var query = new SemanticSearchQuery("test query");
            var config = new SemanticRagConfig();
            var errorMessage = "Search failed";

            semanticRagService.SearchAsync(query, config, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticSearchResult>>.WithFailure(errorMessage));

            // Act
            var result = await semanticRagService.SearchAsync(query, config, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticSearchResult CreateTestSearchResult(string documentId, float score)
        {
            var document = new SemanticDocument(
                documentId,
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);

            return new SemanticSearchResult(document, score, Array.Empty<string>());
        }
    }

    public class GetContextAsyncTests
    {
        [Fact]
        public async Task Should_ReturnContext_When_ValidQueryProvided()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var query = "test query";
            var config = new SemanticRagConfig();
            var expectedContext = CreateTestContext();

            semanticRagService.GetContextAsync(query, config, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticContext>.Success(expectedContext));

            // Act
            var result = await semanticRagService.GetContextAsync(query, config, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedContext);
            result.Value.Query.ShouldBe(query);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ServiceFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var query = "test query";
            var config = new SemanticRagConfig();
            var errorMessage = "Context retrieval failed";

            semanticRagService.GetContextAsync(query, config, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticContext>.WithFailure(errorMessage));

            // Act
            var result = await semanticRagService.GetContextAsync(query, config, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticContext CreateTestContext()
        {
            var documents = new[]
            {
                new SemanticDocument("doc-1", "content1", new Dictionary<string, object>(), null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow)
            };
            var entities = new[]
            {
                new KnowledgeEntity("entity-1", "Person", "John Doe", null, new Dictionary<string, object>(), null)
            };
            var relationships = new[]
            {
                new KnowledgeRelationship("rel-1", "entity-1", "entity-2", "RELATES_TO", new Dictionary<string, object>(), DateTimeOffset.UtcNow)
            };

            return new SemanticContext(documents, entities, relationships, "test query", 0.8f);
        }
    }

    public class IndexDocumentAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSuccess_When_DocumentIndexedSuccessfully()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var document = CreateTestDocument();

            semanticRagService.IndexDocumentAsync(document, Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            // Act
            var result = await semanticRagService.IndexDocumentAsync(document, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_ReturnFailure_When_IndexingFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var document = CreateTestDocument();
            var errorMessage = "Indexing failed";

            semanticRagService.IndexDocumentAsync(document, Arg.Any<CancellationToken>())
                .Returns(Result.WithFailure(errorMessage));

            // Act
            var result = await semanticRagService.IndexDocumentAsync(document, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticDocument CreateTestDocument()
        {
            return new SemanticDocument(
                "doc-1",
                "test content",
                new Dictionary<string, object> { ["type"] = "test" },
                new float[] { 0.1f, 0.2f, 0.3f },
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);
        }
    }

    public class AddEntityAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSuccess_When_EntityAddedSuccessfully()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var entity = CreateTestEntity();

            semanticRagService.AddEntityAsync(entity, Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            // Act
            var result = await semanticRagService.AddEntityAsync(entity, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_ReturnFailure_When_AddingEntityFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var entity = CreateTestEntity();
            var errorMessage = "Entity addition failed";

            semanticRagService.AddEntityAsync(entity, Arg.Any<CancellationToken>())
                .Returns(Result.WithFailure(errorMessage));

            // Act
            var result = await semanticRagService.AddEntityAsync(entity, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static KnowledgeEntity CreateTestEntity()
        {
            return new KnowledgeEntity(
                "entity-1",
                "Person",
                "John Doe",
                "Software Engineer",
                new Dictionary<string, object> { ["age"] = 30 },
                new float[] { 0.1f, 0.2f, 0.3f });
        }
    }

    public class CreateRelationshipAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSuccess_When_RelationshipCreatedSuccessfully()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var relationship = CreateTestRelationship();

            semanticRagService.CreateRelationshipAsync(relationship, Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            // Act
            var result = await semanticRagService.CreateRelationshipAsync(relationship, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CreatingRelationshipFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var relationship = CreateTestRelationship();
            var errorMessage = "Relationship creation failed";

            semanticRagService.CreateRelationshipAsync(relationship, Arg.Any<CancellationToken>())
                .Returns(Result.WithFailure(errorMessage));

            // Act
            var result = await semanticRagService.CreateRelationshipAsync(relationship, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static KnowledgeRelationship CreateTestRelationship()
        {
            return new KnowledgeRelationship(
                "rel-1",
                "entity-1",
                "entity-2",
                "RELATES_TO",
                new Dictionary<string, object> { ["strength"] = "strong" },
                DateTimeOffset.UtcNow);
        }
    }

    public class FindSimilarEntitiesAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSimilarEntities_When_ValidEntityProvided()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var entity = CreateTestEntity();
            var limit = 5;
            var threshold = 0.7f;
            var expectedEntities = new[]
            {
                CreateTestEntity("entity-2"),
                CreateTestEntity("entity-3")
            };

            semanticRagService.FindSimilarEntitiesAsync(entity, limit, threshold, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

            // Act
            var result = await semanticRagService.FindSimilarEntitiesAsync(entity, limit, threshold, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value.ShouldBe(expectedEntities);
            result.Value.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_FindingSimilarEntitiesFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var entity = CreateTestEntity();
            var errorMessage = "Similarity search failed";

            semanticRagService.FindSimilarEntitiesAsync(entity, 5, 0.7f, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeEntity>>.WithFailure(errorMessage));

            // Act
            var result = await semanticRagService.FindSimilarEntitiesAsync(entity, 5, 0.7f, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static KnowledgeEntity CreateTestEntity(string id = "entity-1")
        {
            return new KnowledgeEntity(
                id,
                "Person",
                "John Doe",
                "Software Engineer",
                new Dictionary<string, object>(),
                new float[] { 0.1f, 0.2f, 0.3f });
        }
    }

    public class GetStatsAsyncTests
    {
        [Fact]
        public async Task Should_ReturnStats_When_ServiceSucceeds()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var expectedStats = new SemanticRagStats(
                TotalDocuments: 100,
                TotalEntities: 50,
                TotalRelationships: 25,
                LastIndexedAt: DateTimeOffset.UtcNow,
                AverageDocumentSize: 1024.5,
                EmbeddingDimension: 768);

            semanticRagService.GetStatsAsync(Arg.Any<CancellationToken>())
                .Returns(Result<SemanticRagStats>.Success(expectedStats));

            // Act
            var result = await semanticRagService.GetStatsAsync(cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedStats);
            result.Value.TotalDocuments.ShouldBe(100);
            result.Value.TotalEntities.ShouldBe(50);
            result.Value.TotalRelationships.ShouldBe(25);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_GettingStatsFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var errorMessage = "Stats retrieval failed";

            semanticRagService.GetStatsAsync(Arg.Any<CancellationToken>())
                .Returns(Result<SemanticRagStats>.WithFailure(errorMessage));

            // Act
            var result = await semanticRagService.GetStatsAsync(cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }
    }

    public class ClearAllAsyncTests
    {
        [Fact]
        public async Task Should_ReturnSuccess_When_ClearingAllSucceeds()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();

            semanticRagService.ClearAllAsync(Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            // Act
            var result = await semanticRagService.ClearAllAsync(cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ClearingAllFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var errorMessage = "Clear all failed";

            semanticRagService.ClearAllAsync(Arg.Any<CancellationToken>())
                .Returns(Result.WithFailure(errorMessage));

            // Act
            var result = await semanticRagService.ClearAllAsync(cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }
    }
}