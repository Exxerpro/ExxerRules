using IndFusion.SemanticRag.Domain.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using Microsoft.Extensions.Logging;
using IndQuestResults;

namespace IndFusion.SemanticRag.Application.Queries.VectorSearch;

/// <summary>
/// Query to search for similar vectors based on a search query.
/// </summary>
/// <param name="Query">The vector search query containing embedding and parameters.</param>
public record SearchSimilarVectorsQuery(VectorSearchQuery Query);

/// <summary>
/// Query handler for searching similar vectors.
/// </summary>
public class SearchSimilarVectorsQueryHandler : IQueryHandler<SearchSimilarVectorsQuery, IReadOnlyList<VectorSearchResult>>
{
    private readonly IVectorSearchPort _vectorSearchPort;
    private readonly ILogger<SearchSimilarVectorsQueryHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the SearchSimilarVectorsQueryHandler class.
    /// </summary>
    /// <param name="vectorSearchPort">The vector search port for database operations.</param>
    /// <param name="logger">The logger for this handler.</param>
    public SearchSimilarVectorsQueryHandler(
        IVectorSearchPort vectorSearchPort,
        ILogger<SearchSimilarVectorsQueryHandler> logger)
    {
        _vectorSearchPort = vectorSearchPort ?? throw new ArgumentNullException(nameof(vectorSearchPort));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Handles the search similar vectors query.
    /// </summary>
    /// <param name="request">The search similar vectors query.</param>
    /// <param name="cancellationToken">Cancellation token for the operation.</param>
    /// <returns>A Result containing the search results or an error.</returns>
    public async Task<Result<IReadOnlyList<VectorSearchResult>>> Handle(SearchSimilarVectorsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching for similar vectors with query: {Query}", request.Query.Query);

        // Validate the search query
        var validationResult = request.Query.Validate();
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Search query validation failed: {Error}", validationResult.Error);
            return Result<IReadOnlyList<VectorSearchResult>>.WithFailure(validationResult.Error!);
        }

        try
        {
            var result = await _vectorSearchPort.SearchSimilarVectorsAsync(request.Query, cancellationToken);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Found {Count} similar vectors for query: {Query}", 
                    result.Value?.Count ?? 0, request.Query.Query);
            }
            else
            {
                _logger.LogError("Failed to search similar vectors for query: {Query}, Error: {Error}", 
                    request.Query.Query, result.Error);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while searching similar vectors for query: {Query}", 
                request.Query.Query);
            return Result<IReadOnlyList<VectorSearchResult>>.WithFailure($"Unexpected error while searching vectors: {ex.Message}");
        }
    }
}