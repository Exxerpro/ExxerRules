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
/// Unit tests for the knowledge extraction service.
/// </summary>
public class KnowledgeExtractionServiceTests
{
    private readonly IKnowledgeExtractionService _extractionService;
    private readonly ILogger<IKnowledgeExtractionService> _logger;

    public KnowledgeExtractionServiceTests()
    {
        _extractionService = Substitute.For<IKnowledgeExtractionService>();
        _logger = Substitute.For<ILogger<IKnowledgeExtractionService>>();
    }

    public class ExtractEntitiesAsyncTests
    {
        [Fact]
        public async Task Should_ReturnExtractedEntities_When_ValidDocumentProvided()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestDocument();
            var options = EntityExtractionOptions.Default();
            var expectedEntities = new[]
            {
                CreateTestEntity("entity-1"),
                CreateTestEntity("entity-2")
            };

            extractionService.ExtractEntitiesAsync(document, options, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

            // Act
            var result = await extractionService.ExtractEntitiesAsync(document, options);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedEntities);
            result.Value.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_EntityExtractionFails()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestDocument();
            var options = EntityExtractionOptions.Default();
            var errorMessage = "Entity extraction failed";

            extractionService.ExtractEntitiesAsync(document, options, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeEntity>>.WithFailure(errorMessage));

            // Act
            var result = await extractionService.ExtractEntitiesAsync(document, options);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticDocument CreateTestDocument()
        {
            return new SemanticDocument(
                "doc-1",
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);
        }

        private static KnowledgeEntity CreateTestEntity(string id = "entity-1")
        {
            return new KnowledgeEntity(
                id,
                "Person",
                "John Doe",
                "Software Engineer",
                new Dictionary<string, object>(),
                null);
        }
    }

    public class ExtractRelationshipsAsyncTests
    {
        [Fact]
        public async Task Should_ReturnExtractedRelationships_When_ValidDocumentAndEntitiesProvided()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestDocument();
            var entities = new[] { CreateTestEntity("entity-1"), CreateTestEntity("entity-2") };
            var options = RelationshipExtractionOptions.Default();
            var expectedRelationships = new[]
            {
                CreateTestRelationship("rel-1")
            };

            extractionService.ExtractRelationshipsAsync(document, entities, options, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

            // Act
            var result = await extractionService.ExtractRelationshipsAsync(document, entities, options);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedRelationships);
            result.Value.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_RelationshipExtractionFails()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestDocument();
            var entities = new[] { CreateTestEntity("entity-1") };
            var options = RelationshipExtractionOptions.Default();
            var errorMessage = "Relationship extraction failed";

            extractionService.ExtractRelationshipsAsync(document, entities, options, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure(errorMessage));

            // Act
            var result = await extractionService.ExtractRelationshipsAsync(document, entities, options);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticDocument CreateTestDocument()
        {
            return new SemanticDocument(
                "doc-1",
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);
        }

        private static KnowledgeEntity CreateTestEntity(string id = "entity-1")
        {
            return new KnowledgeEntity(
                id,
                "Person",
                "John Doe",
                "Software Engineer",
                new Dictionary<string, object>(),
                null);
        }

        private static KnowledgeRelationship CreateTestRelationship(string id = "rel-1")
        {
            return new KnowledgeRelationship(
                id,
                "entity-1",
                "entity-2",
                "RELATES_TO",
                new Dictionary<string, object>(),
                DateTimeOffset.UtcNow);
        }
    }

    public class ExtractCodeEntitiesAsyncTests
    {
        [Fact]
        public async Task Should_ReturnExtractedCodeEntities_When_ValidCodeDocumentProvided()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestCodeDocument();
            var options = CodeExtractionOptions.Comprehensive();
            var expectedCodeEntities = new[]
            {
                CreateTestCodeEntity("class-1"),
                CreateTestCodeEntity("method-1")
            };

            extractionService.ExtractCodeEntitiesAsync(document, options, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<CodeEntity>>.Success(expectedCodeEntities));

