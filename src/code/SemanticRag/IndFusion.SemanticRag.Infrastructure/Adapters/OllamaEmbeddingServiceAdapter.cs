using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Errors;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Domain.Ports;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndQuestResults;
using IndQuestResults.Operations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static IndFusion.SemanticRag.Domain.Errors.ResultExtensionsWithErrorCodes;

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
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<float[]>(ErrorCodes.OperationCancelled);
        }

        // Early validation to avoid NullReferenceException
        if (string.IsNullOrWhiteSpace(text))
        {
            return Result<float[]>.WithFailure(ErrorCodes.ParameterNullOrWhitespace);
        }

        try
        {
            var validationResult = ValidateTextLength(text);
            if (validationResult.IsFailure)
            {
                return Result<float[]>.WithFailure(validationResult.Errors);
            }

            return await Task.FromResult(Result<string>.Success(text))
                .ThenAsync(
                    async textValue =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<float[]>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        _logger.LogInformation("Generating embedding for text of length {TextLength}", textValue.Length);

                        var request = new
                        {
                            model = _options.EmbeddingModel,
                            prompt = textValue
                        };

                        var json = JsonSerializer.Serialize(request);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await _httpClient.PostAsync($"{_options.BaseUrl}/api/embeddings", content, cancellationToken);

                        if (!response.IsSuccessStatusCode)
                        {
                            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                            _logger.LogError("Ollama API error: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                            return Result<float[]>.WithFailure(ErrorCodes.EmbeddingServiceError);
                        }

                        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                        // Diagnostic logging for debugging response structure
                        _logger.LogDebug("Ollama API response content (first 500 chars): {ResponseContent}",
                            responseContent.Length > 500 ? responseContent.Substring(0, 500) : responseContent);

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        var embeddingResponse = JsonSerializer.Deserialize<OllamaEmbeddingResponse>(responseContent, options);

                        if (embeddingResponse == null)
                        {
                            _logger.LogError("Failed to deserialize Ollama API response. Response content: {ResponseContent}", responseContent);
                            return Result<float[]>.WithFailure(ErrorCodes.EmbeddingGenerationFailed);
                        }

                        if (embeddingResponse.Embedding == null || embeddingResponse.Embedding.Length == 0)
                        {
                            _logger.LogError("Invalid embedding response from Ollama API - embedding is null or empty. Response: {ResponseContent}", responseContent);
                            return Result<float[]>.WithFailure(ErrorCodes.EmbeddingGenerationFailed);
                        }

                        _logger.LogInformation("Successfully generated embedding with dimension {Dimension}", embeddingResponse.Embedding.Length);
                        return Result<float[]>.Success(embeddingResponse.Embedding);
                    });
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException || ex.CancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Timeout or cancellation while generating embedding");
            return Cancelled<float[]>(ErrorCodes.OperationCancelled);
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Task was cancelled while generating embedding");
            return Cancelled<float[]>(ErrorCodes.OperationCancelled);
        }
        catch (OperationCanceledException)
        {
            return Cancelled<float[]>(ErrorCodes.OperationCancelled);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while generating embedding");
            return WithFailure<float[]>(ErrorCodes.EmbeddingServiceError, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while generating embedding");
            return WithFailure<float[]>(ErrorCodes.EmbeddingServiceError, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<float[]>>> GenerateEmbeddingsAsync(
        IReadOnlyList<string> texts,
        CancellationToken cancellationToken = default)
    {
        // Early validation to avoid NullReferenceException
        if (texts == null || texts.Count == 0)
        {
            return Result<IReadOnlyList<float[]>>.WithFailure(ErrorCodes.CollectionEmpty);
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<IReadOnlyList<float[]>>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<IReadOnlyList<string>>.Success(texts)
                .Ensure(
                    ts => ts != null && ts.Count > 0,
                    ErrorCodes.CollectionEmpty))
                .ThenAsync(
                    async textsList =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<IReadOnlyList<float[]>>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        var results = new List<float[]>();
                        var errors = new List<string>();

                        // Process texts in parallel with concurrency limit
                        var semaphore = new SemaphoreSlim(10, 10); // Default concurrency limit
                        var tasks = textsList.Select(async (text, index) =>
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
                            _logger.LogWarning("Failed to generate {ErrorCount} embeddings out of {TotalCount}", errors.Count, textsList.Count);
                            return Result<IReadOnlyList<float[]>>.WithFailure(ErrorCodes.EmbeddingGenerationFailed);
                        }

                        return Result<IReadOnlyList<float[]>>.Success(results.AsReadOnly());
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<IReadOnlyList<float[]>>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate embeddings");
            return WithFailure<IReadOnlyList<float[]>>(ErrorCodes.EmbeddingGenerationFailed, ex);
        }
    }

    /// <inheritdoc />
    public async Task<Result<VectorEmbedding>> GenerateEmbeddingWithMetadataAsync(
        string text,
        Dictionary<string, object> metadata,
        CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<VectorEmbedding>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await GenerateEmbeddingAsync(text, cancellationToken)
                .ThenAsync(
                    async embedding =>
                    {
                        if (embedding == null)
                        {
                            return Result<VectorEmbedding>.WithFailure(ErrorCodes.VectorEmbeddingRequired);
                        }

                        var vectorEmbedding = new VectorEmbedding(
                            Id: Guid.NewGuid().ToString(),
                            Content: text,
                            Embedding: embedding,
                            Metadata: metadata ?? [],
                            CreatedAt: DateTimeOffset.UtcNow
                        );

                        return Result<VectorEmbedding>.Success(vectorEmbedding);
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<VectorEmbedding>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate embedding with metadata");
            return WithFailure<VectorEmbedding>(ErrorCodes.EmbeddingGenerationFailed, ex);
        }
    }

    /// <inheritdoc />
    public int GetEmbeddingDimension()
    {
        return _options.EmbeddingDimension;
    }

    /// <inheritdoc />
    public int GetMaxTextLength()
    {
        return _options.MaxTextLength;
    }

    /// <inheritdoc />
    public Result ValidateTextLength(string text)
    {
        var result = Result<string>.Success(text)
            .Ensure(
                t => !string.IsNullOrWhiteSpace(t),
                ErrorCodes.ParameterNullOrWhitespace)
            .Ensure(
                t => t.Length <= GetMaxTextLength(),
                ErrorCodes.ValueOutOfRange);

        return result.IsSuccess
            ? Result.Success()
            : Result.WithFailure(result.Errors);
    }

    /// <inheritdoc />
    public async Task<Result<EmbeddingModelInfo>> GetModelInfoAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Cancelled<EmbeddingModelInfo>(ErrorCodes.OperationCancelled);
        }

        try
        {
            return await Task.FromResult(Result<string>.Success(_options.BaseUrl))
                .ThenAsync(
                    async baseUrl =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return Result<EmbeddingModelInfo>.WithFailure(ErrorCodes.OperationCancelled);
                        }

                        var response = await _httpClient.GetAsync($"{baseUrl}/api/tags", cancellationToken);

                        if (!response.IsSuccessStatusCode)
                        {
                            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                            _logger.LogError("Failed to get model info: {StatusCode} - {ErrorContent}", response.StatusCode, errorContent);
                            return Result<EmbeddingModelInfo>.WithFailure(ErrorCodes.EmbeddingServiceError);
                        }

                        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };
                        var tagsResponse = JsonSerializer.Deserialize<OllamaTagsResponse>(responseContent, options);

                        var model = tagsResponse?.Models?.FirstOrDefault(m => m.Name.StartsWith(_options.EmbeddingModel));
                        if (model == null)
                        {
                            return Result<EmbeddingModelInfo>.WithFailure(ErrorCodes.ModelNotFound);
                        }

                        var modelInfo = new EmbeddingModelInfo(
                            ModelName: model.Name,
                            Version: model.ModifiedAt.ToString("yyyy-MM-dd"),
                            Dimension: 1536,
                            MaxTextLength: 8192,
                            SupportedLanguages: new[] { "en" } // Ollama models typically support English
                        );

                        return Result<EmbeddingModelInfo>.Success(modelInfo);
                    });
        }
        catch (OperationCanceledException)
        {
            return Cancelled<EmbeddingModelInfo>(ErrorCodes.OperationCancelled);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting model info");
            return WithFailure<EmbeddingModelInfo>(ErrorCodes.EmbeddingServiceError, ex);
        }
    }
}