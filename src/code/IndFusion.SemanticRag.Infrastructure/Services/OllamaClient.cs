using IndFusion.SemanticRag.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace IndFusion.SemanticRag.Infrastructure.Services;

/// <summary>
/// Client for interacting with Ollama LLM service.
/// </summary>
public class OllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaClient> _logger;
    private readonly OllamaOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaClient"/> class.
    /// </summary>
    /// <param name="httpClient">HTTP client.</param>
    /// <param name="options">Ollama configuration options.</param>
    /// <param name="logger">Logger instance.</param>
    public OllamaClient(
        HttpClient httpClient,
        IOptions<OllamaOptions> options,
        ILogger<OllamaClient> logger)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(options.Value.TimeoutSeconds);
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Generates embeddings for the given text.
    /// </summary>
    /// <param name="text">Text to generate embeddings for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Embedding vector.</returns>
    public async Task<float[]> GenerateEmbeddingAsync(
        string text, 
        CancellationToken cancellationToken = default)
    {
        const int maxRetries = 3;
        var retryCount = 0;

        while (retryCount < maxRetries)
        {
            try
            {
                _logger.LogDebug("Generating embedding for text using model: {Model} (attempt {Attempt})", 
                    _options.EmbeddingModel, retryCount + 1);

                var request = new
                {
                    model = _options.EmbeddingModel,
                    prompt = text
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/embeddings", content, cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Ollama API returned {StatusCode}: {Error}", 
                        response.StatusCode, errorContent);
                    
                    if (retryCount < maxRetries - 1)
                    {
                        retryCount++;
                        await Task.Delay(1000 * retryCount, cancellationToken);
                        continue;
                    }
                }

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponse>(responseContent);

                if (embeddingResponse?.Embedding == null || embeddingResponse.Embedding.Length == 0)
                {
                    throw new InvalidOperationException("Failed to generate embedding from Ollama - empty response");
                }

                _logger.LogDebug("Generated embedding with dimension: {Dimension}", embeddingResponse.Embedding.Length);

                return embeddingResponse.Embedding;
            }
            catch (HttpRequestException ex) when (retryCount < maxRetries - 1)
            {
                retryCount++;
                _logger.LogWarning(ex, "HTTP error generating embedding, retrying in {Delay}ms (attempt {Attempt})", 
                    1000 * retryCount, retryCount);
                await Task.Delay(1000 * retryCount, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating embedding for text");
                throw;
            }
        }

        throw new InvalidOperationException($"Failed to generate embedding after {maxRetries} attempts");
    }

    /// <summary>
    /// Generates text using the configured model.
    /// </summary>
    /// <param name="prompt">Text prompt.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Generated text.</returns>
    public async Task<string> GenerateTextAsync(
        string prompt, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Generating text using model: {Model}", _options.TextModel);

            var request = new
            {
                model = _options.TextModel,
                prompt = prompt,
                stream = false
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/generate", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var generateResponse = JsonSerializer.Deserialize<GenerateResponse>(responseContent);

            if (string.IsNullOrEmpty(generateResponse?.Response))
            {
                throw new InvalidOperationException("Failed to generate text from Ollama");
            }

            _logger.LogDebug("Generated text with length: {Length}", generateResponse.Response.Length);

            return generateResponse.Response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating text for prompt");
            throw;
        }
    }

    private class EmbeddingResponse
    {
        public float[]? Embedding { get; set; }
    }

    private class GenerateResponse
    {
        public string? Response { get; set; }
    }
}
