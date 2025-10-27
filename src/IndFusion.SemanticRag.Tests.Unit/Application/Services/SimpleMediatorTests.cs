using IndFusion.SemanticRag.Application.Commands;
using IndFusion.SemanticRag.Application.Services;
using IndFusion.SemanticRag.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace IndFusion.SemanticRag.Tests.Unit.Application.Services;

/// <summary>
/// Tests for the SimpleMediator class.
/// </summary>
public class SimpleMediatorTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SimpleMediator> _logger;
    private readonly SimpleMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the SimpleMediatorTests class.
    /// </summary>
    public SimpleMediatorTests()
    {
        _serviceProvider = Substitute.For<IServiceProvider>();
        _logger = Substitute.For<ILogger<SimpleMediator>>();
        _mediator = new SimpleMediator(_serviceProvider, _logger);
    }

    /// <summary>
    /// Send_WithValidCommand_ShouldCallHandler.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Send_WithValidCommand_ShouldCallHandler()
    {
        // Arrange
        var command = new ProcessDocumentCommand
        {
            Content = "Test content",
            DocumentType = "TestType"
        };

        var handler = Substitute.For<ICommandHandler<ProcessDocumentCommand>>();
        _serviceProvider.GetService<ICommandHandler<ProcessDocumentCommand>>().Returns(handler);

        // Act
        await _mediator.Send(command, TestContext.Current.CancellationToken);

        // Assert
        await handler.Received(1).Handle(command, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Send_WithNullCommand_ShouldThrowArgumentNullException.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Send_WithNullCommand_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send<ProcessDocumentCommand>(null!, TestContext.Current.CancellationToken));
    }

    /// <summary>
    /// Send_WithNoHandler_ShouldThrowInvalidOperationException.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task Send_WithNoHandler_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var command = new ProcessDocumentCommand();
        _serviceProvider.GetService<ICommandHandler<ProcessDocumentCommand>>().Returns((ICommandHandler<ProcessDocumentCommand>?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _mediator.Send(command, TestContext.Current.CancellationToken));
    }
}
