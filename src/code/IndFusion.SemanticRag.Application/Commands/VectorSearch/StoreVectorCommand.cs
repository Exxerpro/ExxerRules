using IndFusion.SemanticRag.Domain.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Application.Commands.VectorSearch;

/// <summary>
/// Command to store a vector embedding in the vector database.
/// </summary>
/// <param name="Vector">The vector embedding to store.</param>
public record StoreVectorCommand(VectorEmbedding Vector);

/// <summary>
/// Command handler for storing vector embeddings.
/// </summary>
public class StoreVectorCommandHandler : ICommandHandler<StoreVectorCommand>
{
    private readonly IVectorSearchPort _vectorSearchPort;
    private readonly ILogger<StoreVectorCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the StoreVectorCommandHandler class.
    /// </summary>
    /// <param name="vectorSearchPort">The vector search port for database operations.</param>
    /// <param name="logger">The logger for this handler.</param>
    public StoreVectorCommandHandler(
        IVectorSearchPort vectorSearchPort,
        ILogger<StoreVectorCommandHandler> logger)
    {
        _vectorSearchPort = vectorSearchPort ?? throw new ArgumentNullException(nameof(vectorSearchPort));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the store vector command.
    /// </summary>
    /// <param name="request">The store vector command.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result indicating success or failure.</returns>
    public async Task<Result> Handle(StoreVectorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Storing vector with ID: {VectorId}", request.Vector.Id);

        // Validate the vector before storing
        var validationResult = request.Vector.Validate();
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Vector validation failed: {Error}", validationResult.Error);
            return validationResult;
        }

        try
        {
            var result = await _vectorSearchPort.StoreVectorAsync(request.Vector, cancellationToken);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully stored vector with ID: {VectorId}", request.Vector.Id);
            }
            else
            {
                _logger.LogError("Failed to store vector with ID: {VectorId}, Error: {Error}", 
                    request.Vector.Id, result.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while storing vector with ID: {VectorId}", request.Vector.Id);
            return Result.WithFailure($"Unexpected error while storing vector: {ex.Message}");
        }
    }
}



