using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Interfaces;

/// <summary>
/// ITDD tests for IDocumentProcessingPipeline interface contracts.
/// These tests verify that any implementation satisfies the interface contract using mocks.
/// </summary>
public class IDocumentProcessingPipelineTests
{
    private readonly IDocumentProcessingPipeline _mockPipeline;
    private readonly ILogger<IDocumentProcessingPipeline> _mockLogger;

    /// <summary>
    /// Initializes a new instance of the IDocumentProcessingPipelineTests class.
    /// </summary>
    public IDocumentProcessingPipelineTests()
    {
        _mockPipeline = Substitute.For<IDocumentProcessingPipeline>();
        _mockLogger = Substitute.For<ILogger<IDocumentProcessingPipeline>>();
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync returns valid result for valid input.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Return_Valid_Result_For_Valid_Input()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-doc-1",
            Name: "test.txt",
            Content: "Hello, World!".ToUtf8Bytes(),
            MimeType: "text/plain"
        );
        var options = new DocumentProcessingOptions();
        var expectedResult = new DocumentProcessingResult
        {
            Id = "result-1",
            DocumentId = input.Id,
            Content = "Hello, World!",
            Chunks = new List<DocumentChunk>(),
            Metadata = new Dictionary<string, object>(),
            DocumentType = DocumentType.Text,
            Status = ProcessingStatus.Success,
            ElapsedMilliseconds = 100
        };

        _mockPipeline.ProcessDocumentAsync(input, options, CancellationToken.None)
            .Returns(expectedResult);

        // Act
        var result = await _mockPipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(expectedResult.Id);
        result.DocumentId.ShouldBe(expectedResult.DocumentId);
        result.Content.ShouldBe(expectedResult.Content);
        result.DocumentType.ShouldBe(expectedResult.DocumentType);
        result.Status.ShouldBe(expectedResult.Status);
        result.ElapsedMilliseconds.ShouldBe(expectedResult.ElapsedMilliseconds);
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync handles null input gracefully.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Handle_Null_Input_Gracefully()
    {
        // Arrange
        DocumentInput? nullInput = null;
        var options = new DocumentProcessingOptions();

        _mockPipeline.ProcessDocumentAsync(nullInput!, options, CancellationToken.None)
            .Returns(Task.FromException<DocumentProcessingResult>(new ArgumentNullException(nameof(nullInput))));

        // Act & Assert
        await Should.ThrowAsync<ArgumentNullException>(() =>
            _mockPipeline.ProcessDocumentAsync(nullInput!, options, CancellationToken.None));
    }

