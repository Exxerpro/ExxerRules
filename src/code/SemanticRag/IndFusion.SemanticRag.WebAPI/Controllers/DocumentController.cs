using IndFusion.SemanticRag.Application.Commands;
using IndFusion.SemanticRag.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IndFusion.SemanticRag.WebAPI.Controllers;

/// <summary>
/// Controller for document processing and ingestion operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DocumentController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator for handling commands and queries.</param>
    /// <param name="logger">Logger instance.</param>
    public DocumentController(IMediator mediator, ILogger<DocumentController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Processes a document and extracts text content.
    /// </summary>
    /// <param name="request">Document processing request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Processing result.</returns>
    [HttpPost("process")]
    public async Task<ActionResult<string>> ProcessDocumentAsync(
        [FromBody] DocumentProcessingRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                return BadRequest("Document content is required");
            }

            var command = new ProcessDocumentCommand
            {
                Content = request.Content,
                DocumentType = request.MimeType ?? "text/plain",
                Metadata = new Dictionary<string, object>
                {
                    ["Id"] = request.Id ?? Guid.NewGuid().ToString(),
                    ["Name"] = request.Name ?? "Unknown",
                    ["FilePath"] = request.FilePath ?? string.Empty,
                    ["MimeType"] = request.MimeType ?? "text/plain"
                }
            };

            await _mediator.Send(command, cancellationToken);

            return Ok("Document processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing document");
            return StatusCode(500, "Internal server error during document processing");
        }
    }

    /// <summary>
    /// Gets supported document types.
    /// </summary>
    /// <returns>List of supported document types.</returns>
    [HttpGet("supported-types")]
    public ActionResult<IReadOnlyList<string>> GetSupportedDocumentTypes()
    {
        try
        {
            var types = new List<string>
            {
                "text/plain",
                "text/markdown",
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            };

            return Ok(types);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting supported document types");
            return StatusCode(500, "Internal server error getting supported types");
        }
    }
}