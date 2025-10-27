using IndFusion.SemanticRag.Domain.Models;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Models;

/// <summary>
/// Unit tests for Document domain model.
/// </summary>
public class DocumentTests
{
    [Fact]
    public void Should_CreateDocument_When_ValidParametersProvided()
    {
        // Arrange
        var id = "doc-123";
        var content = "Test document content";
        var sourcePath = "/path/to/file.cs";
        var repository = "test-repo";
        var commitHash = "abc123";
        var documentType = DocumentType.SourceCode;
        var metadata = new Dictionary<string, object> { ["key"] = "value" };
        var createdAt = DateTimeOffset.UtcNow;
        var updatedAt = DateTimeOffset.UtcNow;

        // Act
        var document = new Document(
            id, content, sourcePath, repository, commitHash,
            documentType, metadata, createdAt, updatedAt);

        // Assert
        document.Id.ShouldBe(id);
        document.Content.ShouldBe(content);
        document.SourcePath.ShouldBe(sourcePath);
        document.Repository.ShouldBe(repository);
        document.CommitHash.ShouldBe(commitHash);
        document.DocumentType.ShouldBe(documentType);
        document.Metadata.ShouldBe(metadata);
        document.CreatedAt.ShouldBe(createdAt);
        document.UpdatedAt.ShouldBe(updatedAt);
    }

    [Fact]
    public void Should_ValidateSuccessfully_When_ValidDocument()
    {
        // Arrange
        var document = CreateValidDocument();

        // Act
        var result = document.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Theory]
    [InlineData("", "Content", "SourcePath", "Repository", "CommitHash")]
    [InlineData("Id", "", "SourcePath", "Repository", "CommitHash")]
    [InlineData("Id", "Content", "", "Repository", "CommitHash")]
    [InlineData("Id", "Content", "SourcePath", "", "CommitHash")]
    [InlineData("Id", "Content", "SourcePath", "Repository", "")]
    [InlineData("   ", "Content", "SourcePath", "Repository", "CommitHash")]
    [InlineData("Id", "   ", "SourcePath", "Repository", "CommitHash")]
    [InlineData("Id", "Content", "   ", "Repository", "CommitHash")]
    [InlineData("Id", "Content", "SourcePath", "   ", "CommitHash")]
    [InlineData("Id", "Content", "SourcePath", "Repository", "   ")]
    public void Should_ValidateFailure_When_RequiredFieldsAreEmptyOrWhitespace(
        string id, string content, string sourcePath, string repository, string commitHash)
    {
        // Arrange
        var document = new Document(
            id, content, sourcePath, repository, commitHash,
            DocumentType.SourceCode, new Dictionary<string, object>(),
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        // Act
        var result = document.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Should_ValidateFailure_When_IdIsNull()
    {
        // Arrange
        var document = new Document(
            null!, "Content", "SourcePath", "Repository", "CommitHash",
            DocumentType.SourceCode, new Dictionary<string, object>(),
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        // Act
        var result = document.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Id");
    }

    [Fact]
    public void Should_ValidateFailure_When_ContentIsNull()
    {
        // Arrange
        var document = new Document(
            "Id", null!, "SourcePath", "Repository", "CommitHash",
            DocumentType.SourceCode, new Dictionary<string, object>(),
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        // Act
        var result = document.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Content");
    }

    [Fact]
    public void Should_ValidateFailure_When_SourcePathIsNull()
    {
        // Arrange
        var document = new Document(
            "Id", "Content", null!, "Repository", "CommitHash",
            DocumentType.SourceCode, new Dictionary<string, object>(),
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        // Act
        var result = document.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("SourcePath");
    }

    [Fact]
    public void Should_ValidateFailure_When_RepositoryIsNull()
    {
        // Arrange
        var document = new Document(
            "Id", "Content", "SourcePath", null!, "CommitHash",
            DocumentType.SourceCode, new Dictionary<string, object>(),
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        // Act
        var result = document.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Repository");
    }

    [Fact]
    public void Should_ValidateFailure_When_CommitHashIsNull()
    {
        // Arrange
        var document = new Document(
            "Id", "Content", "SourcePath", "Repository", null!,
            DocumentType.SourceCode, new Dictionary<string, object>(),
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        // Act
        var result = document.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("CommitHash");
    }

    [Fact]
    public void Should_ValidateFailure_When_MetadataIsNull()
    {
        // Arrange
        var document = new Document(
            "Id", "Content", "SourcePath", "Repository", "CommitHash",
            DocumentType.SourceCode, null!,
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);

        // Act
        var result = document.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("Metadata");
    }

    private static Document CreateValidDocument()
    {
        return new Document(
            "doc-123",
            "Test document content",
            "/path/to/file.cs",
            "test-repo",
            "abc123",
            DocumentType.SourceCode,
            new Dictionary<string, object> { ["key"] = "value" },
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow);
    }
}