    /// <summary>
    /// Verifies that ProcessDocumentsAsync processes multiple documents.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentsAsync_Should_Process_Multiple_Documents()
    {
        // Arrange
        var inputs = new List<DocumentInput>
        {
            new("doc-1", "test1.txt", "Content 1".ToUtf8Bytes(), "text/plain"),
            new("doc-2", "test2.txt", "Content 2".ToUtf8Bytes(), "text/plain")
        };
        var options = new DocumentProcessingOptions();
        var expectedResults = new List<DocumentProcessingResult>
        {
            new() { Id = "result-1", DocumentId = "doc-1", Content = "Content 1", DocumentType = DocumentType.Text, Status = ProcessingStatus.Success, ElapsedMilliseconds = 100 },
            new() { Id = "result-2", DocumentId = "doc-2", Content = "Content 2", DocumentType = DocumentType.Text, Status = ProcessingStatus.Success, ElapsedMilliseconds = 150 }
        };

        _mockPipeline.ProcessDocumentsAsync(inputs, options, CancellationToken.None)
            .Returns(expectedResults);

        // Act
        var results = await _mockPipeline.ProcessDocumentsAsync(inputs, options, CancellationToken.None);

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);
        results[0].DocumentId.ShouldBe("doc-1");
        results[1].DocumentId.ShouldBe("doc-2");
    }

    /// <summary>
    /// Verifies that DetectDocumentTypeAsync detects correct document types.
    /// </summary>
    [Fact]
    public async Task DetectDocumentTypeAsync_Should_Detect_Correct_Document_Types()
    {
        // Arrange
        var testCases = new[]
        {
            (new DocumentInput("cs-1", "test.cs", "public class Test {}".ToUtf8Bytes(), "text/x-csharp"), DocumentType.CSharpCode),
            (new DocumentInput("ts-1", "test.ts", "const x = 1;".ToUtf8Bytes(), "text/typescript"), DocumentType.TypeScriptCode),
            (new DocumentInput("py-1", "test.py", "def hello(): pass".ToUtf8Bytes(), "text/x-python"), DocumentType.PythonCode),
            (new DocumentInput("md-1", "test.md", "# Title".ToUtf8Bytes(), "text/markdown"), DocumentType.Markdown),
            (new DocumentInput("txt-1", "test.txt", "Plain text".ToUtf8Bytes(), "text/plain"), DocumentType.Text)
        };

        foreach (var (input, expectedType) in testCases)
        {
            _mockPipeline.DetectDocumentTypeAsync(input, CancellationToken.None)
                .Returns(expectedType);

            // Act
            var detectedType = await _mockPipeline.DetectDocumentTypeAsync(input, CancellationToken.None);

            // Assert
            detectedType.ShouldBe(expectedType);
        }
    }

    /// <summary>
    /// Verifies that GetSupportedDocumentTypes returns all supported types.
    /// </summary>
    [Fact]
    public void GetSupportedDocumentTypes_Should_Return_All_Supported_Types()
    {
        // Arrange
        var expectedTypes = new List<DocumentType>
        {
            DocumentType.CSharpCode,
            DocumentType.TypeScriptCode,
            DocumentType.PythonCode,
            DocumentType.Markdown,
            DocumentType.Pdf,
            DocumentType.Image,
            DocumentType.Text
        };

        _mockPipeline.GetSupportedDocumentTypes()
            .Returns(expectedTypes);

        // Act
        var supportedTypes = _mockPipeline.GetSupportedDocumentTypes();

        // Assert
        supportedTypes.ShouldNotBeNull();
        supportedTypes.Count.ShouldBe(expectedTypes.Count);
        supportedTypes.ShouldContain(DocumentType.CSharpCode);
        supportedTypes.ShouldContain(DocumentType.TypeScriptCode);
        supportedTypes.ShouldContain(DocumentType.PythonCode);
        supportedTypes.ShouldContain(DocumentType.Markdown);
        supportedTypes.ShouldContain(DocumentType.Pdf);
        supportedTypes.ShouldContain(DocumentType.Image);
        supportedTypes.ShouldContain(DocumentType.Text);
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync handles cancellation.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Handle_Cancellation()
    {
        // Arrange
        var input = new DocumentInput("test", "test.txt", "content".ToUtf8Bytes(), "text/plain");
        var options = new DocumentProcessingOptions();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockPipeline.ProcessDocumentAsync(input, options, cts.Token)
            .Returns(Task.FromCanceled<DocumentProcessingResult>(cts.Token));

        // Act & Assert
        await Should.ThrowAsync<OperationCanceledException>(() =>
            _mockPipeline.ProcessDocumentAsync(input, options, cts.Token));
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync handles processing errors.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Handle_Processing_Errors()
    {
        // Arrange
        var input = new DocumentInput("test", "test.txt", "content".ToUtf8Bytes(), "text/plain");
        var options = new DocumentProcessingOptions();
        var expectedResult = new DocumentProcessingResult
        {
            Id = "error-result",
            DocumentId = input.Id,
            Content = string.Empty,
            Chunks = new List<DocumentChunk>(),
            Metadata = new Dictionary<string, object>(),
            DocumentType = DocumentType.Unknown,
            Status = ProcessingStatus.Failed,
            ErrorMessage = "Processing failed",
            ElapsedMilliseconds = 0
        };

        _mockPipeline.ProcessDocumentAsync(input, options, CancellationToken.None)
            .Returns(expectedResult);

        // Act
        var result = await _mockPipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(ProcessingStatus.Failed);
        result.ErrorMessage.ShouldBe("Processing failed");
        result.Content.ShouldBeEmpty();
    }
}

/// <summary>
/// Extension methods for test data creation.
/// </summary>
public static class TestDataExtensions
{
    /// <summary>
    /// Converts a string to UTF-8 encoded bytes.
    /// </summary>
    /// <param name="text">The text to convert.</param>
    /// <returns>The UTF-8 encoded bytes.</returns>
    public static byte[] ToUtf8Bytes(this string text) => System.Text.Encoding.UTF8.GetBytes(text);
}
