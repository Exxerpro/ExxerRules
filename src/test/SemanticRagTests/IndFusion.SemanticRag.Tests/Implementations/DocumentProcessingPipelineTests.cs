using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Tests.Implementations;

/// <summary>
/// TDD tests for DocumentProcessingPipeline implementation.
/// These tests verify that the real implementation satisfies the interface contract (LSP compliance).
/// </summary>
public class DocumentProcessingPipelineTests : IDisposable
{
    private readonly ILogger<DocumentProcessingPipeline> _logger;
    private readonly IOcrService _mockOcrService;
    private readonly DocumentProcessingPipeline _pipeline;

    /// <summary>
    /// Initializes a new instance of the DocumentProcessingPipelineTests class.
    /// </summary>
    public DocumentProcessingPipelineTests()
    {
        _logger = Substitute.For<ILogger<DocumentProcessingPipeline>>();
        _mockOcrService = Substitute.For<IOcrService>();
        _pipeline = new DocumentProcessingPipeline(_logger, _mockOcrService);
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync processes text documents successfully.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Process_Text_Document_Successfully()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-text-1",
            Name: "test.txt",
            Content: "Hello, World! This is a test document.".ToUtf8Bytes(),
            MimeType: "text/plain"
        );
        var options = new DocumentProcessingOptions();

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBeNullOrEmpty();
        result.DocumentId.ShouldBe(input.Id);
        result.Content.ShouldBe("Hello, World! This is a test document.");
        result.DocumentType.ShouldBe(DocumentType.Text);
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.ElapsedMilliseconds.ShouldBeGreaterThan(0);
        result.Chunks.ShouldNotBeNull();
        result.Metadata.ShouldNotBeNull();
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync detects C# code correctly.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Detect_CSharp_Code_Correctly()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-cs-1",
            Name: "test.cs",
            Content: """
                using System;
                
                public class TestClass
                {
                    public void TestMethod()
                    {
                        Console.WriteLine("Hello, World!");
                    }
                }
                """.ToUtf8Bytes(),
            MimeType: "text/x-csharp"
        );
        var options = new DocumentProcessingOptions();

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.DocumentType.ShouldBe(DocumentType.CSharpCode);
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.Content.ShouldContain("public class TestClass");
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync detects TypeScript code correctly.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Detect_TypeScript_Code_Correctly()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-ts-1",
            Name: "test.ts",
            Content: """
                interface TestInterface {
                    name: string;
                    age: number;
                }
                
                const testObject: TestInterface = {
                    name: "John",
                    age: 30
                };
                """.ToUtf8Bytes(),
            MimeType: "text/typescript"
        );
        var options = new DocumentProcessingOptions();

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.DocumentType.ShouldBe(DocumentType.TypeScriptCode);
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.Content.ShouldContain("interface TestInterface");
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync detects Python code correctly.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Detect_Python_Code_Correctly()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-py-1",
            Name: "test.py",
            Content: """
                def hello_world():
                    print("Hello, World!")
                
                class TestClass:
                    def __init__(self):
                        self.name = "test"
                """.ToUtf8Bytes(),
            MimeType: "text/x-python"
        );
        var options = new DocumentProcessingOptions();

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.DocumentType.ShouldBe(DocumentType.PythonCode);
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.Content.ShouldContain("def hello_world()");
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync detects Markdown correctly.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Detect_Markdown_Correctly()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-md-1",
            Name: "test.md",
            Content: """
                # Test Document
                
                This is a **markdown** document with some *formatting*.
                
                ## Subsection
                
                - Item 1
                - Item 2
                - Item 3
                """.ToUtf8Bytes(),
            MimeType: "text/markdown"
        );
        var options = new DocumentProcessingOptions();

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.DocumentType.ShouldBe(DocumentType.Markdown);
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.Content.ShouldContain("# Test Document");
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
            new("doc-2", "test2.txt", "Content 2".ToUtf8Bytes(), "text/plain"),
            new("doc-3", "test3.txt", "Content 3".ToUtf8Bytes(), "text/plain")
        };
        var options = new DocumentProcessingOptions();

        // Act
        var results = await _pipeline.ProcessDocumentsAsync(inputs, options, CancellationToken.None);

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(3);
        results[0].DocumentId.ShouldBe("doc-1");
        results[1].DocumentId.ShouldBe("doc-2");
        results[2].DocumentId.ShouldBe("doc-3");
        results.All(r => r.Status == ProcessingStatus.Success).ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that DetectDocumentTypeAsync detects document types by MIME type.
    /// </summary>
    [Fact]
    public async Task DetectDocumentTypeAsync_Should_Detect_By_MimeType()
    {
        // Arrange
        var testCases = new[]
        {
            (new DocumentInput("test1", "test.txt", "content".ToUtf8Bytes(), "text/plain"), DocumentType.Text),
            (new DocumentInput("test2", "test.md", "content".ToUtf8Bytes(), "text/markdown"), DocumentType.Markdown),
            (new DocumentInput("test3", "test.pdf", "content".ToUtf8Bytes(), "application/pdf"), DocumentType.Pdf),
            (new DocumentInput("test4", "test.png", "content".ToUtf8Bytes(), "image/png"), DocumentType.Image),
            (new DocumentInput("test5", "test.cs", "content".ToUtf8Bytes(), "text/x-csharp"), DocumentType.CSharpCode),
            (new DocumentInput("test6", "test.ts", "content".ToUtf8Bytes(), "text/typescript"), DocumentType.TypeScriptCode),
            (new DocumentInput("test7", "test.py", "content".ToUtf8Bytes(), "text/x-python"), DocumentType.PythonCode)
        };

        foreach (var (input, expectedType) in testCases)
        {
            // Act
            var detectedType = await _pipeline.DetectDocumentTypeAsync(input, CancellationToken.None);

            // Assert
            detectedType.ShouldBe(expectedType, $"Failed for MIME type: {input.MimeType}");
        }
    }

    /// <summary>
    /// Verifies that DetectDocumentTypeAsync detects document types by file extension.
    /// </summary>
    [Fact]
    public async Task DetectDocumentTypeAsync_Should_Detect_By_File_Extension()
    {
        // Arrange
        var testCases = new[]
        {
            (new DocumentInput("test1", "test.txt", "content".ToUtf8Bytes(), "text/plain", "test.txt"), DocumentType.Text),
            (new DocumentInput("test2", "test.md", "content".ToUtf8Bytes(), "text/plain", "test.md"), DocumentType.Markdown),
            (new DocumentInput("test3", "test.pdf", "content".ToUtf8Bytes(), "text/plain", "test.pdf"), DocumentType.Pdf),
            (new DocumentInput("test4", "test.png", "content".ToUtf8Bytes(), "text/plain", "test.png"), DocumentType.Image),
            (new DocumentInput("test5", "test.cs", "content".ToUtf8Bytes(), "text/plain", "test.cs"), DocumentType.CSharpCode),
            (new DocumentInput("test6", "test.ts", "content".ToUtf8Bytes(), "text/plain", "test.ts"), DocumentType.TypeScriptCode),
            (new DocumentInput("test7", "test.py", "content".ToUtf8Bytes(), "text/plain", "test.py"), DocumentType.PythonCode)
        };

        foreach (var (input, expectedType) in testCases)
        {
            // Act
            var detectedType = await _pipeline.DetectDocumentTypeAsync(input, CancellationToken.None);

            // Assert
            detectedType.ShouldBe(expectedType, $"Failed for file extension: {input.FilePath}");
        }
    }

    /// <summary>
    /// Verifies that DetectDocumentTypeAsync detects document types by content analysis.
    /// </summary>
    [Fact]
    public async Task DetectDocumentTypeAsync_Should_Detect_By_Content_Analysis()
    {
        // Arrange
        var testCases = new[]
        {
            (new DocumentInput("test1", "unknown", "public class Test {}".ToUtf8Bytes(), "text/plain"), DocumentType.CSharpCode),
            (new DocumentInput("test2", "unknown", "const x = 1;".ToUtf8Bytes(), "text/plain"), DocumentType.TypeScriptCode),
            (new DocumentInput("test3", "unknown", "def hello(): pass".ToUtf8Bytes(), "text/plain"), DocumentType.PythonCode),
            (new DocumentInput("test4", "unknown", "Plain text content".ToUtf8Bytes(), "text/plain"), DocumentType.Text)
        };

        foreach (var (input, expectedType) in testCases)
        {
            // Act
            var detectedType = await _pipeline.DetectDocumentTypeAsync(input, CancellationToken.None);

            // Assert
            detectedType.ShouldBe(expectedType, $"Failed for content analysis: {System.Text.Encoding.UTF8.GetString(input.Content)}");
        }
    }

    /// <summary>
    /// Verifies that GetSupportedDocumentTypes returns all supported types.
    /// </summary>
    [Fact]
    public void GetSupportedDocumentTypes_Should_Return_All_Supported_Types()
    {
        // Act
        var supportedTypes = _pipeline.GetSupportedDocumentTypes();

        // Assert
        supportedTypes.ShouldNotBeNull();
        supportedTypes.Count.ShouldBe(7);
        supportedTypes.ShouldContain(DocumentType.CSharpCode);
        supportedTypes.ShouldContain(DocumentType.TypeScriptCode);
        supportedTypes.ShouldContain(DocumentType.PythonCode);
        supportedTypes.ShouldContain(DocumentType.Markdown);
        supportedTypes.ShouldContain(DocumentType.Pdf);
        supportedTypes.ShouldContain(DocumentType.Image);
        supportedTypes.ShouldContain(DocumentType.Text);
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync creates chunks with fixed size strategy.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Create_Chunks_With_Fixed_Size_Strategy()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-chunks-1",
            Name: "test.txt",
            Content: "This is a long document that should be split into multiple chunks for better processing and analysis.".ToUtf8Bytes(),
            MimeType: "text/plain"
        );
        var options = new DocumentProcessingOptions(
            EnableChunking: true,
            ChunkingStrategy: ChunkingStrategy.FixedSize,
            MaxChunkSize: 20
        );

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.Chunks.ShouldNotBeNull();
        result.Chunks.Count.ShouldBeGreaterThan(1);
        result.Chunks.All(c => c.DocumentId == input.Id).ShouldBeTrue();
        result.Chunks.All(c => !string.IsNullOrEmpty(c.Content)).ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync creates chunks with paragraph strategy.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Create_Chunks_With_Paragraph_Strategy()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-chunks-2",
            Name: "test.txt",
            Content: """
                First paragraph with some content.

                Second paragraph with different content.

                Third paragraph with more content.
                """.ToUtf8Bytes(),
            MimeType: "text/plain"
        );
        var options = new DocumentProcessingOptions(
            EnableChunking: true,
            ChunkingStrategy: ChunkingStrategy.ParagraphBased,
            MaxChunkSize: 100
        );

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.Chunks.ShouldNotBeNull();
        result.Chunks.Count.ShouldBeGreaterThanOrEqualTo(3); // At least 3 paragraphs
        result.Chunks.All(c => c.DocumentId == input.Id).ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync handles cancellation.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Handle_Cancellation()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-cancel-1",
            Name: "test.txt",
            Content: "Content to process".ToUtf8Bytes(),
            MimeType: "text/plain"
        );
        var options = new DocumentProcessingOptions();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, cts.Token);

        // Assert: After functional refactoring, cancellation is caught and returns failed status
        // Since ProcessDocumentAsync catches all exceptions including OperationCanceledException,
        // it returns DocumentProcessingResult with Status = Failed
        result.Status.ShouldBe(ProcessingStatus.Failed);
        result.ErrorMessage.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync extracts metadata when enabled.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Extract_Metadata_When_Enabled()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-metadata-1",
            Name: "test.txt",
            Content: "Test content".ToUtf8Bytes(),
            MimeType: "text/plain",
            FilePath: "/path/to/test.txt",
            Metadata: new Dictionary<string, object> { ["custom"] = "value" }
        );
        var options = new DocumentProcessingOptions(ExtractMetadata: true);

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.Metadata.ShouldNotBeNull();
        result.Metadata.ShouldContainKey("document_id");
        result.Metadata.ShouldContainKey("document_name");
        result.Metadata.ShouldContainKey("document_type");
        result.Metadata.ShouldContainKey("file_size");
        result.Metadata.ShouldContainKey("processed_at");
        result.Metadata.ShouldContainKey("file_path");
        result.Metadata.ShouldContainKey("mime_type");
        result.Metadata.ShouldContainKey("custom");
    }

    /// <summary>
    /// Verifies that ProcessDocumentAsync does not extract metadata when disabled.
    /// </summary>
    [Fact]
    public async Task ProcessDocumentAsync_Should_Not_Extract_Metadata_When_Disabled()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-metadata-2",
            Name: "test.txt",
            Content: "Test content".ToUtf8Bytes(),
            MimeType: "text/plain"
        );
        var options = new DocumentProcessingOptions(ExtractMetadata: false);

        // Act
        var result = await _pipeline.ProcessDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Status.ShouldBe(ProcessingStatus.Success);
        result.Metadata.ShouldBeNull();
    }

    /// <summary>
    /// Disposes of the test resources.
    /// </summary>
    public void Dispose()
    {
        _pipeline?.Dispose();
    }
}
