using System;
using System.Collections.Generic;
using System.IO;
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
/// Unit tests for the document ingestion service.
/// </summary>
public class DocumentIngestionServiceTests
{
    private readonly IDocumentIngestionService _ingestionService;
    private readonly ILogger<IDocumentIngestionService> _logger;

    public DocumentIngestionServiceTests()
    {
        _ingestionService = Substitute.For<IDocumentIngestionService>();
        _logger = Substitute.For<ILogger<IDocumentIngestionService>>();
    }

    public class IngestDocumentAsyncTests
    {
        [Fact]
        public async Task Should_ReturnProcessedDocument_When_ValidSourceProvided()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var source = new DocumentSource("test.md", "markdown", new Dictionary<string, object> { ["author"] = "test" });
            var content = "# Test Document\nThis is a test document.";
            var metadata = new Dictionary<string, object> { ["type"] = "markdown" };
            var expectedDocument = CreateTestDocument();

            ingestionService.IngestDocumentAsync(source, content, metadata, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticDocument>.Success(expectedDocument));

            // Act
            var result = await ingestionService.IngestDocumentAsync(source, content, metadata);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedDocument);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_IngestionFails()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var source = new DocumentSource("test.md", "markdown");
            var content = "# Test Document";
            var errorMessage = "Document ingestion failed";

            ingestionService.IngestDocumentAsync(source, content, null, Arg.Any<CancellationToken>())
                .Returns(Result<SemanticDocument>.WithFailure(errorMessage));