            // Act
            var result = await extractionService.ExtractCodeEntitiesAsync(document, options);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedCodeEntities);
            result.Value.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_CodeEntityExtractionFails()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestCodeDocument();
            var options = CodeExtractionOptions.Comprehensive();
            var errorMessage = "Code entity extraction failed";

            extractionService.ExtractCodeEntitiesAsync(document, options, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<CodeEntity>>.WithFailure(errorMessage));

            // Act
            var result = await extractionService.ExtractCodeEntitiesAsync(document, options);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticDocument CreateTestCodeDocument()
        {
            return new SemanticDocument(
                "code.cs",
                "public class TestClass { public void TestMethod() { } }",
                new Dictionary<string, object> { ["type"] = "code" },
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);
        }

        private static CodeEntity CreateTestCodeEntity(string id = "class-1")
        {
            return new CodeEntity(
                id,
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
    }

    public class ExtractConceptsAsyncTests
    {
        [Fact]
        public async Task Should_ReturnExtractedConcepts_When_ValidDocumentProvided()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestDocument();
            var options = new ConceptExtractionOptions();
            var expectedConcepts = new[]
            {
                CreateTestConcept("concept-1"),
                CreateTestConcept("concept-2")
            };

            extractionService.ExtractConceptsAsync(document, options, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticConcept>>.Success(expectedConcepts));

            // Act
            var result = await extractionService.ExtractConceptsAsync(document, options);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedConcepts);
            result.Value.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_ConceptExtractionFails()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestDocument();
            var options = new ConceptExtractionOptions();
            var errorMessage = "Concept extraction failed";

            extractionService.ExtractConceptsAsync(document, options, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticConcept>>.WithFailure(errorMessage));

            // Act
            var result = await extractionService.ExtractConceptsAsync(document, options);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticDocument CreateTestDocument()
        {
            return new SemanticDocument(
                "doc-1",
                "test content",
                new Dictionary<string, object>(),
                null,
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);
        }

        private static SemanticConcept CreateTestConcept(string id = "concept-1")
        {
            return new SemanticConcept(
                id,
                "Test Concept",
                "A test concept",
                new[] { "synonym1", "synonym2" },
                Frequency: 3,
                Context: "test context",
                null);
        }
    }

    public class ExtractKnowledgeAsyncTests
    {
        [Fact]
        public async Task Should_ReturnComprehensiveKnowledge_When_ValidDocumentProvided()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestDocument();
            var options = ComprehensiveExtractionOptions.Default();
            var expectedResult = CreateTestKnowledgeExtractionResult();

            extractionService.ExtractKnowledgeAsync(document, options, Arg.Any<CancellationToken>())
                .Returns(Result<KnowledgeExtractionResult>.Success(expectedResult));

