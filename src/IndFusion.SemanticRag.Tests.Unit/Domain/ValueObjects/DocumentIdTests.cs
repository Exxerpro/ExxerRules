using IndFusion.SemanticRag.Domain.ValueObjects;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.ValueObjects;

/// <summary>
/// Unit tests for DocumentId value object.
/// </summary>
public class DocumentIdTests
{
    [Fact]
    public void Should_CreateDocumentId_When_ValidValueProvided()
    {
        // Arrange
        var value = "doc-123";

        // Act
        var documentId = new DocumentId(value);

        // Assert
        documentId.Value.ShouldBe(value);
    }

    [Fact]
    public void Should_ValidateSuccessfully_When_ValidDocumentId()
    {
        // Arrange
        var documentId = new DocumentId("doc-123");

        // Act
        var result = documentId.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Should_ValidateFailure_When_ValueIsNullOrEmptyOrWhitespace(string? value)
    {
        // Arrange
        var documentId = new DocumentId(value!);

        // Act
        var result = documentId.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cannot be null, empty, or whitespace");
    }

    [Fact]
    public void Should_ValidateFailure_When_ValueExceedsMaxLength()
    {
        // Arrange
        var longValue = new string('a', 256); // 256 characters
        var documentId = new DocumentId(longValue);

        // Act
        var result = documentId.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("cannot exceed 255 characters");
    }

    [Theory]
    [InlineData("doc@123")] // Contains @
    [InlineData("doc#123")] // Contains #
    [InlineData("doc$123")] // Contains $
    [InlineData("doc%123")] // Contains %
    [InlineData("doc 123")] // Contains space
    [InlineData("doc\t123")] // Contains tab
    [InlineData("doc\n123")] // Contains newline
    public void Should_ValidateFailure_When_ValueContainsInvalidCharacters(string value)
    {
        // Arrange
        var documentId = new DocumentId(value);

        // Act
        var result = documentId.Validate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error!.ShouldContain("can only contain letters, digits, hyphens, underscores, and dots");
    }

    [Theory]
    [InlineData("doc-123")]
    [InlineData("doc_123")]
    [InlineData("doc.123")]
    [InlineData("DOC123")]
    [InlineData("doc123")]
    [InlineData("123doc")]
    [InlineData("doc-123_test.456")]
    public void Should_ValidateSuccessfully_When_ValueContainsValidCharacters(string value)
    {
        // Arrange
        var documentId = new DocumentId(value);

        // Act
        var result = documentId.Validate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Should_CreateSuccessfully_When_ValidValueProvided()
    {
        // Arrange
        var value = "doc-123";

        // Act
        var result = DocumentId.Create(value);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(value);
    }

    [Fact]
    public void Should_CreateFailure_When_InvalidValueProvided()
    {
        // Arrange
        var value = "";

        // Act
        var result = DocumentId.Create(value);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact]
    public void Should_ImplicitlyConvertToString()
    {
        // Arrange
        var documentId = new DocumentId("doc-123");

        // Act
        string value = documentId;

        // Assert
        value.ShouldBe("doc-123");
    }

    [Fact]
    public void Should_ReturnStringValue_When_ToStringCalled()
    {
        // Arrange
        var documentId = new DocumentId("doc-123");

        // Act
        var result = documentId.ToString();

        // Assert
        result.ShouldBe("doc-123");
    }

    [Fact]
    public void Should_BeEqual_When_SameValue()
    {
        // Arrange
        var documentId1 = new DocumentId("doc-123");
        var documentId2 = new DocumentId("doc-123");

        // Act & Assert
        documentId1.ShouldBe(documentId2);
        (documentId1 == documentId2).ShouldBeTrue();
        (documentId1 != documentId2).ShouldBeFalse();
    }

    [Fact]
    public void Should_NotBeEqual_When_DifferentValue()
    {
        // Arrange
        var documentId1 = new DocumentId("doc-123");
        var documentId2 = new DocumentId("doc-456");

        // Act & Assert
        documentId1.ShouldNotBe(documentId2);
        (documentId1 == documentId2).ShouldBeFalse();
        (documentId1 != documentId2).ShouldBeTrue();
    }
}