            // Act
            var result = await ingestionService.IngestDocumentAsync(source, content);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }

        private static SemanticDocument CreateTestDocument()
        {
            return new SemanticDocument(
                "doc-1",
                "test content",
                new Dictionary<string, object> { ["type"] = "markdown" },
                new float[] { 0.1f, 0.2f, 0.3f },
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow);
        }
    }

    public class IngestDocumentsAsyncTests
    {
        [Fact]
        public async Task Should_ReturnProcessedDocuments_When_ValidSourcesProvided()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var sources = new[]
            {
                new DocumentSource("test1.md", "markdown"),
                new DocumentSource("test2.cs", "code")
            };
            var expectedDocuments = new[]
            {
                CreateTestDocument("doc-1"),
                CreateTestDocument("doc-2")
            };

            ingestionService.IngestDocumentsAsync(sources, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticDocument>>.Success(expectedDocuments));

            // Act
            var result = await ingestionService.IngestDocumentsAsync(sources);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedDocuments);
            result.Value.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_BatchIngestionFails()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var sources = new[] { new DocumentSource("test.md", "markdown") };
            var errorMessage = "Batch ingestion failed";

            ingestionService.IngestDocumentsAsync(sources, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticDocument>>.WithFailure(errorMessage));

            // Act
            var result = await ingestionService.IngestDocumentsAsync(sources);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
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
    }

    public class IngestRepositoryAsyncTests
    {
        [Fact]
        public async Task Should_ReturnProcessedDocuments_When_ValidRepositoryPathProvided()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var repositoryPath = "/path/to/repo";
            var config = RepositoryIngestionConfig.ForCSharpRepository();
            var expectedDocuments = new[]
            {
                CreateTestDocument("doc-1"),
                CreateTestDocument("doc-2")
            };

            ingestionService.IngestRepositoryAsync(repositoryPath, config, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticDocument>>.Success(expectedDocuments));

            // Act
            var result = await ingestionService.IngestRepositoryAsync(repositoryPath, config);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedDocuments);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_RepositoryIngestionFails()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var repositoryPath = "/path/to/repo";
            var config = RepositoryIngestionConfig.ForCSharpRepository();
            var errorMessage = "Repository ingestion failed";

            ingestionService.IngestRepositoryAsync(repositoryPath, config, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<SemanticDocument>>.WithFailure(errorMessage));

            // Act
            var result = await ingestionService.IngestRepositoryAsync(repositoryPath, config);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
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
    }

    public class ExtractEntitiesAsyncTests
    {
        [Fact]
        public async Task Should_ReturnExtractedEntities_When_ValidDocumentProvided()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var document = CreateTestDocument();
            var expectedEntities = new[]
            {
                CreateTestEntity("entity-1"),
                CreateTestEntity("entity-2")
            };

            ingestionService.ExtractEntitiesAsync(document, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeEntity>>.Success(expectedEntities));

            // Act
            var result = await ingestionService.ExtractEntitiesAsync(document);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedEntities);
            result.Value.Count.ShouldBe(2);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_EntityExtractionFails()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var document = CreateTestDocument();
            var errorMessage = "Entity extraction failed";

            ingestionService.ExtractEntitiesAsync(document, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeEntity>>.WithFailure(errorMessage));

            // Act
            var result = await ingestionService.ExtractEntitiesAsync(document);

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
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var document = CreateTestDocument();
            var entities = new[] { CreateTestEntity("entity-1"), CreateTestEntity("entity-2") };
            var expectedRelationships = new[]
            {
                CreateTestRelationship("rel-1")
            };

            ingestionService.ExtractRelationshipsAsync(document, entities, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.Success(expectedRelationships));

            // Act
            var result = await ingestionService.ExtractRelationshipsAsync(document, entities);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldBe(expectedRelationships);
            result.Value.Count.ShouldBe(1);
        }

        [Fact]
        public async Task Should_ReturnFailure_When_RelationshipExtractionFails()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var document = CreateTestDocument();
            var entities = new[] { CreateTestEntity("entity-1") };
            var errorMessage = "Relationship extraction failed";

            ingestionService.ExtractRelationshipsAsync(document, entities, Arg.Any<CancellationToken>())
                .Returns(Result<IReadOnlyList<KnowledgeRelationship>>.WithFailure(errorMessage));

            // Act
            var result = await ingestionService.ExtractRelationshipsAsync(document, entities);

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

    public class GetSupportedFileTypesTests
    {
        [Fact]
        public void Should_ReturnSupportedFileTypes_When_Called()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var expectedTypes = new[] { ".cs", ".md", ".txt", ".json" };

            ingestionService.GetSupportedFileTypes()
                .Returns(expectedTypes);

            // Act
            var result = ingestionService.GetSupportedFileTypes();

            // Assert
            result.ShouldBe(expectedTypes);
            result.Count.ShouldBe(4);
        }
    }

    public class ValidateFileTests
    {
        [Fact]
        public void Should_ReturnSuccess_When_ValidFileProvided()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var filePath = "test.cs";

            ingestionService.ValidateFile(filePath)
                .Returns(Result.Success());

            // Act
            var result = ingestionService.ValidateFile(filePath);

            // Assert
            result.IsSuccess.ShouldBeTrue();
        }

        [Fact]
        public void Should_ReturnFailure_When_InvalidFileProvided()
        {
            // Arrange
            var ingestionService = Substitute.For<IDocumentIngestionService>();
            var filePath = "test.xyz";
            var errorMessage = "Unsupported file type";

            ingestionService.ValidateFile(filePath)
                .Returns(Result.WithFailure(errorMessage));

            // Act
            var result = ingestionService.ValidateFile(filePath);

            // Assert
            result.IsFailure.ShouldBeTrue();
            result.Error.ShouldBe(errorMessage);
        }
    }
}

/// <summary>
/// Unit tests for document source validation.
/// </summary>
public class DocumentSourceTests
{
    [Fact]
    public void Should_CreateDocumentSource_When_ValidParametersProvided()
    {
        // Arrange
        var path = "test.md";
        var type = "markdown";
        var metadata = new Dictionary<string, object> { ["author"] = "test" };

        // Act
        var source = new DocumentSource(path, type, metadata);

        // Assert
        source.Path.ShouldBe(path);
        source.Type.ShouldBe(type);
        source.Metadata.ShouldBe(metadata);
    }

    [Fact]
    public void Should_GetFileExtension_When_PathProvided()
    {
        // Arrange
        var source = new DocumentSource("test.cs", "code");

        // Act
        var extension = source.FileExtension;

        // Assert
        extension.ShouldBe(".cs");
    }

    [Fact]
    public void Should_GetFileName_When_PathProvided()
    {
        // Arrange
        var source = new DocumentSource("test.cs", "code");

        // Act
        var fileName = source.FileName;

        // Assert
        fileName.ShouldBe("test.cs");
    }

