using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using IndQuestResults;
using KnowledgeExtractionResultModel = IndFusion.SemanticRag.Domain.Models.KnowledgeExtractionResult;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Application.Services;

/// <summary>
/// Unit tests for the semantic RAG orchestration service.
/// </summary>
public class SemanticRagOrchestrationServiceTests
{
    private readonly ISemanticRagService _semanticRagService;
    private readonly ISemanticSearchService _searchService;
    private readonly IDocumentIngestionService _ingestionService;
    private readonly IKnowledgeExtractionService _extractionService;
    private readonly ILogger<SemanticRagOrchestrationService> _logger;
    private readonly SemanticRagOrchestrationService _orchestrationService;

    public SemanticRagOrchestrationServiceTests()
    {
        _semanticRagService = Substitute.For<ISemanticRagService>();
        _searchService = Substitute.For<ISemanticSearchService>();
        _ingestionService = Substitute.For<IDocumentIngestionService>();
        _extractionService = Substitute.For<IKnowledgeExtractionService>();
        _logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

        _orchestrationService = new SemanticRagOrchestrationService(
            _semanticRagService,
            _searchService,
            _ingestionService,
            _extractionService,
            _logger);
    }

    public class ComprehensiveSearchAsyncTests
    {
        [Fact(Timeout = 5000)]
        public async Task Should_ReturnComprehensiveSearchResult_When_ValidQueryProvided()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var searchService = Substitute.For<ISemanticSearchService>();
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

            var orchestrationService = new SemanticRagOrchestrationService(
                semanticRagService,
                searchService,
                ingestionService,
                extractionService,
                logger);

            var query = "test query";
            var options = ComprehensiveSearchOptions.Default();
            var searchResponse = CreateTestSearchResponse();
            var context = CreateTestContext();

            searchService.SearchAsync(query, options.SearchOptions, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticSearchResponse>.Success(searchResponse));

            semanticRagService.GetContextAsync(query, options.RagConfig, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticContext>.Success(context));

            // Mock extraction service since EnableKnowledgeExtraction is true by default
            // Create a simple extraction result inline since CreateTestKnowledgeExtractionResult is in IngestRepositoryAsyncTests nested class
            var entities = new[] { new KnowledgeEntity("entity-1", "Test Entity", "TestType", "Description", new Dictionary<string, object>(), 0.9f, DateTime.UtcNow) };
            var relationships = new[] { new KnowledgeRelationship("rel-1", "entity-1", "entity-2", "RELATED_TO", new Dictionary<string, object>(), DateTimeOffset.UtcNow) };
            var codeEntities = new List<IndFusion.SemanticRag.Domain.Services.CodeEntity>();
            var concepts = new List<IndFusion.SemanticRag.Domain.Services.SemanticConcept>();
            var extractionResult = new IndFusion.SemanticRag.Domain.Services.KnowledgeExtractionResult(
                entities,
                relationships,
                codeEntities,
                concepts,
                ProcessingTimeMs: 1000,
                Confidence: 0.8f);
            
            extractionService.ExtractKnowledgeAsync(Arg.Any<SemanticDocument>(), Arg.Any<ComprehensiveExtractionOptions>(), Arg.Any<CancellationToken>())
                .Returns(Result<IndFusion.SemanticRag.Domain.Services.KnowledgeExtractionResult>.Success(extractionResult));

            // Act
            var result = await orchestrationService.ComprehensiveSearchAsync(query, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue($"Expected success but got error: {result.Error}");
            result.Value.Query.ShouldBe(query);
            
            // Compare SearchResults by content, not reference
            result.Value.SearchResults.ShouldNotBeNull();
            result.Value.SearchResults.Count.ShouldBe(searchResponse.Results.Count);
            for (int i = 0; i < result.Value.SearchResults.Count; i++)
            {
                result.Value.SearchResults[i].Id.ShouldBe(searchResponse.Results[i].Id);
                result.Value.SearchResults[i].Document?.Id.ShouldBe(searchResponse.Results[i].Document?.Id);
            }
            
            // Compare AdditionalContext by content, not reference
            // Note: SemanticContext does not have a Query property - it has Id, Name, Description, etc.
            if (context != null)
            {
                result.Value.AdditionalContext.ShouldNotBeNull();
                result.Value.AdditionalContext!.Id.ShouldBe(context.Id);
                result.Value.AdditionalContext!.Name.ShouldBe(context.Name);
            }
            else
            {
                // If context is null, AdditionalContext should also be null (since EnableContextRetrieval is true by default)
                // But the mock returns a context, so we verify it's not null
                result.Value.AdditionalContext.ShouldNotBeNull();
            }
        }

