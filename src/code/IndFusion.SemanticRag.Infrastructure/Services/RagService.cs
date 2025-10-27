using IndFusion.SemanticRag.Application.Interfaces;
using IndFusion.SemanticRag.Domain.Models;
using Microsoft.Extensions.Logging;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Implementation of RAG service combining vector search with LLM generation.
/// </summary>
public class RagService : IRagService
{
    private readonly IVectorSearchService _vectorSearchService;
    private readonly OllamaClient _ollamaClient;
    private readonly ILogger<RagService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="RagService"/> class.
    /// </summary>
    /// <param name="vectorSearchService">Vector search service.</param>
    /// <param name="ollamaClient">Ollama client for text generation.</param>
    /// <param name="logger">Logger instance.</param>
    public RagService(
        IVectorSearchService vectorSearchService,
        OllamaClient ollamaClient,
        ILogger<RagService> logger)
    {
        _vectorSearchService = vectorSearchService;
        _ollamaClient = ollamaClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<RagResponse> QueryAsync(
        string query, 
        string? context = null, 
        int maxResults = 5, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Processing RAG query: {Query}", query);

            var startTime = DateTime.UtcNow;

            // Step 1: Retrieve relevant documents
            var searchOptions = new VectorSearchOptions
            {
                Limit = maxResults,
                Threshold = 0.7f,
                IncludeMetadata = true
            };

            var searchResults = await _vectorSearchService.SearchSimilarAsync(query, searchOptions, cancellationToken);

            // Step 2: Prepare context from retrieved documents
            var contextDocuments = searchResults.Results
                .Select(result => result.Vector.Content)
                .ToList();

            // Add additional context if provided
            if (!string.IsNullOrEmpty(context))
            {
                contextDocuments.Insert(0, context);
            }

            // Step 3: Generate response using LLM
            var answer = await GenerateResponseAsync(query, contextDocuments, cancellationToken);

            // Step 4: Calculate confidence based on source relevance
            var confidence = CalculateConfidence(searchResults.Results);

            // Step 5: Map sources
            var sources = searchResults.Results.Select(result => new RagSource
            {
                Id = result.Vector.Id,
                Content = result.Vector.Content,
                RelevanceScore = result.Similarity,
                Metadata = new Dictionary<string, object>(result.Vector.Metadata),
                Type = result.Vector.Metadata.GetValueOrDefault("type")?.ToString() ?? "unknown"
            }).ToList();

            var elapsedMs = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

            _logger.LogInformation(
                "RAG query completed in {ElapsedMs}ms with {SourceCount} sources", 
                elapsedMs, 
                sources.Count);

            return new RagResponse
            {
                Answer = answer,
                Sources = sources,
                Confidence = confidence,
                ElapsedMilliseconds = elapsedMs,
                Query = query
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing RAG query: {Query}", query);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<VectorSearchResponse> SearchRelevantDocumentsAsync(
        string query, 
        int maxResults = 10, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching for relevant documents: {Query}", query);

            var searchOptions = new VectorSearchOptions
            {
                Limit = maxResults,
                Threshold = 0.6f,
                IncludeMetadata = true
            };

            var results = await _vectorSearchService.SearchSimilarAsync(query, searchOptions, cancellationToken);

            _logger.LogInformation("Found {Count} relevant documents", results.TotalCount);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching for relevant documents: {Query}", query);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<string> GenerateResponseAsync(
        string query, 
        IReadOnlyList<string> context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating response for query with {ContextCount} context documents", context.Count);

            // Prepare the prompt with context
            var contextText = string.Join("\n\n", context);
            var prompt = $@"Based on the following context, please answer the question. If the answer cannot be found in the context, please say so.

Context:
{contextText}

Question: {query}

Answer:";

            var response = await _ollamaClient.GenerateTextAsync(prompt, cancellationToken);

            _logger.LogDebug("Generated response with length: {Length}", response.Length);

            return response.Trim();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating response for query");
            throw;
        }
    }

    /// <summary>
    /// Calculates confidence score based on source relevance.
    /// </summary>
    /// <param name="results">Search results.</param>
    /// <returns>Confidence score between 0 and 1.</returns>
    private static float CalculateConfidence(IReadOnlyList<VectorSearchResult> results)
    {
        if (!results.Any())
            return 0.0f;

        // Calculate average relevance score
        var averageScore = results.Average(r => r.Similarity);
        
        // Apply confidence curve (higher scores get diminishing returns)
        return Math.Min(1.0f, (float)Math.Pow(averageScore, 0.8));
    }
}


