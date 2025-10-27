using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace IndFusion.SemanticRag.WebAPI.Controllers;

/// <summary>
/// Controller for Semantic RAG operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SemanticRagController : ControllerBase
{
    private readonly IVectorSearchService _vectorSearchService;
    private readonly ISemanticPatternEngine _patternEngine;
    private readonly ICodeAnalysisService _codeAnalysisService;
    private readonly ILogger<SemanticRagController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticRagController"/> class.
    /// </summary>
    /// <param name="vectorSearchService">Vector search service.</param>
    /// <param name="patternEngine">Semantic pattern engine.</param>
    /// <param name="codeAnalysisService">Code analysis service.</param>
    /// <param name="logger">Logger instance.</param>
    public SemanticRagController(
        IVectorSearchService vectorSearchService,
        ISemanticPatternEngine patternEngine,
        ICodeAnalysisService codeAnalysisService,
        ILogger<SemanticRagController> logger)
    {
        _vectorSearchService = vectorSearchService;
        _patternEngine = patternEngine;
        _codeAnalysisService = codeAnalysisService;
        _logger = logger;
    }

    /// <summary>
    /// Searches for similar content using vector embeddings.
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
    /// Analyzes code for semantic pattern violations.
    /// </summary>
    /// <param name="request">Code analysis request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Analysis results.</returns>
    [HttpPost("analyze-code")]
    public async Task<ActionResult<CodeAnalysisResult>> AnalyzeCodeAsync(
        [FromBody] CodeAnalysisRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _codeAnalysisService.AnalyzeCodeAsync(
                request.Code, 
                request.Language, 
                cancellationToken);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during code analysis");
            return StatusCode(500, "Internal server error during code analysis");
        }
    }

    /// <summary>
    /// Analyzes a project for pattern violations.
    /// </summary>
    /// <param name="projectPath">Path to the project.</param>
    /// <param name="patternTypes">Types of patterns to analyze.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pattern violations.</returns>
    [HttpGet("analyze-project")]
    public async Task<ActionResult<IReadOnlyList<PatternViolation>>> AnalyzeProjectAsync(
        [FromQuery] string projectPath,
        [FromQuery] string[]? patternTypes = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var violations = await _patternEngine.AnalyzeProjectAsync(
                projectPath, 
                patternTypes, 
                cancellationToken);

            return Ok(violations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during project analysis");
            return StatusCode(500, "Internal server error during project analysis");
        }
    }

    /// <summary>
    /// Gets pattern guidance for a development context.
    /// </summary>
    /// <param name="context">Development context.</param>
    /// <param name="patternTypes">Types of patterns to get guidance for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Pattern guidance.</returns>
    [HttpGet("guidance")]
    public async Task<ActionResult<PatternGuidance>> GetGuidanceAsync(
        [FromQuery] string context,
        [FromQuery] string[]? patternTypes = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var guidance = await _patternEngine.GetPatternGuidanceAsync(
                context, 
                patternTypes, 
                cancellationToken);

            return Ok(guidance);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pattern guidance");
            return StatusCode(500, "Internal server error getting guidance");
        }
    }
}

/// <summary>
/// Request model for code analysis.
/// </summary>
public record CodeAnalysisRequest
{
    /// <summary>
    /// The code to analyze.
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Programming language of the code.
    /// </summary>
    public required string Language { get; init; }

    /// <summary>
    /// Optional context for analysis.
    /// </summary>
    public string? Context { get; init; }
}
