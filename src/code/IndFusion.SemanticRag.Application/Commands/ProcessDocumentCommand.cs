using IndFusion.SemanticRag.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using IndQuestResults;

namespace IndFusion.SemanticRag.Application.Commands;

/// <summary>
/// Command to process a document.
/// </summary>
public class ProcessDocumentCommand
{
    /// <summary>
    /// Gets or sets the document content.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document type.
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// Handler for the ProcessDocumentCommand.
/// </summary>
public class ProcessDocumentCommandHandler : ICommandHandler<ProcessDocumentCommand>
{
    private readonly ILogger<ProcessDocumentCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the ProcessDocumentCommandHandler class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public ProcessDocumentCommandHandler(ILogger<ProcessDocumentCommandHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<Result> Handle(ProcessDocumentCommand command, CancellationToken cancellationToken = default)
    {
        if (command == null)
        {
            return Result.WithFailure("Command cannot be null");
        }

        if (string.IsNullOrWhiteSpace(command.Content))
        {
            return Result.WithFailure("Document content cannot be null or empty");
        }

        _logger.LogInformation("Processing document of type {DocumentType} with {ContentLength} characters", 
            command.DocumentType, command.Content.Length);

        try
        {
            // Simulate document processing
            await Task.Delay(100, cancellationToken);

            _logger.LogInformation("Document processed successfully");
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document");
            return Result.WithFailure($"Error processing document: {ex.Message}");
        }
    }
}
