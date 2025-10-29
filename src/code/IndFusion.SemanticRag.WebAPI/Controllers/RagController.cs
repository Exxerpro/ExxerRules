using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace IndFusion.SemanticRag.WebAPI.Controllers;

/// <summary>
/// Controller for RAG (Retrieval-Augmented Generation) operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RagController : ControllerBase
{
    private readonly IRagService _ragService;
    private readonly IVectorSearchService _vectorSearchService;
    private readonly ILogger<RagController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RagController"/> class.
    /// </summary>
    /// <param name="ragService">RAG service.</param>
    /// <param name="vectorSearchService">Vector search service.</param>
    /// <param name="logger">Logger instance.</param>
    public RagController(
        IRagService ragService,
        IVectorSearchService vectorSearchService,
        ILogger<RagController> logger)
    {
        _ragService = ragService;
        _vectorSearchService = vectorSearchService;
        _logger = logger;
    }

    /// <summary>
    /// Performs a RAG query to get an AI-generated answer based on relevant documents.
    /// </summary>
    /// <param name="request">RAG query request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>RAG response with answer and sources.</returns>
    [HttpPost("query")]
    public async Task<ActionResult<RagResponse>> QueryAsync(
        [FromBody] RagQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Query))
            {
                return BadRequest("Query cannot be empty");
            }

            var result = await _ragService.QueryAsync(
                request.Query, 
                request.Context, 
                request.MaxResults, 
                cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RAG query");
            return StatusCode(500, "Internal server error during RAG query");
        }
    }

    /// <summary>
    /// Searches for relevant documents using vector similarity.
    /// </summary>
    /// <param name="query">Search query.</param>
    /// <param name="limit">Maximum number of results.</param>
    /// <param name="threshold">Similarity threshold.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search results.</returns>
    [HttpGet("search")]
    public async Task<ActionResult<VectorSearchResponse>> SearchAsync(
        [FromQuery] string query,
        [FromQuery] int limit = 10,
        [FromQuery] float threshold = 0.7f,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query cannot be empty");
            }

            var options = new VectorSearchOptions
            {
                Limit = limit,
                Threshold = threshold,
                IncludeMetadata = true
            };

            var result = await _vectorSearchService.SearchSimilarAsync(query, options, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during vector search");
            return StatusCode(500, "Internal server error during search");
        }
    }

    /// <summary>
    /// Stores a document in the vector database for future retrieval.
    /// </summary>
    /// <param name="request">Document storage request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Storage result.</returns>
    [HttpPost("store")]
    public async Task<ActionResult<DocumentStorageResult>> StoreDocumentAsync(
        [FromBody] DocumentStorageRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Id and Content are required");
            }

            await _vectorSearchService.StoreDocumentAsync(
                request.Id, 
                request.Content, 
                request.Metadata ?? new Dictionary<string, object>(), 
                cancellationToken);

            return Ok(new DocumentStorageResult
            {
                Id = request.Id,
                Success = true,
                Message = "Document stored successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing document");
            return StatusCode(500, "Internal server error during document storage");
        }
    }
}

/// <summary>
/// Request model for RAG queries.
/// </summary>
public record RagQueryRequest
{
    /// <summary>
    /// The query to process.
    /// </summary>
    public required string Query { get; init; }

    /// <summary>
    /// Additional context for the query.
    /// </summary>
    public string? Context { get; init; }

    /// <summary>
    /// Maximum number of relevant documents to retrieve.
    /// </summary>
    public int MaxResults { get; init; } = 5;
}

/// <summary>
/// Request model for document storage.
/// </summary>
public record DocumentStorageRequest
{
    /// <summary>
    /// Unique identifier for the document.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Document content.
    /// </summary>
    public required string Content { get; init; }

    /// <summary>
    /// Document metadata.
    /// </summary>
    public Dictionary<string, object>? Metadata { get; init; }
}

/// <summary>
/// Result model for document storage operations.
/// </summary>
public record DocumentStorageResult
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Whether the operation was successful.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Result message.
    /// </summary>
    public required string Message { get; init; }
}






