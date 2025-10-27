using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Services;
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
        [Fact]
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

            // Act
            var result = await orchestrationService.ComprehensiveSearchAsync(query, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.Query.ShouldBe(query);
            result.Value.SearchResults.ShouldBe(searchResponse.Results);
            result.Value.AdditionalContext.ShouldBe(context);
        }

        [Fact]
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
            result.Error.ShouldContain("Search failed");
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
            var document = new SemanticDocument(
                documentId,
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);

            return new SemanticSearchResult(document, score, Array.Empty<string>());
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

    public class IngestRepositoryAsyncTests
    {
        [Fact]
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
            var config = RepositoryIngestionConfig.ForCSharpRepository();
            var documents = new[] { CreateTestDocument("doc-1"), CreateTestDocument("doc-2") };
            var extractionResult = CreateTestKnowledgeExtractionResult();

            ingestionService.IngestRepositoryAsync(repositoryPath, config, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticDocument>>.Success(documents));

            extractionService.ExtractKnowledgeAsync(Arg.Any<SemanticDocument>(), Arg.Any<ComprehensiveExtractionOptions>(), Arg.Any<CancellationToken>())
                .Returns(Result<KnowledgeExtractionResult>.Success(extractionResult));

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

        [Fact]
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
            var config = RepositoryIngestionConfig.ForCSharpRepository();
            var errorMessage = "Document ingestion failed";

            ingestionService.IngestRepositoryAsync(repositoryPath, config, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticDocument>>.WithFailure(errorMessage));

            // Act
            var result = await orchestrationService.IngestRepositoryAsync(repositoryPath, config, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldContain("Document ingestion failed");
        }

        private static SemanticDocument CreateTestDocument(string id = "doc-1")
        {
            return new SemanticDocument(
                id,
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);
        }

        private static KnowledgeExtractionResult CreateTestKnowledgeExtractionResult()
        {
            var entities = new[] { CreateTestEntity() };
            var relationships = new[] { CreateTestRelationship() };
            var codeEntities = new[] { CreateTestCodeEntity() };
            var concepts = new[] { CreateTestConcept() };

            return new KnowledgeExtractionResult(
                entities,
                relationships,
                codeEntities,
                concepts,
                ProcessingTimeMs: 1000,
                Confidence: 0.8f);
        }

        private static KnowledgeEntity CreateTestEntity()
        {
            return new KnowledgeEntity(
                "entity-1",
                "Person",
                "John Doe",
                "Software Engineer",
                new Dictionary<string, object>(),
                null);
        }

        private static KnowledgeRelationship CreateTestRelationship()
        {
            return new KnowledgeRelationship(
                "rel-1",
                "entity-1",
                "entity-2",
                "RELATES_TO",
                new Dictionary<string, object>(),
                DateTimeOffset.UtcNow);
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
        [Fact]
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
                new SemanticRagConfig());
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

        [Fact]
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
                new SemanticRagConfig());
            var errorMessage = "Context retrieval failed";

            semanticRagService.GetContextAsync(question, options.RagConfig, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticContext>.WithFailure(errorMessage));

            // Act
            var result = await orchestrationService.AnswerQuestionAsync(question, options, cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldContain("Context retrieval failed");
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

    public class GetSystemHealthAsyncTests
    {
        [Fact]
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

        [Fact]
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
            result.Error.ShouldContain("Stats retrieval failed");
        }
    }
}

/// <summary>
/// Unit tests for comprehensive search options validation.
/// </summary>
public class ComprehensiveSearchOptionsTests
{
    [Fact]
    public void Should_CreateDefaultOptions_When_DefaultCalled()
    {
        // Act
        var options = ComprehensiveSearchOptions.Default();

        // Assert
        options.SearchOptions.ShouldNotBe(default(SemanticSearchOptions));
        options.RagConfig.ShouldNotBe(default(SemanticRagConfig));
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
    [Fact]
    public void Should_CreateQuestionAnswerOptions_When_ValidParametersProvided()
    {
        // Arrange
        var searchOptions = new SemanticSearchOptions();
        var ragConfig = new SemanticRagConfig();
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