        [Fact(Timeout = 5000)]
        public async Task Should_ReturnFailure_When_SearchFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var searchService = Substitute.For<ISemanticSearchService>();
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

            var orchestrationService = new SemanticRagOrchestrationService(
                semanticRagService,
                searchService,
                ingestionService,
                extractionService,
                logger);

            var query = "test query";
            var options = ComprehensiveSearchOptions.Default();
            var errorMessage = "Search failed";

            searchService.SearchAsync(query, options.SearchOptions, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticSearchResponse>.WithFailure(errorMessage));

            // Act
            var result = await orchestrationService.ComprehensiveSearchAsync(query, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNullOrEmpty();
        }

        private static SemanticSearchResponse CreateTestSearchResponse()
        {
            var results = new[]
            {
                CreateTestSearchResult("doc-1", 0.9f),
                CreateTestSearchResult("doc-2", 0.8f)
            };

            return new SemanticSearchResponse(
                results,
                TotalCount: 2,
                Query: "test query",
                ProcessingTimeMs: 150,
                Context: null,
                Suggestions: new[] { "test suggestion" });
        }

        private static SemanticSearchResult CreateTestSearchResult(string documentId, float score)
        {
            // ✅ Use fluent builder from TestDataBuilders
            var documentResult = TestDataBuilders.CreateValidSemanticDocument(
                id: documentId,
                title: "Test Document",
                content: "test content");
            documentResult.IsSuccess.ShouldBeTrue();
            var document = documentResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

            var results = new List<SearchResultItem>
            {
                new SearchResultItem("result-1", "test content", score, new Dictionary<string, object>(), document.Id)
            };
            return new SemanticSearchResult("search-1", "query-1", results, document, 1L, TimeSpan.FromMilliseconds(100), new Dictionary<string, object>());
        }

