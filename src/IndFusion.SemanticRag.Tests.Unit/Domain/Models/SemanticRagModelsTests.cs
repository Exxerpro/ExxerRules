using System;
using System.Collections.Generic;
using System.Linq;
using IndFusion.SemanticRag.Domain.Models;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Models;

/// <summary>
/// Unit tests for semantic RAG domain models.
/// </summary>
public class SemanticRagModelsTests
{
    public class SemanticDocumentTests
    {
        [Fact]
        public void Should_CreateSemanticDocument_When_ValidParametersProvided()
        {
            // Arrange
            var id = "doc-1";
            var content = "This is a test document";
            var metadata = new Dictionary<string, object>
            {
                ["type"] = "markdown",
                ["source"] = "test-repo",
                ["language"] = "en"
            };
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };
            var createdAt = DateTimeOffset.UtcNow;
            var updatedAt = DateTimeOffset.UtcNow;

            // Act
            var document = new SemanticDocument(id, content, metadata, embedding, createdAt, updatedAt);

            // Assert
            document.Id.ShouldBe(id);
            document.Content.ShouldBe(content);
            document.Metadata.ShouldBe(metadata);
            document.Embedding.ShouldBe(embedding);
            document.CreatedAt.ShouldBe(createdAt);
            document.UpdatedAt.ShouldBe(updatedAt);
        }

