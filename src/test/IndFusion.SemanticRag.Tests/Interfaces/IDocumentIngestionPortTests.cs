using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IngestionStatusEnum = IndFusion.SemanticRag.Domain.Models.IngestionStatus;

namespace IndFusion.SemanticRag.Tests.Interfaces;

/// <summary>
/// ITDD tests for IDocumentIngestionPort interface contracts.
/// These tests verify that any implementation satisfies the interface contract using mocks.
/// </summary>
public class IDocumentIngestionPortTests
{
    private readonly IDocumentIngestionPort _mockIngestionPort;

    /// <summary>
    /// Initializes a new instance of the IDocumentIngestionPortTests class.
    /// </summary>
    public IDocumentIngestionPortTests()
    {
        _mockIngestionPort = Substitute.For<IDocumentIngestionPort>();
    }

    /// <summary>
    /// Verifies that IngestDocumentAsync returns success result for valid input.
    /// </summary>
    [Fact]
    public async Task IngestDocumentAsync_Should_Return_Success_Result_For_Valid_Input()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "test-doc-1",
            Name: "test.txt",
            Content: "Hello, World!".ToUtf8Bytes(),
            MimeType: "text/plain"
        );
        var options = DocumentIngestionOptions.Default();
        var expectedResult = new DocumentIngestionResult
        {
            Id = "ingestion-1",
            DocumentId = input.Id,
            Status = IngestionStatusEnum.Completed,
            ProcessingResult = new DocumentProcessingResult
            {
                Id = "proc-1",
                DocumentId = input.Id,
                Content = "Hello, World!",
                DocumentType = DocumentType.Text,
                Status = ProcessingStatus.Success,
                ElapsedMilliseconds = 100
            },
            VectorEmbeddings = new List<VectorEmbedding>(),
            ExtractedEntities = new List<KnowledgeEntity>(),
            MappedRelationships = new List<KnowledgeRelationship>(),
            Metadata = new Dictionary<string, object>(),
            StartedAt = DateTimeOffset.UtcNow,
            CompletedAt = DateTimeOffset.UtcNow,
            DurationMs = 150,
            ProgressPercentage = 100
        };

        _mockIngestionPort.IngestDocumentAsync(input, options, CancellationToken.None)
            .Returns(Result<DocumentIngestionResult>.Success(expectedResult));

        // Act
        var result = await _mockIngestionPort.IngestDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Id.ShouldBe(expectedResult.Id);
        result.Value.DocumentId.ShouldBe(expectedResult.DocumentId);
        result.Value.Status.ShouldBe(expectedResult.Status);
        result.Value.ProgressPercentage.ShouldBe(100);
    }

    /// <summary>
    /// Verifies that IngestDocumentAsync returns failure result for invalid input.
    /// </summary>
    [Fact]
    public async Task IngestDocumentAsync_Should_Return_Failure_Result_For_Invalid_Input()
    {
        // Arrange
        var input = new DocumentInput(
            Id: "",
            Name: "",
            Content: Array.Empty<byte>(),
            MimeType: ""
        );
        var options = DocumentIngestionOptions.Default();

        _mockIngestionPort.IngestDocumentAsync(input, options, CancellationToken.None)
            .Returns(Result<DocumentIngestionResult>.WithFailure("Invalid document input"));

        // Act
        var result = await _mockIngestionPort.IngestDocumentAsync(input, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Invalid document input");
    }

    /// <summary>
    /// Verifies that IngestDocumentsAsync processes multiple documents.
    /// </summary>
    [Fact]
    public async Task IngestDocumentsAsync_Should_Process_Multiple_Documents()
    {
        // Arrange
        var inputs = new List<DocumentInput>
        {
            new("doc-1", "test1.txt", "Content 1".ToUtf8Bytes(), "text/plain"),
            new("doc-2", "test2.txt", "Content 2".ToUtf8Bytes(), "text/plain")
        };
        var options = DocumentIngestionOptions.Default();
        var expectedResults = new List<DocumentIngestionResult>
        {
            new() { Id = "ingestion-1", DocumentId = "doc-1", Status = IngestionStatusEnum.Completed, ProgressPercentage = 100 },
            new() { Id = "ingestion-2", DocumentId = "doc-2", Status = IngestionStatusEnum.Completed, ProgressPercentage = 100 }
        };

        _mockIngestionPort.IngestDocumentsAsync(inputs, options, CancellationToken.None)
            .Returns(Result<IReadOnlyList<DocumentIngestionResult>>.Success(expectedResults));

        // Act
        var result = await _mockIngestionPort.IngestDocumentsAsync(inputs, options, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(2);
        result.Value[0].DocumentId.ShouldBe("doc-1");
        result.Value[1].DocumentId.ShouldBe("doc-2");
    }

    /// <summary>
    /// Verifies that IngestRepositoryAsync processes repository successfully.
    /// </summary>
    [Fact]
    public async Task IngestRepositoryAsync_Should_Process_Repository_Successfully()
    {
        // Arrange
        var repositoryPath = "/path/to/repo";
        var config = RepositoryIngestionConfig.Default();
        var expectedResult = new RepositoryIngestionResult(
            ProcessedDocuments: new List<SemanticDocument>
            {
                new("doc-1", "Test Document", "Content", new Dictionary<string, object>(), DateTime.UtcNow, DateTime.UtcNow)
            },
            TotalDocuments: 1,
            ExtractedKnowledge: new List<KnowledgeExtractionResult>(),
            ProcessingTimeMs: 1000,
            Success: true
        );

        _mockIngestionPort.IngestRepositoryAsync(repositoryPath, config, CancellationToken.None)
            .Returns(Result<RepositoryIngestionResult>.Success(expectedResult));

        // Act
        var result = await _mockIngestionPort.IngestRepositoryAsync(repositoryPath, config, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.TotalDocuments.ShouldBe(1);
        result.Value.Success.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that GetIngestionStatusAsync returns current status.
    /// </summary>
    [Fact]
    public async Task GetIngestionStatusAsync_Should_Return_Current_Status()
    {
        // Arrange
        var documentId = "test-doc-1";
        var expectedStatus = new DocumentIngestionStatus
        {
            DocumentId = documentId,
            Status = IngestionStatusEnum.Processing,
            ProgressPercentage = 50,
            CurrentStage = "Processing document",
            StartedAt = DateTimeOffset.UtcNow.AddMinutes(-1),
            Metadata = new Dictionary<string, object>()
        };

        _mockIngestionPort.GetIngestionStatusAsync(documentId, CancellationToken.None)
            .Returns(Result<DocumentIngestionStatus>.Success(expectedStatus));

        // Act
        var result = await _mockIngestionPort.GetIngestionStatusAsync(documentId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.DocumentId.ShouldBe(documentId);
        result.Value.Status.ShouldBe(IngestionStatusEnum.Processing);
        result.Value.ProgressPercentage.ShouldBe(50);
        result.Value.CurrentStage.ShouldBe("Processing document");
    }

    /// <summary>
    /// Verifies that CancelIngestionAsync cancels ongoing ingestion.
    /// </summary>
    [Fact]
    public async Task CancelIngestionAsync_Should_Cancel_Ongoing_Ingestion()
    {
        // Arrange
        var documentId = "test-doc-1";

        _mockIngestionPort.CancelIngestionAsync(documentId, CancellationToken.None)
            .Returns(Result.Success());

        // Act
        var result = await _mockIngestionPort.CancelIngestionAsync(documentId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
    }

    /// <summary>
    /// Verifies that CancelIngestionAsync handles non-existent document.
    /// </summary>
    [Fact]
    public async Task CancelIngestionAsync_Should_Handle_Non_Existent_Document()
    {
        // Arrange
        var documentId = "non-existent-doc";

        _mockIngestionPort.CancelIngestionAsync(documentId, CancellationToken.None)
            .Returns(Result.WithFailure("Document not found"));

        // Act
        var result = await _mockIngestionPort.CancelIngestionAsync(documentId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Document not found");
    }

    /// <summary>
    /// Verifies that IngestDocumentAsync handles cancellation.
    /// </summary>
    [Fact]
    public async Task IngestDocumentAsync_Should_Handle_Cancellation()
    {
        // Arrange
        var input = new DocumentInput("test", "test.txt", "content".ToUtf8Bytes(), "text/plain");
        var options = DocumentIngestionOptions.Default();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        _mockIngestionPort.IngestDocumentAsync(input, options, cts.Token)
            .Returns(Result<DocumentIngestionResult>.WithFailure("Operation was cancelled"));

        // Act
        var result = await _mockIngestionPort.IngestDocumentAsync(input, options, cts.Token);

        // Assert
        result.ShouldNotBeNull();
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Operation was cancelled");
    }

    /// <summary>
    /// Verifies that GetIngestionStatusAsync returns correct status for all states.
    /// </summary>
    /// <param name="expectedStatus">The expected ingestion status.</param>
    /// <param name="expectedProgress">The expected progress percentage.</param>
    [Theory]
    [InlineData(IngestionStatusEnum.Pending, 0)]
    [InlineData(IngestionStatusEnum.Processing, 50)]
    [InlineData(IngestionStatusEnum.Completed, 100)]
    [InlineData(IngestionStatusEnum.Failed, 0)]
    [InlineData(IngestionStatusEnum.Cancelled, 25)]
    public async Task GetIngestionStatusAsync_Should_Return_Correct_Status_For_All_States(
        IngestionStatusEnum expectedStatus, int expectedProgress)
    {
        // Arrange
        var documentId = "test-doc-1";
        var expectedStatusObj = new DocumentIngestionStatus
        {
            DocumentId = documentId,
            Status = expectedStatus,
            ProgressPercentage = expectedProgress,
            CurrentStage = expectedStatus.ToString(),
            StartedAt = DateTimeOffset.UtcNow,
            Metadata = new Dictionary<string, object>()
        };

        _mockIngestionPort.GetIngestionStatusAsync(documentId, CancellationToken.None)
            .Returns(Result<DocumentIngestionStatus>.Success(expectedStatusObj));

        // Act
        var result = await _mockIngestionPort.GetIngestionStatusAsync(documentId, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Status.ShouldBe(expectedStatus);
        result.Value.ProgressPercentage.ShouldBe(expectedProgress);
    }
}
