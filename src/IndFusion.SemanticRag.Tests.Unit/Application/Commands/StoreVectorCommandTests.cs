using IndFusion.SemanticRag.Application.Commands.VectorSearch;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
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

    [Fact]
    public async Task Should_StoreVectorSuccessfully_When_VectorIsValid()
    {
        // Arrange
        var vector = new VectorEmbedding(
            "test-id",
            "test content",
            new float[] { 0.1f, 0.2f, 0.3f },
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        var command = new StoreVectorCommand(vector);
        _vectorSearchPort.StoreVectorAsync(vector, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        await _vectorSearchPort.Received(1).StoreVectorAsync(vector, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ReturnFailure_When_VectorValidationFails()
    {
        // Arrange
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
        result.Error.ShouldBe("Vector ID cannot be null or empty");
        await _vectorSearchPort.DidNotReceive().StoreVectorAsync(Arg.Any<VectorEmbedding>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ReturnFailure_When_VectorSearchPortFails()
    {
        // Arrange
        var vector = new VectorEmbedding(
            "test-id",
            "test content",
            new float[] { 0.1f, 0.2f, 0.3f },
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        var command = new StoreVectorCommand(vector);
        _vectorSearchPort.StoreVectorAsync(vector, Arg.Any<CancellationToken>())
            .Returns(Result.WithFailure("Database connection failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe("Database connection failed");
    }

    [Fact]
    public async Task Should_HandleException_When_UnexpectedErrorOccurs()
    {
        // Arrange
        var vector = new VectorEmbedding(
            "test-id",
            "test content",
            new float[] { 0.1f, 0.2f, 0.3f },
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        var command = new StoreVectorCommand(vector);
        _vectorSearchPort.StoreVectorAsync(vector, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Result>(new InvalidOperationException("Unexpected error")));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNull();
        result.Error.ShouldContain("Unexpected error while storing vector");
        result.Error.ShouldContain("Unexpected error");
    }

    [Fact]
    public async Task Should_LogInformation_When_VectorIsStoredSuccessfully()
    {
        // Arrange
        var vector = new VectorEmbedding(
            "test-id",
            "test content",
            new float[] { 0.1f, 0.2f, 0.3f },
            new Dictionary<string, object> { ["type"] = "text" },
            DateTimeOffset.UtcNow);

        var command = new StoreVectorCommand(vector);
        _vectorSearchPort.StoreVectorAsync(vector, Arg.Any<CancellationToken>())
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