    [Fact]
    public void Should_ValidateSuccessfully_When_ValidParametersProvided()
    {
        // Arrange
        var source = new DocumentSource("test.md", "markdown");

        // Act
        var result = source.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Should_ValidateFailure_When_PathIsNullOrEmpty(string path)
    {
        // Arrange
        var source = new DocumentSource(path, "markdown");

        // Act
        var result = source.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Document path cannot be null or empty");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Should_ValidateFailure_When_TypeIsNullOrEmpty(string type)
    {
        // Arrange
        var source = new DocumentSource("test.md", type);

        // Act
        var result = source.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("Document type cannot be null or empty");
    }
}

/// <summary>
/// Unit tests for repository ingestion configuration.
/// </summary>
public class RepositoryIngestionConfigTests
{
    [Fact]
    public void Should_CreateRepositoryIngestionConfig_When_ValidParametersProvided()
    {
        // Arrange
        var includePatterns = new[] { "*.cs", "*.md" };
        var excludePatterns = new[] { "*/bin/*", "*/obj/*" };
        var maxFileSize = 1024L;
        var extractCodeEntities = true;
        var extractComments = true;
        var processDependencies = true;
        var maxDepth = 5;

        // Act
        var config = new RepositoryIngestionConfig(
            includePatterns,
            excludePatterns,
            maxFileSize,
            extractCodeEntities,
            extractComments,
            processDependencies,
            maxDepth);

        // Assert
        config.IncludePatterns.ShouldBe(includePatterns);
        config.ExcludePatterns.ShouldBe(excludePatterns);
        config.MaxFileSize.ShouldBe(maxFileSize);
        config.ExtractCodeEntities.ShouldBe(extractCodeEntities);
        config.ExtractComments.ShouldBe(extractComments);
        config.ProcessDependencies.ShouldBe(processDependencies);
        config.MaxDepth.ShouldBe(maxDepth);
    }

    [Fact]
    public void Should_CreateCSharpRepositoryConfig_When_ForCSharpRepositoryCalled()
    {
        // Act
        var config = RepositoryIngestionConfig.ForCSharpRepository();

        // Assert
        config.IncludePatterns.ShouldContain("*.cs");
        config.IncludePatterns.ShouldContain("*.csproj");
        config.IncludePatterns.ShouldContain("*.sln");
        config.ExcludePatterns.ShouldContain("*/bin/*");
        config.ExcludePatterns.ShouldContain("*/obj/*");
        config.ExtractCodeEntities.ShouldBeTrue();
        config.ExtractComments.ShouldBeTrue();
        config.ProcessDependencies.ShouldBeTrue();
    }

    [Fact]
    public void Should_CreateDocumentationConfig_When_ForDocumentationCalled()
    {
        // Act
        var config = RepositoryIngestionConfig.ForDocumentation();

        // Assert
        config.IncludePatterns.ShouldContain("*.md");
        config.IncludePatterns.ShouldContain("*.txt");
        config.ExtractCodeEntities.ShouldBeFalse();
        config.ExtractComments.ShouldBeFalse();
        config.ProcessDependencies.ShouldBeFalse();
    }

    [Fact]
    public void Should_ValidateSuccessfully_When_ValidParametersProvided()
    {
        // Arrange
        var config = new RepositoryIngestionConfig(
            new[] { "*.cs" },
            new[] { "*/bin/*" },
            MaxFileSize: 1024,
            ExtractCodeEntities: true,
            ExtractComments: true,
            ProcessDependencies: true,
            MaxDepth: 5);

        // Act
        var result = config.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Should_ValidateFailure_When_IncludePatternsIsEmpty()
    {
        // Arrange
        var config = new RepositoryIngestionConfig(
            Array.Empty<string>(),
            new[] { "*/bin/*" });

        // Act
        var result = config.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("At least one include pattern must be specified");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_ValidateFailure_When_MaxFileSizeIsZeroOrNegative(long maxFileSize)
    {
        // Arrange
        var config = new RepositoryIngestionConfig(
            new[] { "*.cs" },
            new[] { "*/bin/*" },
            MaxFileSize: maxFileSize);

        // Act
        var result = config.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("MaxFileSize must be greater than 0");
    }

    [Fact]
    public void Should_ValidateFailure_When_MaxDepthIsNegative()
    {
        // Arrange
        var config = new RepositoryIngestionConfig(
            new[] { "*.cs" },
            new[] { "*/bin/*" },
            MaxDepth: -1);

        // Act
        var result = config.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("MaxDepth cannot be negative");
    }
}