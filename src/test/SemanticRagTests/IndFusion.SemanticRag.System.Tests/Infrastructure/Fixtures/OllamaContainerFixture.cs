using System.Net.Http;
using System.Text.Json;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using IndFusion.SemanticRag.System.Tests.Infrastructure.Utilities;
using Testcontainers.Ollama;

namespace IndFusion.SemanticRag.System.Tests.Infrastructure.Fixtures;

/// <summary>
/// Ollama LLM service container fixture for SemanticRag system tests.
/// Implements xUnit v3 IAsyncLifetime pattern for proper container lifecycle management.
/// Provides containerized Ollama instance for embedding generation and LLM testing.
/// Handles Docker unavailability gracefully - when Docker is not available, IsAvailable is set to false
/// and tests should skip gracefully using SkipException.
/// </summary>
public sealed class OllamaContainerFixture : IAsyncLifetime
{
    private OllamaContainer? _container;
    private readonly ILogger<OllamaContainerFixture> _logger;

    /// <summary>
    /// Gets the hostname where the Ollama container is accessible.
    /// </summary>
    public string Hostname => _container?.Hostname ?? "localhost";

    /// <summary>
    /// Gets the mapped HTTP port for Ollama (default: 11434).
    /// </summary>
    public int HttpPort => _container?.GetMappedPublicPort(11434) ?? 11434;

    /// <summary>
    /// Gets the HTTP endpoint for Ollama.
    /// </summary>
    public string HttpEndpoint { get; private set; } = null!;