        private static SemanticContext CreateTestContext()
        {
            // ✅ Use fluent builders from TestDataBuilders
            var documentResult = TestDataBuilders.CreateValidSemanticDocument(
                id: "doc-1",
                title: "Document 1",
                content: "content1");
            documentResult.IsSuccess.ShouldBeTrue();
            var document = documentResult.Value;

            var entityResult = TestDataBuilders.CreateValidKnowledgeEntity(
                id: "entity-1",
                name: "John Doe",
                type: "Person");
            entityResult.IsSuccess.ShouldBeTrue();
            var entity = entityResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

            var relationshipResult = TestDataBuilders.CreateValidKnowledgeRelationship(
                id: "rel-1",
                fromNodeId: "entity-1",
                toNodeId: "entity-2");
            relationshipResult.IsSuccess.ShouldBeTrue();
            var relationship = relationshipResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

            var documents = new[] { document! }; // Null-forgiving: IsSuccess guarantees non-null
            var entities = new[] { entity! }; // Null-forgiving: IsSuccess guarantees non-null
            var relationships = new[] { relationship! }; // Null-forgiving: IsSuccess guarantees non-null

            var relationshipsList = relationships.Select(r => new EntityRelationship(r.Id, r.FromNodeId, r.ToNodeId, r.RelationshipType, 0.9, r.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))).ToList();
            return new SemanticContext("context-1", "Test Context", "A test context", documents, entities, relationshipsList, new Dictionary<string, object>(), DateTime.UtcNow);
        }
    }

    public class IngestRepositoryAsyncTests
    {
        [Fact(Timeout = 5000)]
        public async Task Should_ReturnIngestionResult_When_ValidRepositoryPathProvided()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var searchService = Substitute.For<ISemanticSearchService>();
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

            var orchestrationService = new SemanticRagOrchestrationService(
                semanticRagService,
                searchService,
                ingestionService,
                extractionService,
                logger);

            var repositoryPath = "/path/to/repo";
            var config = IndFusion.SemanticRag.Domain.Models.RepositoryIngestionConfig.Default();
            var documents = new[] { CreateTestDocument("doc-1"), CreateTestDocument("doc-2") };
            var extractionResult = CreateTestKnowledgeExtractionResult();

            var servicesConfig = new IndFusion.SemanticRag.Domain.Services.RepositoryIngestionConfig(
                IncludePatterns: config.IncludePatterns ?? new List<string>(),
                ExcludePatterns: config.ExcludePatterns ?? new List<string>(),
                MaxFileSize: config.MaxFileSize,
                ExtractCodeEntities: true,
                ExtractComments: true
            );
            ingestionService.IngestRepositoryAsync(repositoryPath, servicesConfig, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticDocument>>.Success(documents));

            extractionService.ExtractKnowledgeAsync(Arg.Any<SemanticDocument>(), Arg.Any<ComprehensiveExtractionOptions>(), Arg.Any<CancellationToken>())
                .Returns(Result<IndFusion.SemanticRag.Domain.Services.KnowledgeExtractionResult>.Success(extractionResult));

            semanticRagService.IndexDocumentAsync(Arg.Any<SemanticDocument>(), Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            semanticRagService.AddEntityAsync(Arg.Any<KnowledgeEntity>(), Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            semanticRagService.CreateRelationshipAsync(Arg.Any<KnowledgeRelationship>(), Arg.Any<CancellationToken>())
                .Returns(Result.Success());

            // Act
            var result = await orchestrationService.IngestRepositoryAsync(repositoryPath, config, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.TotalDocuments.ShouldBe(2);
            result.Value.Success.ShouldBeTrue();
        }

        [Fact(Timeout = 5000)]
        public async Task Should_ReturnFailure_When_DocumentIngestionFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var searchService = Substitute.For<ISemanticSearchService>();
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

            var orchestrationService = new SemanticRagOrchestrationService(
                semanticRagService,
                searchService,
                ingestionService,
                extractionService,
                logger);

            var repositoryPath = "/path/to/repo";
            var config = IndFusion.SemanticRag.Domain.Models.RepositoryIngestionConfig.Default();
            var errorMessage = "Document ingestion failed";

            var servicesConfig = new IndFusion.SemanticRag.Domain.Services.RepositoryIngestionConfig(
                IncludePatterns: config.IncludePatterns ?? new List<string>(),
                ExcludePatterns: config.ExcludePatterns ?? new List<string>(),
                MaxFileSize: config.MaxFileSize,
                ExtractCodeEntities: true,
                ExtractComments: true
            );
            ingestionService.IngestRepositoryAsync(repositoryPath, servicesConfig, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticDocument>>.WithFailure(errorMessage));

            // Act
            var result = await orchestrationService.IngestRepositoryAsync(repositoryPath, config, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNullOrEmpty();
        }

        private static SemanticDocument CreateTestDocument(string id = "doc-1")
        {
            // ✅ Use fluent builder from TestDataBuilders
            var documentResult = TestDataBuilders.CreateValidSemanticDocument(
                id: id,
                title: "Test Document",
                content: "test content");
            documentResult.IsSuccess.ShouldBeTrue();
            return documentResult.Value!; // Null-forgiving: IsSuccess guarantees non-null
        }

        private static IndFusion.SemanticRag.Domain.Services.KnowledgeExtractionResult CreateTestKnowledgeExtractionResult()
        {
            var entities = new[] { CreateTestEntity() };
            var relationships = new[] { CreateTestRelationship() };
            var codeEntities = new[] { CreateTestCodeEntity() };
            var concepts = new[] { CreateTestConcept() };

            return new IndFusion.SemanticRag.Domain.Services.KnowledgeExtractionResult(
                entities,
                relationships,
                codeEntities,
                concepts,
                ProcessingTimeMs: 1000,
                Confidence: 0.8f);
        }

        private static KnowledgeEntity CreateTestEntity()
        {
            // ✅ Use fluent builder from TestDataBuilders
            var entityResult = TestDataBuilders.CreateValidKnowledgeEntity(
                id: "entity-1",
                name: "John Doe",
                type: "Person");
            entityResult.IsSuccess.ShouldBeTrue();
            return entityResult.Value!; // Null-forgiving: IsSuccess guarantees non-null
        }

        private static KnowledgeRelationship CreateTestRelationship()
        {
            // ✅ Use fluent builder from TestDataBuilders
            var relationshipResult = TestDataBuilders.CreateValidKnowledgeRelationship(
                id: "rel-1",
                fromNodeId: "entity-1",
                toNodeId: "entity-2");
            relationshipResult.IsSuccess.ShouldBeTrue();
            return relationshipResult.Value!; // Null-forgiving: IsSuccess guarantees non-null
        }

        private static CodeEntity CreateTestCodeEntity()
        {
            return new CodeEntity(
                "class-1",
                "CLASS",
                "TestClass",
                "TestNamespace.TestClass",
                "TestNamespace",
                "public",
                null,
                null,
                new Dictionary<string, object>(),
                null);
        }

        private static SemanticConcept CreateTestConcept()
        {
            return new SemanticConcept(
                "concept-1",
                "Test Concept",
                "A test concept",
                new[] { "synonym1" },
                Frequency: 1,
                Context: "test context",
                null);
        }
    }

    public class AnswerQuestionAsyncTests
    {
        [Fact(Timeout = 5000)]
        public async Task Should_ReturnAnswerResult_When_ValidQuestionProvided()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var searchService = Substitute.For<ISemanticSearchService>();
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

            var orchestrationService = new SemanticRagOrchestrationService(
                semanticRagService,
                searchService,
                ingestionService,
                extractionService,
                logger);

            var question = "What is the purpose of this code?";
            var options = new QuestionAnswerOptions(
                new SemanticSearchOptions(),
                new SemanticRagConfig(
                    Id: "test-config",
                    Name: "Test Config",
                    EmbeddingModel: "test-model",
                    VectorDimensions: 1536,
                    SimilarityThreshold: 0.7,
                    MaxResults: 10,
                    Properties: new Dictionary<string, object>()));
            var context = CreateTestContext();
            var searchResponse = CreateTestSearchResponse();

            semanticRagService.GetContextAsync(question, options.RagConfig, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticContext>.Success(context));

            searchService.SearchAsync(question, options.SearchOptions, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticSearchResponse>.Success(searchResponse));

            // Act
            var result = await orchestrationService.AnswerQuestionAsync(question, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.Question.ShouldBe(question);
            result.Value.Answer.ShouldNotBeNullOrEmpty();
            result.Value.SupportingDocuments.Count.ShouldBeGreaterThan(0);
        }

        [Fact(Timeout = 5000)]
        public async Task Should_ReturnFailure_When_ContextRetrievalFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var searchService = Substitute.For<ISemanticSearchService>();
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

            var orchestrationService = new SemanticRagOrchestrationService(
                semanticRagService,
                searchService,
                ingestionService,
                extractionService,
                logger);

            var question = "What is the purpose of this code?";
            var options = new QuestionAnswerOptions(
                new SemanticSearchOptions(),
                new SemanticRagConfig(
                    Id: "test-config",
                    Name: "Test Config",
                    EmbeddingModel: "test-model",
                    VectorDimensions: 1536,
                    SimilarityThreshold: 0.7,
                    MaxResults: 10,
                    Properties: new Dictionary<string, object>()));
            var errorMessage = "Context retrieval failed";

            semanticRagService.GetContextAsync(question, options.RagConfig, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticContext>.WithFailure(errorMessage));

            // Act
            var result = await orchestrationService.AnswerQuestionAsync(question, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNullOrEmpty();
        }

        private static SemanticContext CreateTestContext()
        {
            // ✅ Use fluent builders from TestDataBuilders
            var documentResult = TestDataBuilders.CreateValidSemanticDocument(
                id: "doc-1",
                title: "Document 1",
                content: "content1");
            documentResult.IsSuccess.ShouldBeTrue();
            var document = documentResult.Value;

            var entityResult = TestDataBuilders.CreateValidKnowledgeEntity(
                id: "entity-1",
                name: "John Doe",
                type: "Person");
            entityResult.IsSuccess.ShouldBeTrue();
            var entity = entityResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

            var relationshipResult = TestDataBuilders.CreateValidKnowledgeRelationship(
                id: "rel-1",
                fromNodeId: "entity-1",
                toNodeId: "entity-2");
            relationshipResult.IsSuccess.ShouldBeTrue();
            var relationship = relationshipResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

            var documents = new[] { document! }; // Null-forgiving: IsSuccess guarantees non-null
            var entities = new[] { entity! }; // Null-forgiving: IsSuccess guarantees non-null
            var relationships = new[] { relationship! }; // Null-forgiving: IsSuccess guarantees non-null

            var relationshipsList = relationships.Select(r => new EntityRelationship(r.Id, r.FromNodeId, r.ToNodeId, r.RelationshipType, 0.9, r.Properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value))).ToList();
            return new SemanticContext("context-1", "Test Context", "A test context", documents, entities, relationshipsList, new Dictionary<string, object>(), DateTime.UtcNow);
        }

        private static SemanticSearchResponse CreateTestSearchResponse()
        {
            var results = new[]
            {
                CreateTestSearchResult("doc-1", 0.9f)
            };

            return new SemanticSearchResponse(
                results,
                TotalCount: 1,
                Query: "test query",
                ProcessingTimeMs: 100,
                Context: null);
        }

        private static SemanticSearchResult CreateTestSearchResult(string documentId, float score)
        {
            // ✅ Use fluent builder from TestDataBuilders
            var documentResult = TestDataBuilders.CreateValidSemanticDocument(
                id: documentId,
                title: "Test Document",
                content: "test content");
            documentResult.IsSuccess.ShouldBeTrue();
            var document = documentResult.Value!; // Null-forgiving: IsSuccess guarantees non-null

            var results = new List<SearchResultItem>
            {
                new SearchResultItem("result-1", "test content", score, new Dictionary<string, object>(), document.Id)
            };
            return new SemanticSearchResult("search-1", "query-1", results, document, 1L, TimeSpan.FromMilliseconds(100), new Dictionary<string, object>());
        }
    }

    public class GetSystemHealthAsyncTests
    {
        [Fact(Timeout = 5000)]
        public async Task Should_ReturnHealthResult_When_SystemIsHealthy()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var searchService = Substitute.For<ISemanticSearchService>();
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

            var orchestrationService = new SemanticRagOrchestrationService(
                semanticRagService,
                searchService,
                ingestionService,
                extractionService,
                logger);

            var stats = new SemanticRagStats(
                TotalDocuments: 100,
                TotalEntities: 50,
                TotalRelationships: 25,
                LastIndexedAt: DateTimeOffset.UtcNow,
                AverageDocumentSize: 1024.5,
                EmbeddingDimension: 768);

            semanticRagService.GetStatsAsync(Arg.Any<CancellationToken>())
                .Returns(Result<SemanticRagStats>.Success(stats));

            // Act
            var result = await orchestrationService.GetSystemHealthAsync(cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.IsHealthy.ShouldBeTrue();
            result.Value.TotalDocuments.ShouldBe(100);
            result.Value.TotalEntities.ShouldBe(50);
            result.Value.TotalRelationships.ShouldBe(25);
        }

        [Fact(Timeout = 5000)]
        public async Task Should_ReturnFailure_When_StatsRetrievalFails()
        {
            // Arrange
            var semanticRagService = Substitute.For<ISemanticRagService>();
            var searchService = Substitute.For<ISemanticSearchService>();
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var logger = Substitute.For<ILogger<SemanticRagOrchestrationService>>();

            var orchestrationService = new SemanticRagOrchestrationService(
                semanticRagService,
                searchService,
                ingestionService,
                extractionService,
                logger);

            var errorMessage = "Stats retrieval failed";

            semanticRagService.GetStatsAsync(Arg.Any<CancellationToken>())
                .Returns(Result<SemanticRagStats>.WithFailure(errorMessage));

            // Act
            var result = await orchestrationService.GetSystemHealthAsync(cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldNotBeNullOrEmpty();
        }
    }
}

/// <summary>
/// Unit tests for comprehensive search options validation.
/// </summary>
public class ComprehensiveSearchOptionsTests
{
    [Fact(Timeout = 5000)]
    public void Should_CreateDefaultOptions_When_DefaultCalled()
    {
        // Act
        var options = ComprehensiveSearchOptions.Default();

        // Assert
        // Note: SemanticSearchOptions is a struct, so new SemanticSearchOptions() equals default(SemanticSearchOptions)
        // Instead, we verify that the options are properly configured with expected values
        options.RagConfig.ShouldNotBe(default(SemanticRagConfig));
        options.RagConfig.Id.ShouldBe("default");
        options.RagConfig.Name.ShouldBe("Default Configuration");
        options.EnableKnowledgeExtraction.ShouldBeTrue();
        options.MaxResultsForExtraction.ShouldBe(5);
        options.EnableContextRetrieval.ShouldBeTrue();
    }
}

/// <summary>
/// Unit tests for question answer options validation.
/// </summary>
public class QuestionAnswerOptionsTests
{
    [Fact(Timeout = 5000)]
    public void Should_CreateQuestionAnswerOptions_When_ValidParametersProvided()
    {
        // Arrange
        var searchOptions = new SemanticSearchOptions();
        var ragConfig = new SemanticRagConfig(
            Id: "test-config",
            Name: "Test Config",
            EmbeddingModel: "test-model",
            VectorDimensions: 1536,
            SimilarityThreshold: 0.7,
            MaxResults: 10,
            Properties: new Dictionary<string, object>());
        var maxContextDocuments = 10;
        var includeEntityContext = true;
        var includeRelationshipContext = true;

        // Act
        var options = new QuestionAnswerOptions(
            searchOptions,
            ragConfig,
            maxContextDocuments,
            includeEntityContext,
            includeRelationshipContext);

        // Assert
        options.SearchOptions.ShouldBe(searchOptions);
        options.RagConfig.ShouldBe(ragConfig);
        options.MaxContextDocuments.ShouldBe(maxContextDocuments);
        options.IncludeEntityContext.ShouldBe(includeEntityContext);
        options.IncludeRelationshipContext.ShouldBe(includeRelationshipContext);
    }
}