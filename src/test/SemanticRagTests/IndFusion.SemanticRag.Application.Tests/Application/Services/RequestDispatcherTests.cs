using IndFusion.SemanticRag.Application.Commands;
using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IndFusion.SemanticRag.Application.Tests.Application.Services;

/// <summary>
/// Tests for the RequestDispatcher class.
/// </summary>
public class RequestDispatcherTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RequestDispatcher> _logger;
    private readonly RequestDispatcher _mediator;

    /// <summary>
    /// Initializes a new instance of the RequestDispatcherTests class.
    /// </summary>
    public RequestDispatcherTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _logger = Substitute.For<ILogger<RequestDispatcher>>();
        _mediator = new RequestDispatcher(_serviceProvider, _logger);
    }

    /// <summary>
    /// Send_WithValidCommand_ShouldCallHandler.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact(Timeout = 5000)]
    public async Task Send_WithValidCommand_ShouldCallHandler()
    {
        // Arrange
        var command = new ProcessDocumentCommand
        {
            Content = "Test content",
            DocumentType = "TestType"
        };

        var handler = Substitute.For<ICommandHandler<ProcessDocumentCommand>>();
        _serviceProvider.GetService(typeof(ICommandHandler<ProcessDocumentCommand>)).Returns(handler);

        // Act
        await _mediator.Send(command, TestContext.Current.CancellationToken);

        // Assert
        await handler.Received(1).Handle(command, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Send_WithNullCommand_ShouldReturnFailure.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact(Timeout = 5000)]
    public async Task Send_WithNullCommand_ShouldReturnFailure()
    {
        // Act
        var result = await _mediator.Send<ProcessDocumentCommand>(null!, TestContext.Current.CancellationToken);

        // Assert: RequestDispatcher returns Result<T> instead of throwing exceptions (functional pattern)
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }

    /// <summary>
    /// Send_WithNoHandler_ShouldReturnFailure.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact(Timeout = 5000)]
    public async Task Send_WithNoHandler_ShouldReturnFailure()
    {
        // Arrange
        var command = new ProcessDocumentCommand();

        _serviceProvider.GetService(typeof(ICommandHandler<ProcessDocumentCommand>)).Returns((ICommandHandler<ProcessDocumentCommand>?)null);

        // Act
        var result = await _mediator.Send(command, TestContext.Current.CancellationToken);

        // Assert: RequestDispatcher returns Result<T> instead of throwing exceptions (functional pattern)
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldNotBeNullOrEmpty();
    }
}