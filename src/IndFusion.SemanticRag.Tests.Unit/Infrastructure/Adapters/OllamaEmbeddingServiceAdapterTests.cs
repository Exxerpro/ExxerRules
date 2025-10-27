using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IndFusion.SemanticRag.Domain.Models;
using IndFusion.SemanticRag.Infrastructure.Adapters;
using IndFusion.SemanticRag.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using Xunit;

namespace IndFusion.SemanticRag.Tests.Unit.Infrastructure.Adapters;

/// <summary>
/// Unit tests for the Ollama embedding service adapter.
/// </summary>
public class OllamaEmbeddingServiceAdapterTests
{
    private readonly ILogger<OllamaEmbeddingServiceAdapter> _logger;
    private readonly IOptions<OllamaOptions> _options;
    private readonly OllamaOptions _ollamaOptions;

    public OllamaEmbeddingServiceAdapterTests()
    {
        _logger = Substitute.For<ILogger<OllamaEmbeddingServiceAdapter>>();
        _ollamaOptions = new OllamaOptions
        {
            BaseUrl = "http://localhost:11434",
            Model = "nomic-embed-text",
            EmbeddingDimension = 768,
            MaxTextLength = 8192,
            MaxConcurrency = 5,
            TimeoutSeconds = 30
        };
        _options = Options.Create(_ollamaOptions);
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_WithValidText_ShouldReturnEmbedding()
    {
        // Arrange
        var text = "This is a test text";
        var expectedEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
        
        var httpClient = CreateMockHttpClient(expectedEmbedding);
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingAsync(text, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Length.ShouldBe(3);
        result.Value[0].ShouldBe(0.1f);
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_WithNullText_ShouldReturnFailure()
    {
        // Arrange
        var httpClient = new HttpClient();
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingAsync(null!, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("null or empty");
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_WithEmptyText_ShouldReturnFailure()
    {
        // Arrange
        var httpClient = new HttpClient();
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingAsync(string.Empty, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("null or empty");
    }

    [Fact]
    public async Task GenerateEmbeddingAsync_WithTextTooLong_ShouldReturnFailure()
    {
        // Arrange
        var httpClient = new HttpClient();
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);
        var longText = new string('a', _ollamaOptions.MaxTextLength + 1);

        // Act
        var result = await adapter.GenerateEmbeddingAsync(longText, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("exceeds maximum");
    }

    [Fact]
    public async Task GenerateEmbeddingsAsync_WithValidTexts_ShouldReturnEmbeddings()
    {
        // Arrange
        var texts = new List<string> { "Text 1", "Text 2", "Text 3" }.AsReadOnly();
        var expectedEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
        
        var httpClient = CreateMockHttpClient(expectedEmbedding);
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingsAsync(texts, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Count.ShouldBe(3);
    }

    [Fact]
    public async Task GenerateEmbeddingWithMetadataAsync_WithValidText_ShouldReturnVectorEmbedding()
    {
        // Arrange
        var text = "This is a test text";
        var metadata = new Dictionary<string, object> { { "source", "test" } };
        var expectedEmbedding = new float[] { 0.1f, 0.2f, 0.3f };
        
        var httpClient = CreateMockHttpClient(expectedEmbedding);
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var result = await adapter.GenerateEmbeddingWithMetadataAsync(text, metadata, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(text);
        result.Value.Embedding.Length.ShouldBe(3);
        result.Value.Metadata.ShouldContainKey("source");
    }

    [Fact]
    public void GetEmbeddingDimension_ShouldReturnConfiguredDimension()
    {
        // Arrange
        var httpClient = new HttpClient();
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var dimension = adapter.GetEmbeddingDimension();

        // Assert
        dimension.ShouldBe(_ollamaOptions.EmbeddingDimension);
    }

    [Fact]
    public void GetMaxTextLength_ShouldReturnConfiguredMaxLength()
    {
        // Arrange
        var httpClient = new HttpClient();
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);

        // Act
        var maxLength = adapter.GetMaxTextLength();

        // Assert
        maxLength.ShouldBe(_ollamaOptions.MaxTextLength);
    }

    [Fact]
    public void ValidateTextLength_WithValidText_ShouldReturnSuccess()
    {
        // Arrange
        var httpClient = new HttpClient();
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);
        var text = "Valid text";

        // Act
        var result = adapter.ValidateTextLength(text);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void ValidateTextLength_WithTextTooLong_ShouldReturnFailure()
    {
        // Arrange
        var httpClient = new HttpClient();
        var adapter = new OllamaEmbeddingServiceAdapter(httpClient, _logger, _options);
        var longText = new string('a', _ollamaOptions.MaxTextLength + 1);

        // Act
        var result = adapter.ValidateTextLength(longText);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldContain("exceeds maximum");
    }

    private HttpClient CreateMockHttpClient(float[] expectedEmbedding)
    {
        var response = new
        {
            embedding = expectedEmbedding
        };

        var json = JsonSerializer.Serialize(response);
        var handler = new MockHttpMessageHandler(json, HttpStatusCode.OK);
        return new HttpClient(handler);
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response)
            });
        }
    }
}

