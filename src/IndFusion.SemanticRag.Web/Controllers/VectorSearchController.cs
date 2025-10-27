using IndFusion.SemanticRag.Application.Commands.VectorSearch;
using IndFusion.SemanticRag.Application.Queries.VectorSearch;
using IndFusion.SemanticRag.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IndFusion.SemanticRag.Web.Controllers;

/// <summary>
/// Controller for vector search operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class VectorSearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VectorSearchController> _logger;

    /// <summary>
    /// Initializes a new instance of the VectorSearchController class.
    /// </summary>
    /// <param name="mediator">The mediator for handling commands and queries.</param>
    /// <param name="logger">The logger for this controller.</param>
    public VectorSearchController(IMediator mediator, ILogger<VectorSearchController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Stores a vector embedding in the vector database.
    /// </summary>
    /// <param name="vector">The vector embedding to store.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result indicating success or failure.</returns>
    [HttpPost("store")]
    public async Task<IActionResult> StoreVectorAsync(
        [FromBody] VectorEmbedding vector,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Received request to store vector with ID: {VectorId}", vector.Id);
            
            var command = new StoreVectorCommand(vector);
            var result = await _mediator.Send(command, cancellationToken);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Successfully stored vector with ID: {VectorId}", vector.Id);
                return Ok(new { success = true, message = "Vector stored successfully" });
            }
            
            _logger.LogWarning("Failed to store vector with ID: {VectorId}, Error: {Error}", 
                vector.Id, result.Error);
            return BadRequest(new { success = false, error = result.Error });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while storing vector with ID: {VectorId}", vector.Id);
            return StatusCode(500, new { success = false, error = "Internal server error" });
        }
    }

    /// <summary>
    /// Searches for similar vectors using the provided query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the search results.</returns>
    [HttpPost("search")]
    public async Task<IActionResult> SearchSimilarVectorsAsync(
        [FromBody] VectorSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Received request to search similar vectors for query: {Query}", query.Query);
            
            var searchQuery = new SearchSimilarVectorsQuery(query);
            var result = await _mediator.Send(searchQuery, cancellationToken);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Found {Count} similar vectors for query: {Query}", 
                    result.Value.Count, query.Query);
                return Ok(new { success = true, results = result.Value });
            }
            
            _logger.LogWarning("Failed to search similar vectors for query: {Query}, Error: {Error}", 
                query.Query, result.Error);
            return BadRequest(new { success = false, error = result.Error });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while searching similar vectors for query: {Query}", query.Query);
            return StatusCode(500, new { success = false, error = "Internal server error" });
        }
    }

    /// <summary>
    /// Gets a vector by its ID.
    /// </summary>
    /// <param name="id">The vector ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Result containing the vector or failure.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetVectorByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Received request to get vector with ID: {VectorId}", id);
            
            // This would need a separate query handler
            return NotFound(new { success = false, error = "Get vector by ID not implemented yet" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting vector with ID: {VectorId}", id);
            return StatusCode(500, new { success = false, error = "Internal server error" });
        }
    }
}