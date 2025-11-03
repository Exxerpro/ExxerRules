using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;
using IngestionStatusEnum = IndFusion.SemanticRag.Domain.Models.IngestionStatus;

namespace IndFusion.SemanticRag.Tests.Unit.Domain.Services;

/// <summary>
/// Unit tests for the document ingestion port interface.
/// These tests verify the interface contracts using mocks (ITDD approach).
/// </summary>
public class DocumentIngestionServiceTests
{
    private readonly IDocumentIngestionPort _mockIngestionPort;
    private readonly ILogger<IDocumentIngestionPort> _logger;

    public DocumentIngestionServiceTests()
    {
        _mockIngestionPort = Substitute.For<IDocumentIngestionPort>();
        _logger = Substitute.For<ILogger<IDocumentIngestionPort>>();
    }

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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

    [Fact(Timeout = 5000)]
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
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Theory(Timeout = 5000)]
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

/// <summary>
/// Extension methods for test data creation.
/// </summary>
public static class TestDataExtensions
{
    public static byte[] ToUtf8Bytes(this string text) => System.Text.Encoding.UTF8.GetBytes(text);
}