using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IndQuestResults;

namespace IndFusion.SemanticRag.Infrastructure.Adapters;

/// <summary>
/// Ollama-based implementation of the embedding service port.
/// </summary>
public class OllamaEmbeddingServiceAdapter : IEmbeddingServicePort
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaEmbeddingServiceAdapter> _logger;
    private readonly OllamaOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaEmbeddingServiceAdapter"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client for Ollama API calls.</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="options">Ollama configuration options.</param>
    public OllamaEmbeddingServiceAdapter(
        HttpClient httpClient,
        ILogger<OllamaEmbeddingServiceAdapter> logger,
        IOptions<OllamaOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public async Task<Result<float[]>> GenerateEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result<float[]>.WithFailure("Text cannot be null or empty");

        var validation = ValidateTextLength(text);
        if (validation.IsFailure)
            return Result<float[]>.WithFailure(validation.Error!);

        try
        {
            _logger.LogInformation("Generating embedding for text of length {TextLength}", text.Length);

            var request = new
            {
                model = _options.EmbeddingModel,
                prompt = text
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_options.BaseUrl}/api/embeddings", content, cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Ollama API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return Result<float[]>.WithFailure($"Ollama API error: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var embeddingResponse = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(responseContent);

            if (embeddingResponse?.Embedding == null || embeddingResponse.Embedding.Length == 0)
            {
                _logger.LogError("Invalid embedding response from Ollama API");
                return Result<float[]>.WithFailure("Invalid embedding response from Ollama API");
            }

            _logger.LogInformation("Successfully generated embedding with dimension {Dimension}", embeddingResponse.Embedding.Length);
            return Result<float[]>.Success(embeddingResponse.Embedding);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while generating embedding");
            return Result<float[]>.WithFailure($"HTTP request failed: {ex.Message}");
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout while generating embedding");
            return Result<float[]>.WithFailure("Timeout while generating embedding");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while generating embedding");
            return Result<float[]>.WithFailure($"Unexpected error: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<float[]>>> GenerateEmbeddingsAsync(
        IReadOnlyList<string> texts, 
        CancellationToken cancellationToken = default)
    {
        if (texts == null || texts.Count == 0)
            return Result<IReadOnlyList<float[]>>.WithFailure("Texts cannot be null or empty");

        var results = new List<float[]>();
        var errors = new List<string>();

        // Process texts in parallel with concurrency limit
        var semaphore = new SemaphoreSlim(10, 10); // Default concurrency limit
        var tasks = texts.Select(async (text, index) =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var result = await GenerateEmbeddingAsync(text, cancellationToken);
                if (result.IsSuccess)
                {
                    lock (results)
                    {
                        if (result.Value != null)
                        {
                            results.Add(result.Value);
                        }
                    }
                }
                else
                {
                    lock (errors)
                    {
                        errors.Add($"Text {index}: {result.Error}");
                    }
                }
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        if (errors.Count > 0)
        {
            _logger.LogWarning("Failed to generate {ErrorCount} embeddings out of {TotalCount}", errors.Count, texts.Count);
            return Result<IReadOnlyList<float[]>>.WithFailure($"Failed to generate {errors.Count} embeddings: {string.Join("; ", errors)}");
        }

        return Result<IReadOnlyList<float[]>>.Success(results.AsReadOnly());
    }

    /// <inheritdoc />
    public async Task<Result<VectorEmbedding>> GenerateEmbeddingWithMetadataAsync(
        string text, 
        Dictionary<string, object> metadata, 
        CancellationToken cancellationToken = default)
    {
        var embeddingResult = await GenerateEmbeddingAsync(text, cancellationToken);
        if (embeddingResult.IsFailure)
            return Result<VectorEmbedding>.WithFailure(embeddingResult.Error!);

        if (embeddingResult.Value == null)
            return Result<VectorEmbedding>.WithFailure("Generated embedding is null");

        var vectorEmbedding = new VectorEmbedding(
            Id: Guid.NewGuid().ToString(),
            Content: text,
            Embedding: embeddingResult.Value,
            Metadata: metadata ?? new Dictionary<string, object>(),
            CreatedAt: DateTimeOffset.UtcNow
        );

        return Result<VectorEmbedding>.Success(vectorEmbedding);
    }

    /// <inheritdoc />
    public int GetEmbeddingDimension()
    {
        return 1536; // Standard embedding dimension for most models
    }

    /// <inheritdoc />
    public int GetMaxTextLength()
    {
        return 8192; // Default max text length
    }

    /// <inheritdoc />
    public Result ValidateTextLength(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result.WithFailure("Text cannot be null or empty");

        const int maxLength = 8192;
        if (text.Length > maxLength)
            return Result.WithFailure($"Text length {text.Length} exceeds maximum allowed length {maxLength}");

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<EmbeddingModelInfo>> GetModelInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_options.BaseUrl}/api/tags", cancellationToken);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError("Failed to get model info: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                return Result<EmbeddingModelInfo>.WithFailure($"Failed to get model info: {response.StatusCode}");
            }

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var tagsResponse = JsonSerializer.Deserialize<OllamaTagsResponse>(responseContent);

            var model = tagsResponse?.Models?.FirstOrDefault(m => m.Name.StartsWith(_options.EmbeddingModel));
            if (model == null)
            {
                return Result<EmbeddingModelInfo>.WithFailure($"Model {_options.EmbeddingModel} not found");
            }

            var modelInfo = new EmbeddingModelInfo(
                ModelName: model.Name,
                Version: model.ModifiedAt.ToString("yyyy-MM-dd"),
                Dimension: 1536,
                MaxTextLength: 8192,
                SupportedLanguages: new[] { "en" } // Ollama models typically support English
            );

            return Result<EmbeddingModelInfo>.Success(modelInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting model info");
            return Result<EmbeddingModelInfo>.WithFailure($"Error getting model info: {ex.Message}");
        }
    }
}

/// <summary>
/// Response model for Ollama embedding API.
/// </summary>
internal record OllamaEmbeddingResponse(
    float[] Embedding);

/// <summary>
/// Response model for Ollama tags API.
/// </summary>
internal record OllamaTagsResponse(
    OllamaModel[] Models);

/// <summary>
/// Model information from Ollama tags API.
/// </summary>
internal record OllamaModel(
    string Name,
    DateTime ModifiedAt,
    long Size);