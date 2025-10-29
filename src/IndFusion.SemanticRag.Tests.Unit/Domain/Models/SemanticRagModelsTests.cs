using System;
using System.Collections.Generic;
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
            var title = "Test Document";
            var content = "This is a test document";
            var metadata = new Dictionary<string, object>
            {
                ["type"] = "markdown",
                ["source"] = "test-repo",
                ["language"] = "en"
            };
            var createdAt = DateTime.UtcNow;
            var updatedAt = DateTime.UtcNow;

            // Act
            var document = new SemanticDocument(id, title, content, metadata, createdAt, updatedAt);

            // Assert
            document.Id.ShouldBe(id);
            document.Title.ShouldBe(title);
            document.Content.ShouldBe(content);
            document.Metadata.ShouldBe(metadata);
            document.CreatedAt.ShouldBe(createdAt);
            document.UpdatedAt.ShouldBe(updatedAt);
        }
    }

    public class KnowledgeEntityTests
    {
        [Fact]
        public void Should_CreateKnowledgeEntity_When_ValidParametersProvided()
        {
            // Arrange
            var id = "entity-1";
            var name = "John Doe";
            var type = "Person";
            var description = "Software Engineer";
            var properties = new Dictionary<string, object> { ["age"] = 30 };

            // Act
            var entity = new KnowledgeEntity(id, name, type, description, properties);

            // Assert
            entity.Id.ShouldBe(id);
            entity.Name.ShouldBe(name);
            entity.Type.ShouldBe(type);
            entity.Description.ShouldBe(description);
            entity.Properties.ShouldBe(properties);
        }
    }

    public class KnowledgeRelationshipTests
    {
        [Fact]
        public void Should_CreateKnowledgeRelationship_When_ValidParametersProvided()
        {
            // Arrange
            var id = "rel-1";
            var fromNodeId = "entity-1";
            var toNodeId = "entity-2";
            var relationshipType = "RELATES_TO";
            var properties = new Dictionary<string, object> { ["strength"] = "strong" };
            var createdAt = DateTimeOffset.UtcNow;

            // Act
            var relationship = new KnowledgeRelationship(id, fromNodeId, toNodeId, relationshipType, properties, createdAt);

            // Assert
            relationship.Id.ShouldBe(id);
            relationship.FromNodeId.ShouldBe(fromNodeId);
            relationship.ToNodeId.ShouldBe(toNodeId);
            relationship.RelationshipType.ShouldBe(relationshipType);
            relationship.Properties.ShouldBe(properties);
            relationship.CreatedAt.ShouldBe(createdAt);
        }
    }

    public class SemanticSearchResultTests
    {
        [Fact]
        public void Should_CreateSemanticSearchResult_When_ValidParametersProvided()
        {
            // Arrange
            var id = "search-1";
            var queryId = "query-1";
            var results = new List<SearchResultItem>
            {
                new("item-1", "Content 1", 0.9, new Dictionary<string, object>(), "source-1"),
                new("item-2", "Content 2", 0.8, new Dictionary<string, object>(), "source-2")
            };
            var document = new SemanticDocument("doc-1", "Test Doc", "Content", new Dictionary<string, object>(), DateTime.UtcNow, DateTime.UtcNow);
            var totalCount = 2L;
            var queryTime = TimeSpan.FromMilliseconds(100);
            var metadata = new Dictionary<string, object> { ["search_type"] = "semantic" };

            // Act
            var result = new SemanticSearchResult(id, queryId, results, document, totalCount, queryTime, metadata);

            // Assert
            result.Id.ShouldBe(id);
            result.QueryId.ShouldBe(queryId);
            result.Results.ShouldBe(results);
            result.Document.ShouldBe(document);
            result.TotalCount.ShouldBe(totalCount);
            result.QueryTime.ShouldBe(queryTime);
            result.Metadata.ShouldBe(metadata);
        }
    }

    public class SemanticContextTests
    {
        [Fact]
        public void Should_CreateSemanticContext_When_ValidParametersProvided()
        {
            // Arrange
            var id = "context-1";
            var name = "Test Context";
            var description = "A test context for semantic operations";
            var documents = new List<SemanticDocument>
            {
                new("doc-1", "Title 1", "Content 1", new Dictionary<string, object>(), DateTime.UtcNow, DateTime.UtcNow)
            };
            var entities = new List<KnowledgeEntity>
            {
                new("entity-1", "Entity 1", "Person", "Description", new Dictionary<string, object>())
            };
            var relationships = new List<EntityRelationship>
            {
                new("rel-1", "entity-1", "entity-2", "RELATES_TO", new Dictionary<string, object>())
            };
            var properties = new Dictionary<string, object> { ["context_type"] = "test" };
            var createdAt = DateTime.UtcNow;

            // Act
            var context = new SemanticContext(id, name, description, documents, entities, relationships, properties, createdAt);

            // Assert
            context.Id.ShouldBe(id);
            context.Name.ShouldBe(name);
            context.Description.ShouldBe(description);
            context.Documents.ShouldBe(documents);
            context.Entities.ShouldBe(entities);
            context.Relationships.ShouldBe(relationships);
            context.Properties.ShouldBe(properties);
            context.CreatedAt.ShouldBe(createdAt);
        }
    }

    public class SemanticRagConfigTests
    {
        [Fact]
        public void Should_CreateSemanticRagConfig_When_ValidParametersProvided()
        {
            // Arrange
            var id = "config-1";
            var name = "Test Config";
            var embeddingModel = "text-embedding-ada-002";
            var vectorDimensions = 1536;
            var similarityThreshold = 0.7;
            var maxResults = 10;
            var properties = new Dictionary<string, object> { ["model_version"] = "1.0" };

            // Act
            var config = new SemanticRagConfig(id, name, embeddingModel, vectorDimensions, similarityThreshold, maxResults, properties);

            // Assert
            config.Id.ShouldBe(id);
            config.Name.ShouldBe(name);
            config.EmbeddingModel.ShouldBe(embeddingModel);
            config.VectorDimensions.ShouldBe(vectorDimensions);
            config.SimilarityThreshold.ShouldBe(similarityThreshold);
            config.MaxResults.ShouldBe(maxResults);
            config.Properties.ShouldBe(properties);
        }
    }
}