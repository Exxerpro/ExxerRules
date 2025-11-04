using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Tests.Shared;

namespace IndFusion.SemanticRag.Domain.Tests.Domain.Models;

/// <summary>
/// Unit tests for semantic RAG domain models.
/// </summary>
public class SemanticRagModelsTests
{
    public class SemanticDocumentTests
    {
        [Fact(Timeout = 5000)]
        public void Should_CreateSemanticDocument_When_ValidParametersProvided()
        {
            // Arrange
            var id = "doc-1";
            var title = "Test Document";
            var content = "This is a test document";

            // ✅ Use fluent builder from TestDataBuilders
            var documentResult = TestDataBuilders.CreateValidSemanticDocument(
                id: id,
                title: title,
                content: content);
            documentResult.IsSuccess.ShouldBeTrue();
            var document = documentResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

            // Assert
            document.Id.ShouldBe(id);
            document.Title.ShouldBe(title);
            document.Content.ShouldBe(content);
            // Note: TestDataBuilders.CreateValidSemanticDocument creates metadata with ["source"] = "test"
            document.Metadata.ShouldNotBeNull();
            document.Metadata.ShouldContainKey("source");
            document.Metadata["source"].ShouldBe("test");
            // Note: CreatedAt/UpdatedAt are set by the builder, so we verify they are set
            document.CreatedAt.ShouldBeGreaterThan(DateTime.MinValue);
            document.UpdatedAt.ShouldBeGreaterThan(DateTime.MinValue);
        }
    }

    public class KnowledgeEntityTests
    {
        [Fact(Timeout = 5000)]
        public void Should_CreateKnowledgeEntity_When_ValidParametersProvided()
        {
            // Arrange
            var id = "entity-1";
            var name = "John Doe";
            var type = "Person";
            var description = "Software Engineer";
            var properties = new Dictionary<string, object> { ["age"] = 30 };

            // ✅ Use fluent builder from TestDataBuilders with custom properties
            var entityResult = TestDataBuilders
                .CreateValidKnowledgeEntity(id: id, name: name, type: type)
                .Map(e => new KnowledgeEntity(
                    e.Id,
                    e.Name,
                    e.Type,
                    description,
                    properties,
                    0.9,
                    DateTime.UtcNow));
            entityResult.IsSuccess.ShouldBeTrue();
            var entity = entityResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

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
        [Fact(Timeout = 5000)]
        public void Should_CreateKnowledgeRelationship_When_ValidParametersProvided()
        {
            // Arrange
            var id = "rel-1";
            var fromNodeId = "entity-1";
            var toNodeId = "entity-2";
            var relationshipType = "RELATES_TO";
            var properties = new Dictionary<string, object> { ["strength"] = "strong" };
            var createdAt = DateTimeOffset.UtcNow;

            // ✅ Use fluent builder from TestDataBuilders
            var relationshipResult = TestDataBuilders
                .CreateValidKnowledgeRelationship(id: id, fromNodeId: fromNodeId, toNodeId: toNodeId)
                .Map(r => new KnowledgeRelationship(
                    r.Id,
                    r.FromNodeId,
                    r.ToNodeId,
                    relationshipType,
                    properties,
                    createdAt));
            relationshipResult.IsSuccess.ShouldBeTrue();
            var relationship = relationshipResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

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
        [Fact(Timeout = 5000)]
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
            // ✅ Use fluent builder from TestDataBuilders
            var documentResult = TestDataBuilders.CreateValidSemanticDocument(
                id: "doc-1",
                title: "Test Doc",
                content: "Content");
            documentResult.IsSuccess.ShouldBeTrue();
            var document = documentResult.Value;
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
        [Fact(Timeout = 5000)]
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
                new("entity-1", "Entity 1", "Person", "Description", new Dictionary<string, object>(), 0.9, DateTime.UtcNow)
            };
            var relationships = new List<EntityRelationship>
            {
                new("rel-1", "entity-1", "entity-2", "RELATES_TO", 0.9, new Dictionary<string, object>())
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
        [Fact(Timeout = 5000)]
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