        [Fact]
        public void Should_GetDocumentTypeFromMetadata_When_MetadataContainsType()
        {
            // Arrange
            var metadata = new Dictionary<string, object> { ["type"] = "code" };
            var document = new SemanticDocument("doc-1", "content", metadata, null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            var documentType = document.DocumentType;

            // Assert
            documentType.ShouldBe("code");
        }

        [Fact]
        public void Should_GetDefaultDocumentType_When_MetadataDoesNotContainType()
        {
            // Arrange
            var metadata = new Dictionary<string, object>();
            var document = new SemanticDocument("doc-1", "content", metadata, null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            var documentType = document.DocumentType;

            // Assert
            documentType.ShouldBe("unknown");
        }

        [Fact]
        public void Should_ReturnTrueForHasEmbedding_When_EmbeddingIsProvided()
        {
            // Arrange
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };
            var document = new SemanticDocument("doc-1", "content", new Dictionary<string, object>(), embedding, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            var hasEmbedding = document.HasEmbedding;

            // Assert
            hasEmbedding.ShouldBeTrue();
        }

        [Fact]
        public void Should_ReturnFalseForHasEmbedding_When_EmbeddingIsNull()
        {
            // Arrange
            var document = new SemanticDocument("doc-1", "content", new Dictionary<string, object>(), null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            var hasEmbedding = document.HasEmbedding;

            // Assert
            hasEmbedding.ShouldBeFalse();
        }

        [Fact]
        public void Should_ReturnFalseForHasEmbedding_When_EmbeddingIsEmpty()
        {
            // Arrange
            var embedding = new float[0];
            var document = new SemanticDocument("doc-1", "content", new Dictionary<string, object>(), embedding, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

            // Act
            var hasEmbedding = document.HasEmbedding;

            // Assert
            hasEmbedding.ShouldBeFalse();
        }
    }

    public class SemanticSearchQueryTests
    {
        [Fact]
        public void Should_CreateSemanticSearchQuery_When_ValidParametersProvided()
        {
            // Arrange
            var query = "test query";
            var filters = new Dictionary<string, object> { ["type"] = "code" };
            var limit = 20;
            var threshold = 0.8f;
            var includeMetadata = false;

            // Act
            var searchQuery = new SemanticSearchQuery(query, filters, limit, threshold, includeMetadata);

            // Assert
            searchQuery.Query.ShouldBe(query);
            searchQuery.Filters.ShouldBe(filters);
            searchQuery.Limit.ShouldBe(limit);
            searchQuery.Threshold.ShouldBe(threshold);
            searchQuery.IncludeMetadata.ShouldBe(includeMetadata);
        }

        [Fact]
        public void Should_ValidateSuccessfully_When_ValidParametersProvided()
        {
            // Arrange
            var searchQuery = new SemanticSearchQuery("test query", null, 10, 0.7f, true);

            // Act
            var result = searchQuery.Validate();

            // Assert
            result.IsSuccess.ShouldBeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_ValidateFailure_When_QueryIsNullOrEmpty(string query)
        {
            // Arrange
            var searchQuery = new SemanticSearchQuery(query, null, 10, 0.7f, true);

            // Act
            var result = searchQuery.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNull();
            result.Error.ShouldContain("Query cannot be null or empty");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_ValidateFailure_When_LimitIsZeroOrNegative(int limit)
        {
            // Arrange
            var searchQuery = new SemanticSearchQuery("test", null, limit, 0.7f, true);

            // Act
            var result = searchQuery.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNull();
            result.Error.ShouldContain("Limit must be greater than 0");
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(1.1f)]
        public void Should_ValidateFailure_When_ThresholdIsOutOfRange(float threshold)
        {
            // Arrange
            var searchQuery = new SemanticSearchQuery("test", null, 10, threshold, true);

            // Act
            var result = searchQuery.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNull();
            result.Error.ShouldContain("Threshold must be between 0.0 and 1.0");
        }
    }

    public class SemanticSearchResultTests
    {
        [Fact]
        public void Should_CreateSemanticSearchResult_When_ValidParametersProvided()
        {
            // Arrange
            var document = new SemanticDocument("doc-1", "content", new Dictionary<string, object>(), null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
            var score = 0.85f;
            var highlights = new[] { "highlight1", "highlight2" };

            // Act
            var result = new SemanticSearchResult(document, score, highlights);

            // Assert
            result.Document.ShouldBe(document);
            result.Score.ShouldBe(score);
            result.Highlights.ShouldBe(highlights);
        }

        [Theory]
        [InlineData(0.8f, 0.7f, true)]
        [InlineData(0.6f, 0.7f, false)]
        [InlineData(0.7f, 0.7f, true)]
        public void Should_ReturnCorrectThresholdCheck_When_ScoreAndThresholdProvided(float score, float threshold, bool expected)
        {
            // Arrange
            var document = new SemanticDocument("doc-1", "content", new Dictionary<string, object>(), null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
            var result = new SemanticSearchResult(document, score, Array.Empty<string>());

            // Act
            var meetsThreshold = result.MeetsThreshold(threshold);

            // Assert
            meetsThreshold.ShouldBe(expected);
        }
    }

    public class KnowledgeEntityTests
    {
        [Fact]
        public void Should_CreateKnowledgeEntity_When_ValidParametersProvided()
        {
            // Arrange
            var id = "entity-1";
            var type = "Person";
            var name = "John Doe";
            var description = "Software Engineer";
            var properties = new Dictionary<string, object> { ["age"] = 30 };
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };

            // Act
            var entity = new KnowledgeEntity(id, type, name, description, properties, embedding);

            // Assert
            entity.Id.ShouldBe(id);
            entity.Type.ShouldBe(type);
            entity.Name.ShouldBe(name);
            entity.Description.ShouldBe(description);
            entity.Properties.ShouldBe(properties);
            entity.Embedding.ShouldBe(embedding);
        }

        [Fact]
        public void Should_GetDisplayNameWithDescription_When_DescriptionIsProvided()
        {
            // Arrange
            var entity = new KnowledgeEntity("entity-1", "Person", "John Doe", "Software Engineer", new Dictionary<string, object>(), null);

            // Act
            var displayName = entity.DisplayName;

            // Assert
            displayName.ShouldBe("John Doe: Software Engineer");
        }

        [Fact]
        public void Should_GetDisplayNameWithoutDescription_When_DescriptionIsNull()
        {
            // Arrange
            var entity = new KnowledgeEntity("entity-1", "Person", "John Doe", null, new Dictionary<string, object>(), null);

            // Act
            var displayName = entity.DisplayName;

            // Assert
            displayName.ShouldBe("John Doe");
        }

        [Fact]
        public void Should_ReturnTrueForHasEmbedding_When_EmbeddingIsProvided()
        {
            // Arrange
            var embedding = new float[] { 0.1f, 0.2f, 0.3f };
            var entity = new KnowledgeEntity("entity-1", "Person", "John Doe", null, new Dictionary<string, object>(), embedding);

            // Act
            var hasEmbedding = entity.HasEmbedding;

            // Assert
            hasEmbedding.ShouldBeTrue();
        }
    }

    public class KnowledgeRelationshipTests
    {
        [Fact]
        public void Should_CreateKnowledgeRelationship_When_ValidParametersProvided()
        {
            // Arrange
            var id = "rel-1";
            var sourceId = "entity-1";
            var targetId = "entity-2";
            var type = "RELATES_TO";
            var properties = new Dictionary<string, object> { ["strength"] = "strong" };

            // Act
            var relationship = new KnowledgeRelationship(id, sourceId, targetId, type, properties, DateTimeOffset.UtcNow);

            // Assert
            relationship.Id.ShouldBe(id);
            relationship.FromNodeId.ShouldBe(sourceId);
            relationship.ToNodeId.ShouldBe(targetId);
            relationship.RelationshipType.ShouldBe(type);
            relationship.Properties.ShouldBe(properties);
        }

        [Fact]
        public void Should_ValidateSuccessfully_When_ValidParametersProvided()
        {
            // Arrange
            var relationship = new KnowledgeRelationship("rel-1", "entity-1", "entity-2", "RELATES_TO", new Dictionary<string, object>(), DateTimeOffset.UtcNow);

            // Act
            var result = relationship.Validate();

            // Assert
            result.IsSuccess.ShouldBeTrue();
        }

        [Theory]
        [InlineData("", "entity-1", "entity-2", "RELATES_TO")]
        [InlineData("rel-1", "", "entity-2", "RELATES_TO")]
        [InlineData("rel-1", "entity-1", "", "RELATES_TO")]
        [InlineData("rel-1", "entity-1", "entity-2", "")]
        public void Should_ValidateFailure_When_RequiredFieldsAreEmpty(string id, string sourceId, string targetId, string type)
        {
            // Arrange
            var relationship = new KnowledgeRelationship(id, sourceId, targetId, type, new Dictionary<string, object>(), DateTimeOffset.UtcNow);

            // Act
            var result = relationship.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(1.1f)]
        public void Should_ValidateFailure_When_WeightIsOutOfRange(float weight)
        {
            // Arrange
            var relationship = new GraphRelationship("rel-1", "RELATES_TO", "entity-1", "entity-2", new Dictionary<string, object>(), weight);

            // Act
            var result = relationship.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNull();
            result.Error.ShouldContain("Weight must be between 0.0 and 1.0");
        }
    }

    public class SemanticContextTests
    {
        [Fact]
        public void Should_CreateSemanticContext_When_ValidParametersProvided()
        {
            // Arrange
            var documents = new[] { CreateTestDocument("doc-1") };
            var entities = new[] { CreateTestEntity("entity-1") };
            var relationships = new[] { CreateTestRelationship("rel-1") };
            var query = "test query";
            var confidence = 0.85f;

            // Act
            var context = new SemanticContext(documents, entities, relationships, query, confidence);

            // Assert
            context.Documents.ShouldBe(documents);
            context.Entities.ShouldBe(entities);
            context.Relationships.ShouldBe(relationships);
            context.Query.ShouldBe(query);
            context.Confidence.ShouldBe(confidence);
        }

        [Fact]
        public void Should_CalculateTotalItems_When_ContextHasItems()
        {
            // Arrange
            var documents = new[] { CreateTestDocument("doc-1"), CreateTestDocument("doc-2") };
            var entities = new[] { CreateTestEntity("entity-1") };
            var relationships = new[] { CreateTestRelationship("rel-1"), CreateTestRelationship("rel-2") };
            var context = new SemanticContext(documents, entities, relationships, "test", 0.8f);

            // Act
            var totalItems = context.TotalItems;

            // Assert
            totalItems.ShouldBe(5); // 2 documents + 1 entity + 2 relationships
        }

        [Theory]
        [InlineData(3, true)]
        [InlineData(5, true)]
        [InlineData(2, false)]
        public void Should_ReturnCorrectSufficientContext_When_MinimumItemsProvided(int minimumItems, bool expected)
        {
            // Arrange
            var documents = new[] { CreateTestDocument("doc-1") };
            var entities = new[] { CreateTestEntity("entity-1") };
            var relationships = new[] { CreateTestRelationship("rel-1") };
            var context = new SemanticContext(documents, entities, relationships, "test", 0.8f);

            // Act
            var hasSufficientContext = context.HasSufficientContext(minimumItems);

            // Assert
            hasSufficientContext.ShouldBe(expected);
        }

        [Fact]
        public void Should_GenerateSummary_When_ContextHasItems()
        {
            // Arrange
            var documents = new[] { CreateTestDocument("doc-1") };
            var entities = new[] { CreateTestEntity("entity-1") };
            var relationships = new[] { CreateTestRelationship("rel-1") };
            var context = new SemanticContext(documents, entities, relationships, "test", 0.85f);

            // Act
            var summary = context.GetSummary();

            // Assert
            summary.ShouldContain("1 documents");
            summary.ShouldContain("1 entities");
            summary.ShouldContain("1 relationships");
            summary.ShouldContain("85.0%");
        }

        private static SemanticDocument CreateTestDocument(string id) =>
            new(id, "content", new Dictionary<string, object>(), null, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        private static KnowledgeEntity CreateTestEntity(string id) =>
            new(id, "Test", "Test Entity", null, new Dictionary<string, object>(), null);

        private static KnowledgeRelationship CreateTestRelationship(string id) =>
            new(id, "entity-1", "entity-2", "RELATES_TO", new Dictionary<string, object>(), DateTimeOffset.UtcNow);
    }

    public class SemanticRagConfigTests
    {
        [Fact]
        public void Should_CreateSemanticRagConfig_When_ValidParametersProvided()
        {
            // Arrange
            var maxDocuments = 20;
            var maxEntities = 10;
            var similarityThreshold = 0.8f;
            var enableGraphTraversal = true;
            var maxTraversalDepth = 3;
            var enableHybridSearch = false;

            // Act
            var config = new SemanticRagConfig(maxDocuments, maxEntities, similarityThreshold, enableGraphTraversal, maxTraversalDepth, enableHybridSearch);

            // Assert
            config.MaxDocuments.ShouldBe(maxDocuments);
            config.MaxEntities.ShouldBe(maxEntities);
            config.SimilarityThreshold.ShouldBe(similarityThreshold);
            config.EnableGraphTraversal.ShouldBe(enableGraphTraversal);
            config.MaxTraversalDepth.ShouldBe(maxTraversalDepth);
            config.EnableHybridSearch.ShouldBe(enableHybridSearch);
        }

        [Fact]
        public void Should_ValidateSuccessfully_When_ValidParametersProvided()
        {
            // Arrange
            var config = new SemanticRagConfig(10, 5, 0.7f, true, 2, true);

            // Act
            var result = config.Validate();

            // Assert
            result.IsSuccess.ShouldBeTrue();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_ValidateFailure_When_MaxDocumentsIsZeroOrNegative(int maxDocuments)
        {
            // Arrange
            var config = new SemanticRagConfig(maxDocuments, 5, 0.7f, true, 2, true);

            // Act
            var result = config.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNull();
            result.Error.ShouldContain("MaxDocuments must be greater than 0");
        }

        [Fact]
        public void Should_ValidateFailure_When_MaxEntitiesIsNegative()
        {
            // Arrange
            var config = new SemanticRagConfig(10, -1, 0.7f, true, 2, true);

            // Act
            var result = config.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNull();
            result.Error.ShouldContain("MaxEntities cannot be negative");
        }

        [Theory]
        [InlineData(-0.1f)]
        [InlineData(1.1f)]
        public void Should_ValidateFailure_When_SimilarityThresholdIsOutOfRange(float threshold)
        {
            // Arrange
            var config = new SemanticRagConfig(10, 5, threshold, true, 2, true);

            // Act
            var result = config.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNull();
            result.Error.ShouldContain("SimilarityThreshold must be between 0.0 and 1.0");
        }

        [Fact]
        public void Should_ValidateFailure_When_MaxTraversalDepthIsNegative()
        {
            // Arrange
            var config = new SemanticRagConfig(10, 5, 0.7f, true, -1, true);

            // Act
            var result = config.Validate();

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNull();
            result.Error.ShouldContain("MaxTraversalDepth cannot be negative");
        }
    }
}