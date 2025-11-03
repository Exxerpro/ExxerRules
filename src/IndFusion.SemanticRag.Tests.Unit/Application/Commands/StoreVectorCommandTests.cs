using IndFusion.SemanticRag.Application.Commands.VectorSearch;
using IndFusion.SemanticRag.Domain.Builders;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Tests.Unit.Shared;
using IndQuestResults;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Application.Commands;

/// <summary>
/// Unit tests for StoreVectorCommandHandler.
/// </summary>
public class StoreVectorCommandTests
{
    private readonly IVectorSearchPort _vectorSearchPort;
    private readonly ILogger<StoreVectorCommandHandler> _logger;
    private readonly StoreVectorCommandHandler _handler;

    public StoreVectorCommandTests()
    {
        _vectorSearchPort = Substitute.For<IVectorSearchPort>();
        _logger = Substitute.For<ILogger<StoreVectorCommandHandler>>();
        _handler = new StoreVectorCommandHandler(_vectorSearchPort, _logger);
    }

    [Fact(Timeout = 5000)]
    public async Task Should_StoreVectorSuccessfully_When_VectorIsValid()
    {
        // ✅ Use factory builder with railway pattern
        var vectorResult = VectorEmbeddingBuilder.Build(
            id: "test-id",
            content: "test content",
            embedding: new float[] { 0.1f, 0.2f, 0.3f },
            metadata: new Dictionary<string, object> { ["type"] = "text" },
            createdAt: DateTimeOffset.UtcNow);
        
        // ✅ Validate railway pattern
        vectorResult.IsSuccess.ShouldBeTrue();
        var vector = vectorResult.Value;

        var command = new StoreVectorCommand(vector);
        _vectorSearchPort.IndexAsync(vector, Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success()));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await _vectorSearchPort.Received(1).IndexAsync(vector, Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task Should_ReturnFailure_When_VectorValidationFails()
    {
        // ✅ Use factory builder - should return failure for invalid input
        var invalidVectorResult = VectorEmbeddingBuilder.Build(
            id: "", // Invalid: empty ID
            content: "test content",
            embedding: new float[] { 0.1f, 0.2f, 0.3f },
            metadata: new Dictionary<string, object> { ["type"] = "text" },
            createdAt: DateTimeOffset.UtcNow);
        
        // ✅ Railway pattern - builder returns failure for invalid input
        invalidVectorResult.IsFailure.ShouldBeTrue();
        // For this test, we need an invalid vector, so we'll use a direct constructor
        // But in real code, we should handle the failure result properly
        var invalidVector = new VectorEmbedding(
            "", // Invalid: empty ID
            "test content",
            new float[] { 0.1f, 0.2f, 0.3f },
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        var command = new StoreVectorCommand(invalidVector);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
        await _vectorSearchPort.DidNotReceive().IndexAsync(Arg.Any<VectorEmbedding>(), Arg.Any<CancellationToken>());
    }

    [Fact(Timeout = 5000)]
    public async Task Should_ReturnFailure_When_VectorSearchPortFails()
    {
        // ✅ Use factory builder with railway pattern
        var vectorResult = VectorEmbeddingBuilder.Build(
            id: "test-id",
            content: "test content",
            embedding: new float[] { 0.1f, 0.2f, 0.3f },
            metadata: new Dictionary<string, object> { ["type"] = "text" },
            createdAt: DateTimeOffset.UtcNow);
        vectorResult.IsSuccess.ShouldBeTrue();
        var vector = vectorResult.Value;

        var command = new StoreVectorCommand(vector);
        _vectorSearchPort.IndexAsync(vector, Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Database connection failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public async Task Should_HandleException_When_UnexpectedErrorOccurs()
    {
        // ✅ Use factory builder with railway pattern
        var vectorResult = VectorEmbeddingBuilder.Build(
            id: "test-id",
            content: "test content",
            embedding: new float[] { 0.1f, 0.2f, 0.3f },
            metadata: new Dictionary<string, object> { ["type"] = "text" },
            createdAt: DateTimeOffset.UtcNow);
        vectorResult.IsSuccess.ShouldBeTrue();
        var vector = vectorResult.Value;

        var command = new StoreVectorCommand(vector);
        _vectorSearchPort.IndexAsync(vector, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Result>(new InvalidOperationException("Unexpected error")));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    [Fact(Timeout = 5000)]
    public async Task Should_LogInformation_When_VectorIsStoredSuccessfully()
    {
        // ✅ Use factory builder with railway pattern
        var vectorResult = VectorEmbeddingBuilder.Build(
            id: "test-id",
            content: "test content",
            embedding: new float[] { 0.1f, 0.2f, 0.3f },
            metadata: new Dictionary<string, object> { ["type"] = "text" },
            createdAt: DateTimeOffset.UtcNow);
        vectorResult.IsSuccess.ShouldBeTrue();
        var vector = vectorResult.Value;

        var command = new StoreVectorCommand(vector);
        _vectorSearchPort.IndexAsync(vector, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _logger.Received().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains("Storing vector with ID: test-id")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}