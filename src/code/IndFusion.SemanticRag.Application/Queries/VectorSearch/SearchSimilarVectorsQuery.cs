using IndFusion.SemanticRag.Domain.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Application.Queries.VectorSearch;

/// <summary>
/// Query to search for similar vectors using vector similarity search.
/// </summary>
/// <param name="Query">The vector search query containing search parameters.</param>
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
    /// <returns>A Result containing the search results.</returns>
    public async Task<Result<IReadOnlyList<VectorSearchResult>>> Handle(
        SearchSimilarVectorsQuery request, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching for similar vectors with query: {Query}", request.Query.Query ?? "null");

        // Validate the search query before processing
        var validationResult = request.Query.Validate();
        if (validationResult.IsFailure)
        {
            var errorMessage = validationResult.Error ?? "Validation failed";
            _logger.LogWarning("Search query validation failed: {Error}", errorMessage);
            return Result<IReadOnlyList<VectorSearchResult>>.WithFailure(errorMessage);
        }

        try
        {
            var result = await _vectorSearchPort.SearchSimilarVectorsAsync(request.Query, cancellationToken);
            
            if (result.IsSuccess)
            {
                // Safe to access Value when IsSuccess is true - use null-forgiving operator
                var searchResults = result.Value!;
                _logger.LogInformation("Successfully found {Count} similar vectors for query: {Query}", 
                    searchResults.Count, request.Query.Query ?? "null");
            }
            else
            {
                var errorMessage = result.Error ?? "Unknown error";
                _logger.LogError("Failed to search similar vectors for query: {Query}, Error: {Error}", 
                    request.Query.Query ?? "null", errorMessage);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while searching similar vectors for query: {Query}", 
                request.Query.Query ?? "null");
            return Result<IReadOnlyList<VectorSearchResult>>.WithFailure(
                $"Unexpected error while searching similar vectors: {ex.Message}");
        }
    }
}