            // Act
            var result = await extractionService.ExtractKnowledgeAsync(document, options);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedResult);
            result.Value.HasKnowledge.ShouldBeTrue();
        }

        [Fact]
        public async Task Should_ReturnFailure_When_KnowledgeExtractionFails()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var document = CreateTestDocument();
            var options = ComprehensiveExtractionOptions.Default();
            var errorMessage = "Knowledge extraction failed";

            extractionService.ExtractKnowledgeAsync(document, options, Arg.Any<CancellationToken>())
                .Returns(Result<KnowledgeExtractionResult>.WithFailure(errorMessage));

            // Act
            var result = await extractionService.ExtractKnowledgeAsync(document, options);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticDocument CreateTestDocument()
        {
            return new SemanticDocument(
                "doc-1",
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

    public class ValidateEntitiesAsyncTests
    {
        [Fact]
        public async Task Should_ReturnValidationResult_When_ValidEntitiesProvided()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var entities = new[] { CreateTestEntity("entity-1"), CreateTestEntity("entity-2") };
            var expectedResult = new EntityValidationResult(
                entities,
                Array.Empty<KnowledgeEntity>(),
                Array.Empty<string>(),
                OverallQuality: 0.9f);

            extractionService.ValidateEntitiesAsync(entities, Arg.Any<CancellationToken>())
                .Returns(Result<EntityValidationResult>.Success(expectedResult));

            // Act
            var result = await extractionService.ValidateEntitiesAsync(entities);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedResult);
            result.Value.ValidEntities.Count.ShouldBe(2);
            result.Value.InvalidEntities.Count.ShouldBe(0);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_EntityValidationFails()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var entities = new[] { CreateTestEntity("entity-1") };
            var errorMessage = "Entity validation failed";

            extractionService.ValidateEntitiesAsync(entities, Arg.Any<CancellationToken>())
                .Returns(Result<EntityValidationResult>.WithFailure(errorMessage));

            // Act
            var result = await extractionService.ValidateEntitiesAsync(entities);

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
                null);
        }
    }

    public class GetSupportedEntityTypesTests
    {
        [Fact]
        public void Should_ReturnSupportedEntityTypes_When_Called()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var expectedTypes = new[] { "PERSON", "ORGANIZATION", "LOCATION", "CONCEPT" };

            extractionService.GetSupportedEntityTypes()
                .Returns(expectedTypes);

            // Act
            var result = extractionService.GetSupportedEntityTypes();

            // Assert
            result.ShouldBe(expectedTypes);
            result.Count.ShouldBe(4);
        }
    }

    public class GetSupportedRelationshipTypesTests
    {
        [Fact]
        public void Should_ReturnSupportedRelationshipTypes_When_Called()
        {
            // Arrange
            var extractionService = Substitute.For<IKnowledgeExtractionService>();
            var expectedTypes = new[] { "RELATES_TO", "CONTAINS", "SIMILAR_TO", "DEPENDS_ON" };

            extractionService.GetSupportedRelationshipTypes()
                .Returns(expectedTypes);

            // Act
            var result = extractionService.GetSupportedRelationshipTypes();

            // Assert
            result.ShouldBe(expectedTypes);
            result.Count.ShouldBe(4);
        }
    }
}

/// <summary>
/// Unit tests for entity extraction options validation.
/// </summary>
public class EntityExtractionOptionsTests
{
    [Fact]
    public void Should_CreateDefaultOptions_When_DefaultCalled()
    {
        // Act
        var options = EntityExtractionOptions.Default();

        // Assert
        options.EntityTypes.ShouldContain("PERSON");
        options.EntityTypes.ShouldContain("ORGANIZATION");
        options.EntityTypes.ShouldContain("LOCATION");
        options.EntityTypes.ShouldContain("CONCEPT");
        options.EntityTypes.ShouldContain("TECHNOLOGY");
        options.MinConfidence.ShouldBe(0.7f);
        options.MaxEntities.ShouldBe(50);
        options.IncludeContext.ShouldBeTrue();
        options.EnableNestedExtraction.ShouldBeTrue();
    }

    [Fact]
    public void Should_CreateCodeOptions_When_ForCodeCalled()
    {
        // Act
        var options = EntityExtractionOptions.ForCode();

        // Assert
        options.EntityTypes.ShouldContain("CLASS");
        options.EntityTypes.ShouldContain("METHOD");
        options.EntityTypes.ShouldContain("INTERFACE");
        options.EntityTypes.ShouldContain("NAMESPACE");
        options.EntityTypes.ShouldContain("PROPERTY");
        options.EntityTypes.ShouldContain("FIELD");
        options.MinConfidence.ShouldBe(0.8f);
        options.MaxEntities.ShouldBe(200);
    }
}

/// <summary>
/// Unit tests for relationship extraction options validation.
/// </summary>
public class RelationshipExtractionOptionsTests
{
    [Fact]
    public void Should_CreateDefaultOptions_When_DefaultCalled()
    {
        // Act
        var options = RelationshipExtractionOptions.Default();

        // Assert
        options.RelationshipTypes.ShouldContain("RELATES_TO");
        options.RelationshipTypes.ShouldContain("CONTAINS");
        options.RelationshipTypes.ShouldContain("SIMILAR_TO");
        options.RelationshipTypes.ShouldContain("DEPENDS_ON");
        options.RelationshipTypes.ShouldContain("IMPLEMENTS");
        options.MinConfidence.ShouldBe(0.6f);
        options.MaxRelationships.ShouldBe(30);
        options.EnableBidirectional.ShouldBeTrue();
        options.IncludeWeight.ShouldBeTrue();
    }
}

