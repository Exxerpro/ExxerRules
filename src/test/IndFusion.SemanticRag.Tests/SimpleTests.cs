namespace IndFusion.SemanticRag.Tests;

/// <summary>
/// Simple tests to verify the test setup is working.
/// </summary>
public class SimpleTests
{
    [Fact]
    public void Should_Be_True()
    {
        // Arrange & Act
        var result = true;

        // Assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void Should_Create_DocumentInput()
    {
        // Arrange & Act
        var input = new DocumentInput(
            Id: "test-1",
            Name: "test.txt",
            Content: "Hello, World!".ToUtf8Bytes(),
            MimeType: "text/plain"
        );

        // Assert
        input.ShouldNotBeNull();
        input.Id.ShouldBe("test-1");
        input.Name.ShouldBe("test.txt");
        input.MimeType.ShouldBe("text/plain");
    }

    [Fact]
    public void Should_Create_DocumentProcessingOptions()
    {
        // Arrange & Act
        var options = new DocumentProcessingOptions();

        // Assert
        options.ShouldNotBeNull();
        options.EnableOcr.ShouldBeTrue();
        options.EnableChunking.ShouldBeTrue();
        options.ChunkingStrategy.ShouldBe(ChunkingStrategy.Recursive);
    }

    [Fact]
    public void Should_Create_DocumentProcessingResult()
    {
        // Arrange & Act
        var result = new DocumentProcessingResult
        {
            Id = "result-1",
            DocumentId = "doc-1",
            Content = "Test content",
            DocumentType = DocumentType.Text,
            Status = ProcessingStatus.Success,
            ElapsedMilliseconds = 100
        };

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe("result-1");
        result.DocumentId.ShouldBe("doc-1");
        result.Content.ShouldBe("Test content");
        result.DocumentType.ShouldBe(DocumentType.Text);
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.ElapsedMilliseconds.ShouldBe(100);
    }
}

/// <summary>
/// Extension methods for test data creation.
/// </summary>
public static class TestDataExtensions
{
    public static byte[] ToUtf8Bytes(this string text) => System.Text.Encoding.UTF8.GetBytes(text);
}