    /// <summary>
    /// Gets the Ollama configuration options with container endpoints.
    /// </summary>
    public OllamaOptions Options { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the Ollama container is running and ready.
    /// </summary>
    public bool IsAvailable => _container != null && !string.IsNullOrEmpty(HttpEndpoint);

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaContainerFixture"/> class.
    /// </summary>
    public OllamaContainerFixture()
    {
        _logger = XUnitLogger.CreateLogger<OllamaContainerFixture>();
    }

    /// <summary>
    /// Asynchronously initializes the Ollama container.
    /// Starts the container, pulls the required model, and creates the connection endpoints.
    /// Handles Docker unavailability gracefully by setting IsAvailable to false.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous initialization operation</returns>
    public async ValueTask InitializeAsync()
    {
        try
        {
            _logger.LogInformation("🚀 Initializing Ollama container for SemanticRag system tests...");

            _container = new OllamaBuilder()
                .WithImage("ollama/ollama:0.6.6")
                .WithAutoRemove(true)
                .WithCleanUp(true)
                .Build();

            _logger.LogInformation("⏳ Starting Ollama container...");
            await _container.StartAsync();

            HttpEndpoint = $"http://{Hostname}:{HttpPort}";

            Options = new OllamaOptions
            {
                BaseUrl = HttpEndpoint,
                EmbeddingModel = "nomic-embed-text",
                EmbeddingDimension = 384,
                MaxTextLength = 8192,
                MaxConcurrency = 5,
                TimeoutSeconds = 30
            };

            // Pull the required embedding model (this can take several minutes)
            _logger.LogInformation("📥 Pulling embedding model: {Model} (this may take several minutes)...", Options.EmbeddingModel);
            try
            {
                using var pullCts = new CancellationTokenSource(TimeSpan.FromMinutes(10)); // Give model pull up to 10 minutes
                var execResult = await _container.ExecAsync(["ollama", "pull", Options.EmbeddingModel], pullCts.Token);
                
                if (execResult.ExitCode == 0)
                {
                    _logger.LogInformation("✅ Model {Model} pulled successfully", Options.EmbeddingModel);
                    
                    // Verify model is available by checking if it's in the list
                    await WaitForModelReadyAsync(Options.EmbeddingModel, TimeSpan.FromSeconds(30));
                }
                else
                {
                    _logger.LogWarning("⚠️ Model pull returned exit code {ExitCode}, checking if model is already available...", execResult.ExitCode);
                    // Check if model is already available
                    await WaitForModelReadyAsync(Options.EmbeddingModel, TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception modelEx)
            {
                // Model might already be pulled, or there might be a network issue
                _logger.LogWarning(modelEx, "⚠️ Error pulling model {Model}, checking if it's already available...", Options.EmbeddingModel);
                // Try to verify model is available anyway
                try
                {
                    await WaitForModelReadyAsync(Options.EmbeddingModel, TimeSpan.FromSeconds(10));
                }
                catch (Exception verifyEx)
                {
                    _logger.LogError(verifyEx, "❌ Model {Model} is not available - tests may fail", Options.EmbeddingModel);
                    // Continue anyway - tests will fail if model is actually needed
                }
            }

            _logger.LogInformation("✅ Ollama container started - HTTP: {HttpEndpoint}", HttpEndpoint);
        }
        catch (Exception ex) when (ex.Message.Contains("Docker", StringComparison.OrdinalIgnoreCase) ||
                                  ex.Message.Contains("docker", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning(ex, "⚠️ Docker-related error - Ollama container will not be started");
            DockerSkipConditions.ShouldSkipDockerTests = true;

            HttpEndpoint = "http://localhost:11434";
            Options = new OllamaOptions
            {
                BaseUrl = HttpEndpoint,
                EmbeddingModel = "nomic-embed-text",
                EmbeddingDimension = 384,
                MaxTextLength = 8192,
                MaxConcurrency = 5,
                TimeoutSeconds = 30
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to initialize Ollama container");
            throw;
        }
    }

    /// <summary>
    /// Asynchronously disposes of the Ollama container resources.
    /// Ensures proper cleanup of Docker containers.
    /// Explicitly stops containers before disposal to prevent resource leaks.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous disposal operation</returns>
    public async ValueTask DisposeAsync()
    {
        try
        {
            _logger.LogInformation("🧹 Cleaning up Ollama container...");

            // Explicitly stop and dispose container
            if (_container != null)
            {
                try
                {
                    // Stop the container first (ensures proper cleanup)
                    await _container.StopAsync();
                    _logger.LogInformation("✅ Ollama container stopped");
                }
                catch (Exception stopEx)
                {
                    _logger.LogWarning(stopEx, "⚠️ Error stopping Ollama container (non-fatal, will attempt disposal)");
                }

                try
                {
                    // Dispose the container (removes it)
                    await _container.DisposeAsync();
                    _logger.LogInformation("✅ Ollama container disposed");
                }
                catch (Exception disposeEx)
                {
                    _logger.LogWarning(disposeEx, "⚠️ Error disposing Ollama container (non-fatal)");
                }

                _container = null;
            }

            _logger.LogInformation("✅ Ollama container cleanup completed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "⚠️ Error during Ollama container cleanup (non-fatal)");
        }
    }

    /// <summary>
    /// Waits for the specified model to be available in the Ollama container.
    /// Polls the Ollama API to check if the model is in the list of available models.
    /// </summary>
    /// <param name="modelName">Name of the model to wait for.</param>
    /// <param name="timeout">Maximum time to wait for the model to become available.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task WaitForModelReadyAsync(string modelName, TimeSpan timeout)
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5)
        };

        var startTime = DateTime.UtcNow;
        var pollInterval = TimeSpan.FromSeconds(2);
        var maxAttempts = (int)(timeout.TotalSeconds / pollInterval.TotalSeconds);

        _logger.LogInformation("⏳ Waiting for model {Model} to be available (timeout: {Timeout}s)...", modelName, timeout.TotalSeconds);

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            try
            {
                var response = await httpClient.GetAsync($"{HttpEndpoint}/api/tags");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var tagsResponse = JsonSerializer.Deserialize<OllamaTagsResponse>(content);
                    
                    if (tagsResponse?.Models != null)
                    {
                        var model = tagsResponse.Models.FirstOrDefault(m => 
                            m.Name.Equals(modelName, StringComparison.OrdinalIgnoreCase) ||
                            m.Name.StartsWith(modelName, StringComparison.OrdinalIgnoreCase));
                        
                        if (model != null)
                        {
                            _logger.LogInformation("✅ Model {Model} is now available", modelName);
                            return;
                        }
                    }
                }

                if (DateTime.UtcNow - startTime > timeout)
                {
                    throw new TimeoutException($"Model {modelName} did not become available within {timeout.TotalSeconds} seconds");
                }

                await Task.Delay(pollInterval);
            }
            catch (HttpRequestException ex)
            {
                // API might not be ready yet, continue polling
                _logger.LogDebug("Ollama API not ready yet (attempt {Attempt}/{MaxAttempts}): {Error}", 
                    attempt + 1, maxAttempts, ex.Message);
                
                if (DateTime.UtcNow - startTime > timeout)
                {
                    throw new TimeoutException($"Ollama API did not become available within {timeout.TotalSeconds} seconds", ex);
                }

                await Task.Delay(pollInterval);
            }
        }

        throw new TimeoutException($"Model {modelName} did not become available within {timeout.TotalSeconds} seconds");
    }

    /// <summary>
    /// Response model for Ollama tags API.
    /// </summary>
    private record OllamaTagsResponse(OllamaModel[]? Models);

    /// <summary>
    /// Model information from Ollama tags API.
    /// </summary>
    private record OllamaModel(string Name, DateTime ModifiedAt, long Size);
}