/// <summary>
/// Unit tests for code extraction options validation.
/// </summary>
public class CodeExtractionOptionsTests
{
    [Fact]
    public void Should_CreateComprehensiveOptions_When_ComprehensiveCalled()
    {
        // Act
        var options = CodeExtractionOptions.Comprehensive();

        // Assert
        options.ExtractClasses.ShouldBeTrue();
        options.ExtractMethods.ShouldBeTrue();
        options.ExtractInterfaces.ShouldBeTrue();
        options.ExtractProperties.ShouldBeTrue();
        options.ExtractFields.ShouldBeTrue();
        options.ExtractNamespaces.ShouldBeTrue();
        options.IncludeAccessModifiers.ShouldBeTrue();
        options.IncludeParameters.ShouldBeTrue();
        options.IncludeReturnTypes.ShouldBeTrue();
    }

    [Fact]
    public void Should_CreateMinimalOptions_When_MinimalCalled()
    {
        // Act
        var options = CodeExtractionOptions.Minimal();

        // Assert
        options.ExtractClasses.ShouldBeTrue();
        options.ExtractMethods.ShouldBeFalse();
        options.ExtractInterfaces.ShouldBeFalse();
        options.ExtractProperties.ShouldBeFalse();
        options.ExtractFields.ShouldBeFalse();
        options.ExtractNamespaces.ShouldBeTrue();
        options.IncludeAccessModifiers.ShouldBeFalse();
        options.IncludeParameters.ShouldBeFalse();
        options.IncludeReturnTypes.ShouldBeFalse();
    }
}

/// <summary>
/// Unit tests for concept extraction options validation.
/// </summary>
public class ConceptExtractionOptionsTests
{
    [Fact]
    public void Should_ValidateSuccessfully_When_ValidParametersProvided()
    {
        // Arrange
        var options = new ConceptExtractionOptions(
            MinFrequency: 2,
            MaxConcepts: 50,
            IncludeSynonyms: true,
            IncludeDefinitions: true,
            MinLength: 3,
            MaxLength: 50);

        // Act
        var result = options.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_ValidateFailure_When_MinFrequencyIsZeroOrNegative(int minFrequency)
    {
        // Arrange
        var options = new ConceptExtractionOptions(MinFrequency: minFrequency);

        // Act
        var result = options.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("MinFrequency must be at least 1");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_ValidateFailure_When_MaxConceptsIsZeroOrNegative(int maxConcepts)
    {
        // Arrange
        var options = new ConceptExtractionOptions(MaxConcepts: maxConcepts);

        // Act
        var result = options.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("MaxConcepts must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_ValidateFailure_When_MinLengthIsZeroOrNegative(int minLength)
    {
        // Arrange
        var options = new ConceptExtractionOptions(MinLength: minLength);

        // Act
        var result = options.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("MinLength must be at least 1");
    }

    [Fact]
    public void Should_ValidateFailure_When_MaxLengthIsLessThanMinLength()
    {
        // Arrange
        var options = new ConceptExtractionOptions(MinLength: 10, MaxLength: 5);

        // Act
        var result = options.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("MaxLength must be greater than or equal to MinLength");
    }
}

/// <summary>
/// Unit tests for comprehensive extraction options validation.
/// </summary>
public class ComprehensiveExtractionOptionsTests
{
    [Fact]
    public void Should_CreateDefaultOptions_When_DefaultCalled()
    {
        // Act
        var options = ComprehensiveExtractionOptions.Default();

        // Assert
        options.EntityOptions.ShouldNotBe(default(EntityExtractionOptions));
        options.RelationshipOptions.ShouldNotBe(default(RelationshipExtractionOptions));
        options.CodeOptions.ShouldNotBe(default(CodeExtractionOptions));
        options.ConceptOptions.ShouldNotBe(default(ConceptExtractionOptions));
        options.EnableParallelProcessing.ShouldBeTrue();
        options.MaxProcessingTimeMs.ShouldBe(30000);
    }
}